using STRINGS;
using TUNING;
using UnityEngine;

public class TemporalTearAnalyzerConfig : IEntityConfig
{
	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("TemporalTearAnalyzer", STRINGS.BUILDINGS.PREFABS.TEMPORALTEARANALYZER.NAME, STRINGS.BUILDINGS.PREFABS.TEMPORALTEARANALYZER.DESC, 2000f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("temporal_tear_analyzer_kanim"), initialAnim: "off", sceneLayer: Grid.SceneLayer.Building, width: 5, height: 4);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		gameObject.AddOrGetDef<TemporalTearAnalyzer.Def>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		OccupyArea component = inst.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
