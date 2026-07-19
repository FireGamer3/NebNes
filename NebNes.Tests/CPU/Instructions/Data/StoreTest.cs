using FluentAssertions;
using NebNes.CPU.Instructions.Data;

namespace NebNes.Tests.CPU.Instructions.Data {
    public class StoreTest {
        [Fact]
        public void STA_writes_accumulator_to_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x37);
            new STA(cpu, bus).runAddress(0x0010);
            bus.read(0x0010).Should().Be(0x37);
        }

        [Fact]
        public void STX_writes_x_to_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.X.set(0x37);
            new STX(cpu, bus).runAddress(0x0010);
            bus.read(0x0010).Should().Be(0x37);
        }

        [Fact]
        public void STY_writes_y_to_memory() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.Y.set(0x37);
            new STY(cpu, bus).runAddress(0x0010);
            bus.read(0x0010).Should().Be(0x37);
        }

        [Fact]
        public void Stores_do_not_affect_flags() {
            var (cpu, bus) = Cpu6502.Fresh();
            cpu.A.set(0x00);
            byte before = cpu.Flags.get();
            new STA(cpu, bus).runAddress(0x0010);
            cpu.Flags.get().Should().Be(before);
        }
    }
}
