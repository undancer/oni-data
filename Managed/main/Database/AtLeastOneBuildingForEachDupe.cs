using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class AtLeastOneBuildingForEachDupe : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private List<Tag> validBuildingTypes = new List<Tag>();

		public AtLeastOneBuildingForEachDupe(List<Tag> validBuildingTypes)
		{
			this.validBuildingTypes = validBuildingTypes;
		}

		public override bool Success()
		{
			if (Components.LiveMinionIdentities.Items.Count <= 0)
			{
				return false;
			}
			int num = 0;
			foreach (IBasicBuilding item in Components.BasicBuildings.Items)
			{
				KPrefabID component = item.transform.GetComponent<KPrefabID>();
				Tag prefabTag = component.PrefabTag;
				if (validBuildingTypes.Contains(prefabTag))
				{
					num++;
					if (prefabTag == "FlushToilet" || prefabTag == "Outhouse")
					{
						return true;
					}
				}
			}
			return num >= Components.LiveMinionIdentities.Items.Count;
		}

		public override bool Fail()
		{
			return false;
		}

		public void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			validBuildingTypes = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				validBuildingTypes.Add(new Tag(name));
			}
		}

		public override string GetProgress(bool complete)
		{
			if (validBuildingTypes.Contains("FlushToilet"))
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_TOILET;
			}
			if (complete)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILT_ONE_BED_PER_DUPLICANT;
			}
			int num = 0;
			foreach (IBasicBuilding item in Components.BasicBuildings.Items)
			{
				KPrefabID component = item.transform.GetComponent<KPrefabID>();
				if (validBuildingTypes.Contains(component.PrefabTag))
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.BUILING_BEDS, complete ? Components.LiveMinionIdentities.Items.Count : num, Components.LiveMinionIdentities.Items.Count);
		}
	}
}
