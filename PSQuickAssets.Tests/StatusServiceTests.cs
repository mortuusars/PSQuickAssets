using PSQuickAssets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PSQuickAssets.Tests;

public class StatusServiceTests
{
    internal StatusService _statusService = new StatusService();

    [Fact]
    public void LoadingShouldSetIsLoadingToTrue()
    {
        using (var _ = _statusService.LoadingStatus())
        {
            Assert.True(_statusService.IsLoading);
        }

        Assert.False(_statusService.IsLoading);
    }

    [Fact]
    public void LoadingMultipleTasksShouldIsLoadingShouldStayTrue()
    {
        using (var _ = _statusService.LoadingStatus())
        {
            Assert.True(_statusService.IsLoading);

            using (var asd = _statusService.LoadingStatus())
            {
                Assert.True(_statusService.IsLoading);
            }

            Assert.True(_statusService.IsLoading);
        }

        Assert.False(_statusService.IsLoading);
    }
}
