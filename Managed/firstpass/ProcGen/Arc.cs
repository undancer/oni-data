using KSerialization;
using Satsuma;

namespace ProcGen
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Arc
	{
		private bool arcSet;

		[Serialize]
		public string type = "";

		[Serialize]
		public TagSet tags;

		public Satsuma.Arc arc { get; private set; }

		public void SetArc(Satsuma.Arc arc)
		{
			Debug.Assert(!arcSet, "Tried setting up an Arc twice, no go.");
			this.arc = arc;
			arcSet = true;
		}

		public void SetType(string type)
		{
			this.type = type;
		}

		public Arc()
		{
		}

		public Arc(string type)
		{
			this.type = type;
		}

		public Arc(Satsuma.Arc arc, string type)
		{
			this.arc = arc;
			this.type = type;
		}
	}
}
