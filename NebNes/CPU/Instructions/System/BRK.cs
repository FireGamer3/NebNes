using NebNes.Misc;

namespace NebNes.CPU.Instructions.System {
    public class BRK : Instruction, IInstruction {
        public BRK(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.triggerInterrupt(Enums.InterruptIndex.BRK);
        }
    }
}
