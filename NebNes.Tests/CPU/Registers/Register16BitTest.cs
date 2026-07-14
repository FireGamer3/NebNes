using FluentAssertions;
using NebNes.CPU.Registers;

namespace NebNes.Tests.CPU.Registers {
    public class Register16BitTest {
        [Fact]
        public void Register16Bit_defaults_to_zero() {
            new Register16Bit().get().Should().Be(0);
        }

        [Fact]
        public void Register16Bit_set_and_get() {
            Register16Bit reg = new Register16Bit();
            reg.set(0xBEEF);
            reg.get().Should().Be(0xBEEF);
        }

        [Fact]
        public void Register16Bit_increment() {
            Register16Bit reg = new Register16Bit();
            reg.set(0x00FF);
            reg.increment();
            reg.get().Should().Be(0x0100);
        }

        [Fact]
        public void Register16Bit_increment_wraps_at_65535() {
            Register16Bit reg = new Register16Bit();
            reg.set(0xFFFF);
            reg.increment();
            reg.get().Should().Be(0x0000);
        }

        [Fact]
        public void Register16Bit_decrement() {
            Register16Bit reg = new Register16Bit();
            reg.set(0x0100);
            reg.decrement();
            reg.get().Should().Be(0x00FF);
        }

        [Fact]
        public void Register16Bit_decrement_wraps_below_zero() {
            Register16Bit reg = new Register16Bit();
            reg.set(0x0000);
            reg.decrement();
            reg.get().Should().Be(0xFFFF);
        }
    }
}
