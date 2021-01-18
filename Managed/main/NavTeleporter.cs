public class NavTeleporter : KMonoBehaviour
{
	private NavTeleporter target;

	private int lastRegisteredCell = Grid.InvalidCell;

	public CellOffset offset;

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

	public int GetCell()
	{
		int cell = Grid.PosToCell(this);
		return Grid.OffsetCell(cell, offset);
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
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME);
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
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME);
		navGrid.teleportTransitions[lastRegisteredCell] = cell;
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
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid(MinionConfig.MINION_NAV_GRID_NAME);
		navGrid.teleportTransitions.Remove(lastRegisteredCell);
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
