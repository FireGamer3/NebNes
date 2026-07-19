using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class BMI : Instruction, IInstruction {
        public BMI(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { 
            if(cpu.Flags.getFlag(Enums.FlagsIndex.N)) {
                cpu.addExtraCycles();
                cpu.PC.set(address);
            }else cpu.clearExtraCycles();
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
