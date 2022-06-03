using PSQuickAssets.Services;
using Xunit;

namespace PSQuickAssets.Tests;

public class StatusServiceTests
{
    internal StatusService _statusService = new StatusService();

    [Fact]
    public void LoadingShouldSetIsLoadingToTrue()
    {
        using (var _ = _statusService.Loading("asd"))
        {
            Assert.True(_statusService.IsLoading);
        }

        Assert.False(_statusService.IsLoading);
    }

    [Fact]
    public void LoadingMultipleTasksShouldIsLoadingShouldStayTrue()
    {
        using (var _ = _statusService.Loading("asd"))
        {
            Assert.True(_statusService.IsLoading);

            using (var asd = _statusService.Loading("asdasd"))
            {
                Assert.True(_statusService.IsLoading);
            }

            Assert.True(_statusService.IsLoading);
        }

        Assert.False(_statusService.IsLoading);
    }
}
