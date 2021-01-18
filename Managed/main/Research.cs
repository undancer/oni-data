using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Research")]
public class Research : KMonoBehaviour, ISaveLoadable
{
	private struct SaveData
	{
		public string activeResearchId;

		public string targetResearchId;

		public TechInstance.SaveData[] techs;
	}

	public static Research Instance;

	[MyCmpAdd]
	private Notifier notifier;

	private List<TechInstance> techs = new List<TechInstance>();

	private List<TechInstance> queuedTech = new List<TechInstance>();

	private TechInstance activeResearch;

	private Notification NoResearcherRole = new Notification(RESEARCH.MESSAGING.NO_RESEARCHER_SKILL, NotificationType.Bad, HashedString.Invalid, (List<Notification> list, object data) => RESEARCH.MESSAGING.NO_RESEARCHER_SKILL_TOOLTIP, null, expires: false, 12f);

	private Notification MissingResearchStation = new Notification(RESEARCH.MESSAGING.MISSING_RESEARCH_STATION, NotificationType.Bad, HashedString.Invalid, (List<Notification> list, object data) => RESEARCH.MESSAGING.MISSING_RESEARCH_STATION_TOOLTIP.ToString().Replace("{0}", Instance.GetMissingResearchBuildingName()), null, expires: false, 11f);

	private List<ResearchCenter> researchCenterPrefabs = new List<ResearchCenter>();

	public ResearchTypes researchTypes;

	public bool UseGlobalPointInventory;

	[Serialize]
	public ResearchPointInventory globalPointInventory;

	[Serialize]
	private SaveData saveData;

	private static readonly EventSystem.IntraObjectHandler<Research> OnRolesUpdatedDelegate = new EventSystem.IntraObjectHandler<Research>(delegate(Research component, object data)
	{
		component.OnRolesUpdated(data);
	});

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public bool IsBeingResearched(Tech tech)
	{
		if (activeResearch == null || tech == null)
		{
			return false;
		}
		return activeResearch.tech == tech;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		researchTypes = new ResearchTypes();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (globalPointInventory == null)
		{
			globalPointInventory = new ResearchPointInventory();
		}
		Subscribe(-1523247426, OnRolesUpdatedDelegate);
		Components.ResearchCenters.OnAdd += CheckResearchBuildings;
		Components.ResearchCenters.OnRemove += CheckResearchBuildings;
		foreach (KPrefabID prefab in Assets.Prefabs)
		{
			ResearchCenter component = prefab.GetComponent<ResearchCenter>();
			if (component != null)
			{
				researchCenterPrefabs.Add(component);
			}
		}
	}

	public ResearchType GetResearchType(string id)
	{
		return researchTypes.GetResearchType(id);
	}

	public TechInstance GetActiveResearch()
	{
		return activeResearch;
	}

	public TechInstance GetTargetResearch()
	{
		if (queuedTech != null && queuedTech.Count > 0)
		{
			return queuedTech[queuedTech.Count - 1];
		}
		return null;
	}

	public TechInstance Get(Tech tech)
	{
		foreach (TechInstance tech2 in techs)
		{
			if (tech2.tech == tech)
			{
				return tech2;
			}
		}
		return null;
	}

	public TechInstance GetOrAdd(Tech tech)
	{
		TechInstance techInstance = techs.Find((TechInstance tc) => tc.tech == tech);
		if (techInstance != null)
		{
			return techInstance;
		}
		TechInstance techInstance2 = new TechInstance(tech);
		techs.Add(techInstance2);
		return techInstance2;
	}

	public void GetNextTech()
	{
		if (queuedTech.Count > 0)
		{
			queuedTech.RemoveAt(0);
		}
		if (queuedTech.Count > 0)
		{
			SetActiveResearch(queuedTech[queuedTech.Count - 1].tech);
		}
		else
		{
			SetActiveResearch(null);
		}
	}

	private void AddTechToQueue(Tech tech)
	{
		TechInstance orAdd = GetOrAdd(tech);
		if (!orAdd.IsComplete())
		{
			queuedTech.Add(orAdd);
		}
		orAdd.tech.requiredTech.ForEach(delegate(Tech _tech)
		{
			AddTechToQueue(_tech);
		});
	}

	public void CancelResearch(Tech tech, bool clickedEntry = true)
	{
		TechInstance ti = queuedTech.Find((TechInstance qt) => qt.tech == tech);
		if (ti == null)
		{
			return;
		}
		if (ti == queuedTech[queuedTech.Count - 1] && clickedEntry)
		{
			SetActiveResearch(null);
		}
		int i;
		for (i = ti.tech.unlockedTech.Count - 1; i >= 0; i--)
		{
			if (queuedTech.Find((TechInstance qt) => qt.tech == ti.tech.unlockedTech[i]) != null)
			{
				CancelResearch(ti.tech.unlockedTech[i], clickedEntry: false);
			}
		}
		queuedTech.Remove(ti);
		if (clickedEntry)
		{
			Trigger(-1914338957, queuedTech);
		}
	}

