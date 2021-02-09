using System;
using PSQuickAssets.Infrastructure;
using Xunit;

namespace PSQuickAssets.Tests
{
    public class StringFormatterTests
    {
        StringFormatter formatter = new StringFormatter();

        [Fact]
        public void ShortenThrowsIfInputIsNull()
        {
            Assert.Throws<ArgumentException>(() => formatter.CutMiddle(null, 5));
        }

        [Fact]
        public void ShortenReturnsSameIfShorter()
        {
            var result = formatter.CutMiddle("asd", 5);

            Assert.Equal("asd", result);
        }

        [Fact]
        public void ShortenReturnsSameIfLengthIsSame()
        {
            var result = formatter.CutMiddle("asddf", 5);

            Assert.Equal("asddf", result);
        }

        [Fact]
        public void ShortenReturnsEmptyStringWhenPassedEmptyString()
        {
            var result = formatter.CutMiddle("", 123);

            Assert.Equal("", result);
        }

        [Fact]
        public void ShortenReturnsShortenedString()
        {
            var result = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 5);
            var result2 = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 7);

            Assert.Equal("...s.", result);
            Assert.Equal("...sts.", result2);
        }

        [Fact]
        public void ShortenReturnsShortenedStringWithDotsInMiddle()
        {
            var result = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 12);
            var result2 = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 10);
            var result3 = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 22);

            Assert.Equal("Test...ests.", result);
            Assert.Equal("Tes...sts.", result2);
            Assert.Equal("Test run ...tal tests.", result3);
        }

        [Fact]
        public void ShortenReturnsCorrectLength()
        {
            var result = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 12);
            var result2 = formatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 20);

            Assert.Equal(12, result.Length);
            Assert.Equal(20, result2.Length);
        }



        [Fact]
        public void CutStartReturnsProperLength()
        {
            var result = formatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 12);

            Assert.Equal(12, result.Length);
        }

        [Fact]
        public void CutStartReturnsProperString()
        {
            var result = formatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 7, numberOfPeriods: 3);
            var result2 = formatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 12, numberOfPeriods: 3);

            Assert.Equal("...al tests.", result2);
        }

        [Fact]
        public void CutStartReturnsProperNumberOfDots()
        {
            var result = formatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 10, numberOfPeriods: 6);

            Assert.Equal("......sts.", result);
        }

        [Fact]
        public void CutEndReturnsProperString()
        {
            var result = formatter.CutEnd("Test run finished in 1.1 sec. 5 total tests.", 10, numberOfPeriods: 3);

            Assert.Equal("Test ru...", result);
        }
    }
}
