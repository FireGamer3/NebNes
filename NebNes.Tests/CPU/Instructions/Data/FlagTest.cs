using FluentAssertions;
using NebNes.CPU.Instructions.Data;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Data {
    public class FlagTest {
        // ---- Set flag instructions ----

        [Fact]
        public void SEC_sets_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            new SEC(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
        }

        [Fact]
        public void SED_sets_decimal() {
            var (cpu, bus) = Cpu6502.Fresh();
            new SED(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.D).Should().BeTrue();
        }

        [Fact]
        public void SEI_sets_interrupt_disable() {
            var (cpu, bus) = Cpu6502.Fresh();
            new SEI(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.I).Should().BeTrue();
        }

        // ---- Clear flag instructions ----

        [Fact]
        public void CLC_clears_carry() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            new CLC(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
        }

        [Fact]
        public void CLD_clears_decimal() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.D);
            new CLD(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.D).Should().BeFalse();
        }

        [Fact]
        public void CLI_clears_interrupt_disable() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.I);
            new CLI(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.I).Should().BeFalse();
        }

        [Fact]
        public void CLV_clears_overflow() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.V);
            new CLV(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.V).Should().BeFalse();
        }

        // ---- Only the targeted flag changes ----

        [Fact]
        public void SEC_leaves_other_flags_untouched() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.N);
            cpu.Flags.setFlag(FlagsIndex.Z);
            new SEC(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void CLC_leaves_other_flags_untouched() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.Flags.setFlag(FlagsIndex.N);
            new CLC(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeFalse();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }
    }
}
