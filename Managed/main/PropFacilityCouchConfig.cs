using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

public class PropFacilityCouchConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("PropFacilityCouch", STRINGS.BUILDINGS.PREFABS.PROPFACILITYCOUCH.NAME, STRINGS.BUILDINGS.PREFABS.PROPFACILITYCOUCH.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_couch_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 4, height: 2, element: SimHashes.Creature, additionalTags: new List<Tag>
		{
			GameTags.Gravitas
		});
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Granite);
		component.Temperature = 294.15f;
		gameObject.AddOrGet<Demolishable>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		int cell = Grid.PosToCell(inst);
		CellOffset[] occupiedCellsOffsets = component.OccupiedCellsOffsets;
		foreach (CellOffset offset in occupiedCellsOffsets)
		{
			Grid.GravitasFacility[Grid.OffsetCell(cell, offset)] = true;
		}
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
