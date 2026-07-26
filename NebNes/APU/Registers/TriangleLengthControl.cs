using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class TriangleLengthControl : BaseRegister {
        public TriangleLengthControl(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.triangle.linearLengthCounter.reload = linearCounterReload();
        }

        public byte halt() {
            return ByteLib.getBit(value, 7);
        }

        public byte linearCounterReload() {
            return ByteLib.getBits(value, 0, 7);
        }
    }
}
