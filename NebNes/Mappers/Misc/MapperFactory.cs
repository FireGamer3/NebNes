using NebNes.CPU;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers.Misc {
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
                case 4:
                    return new MMC3(cpu, cart);
                case 66:
                    return new GxROM(cpu, cart);
                default:
                    throw new ArgumentException("Mapper with ID: " +  cart.getMapperID() + ", Not Found");
            }
        }
    }
}
