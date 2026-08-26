using NebNes.Misc;

namespace NebNes.PPU.Misc {
    public struct Sprite {
        private const byte TILE_SIZE_PIXELS = 8;
        private const byte PALETTE_FOREGROUND_START = 4;
        private const byte SPRITE_ATTR_PALETTE_BITS_START = 0;
        private const byte SPRITE_ATTR_PALETTE_BITS_SIZE = 2;
        private const byte SPRITE_ATTR_PRIORITY_BIT = 5;
        private const byte SPRITE_ATTR_HORIZONTAL_FLIP_BIT = 6;
        private const byte SPRITE_ATTR_VERTICAL_FLIP_BIT = 7;

        public byte id;
        public byte x;
        public int y;
        public bool is8x16;
        public byte patternTableID;
        public byte tileID;
        public byte attributes;

        public Sprite(byte id, byte x, int y, bool is8x16, byte patternTableID, byte topTileID, byte attributes) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.is8x16 = is8x16;
            this.patternTableID = patternTableID;
            this.tileID = topTileID;
            this.attributes = attributes;
        }

        public byte tileIdFor(int insideY) {
            int index = insideY >= TILE_SIZE_PIXELS ? 1 : 0;
            if (is8x16 && flipY) index = index == 0 ? 1 : 0;

            return (byte)(tileID + index);
        }

        public bool shouldRenderInScanline(int scanline) {
            int diff = diffY(scanline);

            return diff >= 0 && diff < height;
        }

        public int diffY(int scanline) {
            return scanline - y;
        }

        public byte paletteId =>
            (byte)(PALETTE_FOREGROUND_START +
                ByteLib.getBits(attributes, SPRITE_ATTR_PALETTE_BITS_START, SPRITE_ATTR_PALETTE_BITS_SIZE));

        public bool isInFrontOfBackground => ByteLib.getBit(attributes, SPRITE_ATTR_PRIORITY_BIT) == 0;

        public bool flipX => ByteLib.getFlag(attributes, SPRITE_ATTR_HORIZONTAL_FLIP_BIT);

        public bool flipY => ByteLib.getFlag(attributes, SPRITE_ATTR_VERTICAL_FLIP_BIT);

        public int height => is8x16 ? 16 : 8;
    }
}
