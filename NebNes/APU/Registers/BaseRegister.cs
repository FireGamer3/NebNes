namespace NebNes.APU.Registers {
    public class BaseRegister {
        protected NesAPU apu;
        protected int id;
        protected byte value = 0;

        public BaseRegister(NesAPU apu, int id) {
            this.apu = apu;
            this.id = id;
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
