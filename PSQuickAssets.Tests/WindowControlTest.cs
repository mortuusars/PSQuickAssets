using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PSQuickAssets.Tests
{
    public class WindowControlTest
    {
        [Fact]
        public void IsProcessRunningCaseInsensitive()
        {
            Assert.Equal(IsRunningCapital(), Utils.WindowControl.IsProcessRunning("photoshop"));
        }

        private bool IsRunningCapital()
        {
            return Array.Exists(Process.GetProcesses(), p => p.ProcessName == "Photoshop");
        }
    }
}
