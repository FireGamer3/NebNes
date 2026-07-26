using NebNes.Misc;

namespace NebNes.PPU.Registers {
    public class PPUStatus : BaseRegister {
        public PPUStatus(NesPPU ppu) : base(ppu) { }

        public override void onLoad() {
            set(0b10000000);
        }

        public override byte onRead() {
            byte v = value;
            set(ByteLib.setBit(v, 7, false));
            ppu.loopy.onPPUStatusRead();
            return v;
        }

        public byte spriteOverflow() {
            return ByteLib.getBit(value, 5);
        }

        public byte sprite0Hit() {
            return ByteLib.getBit(value, 6);
        }

        public byte isInVBlankInterval() {
            return ByteLib.getBit(value, 7);
        }

        public void setInVBlankInterval(bool value) {
            set(ByteLib.setBit(this.value, 7, value));
        }

        public void setSprite0Hit(bool value) {
            set(ByteLib.setBit(this.value, 6, value));
        }

        public void setSpriteOverflow(bool value) {
            set(ByteLib.setBit(this.value, 5, value));
        }
    }
}
