using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ResearchCenter")]
public class ResearchCenter : Workable, IGameObjectEffectDescriptor, ISim200ms, IResearchCenter
{
	private Chore chore;

	[MyCmpAdd]
	protected Notifier notifier;

	[MyCmpAdd]
	protected Operational operational;

	[MyCmpAdd]
	protected Storage storage;

	[MyCmpGet]
	private ElementConverter elementConverter;

	[SerializeField]
	public string research_point_type_id;

	[SerializeField]
	public Tag inputMaterial;

	[SerializeField]
	public float mass_per_point;

	[SerializeField]
	private float remainder_mass_points;

	public static readonly Operational.Flag ResearchSelectedFlag = new Operational.Flag("researchSelected", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<ResearchCenter> UpdateWorkingStateDelegate = new EventSystem.IntraObjectHandler<ResearchCenter>(delegate(ResearchCenter component, object data)
	{
		component.UpdateWorkingState(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ResearchCenter> CheckHasMaterialDelegate = new EventSystem.IntraObjectHandler<ResearchCenter>(delegate(ResearchCenter component, object data)
	{
		component.CheckHasMaterial(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		workerStatusItem = Db.Get().DuplicantStatusItems.Researching;
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
		ElementConverter obj = elementConverter;
		obj.onConvertMass = (Action<float>)Delegate.Combine(obj.onConvertMass, new Action<float>(ConvertMassToResearchPoints));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1914338957, UpdateWorkingStateDelegate);
		Subscribe(-125623018, UpdateWorkingStateDelegate);
		Subscribe(187661686, UpdateWorkingStateDelegate);
		Subscribe(-1697596308, CheckHasMaterialDelegate);
		Components.ResearchCenters.Add(this);
		UpdateWorkingState(null);
	}

	private void ConvertMassToResearchPoints(float mass_consumed)
	{
		remainder_mass_points += mass_consumed / mass_per_point - (float)Mathf.FloorToInt(mass_consumed / mass_per_point);
		int num = Mathf.FloorToInt(mass_consumed / mass_per_point);
		num += Mathf.FloorToInt(remainder_mass_points);
		remainder_mass_points -= Mathf.FloorToInt(remainder_mass_points);
		ResearchType researchType = Research.Instance.GetResearchType(research_point_type_id);
		if (num > 0)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, researchType.name, base.transform);
			for (int i = 0; i < num; i++)
			{
				Research.Instance.AddResearchPoints(research_point_type_id, 1f);
			}
		}
	}

	public void Sim200ms(float dt)
	{
		if (!operational.IsActive && operational.IsOperational && chore == null && HasMaterial())
		{
			chore = CreateChore();
			SetWorkTime(float.PositiveInfinity);
		}
	}

	protected virtual Chore CreateChore()
	{
		return new WorkChore<ResearchCenter>(Db.Get().ChoreTypes.Research, this, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: true)
		{
			preemption_cb = CanPreemptCB
		};
	}

	private static bool CanPreemptCB(Chore.Precondition.Context context)
	{
		Worker component = context.chore.driver.GetComponent<Worker>();
		float num = Db.Get().AttributeConverters.ResearchSpeed.Lookup(component).Evaluate();
		Worker worker = context.consumerState.worker;
		return Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate() > num;
	}

	public override float GetPercentComplete()
	{
		if (Research.Instance.GetActiveResearch() == null)
		{
			return 0f;
		}
		float num = Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID[research_point_type_id];
		float value = 0f;
		if (!Research.Instance.GetActiveResearch().tech.costsByResearchTypeID.TryGetValue(research_point_type_id, out value))
		{
			return 1f;
		}
		return num / value;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		operational.SetActive(value: true);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		float num = (currentlyLit ? (1f + DUPLICANTSTATS.LIGHT.LIGHT_WORK_EFFICIENCY_BONUS) : 1f);
		float num2 = 1f + Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate() + num;
		if (Game.Instance.FastWorkersModeActive)
		{
			num2 *= 2f;
		}
		elementConverter.SetWorkSpeedMultiplier(num2);
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		ShowProgressBar(show: false);
		operational.SetActive(value: false);
	}

	protected bool ResearchComponentCompleted()
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null)
		{
			float value = 0f;
			float value2 = 0f;
			activeResearch.progressInventory.PointsByTypeID.TryGetValue(research_point_type_id, out value);
			activeResearch.tech.costsByResearchTypeID.TryGetValue(research_point_type_id, out value2);
			if (value >= value2)
			{
				return true;
			}
		}
		return false;
	}

	protected bool IsAllResearchComplete()
	{
		foreach (Tech resource in Db.Get().Techs.resources)
		{
			if (!resource.IsComplete())
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void UpdateWorkingState(object data)
	{
		bool flag = false;
		bool flag2 = false;
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null)
		{
			flag = true;
			if (activeResearch.tech.costsByResearchTypeID.ContainsKey(research_point_type_id) && Research.Instance.Get(activeResearch.tech).progressInventory.PointsByTypeID[research_point_type_id] < activeResearch.tech.costsByResearchTypeID[research_point_type_id])
			{
				flag2 = true;
			}
		}
		if (operational.GetFlag(EnergyConsumer.PoweredFlag) && !IsAllResearchComplete())
		{
			if (flag)
			{
				GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
				if (!flag2 && !ResearchComponentCompleted())
				{
					GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
					GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
				}
				else
				{
					GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
				}
			}
			else
			{
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
				GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
			}
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected);
		}
		operational.SetFlag(ResearchSelectedFlag, flag && flag2);
		if ((!flag || !flag2) && (bool)base.worker)
		{
			StopWork(base.worker, aborted: true);
		}
	}

	private void ClearResearchScreen()
	{
		Game.Instance.Trigger(-1974454597);
	}

	public string GetResearchType()
	{
		return research_point_type_id;
	}

	private void CheckHasMaterial(object o = null)
	{
		if (!HasMaterial() && chore != null)
		{
			chore.Cancel("No material remaining");
			chore = null;
		}
	}

	private bool HasMaterial()
	{
		return storage.MassStored() > 0f;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, UpdateWorkingState);
		Research.Instance.Unsubscribe(-125623018, UpdateWorkingState);
		Unsubscribe(-1852328367, UpdateWorkingState);
		Components.ResearchCenters.Remove(this);
		ClearResearchScreen();
	}

