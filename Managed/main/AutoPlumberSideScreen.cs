using UnityEngine;

public class AutoPlumberSideScreen : SideScreenContent
{
	public KButton activateButton;

	public KButton powerButton;

	public KButton pipesButton;

	public KButton solidsButton;

	public KButton minionButton;

	private Building building;

	protected override void OnSpawn()
	{
		activateButton.onClick += delegate
		{
			DevAutoPlumber.AutoPlumbBuilding(building);
		};
		powerButton.onClick += delegate
		{
			DevAutoPlumber.DoElectricalPlumbing(building);
		};
		pipesButton.onClick += delegate
		{
			DevAutoPlumber.DoLiquidAndGasPlumbing(building);
		};
		solidsButton.onClick += delegate
		{
			DevAutoPlumber.SetupSolidOreDelivery(building);
		};
		minionButton.onClick += delegate
		{
			SpawnMinion();
		};
	}

	private void SpawnMinion()
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID));
		gameObject.name = Assets.GetPrefab(MinionConfig.ID).name;
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		Vector3 position = Grid.CellToPos(Grid.PosToCell(building), CellAlignment.Center, Grid.SceneLayer.Move);
		gameObject.transform.SetLocalPosition(position);
		gameObject.SetActive(value: true);
		new MinionStartingStats(is_starter_minion: false).Apply(gameObject);
	}

	public override int GetSideScreenSortOrder()
	{
		return -150;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		if (DebugHandler.InstantBuildMode)
		{
			return target.GetComponent<Building>() != null;
		}
		return false;
	}

	public override void SetTarget(GameObject target)
	{
		building = target.GetComponent<Building>();
		Refresh();
	}

	public override void ClearTarget()
	{
	}

	private void Refresh()
	{
	}
}
