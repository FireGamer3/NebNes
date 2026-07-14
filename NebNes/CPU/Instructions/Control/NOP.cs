using NebNes.Misc;

namespace NebNes.CPU.Instructions.Control {
    public class NOP : Instruction, IInstruction {
        public NOP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        public void runValue(ushort address, byte value) { }
    }
}
