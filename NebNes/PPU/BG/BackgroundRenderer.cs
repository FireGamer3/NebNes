using NebNes.Misc;
using NebNes.PPU.Misc;
using System.Xml;

namespace NebNes.PPU.BG {
    public class BackgroundRenderer {
        NesPPU ppu;

        public BackgroundRenderer(NesPPU ppu) {
            this.ppu = ppu;
        }

        public void renderScanline() {
            int y = ppu.scanline;
            for (int x = 0; x < 272;) {
                if (ppu.ppuMask.showBackground() == 0 || (ppu.ppuMask.showBackgroundInFirst8Pixels() == 0 && x < 8)) {
                    if(x < 256) ppu.plotBG(x, y, ppu.getColor(0, 0), 0);
                    x++;
                    continue;
                }
                int scrolledX =  ppu.loopy.scrolledX(x);
                int scrolledY = ppu.loopy.scrolledY();

                int nameTableID = ppu.loopy.nameTableID(scrolledX);

                int nameTableX = scrolledX % 256;
                int nameTableY = scrolledY % 240;

                byte patternTableID = ppu.ppuCtrl.backgroundPatternTableId();

                byte tileX = (byte)(nameTableX / 8);
                byte tileY = (byte)(nameTableY / 8);
                int tileIndex = tileY * 32 + tileX;
                byte tileID = ppu.bus.read((ushort)(0x2000 + (nameTableID * 1024) + tileIndex));
                byte paletteID = getBackgroundPaletteId(nameTableID, nameTableX, nameTableY);
                byte tileInsideY = (byte)(nameTableY % 8);

                byte tileStartX = (byte)(nameTableX % 8);
                byte tilePixels = (byte)Math.Min(8 - tileStartX, 272 - x);
                Tile tile = new Tile(ppu, patternTableID, tileID, tileInsideY);
                for (int xx = 0; xx < tilePixels; xx++) {
                    byte colorIndex = tile.getColorIndex((byte)(tileStartX + xx));
                    uint color = colorIndex > 0 ? ppu.getColor(paletteID, colorIndex) : ppu.getColor(0,0);
                    if (x + xx < 256) ppu.plotBG((x + xx), y, color, colorIndex);
                }
                x += tilePixels;
            }
        }

        public byte getBackgroundPaletteId(int nameTableId, int x, int y) {
            int metaBlockX = x / 32;
            int metaBlockY = y / 32;
            int metaBlockIndex = metaBlockY * 8 + metaBlockX;
            ushort startAddress = (ushort)(0x23c0 + (nameTableId * 1024));
            byte block = ppu.bus.read((ushort)(startAddress + metaBlockIndex));
            byte blockX = (byte)((x % 32) / 16);
            byte blockY = (byte)((y % 32) / 16);
            byte blockIndex = (byte)(blockY * 2 + blockX);
            byte low = ByteLib.getBit(block, blockIndex * 2);
            byte high = ByteLib.getBit(block, (blockIndex * 2) + 1);
            return ByteLib.buildU2(high, low);
        }
    }
}
