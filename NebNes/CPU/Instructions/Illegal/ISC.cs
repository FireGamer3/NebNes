using NebNes.CPU.Instructions.ALU;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class ISC : Instruction, IInstruction {
        SBC sbc;
        public ISC(MOS6502 cpu, Bus bus) : base(cpu, bus) {
            sbc = new SBC(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            value += 1;
            bus.write(address, value);
            cpu.Flags.updateZeroAndNegative(value);
            sbc.runImmediate(value);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
