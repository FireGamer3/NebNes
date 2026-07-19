using NebNes.CPU;
using NebNes.Misc;

namespace NebNes.Tests.CPU.Instructions.Branching {
    internal static class Cpu6502 {
        /// <summary>
        /// Builds a fresh CPU + Bus for instruction tests. The mapper is null because
        /// the Branching instructions only touch the PC, flags and stack ($0100-$01FF),
        /// which the Bus services directly without consulting the cartridge.
        /// </summary>
        public static (MOS6502 cpu, Bus bus) Fresh() {
            Bus bus = new Bus(null!);
            return (new MOS6502(bus), bus);
        }
    }
}
