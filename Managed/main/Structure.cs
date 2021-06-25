using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Structure")]
public class Structure : KMonoBehaviour
{
	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private Operational operational;

	public static readonly Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);

	private bool isEntombed;

	private HandleVector<int>.Handle partitionerEntry;

	private static EventSystem.IntraObjectHandler<Structure> RocketLandedDelegate = new EventSystem.IntraObjectHandler<Structure>(delegate(Structure cmp, object data)
	{
		cmp.RocketChanged(data);
	});

	public bool IsEntombed()
	{
		return isEntombed;
	}

	public static bool IsBuildingEntombed(Building building)
	{
		if (!Grid.IsValidCell(Grid.PosToCell(building)))
		{
			return false;
		}
		for (int i = 0; i < building.PlacementCells.Length; i++)
		{
			int num = building.PlacementCells[i];
			if (Grid.Element[num].IsSolid && !Grid.Foundation[num])
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Extents extents = building.GetExtents();
		partitionerEntry = GameScenePartitioner.Instance.Add("Structure.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, OnSolidChanged);
		OnSolidChanged(null);
		Subscribe(-887025858, RocketLandedDelegate);
	}

	public void UpdatePosition(int cell)
	{
		GameScenePartitioner.Instance.UpdatePosition(partitionerEntry, cell);
	}

	private void RocketChanged(object data)
	{
		OnSolidChanged(data);
	}

	private void OnSolidChanged(object data)
	{
		bool flag = IsBuildingEntombed(building);
		if (flag != isEntombed)
		{
			isEntombed = flag;
			if (isEntombed)
			{
				GetComponent<KPrefabID>().AddTag(GameTags.Entombed);
			}
			else
			{
				GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
			}
			operational.SetFlag(notEntombedFlag, !isEntombed);
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, isEntombed, this);
			Trigger(-1089732772);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
	}
}
