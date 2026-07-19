using FluentAssertions;
using NebNes.CPU.Instructions.Checks;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Checks {
    public class BitTest {
        // BIT sets Z from (A & M) == 0, and copies M bit 7 into N and M bit 6 into V.
        // A is never modified.

        [Fact]
        public void BIT_zero_result_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x0F);
            new BIT(cpu, bus).runImmediate(0xF0); // 0x0F & 0xF0 == 0
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.A.get().Should().Be(0x0F); // accumulator untouched
        }

        [Fact]
        public void BIT_nonzero_result_clears_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x01);
            new BIT(cpu, bus).runImmediate(0x01); // 0x01 & 0x01 != 0
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
        }

        [Fact]
        public void BIT_copies_memory_bit7_into_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xFF);
            new BIT(cpu, bus).runImmediate(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeFalse();
        }

        [Fact]
        public void BIT_copies_memory_bit6_into_overflow() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xFF);
            new BIT(cpu, bus).runImmediate(0x40);
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void BIT_clears_n_and_v_when_memory_bits_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.N);
            cpu.Flags.setFlag(FlagsIndex.V);
            cpu.A.set(0xFF);
            new BIT(cpu, bus).runImmediate(0x3F); // bits 7 and 6 clear
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeFalse();
        }
    }
}
