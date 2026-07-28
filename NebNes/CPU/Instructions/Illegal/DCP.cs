using NebNes.CPU.Instructions.Checks;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class DCP : Instruction, IInstruction {
        CMP cmp;
        public DCP(MOS6502 cpu, Bus bus) : base(cpu, bus) {
            cmp = new CMP(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            value -= 1;
            bus.write(address, value);
            cpu.Flags.updateZeroAndNegative(value);
            cmp.runImmediate(value);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
