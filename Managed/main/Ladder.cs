using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Ladder")]
public class Ladder : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public float upwardsMovementSpeedMultiplier = 1f;

	public float downwardsMovementSpeedMultiplier = 1f;

	public bool isPole;

	public CellOffset[] offsets = new CellOffset[1] { CellOffset.none };

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Rotatable component = GetComponent<Rotatable>();
		CellOffset[] array = offsets;
		foreach (CellOffset cellOffset in array)
		{
			CellOffset offset = cellOffset;
			if (component != null)
			{
				offset = component.GetRotatedCellOffset(cellOffset);
			}
			int i2 = Grid.OffsetCell(Grid.PosToCell(this), offset);
			Grid.HasPole[i2] = isPole;
			Grid.HasLadder[i2] = !isPole;
		}
		GetComponent<KPrefabID>().AddTag(GameTags.Ladders);
		Components.Ladders.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Rotatable component = GetComponent<Rotatable>();
		CellOffset[] array = offsets;
		foreach (CellOffset cellOffset in array)
		{
			CellOffset offset = cellOffset;
			if (component != null)
			{
				offset = component.GetRotatedCellOffset(cellOffset);
			}
			int num = Grid.OffsetCell(Grid.PosToCell(this), offset);
			if (Grid.Objects[num, 24] == null)
			{
				Grid.HasPole[num] = false;
				Grid.HasLadder[num] = false;
			}
		}
		Components.Ladders.Remove(this);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = null;
		if (upwardsMovementSpeedMultiplier != 1f)
		{
			list = new List<Descriptor>();
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(upwardsMovementSpeedMultiplier * 100f - 100f)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(upwardsMovementSpeedMultiplier * 100f - 100f)));
			list.Add(item);
		}
		return list;
	}
}
