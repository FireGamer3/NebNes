using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class GxROM : Mapper, IMapper {
        byte chrRomBank = 0;
        byte prgRomBank = 0;
        public GxROM(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

        public byte cpuRead(ushort address) {
            if (address >= 0x8000 && address <= 0xbfff) {
                return getPrgPage(prgRomBank)[address - 0x8000];
            } else if (address >= 0xc000 && address <= 0xffff) {
                return getPrgPage(prgRomBank + 1)[address - 0xc000];
            }
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x8000) {
                chrRomBank = ByteLib.getBits(value, 0, 2);
                if(cart.prgRomPages() == 4) {
                    prgRomBank = ByteLib.getBit(value, 4);
                } else prgRomBank = ByteLib.getBits(value, 4, 2);
            }
        }

        public byte ppuRead(ushort address) {
            return getChrPage(chrRomBank)[address];
        }

        public void ppuWrite(ushort address, byte value) {
            if(cart.usesChrRam()) {
                getChrPage(chrRomBank)[address] = value;
            }
        }

        public void tick() { }
    }
}
