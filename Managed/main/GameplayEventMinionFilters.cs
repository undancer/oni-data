using Database;

public class GameplayEventMinionFilters
{
	private static GameplayEventMinionFilters _instance;

	public static GameplayEventMinionFilters Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameplayEventMinionFilters();
			}
			return _instance;
		}
	}

	public GameplayEventMinionFilter HasMasteredSkill(Skill skill)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.GetComponent<MinionResume>().HasMasteredSkill(skill.Id),
			id = "HasMasteredSkill"
		};
	}

	public GameplayEventMinionFilter HasSkillAptitude(Skill skill)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.GetComponent<MinionResume>().HasSkillAptitude(skill),
			id = "HasSkillAptitude"
		};
	}

	public GameplayEventMinionFilter HasChoreGroupPriorityOrHigher(ChoreGroup choreGroup, int priority)
	{
		return new GameplayEventMinionFilter
		{
			filter = delegate(MinionIdentity minion)
			{
				ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
				return !component.IsChoreGroupDisabled(choreGroup) && component.GetPersonalPriority(choreGroup) >= priority;
			},
			id = "HasChoreGroupPriorityOrHigher"
		};
	}

	public GameplayEventMinionFilter AgeRange(float min = 0f, float max = float.PositiveInfinity)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => minion.arrivalTime >= min && minion.arrivalTime <= max,
			id = "AgeRange"
		};
	}

	public GameplayEventMinionFilter PriorityIn()
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => true,
			id = "PriorityIn"
		};
	}

	public GameplayEventMinionFilter Not(GameplayEventMinionFilter filter)
	{
		return new GameplayEventMinionFilter
		{
			filter = (MinionIdentity minion) => !filter.filter(minion),
			id = "Not[" + filter.id + "]"
		};
	}

	public GameplayEventMinionFilter Or(GameplayEventMinionFilter precondition1, GameplayEventMinionFilter precondition2)
	{
		GameplayEventMinionFilter gameplayEventMinionFilter = new GameplayEventMinionFilter();
		gameplayEventMinionFilter.filter = (MinionIdentity minion) => precondition1.filter(minion) || precondition2.filter(minion);
		gameplayEventMinionFilter.id = "[" + precondition1.id + "]-OR-[" + precondition2.id + "]";
		return gameplayEventMinionFilter;
	}
}
