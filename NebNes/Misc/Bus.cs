using NebNes.Mappers;


namespace NebNes.Misc {
    public class Bus {
        private byte[] wram = new byte[2048];
        private IMapper mapper;
        private byte busValue = 0;

        public Bus(IMapper mapper) { 
            this.mapper = mapper;
        }

        public byte read(ushort address) {
            // 🐏 WRAM (2 KiB) & Mirror
            if (address <= 0x1fff)
                return writeBusValue(wram[address & 0x07FF]);

            // 🖥️ PPU registers
            //TODO

            // 🚽 Mirrors of $2000-2007
            else if(address >= 0x2008 && address <= 0x3fff)
                return read((ushort)(0x2000 + (address - 0x2008) % 0x0008));

            // 🔊 APU registers
            //TODO
            // 🖥️ PPU's OAMDMA register
            //TODO
            // 🔊 APUStatus register
            //TODO
            // 🎮 Controller port 1
            //TODO
            // 🎮 Controller port 2
            //TODO

            // 💾 Cartridge space (PRG-ROM, mapper, etc.)
            else if(address >= 0x4020 && address <= 0xFFFF)
                return writeBusValue(mapper.cpuRead(address));

            //If we fall out of the range of anything we return the last returned bus value (Open Bus)
            return busValue;
        }

        public void write(ushort address, byte value) {
            // 🐏 WRAM (2 KiB) & Mirror
            if (address <= 0x1fff)
                wram[address & 0x07FF] = value;

            // 🖥️ PPU registers
            //TODO

            // 🚽 Mirrors of $2000-2007
            else if (address >= 0x2008 && address <= 0x3fff)
                write((ushort)(0x2000 + (address - 0x2008) % 0x0008), value);

            // 🔊 APU registers
            //TODO
            // 🖥️ PPU's OAMDMA register
            //TODO
            // 🔊 APUStatus register
            //TODO
            // 🎮 Controller port 1
            //TODO
            // 🔊 APUFrameCounter register
            //TODO

            // 💾 Cartridge space (PRG-ROM, mapper, etc.)
            else if(address >= 0x4020 && address <= 0xFFFF)
                mapper.cpuWrite(address, value);

            writeBusValue(value);
        }

        private byte writeBusValue(byte val) {
            busValue = val;
            return busValue;
        }
    }
}
