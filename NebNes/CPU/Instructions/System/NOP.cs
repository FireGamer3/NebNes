using NebNes.Misc;

namespace NebNes.CPU.Instructions.System {
    public class NOP : Instruction, IInstruction {
        public NOP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            bus.read(address);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
