using NebNes.CPU.Registers;
using NebNes.Misc;

namespace NebNes.CPU.Misc {
    public class Stack {
        Bus bus;
        BaseRegister sp;

        public Stack(Bus bus, BaseRegister sp) {
            this.bus = bus;
            this.sp = sp;
        }

        public ushort currentAddress() {
            return (ushort)(0x0100 + sp.get());
        }

        public void push(byte value) {
            bus.write(currentAddress(), value);
            sp.decrement();
        }

        public byte pop() {
            sp.increment();
            return bus.read(currentAddress());
        }

        public void push(ushort value) {
            byte low = (byte)(value & 0xFF);
            byte high = (byte)(value >> 8);
            push(high);
            push(low);
        }

        public ushort pop16() {
            byte low = pop();
            byte high = pop();
            return (ushort)(((high & 0xff) << 8) | (low & 0xff));
        }
    }
}
