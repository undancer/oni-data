using System.Linq;
using Database;
using Klei.AI;

public class GameplayEventPreconditions
{
	private static GameplayEventPreconditions _instance;

	public static GameplayEventPreconditions Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameplayEventPreconditions();
			}
			return _instance;
		}
	}

	public GameplayEventPrecondition LiveMinions(int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Components.LiveMinionIdentities.Count >= count,
			description = $"At least {count} dupes alive"
		};
	}

	public GameplayEventPrecondition BuildingExists(string buildingId, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => BuildingInventory.Instance.BuildingCount(new Tag(buildingId)) >= count,
			description = $"{count} {buildingId} has been built"
		};
	}

	public GameplayEventPrecondition ResearchCompleted(string techName)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Research.Instance.Get(Db.Get().Techs.Get(techName)).IsComplete(),
			description = "Has researched " + techName + "."
		};
	}

	public GameplayEventPrecondition AchievementUnlocked(ColonyAchievement achievement)
	{
		return new GameplayEventPrecondition
		{
			condition = () => SaveGame.Instance.GetComponent<ColonyAchievementTracker>().IsAchievementUnlocked(achievement),
			description = "Unlocked the " + achievement.Id + " achievement"
		};
	}

	public GameplayEventPrecondition RoomBuilt(RoomType roomType)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Game.Instance.roomProber.rooms.Exists((Room match) => match.roomType == roomType),
			description = "Built a " + roomType.Id + " room"
		};
	}

	public GameplayEventPrecondition CycleRestriction(float min = 0f, float max = float.PositiveInfinity)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameUtil.GetCurrentTimeInCycles() >= min && GameUtil.GetCurrentTimeInCycles() <= max,
			description = $"After cycle {min} and before cycle {max}"
		};
	}

	public GameplayEventPrecondition MinionsWithEffect(string effectId, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Components.LiveMinionIdentities.Items.Count((MinionIdentity minion) => minion.GetComponent<Effects>().Get(effectId) != null) >= count,
			description = $"At least {count} dupes have the {effectId} effect applied"
		};
	}

	public GameplayEventPrecondition MinionsWithStatusItem(StatusItem statusItem, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Components.LiveMinionIdentities.Items.Count((MinionIdentity minion) => minion.GetComponent<KSelectable>().HasStatusItem(statusItem)) >= count,
			description = $"At least {count} dupes have the {statusItem} status item"
		};
	}

	public GameplayEventPrecondition MinionsWithChoreGroupPriorityOrGreater(ChoreGroup choreGroup, int count, int priority)
	{
		return new GameplayEventPrecondition
		{
			condition = () => Components.LiveMinionIdentities.Items.Count(delegate(MinionIdentity minion)
			{
				ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
				return !component.IsChoreGroupDisabled(choreGroup) && component.GetPersonalPriority(choreGroup) >= priority;
			}) >= count,
			description = $"At least {count} dupes have their {choreGroup.Name} set to {priority} or higher."
		};
	}

	public GameplayEventPrecondition PastEventCount(string evtId, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameplayEventManager.Instance.NumberOfPastEvents(evtId) >= count,
			description = $"The {evtId} event has triggered {count} times."
		};
	}

	public GameplayEventPrecondition PastEventCountAndNotActive(GameplayEvent evt, int count = 1)
	{
		return new GameplayEventPrecondition
		{
			condition = () => GameplayEventManager.Instance.NumberOfPastEvents(evt.IdHash) >= count && !GameplayEventManager.Instance.IsGameplayEventActive(evt),
			description = $"The {evt.Id} event has triggered {count} times and is not active."
		};
	}

	public GameplayEventPrecondition Not(GameplayEventPrecondition precondition)
	{
		return new GameplayEventPrecondition
		{
			condition = () => !precondition.condition(),
			description = "Not[" + precondition.description + "]"
		};
	}

	public GameplayEventPrecondition Or(GameplayEventPrecondition precondition1, GameplayEventPrecondition precondition2)
	{
		GameplayEventPrecondition gameplayEventPrecondition = new GameplayEventPrecondition();
		gameplayEventPrecondition.condition = () => precondition1.condition() || precondition2.condition();
		gameplayEventPrecondition.description = "[" + precondition1.description + "]-OR-[" + precondition2.description + "]";
		return gameplayEventPrecondition;
	}
}
