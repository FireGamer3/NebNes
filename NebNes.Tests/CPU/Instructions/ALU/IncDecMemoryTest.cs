using FluentAssertions;
using NebNes.CPU.Instructions.ALU;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.ALU {
    public class IncDecMemoryTest {
        // ---- INC ----

        [Fact]
        public void INC_increments_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0x0F);
            new INC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0x10);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void INC_wraps_to_zero_and_sets_zero_flag() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0xFF);
            new INC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void INC_sets_negative_flag() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0x7F);
            new INC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- DEC ----

        [Fact]
        public void DEC_decrements_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0x10);
            new DEC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0x0F);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void DEC_sets_zero_flag() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0x01);
            new DEC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void DEC_wraps_below_zero_and_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0020, 0x00);
            new DEC(cpu, bus).runAddress(0x0020);
            bus.read(0x0020).Should().Be(0xFF);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }
    }
}
