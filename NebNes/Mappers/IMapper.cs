using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.Mappers {
    public interface IMapper {
        public void onLoad();
        public byte cpuRead(ushort address);
        public void cpuWrite(ushort address, byte value);
        public byte ppuRead(ushort address);
        public void ppuWrite(ushort address, byte value);
        public void tick();
    }
}
