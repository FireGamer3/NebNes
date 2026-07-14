using FluentAssertions;
using NebNes.CPU.Instructions.ALU;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.ALU {
    public class IncDecRegisterTest {
        // ---- INX ----

        [Fact]
        public void INX_increments_X() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x10);
            new INX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x11);
        }

        [Fact]
        public void INX_wraps_and_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0xFF);
            new INX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void INX_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x7F);
            new INX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- INY ----

        [Fact]
        public void INY_increments_Y() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x10);
            new INY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0x11);
        }

        [Fact]
        public void INY_wraps_and_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0xFF);
            new INY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        // ---- DEX ----

        [Fact]
        public void DEX_decrements_X() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x10);
            new DEX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x0F);
        }

        [Fact]
        public void DEX_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x01);
            new DEX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void DEX_wraps_below_zero_and_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x00);
            new DEX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0xFF);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- DEY ----

        [Fact]
        public void DEY_decrements_Y() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x10);
            new DEY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0x0F);
        }

        [Fact]
        public void DEY_wraps_below_zero_and_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x00);
            new DEY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0xFF);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }
    }
}
