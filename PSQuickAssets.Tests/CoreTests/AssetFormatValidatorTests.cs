using PSQA.Assets;
using Xunit;

namespace PSQuickAssets.Tests.CoreTests;

public class AssetFormatValidatorTests
{
    public static AssetFormatValidator _validator;

    [Theory]
    [InlineData(".jpg", true)]
    [InlineData("jpg", true)]
    [InlineData("  png  ", true)]
    [InlineData("asd", false)]
    [InlineData("", false)]
    [InlineData("    ", false)]
    public void ValidateTests(string input, bool expected)
    {
        Assert.Equal(expected, _validator.Validate(input).Success);
    }
}
