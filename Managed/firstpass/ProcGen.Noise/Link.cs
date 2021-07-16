namespace ProcGen.Noise
{
	public class Link
	{
		public enum Type
		{
			None,
			Primitive,
			Filter,
			Transformer,
			Selector,
			Modifier,
			Combiner,
			FloatPoints,
			ControlPoints,
			Terminator
		}

		public Type type
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public Link()
		{
		}

		public Link(Type type, string name)
		{
			this.type = type;
			this.name = name;
		}
	}
}
