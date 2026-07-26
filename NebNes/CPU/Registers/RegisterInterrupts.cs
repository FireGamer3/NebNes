using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.CPU.Registers {
    public class RegisterInterrupts {
        private byte value = 0;

        public byte get() {
            return value;
        }

        public void set(byte value) {
            this.value = value;
        }

        public bool getInterrupt(InterruptIndex bit) {
            return (value & (0x01 << (int)bit)) != 0;
        }

        public void setInterrupt(InterruptIndex bit) {
            value = ByteLib.setBit(value, (int)bit);
        }

        public void clearInterrupt(InterruptIndex bit) {
            value &= (byte)~(0x01 << (int)bit);
        }
    }
}
