public class NavTeleporter : KMonoBehaviour
{
	private NavTeleporter target;

	private int lastRegisteredCell = Grid.InvalidCell;

	public CellOffset offset;

	private int overrideCell = -1;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		GetComponent<KPrefabID>().AddTag(GameTags.NavTeleporters);
		Register();
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChanged, "NavTeleporterCellChanged");
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		int cell = GetCell();
		if (cell != Grid.InvalidCell)
		{
			Grid.HasNavTeleporter[cell] = false;
		}
		Deregister();
		Components.NavTeleporters.Remove(this);
	}

	public void SetOverrideCell(int cell)
	{
		overrideCell = cell;
	}

	public int GetCell()
	{
		if (overrideCell >= 0)
		{
			return overrideCell;
		}
		return Grid.OffsetCell(Grid.PosToCell(this), offset);
	}

	public void TwoWayTarget(NavTeleporter nt)
	{
		if (target != null)
		{
			if (nt != null)
			{
				nt.SetTarget(null);
			}
			BreakLink();
		}
		target = nt;
		if (target != null)
		{
			SetLink();
			if (nt != null)
			{
				nt.SetTarget(this);
			}
		}
	}

	public void EnableTwoWayTarget(bool enable)
	{
		if (enable)
		{
			target.SetLink();
			SetLink();
		}
		else
		{
			target.BreakLink();
			BreakLink();
		}
	}

	public void SetTarget(NavTeleporter nt)
	{
		if (target != null)
		{
			BreakLink();
		}
		target = nt;
		if (target != null)
		{
			SetLink();
		}
	}

	private void Register()
	{
		int cell = GetCell();
		if (!Grid.IsValidCell(cell))
		{
			lastRegisteredCell = Grid.InvalidCell;
			return;
		}
		Grid.HasNavTeleporter[cell] = true;
		Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME);
		Pathfinding.Instance.AddDirtyNavGridCell(cell);
		lastRegisteredCell = cell;
		if (target != null)
		{
			SetLink();
		}
	}

	private void SetLink()
	{
		int cell = target.GetCell();
		Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME).teleportTransitions[lastRegisteredCell] = cell;
		Pathfinding.Instance.AddDirtyNavGridCell(lastRegisteredCell);
	}

	private void Deregister()
	{
		if (lastRegisteredCell != Grid.InvalidCell)
		{
			BreakLink();
			Grid.HasNavTeleporter[lastRegisteredCell] = false;
			Pathfinding.Instance.AddDirtyNavGridCell(lastRegisteredCell);
			lastRegisteredCell = Grid.InvalidCell;
		}
	}

	private void BreakLink()
	{
		Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME).teleportTransitions.Remove(lastRegisteredCell);
		Pathfinding.Instance.AddDirtyNavGridCell(lastRegisteredCell);
	}

	private void OnCellChanged()
	{
		Deregister();
		Register();
		if (target != null)
		{
			NavTeleporter component = target.GetComponent<NavTeleporter>();
			if (component != null)
			{
				component.SetTarget(this);
			}
		}
	}
}
