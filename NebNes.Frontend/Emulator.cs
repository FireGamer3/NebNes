using System.Diagnostics;
using NebNes.CPU.Interfaces;
using NebNes.Enums;

namespace NebNes.Frontend {
    internal sealed class Emulator : IEmulatorHost, IDisposable {
        private readonly Display _display;
        private readonly AudioOutput _audio;
        private NES? _nes;
        private byte[]? _romData;
        private string? _tracePath;
        private long _traceLines;
        // Frames left in a keypress-triggered trace; 0 when none is running.
        private int _timedTraceFrames;
        private string? _timedTracePath;

        private readonly Stopwatch _fpsClock = Stopwatch.StartNew();
        private int _frameCount;

        /// <summary>Raised roughly once per second with the measured frames-per-second.</summary>
        public Action<double>? FpsUpdated;

        public Emulator(Display display, AudioOutput audio) {
            _display = display;
            _audio = audio;
        }

        /// <summary>
        /// Arms CPU instruction tracing for every console built from here on (including after a
        /// reset). Each new console truncates the file, so the log always covers the current run.
        /// </summary>
        public void EnableTrace(string path, long maxLines) {
            _tracePath = path;
            _traceLines = maxLines;
            StartTrace();
        }

        public void LoadRom(string path) {
            byte[] data = File.ReadAllBytes(path);
            Swap(new NES(this, data), data);
        }

        /// <summary>Power-cycles the console by rebuilding it from the loaded ROM. No-op if nothing is loaded.</summary>
        public void Reset() {
            if (_romData is null) return;
            Swap(new NES(this, _romData), _romData);
        }

        /// <summary>
        /// Installs an already-constructed console. The new one is built before the old one is
        /// torn down, so a throwing constructor (bad header, unsupported mapper) leaves the
        /// currently-running console untouched instead of stranding a disposed one in place.
        /// </summary>
        private void Swap(NES next, byte[] romData) {
            // Disposing the old console closes its tracer, so a timed trace ends with it.
            if (_timedTraceFrames > 0) FinishTimedTrace();
            _nes?.Dispose();
            _nes = next;
            _romData = romData;
            StartTrace();
        }

        /// <summary>
        /// Records the next <paramref name="seconds"/> of emulated time (60 frames/s) as a CPU
        /// trace to a timestamped file in the working directory. Ignored if a trace is already
        /// running. A crash mid-recording still leaves the tail on disk, since the tracer flushes
        /// as it goes and is closed on dispose.
        /// </summary>
        public void RecordTrace(double seconds) {
            if (_nes is null) {
                Console.WriteLine("No ROM loaded; nothing to trace.");
                return;
            }
            if (_tracePath is not null) {
                Console.WriteLine("A --trace is already running; ignoring T.");
                return;
            }
            if (_timedTraceFrames > 0) {
                Console.WriteLine($"Already recording ({_timedTraceFrames} frames left).");
                return;
            }
            _timedTraceFrames = (int)Math.Ceiling(seconds * 60);
            _timedTracePath = $"cpu-trace-{DateTime.Now:yyyyMMdd-HHmmss}.log";
            _nes.StartTrace(_timedTracePath);
            Console.WriteLine($"Recording {seconds:0.#}s of CPU trace -> {Path.GetFullPath(_timedTracePath)}");
        }

        private void FinishTimedTrace() {
            long lines = _nes?.TracedInstructions ?? 0;
            _nes?.StopTrace();
            _timedTraceFrames = 0;
            Console.WriteLine($"Trace finished: {lines:N0} instructions -> {Path.GetFullPath(_timedTracePath!)}");
            _timedTracePath = null;
        }

        private void StartTrace() {
            if (_tracePath is null || _nes is null) return;
            _nes.StartTrace(_tracePath, _traceLines);
            Console.WriteLine($"CPU trace -> {Path.GetFullPath(_tracePath)}" +
                              (_traceLines > 0 ? $" (first {_traceLines:N0} instructions)" : " (unlimited)"));
        }

        public void Dispose() {
            _nes?.Dispose();
            _nes = null;
        }

        public bool IsRomLoaded => _nes is not null;

        public void RunAudioSync(int needed) {
            _nes!.stepSamples(needed);
        }

        public void PresentFrame(uint[] framebuffer) {
            _display.UpdateFrame(framebuffer);

            if (_timedTraceFrames > 0 && --_timedTraceFrames == 0) FinishTimedTrace();

            _frameCount++;
            double elapsed = _fpsClock.Elapsed.TotalSeconds;
            if (elapsed >= 1.0) {
                FpsUpdated?.Invoke(_frameCount / elapsed);
                _frameCount = 0;
                _fpsClock.Restart();
            }
        }

        public void PushAudio(float[] samples, int count) {
            _audio.Queue(samples, count);
        }

        public void SetButton(int player, ButtonKey key, bool pressed) {
            ArgumentOutOfRangeException.ThrowIfNegative(player);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(player, 2);
            _nes?.SetControllerButtonState(player, key, pressed);
        }
    }
}
