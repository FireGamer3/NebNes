using NebNes.Misc;

namespace NebNes.PPU.Registers {
    public class PPUCtrl : BaseRegister {
        public PPUCtrl(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte val) {
            set(val);
            ppu.loopy.onPPUCtrlWrite(val);
        }

        public byte vramAddressIncrement32() {
            return ByteLib.getBit(value, 2);
        }

        public byte sprite8x8PatternTableId() {
            return ByteLib.getBit(value, 3);
        }

        public byte backgroundPatternTableId() {
            return ByteLib.getBit(value, 4);
        }

        public byte spriteSize() {
            return ByteLib.getBit(value, 5);
        }

        public byte generateNMIOnVBlank() {
            return ByteLib.getBit(value, 7);
        }
    }
}
