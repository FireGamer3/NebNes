using NebNes.CPU;
using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    internal class MMC1 : Mapper, IMapper {
        private byte[] prgRam = new byte[0x2000];
        int srLen = 0;
        byte sr = 0;
        byte control = 0x0C;
        byte chrBank0 = 0;
        byte chrBank1 = 0;
        byte prgBank = 0;
        long lastCycle = 0;

        public MMC1(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

        public byte cpuRead(ushort address) {
            if(address >= 0x6000 && address <= 0x7FFF) {
                return prgRam[address - 0x6000];
            }
            if (address >= 0x8000 && address <= 0xBFFF) {
                return getPrgPage(prgBankLow())[address - 0x8000];
            }
            if (address >= 0xC000 && address <= 0xFFFF) {
                return getPrgPage(prgBankHigh())[address - 0xC000];
            }
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x6000 && address <= 0x7FFF)
                prgRam[address - 0x6000] = value;
            if (address >= 0x8000) {
                if(ByteLib.getBit(value, 7) == 1) {
                    resetMapper();
                    return;
                } else {
                    if (cpu.currentCycle == lastCycle) return;
                    lastCycle = cpu.currentCycle;
                    sr = (byte)(sr >> 1);
                    sr = ByteLib.setBit(sr, 4, ByteLib.getBit(value, 0) == 1);
                    srLen++;
                    if(srLen == 5) {
                        registerWrite((byte)((byte)(address >> 13) & 3), sr);
                        sr = 0;
                        srLen = 0;
                    }
                }
            }
        }

        public byte ppuRead(ushort address) {
            if (chrBankMode() == 0) {
                int bank8k = ByteLib.getBits(chrBank0, 1, 4);
                return getChrPage(bank8k)[address];
            } else {
                int bank4k = address < 0x1000
                    ? ByteLib.getBits(chrBank0, 0, 5)
                    : ByteLib.getBits(chrBank1, 0, 5);
                return readChr4k(bank4k, address & 0x0FFF);
            }
        }

        public void ppuWrite(ushort address, byte value) {
            if(cart.usesChrRam()) {
                getChrPage(0)[address] = value;
            }
        }

        public void tick() { }

        public override PPUBgMirroring getMirroring() {
            return nameTableArrangement() switch {
                0 => PPUBgMirroring.SINGLE_SCREEN_LOW,
                1 => PPUBgMirroring.SINGLE_SCREEN_HIGH,
                2 => PPUBgMirroring.VERTICAL,
                _ => PPUBgMirroring.HORIZONTAL,
            };
        }

        private void resetMapper() {
            sr = 0;
            srLen = 0;
            control = ByteLib.setBits(control, 2, 2, 0b11);
        }

        private void registerWrite(byte selector, byte value) {
            switch(selector) {
                case 0:
                    control = value;
                    break;
                case 1:
                    chrBank0 = value;
                    break;
                case 2:
                    chrBank1 = value;
                    break;
                case 3:
                    prgBank = value;
                    break;
            }
        }

        private byte nameTableArrangement() {
            return ByteLib.getBits(control, 0, 2);
        }

        private byte prgBankMode() {
            return ByteLib.getBits(control, 2, 2);
        }

        private byte chrBankMode() {
            return ByteLib.getBit(control, 4);
        }

        private byte prgBankLow() {
            switch (prgBankMode()) {
                case 0:
                case 1:
                    return (byte)(ByteLib.getBits(prgBank, 0, 4) & 0xE);
                case 2:
                    return 0;
                case 3:
                    return ByteLib.getBits(prgBank, 0, 4);
                default:
                    return 0;
            }
        }

        private byte prgBankHigh() {
            switch (prgBankMode()) {
                case 0:
                case 1:
                    return (byte)((byte)(ByteLib.getBits(prgBank, 0, 4) & 0xE) + 1);
                case 2:
                    return ByteLib.getBits(prgBank, 0, 4);
                case 3:
                    return (byte)(prgPages.Length - 1);
                default:
                    return 0;
            }
        }
        private byte readChr4k(int bank4k, int offset) {
            int totalBanks = chrPages.Length * 2;
            bank4k %= totalBanks;
            return chrPages[bank4k / 2][(bank4k & 1) * 0x1000 + offset];
        }
    }
}
