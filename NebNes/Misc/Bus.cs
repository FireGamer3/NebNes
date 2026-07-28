using NebNes.APU;
using NebNes.Mappers.Interfaces;
using NebNes.PPU;


namespace NebNes.Misc {
    public class Bus {
        private byte[] wram = new byte[2048];
        private IMapper? mapper;
        private NesPPU ppu;
        private NesAPU apu;
        private Controller[]? controllers;
        private byte busValue = 0;

        public Bus(IMapper? mapper = null) {
            this.mapper = mapper;
        }

        public void Initialize(IMapper mapper, NesPPU ppu, NesAPU apu, Controller[] controllers) {
            this.mapper = mapper;
            this.ppu = ppu;
            this.apu = apu;
            this.controllers = controllers;
        }

        public byte read(ushort address) {
            // 🐏 WRAM (2 KiB) & Mirror
            if (address <= 0x1fff)
                return writeBusValue(wram[address & 0x07FF]);

            // 🖥️ PPU registers
            if (address >= 0x2000 && address <= 0x2007)
                return writeBusValue(ppu.registerRead(address));

            // 🚽 Mirrors of $2000-2007
            else if(address >= 0x2008 && address <= 0x3fff)
                return read((ushort)(0x2000 + (address - 0x2008) % 0x0008));

            // 🔊 APU registers
            if (address >= 0x4000 && address <= 0x4013)
                return apu.registers.read(address);

            // 🖥️ PPU's OAMDMA register
            if (address == 0x4014)
                return writeBusValue(ppu.registerRead(address));

            // 🔊 APUStatus register
            if (address == 0x4015)
                return apu.registers.read(address);

            // 🎮 Controller port 1
            if (address == 0x4016)
                return WriteControllerBusValue(controllers![0].OnRead());

            // 🎮 Controller port 2
            if (address == 0x4017)
                return WriteControllerBusValue(controllers![1].OnRead());

            // 💾 Cartridge space (PRG-ROM, mapper, etc.)
            else if(address >= 0x4020 && address <= 0xFFFF)
                return writeBusValue(mapper!.cpuRead(address));

            //If we fall out of the range of anything we return the last returned bus value (Open Bus)
            return busValue;
        }

        public void write(ushort address, byte value) {
            // 🐏 WRAM (2 KiB) & Mirror
            if (address <= 0x1fff)
                wram[address & 0x07FF] = value;

            // 🖥️ PPU registers
            if (address >= 0x2000 && address <= 0x2007)
                ppu.registerWrite(address, value);

            // 🚽 Mirrors of $2000-2007
            else if (address >= 0x2008 && address <= 0x3fff)
                write((ushort)(0x2000 + (address - 0x2008) % 0x0008), value);

            // 🔊 APU registers
            if (address >= 0x4000 && address <= 0x4013) {
                apu.registers.write(address, value);
                return;
            }

            // 🖥️ PPU's OAMDMA register
            if (address == 0x4014)
                ppu.registerWrite(address, value);

            // 🔊 APUStatus register
            if (address == 0x4015) {
                apu.registers.write(address, value);
                return;
            }

            // 🎮 Controller port 1
            if (address == 0x4016)
                controllers![0].OnWrite(value);

            // 🔊 APUFrameCounter register
            if (address == 0x4017) {
                apu.registers.write(address, value);
                return;
            }

            // 💾 Cartridge space (PRG-ROM, mapper, etc.)
            else if(address >= 0x4020 && address <= 0xFFFF)
                mapper!.cpuWrite(address, value);

            writeBusValue(value);
        }

        /// <summary>
        /// Read for debuggers/tracers only. Skips every mapped register (a real read there can clock
        /// the PPU, APU or a controller shift register) and leaves the open-bus latch untouched.
        /// </summary>
        public byte peek(ushort address) {
            if (address <= 0x1fff)
                return wram[address & 0x07FF];

            if (address >= 0x4020 && mapper is not null)
                return mapper.cpuRead(address);

            return busValue;
        }

        public ushort read16(ushort address) {
            byte lo = read(address);
            byte hi = read((ushort)(address + 1));
            return ByteLib.buildU16(lo, hi);
        }

        private byte writeBusValue(byte val) {
            busValue = val;
            return busValue;
        }

        private byte WriteControllerBusValue(byte value) {
            byte lo = (byte)(value & 0x0F);
            byte hi = (byte)(busValue >> 8);
            byte final = ByteLib.buildU8(lo, hi);
            busValue = final;
            return final;
        }
    }
}
