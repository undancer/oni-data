using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSurfaceSatellite1Config : IEntityConfig
{
	public const string ID = "PropSurfaceSatellite1";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropSurfaceSatellite1", STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE1.NAME, STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE1.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("satellite1_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 3, height: 3, element: SimHashes.Creature, additionalTags: new List<Tag> { GameTags.Gravitas });
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		setLocker.numDataBanks = new int[2] { 4, 9 };
		gameObject.AddOrGet<Demolishable>();
		gameObject.AddOrGet<LoreBearer>();
		return gameObject;
	}

	public static string[][] GetLockerBaseContents()
	{
		string text = (DlcManager.FeatureClusterSpaceEnabled() ? "OrbitalResearchDatabank" : "ResearchDatabank");
		return new string[3][]
		{
			new string[3] { text, text, text },
			new string[3] { "ColdBreatherSeed", "ColdBreatherSeed", "ColdBreatherSeed" },
			new string[4] { "Atmo_Suit", "Glom", "Glom", "Glom" }
		};
	}

	public void OnPrefabInit(GameObject inst)
	{
		SetLocker component = inst.GetComponent<SetLocker>();
		component.possible_contents_ids = GetLockerBaseContents();
		component.ChooseContents();
		OccupyArea component2 = inst.GetComponent<OccupyArea>();
		component2.objectLayers = new ObjectLayer[1] { ObjectLayer.Building };
		int cell = Grid.PosToCell(inst);
		CellOffset[] occupiedCellsOffsets = component2.OccupiedCellsOffsets;
		foreach (CellOffset offset in occupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(cell, offset)] = true;
		}
		RadiationEmitter radiationEmitter = inst.AddOrGet<RadiationEmitter>();
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.radiusProportionalToRads = false;
		radiationEmitter.emitRadiusX = 12;
		radiationEmitter.emitRadiusY = 12;
		radiationEmitter.emitRads = 2400f / ((float)radiationEmitter.emitRadiusX / 6f);
	}

	public void OnSpawn(GameObject inst)
	{
		RadiationEmitter component = inst.GetComponent<RadiationEmitter>();
		if (component != null)
		{
			component.SetEmitting(emitting: true);
		}
	}
}
