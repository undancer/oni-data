using System;

namespace YamlDotNet.Core.Tokens
{
	[Serializable]
	public class FlowSequenceStart : Token
	{
		public FlowSequenceStart()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public FlowSequenceStart(Mark start, Mark end)
			: base(start, end)
		{
		}
	}
}
