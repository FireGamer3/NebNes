using NebNes.CPU;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class CNROM : Mapper, IMapper {
        private byte[] prgRam = new byte[2048];
        private byte chrBank = 0;

        public CNROM(MOS6502 cpu, Cart cart) : base(cpu, cart) { }

        public byte cpuRead(ushort address) {
            if(address >= 0x6000 && address <= 0x7FFF && cart.hasPrgRam())
                return prgRam[(address - 0x6000) % 2048];
            if (address >= 0x8000 && address <= 0xBFFF)
                return getPrgPage(0)[address - 0x8000];
            if (address >= 0xC000)
                return getPrgPage(1)[address - 0xC000];
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x6000 && address <= 0x7FFF && cart.hasPrgRam())
                prgRam[(address - 0x6000) % 2048] = value;
            if (address >= 0x8000)
                chrBank = ByteLib.getBits((byte)(value & prg[address - 0x8000]), 0, 4);
        }

        public byte ppuRead(ushort address) {
            return getChrPage(chrBank)[address];
        }

        public void ppuWrite(ushort address, byte value) {
            if(cart.usesChrRam())
                getChrPage(chrBank)[address] = value;
        }

        public void tick() { }
    }
}
