using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HarvestablePOIConfigurator")]
public class HarvestablePOIConfigurator : KMonoBehaviour
{
	public class HarvestablePOIType
	{
		public string id;

		public HashedString idHash;

		public Dictionary<SimHashes, float> harvestableElements;

		public float poiCapacityMin;

		public float poiCapacityMax;

		public float poiRechargeMin;

		public float poiRechargeMax;

		public bool canProvideArtifacts;

		public string dlcID;

		public HarvestablePOIType(string id, Dictionary<SimHashes, float> harvestableElements, float poiCapacityMin = 54000f, float poiCapacityMax = 81000f, float poiRechargeMin = 30000f, float poiRechargeMax = 60000f, bool canProvideArtifacts = true, string dlcID = "EXPANSION1_ID")
		{
			this.id = id;
			idHash = id;
			this.harvestableElements = harvestableElements;
			this.poiCapacityMin = poiCapacityMin;
			this.poiCapacityMax = poiCapacityMax;
			this.poiRechargeMin = poiRechargeMin;
			this.poiRechargeMax = poiRechargeMax;
			this.canProvideArtifacts = canProvideArtifacts;
			this.dlcID = dlcID;
			if (_poiTypes == null)
			{
				_poiTypes = new List<HarvestablePOIType>();
			}
			_poiTypes.Add(this);
		}
	}

	[Serializable]
	public class HarvestablePOIInstanceConfiguration
	{
		public HashedString typeId;

		private bool didInit;

		public float capacityRoll;

		public float rechargeRoll;

		private float poiTotalCapacity;

		private float poiRecharge;

		public HarvestablePOIType poiType => FindType(typeId);

		private void Init()
		{
			if (!didInit)
			{
				didInit = true;
				poiTotalCapacity = MathUtil.ReRange(capacityRoll, 0f, 1f, poiType.poiCapacityMin, poiType.poiCapacityMax);
				poiRecharge = MathUtil.ReRange(rechargeRoll, 0f, 1f, poiType.poiRechargeMin, poiType.poiRechargeMax);
			}
		}

		public Dictionary<SimHashes, float> GetElementsWithWeights()
		{
			Init();
			return poiType.harvestableElements;
		}

		public bool CanProvideArtifacts()
		{
			Init();
			return poiType.canProvideArtifacts;
		}

		public float GetMaxCapacity()
		{
			Init();
			return poiTotalCapacity;
		}

		public float GetRechargeTime()
		{
			Init();
			return poiRecharge;
		}
	}

	private static List<HarvestablePOIType> _poiTypes;

	public HashedString presetType;

	public float presetMin;

	public float presetMax = 1f;

	public static HarvestablePOIType FindType(HashedString typeId)
	{
		HarvestablePOIType harvestablePOIType = null;
		if (typeId != HashedString.Invalid)
		{
			harvestablePOIType = _poiTypes.Find((HarvestablePOIType t) => t.id == typeId);
		}
		if (harvestablePOIType == null)
		{
			Debug.LogError($"Tried finding a harvestable poi with id {typeId.ToString()} but it doesn't exist!");
		}
		return harvestablePOIType;
	}

	public HarvestablePOIInstanceConfiguration MakeConfiguration()
	{
		return CreateRandomInstance(presetType, presetMin, presetMax);
	}

	private HarvestablePOIInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
		ClusterGridEntity component = GetComponent<ClusterGridEntity>();
		Vector3 position = ClusterGrid.Instance.GetPosition(component);
		System.Random randomSource = new System.Random(globalWorldSeed + (int)position.x + (int)position.y);
		return new HarvestablePOIInstanceConfiguration
		{
			typeId = typeId,
			capacityRoll = Roll(randomSource, min, max),
			rechargeRoll = Roll(randomSource, min, max)
		};
	}

	private float Roll(System.Random randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}
}
