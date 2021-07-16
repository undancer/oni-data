using UnityEngine;

public class EyeAnimation : IEntityConfig
{
	public static string ID = "EyeAnimation";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_ALL_VERSIONS;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, is_selectable: false);
		gameObject.AddOrGet<KBatchedAnimController>().AnimFiles = new KAnimFile[1]
		{
			Assets.GetAnim("anim_blinks_kanim")
		};
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
