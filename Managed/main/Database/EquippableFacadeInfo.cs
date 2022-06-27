using System.Collections.Generic;

namespace Database
{
	public class EquippableFacadeInfo
	{
		public class equippable
		{
			public string name { get; set; }

			public string buildoverride { get; set; }

			public string animfile { get; set; }
		}

		public string defID { get; set; }

		public List<equippable> equippables { get; set; }
	}
}
