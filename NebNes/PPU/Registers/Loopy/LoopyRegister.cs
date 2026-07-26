using NebNes.Misc;

namespace NebNes.PPU.Registers.Loopy {
    public class LoopyRegister {
        public LoopyAddress vAddress = new LoopyAddress();
        public LoopyAddress tAddress = new LoopyAddress();
        public int fineX = 0;
        public bool latch = false;

        public int scrolledX(int x) {
            return vAddress.coarseX * Constants.TILE_SIZE_PIXELS + fineX + (x % Constants.TILE_SIZE_PIXELS);
        }

        public int scrolledY() {
            return vAddress.coarseY * Constants.TILE_SIZE_PIXELS + vAddress.fineY;
        }

        public int nameTableID(int scrolledX) {
            int baseNameTableId = vAddress.nameTableId;
            int offset =
              scrolledX >= Constants.SCREEN_WIDTH ? Constants.NAME_TABLE_OFFSETS[baseNameTableId] : 0;
            return baseNameTableId + offset;
        }

        public void onPPUCtrlWrite(byte value) {
            tAddress.nameTableId = ByteLib.getBits(value, 0, 2);
        }

        public void onPPUStatusRead() {
            latch = false;
        }

        public void onPPUScrollWrite(byte value) {
            if(!latch) {
                tAddress.coarseX = ByteLib.getBits(value, 3, 5);
                fineX = ByteLib.getBits(value, 0, 3);
            }else {
                tAddress.coarseY = ByteLib.getBits(value, 3, 5);
                tAddress.fineY = ByteLib.getBits(value, 0, 3);
            }
            latch = !latch;
        }

        public void onPPUAddrWrite(byte value) {
            if(!latch) {
                ushort number = tAddress.toUshort();
                byte high = ByteLib.highByteOf(number);
                high = ByteLib.setBits(high, 0, 6, ByteLib.getBits(value, 0, 6));
                high = ByteLib.setBits(high, 6, 1, 0);
                number = ByteLib.buildU16(ByteLib.lowByteOf(number), high);
                tAddress.setValue(number);
            }else {
                ushort number = tAddress.toUshort();
                number = ByteLib.buildU16(value, ByteLib.highByteOf(number));
                tAddress.setValue(number);
                vAddress.setValue(number);
            }
            latch = !latch;
        }

        public void onPreLine(int cycle) {
            if (cycle >= 280 && cycle <= 304) copyY();

            onLine(cycle);
        }

        public void onVisibleLine(int cycle) {
            onLine(cycle);
        }

        public void onPlot(int x) {
            int cycle = x + 1;
            if (cycle >= 8 && cycle <= 256 && cycle % 8 == 0)
                vAddress.incrementX();
        }

        private void onLine(int cycle) {
            if (cycle == 256) vAddress.incrementY();
            if (cycle == 257) copyX();
        }

        private void copyX() {
            ushort v = vAddress.toUshort();
            ushort t = tAddress.toUshort();
            vAddress.setValue((ushort)((v & 0b111101111100000) | (t & 0b000010000011111)));
        }

        private void copyY() {
            ushort v = vAddress.toUshort();
            ushort t = tAddress.toUshort();
            vAddress.setValue((ushort)((v & 0b000010000011111) | (t & 0b111101111100000)));
        }
    }
}
