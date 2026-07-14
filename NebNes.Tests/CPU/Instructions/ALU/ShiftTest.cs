using FluentAssertions;
using NebNes.CPU.Instructions.ALU;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.ALU {
    public class ShiftTest {
        // ---- ASL ----

        [Fact]
        public void ASL_acc_shifts_left() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x40);
            new ASL(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();  // result bit 7
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
        }

        [Fact]
        public void ASL_acc_sets_carry_and_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x80);
            new ASL(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void ASL_address_shifts_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            bus.write(0x0010, 0x40);
            new ASL(cpu, bus).runAddress(0x0010);
            bus.read(0x0010).Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        // ---- LSR ----

        [Fact]
        public void LSR_acc_shifts_right() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x02);
            new LSR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x01);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void LSR_acc_sets_carry_and_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x01);
            new LSR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void LSR_always_clears_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0xFF);
            new LSR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x7F);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse(); // bit 7 always 0 after LSR
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
        }

        // ---- ROL ----

        [Fact]
        public void ROL_rotates_carry_into_bit0() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.A.set(0x01);
            new ROL(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x03); // (0x01 << 1) | carry
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
        }

        [Fact]
        public void ROL_sets_carry_out_and_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x80);
            new ROL(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void ROL_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x40);
            new ROL(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
        }

        // ---- ROR ----

        [Fact]
        public void ROR_rotates_carry_into_bit7() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.A.set(0x00);
            new ROR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x80); // carry rotated into bit 7
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        [Fact]
        public void ROR_sets_carry_out_and_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x01);
            new ROR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void ROR_basic_shift() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x02);
            new ROR(cpu, bus).runAcc();
            cpu.A.get().Should().Be(0x01);
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }
    }
}
