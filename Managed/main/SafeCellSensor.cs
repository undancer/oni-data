public class SafeCellSensor : Sensor
{
	private MinionBrain brain;

	private Navigator navigator;

	private KPrefabID prefabid;

	private int cell = Grid.InvalidCell;

	public SafeCellSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = GetComponent<Navigator>();
		brain = GetComponent<MinionBrain>();
		prefabid = GetComponent<KPrefabID>();
	}

	public override void Update()
	{
		if (!prefabid.HasTag(GameTags.Idle))
		{
			cell = Grid.InvalidCell;
			return;
		}
		bool flag = HasSafeCell();
		RunSafeCellQuery(avoid_light: false);
		bool flag2 = HasSafeCell();
		if (flag2 != flag)
		{
			if (flag2)
			{
				sensors.Trigger(982561777);
			}
			else
			{
				sensors.Trigger(506919987);
			}
		}
	}

	public void RunSafeCellQuery(bool avoid_light)
	{
		MinionPathFinderAbilities obj = (MinionPathFinderAbilities)navigator.GetCurrentAbilities();
		obj.SetIdleNavMaskEnabled(enabled: true);
		SafeCellQuery safeCellQuery = PathFinderQueries.safeCellQuery.Reset(brain, avoid_light);
		navigator.RunQuery(safeCellQuery);
		obj.SetIdleNavMaskEnabled(enabled: false);
		cell = safeCellQuery.GetResultCell();
		if (cell == Grid.PosToCell(navigator))
		{
			cell = Grid.InvalidCell;
		}
	}

	public int GetSensorCell()
	{
		return cell;
	}

	public int GetCellQuery()
	{
		if (cell == Grid.InvalidCell)
		{
			RunSafeCellQuery(avoid_light: false);
		}
		return cell;
	}

	public int GetSleepCellQuery()
	{
		if (cell == Grid.InvalidCell)
		{
			RunSafeCellQuery(avoid_light: true);
		}
		return cell;
	}

	public bool HasSafeCell()
	{
		if (cell != Grid.InvalidCell)
		{
			return cell != Grid.PosToCell(sensors);
		}
		return false;
	}
}
