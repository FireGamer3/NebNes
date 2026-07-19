using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class SEI : Instruction, IInstruction {
        public SEI(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.Flags.setFlag(Enums.FlagsIndex.I);
        }
    }
}
