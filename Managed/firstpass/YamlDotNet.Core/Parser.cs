using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;

namespace YamlDotNet.Core
{
	public class Parser : IParser
	{
		private class EventQueue
		{
			private readonly Queue<ParsingEvent> highPriorityEvents = new Queue<ParsingEvent>();

			private readonly Queue<ParsingEvent> normalPriorityEvents = new Queue<ParsingEvent>();

			public int Count => highPriorityEvents.Count + normalPriorityEvents.Count;

			public void Enqueue(ParsingEvent @event)
			{
				EventType type = @event.Type;
				if (type == EventType.StreamStart || type == EventType.DocumentStart)
				{
					highPriorityEvents.Enqueue(@event);
				}
				else
				{
					normalPriorityEvents.Enqueue(@event);
				}
			}

			public ParsingEvent Dequeue()
			{
				if (highPriorityEvents.Count <= 0)
				{
					return normalPriorityEvents.Dequeue();
				}
				return highPriorityEvents.Dequeue();
			}
		}

		private readonly Stack<ParserState> states = new Stack<ParserState>();

		private readonly TagDirectiveCollection tagDirectives = new TagDirectiveCollection();

		private ParserState state;

		private readonly IScanner scanner;

		private ParsingEvent currentEvent;

		private Token currentToken;

		private readonly EventQueue pendingEvents = new EventQueue();

		public ParsingEvent Current => currentEvent;

		private Token GetCurrentToken()
		{
			if (currentToken == null)
			{
				while (scanner.MoveNextWithoutConsuming())
				{
					currentToken = scanner.Current;
					if (!(currentToken is YamlDotNet.Core.Tokens.Comment comment))
					{
						break;
					}
					pendingEvents.Enqueue(new YamlDotNet.Core.Events.Comment(comment.Value, comment.IsInline, comment.Start, comment.End));
				}
			}
			return currentToken;
		}

		public Parser(TextReader input)
			: this(new Scanner(input))
		{
		}

		public Parser(IScanner scanner)
		{
			this.scanner = scanner;
		}

		public bool MoveNext()
		{
			if (state == ParserState.StreamEnd)
			{
				currentEvent = null;
				return false;
			}
			if (pendingEvents.Count == 0)
			{
				pendingEvents.Enqueue(StateMachine());
			}
			currentEvent = pendingEvents.Dequeue();
			return true;
		}

		private ParsingEvent StateMachine()
		{
			switch (state)
			{
			case ParserState.StreamStart:
				return ParseStreamStart();
			case ParserState.ImplicitDocumentStart:
				return ParseDocumentStart(isImplicit: true);
			case ParserState.DocumentStart:
				return ParseDocumentStart(isImplicit: false);
			case ParserState.DocumentContent:
				return ParseDocumentContent();
			case ParserState.DocumentEnd:
				return ParseDocumentEnd();
			case ParserState.BlockNode:
				return ParseNode(isBlock: true, isIndentlessSequence: false);
			case ParserState.BlockNodeOrIndentlessSequence:
				return ParseNode(isBlock: true, isIndentlessSequence: true);
			case ParserState.FlowNode:
				return ParseNode(isBlock: false, isIndentlessSequence: false);
			case ParserState.BlockSequenceFirstEntry:
				return ParseBlockSequenceEntry(isFirst: true);
			case ParserState.BlockSequenceEntry:
				return ParseBlockSequenceEntry(isFirst: false);
			case ParserState.IndentlessSequenceEntry:
				return ParseIndentlessSequenceEntry();
			case ParserState.BlockMappingFirstKey:
				return ParseBlockMappingKey(isFirst: true);
			case ParserState.BlockMappingKey:
				return ParseBlockMappingKey(isFirst: false);
			case ParserState.BlockMappingValue:
				return ParseBlockMappingValue();
			case ParserState.FlowSequenceFirstEntry:
				return ParseFlowSequenceEntry(isFirst: true);
			case ParserState.FlowSequenceEntry:
				return ParseFlowSequenceEntry(isFirst: false);
			case ParserState.FlowSequenceEntryMappingKey:
				return ParseFlowSequenceEntryMappingKey();
			case ParserState.FlowSequenceEntryMappingValue:
				return ParseFlowSequenceEntryMappingValue();
			case ParserState.FlowSequenceEntryMappingEnd:
				return ParseFlowSequenceEntryMappingEnd();
			case ParserState.FlowMappingFirstKey:
				return ParseFlowMappingKey(isFirst: true);
			case ParserState.FlowMappingKey:
				return ParseFlowMappingKey(isFirst: false);
			case ParserState.FlowMappingValue:
				return ParseFlowMappingValue(isEmpty: false);
			case ParserState.FlowMappingEmptyValue:
				return ParseFlowMappingValue(isEmpty: true);
			default:
				Debug.Assert(condition: false, "Invalid state");
				throw new InvalidOperationException();
			}
		}

