using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public sealed class MergingParser : IParser
	{
		private class ParsingEventCloner : IParsingEventVisitor
		{
			private ParsingEvent clonedEvent;

			public ParsingEvent Clone(ParsingEvent e)
			{
				e.Accept(this);
				return clonedEvent;
			}

			void IParsingEventVisitor.Visit(AnchorAlias e)
			{
				clonedEvent = new AnchorAlias(e.Value, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(StreamStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(StreamEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentStart e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(DocumentEnd e)
			{
				throw new NotSupportedException();
			}

			void IParsingEventVisitor.Visit(Scalar e)
			{
				clonedEvent = new Scalar(null, e.Tag, e.Value, e.Style, e.IsPlainImplicit, e.IsQuotedImplicit, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceStart e)
			{
				clonedEvent = new SequenceStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(SequenceEnd e)
			{
				clonedEvent = new SequenceEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingStart e)
			{
				clonedEvent = new MappingStart(null, e.Tag, e.IsImplicit, e.Style, e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(MappingEnd e)
			{
				clonedEvent = new MappingEnd(e.Start, e.End);
			}

			void IParsingEventVisitor.Visit(Comment e)
			{
				throw new NotSupportedException();
			}
		}

		private readonly List<ParsingEvent> _allEvents = new List<ParsingEvent>();

		private readonly IParser _innerParser;

		private int _currentIndex = -1;

		public ParsingEvent Current { get; private set; }

		public MergingParser(IParser innerParser)
		{
			_innerParser = innerParser;
		}

		public bool MoveNext()
		{
			if (_currentIndex < 0)
			{
				while (_innerParser.MoveNext())
				{
					_allEvents.Add(_innerParser.Current);
				}
				for (int num = _allEvents.Count - 2; num >= 0; num--)
				{
					Scalar scalar = _allEvents[num] as Scalar;
					if (scalar == null || !(scalar.Value == "<<"))
					{
						continue;
					}
					AnchorAlias anchorAlias = _allEvents[num + 1] as AnchorAlias;
					if (anchorAlias != null)
					{
						IEnumerable<ParsingEvent> mappingEvents = GetMappingEvents(anchorAlias.Value);
						_allEvents.RemoveRange(num, 2);
						_allEvents.InsertRange(num, mappingEvents);
						continue;
					}
					if (_allEvents[num + 1] is SequenceStart)
					{
						List<IEnumerable<ParsingEvent>> list = new List<IEnumerable<ParsingEvent>>();
						bool flag = false;
						for (int i = num + 2; i < _allEvents.Count; i++)
						{
							anchorAlias = _allEvents[i] as AnchorAlias;
							if (anchorAlias != null)
							{
								list.Add(GetMappingEvents(anchorAlias.Value));
							}
							else if (_allEvents[i] is SequenceEnd)
							{
								_allEvents.RemoveRange(num, i - num + 1);
								_allEvents.InsertRange(num, list.SelectMany((IEnumerable<ParsingEvent> e) => e));
								flag = true;
								break;
							}
						}
						if (flag)
						{
							continue;
						}
					}
					throw new SemanticErrorException(scalar.Start, scalar.End, "Unrecognized merge key pattern");
				}
			}
			int num2 = _currentIndex + 1;
			if (num2 < _allEvents.Count)
			{
				Current = _allEvents[num2];
				_currentIndex = num2;
				return true;
			}
			return false;
		}

		private IEnumerable<ParsingEvent> GetMappingEvents(string mappingAlias)
		{
			ParsingEventCloner cloner = new ParsingEventCloner();
			int nesting = 0;
			return (from e in _allEvents.SkipWhile(delegate(ParsingEvent e)
				{
					MappingStart mappingStart = e as MappingStart;
					return mappingStart == null || mappingStart.Anchor != mappingAlias;
				}).Skip(1).TakeWhile((ParsingEvent e) => (nesting += e.NestingIncrease) >= 0)
				select cloner.Clone(e)).ToList();
		}
	}
}
