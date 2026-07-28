using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class SAX : Instruction, IInstruction {
        public SAX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte res = (byte)(cpu.A.get() & cpu.X.get());
            bus.write(address, res);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
