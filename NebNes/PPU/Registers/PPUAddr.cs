namespace NebNes.PPU.Registers {
    public class PPUAddr : BaseRegister {
        public PPUAddr(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte value) {
            ppu.loopy.onPPUAddrWrite(value);
        }

        public ushort getAddress() {
            return ppu.loopy.vAddress.getValue();
        }

        public void setAddress(ushort addr) {
            ppu.loopy.vAddress.setValue(addr);
        }
    }
}
