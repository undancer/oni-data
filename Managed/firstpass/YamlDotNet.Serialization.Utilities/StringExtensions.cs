using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class StringExtensions
	{
		private static string ToCamelOrPascalCase(string str, Func<char, char> firstLetterTransform)
		{
			string text = Regex.Replace(str, "([_\\-])(?<char>[a-z])", (Match match) => match.Groups["char"].Value.ToUpperInvariant(), RegexOptions.IgnoreCase);
			return firstLetterTransform(text[0]) + text.Substring(1);
		}

		public static string ToCamelCase(this string str)
		{
			return ToCamelOrPascalCase(str, char.ToLowerInvariant);
		}

		public static string ToPascalCase(this string str)
		{
			return ToCamelOrPascalCase(str, char.ToUpperInvariant);
		}

		public static string FromCamelCase(this string str, string separator)
		{
			str = char.ToLower(str[0]) + str.Substring(1);
			str = Regex.Replace(str.ToCamelCase(), "(?<char>[A-Z])", (Match match) => separator + match.Groups["char"].Value.ToLowerInvariant());
			return str;
		}
	}
}
