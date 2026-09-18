using Silk.NET.SDL;

namespace NebNes.Frontend {
    internal sealed unsafe class AudioOutput : IDisposable {
        public const int SampleRate = 44100;
        public const int Channels = 1;

        private const int MaxBufferedSamples = SampleRate / 4;

        private readonly Sdl _sdl;
        private readonly uint _device;

        // ~50 ms of cushion. With audio as the master clock the producer refills this almost
        // instantly, so a deeper target just buys jitter tolerance (GC pauses, Sleep granularity)
        // at the cost of latency. Lower it if you want tighter latency and don't hear underruns.
        public const int TargetBufferedSamples = SampleRate / 20;

        public AudioOutput() {
            SdlHost.Init(SdlHost.InitAudio);
            _sdl = SdlHost.Api;

            var desired = new AudioSpec {
                Freq = SampleRate,
                Format = Sdl.AudioF32,
                Channels = Channels,
                Samples = 1024,
                Callback = default,
            };

            AudioSpec obtained = default;

            // allowed_changes = 0: SDL must hand back exactly what we asked for (converting
            // internally if the hardware disagrees), so `obtained` should mirror `desired`.
            // Log it anyway — a mismatch here is the first thing to check when audio sounds wrong.
            _device = _sdl.OpenAudioDevice((byte*)null, 0, &desired, &obtained, 0);
            if (_device == 0) {
                SdlHost.QuitSubSystem(SdlHost.InitAudio);
                throw new InvalidOperationException($"SDL_OpenAudioDevice failed: {_sdl.GetErrorS()}");
            }

            Console.WriteLine($"Audio: {obtained.Freq} Hz, {obtained.Channels} ch, " +
                              $"format 0x{obtained.Format:X4}, {obtained.Samples} sample buffer");

            _sdl.PauseAudioDevice(_device, 0);
        }

        public int QueuedSamples => (int)(_sdl.GetQueuedAudioSize(_device) / sizeof(float));

        public int SamplesNeeded() => Math.Max(0, TargetBufferedSamples - QueuedSamples);

        public void Queue(float[] samples) => Queue(samples, samples.Length);

        public void Queue(float[] samples, int count) {
            ArgumentNullException.ThrowIfNull(samples);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count, samples.Length);
            if (count <= 0) return;

            // Backstop only: SamplesNeeded() caps generation well below this, so reaching it
            // means the producer bypassed the pacing and the excess is better dropped than queued.
            if (QueuedSamples > MaxBufferedSamples) return;

            fixed (float* p = samples) {
                _sdl.QueueAudio(_device, p, (uint)(count * sizeof(float)));
            }
        }

        public void Dispose() {
            if (_device != 0) {
                _sdl.PauseAudioDevice(_device, 1);
                _sdl.ClearQueuedAudio(_device);
                _sdl.CloseAudioDevice(_device);
            }
            SdlHost.QuitSubSystem(SdlHost.InitAudio);
        }
    }
}
