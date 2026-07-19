using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class STY : Instruction, IInstruction {
        public STY(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            bus.write(address, cpu.Y.get());
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
