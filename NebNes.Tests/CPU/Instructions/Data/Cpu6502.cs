using NebNes.CPU;
using NebNes.Misc;

namespace NebNes.Tests.CPU.Instructions.Data {
    internal static class Cpu6502 {
        /// <summary>
        /// Builds a fresh CPU + Bus for instruction tests. The mapper is null because
        /// the Data instructions only touch zero-page/WRAM/stack ($0000-$1FFF), which the
        /// Bus services directly without consulting the cartridge.
        /// </summary>
        public static (MOS6502 cpu, Bus bus) Fresh() {
            Bus bus = new Bus(null!);
            return (new MOS6502(bus), bus);
        }
    }
}
