using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropSurfaceSatellite3Config : IEntityConfig
{
	public static string ID = "PropSurfaceSatellite3";

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(ID, STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE3.NAME, STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE3.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("satellite3_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 6, height: 6, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
		setLocker.dropOffset = new Vector2I(0, 1);
		gameObject.AddOrGet<LoreBearer>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		SetLocker component = inst.GetComponent<SetLocker>();
		component.possible_contents_ids = new string[3][]
		{
			new string[3]
			{
				"ResearchDatabank",
				"ResearchDatabank",
				"ResearchDatabank"
			},
			new string[3]
			{
				"ColdBreatherSeed",
				"ColdBreatherSeed",
				"ColdBreatherSeed"
			},
			new string[4]
			{
				"Atmo_Suit",
				"Glom",
				"Glom",
				"Glom"
			}
		};
		component.ChooseContents();
		OccupyArea component2 = inst.GetComponent<OccupyArea>();
		component2.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		int cell = Grid.PosToCell(inst);
		CellOffset[] occupiedCellsOffsets = component2.OccupiedCellsOffsets;
		foreach (CellOffset offset in occupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(cell, offset)] = true;
		}
		Radiator radiator = inst.AddOrGet<Radiator>();
		radiator.intensity = 100;
		radiator.projectionCount = 24;
		radiator.angle = 360;
		radiator.direction = 0;
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
