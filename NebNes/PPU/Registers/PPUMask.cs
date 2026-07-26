using NebNes.Misc;

namespace NebNes.PPU.Registers {
    public class PPUMask : BaseRegister {
        public PPUMask(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte value) {
            set(value);
        }

        public bool isRenderingEnabled() {
            return showBackground() == 1 || showSprites() == 1;
        }

        public uint transform(uint color) {
            byte r = (byte)(color & 0xFF);
            byte g = (byte)((color >> 8) & 0xFF);
            byte b = (byte)((color >> 16) & 0xFF);
            if (grayscale() == 1) {
                r = g = b = (byte)Math.Floor((double)((r + g + b) / 3));
            }
            if(emphasizeRed() == 1 || emphasizeBlue() == 1 || emphasizeGreen() == 1) {
                bool all = emphasizeRed() == 1 && emphasizeBlue() == 1 && emphasizeGreen() == 1;
                if(emphasizeRed() == 0 || all) {
                    r = (byte)Math.Floor(r * 0.75);
                }
                if (emphasizeGreen() == 0 || all) {
                    g = (byte)Math.Floor(g * 0.75);
                }
                if (emphasizeBlue() == 0 || all) {
                    b = (byte)Math.Floor(b * 0.75);
                }
            }
            return 0xFF000000u | ((uint)r << 16) | ((uint)g << 8) | ((uint)b << 0);
        }

        public byte grayscale() {
            return ByteLib.getBit(value, 0);
        }

        public byte showBackgroundInFirst8Pixels() {
            return ByteLib.getBit(value, 1);
        }

        public byte showSpritesInFirst8Pixels() {
            return ByteLib.getBit(value, 2);
        }

        public byte showBackground() {
            return ByteLib.getBit(value, 3);
        }

        public byte showSprites() {
            return ByteLib.getBit(value, 4);
        }

        public byte emphasizeRed() {
            return ByteLib.getBit(value, 5);
        }

        public byte emphasizeGreen() {
            return ByteLib.getBit(value, 6);
        }

        public byte emphasizeBlue() {
            return ByteLib.getBit(value, 7);
        }
    }
}
