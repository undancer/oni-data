using System.Collections.Generic;
using UnityEngine;

public class ArtifactPOIConfig : IMultiEntityConfig
{
	public struct ArtifactPOIParams
	{
		public string id;

		public string anim;

		public StringKey nameStringKey;

		public StringKey descStringKey;

		public ArtifactPOIConfigurator.ArtifactPOIType poiType;

		public ArtifactPOIParams(string anim, ArtifactPOIConfigurator.ArtifactPOIType poiType)
		{
			id = "ArtifactSpacePOI_" + poiType.id;
			this.anim = anim;
			nameStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + poiType.id.ToUpper() + ".NAME");
			descStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + poiType.id.ToUpper() + ".DESC");
			this.poiType = poiType;
		}
	}

	public const string GravitasSpaceStation1 = "GravitasSpaceStation1";

	public const string GravitasSpaceStation2 = "GravitasSpaceStation2";

	public const string GravitasSpaceStation3 = "GravitasSpaceStation3";

	public const string GravitasSpaceStation4 = "GravitasSpaceStation4";

	public const string GravitasSpaceStation5 = "GravitasSpaceStation5";

	public const string GravitasSpaceStation6 = "GravitasSpaceStation6";

	public const string GravitasSpaceStation7 = "GravitasSpaceStation7";

	public const string GravitasSpaceStation8 = "GravitasSpaceStation8";

	public const string RussellsTeapot = "RussellsTeapot";

	public List<GameObject> CreatePrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		List<ArtifactPOIParams> list2 = GenerateConfigs();
		foreach (ArtifactPOIParams item in list2)
		{
			list.Add(CreateArtifactPOI(item.id, item.anim, Strings.Get(item.nameStringKey), Strings.Get(item.descStringKey), item.poiType.idHash));
		}
		return list;
	}

	public static GameObject CreateArtifactPOI(string id, string anim, string name, string desc, HashedString poiType)
	{
		GameObject gameObject = EntityTemplates.CreateEntity(id, id);
		gameObject.AddOrGet<SaveLoadRoot>();
		ArtifactPOIConfigurator artifactPOIConfigurator = gameObject.AddOrGet<ArtifactPOIConfigurator>();
		artifactPOIConfigurator.presetType = poiType;
		ArtifactPOIClusterGridEntity artifactPOIClusterGridEntity = gameObject.AddOrGet<ArtifactPOIClusterGridEntity>();
		artifactPOIClusterGridEntity.m_name = name;
		artifactPOIClusterGridEntity.m_Anim = anim;
		gameObject.AddOrGetDef<ArtifactPOIStates.Def>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	private List<ArtifactPOIParams> GenerateConfigs()
	{
		List<ArtifactPOIParams> list = new List<ArtifactPOIParams>();
		list.Add(new ArtifactPOIParams("station_1", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation1")));
		list.Add(new ArtifactPOIParams("station_2", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation2")));
		list.Add(new ArtifactPOIParams("station_3", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation3")));
		list.Add(new ArtifactPOIParams("station_4", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation4")));
		list.Add(new ArtifactPOIParams("station_5", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation5")));
		list.Add(new ArtifactPOIParams("station_6", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation6")));
		list.Add(new ArtifactPOIParams("station_7", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation7")));
		list.Add(new ArtifactPOIParams("station_8", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation8")));
		list.Add(new ArtifactPOIParams("russels_teapot", new ArtifactPOIConfigurator.ArtifactPOIType("RussellsTeapot", "artifact_TeaPot", destroyOnHarvest: true)));
		list.RemoveAll((ArtifactPOIParams poi) => !poi.poiType.dlcID.IsNullOrWhiteSpace() && !DlcManager.IsContentActive(poi.poiType.dlcID));
		return list;
	}
}
