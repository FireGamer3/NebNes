namespace NebNes.PPU.Registers {
    public class PPUScroll : BaseRegister {
        public PPUScroll(NesPPU ppu) : base(ppu) { }

        public override void onWrite(byte value) {
            ppu.loopy.onPPUScrollWrite(value);
        }
    }
}
