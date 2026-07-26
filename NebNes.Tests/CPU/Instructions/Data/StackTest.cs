using FluentAssertions;
using NebNes.CPU.Instructions.Data;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Data {
    public class StackTest {
        // ---- PHA ----

        [Fact]
        public void PHA_pushes_accumulator_and_decrements_sp() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.A.set(0x42);
            new PHA(cpu, bus).runImplicit();
            bus.read(0x01FF).Should().Be(0x42); // stack lives at $0100 + SP
            cpu.SP.get().Should().Be(0xFE);
        }

        // ---- PLA ----

        [Fact]
        public void PLA_pulls_accumulator_and_increments_sp() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFE);
            bus.write(0x01FF, 0x42); // value sitting on the stack
            new PLA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x42);
            cpu.SP.get().Should().Be(0xFF);
        }

        [Fact]
        public void PLA_sets_zero() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFE);
            cpu.A.set(0x55);
            bus.write(0x01FF, 0x00);
            new PLA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x00);
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeTrue();
        }

        [Fact]
        public void PLA_sets_negative() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFE);
            bus.write(0x01FF, 0x80);
            new PLA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x80);
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }

        [Fact]
        public void PHA_then_PLA_round_trips() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.A.set(0x99);
            new PHA(cpu, bus).runImplicit();
            cpu.A.set(0x00); // clobber so the pull has to restore it
            new PLA(cpu, bus).runImplicit();
            cpu.A.get().Should().Be(0x99);
            cpu.SP.get().Should().Be(0xFF);
        }

        // ---- PHP ----

        [Fact]
        public void PHP_pushes_flags_with_break_bit_set() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.Flags.setFlag(FlagsIndex.N);
            new PHP(cpu, bus).runImplicit();
            byte pushed = bus.read(0x01FF);
            (pushed & (1 << (int)FlagsIndex.B)).Should().NotBe(0);   // B (bit 4) forced on
            (pushed & (1 << (int)FlagsIndex.C)).Should().NotBe(0);   // preserved
            (pushed & (1 << (int)FlagsIndex.N)).Should().NotBe(0);   // preserved
            cpu.SP.get().Should().Be(0xFE);
        }

        // ---- PLP ----

        [Fact]
        public void PLP_restores_flags_from_stack() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFE);
            bus.write(0x01FF, 0b1000_0001); // N and C set
            new PLP(cpu, bus).runImplicit();
            cpu.Flags.getFlag(FlagsIndex.C).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.N).Should().BeTrue();
            cpu.Flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            cpu.SP.get().Should().Be(0xFF);
        }

        [Fact]
        public void PHP_then_PLP_round_trips_pushed_state() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.Flags.setFlag(FlagsIndex.C);
            cpu.Flags.setFlag(FlagsIndex.V);
            byte pushedState = 0;
            new PHP(cpu, bus).runImplicit();
            pushedState = bus.read(0x01FF);
            cpu.Flags.set(0x00); // wipe flags
            new PLP(cpu, bus).runImplicit();
            // PLP drops the pushed B flag (bit 4) and forces the unused bit 5 to 1.
            cpu.Flags.get().Should().Be((byte)((pushedState & 0xEF) | 0x20));
        }
    }
}
