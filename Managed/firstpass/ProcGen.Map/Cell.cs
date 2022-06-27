using KSerialization;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Cell : Node
	{
		public long NodeId => base.node.Id;
	}
}
