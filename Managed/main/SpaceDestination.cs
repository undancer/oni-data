using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceDestination
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class ResearchOpportunity
	{
		[Serialize]
		public string description;

		[Serialize]
		public int dataValue;

		[Serialize]
		public bool completed;

		[Serialize]
		public SimHashes discoveredRareResource = SimHashes.Void;

		[Serialize]
		public string discoveredRareItem;

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (discoveredRareResource == (SimHashes)0)
			{
				discoveredRareResource = SimHashes.Void;
			}
			if (dataValue > 50)
			{
				dataValue = 50;
			}
		}

		public ResearchOpportunity(string description, int pointValue)
		{
			this.description = description;
			dataValue = pointValue;
		}

		public bool TryComplete(SpaceDestination destination)
		{
			if (!completed)
			{
				completed = true;
				if (discoveredRareResource != SimHashes.Void && !destination.recoverableElements.ContainsKey(discoveredRareResource))
				{
					destination.recoverableElements.Add(discoveredRareResource, Random.value);
				}
				return true;
			}
			return false;
		}
	}

	private const int MASS_TO_RECOVER_AMOUNT = 1000;

	private static List<Tuple<float, int>> RARE_ELEMENT_CHANCES = new List<Tuple<float, int>>
	{
		new Tuple<float, int>(1f, 0),
		new Tuple<float, int>(0.33f, 1),
		new Tuple<float, int>(0.03f, 2)
	};

	private static readonly List<Tuple<SimHashes, MathUtil.MinMax>> RARE_ELEMENTS = new List<Tuple<SimHashes, MathUtil.MinMax>>
	{
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Katairite, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Niobium, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Fullerene, new MathUtil.MinMax(1f, 10f)),
		new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Isoresin, new MathUtil.MinMax(1f, 10f))
	};

	private const float RARE_ITEM_CHANCE = 0.33f;

	private static readonly List<Tuple<string, MathUtil.MinMax>> RARE_ITEMS = new List<Tuple<string, MathUtil.MinMax>>
	{
		new Tuple<string, MathUtil.MinMax>("GeneShufflerRecharge", new MathUtil.MinMax(1f, 2f))
	};

	[Serialize]
	public int id;

	[Serialize]
	public string type;

	public bool startAnalyzed;

	[Serialize]
	public int distance;

	[Serialize]
	public float activePeriod = 20f;

	[Serialize]
	public float inactivePeriod = 10f;

	[Serialize]
	public float startingOrbitPercentage;

	[Serialize]
	public Dictionary<SimHashes, float> recoverableElements = new Dictionary<SimHashes, float>();

	[Serialize]
	public List<ResearchOpportunity> researchOpportunities = new List<ResearchOpportunity>();

	public List<SpaceMission> missions = new List<SpaceMission>();

	[Serialize]
	private float availableMass;

	public int OneBasedDistance => distance + 1;

	public float CurrentMass => (float)GetDestinationType().minimumMass + availableMass;

	public float AvailableMass => availableMass;

	private static Tuple<SimHashes, MathUtil.MinMax> GetRareElement(SimHashes id)
	{
		foreach (Tuple<SimHashes, MathUtil.MinMax> rARE_ELEMENT in RARE_ELEMENTS)
		{
			if (rARE_ELEMENT.first == id)
			{
				return rARE_ELEMENT;
			}
		}
		return null;
	}

	public SpaceDestination(int id, string type, int distance)
	{
		this.id = id;
		this.type = type;
		this.distance = distance;
		SpaceDestinationType destinationType = GetDestinationType();
		availableMass = destinationType.maxiumMass - destinationType.minimumMass;
		GenerateSurfaceElements();
		GenerateMissions();
		GenerateResearchOpportunities();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 9))
		{
			SpaceDestinationType destinationType = GetDestinationType();
			availableMass = destinationType.maxiumMass - destinationType.minimumMass;
		}
	}

	public SpaceDestinationType GetDestinationType()
	{
		return Db.Get().SpaceDestinationTypes.Get(type);
	}

	public float GetCurrentOrbitPercentage()
	{
		float num = 0.1f * Mathf.Pow(OneBasedDistance, 2f);
		return ((float)GameClock.Instance.GetCycle() + GameClock.Instance.GetCurrentCycleAsPercentage() + startingOrbitPercentage * num) % num / num;
	}

	public ResearchOpportunity TryCompleteResearchOpportunity()
	{
		foreach (ResearchOpportunity researchOpportunity in researchOpportunities)
		{
			if (researchOpportunity.TryComplete(this))
			{
				return researchOpportunity;
			}
		}
		return null;
	}

	public void GenerateSurfaceElements()
	{
		foreach (KeyValuePair<SimHashes, MathUtil.MinMax> item in GetDestinationType().elementTable)
		{
			recoverableElements.Add(item.Key, Random.value);
		}
	}

	public SpacecraftManager.DestinationAnalysisState AnalysisState()
	{
		return SpacecraftManager.instance.GetDestinationAnalysisState(this);
	}

	public void GenerateResearchOpportunities()
	{
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.UPPERATMO, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.LOWERATMO, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.MAGNETICFIELD, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.SURFACE, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		researchOpportunities.Add(new ResearchOpportunity(UI.STARMAP.DESTINATIONSTUDY.SUBSURFACE, ROCKETRY.DESTINATION_RESEARCH.BASIC));
		float num = 0f;
		foreach (Tuple<float, int> rARE_ELEMENT_CHANCE in RARE_ELEMENT_CHANCES)
		{
			num += rARE_ELEMENT_CHANCE.first;
		}
		float num2 = Random.value * num;
		int num3 = 0;
		foreach (Tuple<float, int> rARE_ELEMENT_CHANCE2 in RARE_ELEMENT_CHANCES)
		{
			num2 -= rARE_ELEMENT_CHANCE2.first;
			if (num2 <= 0f)
			{
				num3 = rARE_ELEMENT_CHANCE2.second;
			}
		}
		for (int i = 0; i < num3; i++)
		{
			researchOpportunities[Random.Range(0, researchOpportunities.Count)].discoveredRareResource = RARE_ELEMENTS[Random.Range(0, RARE_ELEMENTS.Count)].first;
		}
		if (Random.value < 0.33f)
		{
			int index = Random.Range(0, researchOpportunities.Count);
			researchOpportunities[index].discoveredRareItem = RARE_ITEMS[Random.Range(0, RARE_ITEMS.Count)].first;
		}
	}

	public void GenerateMissions()
	{
		bool flag = true;
		foreach (SpaceMission mission in missions)
		{
			if (mission.craft == null)
			{
				flag = false;
			}
		}
		if (flag)
		{
			missions.Add(new SpaceMission(this));
		}
	}

	public float GetResourceValue(SimHashes resource, float roll)
	{
		if (GetDestinationType().elementTable.ContainsKey(resource))
		{
			return GetDestinationType().elementTable[resource].Lerp(roll);
		}
		if (SpaceDestinationTypes.extendedElementTable.ContainsKey(resource))
		{
			return SpaceDestinationTypes.extendedElementTable[resource].Lerp(roll);
		}
		return 0f;
	}

	public Dictionary<SimHashes, float> GetMissionResourceResult(float totalCargoSpace, bool solids = true, bool liquids = true, bool gasses = true)
	{
		Dictionary<SimHashes, float> dictionary = new Dictionary<SimHashes, float>();
		float num = 0f;
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && gasses))
			{
				num += GetResourceValue(recoverableElement.Key, recoverableElement.Value);
			}
		}
		float num2 = Mathf.Min(CurrentMass - (float)GetDestinationType().minimumMass, totalCargoSpace);
		foreach (KeyValuePair<SimHashes, float> recoverableElement2 in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement2.Key).IsSolid && solids) || (ElementLoader.FindElementByHash(recoverableElement2.Key).IsLiquid && liquids) || (ElementLoader.FindElementByHash(recoverableElement2.Key).IsGas && gasses))
			{
				float value = num2 * (GetResourceValue(recoverableElement2.Key, recoverableElement2.Value) / num);
				dictionary.Add(recoverableElement2.Key, value);
			}
		}
		return dictionary;
	}

	public Dictionary<Tag, int> GetRecoverableEntities()
	{
		Dictionary<Tag, int> dictionary = new Dictionary<Tag, int>();
		Dictionary<string, int> recoverableEntities = GetDestinationType().recoverableEntities;
		if (recoverableEntities != null)
		{
			foreach (KeyValuePair<string, int> item in recoverableEntities)
			{
				dictionary.Add(item.Key, item.Value);
			}
			return dictionary;
		}
		return dictionary;
	}

	public Dictionary<Tag, int> GetMissionEntityResult()
	{
		return GetRecoverableEntities();
	}

	public void UpdateRemainingResources(CargoBay bay)
	{
		if (!(bay != null))
		{
			return;
		}
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			_ = recoverableElement;
			if (HasElementType(bay.storageType))
			{
				Storage component = bay.GetComponent<Storage>();
				availableMass = Mathf.Max(0f, availableMass - component.capacityKg);
				break;
			}
		}
	}

	public bool HasElementType(CargoBay.CargoType type)
	{
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && type == CargoBay.CargoType.solids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && type == CargoBay.CargoType.liquids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && type == CargoBay.CargoType.gasses))
			{
				return true;
			}
		}
		return false;
	}

	public void Replenish(float dt)
	{
		SpaceDestinationType destinationType = GetDestinationType();
		if (CurrentMass < (float)destinationType.maxiumMass)
		{
			availableMass += destinationType.replishmentPerSim1000ms;
		}
	}

	public float GetAvailableResourcesPercentage(CargoBay.CargoType cargoType)
	{
		float num = 0f;
		float totalMass = GetTotalMass();
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			if ((ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && cargoType == CargoBay.CargoType.solids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && cargoType == CargoBay.CargoType.liquids) || (ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && cargoType == CargoBay.CargoType.gasses))
			{
				num += GetResourceValue(recoverableElement.Key, recoverableElement.Value) / totalMass;
			}
		}
		return num;
	}

	public float GetTotalMass()
	{
		float num = 0f;
		foreach (KeyValuePair<SimHashes, float> recoverableElement in recoverableElements)
		{
			num += GetResourceValue(recoverableElement.Key, recoverableElement.Value);
		}
		return num;
	}
}
