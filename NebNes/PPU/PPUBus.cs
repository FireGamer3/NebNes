using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.PPU {
    public class PPUBus {
        public byte[] vram = new byte[4096];
        public byte[] paletteRam = new byte[32];
        public byte[] oamRam = new byte[256];
        public ushort[] mirroringOffsets = { 0x0, 0x0, 0x0, 0x0 };
        private PPUBgMirroring currentMirroring = (PPUBgMirroring)(-1);
        private Cart? cart;
        private IMapper? mapper;

        public void Initialize(Cart cart,  IMapper mapper) {
            this.cart = cart;
            this.mapper = mapper;
            changeNameTableMirroringTo(cart.getMirroring());
        }

        public byte read(ushort address) {
            // 🕊️ Pattern tables 0 and 1 (mapper)
            if (address <= 0x1FFF)
                return mapper!.ppuRead(address);

            // 🏞️ Name tables 0 to 3 (VRAM + mirror)
            if (address >= 0x2000 && address <= 0x2FFF) {
                refreshMirroring();
                if (address >= 0x2000 && address <= 0x23FF)
                    return vram[mirroringOffsets[0] + (address - 0x2000)];
                if (address >= 0x2400 && address <= 0x27FF)
                    return vram[mirroringOffsets[1] + (address - 0x2400)];
                if (address >= 0x2800 && address <= 0x2BFF)
                    return vram[mirroringOffsets[2] + (address - 0x2800)];
                if (address >= 0x2C00)
                    return vram[mirroringOffsets[3] + (address - 0x2C00)];
            }

            // 🚽 Mirrors of $2000-$2EFF
            if (address >= 0x3000 && address <= 0x3eff)
                return read((ushort)(0x2000 + ((address - 0x3000) % 0x1000)));

            // 🎨 Palette RAM
            if (address >= 0x3F00 && address <= 0x3F1F) {
                if (address == 0x3f10) return read(0x3f00);
                if (address == 0x3f14) return read(0x3f04);
                if (address == 0x3f18) return read(0x3f08);
                if (address == 0x3f1c) return read(0x3f0c);
                return paletteRam[address - 0x3F00];
            }

            // 🚽 Mirrors of $3F00-$3F1F
            if (address >= 0x3f20 && address <= 0x3fff)
                return read((ushort)(0x3f00 + ((address - 0x3f20) % 0x0020)));

            return 0;
        }

        public void write(ushort address, byte value) {
            // 🕊️ Pattern tables 0 and 1 (mapper)
            if (address <= 0x1FFF)
                mapper!.ppuWrite(address, value);

            // 🏞️ Name tables 0 to 3 (VRAM + mirror)
            if (address >= 0x2000 && address <= 0x2FFF) {
                refreshMirroring();
                if (address >= 0x2000 && address <= 0x23FF)
                    vram[mirroringOffsets[0] + (address - 0x2000)] = value;
                if (address >= 0x2400 && address <= 0x27FF)
                    vram[mirroringOffsets[1] + (address - 0x2400)] = value;
                if (address >= 0x2800 && address <= 0x2BFF)
                    vram[mirroringOffsets[2] + (address - 0x2800)] = value;
                if (address >= 0x2C00)
                    vram[mirroringOffsets[3] + (address - 0x2C00)] = value;
            }

            // 🚽 Mirrors of $2000-$2EFF
            if (address >= 0x3000 && address <= 0x3eff)
                write((ushort)(0x2000 + ((address - 0x3000) % 0x1000)), value);

            // 🎨 Palette RAM
            if (address >= 0x3F00 && address <= 0x3F1F) {
                if (address == 0x3f10) {
                    write(0x3f00, value);
                    return;
                }
                if (address == 0x3f14) {
                    write(0x3f04, value);
                    return;
                }
                if (address == 0x3f18) {
                    write(0x3f08, value);
                    return;
                }
                if (address == 0x3f1c) {
                    write(0x3f0c, value);
                    return;
                }
                paletteRam[address - 0x3F00] = value;
            }

            // 🚽 Mirrors of $3F00-$3F1F
            if (address >= 0x3f20 && address <= 0x3fff)
                write((ushort)(0x3f00 + ((address - 0x3f20) % 0x0020)), value);
        }

        private void changeNameTableMirroringTo(PPUBgMirroring id) {
            if(cart!.getMirroring() == PPUBgMirroring.FOUR_SCREEN) {
                mirroringOffsets = getMirroringOffsets(PPUBgMirroring.FOUR_SCREEN);
            }else {
                mirroringOffsets = getMirroringOffsets(id);
            }
        }

        // The mapper is the source of truth for mirroring (MMC1 etc. can change it
        // at runtime). Only rebuild the offset table when the mode actually changes.
        private void refreshMirroring() {
            PPUBgMirroring m = cart!.getMirroring() == PPUBgMirroring.FOUR_SCREEN
                ? PPUBgMirroring.FOUR_SCREEN
                : mapper!.getMirroring();
            if (m != currentMirroring) {
                currentMirroring = m;
                mirroringOffsets = getMirroringOffsets(m);
            }
        }

        private ushort[] getMirroringOffsets(PPUBgMirroring id) {
            switch (id) {
                case PPUBgMirroring.FOUR_SCREEN:
                    return [0x0, 0x400, 0x800, 0xC00];
                case PPUBgMirroring.VERTICAL:
                    return [0x0, 0x400, 0x0, 0x400];
                case PPUBgMirroring.HORIZONTAL:
                    return [0x0, 0x0, 0x400, 0x400];
                case PPUBgMirroring.SINGLE_SCREEN_LOW:
                    return [0x0, 0x0, 0x0, 0x0];
                case PPUBgMirroring.SINGLE_SCREEN_HIGH:
                    return [0x400, 0x400, 0x400, 0x400];
                default:
                    return [0x0, 0x0, 0x0, 0x0];
            }
        }
    }
}
