using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class LAX : Instruction, IInstruction {
        public LAX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            cpu.A.set(value);
            cpu.X.set(value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImplicit() { }
    }
}
