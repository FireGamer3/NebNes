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
    }
}
