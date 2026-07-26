using NebNes.Misc;

namespace NebNes.PPU.Registers {
    public class OAMDMA : BaseRegister {
        public OAMDMA(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte value) {
            for (int i = 0; i < 256; i++) {
                ushort address = ByteLib.buildU16((byte)i, value);
                ppu.bus.oamRam[i] = ppu.cpuBus.read(address);
            }
            ppu.cpu.addExtraCycles(513);
        }
    }
}
