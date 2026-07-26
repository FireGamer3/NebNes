namespace NebNes.PPU.Registers {
    public class BaseRegister {
        protected NesPPU ppu;
        protected byte value = 0;

        public BaseRegister(NesPPU ppu) {
            this.ppu = ppu;
        }

        public virtual void onLoad() {

        }

        public virtual byte onRead() {
            return 0;
        }

        public virtual void onWrite(byte value) {

        }

        public void set(byte value) {
            this.value = value;
        }

        public byte getValue() {
            return value;
        }
    }
}
