using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class BVS : Instruction, IInstruction {
        public BVS(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { 
            if(cpu.Flags.getFlag(Enums.FlagsIndex.V)) {
                cpu.addExtraCycles();
                cpu.PC.set(address);
            }else cpu.clearExtraCycles();
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
