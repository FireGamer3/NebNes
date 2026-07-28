using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class APUControl : BaseRegister {
        public APUControl(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            if (enablePulse1() == 0)
                apu.pulses[0].lengthCounter.reset();
            if(enablePulse2() == 0)
                apu.pulses[1].lengthCounter.reset();
            if(enableTriangle() == 0) {
                apu.triangle.lengthCounter.reset();
                apu.triangle.linearLengthCounter.fullReset();
            }
            if(enableNoise() == 0) {
                apu.noise.lengthCounter.reset();
            }
            if(enableDMC() == 0) {
                apu.dmc.dpcm.stop();
            }else if (apu.dmc.dpcm.remainingBytes() == 0) {
                apu.dmc.dpcm.start();
            }
        }

        public byte enablePulse1() {
            return ByteLib.getBit(value, 0);
        }

        public byte enablePulse2() {
            return ByteLib.getBit(value, 1);
        }

        public byte enableTriangle() {
            return ByteLib.getBit(value, 2);
        }

        public byte enableNoise() {
            return ByteLib.getBit(value, 3);
        }

        public byte enableDMC() {
            return ByteLib.getBit(value, 4);
        }
    }
}
