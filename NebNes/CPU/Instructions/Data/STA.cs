using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class STA : Instruction, IInstruction {
        public STA(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            bus.write(address, cpu.A.get());
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
