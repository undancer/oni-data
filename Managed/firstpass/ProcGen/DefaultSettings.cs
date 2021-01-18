using System;
using System.Collections.Generic;

namespace ProcGen
{
	[Serializable]
	public class DefaultSettings
	{
		public BaseLocation baseData
		{
			get;
			private set;
		}

		public Dictionary<string, object> data
		{
			get;
			private set;
		}

		public List<string> defaultMoveTags
		{
			get;
			private set;
		}

		public List<string> overworldAddTags
		{
			get;
			private set;
		}
	}
}
