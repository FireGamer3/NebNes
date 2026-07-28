using System;
using System.Collections.Generic;
using System.Text;
using NebNes.Enums;

namespace NebNes.Mappers.Interfaces {
    public interface IMapper {
        public void onLoad();
        public byte cpuRead(ushort address);
        public void cpuWrite(ushort address, byte value);
        public byte ppuRead(ushort address);
        public void ppuWrite(ushort address, byte value);
        public void tick();
        public PPUBgMirroring getMirroring();
    }
}
