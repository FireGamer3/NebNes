using FluentAssertions;
using NebNes.CPU.Instructions.Branching;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Branching {
    public class ConditionalBranchTest {
        // A conditional branch redirects PC to the target address when its condition
        // holds, and leaves PC untouched otherwise. The pre-decoded target is passed
        // straight to runAddress.
        private const ushort Origin = 0x8000;
        private const ushort Target = 0x9000;

        // ---- BCC: branch if carry clear ----

        [Fact]
        public void BCC_taken_when_carry_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.C);
            new BCC(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BCC_not_taken_when_carry_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.C);
            new BCC(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BCS: branch if carry set ----

        [Fact]
        public void BCS_taken_when_carry_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.C);
            new BCS(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BCS_not_taken_when_carry_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.C);
            new BCS(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BEQ: branch if zero set ----

        [Fact]
        public void BEQ_taken_when_zero_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.Z);
            new BEQ(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BEQ_not_taken_when_zero_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.Z);
            new BEQ(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BNE: branch if zero clear ----

        [Fact]
        public void BNE_taken_when_zero_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.Z);
            new BNE(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BNE_not_taken_when_zero_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.Z);
            new BNE(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BMI: branch if negative set ----

        [Fact]
        public void BMI_taken_when_negative_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.N);
            new BMI(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BMI_not_taken_when_negative_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.N);
            new BMI(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BPL: branch if negative clear ----

        [Fact]
        public void BPL_taken_when_negative_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.N);
            new BPL(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BPL_not_taken_when_negative_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.N);
            new BPL(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BVC: branch if overflow clear ----

        [Fact]
        public void BVC_taken_when_overflow_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.V);
            new BVC(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BVC_not_taken_when_overflow_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.V);
            new BVC(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }

        // ---- BVS: branch if overflow set ----

        [Fact]
        public void BVS_taken_when_overflow_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.setFlag(FlagsIndex.V);
            new BVS(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Target);
        }

        [Fact]
        public void BVS_not_taken_when_overflow_clear() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(Origin);
            cpu.Flags.clearFlag(FlagsIndex.V);
            new BVS(cpu, bus).runAddress(Target);
            cpu.PC.get().Should().Be(Origin);
        }
    }
}
