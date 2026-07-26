using NebNes.CPU;
using NebNes.Misc;

namespace NebNes.Mappers {
    public static class MapperFactory {

        public static IMapper CreateMapper(MOS6502 cpu, Cart cart) {
            switch(cart.getMapperID()) {
                case 0:
                    return new NROM(cpu, cart);
                case 1:
                    return new MMC1(cpu, cart);
                case 2:
                    return new UxROM(cpu, cart);
                case 3:
                    return new CNROM(cpu, cart);
                default:
                    throw new ArgumentException("Mapper with ID: " +  cart.getMapperID() + ", Not Found");
            }
        }
    }
}