		private void Skip()
		{
			if (currentToken != null)
			{
				currentToken = null;
				scanner.ConsumeCurrent();
			}
		}

		private ParsingEvent ParseStreamStart()
		{
			if (!(GetCurrentToken() is YamlDotNet.Core.Tokens.StreamStart streamStart))
			{
				Token token = GetCurrentToken();
				throw new SemanticErrorException(token.Start, token.End, "Did not find expected <stream-start>.");
			}
			Skip();
			state = ParserState.ImplicitDocumentStart;
			return new YamlDotNet.Core.Events.StreamStart(streamStart.Start, streamStart.End);
		}

		private ParsingEvent ParseDocumentStart(bool isImplicit)
		{
			if (!isImplicit)
			{
				while (GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd)
				{
					Skip();
				}
			}
			if (isImplicit && !(GetCurrentToken() is VersionDirective) && !(GetCurrentToken() is TagDirective) && !(GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentStart) && !(GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd))
			{
				TagDirectiveCollection tags = new TagDirectiveCollection();
				ProcessDirectives(tags);
				states.Push(ParserState.DocumentEnd);
				state = ParserState.BlockNode;
				return new YamlDotNet.Core.Events.DocumentStart(null, tags, isImplicit: true, GetCurrentToken().Start, GetCurrentToken().End);
			}
			if (!(GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd))
			{
				Mark start = GetCurrentToken().Start;
				TagDirectiveCollection tags2 = new TagDirectiveCollection();
				VersionDirective version = ProcessDirectives(tags2);
				Token token = GetCurrentToken();
				if (!(token is YamlDotNet.Core.Tokens.DocumentStart))
				{
					throw new SemanticErrorException(token.Start, token.End, "Did not find expected <document start>.");
				}
				states.Push(ParserState.DocumentEnd);
				state = ParserState.DocumentContent;
				YamlDotNet.Core.Events.DocumentStart result = new YamlDotNet.Core.Events.DocumentStart(version, tags2, isImplicit: false, start, token.End);
				Skip();
				return result;
			}
			state = ParserState.StreamEnd;
			YamlDotNet.Core.Events.StreamEnd result2 = new YamlDotNet.Core.Events.StreamEnd(GetCurrentToken().Start, GetCurrentToken().End);
			if (scanner.MoveNextWithoutConsuming())
			{
				throw new InvalidOperationException("The scanner should contain no more tokens.");
			}
			return result2;
		}

		private VersionDirective ProcessDirectives(TagDirectiveCollection tags)
		{
			VersionDirective versionDirective = null;
			bool flag = false;
			while (true)
			{
				if (GetCurrentToken() is VersionDirective versionDirective2)
				{
					if (versionDirective != null)
					{
						throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found duplicate %YAML directive.");
					}
					if (versionDirective2.Version.Major != 1 || versionDirective2.Version.Minor != 1)
					{
						throw new SemanticErrorException(versionDirective2.Start, versionDirective2.End, "Found incompatible YAML document.");
					}
					versionDirective = versionDirective2;
					flag = true;
				}
				else
				{
					if (!(GetCurrentToken() is TagDirective tagDirective))
					{
						break;
					}
					if (tags.Contains(tagDirective.Handle))
					{
						throw new SemanticErrorException(tagDirective.Start, tagDirective.End, "Found duplicate %TAG directive.");
					}
					tags.Add(tagDirective);
					flag = true;
				}
				Skip();
			}
			AddTagDirectives(tags, Constants.DefaultTagDirectives);
			if (flag)
			{
				tagDirectives.Clear();
			}
			AddTagDirectives(tagDirectives, tags);
			return versionDirective;
		}

		private static void AddTagDirectives(TagDirectiveCollection directives, IEnumerable<TagDirective> source)
		{
			foreach (TagDirective item in source)
			{
				if (!directives.Contains(item))
				{
					directives.Add(item);
				}
			}
		}

