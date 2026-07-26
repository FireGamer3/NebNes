namespace NebNes.PPU.Registers {
    public class PPUData : BaseRegister {
        private byte buffer = 0;

        public PPUData(NesPPU ppu) : base(ppu) { }

        public override byte onRead() {
            byte data = buffer;
            ushort address = ppu.ppuAddr.getAddress();
            buffer = ppu.bus.read(address);
            if(address >= 0x3F00 && address <= 0x3FFF) {
                data = buffer;
            }
            incrementAddress();
            return data;
        }

        public override void onWrite(byte value) {
            ppu.bus.write(ppu.ppuAddr.getAddress(), value);
            incrementAddress();
        }

        private void incrementAddress() {
            if(ppu.ppuCtrl.vramAddressIncrement32() == 1) {
                ppu.ppuAddr.setAddress((ushort)(ppu.ppuAddr.getAddress() + 32));
            }else {
                ppu.ppuAddr.setAddress((ushort)(ppu.ppuAddr.getAddress() + 1));
            }
        }
    }
}
