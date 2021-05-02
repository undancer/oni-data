using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SteamTurbineConfig : IBuildingConfig
{
	public const string ID = "SteamTurbine";

	private const int HEIGHT = 4;

	private const int WIDTH = 5;

	private static readonly List<Storage.StoredItemModifier> StoredItemModifiers = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Insulate,
		Storage.StoredItemModifier.Seal
	};

	public override BuildingDef CreateBuildingDef()
	{
		string[] array = new string[2]
		{
			"RefinedMetal",
			"Plastic"
		};
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(construction_mass: new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER3[0]
		}, construction_materials: array, melting_point: 1600f, build_location_rule: BuildLocationRule.Anywhere, noise: NOISE_POLLUTION.NONE, id: "SteamTurbine", width: 5, height: 4, anim: "steamturbine_kanim", hitpoints: 30, construction_time: 60f, decor: BUILDINGS.DECOR.NONE, temperature_modification_mass_scale: 1f);
		buildingDef.GeneratorWattageRating = 2000f;
		buildingDef.GeneratorBaseCapacity = 2000f;
		buildingDef.Entombable = true;
		buildingDef.IsFoundation = false;
		buildingDef.PermittedRotations = PermittedRotations.FlipH;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerOutput = true;
		buildingDef.PowerOutputOffset = new CellOffset(1, 0);
		buildingDef.OverheatTemperature = 1273.15f;
		buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
		buildingDef.Deprecated = true;
		return buildingDef;
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		base.DoPostConfigureUnderConstruction(go);
		Constructable component = go.GetComponent<Constructable>();
		component.requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Storage storage = go.AddOrGet<Storage>();
		storage.SetDefaultStoredItemModifiers(StoredItemModifiers);
		Turbine turbine = go.AddOrGet<Turbine>();
		turbine.srcElem = SimHashes.Steam;
		MakeBaseSolid.Def def = go.AddOrGetDef<MakeBaseSolid.Def>();
		def.solidOffsets = new CellOffset[5];
		for (int i = 0; i < 5; i++)
		{
			def.solidOffsets[i] = new CellOffset(i - 2, 0);
		}
		turbine.pumpKGRate = 10f;
		turbine.requiredMassFlowDifferential = 3f;
		turbine.minEmitMass = 10f;
		turbine.maxRPM = 4000f;
		turbine.rpmAcceleration = turbine.maxRPM / 30f;
		turbine.rpmDeceleration = turbine.maxRPM / 20f;
		turbine.minGenerationRPM = 3000f;
		turbine.minActiveTemperature = 500f;
		turbine.emitTemperature = 425f;
		go.AddOrGet<Generator>();
		go.AddOrGet<LogicOperationalController>();
		Prioritizable.AddRef(go);
		go.GetComponent<KPrefabID>().prefabSpawnFn += delegate(GameObject game_object)
		{
			HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
			StructureTemperaturePayload new_data = GameComps.StructureTemperatures.GetPayload(handle);
			Extents extents = game_object.GetComponent<Building>().GetExtents();
			Extents newExtents = new Extents(extents.x, extents.y - 1, extents.width, extents.height + 1);
			new_data.OverrideExtents(newExtents);
			GameComps.StructureTemperatures.SetPayload(handle, ref new_data);
		};
	}
}
