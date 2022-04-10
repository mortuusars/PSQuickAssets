using PSQA.Core;
using Xunit;

namespace PSQuickAssets.Tests.CoreTests;

public class ResultTests
{
    public static readonly object[][] ResultData = new object[][]
    {
        new object[] { Result.Ok(), true, string.Empty },
        new object[] { Result.Fail("error"), false, "error" },
    };

    public static readonly object[][] ResultOfTData = new object[][]
    {
        new object[] { Result.Ok(""), true, string.Empty, string.Empty },
        new object[] { Result.Ok<string>(null), true, null, string.Empty },
        new object[] { Result.Fail<string>("error"), false, null, "error" },
    };

    [Theory]
    [MemberData(nameof(ResultData))]
    public void ResultTest(Result result, bool expectedSuccess, string expectedErrorMsg)
    {
        Assert.Equal(expectedSuccess, result.Success);
        Assert.Equal(expectedErrorMsg, result.Error);
    }

    [Theory]
    [MemberData(nameof(ResultOfTData))]
    public void ResultOfTTest(Result<string> result, bool expectedSuccess, string expectedValue, string expectedErrorMsg)
    {
        Assert.Equal(expectedSuccess, result.Success);
        Assert.Equal(expectedValue, result.Value);
        Assert.Equal(expectedErrorMsg, result.Error);
    }
}
