using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class PHP : Instruction, IInstruction {
        public PHP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte flags = cpu.Flags.get();
            cpu.stack.push(ByteLib.setBit(flags, 4));
        }
    }
}
