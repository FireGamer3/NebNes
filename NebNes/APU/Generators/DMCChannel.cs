using NebNes.APU.Registers;
using NebNes.APU.Sampling;
using NebNes.CPU;

namespace NebNes.APU.Generators {
    public class DMCChannel {
        public MOS6502 cpu;
        public DPCM dpcm;
        NesAPU apu;
        private double outputSample = 0;

        public DMCChannel(MOS6502 cpu, NesAPU apu) {
            this.cpu = cpu;
            this.apu = apu;
            dpcm = new DPCM(this);
        }

        public void set(double sample) {
            outputSample = sample;
        }

        public double sample() {
            return outputSample;
        }

        public void step() {
            dpcm.update();
        }

        public DMCRegisters registers() {
            return apu.registers.dmc;
        }
    }
}
