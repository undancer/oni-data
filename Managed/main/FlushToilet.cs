using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FlushToilet : StateMachineComponent<FlushToilet.SMInstance>, IUsable, IGameObjectEffectDescriptor, IBasicBuilding
{
	public class SMInstance : GameStateMachine<States, SMInstance, FlushToilet, object>.GameInstance
	{
		public List<Chore> activeUseChores;

		public SMInstance(FlushToilet master)
			: base(master)
		{
			activeUseChores = new List<Chore>();
			UpdateFullnessState();
			UpdateDirtyState();
		}

		public bool HasValidConnections()
		{
			return Game.Instance.liquidConduitFlow.HasConduit(base.master.inputCell) && Game.Instance.liquidConduitFlow.HasConduit(base.master.outputCell);
		}

		public bool UpdateFullnessState()
		{
			float num = 0f;
			ListPool<GameObject, FlushToilet>.PooledList pooledList = ListPool<GameObject, FlushToilet>.Allocate();
			base.master.storage.Find(WaterTag, pooledList);
			foreach (GameObject item in pooledList)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				num += component.Mass;
			}
			pooledList.Recycle();
			bool flag = num >= base.master.massConsumedPerUse;
			base.master.conduitConsumer.enabled = !flag;
			float positionPercent = Mathf.Clamp01(num / base.master.massConsumedPerUse);
			base.master.fillMeter.SetPositionPercent(positionPercent);
			return flag;
		}

		public void UpdateDirtyState()
		{
			ToiletWorkableUse component = GetComponent<ToiletWorkableUse>();
			float percentComplete = component.GetPercentComplete();
			base.master.contaminationMeter.SetPositionPercent(percentComplete);
		}

		public void Flush()
		{
			base.master.fillMeter.SetPositionPercent(0f);
			base.master.contaminationMeter.SetPositionPercent(1f);
			base.smi.ShowFillMeter();
			Worker worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}

		public void ShowFillMeter()
		{
			base.master.fillMeter.gameObject.SetActive(value: true);
			base.master.contaminationMeter.gameObject.SetActive(value: false);
		}

		public bool HasContaminatedMass()
		{
			foreach (GameObject item in GetComponent<Storage>().items)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component == null || component.ElementID != SimHashes.DirtyWater || !(component.Mass > 0f))
				{
					continue;
				}
				return true;
			}
			return false;
		}

		public void ShowContaminatedMeter()
		{
			base.master.fillMeter.gameObject.SetActive(value: false);
			base.master.contaminationMeter.gameObject.SetActive(value: true);
		}
	}

	public class States : GameStateMachine<States, SMInstance, FlushToilet>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State inuse;
		}

		public State disconnected;

		public State backedup;

		public ReadyStates ready;

		public State fillingInactive;

		public State filling;

		public State flushing;

		public State flushed;

		public BoolParameter outputBlocked;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disconnected;
			disconnected.PlayAnim("off").EventTransition(GameHashes.ConduitConnectionChanged, backedup, (SMInstance smi) => smi.HasValidConnections()).Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			});
			backedup.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull).EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, fillingInactive, GameStateMachine<States, SMInstance, FlushToilet, object>.IsFalse)
				.Enter(delegate(SMInstance smi)
				{
					smi.GetComponent<Operational>().SetActive(value: false);
				});
			filling.PlayAnim("off").Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true);
			}).EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue)
				.EventTransition(GameHashes.OnStorageChange, ready, (SMInstance smi) => smi.UpdateFullnessState())
				.EventTransition(GameHashes.OperationalChanged, fillingInactive, (SMInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			fillingInactive.PlayAnim("off").Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			}).EventTransition(GameHashes.OperationalChanged, filling, (SMInstance smi) => smi.GetComponent<Operational>().IsOperational)
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue);
			ready.DefaultState(ready.idle).ToggleTag(GameTags.Usable).Enter(delegate(SMInstance smi)
			{
				smi.master.fillMeter.SetPositionPercent(1f);
				smi.master.contaminationMeter.SetPositionPercent(0f);
			})
				.PlayAnim("off")
				.EventTransition(GameHashes.ConduitConnectionChanged, disconnected, (SMInstance smi) => !smi.HasValidConnections())
				.ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue)
				.ToggleChore(CreateUrgentUseChore, flushing)
				.ToggleChore(CreateBreakUseChore, flushing);
			ready.idle.Enter(delegate(SMInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToilet).WorkableStartTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), ready.inuse);
			ready.inuse.Enter(delegate(SMInstance smi)
			{
				smi.ShowContaminatedMeter();
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToiletInUse).Update(delegate(SMInstance smi, float dt)
			{
				smi.UpdateDirtyState();
			})
				.WorkableCompleteTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), flushing)
				.WorkableStopTransition((SMInstance smi) => smi.master.GetComponent<ToiletWorkableUse>(), flushed);
			flushing.Enter(delegate(SMInstance smi)
			{
				smi.Flush();
			}).GoTo(flushed);
			flushed.EventTransition(GameHashes.OnStorageChange, fillingInactive, (SMInstance smi) => !smi.HasContaminatedMass()).ParamTransition(outputBlocked, backedup, GameStateMachine<States, SMInstance, FlushToilet, object>.IsTrue);
		}

		private Chore CreateUrgentUseChore(SMInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderFull);
			chore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing);
			return chore;
		}

		private Chore CreateBreakUseChore(SMInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull);
			return chore;
		}

		private Chore CreateUseChore(SMInstance smi, ChoreType choreType)
		{
			WorkChore<ToiletWorkableUse> workChore = new WorkChore<ToiletWorkableUse>(choreType, smi.master, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: true, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds, 5, ignore_building_assignment: false, add_to_daily_report: false);
			smi.activeUseChores.Add(workChore);
			workChore.onExit = (Action<Chore>)Delegate.Combine(workChore.onExit, (Action<Chore>)delegate(Chore exiting_chore)
			{
				smi.activeUseChores.Remove(exiting_chore);
			});
			workChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, smi.master.GetComponent<Assignable>());
			workChore.AddPrecondition(ChorePreconditions.instance.IsExclusivelyAvailableWithOtherChores, smi.activeUseChores);
			return workChore;
		}
	}

	private MeterController fillMeter;

	private MeterController contaminationMeter;

	[SerializeField]
	public float massConsumedPerUse = 5f;

	[SerializeField]
	public float massEmittedPerUse = 5f;

	[SerializeField]
	public float newPeeTemperature;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	[MyCmpGet]
	private ConduitConsumer conduitConsumer;

	[MyCmpGet]
	private Storage storage;

	public static readonly Tag WaterTag = GameTagExtensions.Create(SimHashes.Water);

	private int inputCell;

	private int outputCell;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = GetComponent<Building>();
		inputCell = component.GetUtilityInputCell();
		outputCell = component.GetUtilityOutputCell();
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt += OnConduitsRebuilt;
		liquidConduitFlow.AddConduitUpdater(OnConduitUpdate);
		KBatchedAnimController component2 = GetComponent<KBatchedAnimController>();
		fillMeter = new MeterController(component2, "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f));
		contaminationMeter = new MeterController(component2, "meter_target", "meter_dirty", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f));
		Components.Toilets.Add(this);
		Components.BasicBuildings.Add(this);
		base.smi.StartSM();
		base.smi.ShowFillMeter();
	}

	protected override void OnCleanUp()
	{
		ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
		liquidConduitFlow.onConduitsRebuilt -= OnConduitsRebuilt;
		Components.BasicBuildings.Remove(this);
		Components.Toilets.Remove(this);
		base.OnCleanUp();
	}

	private void OnConduitsRebuilt()
	{
		Trigger(-2094018600);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	private void Flush(Worker worker)
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		storage.Find(WaterTag, pooledList);
		float num = 0f;
		float num2 = massConsumedPerUse;
		foreach (GameObject item in pooledList)
		{
			PrimaryElement component = item.GetComponent<PrimaryElement>();
			float num3 = Mathf.Min(component.Mass, num2);
			component.Mass -= num3;
			num2 -= num3;
			num += num3 * component.Temperature;
		}
		pooledList.Recycle();
		float num4 = massEmittedPerUse - massConsumedPerUse;
		num += num4 * newPeeTemperature;
		float temperature = num / massEmittedPerUse;
		byte index = Db.Get().Diseases.GetIndex(diseaseId);
		storage.AddLiquid(SimHashes.DirtyWater, massEmittedPerUse, temperature, index, diseasePerFlush);
		if (worker != null)
		{
			PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
			component2.AddDisease(index, diseaseOnDupePerFlush, "FlushToilet.Flush");
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[index].Name, diseasePerFlush + diseaseOnDupePerFlush), base.transform, Vector3.up);
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
		}
		else
		{
			DebugUtil.LogWarningArgs("Tried to add disease on toilet use but worker was null");
		}
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Water);
		string arg = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(massConsumedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement));
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.DirtyWater);
		string arg = element.tag.ProperName();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_TOILET, arg, GameUtil.GetFormattedMass(massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(newPeeTemperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_TOILET, arg, GameUtil.GetFormattedMass(massEmittedPerUse, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(newPeeTemperature))));
		Disease disease = Db.Get().Diseases.Get(diseaseId);
		int units = diseasePerFlush + diseaseOnDupePerFlush;
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, disease.Name, GameUtil.GetFormattedDiseaseAmount(units)), Descriptor.DescriptorType.DiseaseSource));
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(RequirementDescriptors());
		list.AddRange(EffectDescriptors());
		return list;
	}

	private void OnConduitUpdate(float dt)
	{
		if (GetSMI() != null)
		{
			ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
			bool value = liquidConduitFlow.GetContents(outputCell).mass > 0f && base.smi.HasContaminatedMass();
			base.smi.sm.outputBlocked.Set(value, base.smi);
		}
	}
}
