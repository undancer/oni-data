using System.IO;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class MinimumMorale : VictoryColonyAchievementRequirement
	{
		public int minimumMorale;

		public override string Name()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE, minimumMorale);
		}

		public override string Description()
		{
			return string.Format(COLONY_ACHIEVEMENTS.THRIVING.REQUIREMENTS.MINIMUM_MORALE_DESCRIPTION, minimumMorale);
		}

		public MinimumMorale(int minimumMorale = 16)
		{
			this.minimumMorale = minimumMorale;
		}

		public override bool Success()
		{
			bool flag = true;
			foreach (MinionAssignablesProxy item in Components.MinionAssignablesProxy)
			{
				GameObject targetGameObject = item.GetTargetGameObject();
				if (targetGameObject != null && !targetGameObject.HasTag(GameTags.Dead))
				{
					AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
					flag = attributeInstance != null && attributeInstance.GetTotalValue() >= (float)minimumMorale && flag;
				}
			}
			return flag;
		}

		public override void Serialize(BinaryWriter writer)
		{
			writer.Write(minimumMorale);
		}

		public override void Deserialize(IReader reader)
		{
			minimumMorale = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			return Description();
		}
	}
}
