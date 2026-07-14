using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace NebNes.Misc {
    public static class ByteLib {
        public static byte toU8(byte b) {
            return (byte)(b & 0xFF);
        }

        public static bool getFlag(byte b, int bit) {
            return getBit(b, bit) != 0;
        }

        public static byte getBit(byte b, int bit) {
            return (byte)((b >> bit) & 1);
        }

        public static byte setBit(byte b, int bit) {
            return (byte)(b | (1 << bit));
        }

        public static byte getBits(byte b, int start, int len) {
            return (byte)((byte)(b >> (byte)start) & (byte)(0xff >> (byte)(8 - len)));
        }

        public static byte buildU8(byte low, byte high) {
            byte res = (byte)(high & 0x0F);
            res = (byte)(res << 4);
            return (byte)(res + (low & 0x0F));
        }

        public static bool isPositive(byte b) {
            return ((b & 0x80) >> 7) == 0;
        }

        public static bool isNegative(byte b) {
            return ((b & 0x80) >> 7) == 1;
        }
    }
}
