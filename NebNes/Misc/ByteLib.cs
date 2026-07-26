using NebNes.CPU.Instructions.Checks;
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

        public static byte setBit(byte b, int bit, bool set = true) {
            return set ? (byte)(b | (1 << bit)) : (byte)(b & ~(1 << bit));
        }

        public static byte getBits(byte b, int start, int len) {
            return (byte)((byte)(b >> (byte)start) & (byte)(0xff >> (byte)(8 - len)));
        }

        public static byte setBits(byte b, int start, int size, byte value) {
            byte mask = (byte)(((1 << size) - 1) << start);
            return (byte)((b & ~mask) | ((value << start) & mask));
        }

        public static byte buildU8(byte low, byte high) {
            byte res = (byte)(high & 0x0F);
            res = (byte)(res << 4);
            return (byte)(res + (low & 0x0F));
        }

        public static byte bitfield(byte bit0, byte bit1, byte bit2, byte bit3, byte bit4, byte bit5, byte bit6, byte bit7) {
            return
              (byte)(((bit0 & 1) << 0) |
              ((bit1 & 1) << 1) |
              ((bit2 & 1) << 2) |
              ((bit3 & 1) << 3) |
              ((bit4 & 1) << 4) |
              ((bit5 & 1) << 5) |
              ((bit6 & 1) << 6) |
              ((bit7 & 1) << 7))
            ;
        }

        public static ushort buildU16(byte lo, byte hi) {
            ushort res = (ushort)(hi & 0xFF);
            res = (ushort)(res << 8);
            return (ushort)(res + lo);
        }

        public static byte buildU2(byte highBit, byte lowBit) {
            return (byte)((highBit << 1) | lowBit);
        }

        public static bool isPositive(byte b) {
            return ((b & 0x80) >> 7) == 0;
        }

        public static bool isNegative(byte b) {
            return ((b & 0x80) >> 7) == 1;
        }

        public static byte highByteOf(ushort val) {
            return (byte)((ushort)(val & 0xFF00) >> 8);
        }

        public static byte lowByteOf(ushort val) {
            return (byte)(val & 0x00FF);
        }
    }
}
