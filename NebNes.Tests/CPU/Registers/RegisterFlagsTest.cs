using FluentAssertions;
using NebNes.CPU.Registers;
using NebNes.Enums;

namespace NebNes.Tests.CPU.Registers {
    public class RegisterFlagsTest {
        // FlagsIndex: C=0, Z=1, I=2, D=3, B=4, X=5, V=6, N=7

        [Fact]
        public void RegisterFlags_set_and_get() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0b10101010);
            flags.get().Should().Be(0b10101010);
        }

        [Fact]
        public void RegisterFlags_setFlag_sets_the_right_bit() {
            RegisterFlags flags = new RegisterFlags();
            flags.setFlag(FlagsIndex.C);
            flags.get().Should().Be(0b00000001);
            flags.setFlag(FlagsIndex.N);
            flags.get().Should().Be(0b10000001);
        }

        [Fact]
        public void RegisterFlags_clearFlag_clears_only_that_bit() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0xFF);
            flags.clearFlag(FlagsIndex.Z);
            flags.get().Should().Be(0b11111101);
        }

        [Fact]
        public void RegisterFlags_getFlag_reads_low_bit() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0b00000001); // C set
            flags.getFlag(FlagsIndex.C).Should().BeTrue();
            flags.getFlag(FlagsIndex.Z).Should().BeFalse();
        }

        [Fact]
        public void RegisterFlags_getFlag_reads_high_bits() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0b10000010); // N and Z set
            flags.getFlag(FlagsIndex.N).Should().BeTrue();
            flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            flags.getFlag(FlagsIndex.V).Should().BeFalse();
        }

        [Fact]
        public void RegisterFlags_updateZero_sets_Z_when_value_zero() {
            RegisterFlags flags = new RegisterFlags();
            flags.updateZero(0x00);
            flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            flags.get().Should().Be(0b00000010);
        }

        [Fact]
        public void RegisterFlags_updateZero_clears_Z_when_value_nonzero() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0xFF);
            flags.updateZero(0x05);
            flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            flags.get().Should().Be(0b11111101);
        }

        [Fact]
        public void RegisterFlags_updateNegative_sets_N_when_bit7_set() {
            RegisterFlags flags = new RegisterFlags();
            flags.updateNegative(0x80);
            flags.getFlag(FlagsIndex.N).Should().BeTrue();
            flags.get().Should().Be(0b10000000);
        }

        [Fact]
        public void RegisterFlags_updateNegative_clears_N_when_bit7_clear() {
            RegisterFlags flags = new RegisterFlags();
            flags.set(0xFF);
            flags.updateNegative(0x7F);
            flags.getFlag(FlagsIndex.N).Should().BeFalse();
            flags.get().Should().Be(0b01111111);
        }

        [Fact]
        public void RegisterFlags_updateZeroAndNegative_zero_value() {
            RegisterFlags flags = new RegisterFlags();
            flags.updateZeroAndNegative(0x00);
            flags.getFlag(FlagsIndex.Z).Should().BeTrue();
            flags.getFlag(FlagsIndex.N).Should().BeFalse();
        }

        [Fact]
        public void RegisterFlags_updateZeroAndNegative_negative_value() {
            RegisterFlags flags = new RegisterFlags();
            flags.updateZeroAndNegative(0x80);
            flags.getFlag(FlagsIndex.Z).Should().BeFalse();
            flags.getFlag(FlagsIndex.N).Should().BeTrue();
        }
    }
}
