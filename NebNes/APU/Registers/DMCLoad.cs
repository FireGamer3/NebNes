using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class DMCLoad : BaseRegister {
        public DMCLoad(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            //apu.dmc.outputSample = directLoad();
        }

        public byte directLoad() {
            return ByteLib.getBits(value, 0, 7);
        }
    }
}
