using MyLab.HttpMetrics;
using Xunit;

namespace UnitTests
{
    public class UrlPathNormalizerBehavior
    {
        [Theory]
        [InlineData(null, "~")]
        [InlineData("", "~")]
        [InlineData("/one/two/three", "/one/two/three")]
        [InlineData("/one/two/three/", "/one/two/three")]
        [InlineData("one/two/three/", "/one/two/three")]
        [InlineData("/one/123/three/", "/one/xxx/three")]
        [InlineData("/123/two/three/", "/xxx/two/three")]
        [InlineData("/one/two/123/", "/one/two/xxx")]
        [InlineData("/one/96f11fe7-2887-46dc-9062-e3b26f18db82/three/", "/one/xxx/three")]
        [InlineData("/one/{B5668494-95E8-4150-9580-30A1A3FB035A}/three/", "/one/xxx/three")]
        [InlineData("/one/ololo-385/three/", "/one/xxx/three")]
        public void ShouldNormalizeUrl(string input, string expected)
        {
            //Arrange
            
            //Act
            var res = UrlPathNormalizer.Normalize(input);

            //Assert
            Assert.Equal(expected, res);
        }
    }
}
