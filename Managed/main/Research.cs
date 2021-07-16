using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
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

	private Notification NoResearcherRole;

	private Notification MissingResearchStation = new Notification(RESEARCH.MESSAGING.MISSING_RESEARCH_STATION, NotificationType.Bad, (List<Notification> list, object data) => RESEARCH.MESSAGING.MISSING_RESEARCH_STATION_TOOLTIP.ToString().Replace("{0}", Instance.GetMissingResearchBuildingName()), null, expires: false, 11f);

	private List<IResearchCenter> researchCenterPrefabs = new List<IResearchCenter>();

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

	public TechInstance GetTechInstance(string techID)
	{
		return techs.Find((TechInstance match) => match.tech.Id == techID);
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
			IResearchCenter component = prefab.GetComponent<IResearchCenter>();
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
			NotifyResearchCenters(GameHashes.ActiveResearchChanged, queuedTech);
		}
	}

	private void NotifyResearchCenters(GameHashes hash, object data)
	{
		foreach (KMonoBehaviour researchCenter in Components.ResearchCenters)
		{
			researchCenter.Trigger(-1914338957, data);
		}
		Trigger((int)hash, data);
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
		NotifyResearchCenters(GameHashes.ActiveResearchChanged, queuedTech);
		CheckBuyResearch();
		CheckResearchBuildings(null);
		if (NoResearcherRole != null)
		{
			notifier.Remove(NoResearcherRole);
			NoResearcherRole = null;
		}
		if (activeResearch != null)
		{
			Skill skill = null;
			if (activeResearch.tech.costsByResearchTypeID.ContainsKey("advanced") && activeResearch.tech.costsByResearchTypeID["advanced"] > 0f && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
			{
				skill = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowAdvancedResearch)[0];
			}
			else if (activeResearch.tech.costsByResearchTypeID.ContainsKey("space") && activeResearch.tech.costsByResearchTypeID["space"] > 0f && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowInterstellarResearch.Id))
			{
				skill = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowInterstellarResearch)[0];
			}
			else if (activeResearch.tech.costsByResearchTypeID.ContainsKey("nuclear") && activeResearch.tech.costsByResearchTypeID["nuclear"] > 0f && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowNuclearResearch.Id))
			{
				skill = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowNuclearResearch)[0];
			}
			else if (activeResearch.tech.costsByResearchTypeID.ContainsKey("orbital") && activeResearch.tech.costsByResearchTypeID["orbital"] > 0f && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowOrbitalResearch.Id))
			{
				skill = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowOrbitalResearch)[0];
			}
			if (skill != null)
			{
				NoResearcherRole = new Notification(RESEARCH.MESSAGING.NO_RESEARCHER_SKILL, NotificationType.Bad, NoResearcherRoleTooltip, skill, expires: false, 12f);
				notifier.Add(NoResearcherRole);
			}
		}
	}

	private string NoResearcherRoleTooltip(List<Notification> list, object data)
	{
		Skill skill = (Skill)data;
		return RESEARCH.MESSAGING.NO_RESEARCHER_SKILL_TOOLTIP.Replace("{ResearchType}", skill.Name);
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
		NotifyResearchCenters(GameHashes.ResearchPointsChanged, null);
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
				foreach (IResearchCenter item2 in Components.ResearchCenters.Items)
				{
					if (item2.GetResearchType() == item.Key)
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
			foreach (IResearchCenter researchCenterPrefab in researchCenterPrefabs)
			{
				if (researchCenterPrefab.GetResearchType() == item.Key)
				{
					return ((KMonoBehaviour)researchCenterPrefab).GetProperName();
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
