using STRINGS;
using UnityEngine;

namespace Database
{
	public class AutomateABuilding : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			foreach (LogicCircuitNetwork network in Game.Instance.logicCircuitSystem.GetNetworks())
			{
				if (network.Receivers.Count <= 0 || network.Senders.Count <= 0)
				{
					continue;
				}
				bool flag = false;
				foreach (ILogicEventReceiver receiver in network.Receivers)
				{
					GameObject gameObject = Grid.Objects[receiver.GetLogicCell(), 1];
					if (gameObject != null)
					{
						KPrefabID component = gameObject.GetComponent<KPrefabID>();
						if (!component.HasTag(GameTags.TemplateBuilding))
						{
							flag = true;
							break;
						}
					}
				}
				bool flag2 = false;
				foreach (ILogicEventSender sender in network.Senders)
				{
					GameObject gameObject2 = Grid.Objects[sender.GetLogicCell(), 1];
					if (gameObject2 != null)
					{
						KPrefabID component2 = gameObject2.GetComponent<KPrefabID>();
						if (!component2.HasTag(GameTags.TemplateBuilding))
						{
							flag2 = true;
							break;
						}
					}
				}
				if (flag && flag2)
				{
					return true;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.AUTOMATE_A_BUILDING;
		}
	}
}
