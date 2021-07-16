using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneticAnalysisStationConfig : IBuildingConfig
{
	public const string ID = "GeneticAnalysisStation";

	public override string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef obj = BuildingTemplates.CreateBuildingDef("GeneticAnalysisStation", 7, 2, "genetic_analysisstation_kanim", 30, 30f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 1600f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER3, decor: BUILDINGS.DECOR.NONE);
		BuildingTemplates.CreateElectricalBuildingDef(obj);
		obj.AudioCategory = "Metal";
		obj.AudioSize = "large";
		obj.EnergyConsumptionWhenActive = 480f;
		obj.ExhaustKilowattsWhenActive = 0.5f;
		obj.SelfHeatKilowattsWhenActive = 4f;
		obj.Deprecated = !DlcManager.FeaturePlantMutationsEnabled();
		return obj;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.AddOrGetDef<GeneticAnalysisStation.Def>();
		go.AddOrGet<GeneticAnalysisStationWorkable>().finishedSeedDropOffset = new Vector3(-3f, 1.5f, 0f);
		Prioritizable.AddRef(go);
		go.AddOrGet<DropAllWorkable>();
		go.AddOrGetDef<PoweredActiveController.Def>();
		Storage storage = go.AddOrGet<Storage>();
		ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
		manualDeliveryKG.SetStorage(storage);
		manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
		manualDeliveryKG.requestedItemTag = GameTags.UnidentifiedSeed;
		manualDeliveryKG.refillMass = 1.1f;
		manualDeliveryKG.minimumMass = 1f;
		manualDeliveryKG.capacity = 5f;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}

	public override void ConfigurePost(BuildingDef def)
	{
		List<Tag> list = new List<Tag>();
		foreach (GameObject item in Assets.GetPrefabsWithTag(GameTags.CropSeed))
		{
			if (item.GetComponent<MutantPlant>() != null)
			{
				list.Add(item.PrefabID());
			}
		}
		def.BuildingComplete.GetComponent<Storage>().storageFilters = list;
	}
}
