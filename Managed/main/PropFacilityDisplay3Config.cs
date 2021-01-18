using STRINGS;
using TUNING;
using UnityEngine;

public class PropFacilityDisplay3Config : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject obj = EntityTemplates.CreatePlacedEntity("PropFacilityDisplay3", STRINGS.BUILDINGS.PREFABS.PROPFACILITYDISPLAY3.NAME, STRINGS.BUILDINGS.PREFABS.PROPFACILITYDISPLAY3.DESC, 50f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("gravitas_display3_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 2, height: 2);
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Steel);
		component.Temperature = 294.15f;
		obj.AddOrGet<LoreBearer>();
		return obj;
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
