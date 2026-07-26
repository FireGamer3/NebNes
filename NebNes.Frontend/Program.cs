using NebNes.Enums;
using Silk.NET.SDL;

namespace NebNes.Frontend {
    internal static class Program {
        // Roughly one emulated second of instructions; enough to catch boot-time divergence
        // without filling the disk. Override with --trace-lines (0 for unlimited).
        private const long DefaultTraceLines = 2_000_000;

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
        private static void Main(string[] args) {
            string? romPath = null;
            int? scaleOverride = null;
            bool nestest = false;
            bool trace = false;
            string tracePath = "cpu-trace.log";
            long traceLines = DefaultTraceLines;

            for (int i = 0; i < args.Length; i++) {
                if (args[i] is "--scale" or "-s") {
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int s) && s > 0)
                        scaleOverride = s;
                } else if (args[i] is "--nestest" or "-t") {
                    nestest = true;
                } else if (args[i] is "--trace" or "-l") {
                    trace = true;
                } else if (args[i] is "--trace-out") {
                    if (i + 1 < args.Length) { tracePath = args[++i]; trace = true; }
                } else if (args[i] is "--trace-lines") {
                    // 0 = unlimited. A trace runs ~1.8M lines per emulated second, so cap it.
                    if (i + 1 < args.Length && long.TryParse(args[++i], out long n) && n >= 0) {
                        traceLines = n;
                        trace = true;
                    }
                } else if (!args[i].StartsWith('-')) {
                    romPath = args[i];
                }
            }

            if (nestest) {
                string nes = romPath ?? "./testRoms/NEStest.nes";
                Environment.Exit(Nestest.Run(nes, "./testRoms/nestest.log"));
            }

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
                    Console.WriteLine($"Failed to load ROM '{path}': {ex.Message}");
                    Console.Error.WriteLine(ex);
                }
            }

            display.KeyChanged = (scancode, pressed) => {
                if (KeyMap.TryGetValue(scancode, out ButtonKey button))
                    emulator.SetButton(0, button, pressed);
            };
            display.FileDropped = TryLoad;
            display.OpenRomRequested = () => {
                string? picked = FileDialog.OpenRom(IntPtr.Zero);
                if (picked is not null) TryLoad(picked);
            };
            display.ResetRequested = () => {
                if (emulator.IsRomLoaded) {
                    emulator.Reset();
                    Console.WriteLine("Reset.");
                }
            };

            if (romPath is not null) {
                TryLoad(romPath);
            } else {
                Console.WriteLine("No ROM given.");
                Console.WriteLine("Usage: NebNes.Frontend <rom.nes> [--scale N] [--trace [--trace-out FILE] [--trace-lines N]]");
                Console.WriteLine("Or drop a .nes file onto the window, or press Ctrl+O to browse.");
            }

            Console.WriteLine($"Window: {Display.NesWidth * scale}x{Display.NesHeight * scale}  " +
                              $"(integer scaling, {scale}x). Ctrl+O load, R reset, Esc/close to quit.");

            var framebuffer = new uint[Display.NesWidth * Display.NesHeight];

            while (display.PumpEvents()) {
                if (emulator.IsRomLoaded) {
                    int needed = audio.SamplesNeeded();
                    if (needed > 0) {
                        emulator.RunAudioSync(needed);
                    } else {
                        // Audio buffer is full; yield instead of spinning a core at 100%.
                        System.Threading.Thread.Sleep(1);
                    }
                } else {
                    TvStatic.Render(framebuffer);
                    emulator.PresentFrame(framebuffer);
                }
            }
        }
    }
}
