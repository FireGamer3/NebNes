using System.Collections.Generic;
using NebNes.PPU.Misc;

namespace NebNes.PPU.Sprites {
    public class SpriteRenderer {
        NesPPU ppu;

        // Reused across scanlines to avoid per-scanline heap allocations.
        private readonly List<Sprite> sprites = new List<Sprite>(8);
        private readonly SpritePixel[] buffer = new SpritePixel[256];
        private readonly uint[] colors = new uint[4];

        public SpriteRenderer(NesPPU ppu) {
            this.ppu = ppu;
        }

        public void renderScanline() {
            if (ppu.ppuMask.showSprites() == 0) return;
            evaluate();
            render();
            draw();
        }

        private void evaluate() {
            sprites.Clear();
            for (int i = 0; i < 64; i++) {
                Sprite sprite = createSprite((byte)i);
                if (sprite.shouldRenderInScanline(ppu.scanline)) {
                    if (sprites.Count >= 8) {
                        ppu.ppuStatus.setSpriteOverflow(true);
                        break;
                    } else {
                        sprites.Add(sprite);
                    }
                }
            }
            sprites.Reverse();
        }

        private void render() {
            int y = ppu.scanline;
            Array.Clear(buffer, 0, buffer.Length);
            foreach (Sprite sprite in sprites) {
                int insideY = sprite.diffY(y);
                int tileInsideY = insideY % 8;
                Tile tile = new Tile(ppu, sprite.patternTableID, sprite.tileIdFor(insideY), sprite.flipY ? 7 - tileInsideY : tileInsideY);
                colors[0] = ppu.getColor(sprite.paletteId, 0);
                colors[1] = ppu.getColor(sprite.paletteId, 1);
                colors[2] = ppu.getColor(sprite.paletteId, 2);
                colors[3] = ppu.getColor(sprite.paletteId, 3);
                for (int insideX = 0; insideX < 8; insideX++) {
                    byte colorIndex = tile.getColorIndex((byte)insideX);
                    int finalX = sprite.flipX ? 7 - insideX : insideX;
                    if (colorIndex > 0) {
                        int px = sprite.x + finalX;
                        if (ppu.ppuMask.showSpritesInFirst8Pixels() == 0 && px < 8) continue;
                        if (px >= 256) continue;
                        buffer[px] = new SpritePixel { set = true, sprite = sprite, color = colors[colorIndex] };
                        if (sprite.id == 0 && ppu.isBackgroundPixelOpaque((byte)px, (byte)y) && ppu.ppuMask.showBackground() == 1)
                            ppu.ppuStatus.setSprite0Hit(true);
                    }
                }
            }
        }

        private void draw() {
            int y = ppu.scanline;
            for (int x = 0; x < buffer.Length; x++) {
                SpritePixel element = buffer[x];
                if (element.set) {
                    bool isInFront = element.sprite.isInFrontOfBackground;
                    bool isBGOpaque = ppu.isBackgroundPixelOpaque((byte)x, (byte)y);
                    if (isInFront || !isBGOpaque)
                        ppu.plot((byte)x, (byte)y, element.color);
                }
            }
        }

        private Sprite createSprite(byte id) {
            bool eightBySixteenMode = ppu.ppuCtrl.spriteSize() == 1;
            int oamIndex = id * 4;
            int spriteY = ppu.bus.oamRam[oamIndex] + 1;
            byte spriteTileID = eightBySixteenMode
                ? (byte)(ppu.bus.oamRam[oamIndex + 1] & 0xFE)
                : ppu.bus.oamRam[oamIndex + 1];
            byte spriteAttributes = ppu.bus.oamRam[oamIndex + 2];
            byte spriteX = ppu.bus.oamRam[oamIndex + 3];
            byte patternTableID = eightBySixteenMode
                ? (byte)(ppu.bus.oamRam[oamIndex + 1] & 0x01)
                : ppu.ppuCtrl.sprite8x8PatternTableId();
            return new Sprite(id, spriteX, spriteY, eightBySixteenMode, patternTableID, spriteTileID, spriteAttributes);
        }

        private struct SpritePixel {
            public bool set;
            public Sprite sprite;
            public uint color;
        }
    }
}
