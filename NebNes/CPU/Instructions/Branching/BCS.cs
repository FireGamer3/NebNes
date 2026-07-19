using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class BCS : Instruction, IInstruction {
        public BCS(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { 
            if(cpu.Flags.getFlag(Enums.FlagsIndex.C)) {
                cpu.addExtraCycles();
                cpu.PC.set(address);
            }else cpu.clearExtraCycles();
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
