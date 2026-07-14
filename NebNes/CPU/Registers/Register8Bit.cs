using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.CPU.Registers {
    public class Register8Bit {
        private byte value = 0;

        public byte get() {
            return value;
        }

        public void set(byte value) {
            this.value = value;
        }

        public void increment() {
            value += 1;
        }

        public void decrement() {
            value -= 1;
        }
    }
}
