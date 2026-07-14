using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.CPU.Registers {
    public class Register16Bit {
        private ushort value = 0;

        public ushort get() {
            return value;
        }

        public void set(ushort value) {
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
