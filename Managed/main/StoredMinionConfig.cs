using Klei.AI;
using UnityEngine;

public class StoredMinionConfig : IEntityConfig
{
	public static string ID = "StoredMinion";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<KPrefabID>().AddTag(ID);
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Schedulable>();
		gameObject.AddOrGet<StoredMinionIdentity>();
		gameObject.AddOrGet<KSelectable>().IsSelectable = false;
		gameObject.AddOrGet<MinionModifiers>().addBaseTraits = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
