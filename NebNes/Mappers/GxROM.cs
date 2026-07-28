using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class GxROM : Mapper, IMapper {
        byte chrRomBank = 0;
        byte prgRomBank = 0;
        public GxROM(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

        public byte cpuRead(ushort address) {
            throw new NotImplementedException();
        }

        public void cpuWrite(ushort address, byte value) {
            throw new NotImplementedException();
        }

        public byte ppuRead(ushort address) {
            throw new NotImplementedException();
        }

        public void ppuWrite(ushort address, byte value) {
            throw new NotImplementedException();
        }

        public void tick() {
            throw new NotImplementedException();
        }
    }
}
