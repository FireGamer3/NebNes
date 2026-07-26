using NebNes.Misc;

namespace NebNes.CPU.Instructions.Checks {
    public class AND : Instruction, IInstruction {
        public AND(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            byte val = (byte)(cpu.A.get() & value);
            cpu.A.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }

        public void runImplicit() { }
    }
}
