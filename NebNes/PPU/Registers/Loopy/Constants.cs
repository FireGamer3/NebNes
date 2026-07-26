using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.PPU.Registers.Loopy {
    public static class Constants {
        public static int LOOPY_ADDR_COARSE_X_OFFSET = 0;
        public static byte LOOPY_ADDR_COARSE_X_MASK = 0b11111;
        public static int LOOPY_ADDR_COARSE_Y_OFFSET = 5;
        public static byte LOOPY_ADDR_COARSE_Y_MASK = 0b11111;
        public static int LOOPY_ADDR_BASE_NAME_TABLE_ID_OFFSET = 10;
        public static byte LOOPY_ADDR_BASE_NAME_TABLE_ID_MASK = 0b11;
        public static int LOOPY_ADDR_FINE_Y_OFFSET = 12;
        public static byte LOOPY_ADDR_FINE_Y_MASK = 0b111;
        public static int[] NAME_TABLE_OFFSETS = [1, -1, 1, -1];
        public static int TILE_SIZE_PIXELS = 8;
        public static int SCREEN_WIDTH = 256;
    }
}
