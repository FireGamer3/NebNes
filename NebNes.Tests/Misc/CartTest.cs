using FluentAssertions;
using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.Tests.Misc {
    public class CartTest {
        [Fact]
        public void Cart_Constructor_with_Invalid_header() {
            byte[] empty = new byte[4];
            Cart cart1 = new Cart(empty);
            cart1.isRomInvalid().Should().BeTrue();
        }

        [Fact]
        public void Cart_Constructor_with_Malformed_header() {
            byte[] malformed = [0x4E, 0x45, 0x53, 0x1F];
            Cart cart1 = new Cart(malformed);
            cart1.isRomInvalid().Should().BeTrue();
        }

        [Fact]
        public void Cart_Constructor_with_Correct_header() {
            byte[] correct = [0x4E, 0x45, 0x53, 0x1A];
            Cart cart1 = new Cart(correct);
            cart1.isRomInvalid().Should().BeFalse();
        }

        [Fact]
        public void Cart_prgRomPages_byte() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02];
            Cart cart1 = new Cart(header);
            cart1.prgRomPages().Should().Be(0x02);
        }

        [Fact]
        public void Cart_chrRomPages_byte() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01];
            Cart cart1 = new Cart(header);
            cart1.chrRomPages().Should().Be(0x01);
        }

        [Fact]
        public void Cart_usesChrRam_true() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x00];
            Cart cart1 = new Cart(header);
            cart1.usesChrRam().Should().BeTrue();
        }

        [Fact]
        public void Cart_usesChrRam_false() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01];
            Cart cart1 = new Cart(header);
            cart1.usesChrRam().Should().BeFalse();
        }

        [Fact]
        public void Cart_has512BytePadding_true() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000100];
            Cart cart1 = new Cart(header);
            cart1.has512BytePadding().Should().BeTrue();
        }

        [Fact]
        public void Cart_has512BytePadding_false() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000000];
            Cart cart1 = new Cart(header);
            cart1.has512BytePadding().Should().BeFalse();
        }

        [Fact]
        public void Cart_hasPrgRam_true() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000010];
            Cart cart1 = new Cart(header);
            cart1.hasPrgRam().Should().BeTrue();
        }

        [Fact]
        public void Cart_hasPrgRam_false() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000000];
            Cart cart1 = new Cart(header);
            cart1.hasPrgRam().Should().BeFalse();
        }

        [Fact]
        public void Cart_getMirroring_vertical() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000001];
            Cart cart1 = new Cart(header);
            cart1.getMirroring().Should().Be(PPUBgMirroring.VERTICAL);
        }

        [Fact]
        public void Cart_getMirroring_horizontal() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000000000];
            Cart cart1 = new Cart(header);
            cart1.getMirroring().Should().Be(PPUBgMirroring.HORIZONTAL);
        }

        [Fact]
        public void Cart_getMirroring_four_screen() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000001000];
            byte[] headerV = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0b000001001];
            Cart cart1 = new Cart(header);
            Cart cart2 = new Cart(headerV);
            cart1.getMirroring().Should().Be(PPUBgMirroring.FOUR_SCREEN);
            cart2.getMirroring().Should().Be(PPUBgMirroring.FOUR_SCREEN);
        }

        [Fact]
        public void Cart_getMapperID_zero() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x00, 0x00];
            Cart cart1 = new Cart(header);
            cart1.getMapperID().Should().Be(0);
        }

        [Fact]
        public void Cart_getMapperID_combines_nibbles() {
            // Mapper low nibble comes from byte 6's high nibble,
            // mapper high nibble comes from byte 7's high nibble.
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x30, 0x40];
            Cart cart1 = new Cart(header);
            cart1.getMapperID().Should().Be(0x43);
        }

        [Fact]
        public void Cart_getPrg_returns_prg_pages() {
            byte[] rom = new byte[16 + 16384];
            rom[0] = 0x4E; rom[1] = 0x45; rom[2] = 0x53; rom[3] = 0x1A;
            rom[4] = 0x01; // 1 PRG page
            rom[5] = 0x01; // 1 CHR page
            rom[6] = 0x00;
            rom[16] = 0xAA;
            rom[16 + 16383] = 0xBB;
            Cart cart1 = new Cart(rom);
            byte[] prg = cart1.getPrg();
            prg.Length.Should().Be(16384);
            prg[0].Should().Be(0xAA);
            prg[16383].Should().Be(0xBB);
        }

        [Fact]
        public void Cart_getPrg_respects_512_byte_padding() {
            byte[] rom = new byte[16 + 512 + 16384];
            rom[0] = 0x4E; rom[1] = 0x45; rom[2] = 0x53; rom[3] = 0x1A;
            rom[4] = 0x01; // 1 PRG page
            rom[5] = 0x01; // 1 CHR page
            rom[6] = 0b000000100; // 512-byte trainer present
            rom[16 + 512] = 0xAA;
            Cart cart1 = new Cart(rom);
            byte[] prg = cart1.getPrg();
            prg.Length.Should().Be(16384);
            prg[0].Should().Be(0xAA);
        }

        [Fact]
        public void Cart_getChr_returns_ram_when_no_chr() {
            byte[] header = [0x4E, 0x45, 0x53, 0x1A, 0x01, 0x00];
            Cart cart1 = new Cart(header);
            byte[] chr = cart1.getChr();
            chr.Length.Should().Be(8192);
        }

        [Fact]
        public void Cart_getChr_returns_chr_pages() {
            byte[] rom = new byte[16 + 16384 + 8192];
            rom[0] = 0x4E; rom[1] = 0x45; rom[2] = 0x53; rom[3] = 0x1A;
            rom[4] = 0x01; // 1 PRG page
            rom[5] = 0x01; // 1 CHR page
            rom[6] = 0x00;
            int chrStart = 16 + 16384;
            rom[chrStart] = 0xCC;
            rom[chrStart + 8191] = 0xDD;
            Cart cart1 = new Cart(rom);
            byte[] chr = cart1.getChr();
            chr.Length.Should().Be(8192);
            chr[0].Should().Be(0xCC);
            chr[8191].Should().Be(0xDD);
        }
    }
}
