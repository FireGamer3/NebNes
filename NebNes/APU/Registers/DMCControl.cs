using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class DMCControl : BaseRegister {
        public DMCControl(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }

        public byte volumeOrEnvelopePeriod() {
            return ByteLib.getBits(value, 0, 4);
        }

        public byte loop() {
            return ByteLib.getBit(value, 6);
        }
    }
}
