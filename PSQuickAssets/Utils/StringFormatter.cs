using System;
using System.Text;

namespace PSQuickAssets
{
    public static class StringFormatter
    {
        /// <summary>
        /// Shortens input string to number of chars. Replacing middle portion with periods.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="numberOfChars"></param>
        /// <param name="numberOfPeriods"></param>
        public static string CutMiddle(string input, uint numberOfChars, uint numberOfPeriods = 3)
        {
            if (input == null)
                throw new ArgumentException("Input cannot be null");

            if (input.Length <= numberOfChars)
                return input;

            if (numberOfChars < 10)
                return CutStart(input, numberOfChars, numberOfPeriods);
            else
                return ShortenMiddle(input, numberOfChars, numberOfPeriods);
        }

        

        public static string CutStart(string input, uint numberOfChars, uint numberOfPeriods = 3)
        {
            if (input == null)
                throw new ArgumentException("Input cannot be null");

            if (input.Length <= numberOfChars)
                return input;

            string end = input[(int)(input.Length - (numberOfChars - numberOfPeriods))..];

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < numberOfPeriods; i++)
                sb.Append('.');

            sb.Append(end);

            return sb.ToString();
        }

        public static string CutEnd(string input, uint numberOfChars, uint numberOfPeriods = 3)
        {
            if (input == null)
                throw new ArgumentException("Input cannot be null");

            if (input.Length <= numberOfChars)
                return input;

            string start = input.Substring(0, (int)(numberOfChars - numberOfPeriods));

            StringBuilder sb = new StringBuilder();

            sb.Append(start);

            for (int i = 0; i < numberOfPeriods; i++)
                sb.Append('.');

            return sb.ToString();
        }

        private static string ShortenMiddle(string input, uint numberOfChars, uint numberOfPeriods)
        {
            int numberMinusDots = (int)(numberOfChars - numberOfPeriods);

            int length = (int)Math.Round(numberMinusDots / 2.0, 0, MidpointRounding.ToPositiveInfinity);

            string start = input.Substring(0, length);
            string end = input.Substring((int)(input.Length - length));

            while ((start.Length + numberOfPeriods + end.Length) > numberOfChars)
                start = start.Remove(start.Length - 1);

            StringBuilder sb = new StringBuilder();

            sb.Append(start);

            for (int i = 0; i < numberOfPeriods; i++)
                sb.Append('.');

            sb.Append(end);

            return sb.ToString();
        }
    }
}
