using NebNes.Misc;

namespace NebNes.Tests.Mappers {
    internal static class MapperTestHelpers {
        /// <summary>
        /// Builds a Cart wrapping a minimal, well-formed iNES image with the given
        /// number of PRG (16 KiB) and CHR (8 KiB) banks. The optional fill callbacks
        /// receive the byte offset within the PRG/CHR region so tests can stamp
        /// recognisable content into each bank.
        /// </summary>
        public static Cart BuildCart(int prgPages, int chrPages, byte mapperId,
                Func<int, byte>? prgFill = null, Func<int, byte>? chrFill = null) {
            int prgLen = prgPages * 16384;
            int chrLen = chrPages * 8192;
            byte[] rom = new byte[16 + prgLen + chrLen];
            rom[0] = 0x4E; rom[1] = 0x45; rom[2] = 0x53; rom[3] = 0x1A;
            rom[4] = (byte)prgPages;
            rom[5] = (byte)chrPages;
            // Mapper low nibble lives in byte 6's high nibble, high nibble in byte 7's.
            rom[6] = (byte)((mapperId & 0x0F) << 4);
            rom[7] = (byte)(mapperId & 0xF0);
            for (int i = 0; i < prgLen; i++) rom[16 + i] = prgFill?.Invoke(i) ?? 0;
            for (int i = 0; i < chrLen; i++) rom[16 + prgLen + i] = chrFill?.Invoke(i) ?? 0;
            return new Cart(rom);
        }
    }
}
