using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Fulbaso.Common.Resources;
using Regex = System.Text.RegularExpressions.Regex;

namespace Fulbaso.Common
{
    public static class StringExtensions
    {
        public static string FormatWith(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static bool NullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        public static bool NullOrBlank(this string text)
        {
            if (text == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(text.Trim());
        }

        public static bool InsensitiveEquals(this object left, object right)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(left, right) == 0;
        }

        public static bool InsensitiveEquals(this string left, string right)
        {
            return StringComparer.InvariantCultureIgnoreCase.Compare(left, right) == 0;
        }

        public static string UnicodeDecode(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            Regex regex = new Regex(Constants.UnicodeRegex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            return regex.Replace(text, match => ((char)int.Parse(match.Groups[1].Value, NumberStyles.HexNumber)).ToString(CultureInfo.InvariantCulture));
        }

        public static string[] SplitOnNewLines(this string text, bool removeEmptyEntries = true)
        {
            StringSplitOptions opts = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            string[] separators = new[] { Environment.NewLine, Constants.NewLine, Constants.EscapedNewLine };
            string[] result = text.Split(separators, opts);
            return result;
        }

        public static string TrimAll(this string text, bool includeLineBreaks = true)
        {
            if (includeLineBreaks)
            {
                return Regex.Replace(text, @"\s+", " ");
            }
            else
            {
                return Regex.Replace(text, @"[^\S\n]+", " ");
            }
        }
    }
}