	public void SetActiveResearch(Tech tech, bool clearQueue = false)
	{
		if (clearQueue)
		{
			queuedTech.Clear();
		}
		activeResearch = null;
		if (tech != null)
		{
			if (queuedTech.Count == 0)
			{
				AddTechToQueue(tech);
			}
			if (queuedTech.Count > 0)
			{
				queuedTech.Sort((TechInstance x, TechInstance y) => x.tech.tier.CompareTo(y.tech.tier));
				activeResearch = queuedTech[0];
			}
		}
		else
		{
			queuedTech.Clear();
		}
		Trigger(-1914338957, queuedTech);
		CheckBuyResearch();
		CheckResearchBuildings(null);
		if (activeResearch != null)
		{
			if (activeResearch.tech.costsByResearchTypeID.Count > 1)
			{
				if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
				{
					notifier.Remove(NoResearcherRole);
					notifier.Add(NoResearcherRole);
				}
			}
			else
			{
				notifier.Remove(NoResearcherRole);
			}
			if (activeResearch.tech.costsByResearchTypeID.Count > 2)
			{
				if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowInterstellarResearch.Id))
				{
					notifier.Remove(NoResearcherRole);
					notifier.Add(NoResearcherRole);
				}
			}
			else
			{
				notifier.Remove(NoResearcherRole);
			}
		}
		else
		{
			notifier.Remove(NoResearcherRole);
		}
	}

	public void AddResearchPoints(string researchTypeID, float points)
	{
		if (!UseGlobalPointInventory && activeResearch == null)
		{
			Debug.LogWarning("No active research to add research points to. Global research inventory is disabled.");
			return;
		}
		(UseGlobalPointInventory ? globalPointInventory : activeResearch.progressInventory).AddResearchPoints(researchTypeID, points);
		CheckBuyResearch();
		Trigger(-125623018);
	}

	private void CheckBuyResearch()
	{
		if (activeResearch == null)
		{
			return;
		}
		ResearchPointInventory researchPointInventory = (UseGlobalPointInventory ? globalPointInventory : activeResearch.progressInventory);
		if (!activeResearch.tech.CanAfford(researchPointInventory))
		{
			return;
		}
		foreach (KeyValuePair<string, float> item in activeResearch.tech.costsByResearchTypeID)
		{
			researchPointInventory.RemoveResearchPoints(item.Key, item.Value);
		}
		activeResearch.Purchased();
		Game.Instance.Trigger(-107300940, activeResearch.tech);
		GetNextTech();
	}

	protected override void OnCleanUp()
	{
		Components.ResearchCenters.OnAdd -= CheckResearchBuildings;
		Components.ResearchCenters.OnRemove -= CheckResearchBuildings;
		base.OnCleanUp();
	}

	public void CompleteQueue()
	{
		while (queuedTech.Count > 0)
		{
			foreach (KeyValuePair<string, float> item in activeResearch.tech.costsByResearchTypeID)
			{
				AddResearchPoints(item.Key, item.Value);
			}
		}
	}

	public List<TechInstance> GetResearchQueue()
	{
		return new List<TechInstance>(queuedTech);
	}

	[OnSerializing]
	internal void OnSerializing()
	{
		saveData = default(SaveData);
		if (activeResearch != null)
		{
			saveData.activeResearchId = activeResearch.tech.Id;
		}
		else
		{
			saveData.activeResearchId = "";
		}
		if (queuedTech != null && queuedTech.Count > 0)
		{
			saveData.targetResearchId = queuedTech[queuedTech.Count - 1].tech.Id;
		}
		else
		{
			saveData.targetResearchId = "";
		}
		saveData.techs = new TechInstance.SaveData[techs.Count];
		for (int i = 0; i < techs.Count; i++)
		{
			saveData.techs[i] = techs[i].Save();
		}
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		if (saveData.techs != null)
		{
			TechInstance.SaveData[] array = saveData.techs;
			for (int i = 0; i < array.Length; i++)
			{
				TechInstance.SaveData save_data = array[i];
				Tech tech = Db.Get().Techs.TryGet(save_data.techId);
				if (tech != null)
				{
					GetOrAdd(tech).Load(save_data);
				}
			}
		}
		foreach (TechInstance tech2 in techs)
		{
			if (saveData.targetResearchId == tech2.tech.Id)
			{
				SetActiveResearch(tech2.tech);
				break;
			}
		}
	}

	private void OnRolesUpdated(object data)
	{
		if (activeResearch != null && activeResearch.tech.costsByResearchTypeID.Count > 1)
		{
			if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
			{
				notifier.Add(NoResearcherRole);
			}
			else
			{
				notifier.Remove(NoResearcherRole);
			}
		}
		else
		{
			notifier.Remove(NoResearcherRole);
		}
	}

	public string GetMissingResearchBuildingName()
	{
		foreach (KeyValuePair<string, float> item in activeResearch.tech.costsByResearchTypeID)
		{
			bool flag = true;
			if (item.Value > 0f)
			{
				flag = false;
				foreach (ResearchCenter item2 in Components.ResearchCenters.Items)
				{
					if (item2.research_point_type_id == item.Key)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				continue;
			}
			foreach (ResearchCenter researchCenterPrefab in researchCenterPrefabs)
			{
				if (researchCenterPrefab.research_point_type_id == item.Key)
				{
					return researchCenterPrefab.GetProperName();
				}
			}
			return null;
		}
		return null;
	}

	private void CheckResearchBuildings(object data)
	{
		if (activeResearch == null)
		{
			notifier.Remove(MissingResearchStation);
		}
		else if (string.IsNullOrEmpty(GetMissingResearchBuildingName()))
		{
			notifier.Remove(MissingResearchStation);
		}
		else
		{
			notifier.Add(MissingResearchStation);
		}
	}
}
