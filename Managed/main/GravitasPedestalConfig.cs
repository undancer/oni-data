using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GravitasPedestalConfig : IBuildingConfig
{
	public const string ID = "GravitasPedestal";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GravitasPedestal", 1, 2, "gravitas_pedestal_nice_kanim", 10, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.RAW_MINERALS, 800f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.BONUS.TIER0);
		obj.DefaultAnimState = "pedestal_nice";
		obj.Floodable = false;
		obj.Overheatable = false;
		obj.ViewMode = OverlayModes.Decor.ID;
		obj.AudioCategory = "Glass";
		obj.AudioSize = "small";
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>(new Storage.StoredItemModifier[2]
		{
			Storage.StoredItemModifier.Seal,
			Storage.StoredItemModifier.Preserve
		}));
		Prioritizable.AddRef(go);
		SingleEntityReceptacle singleEntityReceptacle = go.AddOrGet<SingleEntityReceptacle>();
		singleEntityReceptacle.AddDepositTag(GameTags.PedestalDisplayable);
		singleEntityReceptacle.occupyingObjectRelativePosition = new Vector3(0f, 1.2f, -1f);
		go.AddOrGet<DecorProvider>();
		go.AddOrGet<ItemPedestal>();
		go.AddOrGet<PedestalArtifactSpawner>();
		go.GetComponent<KPrefabID>().AddTag(GameTags.Decoration);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
