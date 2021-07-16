using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class CharacterAnalyzer<TBuffer> where TBuffer : ILookAheadBuffer
	{
		private readonly TBuffer buffer;

		public TBuffer Buffer => buffer;

		public bool EndOfInput
		{
			get
			{
				TBuffer val = buffer;
				return val.EndOfInput;
			}
		}

		public CharacterAnalyzer(TBuffer buffer)
		{
			this.buffer = buffer;
		}

		public char Peek(int offset)
		{
			TBuffer val = buffer;
			return val.Peek(offset);
		}

		public void Skip(int length)
		{
			TBuffer val = buffer;
			val.Skip(length);
		}

		public bool IsAlphaNumericDashOrUnderscore(int offset = 0)
		{
			TBuffer val = buffer;
			char c = val.Peek(offset);
			if ((c < '0' || c > '9') && (c < 'A' || c > 'Z') && (c < 'a' || c > 'z') && c != '_')
			{
				return c == '-';
			}
			return true;
		}

		public bool IsAscii(int offset = 0)
		{
			TBuffer val = buffer;
			return val.Peek(offset) <= '\u007f';
		}

		public bool IsPrintable(int offset = 0)
		{
			TBuffer val = buffer;
			char c = val.Peek(offset);
			switch (c)
			{
			default:
				if (c != '\u0085' && (c < '\u00a0' || c > '\ud7ff'))
				{
					if (c >= '\ue000')
					{
						return c <= '\ufffd';
					}
					return false;
				}
				break;
			case '\t':
			case '\n':
			case '\r':
			case ' ':
			case '!':
			case '"':
			case '#':
			case '$':
			case '%':
			case '&':
			case '\'':
			case '(':
			case ')':
			case '*':
			case '+':
			case ',':
			case '-':
			case '.':
			case '/':
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			case ':':
			case ';':
			case '<':
			case '=':
			case '>':
			case '?':
			case '@':
			case 'A':
			case 'B':
			case 'C':
			case 'D':
			case 'E':
			case 'F':
			case 'G':
			case 'H':
			case 'I':
			case 'J':
			case 'K':
			case 'L':
			case 'M':
			case 'N':
			case 'O':
			case 'P':
			case 'Q':
			case 'R':
			case 'S':
			case 'T':
			case 'U':
			case 'V':
			case 'W':
			case 'X':
			case 'Y':
			case 'Z':
			case '[':
			case '\\':
			case ']':
			case '^':
			case '_':
			case '`':
			case 'a':
			case 'b':
			case 'c':
			case 'd':
			case 'e':
			case 'f':
			case 'g':
			case 'h':
			case 'i':
			case 'j':
			case 'k':
			case 'l':
			case 'm':
			case 'n':
			case 'o':
			case 'p':
			case 'q':
			case 'r':
			case 's':
			case 't':
			case 'u':
			case 'v':
			case 'w':
			case 'x':
			case 'y':
			case 'z':
			case '{':
			case '|':
			case '}':
			case '~':
				break;
			}
			return true;
		}

		public bool IsDigit(int offset = 0)
		{
			TBuffer val = buffer;
			char c = val.Peek(offset);
			if (c >= '0')
			{
				return c <= '9';
			}
			return false;
		}

		public int AsDigit(int offset = 0)
		{
			TBuffer val = buffer;
			return val.Peek(offset) - 48;
		}

		public bool IsHex(int offset)
		{
			TBuffer val = buffer;
			char c = val.Peek(offset);
			if ((c < '0' || c > '9') && (c < 'A' || c > 'F'))
			{
				if (c >= 'a')
				{
					return c <= 'f';
				}
				return false;
			}
			return true;
		}

		public int AsHex(int offset)
		{
			TBuffer val = buffer;
			char c = val.Peek(offset);
			if (c <= '9')
			{
				return c - 48;
			}
			if (c <= 'F')
			{
				return c - 65 + 10;
			}
			return c - 97 + 10;
		}

		public bool IsSpace(int offset = 0)
		{
			return Check(' ', offset);
		}

		public bool IsZero(int offset = 0)
		{
			return Check('\0', offset);
		}

		public bool IsTab(int offset = 0)
		{
			return Check('\t', offset);
		}

		public bool IsWhite(int offset = 0)
		{
			if (!IsSpace(offset))
			{
				return IsTab(offset);
			}
			return true;
		}

		public bool IsBreak(int offset = 0)
		{
			return Check("\r\n\u0085\u2028\u2029", offset);
		}

		public bool IsCrLf(int offset = 0)
		{
			if (Check('\r', offset))
			{
				return Check('\n', offset + 1);
			}
			return false;
		}

		public bool IsBreakOrZero(int offset = 0)
		{
			if (!IsBreak(offset))
			{
				return IsZero(offset);
			}
			return true;
		}

		public bool IsWhiteBreakOrZero(int offset = 0)
		{
			if (!IsWhite(offset))
			{
				return IsBreakOrZero(offset);
			}
			return true;
		}

		public bool Check(char expected, int offset = 0)
		{
			TBuffer val = buffer;
			return val.Peek(offset) == expected;
		}

		public bool Check(string expectedCharacters, int offset = 0)
		{
			Debug.Assert(expectedCharacters.Length > 1, "Use Check(char, int) instead.");
			TBuffer val = buffer;
			char value = val.Peek(offset);
			return expectedCharacters.IndexOf(value) != -1;
		}
	}
}
