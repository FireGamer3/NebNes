using FluentAssertions;
using NebNes.CPU.Instructions.Checks;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Checks {
    public class LogicTest {
        // ---- AND ----

        [Fact]
        public void AND_masks_accumulator() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xCC);
            new AND(cpu, bus).runImmediate(0x0F);
            cpu.A.get().Should().Be(0x0C);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void AND_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xF0);
            new AND(cpu, bus).runImmediate(0x0F);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void AND_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xFF);
            new AND(cpu, bus).runImmediate(0x80);
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- EOR ----

        [Fact]
        public void EOR_xors_accumulator() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xCC);
            new EOR(cpu, bus).runImmediate(0x0F);
            cpu.A.get().Should().Be(0xC3);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        [Fact]
        public void EOR_with_self_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xA5);
            new EOR(cpu, bus).runImmediate(0xA5);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        // ---- ORA ----

        [Fact]
        public void ORA_ors_accumulator() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xC0);
            new ORA(cpu, bus).runImmediate(0x0F);
            cpu.A.get().Should().Be(0xCF);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        [Fact]
        public void ORA_zero_operands_set_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x00);
            new ORA(cpu, bus).runImmediate(0x00);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }
    }
}
