using PSQuickAssets.Update;
using System;
using Xunit;

namespace PSQuickAssets.Tests
{
    public class VersionFinderTests
    {
        [Fact]
        public void GetVersionShouldGetRightVersion()
        {
            Assert.Equal(new Version("1.2.0"), CsProjVersionParser.Parse("asdasdkjwlkj<Version>1.2.0</Version>"));
        }

        [Fact]
        public void GetVersionThrowsIfNotFound()
        {
            Assert.Throws<ArgumentException>(() => CsProjVersionParser.Parse("asdasdkjwlkjon"));
        }
    }
}
