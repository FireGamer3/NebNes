using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TXS : Instruction, IInstruction {
        public TXS(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.X.get();
            cpu.SP.set(val);
        }
    }
}
