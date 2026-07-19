using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class STX : Instruction, IInstruction {
        public STX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            bus.write(address, cpu.X.get());
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
