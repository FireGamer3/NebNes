using NebNes.CPU.Registers;
using NebNes.Misc;

namespace NebNes.CPU {
    public class MOS6502 {
        private Bus bus;
        public Register8Bit A = new Register8Bit();
        public Register8Bit X = new Register8Bit();
        public Register8Bit Y = new Register8Bit();
        public Register8Bit SP = new Register8Bit();
        public Register16Bit PC = new Register16Bit();
        public RegisterFlags Flags = new RegisterFlags();

        public MOS6502(Bus bus) {
            this.bus = bus;

        }
    }
}
