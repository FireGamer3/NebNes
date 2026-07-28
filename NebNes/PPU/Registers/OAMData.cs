namespace NebNes.PPU.Registers {
    public class OAMData : BaseRegister {
        public OAMData(NesPPU ppu) : base(ppu) { }

        public override byte onRead() {
            byte oamAddress = ppu.oamAddr.getValue();
            return ppu.bus.oamRam[oamAddress];
        }

        public override void onWrite(byte value) {
            byte oamAddress = ppu.oamAddr.getValue();
            ppu.bus.oamRam[oamAddress] = value;
            ppu.oamAddr.set((byte)(oamAddress + 1));
        }
    }
}
