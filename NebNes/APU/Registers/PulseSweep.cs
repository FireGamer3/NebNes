using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class PulseSweep : BaseRegister {
        public PulseSweep(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.pulses[id].frequencySweep.start();
        }

        public byte shiftCount() {
            return ByteLib.getBits(value, 0, 3);
        }

        public byte negateFlag() {
            return ByteLib.getBit(value, 3);
        }

        public byte dividerPeriodMinusOne() {
            return ByteLib.getBits(value, 4, 3);
        }

        public byte enabledFlag() {
            return ByteLib.getBit(value, 7);
        }
    }
}
