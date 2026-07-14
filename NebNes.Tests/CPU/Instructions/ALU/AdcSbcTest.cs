using FluentAssertions;
using NebNes.CPU.Instructions.ALU;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.ALU {
    public class AdcSbcTest {
        // ---- ADC ----

        [Fact]
        public void ADC_adds_operand_to_accumulator() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x10);
            new ADC(cpu, bus).runImmediate(0x20);
            cpu.A.get().Should().Be(0x30);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeFalse();
        }

        [Fact]
        public void ADC_includes_carry_in() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.A.set(0x00);
            new ADC(cpu, bus).runImmediate(0x00);
            cpu.A.get().Should().Be(0x01);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
        }

        [Fact]
        public void ADC_sets_carry_and_zero_on_unsigned_overflow() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xFF);
            new ADC(cpu, bus).runImmediate(0x01);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void ADC_sets_overflow_on_signed_overflow() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x50);
            new ADC(cpu, bus).runImmediate(0x50); // 80 + 80 = 160 (signed overflow)
            cpu.A.get().Should().Be(0xA0);
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
        }

        // ---- SBC ----

        [Fact]
        public void SBC_subtracts_with_carry_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C); // carry set => no borrow
            cpu.A.set(0x50);
            new SBC(cpu, bus).runImmediate(0x10);
            cpu.A.get().Should().Be(0x40);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue(); // no borrow
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
        }

        [Fact]
        public void SBC_borrow_clears_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.A.set(0x10);
            new SBC(cpu, bus).runImmediate(0x20); // 0x10 - 0x20 = -0x10
            cpu.A.get().Should().Be(0xF0);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse(); // borrow occurred
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        [Fact]
        public void SBC_equal_values_yields_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.A.set(0x40);
            new SBC(cpu, bus).runImmediate(0x40);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
        }
    }
}
