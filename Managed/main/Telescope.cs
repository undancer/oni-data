using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Telescope")]
public class Telescope : Workable, OxygenBreather.IGasProvider, IGameObjectEffectDescriptor, ISim200ms
{
	public int clearScanCellRadius = 15;

	private OxygenBreather.IGasProvider workerGasProvider = null;

	private Operational operational;

	private float percentClear = 0f;

	private static readonly Operational.Flag visibleSkyFlag = new Operational.Flag("VisibleSky", Operational.Flag.Type.Requirement);

	private static StatusItem reducedVisibilityStatusItem;

	private static StatusItem noVisibilityStatusItem;

	private Storage storage;

	public static readonly Chore.Precondition ContainsOxygen = new Chore.Precondition
	{
		id = "ContainsOxygen",
		sortOrder = 1,
		description = DUPLICANTS.CHORES.PRECONDITIONS.CONTAINS_OXYGEN,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			Storage component = context.chore.target.GetComponent<Storage>();
			PrimaryElement x = component.FindFirstWithMass(GameTags.Oxygen);
			return x != null;
		}
	};

	private Chore chore;

	private Operational.Flag flag = new Operational.Flag("ValidTarget", Operational.Flag.Type.Requirement);

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Research.Id;
		skillExperienceMultiplier = SKILLS.ALL_DAY_EXPERIENCE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SpacecraftManager.instance.Subscribe(532901469, UpdateWorkingState);
		Components.Telescopes.Add(this);
		if (reducedVisibilityStatusItem == null)
		{
			reducedVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_REDUCED", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			reducedVisibilityStatusItem.resolveStringCallback = GetStatusItemString;
			noVisibilityStatusItem = new StatusItem("SPACE_VISIBILITY_NONE", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
			noVisibilityStatusItem.resolveStringCallback = GetStatusItemString;
		}
		OnWorkableEventCB = (Action<WorkableEvent>)Delegate.Combine(OnWorkableEventCB, new Action<WorkableEvent>(OnWorkableEvent));
		operational = GetComponent<Operational>();
		storage = GetComponent<Storage>();
		UpdateWorkingState(null);
	}

	protected override void OnCleanUp()
	{
		Components.Telescopes.Remove(this);
		SpacecraftManager.instance.Unsubscribe(532901469, UpdateWorkingState);
		base.OnCleanUp();
	}

	public void Sim200ms(float dt)
	{
		Building component = GetComponent<Building>();
		Extents extents = component.GetExtents();
		int num = Mathf.Max(0, extents.x - clearScanCellRadius);
		int num2 = Mathf.Min(extents.x + clearScanCellRadius);
		int y = extents.y + extents.height - 3;
		int num3 = num2 - num + 1;
		int num4 = Grid.XYToCell(num, y);
		int num5 = Grid.XYToCell(num2, y);
		int num6 = 0;
		for (int i = num4; i <= num5; i++)
		{
			if (Grid.ExposedToSunlight[i] >= 253)
			{
				num6++;
			}
		}
		Operational component2 = GetComponent<Operational>();
		component2.SetFlag(visibleSkyFlag, num6 > 0);
		bool on = num6 < num3;
		KSelectable component3 = GetComponent<KSelectable>();
		if (num6 > 0)
		{
			component3.ToggleStatusItem(noVisibilityStatusItem, on: false);
			component3.ToggleStatusItem(reducedVisibilityStatusItem, on, this);
		}
		else
		{
			component3.ToggleStatusItem(noVisibilityStatusItem, on: true, this);
			component3.ToggleStatusItem(reducedVisibilityStatusItem, on: false);
		}
		percentClear = (float)num6 / (float)num3;
		if (!component2.IsActive && component2.IsOperational && chore == null)
		{
			chore = CreateChore();
			SetWorkTime(float.PositiveInfinity);
		}
	}

	private static string GetStatusItemString(string src_str, object data)
	{
		Telescope telescope = (Telescope)data;
		string text = src_str;
		text = text.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(telescope.percentClear * 100f));
		return text.Replace("{RADIUS}", telescope.clearScanCellRadius.ToString());
	}

	private void OnWorkableEvent(WorkableEvent ev)
	{
		Worker worker = base.worker;
		if (worker == null)
		{
			return;
		}
		OxygenBreather component = worker.GetComponent<OxygenBreather>();
		KPrefabID component2 = worker.GetComponent<KPrefabID>();
		switch (ev)
		{
		case WorkableEvent.WorkStarted:
			ShowProgressBar(show: true);
			progressBar.SetUpdateFunc(() => SpacecraftManager.instance.HasAnalysisTarget() ? (SpacecraftManager.instance.GetDestinationAnalysisScore(SpacecraftManager.instance.GetStarmapAnalysisDestinationID()) / (float)ROCKETRY.DESTINATION_ANALYSIS.COMPLETE) : 0f);
			workerGasProvider = component.GetGasProvider();
			component.SetGasProvider(this);
			component.GetComponent<CreatureSimTemperatureTransfer>().enabled = false;
			component2.AddTag(GameTags.Shaded);
			break;
		case WorkableEvent.WorkStopped:
			component.SetGasProvider(workerGasProvider);
			component.GetComponent<CreatureSimTemperatureTransfer>().enabled = true;
			ShowProgressBar(show: false);
			component2.RemoveTag(GameTags.Shaded);
			break;
		}
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (SpacecraftManager.instance.HasAnalysisTarget())
		{
			int starmapAnalysisDestinationID = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
			SpaceDestination destination = SpacecraftManager.instance.GetDestination(starmapAnalysisDestinationID);
			float num = 1f / (float)destination.OneBasedDistance;
			float num2 = ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED;
			float dEFAULT_CYCLES_PER_DISCOVERY = ROCKETRY.DESTINATION_ANALYSIS.DEFAULT_CYCLES_PER_DISCOVERY;
			float num3 = num2 / dEFAULT_CYCLES_PER_DISCOVERY;
			float num4 = num3 / 600f;
			float points = dt * num * num4;
			SpacecraftManager.instance.EarnDestinationAnalysisPoints(starmapAnalysisDestinationID, points);
		}
		return base.OnWorkTick(worker, dt);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		Element element = ElementLoader.FindElementByHash(SimHashes.Oxygen);
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(element.tag.ProperName(), string.Format(STRINGS.BUILDINGS.PREFABS.TELESCOPE.REQUIREMENT_TOOLTIP, element.tag.ProperName()), Descriptor.DescriptorType.Requirement);
		descriptors.Add(item);
		return descriptors;
	}

	protected Chore CreateChore()
	{
		WorkChore<Telescope> workChore = new WorkChore<Telescope>(Db.Get().ChoreTypes.Research, this);
		workChore.AddPrecondition(ContainsOxygen);
		return workChore;
	}

	protected void UpdateWorkingState(object data)
	{
		bool flag = false;
		if (SpacecraftManager.instance.HasAnalysisTarget() && SpacecraftManager.instance.GetDestinationAnalysisState(SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.GetStarmapAnalysisDestinationID())) != SpacecraftManager.DestinationAnalysisState.Complete)
		{
			flag = true;
		}
		KSelectable component = GetComponent<KSelectable>();
		bool on = !flag && !SpacecraftManager.instance.AreAllDestinationsAnalyzed();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.NoApplicableAnalysisSelected, on);
		operational.SetFlag(this.flag, flag);
		if (!flag && (bool)base.worker)
		{
			StopWork(base.worker, aborted: true);
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}

	public bool ShouldStoreCO2()
	{
		return false;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float amount)
	{
		if (storage.items.Count <= 0)
		{
			return false;
		}
		GameObject gameObject = storage.items[0];
		if (gameObject == null)
		{
			return false;
		}
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		bool result = component.Mass >= amount;
		component.Mass = Mathf.Max(0f, component.Mass - amount);
		return result;
	}
}
