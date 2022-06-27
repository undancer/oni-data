using STRINGS;

namespace Database
{
	public class VentXKG : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private SimHashes element;

		private float kilogramsToVent;

		public VentXKG(SimHashes element, float kilogramsToVent)
		{
			this.element = element;
			this.kilogramsToVent = kilogramsToVent;
		}

		public override bool Success()
		{
			float num = 0f;
			foreach (UtilityNetwork network in Conduit.GetNetworkManager(ConduitType.Gas).GetNetworks())
			{
				if (!(network is FlowUtilityNetwork flowUtilityNetwork))
				{
					continue;
				}
				foreach (FlowUtilityNetwork.IItem sink in flowUtilityNetwork.sinks)
				{
					Vent component = sink.GameObject.GetComponent<Vent>();
					if (component != null)
					{
						num += component.GetVentedMass(element);
					}
				}
			}
			return num >= kilogramsToVent;
		}

		public void Deserialize(IReader reader)
		{
			element = (SimHashes)reader.ReadInt32();
			kilogramsToVent = reader.ReadSingle();
		}

		public override string GetProgress(bool complete)
		{
			float num = 0f;
			foreach (UtilityNetwork network in Conduit.GetNetworkManager(ConduitType.Gas).GetNetworks())
			{
				if (!(network is FlowUtilityNetwork flowUtilityNetwork))
				{
					continue;
				}
				foreach (FlowUtilityNetwork.IItem sink in flowUtilityNetwork.sinks)
				{
					Vent component = sink.GameObject.GetComponent<Vent>();
					if (component != null)
					{
						num += component.GetVentedMass(element);
					}
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.VENTED_MASS, GameUtil.GetFormattedMass(complete ? kilogramsToVent : num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram), GameUtil.GetFormattedMass(kilogramsToVent, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram));
		}
	}
}
