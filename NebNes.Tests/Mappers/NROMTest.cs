using FluentAssertions;
using NebNes.Mappers;

namespace NebNes.Tests.Mappers {
    public class NROMTest {
        private static NROM BuildNROM(int prgPages, int chrPages,
                Func<int, byte>? prgFill = null, Func<int, byte>? chrFill = null) {
            return new NROM(null!, MapperTestHelpers.BuildCart(prgPages, chrPages, 0, prgFill, chrFill));
        }

        [Fact]
        public void NROM_cpuRead_reads_first_prg_bank() {
            NROM nrom = BuildNROM(1, 1, prgFill: i => i == 0 ? (byte)0x11 : i == 0x3FFF ? (byte)0x22 : (byte)0);
            nrom.cpuRead(0x8000).Should().Be(0x11);
            nrom.cpuRead(0xBFFF).Should().Be(0x22);
        }

        [Fact]
        public void NROM_cpuRead_mirrors_16k_bank_into_upper_half() {
            // A single 16 KiB PRG bank is mirrored across $C000-$FFFF.
            NROM nrom = BuildNROM(1, 1, prgFill: i => i == 0 ? (byte)0x11 : i == 0x3FFF ? (byte)0x22 : (byte)0);
            nrom.cpuRead(0xC000).Should().Be(0x11);
            nrom.cpuRead(0xFFFF).Should().Be(0x22);
        }

        [Fact]
        public void NROM_cpuRead_uses_both_banks_when_32k() {
            // 32 KiB PRG: $8000-$BFFF is bank 0, $C000-$FFFF is bank 1.
            NROM nrom = BuildNROM(2, 1, prgFill: i => i < 16384 ? (byte)0xA1 : (byte)0xB2);
            nrom.cpuRead(0x8000).Should().Be(0xA1);
            nrom.cpuRead(0xC000).Should().Be(0xB2);
        }

        [Fact]
        public void NROM_cpuRead_outside_cartridge_space_returns_zero() {
            NROM nrom = BuildNROM(1, 1, prgFill: i => 0xFF);
            nrom.cpuRead(0x0000).Should().Be(0);
            nrom.cpuRead(0x7FFF).Should().Be(0);
        }

        [Fact]
        public void NROM_cpuWrite_is_ignored() {
            NROM nrom = BuildNROM(1, 1, prgFill: i => i == 0 ? (byte)0x11 : (byte)0);
            nrom.cpuWrite(0x8000, 0xFF);
            nrom.cpuRead(0x8000).Should().Be(0x11);
        }

        [Fact]
        public void NROM_ppuRead_reads_chr() {
            NROM nrom = BuildNROM(1, 1,
                prgFill: i => 0x00,
                chrFill: i => i == 0 ? (byte)0x33 : i == 0x1FFF ? (byte)0x44 : (byte)0);
            nrom.ppuRead(0x0000).Should().Be(0x33);
            nrom.ppuRead(0x1FFF).Should().Be(0x44);
        }

        [Fact]
        public void NROM_ppuWrite_updates_chr_ram() {
            NROM nrom = BuildNROM(1, 0); // chrPages == 0 => CHR-RAM
            nrom.ppuWrite(0x0000, 0x55);
            nrom.ppuRead(0x0000).Should().Be(0x55);
        }

        [Fact]
        public void NROM_ppuWrite_ignored_when_chr_rom() {
            NROM nrom = BuildNROM(1, 1, chrFill: i => i == 0 ? (byte)0x33 : (byte)0);
            nrom.ppuWrite(0x0000, 0x55);
            nrom.ppuRead(0x0000).Should().Be(0x33);
        }
    }
}
