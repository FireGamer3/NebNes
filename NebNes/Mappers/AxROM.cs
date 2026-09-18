using NebNes.CPU;
using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class AxROM : Mapper, IMapper {
        private byte prgBank = 0;
        private byte nameTablePage = 0;
        public AxROM(MOS6502 cpu, Cart cart) : base(cpu, cart) {}

        public byte cpuRead(ushort address) {
            if (address >= 0x8000 && address <= 0xbfff) {
                return getPrgPage(prgBank * 2)[address - 0x8000];
            } else if (address >= 0xc000 && address <= 0xffff) {
                return getPrgPage(prgBank * 2 + 1)[address - 0xc000];
            }
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x8000 && address <= 0xffff) {
                prgBank = ByteLib.getBits(value, 0, 4);
                nameTablePage = ByteLib.getBit(value, 4);
            }
        }

        public byte ppuRead(ushort address) {
            return getChrPage(0)[address];
        }

        public void ppuWrite(ushort address, byte value) {
            if (cart.usesChrRam()) {
                getChrPage(0)[address] = value;
            }
        }

        public void tick() { }

        public override PPUBgMirroring getMirroring() {
            return nameTablePage == 0
                ? PPUBgMirroring.SINGLE_SCREEN_LOW
                : PPUBgMirroring.SINGLE_SCREEN_HIGH;
        }
    }
}
