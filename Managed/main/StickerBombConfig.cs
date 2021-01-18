using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class StickerBombConfig : IEntityConfig
{
	public const string ID = "StickerBomb";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity("StickerBomb", STRINGS.BUILDINGS.PREFABS.STICKERBOMB.NAME, STRINGS.BUILDINGS.PREFABS.STICKERBOMB.DESC, 1f, unitMass: true, Assets.GetAnim("sticker_kanim"), "off", Grid.SceneLayer.Backwall);
		EntityTemplates.AddCollision(gameObject, EntityTemplates.CollisionShape.RECTANGLE, 1f, 1f);
		gameObject.AddOrGet<StickerBomb>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
		inst.AddOrGet<OccupyArea>().OccupiedCellsOffsets = new CellOffset[1];
		inst.AddComponent<Modifiers>();
		inst.AddOrGet<DecorProvider>().SetValues(DECOR.BONUS.TIER2);
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
