using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class FertilityModifiers : ResourceSet<FertilityModifier>
	{
		public List<FertilityModifier> GetForTag(Tag searchTag)
		{
			List<FertilityModifier> list = new List<FertilityModifier>();
			foreach (FertilityModifier resource in resources)
			{
				if (resource.TargetTag == searchTag)
				{
					list.Add(resource);
				}
			}
			return list;
		}
	}
}
