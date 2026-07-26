using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class NoiseForm : BaseRegister {
        public NoiseForm(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }

        public byte periodId() {
            return ByteLib.getBits(value, 0, 4);
        }

        public byte mode() {
            return ByteLib.getBit(value, 7);
        }
    }
}
