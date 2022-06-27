using Klei.AI;

public class CreaturePathFinderAbilities : PathFinderAbilities
{
	public bool canTraverseSubmered;

	public CreaturePathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
	}

	protected override void Refresh(Navigator navigator)
	{
		if (PathFinder.IsSubmerged(Grid.PosToCell(navigator)))
		{
			canTraverseSubmered = true;
			return;
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.MaxUnderwaterTravelCost.Lookup(navigator);
		canTraverseSubmered = attributeInstance == null;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, bool submerged)
	{
		if (submerged && !canTraverseSubmered)
		{
			return false;
		}
		return true;
	}
}
