using NebNes.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace NebNes.CPU.Instructions {
    public class Instruction {
        protected MOS6502 cpu;
        protected Bus bus;

        public Instruction(MOS6502 cpu, Bus bus) {
            this.cpu = cpu;
            this.bus = bus;
        }

        // Taken branch: +1 cycle, +1 more if the target is on a different page than the
        // next instruction (PC already points past the operand here).
        protected void takeBranch(ushort address) {
            cpu.addExtraCycles();
            if ((cpu.PC.get() & 0xFF00) != (address & 0xFF00)) cpu.addExtraCycles();
            cpu.PC.set(address);
        }
    }
}
