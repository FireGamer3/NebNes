using NebNes.APU;
using NebNes.CPU;
using NebNes.CPU.Interfaces;
using NebNes.Enums;
using NebNes.Mappers;
using NebNes.Misc;
using NebNes.PPU;
using System.Numerics;

namespace NebNes {
    public class NES {
        IEmulatorHost host;
        Bus bus;
        MOS6502 cpu;
        Cart cart;
        IMapper mapper;
        Controller[] controllers = {new Controller(), new Controller()};
        NesPPU ppu;
        NesAPU apu;

        private const int AudioChunkSize = 256;
        private readonly float[] audioChunk = new float[AudioChunkSize];
        private int chunkFill = 0;
        private int samplesRemaining = 0;

        // The APU is clocked at half the CPU rate. waitCycles/2 truncates the odd cycle each
        // instruction, so carry the leftover into the next one to keep an exact 2:1 CPU:APU ratio.
        private int apuCycleCarry = 0;

        // Cached delegates so the hot step loop doesn't allocate one per instruction.
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
            int waitCycles = cpu.step();
            for (int i = 0; i < waitCycles; i++) {
                cpu.step();
            }
            int ppuCycles = waitCycles * 3;
            for (int i = 0; i < ppuCycles; i++) {
                ppu.step(onFrame);
            }

            apuCycleCarry += waitCycles;
            int apuCycles = apuCycleCarry / 2;
            apuCycleCarry -= apuCycles * 2;
            for (int i = 0; i < apuCycles; i++) {
                apu.step(onSample);
            }
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
