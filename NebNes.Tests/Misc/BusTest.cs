using FluentAssertions;
using NebNes.Mappers;
using NebNes.Misc;

namespace NebNes.Tests.Misc {
    public class BusTest {
        // Records the addresses/values the Bus routes to the cartridge so tests can
        // assert on delegation, and returns a fixed sentinel for reads.
        private class FakeMapper : IMapper {
            public byte ReadReturn = 0x00;
            public int ReadCount = 0;
            public ushort LastReadAddress;
            public int WriteCount = 0;
            public ushort LastWriteAddress;
            public byte LastWriteValue;

            public void onLoad() { }
            public byte cpuRead(ushort address) {
                ReadCount++;
                LastReadAddress = address;
                return ReadReturn;
            }
            public void cpuWrite(ushort address, byte value) {
                WriteCount++;
                LastWriteAddress = address;
                LastWriteValue = value;
            }
            public byte ppuRead(ushort address) => 0;
            public void ppuWrite(ushort address, byte value) { }
            public void tick() { }
        }

        [Fact]
        public void Bus_wram_read_write_round_trip() {
            Bus bus = new Bus(new FakeMapper());
            bus.write(0x0005, 0x42);
            bus.read(0x0005).Should().Be(0x42);
        }

        [Fact]
        public void Bus_wram_is_mirrored_every_2k() {
            Bus bus = new Bus(new FakeMapper());
            bus.write(0x0001, 0xAB);
            // $0800/$1000/$1800 all mirror $0000-$07FF.
            bus.read(0x0801).Should().Be(0xAB);
            bus.read(0x1001).Should().Be(0xAB);
            bus.read(0x1801).Should().Be(0xAB);
        }

        [Fact]
        public void Bus_wram_write_through_mirror_hits_same_cell() {
            Bus bus = new Bus(new FakeMapper());
            bus.write(0x1802, 0x77); // 0x1802 & 0x07FF == 0x0002
            bus.read(0x0002).Should().Be(0x77);
        }

        [Fact]
        public void Bus_cartridge_read_is_delegated_to_mapper() {
            FakeMapper mapper = new FakeMapper { ReadReturn = 0x99 };
            Bus bus = new Bus(mapper);
            bus.read(0x8000).Should().Be(0x99);
            mapper.ReadCount.Should().Be(1);
            mapper.LastReadAddress.Should().Be(0x8000);
        }

        [Fact]
        public void Bus_cartridge_write_is_delegated_to_mapper() {
            FakeMapper mapper = new FakeMapper();
            Bus bus = new Bus(mapper);
            bus.write(0xC123, 0x55);
            mapper.WriteCount.Should().Be(1);
            mapper.LastWriteAddress.Should().Be(0xC123);
            mapper.LastWriteValue.Should().Be(0x55);
        }

        [Fact]
        public void Bus_wram_access_does_not_reach_mapper() {
            FakeMapper mapper = new FakeMapper();
            Bus bus = new Bus(mapper);
            bus.write(0x0000, 0x01);
            bus.read(0x0000);
            mapper.ReadCount.Should().Be(0);
            mapper.WriteCount.Should().Be(0);
        }

        [Fact]
        public void Bus_open_bus_returns_last_value_read() {
            FakeMapper mapper = new FakeMapper { ReadReturn = 0xBE };
            Bus bus = new Bus(mapper);
            bus.read(0x8000); // latches 0xBE onto the bus
            // $4000-$401F is unmapped here (APU/IO, not implemented) => open bus.
            bus.read(0x4000).Should().Be(0xBE);
        }
    }
}
