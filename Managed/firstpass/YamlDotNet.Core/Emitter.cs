using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public class Emitter : IEmitter
	{
		private class AnchorData
		{
			public string anchor;

			public bool isAlias;
		}

		private class TagData
		{
			public string handle;

			public string suffix;
		}

		private class ScalarData
		{
			public string value;

			public bool isMultiline;

			public bool isFlowPlainAllowed;

			public bool isBlockPlainAllowed;

			public bool isSingleQuotedAllowed;

			public bool isBlockAllowed;

			public bool hasSingleQuotes;

			public ScalarStyle style;
		}

		private const int MinBestIndent = 2;

		private const int MaxBestIndent = 9;

		private const int MaxAliasLength = 128;

		private static readonly Regex uriReplacer = new Regex("[^0-9A-Za-z_\\-;?@=$~\\\\\\)\\]/:&+,\\.\\*\\(\\[!]", RegexOptions.Singleline);

		private readonly TextWriter output;

		private readonly bool outputUsesUnicodeEncoding;

		private readonly bool isCanonical;

		private readonly int bestIndent;

		private readonly int bestWidth;

		private EmitterState state;

		private readonly Stack<EmitterState> states = new Stack<EmitterState>();

		private readonly Queue<ParsingEvent> events = new Queue<ParsingEvent>();

		private readonly Stack<int> indents = new Stack<int>();

		private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();

		private int indent;

		private int flowLevel;

		private bool isMappingContext;

		private bool isSimpleKeyContext;

		private bool isRootContext;

		private int column;

		private bool isWhitespace;

		private bool isIndentation;

		private bool isOpenEnded;

		private bool isDocumentEndWritten;

		private readonly AnchorData anchorData = new AnchorData();

		private readonly TagData tagData = new TagData();

		private readonly ScalarData scalarData = new ScalarData();

		public Emitter(TextWriter output)
			: this(output, 2)
		{
		}

		public Emitter(TextWriter output, int bestIndent)
			: this(output, bestIndent, int.MaxValue)
		{
		}

		public Emitter(TextWriter output, int bestIndent, int bestWidth)
			: this(output, bestIndent, bestWidth, isCanonical: false)
		{
		}

		public Emitter(TextWriter output, int bestIndent, int bestWidth, bool isCanonical)
		{
			if (bestIndent < 2 || bestIndent > 9)
			{
				throw new ArgumentOutOfRangeException("bestIndent", string.Format(CultureInfo.InvariantCulture, "The bestIndent parameter must be between {0} and {1}.", 2, 9));
			}
			this.bestIndent = bestIndent;
			if (bestWidth <= bestIndent * 2)
			{
				throw new ArgumentOutOfRangeException("bestWidth", "The bestWidth parameter must be greater than bestIndent * 2.");
			}
			this.bestWidth = bestWidth;
			this.isCanonical = isCanonical;
			this.output = output;
			outputUsesUnicodeEncoding = IsUnicode(output.Encoding);
		}

		public void Emit(ParsingEvent @event)
		{
			events.Enqueue(@event);
			while (!NeedMoreEvents())
			{
				ParsingEvent evt = events.Peek();
				try
				{
					AnalyzeEvent(evt);
					StateMachine(evt);
				}
				finally
				{
					events.Dequeue();
				}
			}
		}

		private bool NeedMoreEvents()
		{
			if (events.Count == 0)
			{
				return true;
			}
			int num;
			switch (events.Peek().Type)
			{
			case EventType.DocumentStart:
				num = 1;
				break;
			case EventType.SequenceStart:
				num = 2;
				break;
			case EventType.MappingStart:
				num = 3;
				break;
			default:
				return false;
			}
			if (events.Count > num)
			{
				return false;
			}
			int num2 = 0;
			using (Queue<ParsingEvent>.Enumerator enumerator = events.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current.Type)
					{
					case EventType.DocumentStart:
					case EventType.SequenceStart:
					case EventType.MappingStart:
						num2++;
						break;
					case EventType.DocumentEnd:
					case EventType.SequenceEnd:
					case EventType.MappingEnd:
						num2--;
						break;
					}
					if (num2 == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void AnalyzeEvent(ParsingEvent evt)
		{
			anchorData.anchor = null;
			tagData.handle = null;
			tagData.suffix = null;
			if (evt is YamlDotNet.Core.Events.AnchorAlias anchorAlias)
			{
				AnalyzeAnchor(anchorAlias.Value, isAlias: true);
			}
			else if (evt is NodeEvent nodeEvent)
			{
				if (evt is YamlDotNet.Core.Events.Scalar scalar)
				{
					AnalyzeScalar(scalar);
				}
				AnalyzeAnchor(nodeEvent.Anchor, isAlias: false);
				if (!string.IsNullOrEmpty(nodeEvent.Tag) && (isCanonical || nodeEvent.IsCanonical))
				{
					AnalyzeTag(nodeEvent.Tag);
				}
			}
		}

		private void AnalyzeAnchor(string anchor, bool isAlias)
		{
			anchorData.anchor = anchor;
			anchorData.isAlias = isAlias;
		}

		private void AnalyzeScalar(YamlDotNet.Core.Events.Scalar scalar)
		{
			string value = scalar.Value;
			scalarData.value = value;
			if (value.Length == 0)
			{
				if (scalar.Tag == "tag:yaml.org,2002:null")
				{
					scalarData.isMultiline = false;
					scalarData.isFlowPlainAllowed = false;
					scalarData.isBlockPlainAllowed = true;
					scalarData.isSingleQuotedAllowed = false;
					scalarData.isBlockAllowed = false;
				}
				else
				{
					scalarData.isMultiline = false;
					scalarData.isFlowPlainAllowed = false;
					scalarData.isBlockPlainAllowed = false;
					scalarData.isSingleQuotedAllowed = true;
					scalarData.isBlockAllowed = false;
				}
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (value.StartsWith("---", StringComparison.Ordinal) || value.StartsWith("...", StringComparison.Ordinal))
			{
				flag = true;
				flag2 = true;
			}
			CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
			bool flag3 = true;
			bool flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			bool flag9 = false;
			bool flag10 = false;
			bool flag11 = false;
			bool flag12 = false;
			bool flag13 = false;
			bool flag14 = false;
			bool flag15 = !ValueIsRepresentableInOutputEncoding(value);
			bool flag16 = false;
			bool flag17 = true;
			while (!characterAnalyzer.EndOfInput)
			{
				if (flag17)
				{
					if (characterAnalyzer.Check("#,[]{}&*!|>\\\"%@`'"))
					{
						flag = true;
						flag2 = true;
						flag9 = characterAnalyzer.Check('\'');
						flag16 |= characterAnalyzer.Check('\'');
					}
					if (characterAnalyzer.Check("?:"))
					{
						flag = true;
						if (flag4)
						{
							flag2 = true;
						}
					}
					if (characterAnalyzer.Check('-') && flag4)
					{
						flag = true;
						flag2 = true;
					}
				}
				else
				{
					if (characterAnalyzer.Check(",?[]{}"))
					{
						flag = true;
					}
					if (characterAnalyzer.Check(':'))
					{
						flag = true;
						if (flag4)
						{
							flag2 = true;
						}
					}
					if (characterAnalyzer.Check('#') && flag3)
					{
						flag = true;
						flag2 = true;
					}
					flag16 |= characterAnalyzer.Check('\'');
				}
				if (!flag15 && !characterAnalyzer.IsPrintable())
				{
					flag15 = true;
				}
				if (characterAnalyzer.IsBreak())
				{
					flag14 = true;
				}
				if (characterAnalyzer.IsSpace())
				{
					if (flag17)
					{
						flag5 = true;
					}
					if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
					{
						flag7 = true;
					}
					if (flag13)
					{
						flag10 = true;
					}
					flag12 = true;
					flag13 = false;
				}
				else if (characterAnalyzer.IsBreak())
				{
					if (flag17)
					{
						flag6 = true;
					}
					if (characterAnalyzer.Buffer.Position >= characterAnalyzer.Buffer.Length - 1)
					{
						flag8 = true;
					}
					if (flag12)
					{
						flag11 = true;
					}
					flag12 = false;
					flag13 = true;
				}
				else
				{
					flag12 = false;
					flag13 = false;
				}
				flag3 = characterAnalyzer.IsWhiteBreakOrZero();
				characterAnalyzer.Skip(1);
				if (!characterAnalyzer.EndOfInput)
				{
					flag4 = characterAnalyzer.IsWhiteBreakOrZero(1);
				}
				flag17 = false;
			}
			scalarData.isFlowPlainAllowed = true;
			scalarData.isBlockPlainAllowed = true;
			scalarData.isSingleQuotedAllowed = true;
			scalarData.isBlockAllowed = true;
			if (flag5 || flag6 || flag7 || flag8 || flag9)
			{
				scalarData.isFlowPlainAllowed = false;
				scalarData.isBlockPlainAllowed = false;
			}
			if (flag7)
			{
				scalarData.isBlockAllowed = false;
			}
			if (flag10)
			{
				scalarData.isFlowPlainAllowed = false;
				scalarData.isBlockPlainAllowed = false;
				scalarData.isSingleQuotedAllowed = false;
			}
			if (flag11 || flag15)
			{
				scalarData.isFlowPlainAllowed = false;
				scalarData.isBlockPlainAllowed = false;
				scalarData.isSingleQuotedAllowed = false;
				scalarData.isBlockAllowed = false;
			}
			scalarData.isMultiline = flag14;
			if (flag14)
			{
				scalarData.isFlowPlainAllowed = false;
				scalarData.isBlockPlainAllowed = false;
			}
			if (flag)
			{
				scalarData.isFlowPlainAllowed = false;
			}
			if (flag2)
			{
				scalarData.isBlockPlainAllowed = false;
			}
			scalarData.hasSingleQuotes = flag16;
		}

		private bool ValueIsRepresentableInOutputEncoding(string value)
		{
			if (outputUsesUnicodeEncoding)
			{
				return true;
			}
			try
			{
				byte[] bytes = output.Encoding.GetBytes(value);
				return output.Encoding.GetString(bytes, 0, bytes.Length).Equals(value);
			}
			catch (EncoderFallbackException)
			{
				return false;
			}
			catch (ArgumentOutOfRangeException)
			{
				return false;
			}
		}

		private bool IsUnicode(Encoding encoding)
		{
			if (!(encoding is UTF8Encoding) && !(encoding is UnicodeEncoding) && !(encoding is UTF7Encoding))
			{
				return encoding is UTF8Encoding;
			}
			return true;
		}

		private void AnalyzeTag(string tag)
		{
			tagData.handle = tag;
			foreach (TagDirective tagDirective in tagDirectives)
			{
				if (tag.StartsWith(tagDirective.Prefix, StringComparison.Ordinal))
				{
					tagData.handle = tagDirective.Handle;
					tagData.suffix = tag.Substring(tagDirective.Prefix.Length);
					break;
				}
			}
		}

		private void StateMachine(ParsingEvent evt)
		{
			if (evt is YamlDotNet.Core.Events.Comment comment)
			{
				EmitComment(comment);
				return;
			}
			switch (state)
			{
			case EmitterState.StreamStart:
				EmitStreamStart(evt);
				break;
			case EmitterState.FirstDocumentStart:
				EmitDocumentStart(evt, isFirst: true);
				break;
			case EmitterState.DocumentStart:
				EmitDocumentStart(evt, isFirst: false);
				break;
			case EmitterState.DocumentContent:
				EmitDocumentContent(evt);
				break;
			case EmitterState.DocumentEnd:
				EmitDocumentEnd(evt);
				break;
			case EmitterState.FlowSequenceFirstItem:
				EmitFlowSequenceItem(evt, isFirst: true);
				break;
			case EmitterState.FlowSequenceItem:
				EmitFlowSequenceItem(evt, isFirst: false);
				break;
			case EmitterState.FlowMappingFirstKey:
				EmitFlowMappingKey(evt, isFirst: true);
				break;
			case EmitterState.FlowMappingKey:
				EmitFlowMappingKey(evt, isFirst: false);
				break;
			case EmitterState.FlowMappingSimpleValue:
				EmitFlowMappingValue(evt, isSimple: true);
				break;
			case EmitterState.FlowMappingValue:
				EmitFlowMappingValue(evt, isSimple: false);
				break;
			case EmitterState.BlockSequenceFirstItem:
				EmitBlockSequenceItem(evt, isFirst: true);
				break;
			case EmitterState.BlockSequenceItem:
				EmitBlockSequenceItem(evt, isFirst: false);
				break;
			case EmitterState.BlockMappingFirstKey:
				EmitBlockMappingKey(evt, isFirst: true);
				break;
			case EmitterState.BlockMappingKey:
				EmitBlockMappingKey(evt, isFirst: false);
				break;
			case EmitterState.BlockMappingSimpleValue:
				EmitBlockMappingValue(evt, isSimple: true);
				break;
			case EmitterState.BlockMappingValue:
				EmitBlockMappingValue(evt, isSimple: false);
				break;
			case EmitterState.StreamEnd:
				throw new YamlException("Expected nothing after STREAM-END");
			default:
				throw new InvalidOperationException();
			}
		}

		private void EmitComment(YamlDotNet.Core.Events.Comment comment)
		{
			if (comment.IsInline)
			{
				Write(' ');
			}
			else
			{
				WriteIndent();
			}
			Write("# ");
			Write(comment.Value);
			WriteBreak();
			isIndentation = true;
		}

		private void EmitStreamStart(ParsingEvent evt)
		{
			if (!(evt is YamlDotNet.Core.Events.StreamStart))
			{
				throw new ArgumentException("Expected STREAM-START.", "evt");
			}
			indent = -1;
			column = 0;
			isWhitespace = true;
			isIndentation = true;
			state = EmitterState.FirstDocumentStart;
		}

		private void EmitDocumentStart(ParsingEvent evt, bool isFirst)
		{
			if (evt is YamlDotNet.Core.Events.DocumentStart documentStart)
			{
				bool flag = documentStart.IsImplicit && isFirst && !isCanonical;
				TagDirectiveCollection tagDirectiveCollection = NonDefaultTagsAmong(documentStart.Tags);
				if (!isFirst && !isDocumentEndWritten && (documentStart.Version != null || tagDirectiveCollection.Count > 0))
				{
					isDocumentEndWritten = false;
					WriteIndicator("...", needWhitespace: true, whitespace: false, indentation: false);
					WriteIndent();
				}
				if (documentStart.Version != null)
				{
					AnalyzeVersionDirective(documentStart.Version);
					flag = false;
					WriteIndicator("%YAML", needWhitespace: true, whitespace: false, indentation: false);
					WriteIndicator(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", 1, 1), needWhitespace: true, whitespace: false, indentation: false);
					WriteIndent();
				}
				foreach (TagDirective item in tagDirectiveCollection)
				{
					AppendTagDirectiveTo(item, allowDuplicates: false, tagDirectives);
				}
				TagDirective[] defaultTagDirectives = Constants.DefaultTagDirectives;
				for (int i = 0; i < defaultTagDirectives.Length; i++)
				{
					AppendTagDirectiveTo(defaultTagDirectives[i], allowDuplicates: true, tagDirectives);
				}
				if (tagDirectiveCollection.Count > 0)
				{
					flag = false;
					defaultTagDirectives = Constants.DefaultTagDirectives;
					for (int i = 0; i < defaultTagDirectives.Length; i++)
					{
						AppendTagDirectiveTo(defaultTagDirectives[i], allowDuplicates: true, tagDirectiveCollection);
					}
					foreach (TagDirective item2 in tagDirectiveCollection)
					{
						WriteIndicator("%TAG", needWhitespace: true, whitespace: false, indentation: false);
						WriteTagHandle(item2.Handle);
						WriteTagContent(item2.Prefix, needsWhitespace: true);
						WriteIndent();
					}
				}
				if (CheckEmptyDocument())
				{
					flag = false;
				}
				if (!flag)
				{
					WriteIndent();
					WriteIndicator("---", needWhitespace: true, whitespace: false, indentation: false);
					if (isCanonical)
					{
						WriteIndent();
					}
				}
				state = EmitterState.DocumentContent;
			}
			else
			{
				if (!(evt is YamlDotNet.Core.Events.StreamEnd))
				{
					throw new YamlException("Expected DOCUMENT-START or STREAM-END");
				}
				if (isOpenEnded)
				{
					WriteIndicator("...", needWhitespace: true, whitespace: false, indentation: false);
					WriteIndent();
				}
				state = EmitterState.StreamEnd;
			}
		}

		private TagDirectiveCollection NonDefaultTagsAmong(IEnumerable<TagDirective> tagCollection)
		{
			TagDirectiveCollection tagDirectiveCollection = new TagDirectiveCollection();
			if (tagCollection == null)
			{
				return tagDirectiveCollection;
			}
			foreach (TagDirective item2 in tagCollection)
			{
				AppendTagDirectiveTo(item2, allowDuplicates: false, tagDirectiveCollection);
			}
			TagDirective[] defaultTagDirectives = Constants.DefaultTagDirectives;
			foreach (TagDirective item in defaultTagDirectives)
			{
				tagDirectiveCollection.Remove(item);
			}
			return tagDirectiveCollection;
		}

		private void AnalyzeVersionDirective(VersionDirective versionDirective)
		{
			if (versionDirective.Version.Major != 1 || versionDirective.Version.Minor != 1)
			{
				throw new YamlException("Incompatible %YAML directive");
			}
		}

		private static void AppendTagDirectiveTo(TagDirective value, bool allowDuplicates, TagDirectiveCollection tagDirectives)
		{
			if (tagDirectives.Contains(value))
			{
				if (!allowDuplicates)
				{
					throw new YamlException("Duplicate %TAG directive.");
				}
			}
			else
			{
				tagDirectives.Add(value);
			}
		}

		private void EmitDocumentContent(ParsingEvent evt)
		{
			states.Push(EmitterState.DocumentEnd);
			EmitNode(evt, isRoot: true, isMapping: false, isSimpleKey: false);
		}

		private void EmitNode(ParsingEvent evt, bool isRoot, bool isMapping, bool isSimpleKey)
		{
			isRootContext = isRoot;
			isMappingContext = isMapping;
			isSimpleKeyContext = isSimpleKey;
			switch (evt.Type)
			{
			case EventType.Alias:
				EmitAlias();
				break;
			case EventType.Scalar:
				EmitScalar(evt);
				break;
			case EventType.SequenceStart:
				EmitSequenceStart(evt);
				break;
			case EventType.MappingStart:
				EmitMappingStart(evt);
				break;
			default:
				throw new YamlException($"Expected SCALAR, SEQUENCE-START, MAPPING-START, or ALIAS, got {evt.Type}");
			}
		}

		private void EmitAlias()
		{
			ProcessAnchor();
			state = states.Pop();
		}

		private void EmitScalar(ParsingEvent evt)
		{
			SelectScalarStyle(evt);
			ProcessAnchor();
			ProcessTag();
			IncreaseIndent(isFlow: true, isIndentless: false);
			ProcessScalar();
			indent = indents.Pop();
			state = states.Pop();
		}

		private void SelectScalarStyle(ParsingEvent evt)
		{
			YamlDotNet.Core.Events.Scalar scalar = (YamlDotNet.Core.Events.Scalar)evt;
			ScalarStyle scalarStyle = scalar.Style;
			bool flag = tagData.handle == null && tagData.suffix == null;
			if (flag && !scalar.IsPlainImplicit && !scalar.IsQuotedImplicit)
			{
				throw new YamlException("Neither tag nor isImplicit flags are specified.");
			}
			if (scalarStyle == ScalarStyle.Any)
			{
				scalarStyle = ((!scalarData.isMultiline) ? ScalarStyle.Plain : ScalarStyle.Folded);
			}
			if (isCanonical)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if (isSimpleKeyContext && scalarData.isMultiline)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if (scalarStyle == ScalarStyle.Plain)
			{
				if ((flowLevel != 0 && !scalarData.isFlowPlainAllowed) || (flowLevel == 0 && !scalarData.isBlockPlainAllowed))
				{
					scalarStyle = ((scalarData.isSingleQuotedAllowed && !scalarData.hasSingleQuotes) ? ScalarStyle.SingleQuoted : ScalarStyle.DoubleQuoted);
				}
				if (string.IsNullOrEmpty(scalarData.value) && (flowLevel != 0 || isSimpleKeyContext))
				{
					scalarStyle = ScalarStyle.SingleQuoted;
				}
				if (flag && !scalar.IsPlainImplicit)
				{
					scalarStyle = ScalarStyle.SingleQuoted;
				}
			}
			if (scalarStyle == ScalarStyle.SingleQuoted && !scalarData.isSingleQuotedAllowed)
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			if ((scalarStyle == ScalarStyle.Literal || scalarStyle == ScalarStyle.Folded) && (!scalarData.isBlockAllowed || flowLevel != 0 || isSimpleKeyContext))
			{
				scalarStyle = ScalarStyle.DoubleQuoted;
			}
			scalarData.style = scalarStyle;
		}

		private void ProcessScalar()
		{
			switch (scalarData.style)
			{
			case ScalarStyle.Plain:
				WritePlainScalar(scalarData.value, !isSimpleKeyContext);
				break;
			case ScalarStyle.SingleQuoted:
				WriteSingleQuotedScalar(scalarData.value, !isSimpleKeyContext);
				break;
			case ScalarStyle.DoubleQuoted:
				WriteDoubleQuotedScalar(scalarData.value, !isSimpleKeyContext);
				break;
			case ScalarStyle.Literal:
				WriteLiteralScalar(scalarData.value);
				break;
			case ScalarStyle.Folded:
				WriteFoldedScalar(scalarData.value);
				break;
			default:
				throw new InvalidOperationException();
			}
		}

		private void WritePlainScalar(string value, bool allowBreaks)
		{
			if (!isWhitespace)
			{
				Write(' ');
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (IsSpace(c))
				{
					if (allowBreaks && !flag && column > bestWidth && i + 1 < value.Length && value[i + 1] != ' ')
					{
						WriteIndent();
					}
					else
					{
						Write(c);
					}
					flag = true;
					continue;
				}
				if (IsBreak(c, out var breakChar))
				{
					if (!flag2 && c == '\n')
					{
						WriteBreak();
					}
					WriteBreak(breakChar);
					isIndentation = true;
					flag2 = true;
					continue;
				}
				if (flag2)
				{
					WriteIndent();
				}
				Write(c);
				isIndentation = false;
				flag = false;
				flag2 = false;
			}
			isWhitespace = false;
			isIndentation = false;
			if (isRootContext)
			{
				isOpenEnded = true;
			}
		}

		private void WriteSingleQuotedScalar(string value, bool allowBreaks)
		{
			WriteIndicator("'", needWhitespace: true, whitespace: false, indentation: false);
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (c == ' ')
				{
					if (allowBreaks && !flag && column > bestWidth && i != 0 && i + 1 < value.Length && value[i + 1] != ' ')
					{
						WriteIndent();
					}
					else
					{
						Write(c);
					}
					flag = true;
					continue;
				}
				if (IsBreak(c, out var breakChar))
				{
					if (!flag2 && c == '\n')
					{
						WriteBreak();
					}
					WriteBreak(breakChar);
					isIndentation = true;
					flag2 = true;
					continue;
				}
				if (flag2)
				{
					WriteIndent();
				}
				if (c == '\'')
				{
					Write(c);
				}
				Write(c);
				isIndentation = false;
				flag = false;
				flag2 = false;
			}
			WriteIndicator("'", needWhitespace: false, whitespace: false, indentation: false);
			isWhitespace = false;
			isIndentation = false;
		}

		private void WriteDoubleQuotedScalar(string value, bool allowBreaks)
		{
			WriteIndicator("\"", needWhitespace: true, whitespace: false, indentation: false);
			bool flag = false;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (IsPrintable(c) && !IsBreak(c, out var _))
				{
					switch (c)
					{
					case '"':
					case '\\':
						break;
					case ' ':
						if (allowBreaks && !flag && column > bestWidth && i > 0 && i + 1 < value.Length)
						{
							WriteIndent();
							if (value[i + 1] == ' ')
							{
								Write('\\');
							}
						}
						else
						{
							Write(c);
						}
						flag = true;
						continue;
					default:
						Write(c);
						flag = false;
						continue;
					}
				}
				Write('\\');
				switch (c)
				{
				case '\0':
					Write('0');
					break;
				case '\a':
					Write('a');
					break;
				case '\b':
					Write('b');
					break;
				case '\t':
					Write('t');
					break;
				case '\n':
					Write('n');
					break;
				case '\v':
					Write('v');
					break;
				case '\f':
					Write('f');
					break;
				case '\r':
					Write('r');
					break;
				case '\u001b':
					Write('e');
					break;
				case '"':
					Write('"');
					break;
				case '\\':
					Write('\\');
					break;
				case '\u0085':
					Write('N');
					break;
				case '\u00a0':
					Write('_');
					break;
				case '\u2028':
					Write('L');
					break;
				case '\u2029':
					Write('P');
					break;
				default:
				{
					ushort num = c;
					if (num <= 255)
					{
						Write('x');
						Write(num.ToString("X02", CultureInfo.InvariantCulture));
					}
					else if (IsHighSurrogate(c))
					{
						if (i + 1 >= value.Length || !IsLowSurrogate(value[i + 1]))
						{
							throw new SyntaxErrorException("While writing a quoted scalar, found an orphaned high surrogate.");
						}
						Write('U');
						Write(char.ConvertToUtf32(c, value[i + 1]).ToString("X08", CultureInfo.InvariantCulture));
						i++;
					}
					else
					{
						Write('u');
						Write(num.ToString("X04", CultureInfo.InvariantCulture));
					}
					break;
				}
				}
				flag = false;
			}
			WriteIndicator("\"", needWhitespace: false, whitespace: false, indentation: false);
			isWhitespace = false;
			isIndentation = false;
		}

		private void WriteLiteralScalar(string value)
		{
			bool flag = true;
			WriteIndicator("|", needWhitespace: true, whitespace: false, indentation: false);
			WriteBlockScalarHints(value);
			WriteBreak();
			isIndentation = true;
			isWhitespace = true;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (c == '\r' && i + 1 < value.Length && value[i + 1] == '\n')
				{
					continue;
				}
				if (IsBreak(c, out var breakChar))
				{
					WriteBreak(breakChar);
					isIndentation = true;
					flag = true;
					continue;
				}
				if (flag)
				{
					WriteIndent();
				}
				Write(c);
				isIndentation = false;
				flag = false;
			}
		}

		private void WriteFoldedScalar(string value)
		{
			bool flag = true;
			bool flag2 = true;
			WriteIndicator(">", needWhitespace: true, whitespace: false, indentation: false);
			WriteBlockScalarHints(value);
			WriteBreak();
			isIndentation = true;
			isWhitespace = true;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (IsBreak(c, out var breakChar))
				{
					if (!flag && !flag2 && c == '\n')
					{
						int j;
						char breakChar2;
						for (j = 0; i + j < value.Length && IsBreak(value[i + j], out breakChar2); j++)
						{
						}
						if (i + j < value.Length && !IsBlank(value[i + j]) && !IsBreak(value[i + j], out breakChar2))
						{
							WriteBreak();
						}
					}
					WriteBreak(breakChar);
					isIndentation = true;
					flag = true;
				}
				else
				{
					if (flag)
					{
						WriteIndent();
						flag2 = IsBlank(c);
					}
					if (!flag && c == ' ' && i + 1 < value.Length && value[i + 1] != ' ' && column > bestWidth)
					{
						WriteIndent();
					}
					else
					{
						Write(c);
					}
					isIndentation = false;
					flag = false;
				}
			}
		}

		private static bool IsSpace(char character)
		{
			return character == ' ';
		}

		private static bool IsBreak(char character, out char breakChar)
		{
			switch (character)
			{
			case '\n':
			case '\r':
			case '\u0085':
				breakChar = '\n';
				return true;
			case '\u2028':
			case '\u2029':
				breakChar = character;
				return true;
			default:
				breakChar = '\0';
				return false;
			}
		}

		private static bool IsBlank(char character)
		{
			if (character != ' ')
			{
				return character == '\t';
			}
			return true;
		}

		private static bool IsPrintable(char character)
		{
			switch (character)
			{
			default:
				if (character != '\u0085' && (character < '\u00a0' || character > '\ud7ff'))
				{
					if (character >= '\ue000')
					{
						return character <= '\ufffd';
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

		private static bool IsHighSurrogate(char c)
		{
			if ('\ud800' <= c)
			{
				return c <= '\udbff';
			}
			return false;
		}

		private static bool IsLowSurrogate(char c)
		{
			if ('\udc00' <= c)
			{
				return c <= '\udfff';
			}
			return false;
		}

		private void EmitSequenceStart(ParsingEvent evt)
		{
			ProcessAnchor();
			ProcessTag();
			SequenceStart sequenceStart = (SequenceStart)evt;
			if (flowLevel != 0 || isCanonical || sequenceStart.Style == SequenceStyle.Flow || CheckEmptySequence())
			{
				state = EmitterState.FlowSequenceFirstItem;
			}
			else
			{
				state = EmitterState.BlockSequenceFirstItem;
			}
		}

		private void EmitMappingStart(ParsingEvent evt)
		{
			ProcessAnchor();
			ProcessTag();
			MappingStart mappingStart = (MappingStart)evt;
			if (flowLevel != 0 || isCanonical || mappingStart.Style == MappingStyle.Flow || CheckEmptyMapping())
			{
				state = EmitterState.FlowMappingFirstKey;
			}
			else
			{
				state = EmitterState.BlockMappingFirstKey;
			}
		}

		private void ProcessAnchor()
		{
			if (anchorData.anchor != null)
			{
				WriteIndicator(anchorData.isAlias ? "*" : "&", needWhitespace: true, whitespace: false, indentation: false);
				WriteAnchor(anchorData.anchor);
			}
		}

		private void ProcessTag()
		{
			if (tagData.handle == null && tagData.suffix == null)
			{
				return;
			}
			if (tagData.handle != null)
			{
				WriteTagHandle(tagData.handle);
				if (tagData.suffix != null)
				{
					WriteTagContent(tagData.suffix, needsWhitespace: false);
				}
			}
			else
			{
				WriteIndicator("!<", needWhitespace: true, whitespace: false, indentation: false);
				WriteTagContent(tagData.suffix, needsWhitespace: false);
				WriteIndicator(">", needWhitespace: false, whitespace: false, indentation: false);
			}
		}

		private void EmitDocumentEnd(ParsingEvent evt)
		{
			if (evt is YamlDotNet.Core.Events.DocumentEnd documentEnd)
			{
				WriteIndent();
				if (!documentEnd.IsImplicit)
				{
					WriteIndicator("...", needWhitespace: true, whitespace: false, indentation: false);
					WriteIndent();
					isDocumentEndWritten = true;
				}
				state = EmitterState.DocumentStart;
				tagDirectives.Clear();
				return;
			}
			throw new YamlException("Expected DOCUMENT-END.");
		}

		private void EmitFlowSequenceItem(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				WriteIndicator("[", needWhitespace: true, whitespace: true, indentation: false);
				IncreaseIndent(isFlow: true, isIndentless: false);
				flowLevel++;
			}
			if (evt is SequenceEnd)
			{
				flowLevel--;
				indent = indents.Pop();
				if (isCanonical && !isFirst)
				{
					WriteIndicator(",", needWhitespace: false, whitespace: false, indentation: false);
					WriteIndent();
				}
				WriteIndicator("]", needWhitespace: false, whitespace: false, indentation: false);
				state = states.Pop();
			}
			else
			{
				if (!isFirst)
				{
					WriteIndicator(",", needWhitespace: false, whitespace: false, indentation: false);
				}
				if (isCanonical || column > bestWidth)
				{
					WriteIndent();
				}
				states.Push(EmitterState.FlowSequenceItem);
				EmitNode(evt, isRoot: false, isMapping: false, isSimpleKey: false);
			}
		}

		private void EmitFlowMappingKey(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				WriteIndicator("{", needWhitespace: true, whitespace: true, indentation: false);
				IncreaseIndent(isFlow: true, isIndentless: false);
				flowLevel++;
			}
			if (evt is MappingEnd)
			{
				flowLevel--;
				indent = indents.Pop();
				if (isCanonical && !isFirst)
				{
					WriteIndicator(",", needWhitespace: false, whitespace: false, indentation: false);
					WriteIndent();
				}
				WriteIndicator("}", needWhitespace: false, whitespace: false, indentation: false);
				state = states.Pop();
				return;
			}
			if (!isFirst)
			{
				WriteIndicator(",", needWhitespace: false, whitespace: false, indentation: false);
			}
			if (isCanonical || column > bestWidth)
			{
				WriteIndent();
			}
			if (!isCanonical && CheckSimpleKey())
			{
				states.Push(EmitterState.FlowMappingSimpleValue);
				EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: true);
			}
			else
			{
				WriteIndicator("?", needWhitespace: true, whitespace: false, indentation: false);
				states.Push(EmitterState.FlowMappingValue);
				EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: false);
			}
		}

		private void EmitFlowMappingValue(ParsingEvent evt, bool isSimple)
		{
			if (isSimple)
			{
				WriteIndicator(":", needWhitespace: false, whitespace: false, indentation: false);
			}
			else
			{
				if (isCanonical || column > bestWidth)
				{
					WriteIndent();
				}
				WriteIndicator(":", needWhitespace: true, whitespace: false, indentation: false);
			}
			states.Push(EmitterState.FlowMappingKey);
			EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: false);
		}

		private void EmitBlockSequenceItem(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				IncreaseIndent(isFlow: false, isMappingContext && !isIndentation);
			}
			if (evt is SequenceEnd)
			{
				indent = indents.Pop();
				state = states.Pop();
				return;
			}
			WriteIndent();
			WriteIndicator("-", needWhitespace: true, whitespace: false, indentation: true);
			states.Push(EmitterState.BlockSequenceItem);
			EmitNode(evt, isRoot: false, isMapping: false, isSimpleKey: false);
		}

		private void EmitBlockMappingKey(ParsingEvent evt, bool isFirst)
		{
			if (isFirst)
			{
				IncreaseIndent(isFlow: false, isIndentless: false);
			}
			if (evt is MappingEnd)
			{
				indent = indents.Pop();
				state = states.Pop();
				return;
			}
			WriteIndent();
			if (CheckSimpleKey())
			{
				states.Push(EmitterState.BlockMappingSimpleValue);
				EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: true);
			}
			else
			{
				WriteIndicator("?", needWhitespace: true, whitespace: false, indentation: true);
				states.Push(EmitterState.BlockMappingValue);
				EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: false);
			}
		}

		private void EmitBlockMappingValue(ParsingEvent evt, bool isSimple)
		{
			if (isSimple)
			{
				WriteIndicator(":", needWhitespace: false, whitespace: false, indentation: false);
			}
			else
			{
				WriteIndent();
				WriteIndicator(":", needWhitespace: true, whitespace: false, indentation: true);
			}
			states.Push(EmitterState.BlockMappingKey);
			EmitNode(evt, isRoot: false, isMapping: true, isSimpleKey: false);
		}

		private void IncreaseIndent(bool isFlow, bool isIndentless)
		{
			indents.Push(indent);
			if (indent < 0)
			{
				indent = (isFlow ? bestIndent : 0);
			}
			else if (!isIndentless)
			{
				indent += bestIndent;
			}
		}

		private bool CheckEmptyDocument()
		{
			int num = 0;
			foreach (ParsingEvent @event in events)
			{
				num++;
				if (num == 2)
				{
					if (@event is YamlDotNet.Core.Events.Scalar scalar)
					{
						return string.IsNullOrEmpty(scalar.Value);
					}
					break;
				}
			}
			return false;
		}

		private bool CheckSimpleKey()
		{
			if (events.Count < 1)
			{
				return false;
			}
			int num;
			switch (events.Peek().Type)
			{
			case EventType.Alias:
				num = SafeStringLength(anchorData.anchor);
				break;
			case EventType.Scalar:
				if (scalarData.isMultiline)
				{
					return false;
				}
				num = SafeStringLength(anchorData.anchor) + SafeStringLength(tagData.handle) + SafeStringLength(tagData.suffix) + SafeStringLength(scalarData.value);
				break;
			case EventType.SequenceStart:
				if (!CheckEmptySequence())
				{
					return false;
				}
				num = SafeStringLength(anchorData.anchor) + SafeStringLength(tagData.handle) + SafeStringLength(tagData.suffix);
				break;
			case EventType.MappingStart:
				if (!CheckEmptySequence())
				{
					return false;
				}
				num = SafeStringLength(anchorData.anchor) + SafeStringLength(tagData.handle) + SafeStringLength(tagData.suffix);
				break;
			default:
				return false;
			}
			return num <= 128;
		}

		private int SafeStringLength(string value)
		{
			return value?.Length ?? 0;
		}

		private bool CheckEmptySequence()
		{
			if (events.Count < 2)
			{
				return false;
			}
			FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>(events);
			if (fakeList[0] is SequenceStart)
			{
				return fakeList[1] is SequenceEnd;
			}
			return false;
		}

		private bool CheckEmptyMapping()
		{
			if (events.Count < 2)
			{
				return false;
			}
			FakeList<ParsingEvent> fakeList = new FakeList<ParsingEvent>(events);
			if (fakeList[0] is MappingStart)
			{
				return fakeList[1] is MappingEnd;
			}
			return false;
		}

		private void WriteBlockScalarHints(string value)
		{
			CharacterAnalyzer<StringLookAheadBuffer> characterAnalyzer = new CharacterAnalyzer<StringLookAheadBuffer>(new StringLookAheadBuffer(value));
			if (characterAnalyzer.IsSpace() || characterAnalyzer.IsBreak())
			{
				string indicator = string.Format(CultureInfo.InvariantCulture, "{0}", bestIndent);
				WriteIndicator(indicator, needWhitespace: false, whitespace: false, indentation: false);
			}
			isOpenEnded = false;
			string text = null;
			if (value.Length == 0 || !characterAnalyzer.IsBreak(value.Length - 1))
			{
				text = "-";
			}
			else if (value.Length >= 2 && characterAnalyzer.IsBreak(value.Length - 2))
			{
				text = "+";
				isOpenEnded = true;
			}
			if (text != null)
			{
				WriteIndicator(text, needWhitespace: false, whitespace: false, indentation: false);
			}
		}

		private void WriteIndicator(string indicator, bool needWhitespace, bool whitespace, bool indentation)
		{
			if (needWhitespace && !isWhitespace)
			{
				Write(' ');
			}
			Write(indicator);
			isWhitespace = whitespace;
			isIndentation &= indentation;
			isOpenEnded = false;
		}

		private void WriteIndent()
		{
			int num = Math.Max(indent, 0);
			if (!isIndentation || column > num || (column == num && !isWhitespace))
			{
				WriteBreak();
			}
			while (column < num)
			{
				Write(' ');
			}
			isWhitespace = true;
			isIndentation = true;
		}

		private void WriteAnchor(string value)
		{
			Write(value);
			isWhitespace = false;
			isIndentation = false;
		}

		private void WriteTagHandle(string value)
		{
			if (!isWhitespace)
			{
				Write(' ');
			}
			Write(value);
			isWhitespace = false;
			isIndentation = false;
		}

		private void WriteTagContent(string value, bool needsWhitespace)
		{
			if (needsWhitespace && !isWhitespace)
			{
				Write(' ');
			}
			Write(UrlEncode(value));
			isWhitespace = false;
			isIndentation = false;
		}

		private string UrlEncode(string text)
		{
			return uriReplacer.Replace(text, delegate(Match match)
			{
				StringBuilder stringBuilder = new StringBuilder();
				byte[] bytes = Encoding.UTF8.GetBytes(match.Value);
				foreach (byte b in bytes)
				{
					stringBuilder.AppendFormat("%{0:X02}", b);
				}
				return stringBuilder.ToString();
			});
		}

		private void Write(char value)
		{
			output.Write(value);
			column++;
		}

		private void Write(string value)
		{
			output.Write(value);
			column += value.Length;
		}

		private void WriteBreak(char breakCharacter = '\n')
		{
			if (breakCharacter == '\n')
			{
				output.WriteLine();
			}
			else
			{
				output.Write(breakCharacter);
			}
			column = 0;
		}
	}
}
