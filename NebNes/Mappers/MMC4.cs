using NebNes.CPU;
using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    internal class MMC4 : Mapper, IMapper {
        private byte[] prgRam = new byte[0x2000];
        private byte prgBank = 0;

        private byte chrFDzeroBank = 0;
        private byte chrFEzeroBank = 0;
        private byte chrFDoneBank = 0;
        private byte chrFEoneBank = 0;

        private bool latch0high = false;
        private bool latch1high = false;
        private PPUBgMirroring mirroring;

        public MMC4(MOS6502 cpu, Cart cart) : base(cpu, cart) {
            mirroring = cart.getMirroring();
        }

        public byte cpuRead(ushort address) {
            if (address >= 0x6000 && address <= 0x7FFF) {
                return prgRam[address - 0x6000];
            }
            if(address >= 0x8000 && address <= 0xBFFF) {
                return getPrgPage(prgBank)[address - 0x8000];
            }
            if (address >= 0xC000) {
                return getPrgPage(prgPages.Length - 1)[address - 0xC000];
            }
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x6000 && address <= 0x7FFF) {
                prgRam[address - 0x6000] = value;
            } else if (address >= 0xA000 && address <= 0xAFFF) {
                prgBank = ByteLib.getBits(value, 0, 4);
            } else if (address >= 0xB000 && address <= 0xBFFF) {
                chrFDzeroBank = ByteLib.getBits(value, 0, 5);
            } else if (address >= 0xC000 && address <= 0xCFFF) {
                chrFEzeroBank = ByteLib.getBits(value, 0, 5);
            } else if (address >= 0xD000 && address <= 0xDFFF) {
                chrFDoneBank = ByteLib.getBits(value, 0, 5);
            } else if (address >= 0xE000 && address <= 0xEFFF) {
                chrFEoneBank = ByteLib.getBits(value, 0, 5);
            } else if (address >= 0xF000 && address <= 0xFFFF) {
                mirroring = ByteLib.getBit(value, 0) == 0 ? PPUBgMirroring.VERTICAL : PPUBgMirroring.HORIZONTAL;
            }
        }

        public byte ppuRead(ushort address) {
            byte value = readValue(address);
            if (address == 0x0FD8) latch0high = false;
            if (address == 0x0FE8) latch0high = true;

            if (address >= 0x1FD8 && address <= 0x1FDF) latch1high = false;
            if (address >= 0x1FE8 && address <= 0x1FEF) latch1high = true;
            return value;
        }

        public void ppuWrite(ushort address, byte value) { }

        public void tick() { }

        public override PPUBgMirroring getMirroring() {
            return mirroring;
        }

        private byte readValue(ushort address) {
            if (address <= 0x0FFF) {
                return getChrPage4k(latchZeroPage())[address];
            } else if (address >= 0x1000 && address <= 0x1FFF) {
                return getChrPage4k(latchOnePage())[address - 0x1000];
            } else return 0;
        }

        private byte latchZeroPage() {
            return latch0high ? chrFEzeroBank : chrFDzeroBank;
        }

        private byte latchOnePage() {
            return latch1high ? chrFEoneBank : chrFDoneBank;
        }
    }
}
