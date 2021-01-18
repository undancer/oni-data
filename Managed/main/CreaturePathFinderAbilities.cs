using Klei.AI;

public class CreaturePathFinderAbilities : PathFinderAbilities
{
	public int maxUnderwaterCost;

	public CreaturePathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
	}

	protected override void Refresh(Navigator navigator)
	{
		int cell = Grid.PosToCell(navigator);
		if (PathFinder.IsSubmerged(cell))
		{
			maxUnderwaterCost = int.MaxValue;
			return;
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(navigator);
		maxUnderwaterCost = ((attributeInstance != null) ? ((int)attributeInstance.GetTotalValue()) : int.MaxValue);
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (underwater_cost > maxUnderwaterCost)
		{
			return false;
		}
		return true;
	}
}
