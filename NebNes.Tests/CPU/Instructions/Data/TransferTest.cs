using FluentAssertions;
using NebNes.CPU.Instructions.Data;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Data {
    public class TransferTest {
        // ---- TAX ----

        [Fact]
        public void TAX_copies_a_to_x() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x42);
            new TAX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x42);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void TAX_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x55);
            cpu.A.set(0x00);
            new TAX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void TAX_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x80);
            new TAX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- TAY ----

        [Fact]
        public void TAY_copies_a_to_y() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x42);
            new TAY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0x42);
        }

        [Fact]
        public void TAY_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x90);
            new TAY(cpu, bus).runImplicit();
            cpu.Y.get().Should().Be(0x90);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- TXA ----

        [Fact]
        public void TXA_copies_x_to_a() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x42);
            new TXA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x42);
        }

        [Fact]
        public void TXA_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x55);
            cpu.X.set(0x00);
            new TXA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        // ---- TYA ----

        [Fact]
        public void TYA_copies_y_to_a() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x42);
            new TYA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x42);
        }

        [Fact]
        public void TYA_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x80);
            new TYA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- TSX ----

        [Fact]
        public void TSX_copies_sp_to_x() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFD);
            new TSX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0xFD);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue(); // 0xFD has bit 7 set
        }

        [Fact]
        public void TSX_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0x00);
            new TSX(cpu, bus).runImplicit();
            cpu.X.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        // ---- TXS ----

        [Fact]
        public void TXS_copies_x_to_sp() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0xFD);
            new TXS(cpu, bus).runImplicit();
            cpu.SP.get().Should().Be(0xFD);
        }

        [Fact]
        public void TXS_does_not_affect_flags() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x00);
            byte before = cpu.Flags.get();
            new TXS(cpu, bus).runImplicit();
            cpu.Flags.get().Should().Be(before); // TXS is the only transfer that leaves flags alone
        }
    }
}