	public string GetStatusString()
	{
		string result = RESEARCH.MESSAGING.NORESEARCHSELECTED;
		if (Research.Instance.GetActiveResearch() != null)
		{
			result = "<b>" + Research.Instance.GetActiveResearch().tech.Name + "</b>";
			int num = 0;
			foreach (KeyValuePair<string, float> item in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[item.Key] != 0f)
				{
					num++;
				}
			}
			foreach (KeyValuePair<string, float> item2 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[item2.Key] != 0f && item2.Key == research_point_type_id)
				{
					result = result + "\n   - " + Research.Instance.researchTypes.GetResearchType(item2.Key).name;
					result = result + ": " + item2.Value + "/" + Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[item2.Key];
				}
			}
			{
				foreach (KeyValuePair<string, float> item3 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
				{
					if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[item3.Key] != 0f && !(item3.Key == research_point_type_id))
					{
						result = ((num <= 1) ? (result + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEREQUIRED, Research.Instance.researchTypes.GetResearchType(item3.Key).name)) : (result + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEALSOREQUIRED, Research.Instance.researchTypes.GetResearchType(item3.Key).name)));
					}
				}
				return result;
			}
		}
		return result;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.RESEARCH_MATERIALS, inputMaterial.ProperName(), GameUtil.GetFormattedByTag(inputMaterial, mass_per_point)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.RESEARCH_MATERIALS, inputMaterial.ProperName(), GameUtil.GetFormattedByTag(inputMaterial, mass_per_point)), Descriptor.DescriptorType.Requirement));
		descriptors.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(research_point_type_id).name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(research_point_type_id).name)));
		return descriptors;
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}
}
