using System.Diagnostics;
using NebNes.Enums;
using Silk.NET.SDL;
// Silk.NET.SDL exports its own `Thread`, so the BCL one needs an unambiguous name.
using Thread = System.Threading.Thread;

namespace NebNes.Frontend {
    internal static class Program {
        // Roughly one emulated second of instructions; enough to catch boot-time divergence
        // without filling the disk. Override with --trace-lines (0 for unlimited).
        private const long DefaultTraceLines = 2_000_000;

        private const string Usage =
            "Usage: NebNes.Frontend <rom.nes> [--scale N] [--nestest] " +
            "[--trace [--trace-out FILE] [--trace-lines N]]";

        // Player 1 keyboard layout: arrows = D-pad, Z/X = B/A, Enter = Start, RShift = Select.
        private static readonly Dictionary<Scancode, ButtonKey> KeyMap = new() {
            [Scancode.ScancodeUp]     = ButtonKey.BUTTON_UP,
            [Scancode.ScancodeDown]   = ButtonKey.BUTTON_DOWN,
            [Scancode.ScancodeLeft]   = ButtonKey.BUTTON_LEFT,
            [Scancode.ScancodeRight]  = ButtonKey.BUTTON_RIGHT,
            [Scancode.ScancodeZ]      = ButtonKey.BUTTON_B,
            [Scancode.ScancodeX]      = ButtonKey.BUTTON_A,
            [Scancode.ScancodeReturn] = ButtonKey.BUTTON_START,
            [Scancode.ScancodeRshift] = ButtonKey.BUTTON_SELECT,
        };

        private static int Main(string[] args) {
            string? romPath = null;
            int? scaleOverride = null;
            bool nestest = false;
            bool trace = false;
            string tracePath = "cpu-trace.log";
            long traceLines = DefaultTraceLines;

            for (int i = 0; i < args.Length; i++) {
                if (args[i] is "--scale" or "-s") {
                    string? v = TakeValue(args, ref i);
                    if (v is null || !int.TryParse(v, out int s) || s <= 0)
                        return Fail($"--scale needs a positive integer (got {v ?? "nothing"}).");
                    scaleOverride = s;
                } else if (args[i] is "--nestest" or "-t") {
                    nestest = true;
                } else if (args[i] is "--trace" or "-l") {
                    trace = true;
                } else if (args[i] is "--trace-out") {
                    string? outPath = TakeValue(args, ref i);
                    if (outPath is null) return Fail("--trace-out needs a file path.");
                    tracePath = outPath;
                    trace = true;
                } else if (args[i] is "--trace-lines") {
                    // 0 = unlimited. A trace runs ~1.8M lines per emulated second, so cap it.
                    string? v = TakeValue(args, ref i);
                    if (v is null || !long.TryParse(v, out long n) || n < 0)
                        return Fail($"--trace-lines needs a non-negative integer (got {v ?? "nothing"}).");
                    traceLines = n;
                    trace = true;
                } else if (args[i].StartsWith('-')) {
                    return Fail($"Unrecognized option {args[i]}.");
                } else if (romPath is not null) {
                    return Fail($"More than one ROM given ({romPath} and {args[i]}).");
                } else {
                    romPath = args[i];
                }
            }

            if (nestest) {
                try {
                    return Nestest.Run(romPath ?? "./testRoms/NEStest.nes", "./testRoms/nestest.log");
                } catch (Exception ex) {
                    // A missing ROM or unwritable trace path is a setup failure (2), not a CPU failure (1).
                    Console.Error.WriteLine($"nestest: could not run: {ex.Message}");
                    return 2;
                }
            }

            try {
                return RunEmulator(romPath, scaleOverride, trace, tracePath, traceLines);
            } catch (Exception ex) {
                // Without this an emulation-core throw kills the process mid-unwind, so the
                // `using`s below never run and the message races the exit.
                Console.Error.WriteLine("Fatal error:");
                Console.Error.WriteLine(ex);
                return 1;
            } finally {
                SdlHost.Shutdown();
            }
        }

