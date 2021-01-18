using UnityEngine;

public class IdleCellSensor : Sensor
{
	private MinionBrain brain;

	private Navigator navigator;

	private KPrefabID prefabid;

	private int cell;

	public IdleCellSensor(Sensors sensors)
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
		MinionPathFinderAbilities obj = (MinionPathFinderAbilities)navigator.GetCurrentAbilities();
		obj.SetIdleNavMaskEnabled(enabled: true);
		IdleCellQuery idleCellQuery = PathFinderQueries.idleCellQuery.Reset(brain, Random.Range(30, 60));
		navigator.RunQuery(idleCellQuery);
		obj.SetIdleNavMaskEnabled(enabled: false);
		cell = idleCellQuery.GetResultCell();
	}

	public int GetCell()
	{
		return cell;
	}
}
