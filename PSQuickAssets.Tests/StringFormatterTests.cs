using System;
using Xunit;

namespace PSQuickAssets.Tests
{
    public class StringFormatterTests
    {

        [Fact]
        public void ShortenThrowsIfInputIsNull()
        {
            Assert.Throws<ArgumentException>(() => StringFormatter.CutMiddle(null, 5));
        }

        [Fact]
        public void ShortenReturnsSameIfShorter()
        {
            var result = StringFormatter.CutMiddle("asd", 5);

            Assert.Equal("asd", result);
        }

        [Fact]
        public void ShortenReturnsSameIfLengthIsSame()
        {
            var result = StringFormatter.CutMiddle("asddf", 5);

            Assert.Equal("asddf", result);
        }

        [Fact]
        public void ShortenReturnsEmptyStringWhenPassedEmptyString()
        {
            var result = StringFormatter.CutMiddle("", 123);

            Assert.Equal("", result);
        }

        [Fact]
        public void ShortenReturnsShortenedString()
        {
            var result = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 5);
            var result2 = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 7);

            Assert.Equal("...s.", result);
            Assert.Equal("...sts.", result2);
        }

        [Fact]
        public void ShortenReturnsShortenedStringWithDotsInMiddle()
        {
            var result = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 12);
            var result2 = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 10);
            var result3 = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 22);

            Assert.Equal("Test...ests.", result);
            Assert.Equal("Tes...sts.", result2);
            Assert.Equal("Test run ...tal tests.", result3);
        }

        [Fact]
        public void ShortenReturnsCorrectLength()
        {
            var result = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 12);
            var result2 = StringFormatter.CutMiddle("Test run finished in 1.1 sec. 5 total tests.", 20);

            Assert.Equal(12, result.Length);
            Assert.Equal(20, result2.Length);
        }



        [Fact]
        public void CutStartReturnsProperLength()
        {
            var result = StringFormatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 12);

            Assert.Equal(12, result.Length);
        }

        [Fact]
        public void CutStartReturnsProperString()
        {
            var result = StringFormatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 7, numberOfPeriods: 3);
            var result2 = StringFormatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 12, numberOfPeriods: 3);

            Assert.Equal("...al tests.", result2);
        }

        [Fact]
        public void CutStartReturnsProperNumberOfDots()
        {
            var result = StringFormatter.CutStart("Test run finished in 1.1 sec. 5 total tests.", 10, numberOfPeriods: 6);

            Assert.Equal("......sts.", result);
        }

        [Fact]
        public void CutEndReturnsProperString()
        {
            var result = StringFormatter.CutEnd("Test run finished in 1.1 sec. 5 total tests.", 10, numberOfPeriods: 3);

            Assert.Equal("Test ru...", result);
        }
    }
}
