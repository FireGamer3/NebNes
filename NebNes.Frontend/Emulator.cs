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
            _nes?.Dispose();
            _nes = new NES(this, data);
            _romData = data;
            StartTrace();
        }

        /// <summary>Power-cycles the console by rebuilding it from the loaded ROM. No-op if nothing is loaded.</summary>
        public void Reset() {
            if (_romData is null) return;
            _nes?.Dispose();
            _nes = new NES(this, _romData);
            StartTrace();
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

            _frameCount++;
            double elapsed = _fpsClock.Elapsed.TotalSeconds;
            if (elapsed >= 1.0) {
                FpsUpdated?.Invoke(_frameCount / elapsed);
                _frameCount = 0;
                _fpsClock.Restart();
            }
        }

        public void PushAudio(float[] samples) {
            _audio.Queue(samples);
        }

        public void PushAudio(float[] samples, int count) {
            _audio.Queue(samples, count);
        }

        public void SetButton(int player, ButtonKey key, bool pressed) {
            _nes?.SetControllerButtonState(player, key, pressed);
        }
    }
}
