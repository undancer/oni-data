using STRINGS;
using UnityEngine;

public class EggShellConfig : IEntityConfig
{
	public const string ID = "EggShell";

	public static readonly Tag TAG = TagManager.Create("EggShell");

	public const float EGG_TO_SHELL_RATIO = 0.5f;

	public string GetDlcId()
	{
		return "";
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("EggShell", ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.NAME, ITEMS.INDUSTRIAL_PRODUCTS.EGG_SHELL.DESC, 1f, unitMass: false, Assets.GetAnim("eggshells_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, isPickupable: true);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.AddTag(GameTags.Organics);
		gameObject.AddOrGet<EntitySplitter>();
		gameObject.AddOrGet<SimpleMassStatusItem>();
		EntityTemplates.CreateAndRegisterCompostableFromPrefab(gameObject);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
