using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    internal class MMC5 : Mapper, IMapper {
        private byte[] prgRam = new byte[0x20000];
        private byte prgMode = 3;
        public MMC5(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

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

        public void tick() { }
    }
}
