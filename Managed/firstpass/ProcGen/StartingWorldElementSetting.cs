using System;

namespace ProcGen
{
	[Serializable]
	public class StartingWorldElementSetting
	{
		public string element { get; private set; }

		public float amount { get; private set; }
	}
}
