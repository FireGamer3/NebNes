using System.Diagnostics;
using NebNes.CPU.Interfaces;
using NebNes.Enums;

namespace NebNes.Frontend {
    internal sealed class Emulator : IEmulatorHost {
        private readonly Display _display;
        private readonly AudioOutput _audio;
        private NES? _nes;
        private byte[]? _romData;

        private readonly Stopwatch _fpsClock = Stopwatch.StartNew();
        private int _frameCount;

        /// <summary>Raised roughly once per second with the measured frames-per-second.</summary>
        public Action<double>? FpsUpdated;

        public Emulator(Display display, AudioOutput audio) {
            _display = display;
            _audio = audio;
        }

        public void LoadRom(string path) {
            byte[] data = File.ReadAllBytes(path);
            _nes = new NES(this, data);
            _romData = data;
        }

        /// <summary>Power-cycles the console by rebuilding it from the loaded ROM. No-op if nothing is loaded.</summary>
        public void Reset() {
            if (_romData is not null)
                _nes = new NES(this, _romData);
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
