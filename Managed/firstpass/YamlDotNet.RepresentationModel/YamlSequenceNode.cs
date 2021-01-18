using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	[DebuggerDisplay("Count = {children.Count}")]
	public sealed class YamlSequenceNode : YamlNode, IEnumerable<YamlNode>, IEnumerable, IYamlConvertible
	{
		private readonly IList<YamlNode> children = new List<YamlNode>();

		public IList<YamlNode> Children => children;

		public SequenceStyle Style
		{
			get;
			set;
		}

		public override YamlNodeType NodeType => YamlNodeType.Sequence;

		internal YamlSequenceNode(IParser parser, DocumentLoadingState state)
		{
			Load(parser, state);
		}

		private void Load(IParser parser, DocumentLoadingState state)
		{
			SequenceStart sequenceStart = parser.Expect<SequenceStart>();
			Load(sequenceStart, state);
			Style = sequenceStart.Style;
			bool flag = false;
			while (!parser.Accept<SequenceEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(parser, state);
				children.Add(yamlNode);
				flag = flag || yamlNode is YamlAliasNode;
			}
			if (flag)
			{
				state.AddNodeWithUnresolvedAliases(this);
			}
			parser.Expect<SequenceEnd>();
		}

		public YamlSequenceNode()
		{
		}

		public YamlSequenceNode(params YamlNode[] children)
			: this((IEnumerable<YamlNode>)children)
		{
		}

		public YamlSequenceNode(IEnumerable<YamlNode> children)
		{
			foreach (YamlNode child in children)
			{
				this.children.Add(child);
			}
		}

		public void Add(YamlNode child)
		{
			children.Add(child);
		}

		public void Add(string child)
		{
			children.Add(new YamlScalarNode(child));
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			for (int i = 0; i < children.Count; i++)
			{
				if (children[i] is YamlAliasNode)
				{
					children[i] = state.GetNode(children[i].Anchor, throwException: true, children[i].Start, children[i].End);
				}
			}
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new SequenceStart(base.Anchor, base.Tag, isImplicit: true, Style));
			foreach (YamlNode child in children)
			{
				child.Save(emitter, state);
			}
			emitter.Emit(new SequenceEnd());
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlSequenceNode yamlSequenceNode = obj as YamlSequenceNode;
			if (yamlSequenceNode == null || !Equals(yamlSequenceNode) || children.Count != yamlSequenceNode.children.Count)
			{
				return false;
			}
			for (int i = 0; i < children.Count; i++)
			{
				if (!YamlNode.SafeEquals(children[i], yamlSequenceNode.children[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (YamlNode child in children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(child));
			}
			return num;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			level.Increment();
			yield return this;
			foreach (YamlNode child in children)
			{
				foreach (YamlNode item in child.SafeAllNodes(level))
				{
					yield return item;
				}
			}
			level.Decrement();
		}

		internal override string ToString(RecursionLevel level)
		{
			if (!level.TryIncrement())
			{
				return "WARNING! INFINITE RECURSION!";
			}
			StringBuilder stringBuilder = new StringBuilder("[ ");
			foreach (YamlNode child in children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(child.ToString(level));
			}
			stringBuilder.Append(" ]");
			level.Decrement();
			return stringBuilder.ToString();
		}

		public IEnumerator<YamlNode> GetEnumerator()
		{
			return Children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			Load(parser, new DocumentLoadingState());
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			Emit(emitter, new EmitterState());
		}
	}
}
