using NebNes.APU;
using NebNes.CPU;
using NebNes.CPU.Interfaces;
using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Mappers.Misc;
using NebNes.Misc;
using NebNes.PPU;
using System.Numerics;

namespace NebNes {
    public class NES : IDisposable {
        IEmulatorHost host;
        Bus bus;
        MOS6502 cpu;
        Cart cart;
        IMapper mapper;
        Controller[] controllers = {new Controller(), new Controller()};
        NesPPU ppu;
        NesAPU apu;
        CpuTracer? tracer;

        private const int AudioChunkSize = 256;
        private readonly float[] audioChunk = new float[AudioChunkSize];
        private int chunkFill = 0;
        private int samplesRemaining = 0;

        //private int apuCycleCarry = 0;
        private bool apuStep = false;

        private readonly Action<float> onSample;
        private readonly Action<uint[]> onFrame;

        public NES(IEmulatorHost host, byte[] rom) {
            this.host = host;
            onSample = OnSample;
            onFrame = host.PresentFrame;
            bus = new Bus();
            cpu = new MOS6502(bus);
            cart = new Cart(rom);
            controllers[0].SetOther(controllers[1]);
            controllers[1].SetOther(controllers[0]);
            if (cart.isRomInvalid()) throw new Exception("ROM IS NOT A VALID iNES 1.0 HEADER");
            Console.WriteLine("Mapper: " + cart.getMapperID());
            mapper = MapperFactory.CreateMapper(cpu, cart);
            ppu = new NesPPU(cpu, bus, mapper);
            apu = new NesAPU(cpu);
            ppu.bus.Initialize(cart, mapper);
            bus.Initialize(mapper, ppu, apu, controllers);
            cpu.triggerInterrupt(InterruptIndex.RESET);
        }

        /// <summary>
        /// Starts a nestest-format CPU trace at <paramref name="path"/>, beginning with the first
        /// instruction after RESET. Debug aid only — it costs a formatted line per instruction.
        /// </summary>
        public void StartTrace(string path, long maxLines = CpuTracer.Unlimited) {
            StopTrace();
            tracer = CpuTracer.ToFile(cpu, bus, path, maxLines);
            tracer.Attach();
        }

        public void StopTrace() {
            tracer?.Dispose();
            tracer = null;
        }

        public long TracedInstructions => tracer?.LinesWritten ?? 0;

        public void Dispose() {
            StopTrace();
        }

        public void stepSamples(int n) {
            if (n <= 0) return;
            samplesRemaining = n;
            while (samplesRemaining > 0) {
                step();
            }
            if (chunkFill > 0) {
                host.PushAudio(audioChunk, chunkFill);
                chunkFill = 0;
            }
        }

        public void SetControllerButtonState(int index, ButtonKey key, bool state) {
            controllers[index].Update(key, state);
        }

        private void step() {
            cpu.step();
            ppu.step(onFrame);
            ppu.step(onFrame);
            ppu.step(onFrame);
            if(apuStep) apu.step(onSample);
            apuStep = !apuStep;
        }

        private void OnSample(float sample) {
            if (samplesRemaining <= 0) return;
            audioChunk[chunkFill++] = sample;
            samplesRemaining--;
            if (chunkFill == AudioChunkSize) {
                host.PushAudio(audioChunk, chunkFill);
                chunkFill = 0;
            }
        }
    }
}
