using FluentAssertions;
using NebNes.CPU.Instructions.Checks;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Checks {
    public class CompareTest {
        // A compare sets: Z when reg == value, C when reg >= value (unsigned),
        // N from bit 7 of the (reg - value) result.

        // ---- CMP ----

        [Fact]
        public void CMP_greater_sets_carry_only() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x50);
            new CMP(cpu, bus).runImmediate(0x30);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void CMP_equal_sets_zero_and_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x50);
            new CMP(cpu, bus).runImmediate(0x50);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void CMP_less_clears_carry_and_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x50);
            new CMP(cpu, bus).runImmediate(0x60);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue(); // 0x50-0x60 = 0xF0
        }

        [Fact]
        public void CMP_result_positive_high_bit_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x80);
            new CMP(cpu, bus).runImmediate(0x01);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();  // 0x80 >= 0x01
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse(); // 0x80-0x01 = 0x7F
        }

        // ---- CPX ----

        [Fact]
        public void CPX_equal_sets_zero_and_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x20);
            new CPX(cpu, bus).runImmediate(0x20);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
        }

        [Fact]
        public void CPX_less_clears_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x10);
            new CPX(cpu, bus).runImmediate(0x20);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- CPY ----

        [Fact]
        public void CPY_greater_sets_carry_only() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x40);
            new CPY(cpu, bus).runImmediate(0x30);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void CPY_equal_sets_zero_and_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x40);
            new CPY(cpu, bus).runImmediate(0x40);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
        }
    }
}
