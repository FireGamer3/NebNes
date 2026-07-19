using FluentAssertions;
using NebNes.CPU.Instructions.Data;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Data {
    public class LoadTest {
        // ---- LDA ----

        [Fact]
        public void LDA_loads_accumulator() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDA(cpu, bus).runImmediate(0x42);
            cpu.A.get().Should().Be(0x42);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void LDA_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x55);
            new LDA(cpu, bus).runImmediate(0x00);
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void LDA_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDA(cpu, bus).runImmediate(0x80);
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
        }

        // ---- LDX ----

        [Fact]
        public void LDX_loads_x() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDX(cpu, bus).runImmediate(0x42);
            cpu.X.get().Should().Be(0x42);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void LDX_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x55);
            new LDX(cpu, bus).runImmediate(0x00);
            cpu.X.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void LDX_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDX(cpu, bus).runImmediate(0xFF);
            cpu.X.get().Should().Be(0xFF);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- LDY ----

        [Fact]
        public void LDY_loads_y() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDY(cpu, bus).runImmediate(0x42);
            cpu.Y.get().Should().Be(0x42);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void LDY_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x55);
            new LDY(cpu, bus).runImmediate(0x00);
            cpu.Y.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void LDY_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            new LDY(cpu, bus).runImmediate(0x80);
            cpu.Y.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }
    }
}
