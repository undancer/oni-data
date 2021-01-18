using STRINGS;
using TUNING;
using UnityEngine;

public class VendingMachineConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("VendingMachine", STRINGS.BUILDINGS.PREFABS.VENDINGMACHINE.NAME, STRINGS.BUILDINGS.PREFABS.VENDINGMACHINE.DESC, 100f, decor: TUNING.BUILDINGS.DECOR.BONUS.TIER0, noise: NOISE_POLLUTION.NOISY.TIER0, anim: Assets.GetAnim("vendingmachine_kanim"), initialAnim: "on", sceneLayer: Grid.SceneLayer.Building, width: 2, height: 3);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Unobtanium);
		component.Temperature = 294.15f;
		Workable workable = gameObject.AddOrGet<Workable>();
		workable.synchronizeAnims = false;
		workable.resetProgressOnStop = true;
		SetLocker setLocker = gameObject.AddOrGet<SetLocker>();
		setLocker.machineSound = "VendingMachine_LP";
		setLocker.overrideAnim = "anim_break_kanim";
		setLocker.dropOffset = new Vector2I(1, 1);
		setLocker.possible_contents_ids = new string[1]
		{
			"FieldRation"
		};
		gameObject.AddOrGet<LoreBearer>();
		gameObject.AddOrGet<LoopingSounds>();
		gameObject.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
