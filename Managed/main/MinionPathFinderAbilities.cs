using System.Diagnostics;

public class MinionPathFinderAbilities : PathFinderAbilities
{
	private CellOffset[][] transitionVoidOffsets;

	private int proxyID;

	private bool out_of_fuel;

	private bool idleNavMaskEnabled;

	public MinionPathFinderAbilities(Navigator navigator)
		: base(navigator)
	{
		transitionVoidOffsets = new CellOffset[navigator.NavGrid.transitions.Length][];
		for (int i = 0; i < transitionVoidOffsets.Length; i++)
		{
			transitionVoidOffsets[i] = navigator.NavGrid.transitions[i].voidOffsets;
		}
	}

	protected override void Refresh(Navigator navigator)
	{
		proxyID = navigator.GetComponent<MinionIdentity>().assignableProxy.Get().GetComponent<KPrefabID>().InstanceID;
		out_of_fuel = navigator.HasTag(GameTags.JetSuitOutOfFuel);
	}

	public void SetIdleNavMaskEnabled(bool enabled)
	{
		idleNavMaskEnabled = enabled;
	}

	private static bool IsAccessPermitted(int proxyID, int cell, int from_cell, NavType from_nav_type)
	{
		return Grid.HasPermission(cell, proxyID, from_cell, from_nav_type);
	}

	public override int GetSubmergedPathCostPenalty(PathFinder.PotentialPath path, NavGrid.Link link)
	{
		if (!path.HasAnyFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack))
		{
			return link.cost * 2;
		}
		return 0;
	}

	public override bool TraversePath(ref PathFinder.PotentialPath path, int from_cell, NavType from_nav_type, int cost, int transition_id, int underwater_cost)
	{
		if (!IsAccessPermitted(proxyID, path.cell, from_cell, from_nav_type))
		{
			return false;
		}
		CellOffset[] array = transitionVoidOffsets[transition_id];
		foreach (CellOffset offset in array)
		{
			int cell = Grid.OffsetCell(from_cell, offset);
			if (!IsAccessPermitted(proxyID, cell, from_cell, from_nav_type))
			{
				return false;
			}
		}
		if (path.navType == NavType.Tube && from_nav_type == NavType.Floor && !Grid.HasUsableTubeEntrance(from_cell, prefabInstanceID))
		{
			return false;
		}
		if (path.navType == NavType.Hover && (out_of_fuel || !path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack)))
		{
			return false;
		}
		Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
		PathFinder.PotentialPath.Flags pathFlags = PathFinder.PotentialPath.Flags.None;
		bool flag = path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks) && Grid.TryGetSuitMarkerFlags(from_cell, out flags, out pathFlags) && (flags & Grid.SuitMarker.Flags.Operational) != 0;
		bool flag2 = SuitMarker.DoesTraversalDirectionRequireSuit(from_cell, path.cell, flags);
		bool flag3 = path.HasAnyFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack | PathFinder.PotentialPath.Flags.HasOxygenMask);
		if (flag)
		{
			bool flag4 = path.HasFlag(pathFlags);
			if (flag2)
			{
				if (!flag3 && !Grid.HasSuit(from_cell, prefabInstanceID))
				{
					return false;
				}
			}
			else if (flag3 && (flags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) != 0 && (!flag4 || !Grid.HasEmptyLocker(from_cell, prefabInstanceID)))
			{
				return false;
			}
		}
		if (idleNavMaskEnabled && (Grid.PreventIdleTraversal[path.cell] || Grid.PreventIdleTraversal[from_cell]))
		{
			return false;
		}
		if (flag)
		{
			if (flag2)
			{
				if (!flag3)
				{
					path.SetFlags(pathFlags);
				}
			}
			else
			{
				path.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack | PathFinder.PotentialPath.Flags.HasOxygenMask);
			}
		}
		return true;
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void BeginSample(string region_name)
	{
	}

	[Conditional("ENABLE_NAVIGATION_MASK_PROFILING")]
	private void EndSample(string region_name)
	{
	}
}
