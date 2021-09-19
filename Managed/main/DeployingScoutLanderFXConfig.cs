using UnityEngine;

public class DeployingScoutLanderFXConfig : IEntityConfig
{
	public const string ID = "DeployingScoutLanderFXConfig";

	public string[] GetDlcIds()
	{
		return DlcManager.AVAILABLE_EXPANSION1_ONLY;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity("DeployingScoutLanderFXConfig", "DeployingScoutLanderFXConfig", is_selectable: false);
		ClusterFXEntity clusterFXEntity = gameObject.AddOrGet<ClusterFXEntity>();
		clusterFXEntity.kAnimName = "rover01_kanim";
		clusterFXEntity.animName = "landing";
		clusterFXEntity.animPlayMode = KAnim.PlayMode.Loop;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