        /// <summary>Consumes and returns an option's value argument, or null if it's missing.</summary>
        private static string? TakeValue(string[] args, ref int i) =>
            i + 1 < args.Length ? args[++i] : null;

        private static int Fail(string message) {
            Console.Error.WriteLine(message);
            Console.Error.WriteLine(Usage);
            return 2;
        }

        private static int RunEmulator(string? romPath, int? scaleOverride,
                                       bool trace, string tracePath, long traceLines) {
            int scale = scaleOverride ?? Display.SuggestScale();
            string baseTitle = $"NebNes  [{scale}x]";

            using var display = new Display(baseTitle, scale);
            using var audio = new AudioOutput();
            using var emulator = new Emulator(display, audio);
            if (trace) emulator.EnableTrace(tracePath, traceLines);

            string titleBase = baseTitle;
            double lastFps = 0;
            void RefreshTitle() =>
                display.SetTitle(lastFps > 0 ? $"{titleBase}  -  {lastFps:0} FPS" : titleBase);
            emulator.FpsUpdated = fps => { lastFps = fps; RefreshTitle(); };

            void TryLoad(string path) {
                try {
                    emulator.LoadRom(path);
                    titleBase = $"{baseTitle}  -  {Path.GetFileName(path)}";
                    RefreshTitle();
                    Console.WriteLine($"Loaded ROM: {path}");
                } catch (Exception ex) {
                    Console.WriteLine($"Failed to load ROM {path}: {ex.Message}");
                    Console.Error.WriteLine(ex);
                }
            }

            display.KeyChanged = (scancode, pressed) => {
                if (KeyMap.TryGetValue(scancode, out ButtonKey button))
                    emulator.SetButton(0, button, pressed);
            };
            display.FileDropped = TryLoad;
            display.OpenRomRequested = () => {
                try {
                    // Parented to the emulator window so it can't open behind it.
                    string? picked = FileDialog.OpenRom(display.NativeHandle);
                    if (picked is not null) TryLoad(picked);
                } catch (Exception ex) {
                    Console.WriteLine($"Could not open the file dialog: {ex.Message}");
                }
            };
            display.ResetRequested = () => {
                if (emulator.IsRomLoaded) {
                    emulator.Reset();
                    Console.WriteLine("Reset.");
                }
            };
            display.TraceRequested = () => emulator.RecordTrace(5);

            if (romPath is not null) {
                TryLoad(romPath);
            } else {
                Console.WriteLine("No ROM given.");
                Console.WriteLine(Usage);
                Console.WriteLine("Or drop a .nes file onto the window, or press Ctrl+O to browse.");
            }

            Console.WriteLine($"Window: {Display.NesWidth * scale}x{Display.NesHeight * scale}  " +
                              $"(integer scaling, {scale}x). Ctrl+O load, R reset, T record 5s CPU trace, Esc/close to quit.");

            var framebuffer = new uint[Display.NesWidth * Display.NesHeight];

            // With no ROM there's no audio clock to pace against, and vsync is off by design,
            // so the idle screen has to throttle itself or it spins a core flat out.
            int refresh = display.RefreshRate;
            double idleFrameMs = 1000.0 / (refresh > 0 ? refresh : 60);
            var idleClock = Stopwatch.StartNew();
            double nextIdleFrame = 0;

            while (display.PumpEvents()) {
                if (emulator.IsRomLoaded) {
                    int needed = audio.SamplesNeeded();
                    if (needed > 0) {
                        emulator.RunAudioSync(needed);
                    } else {
                        // Audio buffer is full; yield instead of spinning a core at 100%.
                        Thread.Sleep(1);
                    }
                } else {
                    double now = idleClock.Elapsed.TotalMilliseconds;
                    if (now >= nextIdleFrame) {
                        TvStatic.Render(framebuffer);
                        emulator.PresentFrame(framebuffer);
                        nextIdleFrame = now + idleFrameMs;
                    } else {
                        Thread.Sleep(1);
                    }
                }
            }

            return 0;
        }
    }
}
