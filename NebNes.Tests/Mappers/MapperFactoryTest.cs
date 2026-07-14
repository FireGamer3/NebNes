using FluentAssertions;
using NebNes.Mappers;
using NebNes.Misc;

namespace NebNes.Tests.Mappers {
    public class MapperFactoryTest {
        [Fact]
        public void CreateMapper_returns_NROM_for_mapper_0() {
            Cart cart = MapperTestHelpers.BuildCart(1, 1, 0);
            IMapper mapper = MapperFactory.CreateMapper(null!, cart);
            mapper.Should().BeOfType<NROM>();
        }

        [Fact]
        public void CreateMapper_returns_UxROM_for_mapper_2() {
            Cart cart = MapperTestHelpers.BuildCart(2, 0, 2);
            IMapper mapper = MapperFactory.CreateMapper(null!, cart);
            mapper.Should().BeOfType<UxROM>();
        }

        [Fact]
        public void CreateMapper_throws_for_unsupported_mapper() {
            Cart cart = MapperTestHelpers.BuildCart(1, 1, 1);
            Action act = () => MapperFactory.CreateMapper(null!, cart);
            act.Should().Throw<ArgumentException>().WithMessage("*1*");
        }
    }
}
