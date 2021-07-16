using KSerialization;
using UnityEngine;

namespace ProcGen.Map
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Corner
	{
		public Vector2 position;

		public Corner(Vector2 position)
		{
			this.position = position;
		}
	}
}
