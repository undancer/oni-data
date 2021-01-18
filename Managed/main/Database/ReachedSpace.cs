using STRINGS;

namespace Database
{
	public class ReachedSpace : VictoryColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private SpaceDestinationType destinationType;

		public ReachedSpace(SpaceDestinationType destinationType = null)
		{
			this.destinationType = destinationType;
		}

		public override string Name()
		{
			if (destinationType != null)
			{
				return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION, destinationType.Name);
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION;
		}

		public override string Description()
		{
			if (destinationType != null)
			{
				return string.Format(COLONY_ACHIEVEMENTS.DISTANT_PLANET_REACHED.REQUIREMENTS.REACHED_SPACE_DESTINATION_DESCRIPTION, destinationType.Name);
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.REACH_SPACE_ANY_DESTINATION_DESCRIPTION;
		}

		public override bool Success()
		{
			foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
			{
				if (item.state == Spacecraft.MissionState.Grounded || item.state == Spacecraft.MissionState.Destroyed)
				{
					continue;
				}
				SpaceDestination destination = SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.savedSpacecraftDestinations[item.id]);
				if (destinationType == null || destination.GetDestinationType() == destinationType)
				{
					if (destinationType == Db.Get().SpaceDestinationTypes.Wormhole)
					{
						Game.Instance.unlocks.Unlock("temporaltear");
					}
					return true;
				}
			}
			if (SpacecraftManager.instance.hasVisitedWormHole)
			{
				return true;
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
			if (reader.ReadByte() == 0)
			{
				string id = reader.ReadKleiString();
				destinationType = Db.Get().SpaceDestinationTypes.Get(id);
			}
		}

		public override string GetProgress(bool completed)
		{
			if (destinationType == null)
			{
				return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET;
			}
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.LAUNCHED_ROCKET_TO_WORMHOLE;
		}
	}
}
