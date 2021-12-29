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
            Assert.Equal(new Version("1.2.0"), UpdateVersionFinder.GetVersionFromFile("asdasdkjwlkj<Version>1.2.0</Version>"));
        }

        [Fact]
        public void GetVersionThrowsIfNotFound()
        {
            Assert.Throws<ArgumentException>(() => UpdateVersionFinder.GetVersionFromFile("asdasdkjwlkjon"));
        }
    }
}
