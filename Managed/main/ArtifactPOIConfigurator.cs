using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ArtifactPOIConfigurator")]
public class ArtifactPOIConfigurator : KMonoBehaviour
{
	public class ArtifactPOIType
	{
		public string id;

		public HashedString idHash;

		public string harvestableArtifactID;

		public bool destroyOnHarvest;

		public float poiRechargeTimeMin;

		public float poiRechargeTimeMax;

		public string dlcID;

		public ArtifactPOIType(string id, string harvestableArtifactID = null, bool destroyOnHarvest = false, float poiRechargeTimeMin = 30000f, float poiRechargeTimeMax = 60000f, string dlcID = "EXPANSION1_ID")
		{
			this.id = id;
			idHash = id;
			this.harvestableArtifactID = harvestableArtifactID;
			this.destroyOnHarvest = destroyOnHarvest;
			this.poiRechargeTimeMin = poiRechargeTimeMin;
			this.poiRechargeTimeMax = poiRechargeTimeMax;
			this.dlcID = dlcID;
			if (_poiTypes == null)
			{
				_poiTypes = new List<ArtifactPOIType>();
			}
			_poiTypes.Add(this);
		}
	}

	[Serializable]
	public class ArtifactPOIInstanceConfiguration
	{
		public HashedString typeId;

		private bool didInit = false;

		public float rechargeRoll;

		private float poiRechargeTime;

		public ArtifactPOIType poiType => FindType(typeId);

		private void Init()
		{
			if (!didInit)
			{
				didInit = true;
				poiRechargeTime = MathUtil.ReRange(rechargeRoll, 0f, 1f, poiType.poiRechargeTimeMin, poiType.poiRechargeTimeMax);
			}
		}

		public bool DestroyOnHarvest()
		{
			Init();
			return poiType.destroyOnHarvest;
		}

		public string GetArtifactID()
		{
			Init();
			return poiType.harvestableArtifactID;
		}

		public float GetRechargeTime()
		{
			Init();
			return poiRechargeTime;
		}
	}

	private static List<ArtifactPOIType> _poiTypes;

	public static ArtifactPOIType defaultArtifactPoiType = new ArtifactPOIType("HarvestablePOIArtifacts");

	public HashedString presetType;

	public float presetMin = 0f;

	public float presetMax = 1f;

	public static ArtifactPOIType FindType(HashedString typeId)
	{
		ArtifactPOIType artifactPOIType = null;
		if (typeId != HashedString.Invalid)
		{
			artifactPOIType = _poiTypes.Find((ArtifactPOIType t) => t.id == typeId);
		}
		if (artifactPOIType == null)
		{
			Debug.LogError($"Tried finding a harvestable poi with id {typeId.ToString()} but it doesn't exist!");
		}
		return artifactPOIType;
	}

	public ArtifactPOIInstanceConfiguration MakeConfiguration()
	{
		return CreateRandomInstance(presetType, presetMin, presetMax);
	}

	private ArtifactPOIInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
		ClusterGridEntity component = GetComponent<ClusterGridEntity>();
		Vector3 position = ClusterGrid.Instance.GetPosition(component);
		globalWorldSeed = globalWorldSeed + (int)position.x + (int)position.y;
		System.Random randomSource = new System.Random(globalWorldSeed);
		return new ArtifactPOIInstanceConfiguration
		{
			typeId = typeId,
			rechargeRoll = Roll(randomSource, min, max)
		};
	}

	private float Roll(System.Random randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}
}
