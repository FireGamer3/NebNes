using FluentAssertions;
using NebNes.CPU.Instructions.Branching;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Instructions.Branching {
    public class JumpTest {
        // ---- JMP ----

        [Fact]
        public void JMP_sets_pc_to_target() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.PC.set(0x8000);
            new JMP(cpu, bus).runAddress(0x9000);
            cpu.PC.get().Should().Be(0x9000);
        }

        // ---- JSR ----

        [Fact]
        public void JSR_pushes_return_address_and_jumps() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.PC.set(0x8003);
            new JSR(cpu, bus).runAddress(0x9000);

            cpu.PC.get().Should().Be(0x9000);
            cpu.SP.get().Should().Be(0xFD); // two bytes pushed
            // JSR pushes PC-1 (0x8002) high byte first, then low byte.
            bus.read(0x01FF).Should().Be(0x80); // high
            bus.read(0x01FE).Should().Be(0x02); // low
        }

        // ---- RTS ----

        [Fact]
        public void RTS_pulls_address_and_adds_one() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFD);
            bus.write(0x01FF, 0x80); // high of stored PC (0x8002)
            bus.write(0x01FE, 0x02); // low
            new RTS(cpu, bus).runImplicit();

            cpu.PC.get().Should().Be(0x8003); // stored 0x8002 + 1
            cpu.SP.get().Should().Be(0xFF);
        }

        [Fact]
        public void JSR_then_RTS_round_trips() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFF);
            cpu.PC.set(0x8003); // address of the byte after the JSR operand
            new JSR(cpu, bus).runAddress(0x9000);
            cpu.PC.get().Should().Be(0x9000);

            new RTS(cpu, bus).runImplicit();
            cpu.PC.get().Should().Be(0x8003); // back where we left off
            cpu.SP.get().Should().Be(0xFF);
        }

        // ---- RTI ----

        [Fact]
        public void RTI_restores_flags_then_pc() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFC);
            bus.write(0x01FD, 0xC1); // flags (popped first)
            bus.write(0x01FE, 0x00); // PC low
            bus.write(0x01FF, 0x90); // PC high
            new RTI(cpu, bus).runImplicit();

            cpu.Flags.get().Should().Be(0xC1);
            cpu.PC.get().Should().Be(0x9000);
            cpu.SP.get().Should().Be(0xFF);
        }

        [Fact]
        public void RTI_does_not_add_one_to_pc() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.SP.set(0xFC);
            bus.write(0x01FD, 0x00); // flags
            bus.write(0x01FE, 0x34); // PC low
            bus.write(0x01FF, 0x12); // PC high
            new RTI(cpu, bus).runImplicit();
            cpu.PC.get().Should().Be(0x1234); // exact address, unlike RTS
        }
    }
}
