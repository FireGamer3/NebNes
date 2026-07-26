using NebNes.Misc;

namespace NebNes.PPU.BG {
    public struct Tile {
        public ushort tableAddress;
        public ushort lowPlaneAddress;
        public ushort highPlaneAddress;
        private byte lowRow;
        private byte highRow;

        public Tile(NesPPU ppu, byte patternTableID, byte tileID, int y) {
            tableAddress = (ushort)(patternTableID == 1 ? 0x1000 : 0x0000);
            lowPlaneAddress = (ushort)(tableAddress + (tileID * 16));
            highPlaneAddress = (ushort)(lowPlaneAddress + 8);
            lowRow = ppu.bus.read((ushort)(lowPlaneAddress + y));
            highRow = ppu.bus.read((ushort)(highPlaneAddress + y));
        }

        public byte getColorIndex(byte x) {
            byte position = (byte)(7 - x);
            byte low = ByteLib.getBit(lowRow, position);
            byte high = ByteLib.getBit(highRow, position);
            return ByteLib.buildU2(high, low);
  }
    }
}
