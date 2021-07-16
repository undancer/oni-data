using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class FoodSplatConfig : IEntityConfig
{
	public const string ID = "FoodSplat";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		return EntityTemplates.CreateBasicEntity("FoodSplat", ITEMS.FOOD.FOODSPLAT.NAME, ITEMS.FOOD.FOODSPLAT.DESC, 1f, unitMass: true, Assets.GetAnim("sticker_kanim"), "idle_sticker_a", Grid.SceneLayer.Backwall);
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<OccupyArea>().OccupiedCellsOffsets = new CellOffset[1];
		inst.AddComponent<Modifiers>();
		inst.AddOrGet<KSelectable>();
		inst.AddOrGet<DecorProvider>().SetValues(DECOR.PENALTY.TIER2);
		inst.AddOrGetDef<Splat.Def>();
		inst.AddOrGet<SplatWorkable>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