		private ParsingEvent ParseDocumentContent()
		{
			if (GetCurrentToken() is VersionDirective || GetCurrentToken() is TagDirective || GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentStart || GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd || GetCurrentToken() is YamlDotNet.Core.Tokens.StreamEnd)
			{
				state = states.Pop();
				return ProcessEmptyScalar(scanner.CurrentPosition);
			}
			return ParseNode(isBlock: true, isIndentlessSequence: false);
		}

		private static ParsingEvent ProcessEmptyScalar(Mark position)
		{
			return new YamlDotNet.Core.Events.Scalar(null, null, string.Empty, ScalarStyle.Plain, isPlainImplicit: true, isQuotedImplicit: false, position, position);
		}

		private ParsingEvent ParseNode(bool isBlock, bool isIndentlessSequence)
		{
			if (GetCurrentToken() is YamlDotNet.Core.Tokens.AnchorAlias anchorAlias)
			{
				state = states.Pop();
				YamlDotNet.Core.Events.AnchorAlias result = new YamlDotNet.Core.Events.AnchorAlias(anchorAlias.Value, anchorAlias.Start, anchorAlias.End);
				Skip();
				return result;
			}
			Mark start = GetCurrentToken().Start;
			Anchor anchor = null;
			YamlDotNet.Core.Tokens.Tag tag = null;
			while (true)
			{
				if (anchor == null && (anchor = GetCurrentToken() as Anchor) != null)
				{
					Skip();
					continue;
				}
				if (tag != null || (tag = GetCurrentToken() as YamlDotNet.Core.Tokens.Tag) == null)
				{
					break;
				}
				Skip();
			}
			string text = null;
			if (tag != null)
			{
				if (string.IsNullOrEmpty(tag.Handle))
				{
					text = tag.Suffix;
				}
				else
				{
					if (!tagDirectives.Contains(tag.Handle))
					{
						throw new SemanticErrorException(tag.Start, tag.End, "While parsing a node, find undefined tag handle.");
					}
					text = tagDirectives[tag.Handle].Prefix + tag.Suffix;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = null;
			}
			string text2 = ((anchor == null) ? null : (string.IsNullOrEmpty(anchor.Value) ? null : anchor.Value));
			bool flag = string.IsNullOrEmpty(text);
			if (isIndentlessSequence && GetCurrentToken() is BlockEntry)
			{
				state = ParserState.IndentlessSequenceEntry;
				return new SequenceStart(text2, text, flag, SequenceStyle.Block, start, GetCurrentToken().End);
			}
			if (GetCurrentToken() is YamlDotNet.Core.Tokens.Scalar scalar)
			{
				bool isPlainImplicit = false;
				bool isQuotedImplicit = false;
				if ((scalar.Style == ScalarStyle.Plain && text == null) || text == "!")
				{
					isPlainImplicit = true;
				}
				else if (text == null)
				{
					isQuotedImplicit = true;
				}
				state = states.Pop();
				YamlDotNet.Core.Events.Scalar result2 = new YamlDotNet.Core.Events.Scalar(text2, text, scalar.Value, scalar.Style, isPlainImplicit, isQuotedImplicit, start, scalar.End);
				Skip();
				return result2;
			}
			if (GetCurrentToken() is FlowSequenceStart flowSequenceStart)
			{
				state = ParserState.FlowSequenceFirstEntry;
				return new SequenceStart(text2, text, flag, SequenceStyle.Flow, start, flowSequenceStart.End);
			}
			if (GetCurrentToken() is FlowMappingStart flowMappingStart)
			{
				state = ParserState.FlowMappingFirstKey;
				return new MappingStart(text2, text, flag, MappingStyle.Flow, start, flowMappingStart.End);
			}
			if (isBlock)
			{
				if (GetCurrentToken() is BlockSequenceStart blockSequenceStart)
				{
					state = ParserState.BlockSequenceFirstEntry;
					return new SequenceStart(text2, text, flag, SequenceStyle.Block, start, blockSequenceStart.End);
				}
				if (GetCurrentToken() is BlockMappingStart)
				{
					state = ParserState.BlockMappingFirstKey;
					return new MappingStart(text2, text, flag, MappingStyle.Block, start, GetCurrentToken().End);
				}
			}
			if (text2 != null || tag != null)
			{
				state = states.Pop();
				return new YamlDotNet.Core.Events.Scalar(text2, text, string.Empty, ScalarStyle.Plain, flag, isQuotedImplicit: false, start, GetCurrentToken().End);
			}
			Token token = GetCurrentToken();
			throw new SemanticErrorException(token.Start, token.End, "While parsing a node, did not find expected node content.");
		}

		private ParsingEvent ParseDocumentEnd()
		{
			bool isImplicit = true;
			Mark start = GetCurrentToken().Start;
			Mark end = start;
			if (GetCurrentToken() is YamlDotNet.Core.Tokens.DocumentEnd)
			{
				end = GetCurrentToken().End;
				Skip();
				isImplicit = false;
			}
			state = ParserState.DocumentStart;
			return new YamlDotNet.Core.Events.DocumentEnd(isImplicit, start, end);
		}

		private ParsingEvent ParseBlockSequenceEntry(bool isFirst)
		{
			if (isFirst)
			{
				GetCurrentToken();
				Skip();
			}
			if (GetCurrentToken() is BlockEntry)
			{
				Mark end = GetCurrentToken().End;
				Skip();
				if (!(GetCurrentToken() is BlockEntry) && !(GetCurrentToken() is BlockEnd))
				{
					states.Push(ParserState.BlockSequenceEntry);
					return ParseNode(isBlock: true, isIndentlessSequence: false);
				}
				state = ParserState.BlockSequenceEntry;
				return ProcessEmptyScalar(end);
			}
			if (GetCurrentToken() is BlockEnd)
			{
				state = states.Pop();
				SequenceEnd result = new SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
				Skip();
				return result;
			}
			Token token = GetCurrentToken();
			throw new SemanticErrorException(token.Start, token.End, "While parsing a block collection, did not find expected '-' indicator.");
		}

		private ParsingEvent ParseIndentlessSequenceEntry()
		{
			if (GetCurrentToken() is BlockEntry)
			{
				Mark end = GetCurrentToken().End;
				Skip();
				if (!(GetCurrentToken() is BlockEntry) && !(GetCurrentToken() is Key) && !(GetCurrentToken() is Value) && !(GetCurrentToken() is BlockEnd))
				{
					states.Push(ParserState.IndentlessSequenceEntry);
					return ParseNode(isBlock: true, isIndentlessSequence: false);
				}
				state = ParserState.IndentlessSequenceEntry;
				return ProcessEmptyScalar(end);
			}
			state = states.Pop();
			return new SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
		}

		private ParsingEvent ParseBlockMappingKey(bool isFirst)
		{
			if (isFirst)
			{
				GetCurrentToken();
				Skip();
			}
			if (GetCurrentToken() is Key)
			{
				Mark end = GetCurrentToken().End;
				Skip();
				if (!(GetCurrentToken() is Key) && !(GetCurrentToken() is Value) && !(GetCurrentToken() is BlockEnd))
				{
					states.Push(ParserState.BlockMappingValue);
					return ParseNode(isBlock: true, isIndentlessSequence: true);
				}
				state = ParserState.BlockMappingValue;
				return ProcessEmptyScalar(end);
			}
			if (GetCurrentToken() is BlockEnd)
			{
				state = states.Pop();
				MappingEnd result = new MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
				Skip();
				return result;
			}
			Token token = GetCurrentToken();
			throw new SemanticErrorException(token.Start, token.End, "While parsing a block mapping, did not find expected key.");
		}

		private ParsingEvent ParseBlockMappingValue()
		{
			if (GetCurrentToken() is Value)
			{
				Mark end = GetCurrentToken().End;
				Skip();
				if (!(GetCurrentToken() is Key) && !(GetCurrentToken() is Value) && !(GetCurrentToken() is BlockEnd))
				{
					states.Push(ParserState.BlockMappingKey);
					return ParseNode(isBlock: true, isIndentlessSequence: true);
				}
				state = ParserState.BlockMappingKey;
				return ProcessEmptyScalar(end);
			}
			state = ParserState.BlockMappingKey;
			return ProcessEmptyScalar(GetCurrentToken().Start);
		}

		private ParsingEvent ParseFlowSequenceEntry(bool isFirst)
		{
			if (isFirst)
			{
				GetCurrentToken();
				Skip();
			}
			if (!(GetCurrentToken() is FlowSequenceEnd))
			{
				if (!isFirst)
				{
					if (!(GetCurrentToken() is FlowEntry))
					{
						Token token = GetCurrentToken();
						throw new SemanticErrorException(token.Start, token.End, "While parsing a flow sequence, did not find expected ',' or ']'.");
					}
					Skip();
				}
				if (GetCurrentToken() is Key)
				{
					state = ParserState.FlowSequenceEntryMappingKey;
					MappingStart result = new MappingStart(null, null, isImplicit: true, MappingStyle.Flow);
					Skip();
					return result;
				}
				if (!(GetCurrentToken() is FlowSequenceEnd))
				{
					states.Push(ParserState.FlowSequenceEntry);
					return ParseNode(isBlock: false, isIndentlessSequence: false);
				}
			}
			state = states.Pop();
			SequenceEnd result2 = new SequenceEnd(GetCurrentToken().Start, GetCurrentToken().End);
			Skip();
			return result2;
		}

		private ParsingEvent ParseFlowSequenceEntryMappingKey()
		{
			if (!(GetCurrentToken() is Value) && !(GetCurrentToken() is FlowEntry) && !(GetCurrentToken() is FlowSequenceEnd))
			{
				states.Push(ParserState.FlowSequenceEntryMappingValue);
				return ParseNode(isBlock: false, isIndentlessSequence: false);
			}
			Mark end = GetCurrentToken().End;
			Skip();
			state = ParserState.FlowSequenceEntryMappingValue;
			return ProcessEmptyScalar(end);
		}

		private ParsingEvent ParseFlowSequenceEntryMappingValue()
		{
			if (GetCurrentToken() is Value)
			{
				Skip();
				if (!(GetCurrentToken() is FlowEntry) && !(GetCurrentToken() is FlowSequenceEnd))
				{
					states.Push(ParserState.FlowSequenceEntryMappingEnd);
					return ParseNode(isBlock: false, isIndentlessSequence: false);
				}
			}
			state = ParserState.FlowSequenceEntryMappingEnd;
			return ProcessEmptyScalar(GetCurrentToken().Start);
		}

		private ParsingEvent ParseFlowSequenceEntryMappingEnd()
		{
			state = ParserState.FlowSequenceEntry;
			return new MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
		}

		private ParsingEvent ParseFlowMappingKey(bool isFirst)
		{
			if (isFirst)
			{
				GetCurrentToken();
				Skip();
			}
			if (!(GetCurrentToken() is FlowMappingEnd))
			{
				if (!isFirst)
				{
					if (!(GetCurrentToken() is FlowEntry))
					{
						Token token = GetCurrentToken();
						throw new SemanticErrorException(token.Start, token.End, "While parsing a flow mapping,  did not find expected ',' or '}'.");
					}
					Skip();
				}
				if (GetCurrentToken() is Key)
				{
					Skip();
					if (!(GetCurrentToken() is Value) && !(GetCurrentToken() is FlowEntry) && !(GetCurrentToken() is FlowMappingEnd))
					{
						states.Push(ParserState.FlowMappingValue);
						return ParseNode(isBlock: false, isIndentlessSequence: false);
					}
					state = ParserState.FlowMappingValue;
					return ProcessEmptyScalar(GetCurrentToken().Start);
				}
				if (!(GetCurrentToken() is FlowMappingEnd))
				{
					states.Push(ParserState.FlowMappingEmptyValue);
					return ParseNode(isBlock: false, isIndentlessSequence: false);
				}
			}
			state = states.Pop();
			MappingEnd result = new MappingEnd(GetCurrentToken().Start, GetCurrentToken().End);
			Skip();
			return result;
		}

		private ParsingEvent ParseFlowMappingValue(bool isEmpty)
		{
			if (isEmpty)
			{
				state = ParserState.FlowMappingKey;
				return ProcessEmptyScalar(GetCurrentToken().Start);
			}
			if (GetCurrentToken() is Value)
			{
				Skip();
				if (!(GetCurrentToken() is FlowEntry) && !(GetCurrentToken() is FlowMappingEnd))
				{
					states.Push(ParserState.FlowMappingKey);
					return ParseNode(isBlock: false, isIndentlessSequence: false);
				}
			}
			state = ParserState.FlowMappingKey;
			return ProcessEmptyScalar(GetCurrentToken().Start);
		}
	}
}
