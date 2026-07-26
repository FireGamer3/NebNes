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
                //reset length counters
            }
            if(enableNoise() == 0) {
                //reset length counter
            }
            if(enableDMC() == 0) {
                //stop DPCM
            }else { // add if for remaining bytes
                //start if there are bytes ready
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
