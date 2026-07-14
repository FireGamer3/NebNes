using FluentAssertions;
using NebNes.CPU.Registers;

namespace NebNes.Tests.CPU.Registers {
    public class Register8BitTest {
        [Fact]
        public void Register8Bit_defaults_to_zero() {
            new Register8Bit().get().Should().Be(0);
        }

        [Fact]
        public void Register8Bit_set_and_get() {
            Register8Bit reg = new Register8Bit();
            reg.set(0x7F);
            reg.get().Should().Be(0x7F);
        }

        [Fact]
        public void Register8Bit_increment() {
            Register8Bit reg = new Register8Bit();
            reg.set(0x10);
            reg.increment();
            reg.get().Should().Be(0x11);
        }

        [Fact]
        public void Register8Bit_increment_wraps_at_255() {
            Register8Bit reg = new Register8Bit();
            reg.set(0xFF);
            reg.increment();
            reg.get().Should().Be(0x00);
        }

        [Fact]
        public void Register8Bit_decrement() {
            Register8Bit reg = new Register8Bit();
            reg.set(0x10);
            reg.decrement();
            reg.get().Should().Be(0x0F);
        }

        [Fact]
        public void Register8Bit_decrement_wraps_below_zero() {
            Register8Bit reg = new Register8Bit();
            reg.set(0x00);
            reg.decrement();
            reg.get().Should().Be(0xFF);
        }
    }
}
