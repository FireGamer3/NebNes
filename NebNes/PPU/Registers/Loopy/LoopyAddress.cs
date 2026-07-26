namespace NebNes.PPU.Registers.Loopy {
    public class LoopyAddress {
        public int coarseX = 0;
        public int coarseY = 0;
        public int nameTableId = 0;
        public int fineY = 0;


        public void incrementX() {
            if (coarseX == 31) {
                coarseX = 0;
                switchHorizontalNameTable();
            } else {
                coarseX++;
            }
        }

        public void incrementY() {
            if (fineY < 7) {
                fineY++;
            } else {
                fineY = 0;

                if (coarseY == 29) {
                    coarseY = 0;
                    switchVerticalNameTable();
                } else if (coarseY == 31) {
                    coarseY = 0;
                } else {
                    coarseY++;
                }
            }
        }

        public ushort getValue() {
            return (ushort)(toUshort() & 0b11111111111111);
        }

        public void setValue(ushort number) {
            coarseX =
                (number >> Constants.LOOPY_ADDR_COARSE_X_OFFSET) & Constants.LOOPY_ADDR_COARSE_X_MASK;
            coarseY =
              (number >> Constants.LOOPY_ADDR_COARSE_Y_OFFSET) & Constants.LOOPY_ADDR_COARSE_Y_MASK;
            nameTableId =
              (number >> Constants.LOOPY_ADDR_BASE_NAME_TABLE_ID_OFFSET) &
              Constants.LOOPY_ADDR_BASE_NAME_TABLE_ID_MASK;
            fineY = (number >> Constants.LOOPY_ADDR_FINE_Y_OFFSET) & Constants.LOOPY_ADDR_FINE_Y_MASK;
        }

        public ushort toUshort() {
            return (ushort)((coarseX << Constants.LOOPY_ADDR_COARSE_X_OFFSET) |
                          (coarseY << Constants.LOOPY_ADDR_COARSE_Y_OFFSET) |
                          (nameTableId << Constants.LOOPY_ADDR_BASE_NAME_TABLE_ID_OFFSET) |
                          (fineY << Constants.LOOPY_ADDR_FINE_Y_OFFSET));
        }

        private void switchHorizontalNameTable() {
            nameTableId = nameTableId ^ 0b1;
        }

        private void switchVerticalNameTable() {
            nameTableId = nameTableId ^ 0b10;
        }
    }
}
