using FluentAssertions;
using NebNes.Mappers;

namespace NebNes.Tests.Mappers {
    public class UxROMTest {
        // Fills every byte of each 16 KiB PRG bank with that bank's index, so a read
        // result identifies which bank it came from.
        private static readonly Func<int, byte> BankMarker = i => (byte)(i / 16384);

        private static UxROM BuildUxROM(int prgPages, int chrPages,
                Func<int, byte>? prgFill = null, Func<int, byte>? chrFill = null) {
            return new UxROM(null!, MapperTestHelpers.BuildCart(prgPages, chrPages, 2, prgFill, chrFill));
        }

        [Fact]
        public void UxROM_switchable_bank_defaults_to_page_zero() {
            UxROM m = BuildUxROM(4, 0, prgFill: BankMarker);
            m.cpuRead(0x8000).Should().Be(0);
            m.cpuRead(0xBFFF).Should().Be(0);
        }

        [Fact]
        public void UxROM_cpuWrite_selects_switchable_bank() {
            UxROM m = BuildUxROM(4, 0, prgFill: BankMarker);
            m.cpuWrite(0x8000, 2);
            m.cpuRead(0x8000).Should().Be(2);
            m.cpuRead(0xBFFF).Should().Be(2);
        }

        [Fact]
        public void UxROM_fixed_bank_is_always_last() {
            UxROM m = BuildUxROM(4, 0, prgFill: BankMarker);
            m.cpuRead(0xC000).Should().Be(3); // last of four banks
            m.cpuWrite(0x8000, 1);             // switching the low bank...
            m.cpuRead(0xC000).Should().Be(3); // ...leaves the fixed bank untouched
        }

        [Fact]
        public void UxROM_bank_select_wraps_around_bank_count() {
            UxROM m = BuildUxROM(4, 0, prgFill: BankMarker);
            m.cpuWrite(0x8000, 5); // 5 % 4 == 1
            m.cpuRead(0x8000).Should().Be(1);
        }

        [Fact]
        public void UxROM_cpuRead_outside_cartridge_space_returns_zero() {
            UxROM m = BuildUxROM(4, 0, prgFill: i => 0xFF);
            m.cpuRead(0x0000).Should().Be(0);
            m.cpuRead(0x7FFF).Should().Be(0);
        }

        [Fact]
        public void UxROM_ppuWrite_updates_chr_ram() {
            UxROM m = BuildUxROM(2, 0);
            m.ppuWrite(0x0000, 0x55);
            m.ppuRead(0x0000).Should().Be(0x55);
        }
    }
}
