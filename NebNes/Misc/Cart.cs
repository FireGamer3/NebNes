using NebNes.Enums;

namespace NebNes.Misc {
    public class Cart {
        private byte[] _romBytes;

        public Cart(byte[] romBytes) {
            _romBytes = romBytes;
        }

        public byte prgRomPages() {
            return _romBytes[4];
        }

        public byte chrRomPages() {
            return _romBytes[5];
        }

        public bool usesChrRam() {
            return _romBytes[5] == 0;
        }

        public bool has512BytePadding() {
            return ByteLib.getFlag(_romBytes[6], 2);
        }

        public bool hasPrgRam() {
            return ByteLib.getFlag(_romBytes[6], 1);
        }

        public PPUBgMirroring getMirroring() {
            byte mainMirrorBit = ByteLib.getBit(_romBytes[6], 0);
            byte fourScreenOverride = ByteLib.getBit(_romBytes[6], 3);
            if (fourScreenOverride == 1) return PPUBgMirroring.FOUR_SCREEN;
            if (mainMirrorBit == 1) return PPUBgMirroring.VERTICAL;
            return PPUBgMirroring.HORIZONTAL;
        }

        public byte getMapperID() {
            return ByteLib.buildU8(ByteLib.getBits(_romBytes[6], 4, 4), ByteLib.getBits(_romBytes[7], 4, 4));
        }

        public byte[] getPrg() {
            int startIndex = prgStartByte();
            return _romBytes[startIndex..(startIndex + (16384 * prgRomPages()))];
        }

        public byte[] getChr() {
            if (usesChrRam()) return new byte[8192];
            int startIndex = prgStartByte() + (16384 * prgRomPages());
            return _romBytes[startIndex..(startIndex + (8192 * chrRomPages()))];
        }

        public bool isRomInvalid() {
            byte[] inesHeader = [0x4E, 0x45, 0x53, 0x1A];
            for (int i = 0; i < 4; i++) {
                if (inesHeader[i] != _romBytes[i]) return true;
            }
            return false;
        }

        private int prgStartByte() {
            return 16 + (has512BytePadding() ? 512 : 0);
        }
    }
}
