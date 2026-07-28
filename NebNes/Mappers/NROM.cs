using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class NROM : Mapper, IMapper {
        public NROM(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

        public byte cpuRead(ushort address) {
            if (address >= 0x8000 && address <= 0xbfff) {
                return getPrgPage(0)[address - 0x8000];
            } else if (address >= 0xc000 && address <= 0xffff) {
                return getPrgPage(1)[address - 0xc000];
            }
            return 0;
        }

        public void cpuWrite(ushort address, byte value) { }

        public byte ppuRead(ushort address) {
            return getChrPage(0)[address];
        }

        public void ppuWrite(ushort address, byte value) {
            if (cart.usesChrRam()) {
                getChrPage(0)[address] = value;
            }
        }

        public void tick() { }
    }
}
