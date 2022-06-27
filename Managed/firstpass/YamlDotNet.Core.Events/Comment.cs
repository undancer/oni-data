namespace YamlDotNet.Core.Events
{
	public class Comment : ParsingEvent
	{
		public string Value { get; private set; }

		public bool IsInline { get; private set; }

		internal override EventType Type => EventType.Comment;

		public Comment(string value, bool isInline)
			: this(value, isInline, Mark.Empty, Mark.Empty)
		{
		}

		public Comment(string value, bool isInline, Mark start, Mark end)
			: base(start, end)
		{
			Value = value;
			IsInline = isInline;
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
