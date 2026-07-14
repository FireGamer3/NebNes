using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.CPU.Registers {
    public class RegisterFlags {
        private byte value = 0;

        public byte get() {
            return value;
        }

        public void set(byte value) {
            this.value = value;
        }

        public bool getFlag(FlagsIndex bit) {
            return (value & (0x01 << (int)bit)) == 1;
        }

        public void setFlag(FlagsIndex bit) {
            value = ByteLib.setBit(value, (int)bit);
        }

        public void clearFlag(FlagsIndex bit) {
            value &= (byte)~(0x01 << (int)bit);
        }

        public void updateZero(byte value) {
            this.value = (byte)(value == 0 ? ByteLib.setBit(this.value, 2) : (this.value & 0b11111101));
        }

        public void updateNegative(byte value) {
            this.value = (byte)(ByteLib.getFlag(value, 7) ? ByteLib.setBit(this.value, 7) : (this.value & 0b01111111));
        }

        public void updateZeroAndNegative(byte value) {
            updateZero(value);
            updateNegative(value);
        }
    }
}
