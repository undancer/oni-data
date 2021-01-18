using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class Toilet : StateMachineComponent<Toilet.StatesInstance>, ISaveLoadable, IUsable, IGameObjectEffectDescriptor, IBasicBuilding
{
	[Serializable]
	public struct SpawnInfo
	{
		[HashedEnum]
		public SimHashes elementID;

		public float mass;

		public float interval;

		public SpawnInfo(SimHashes element_id, float mass, float interval)
		{
			elementID = element_id;
			this.mass = mass;
			this.interval = interval;
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Toilet, object>.GameInstance
	{
		public Chore cleanChore;

		public List<Chore> activeUseChores;

		public float monsterSpawnTime = 1200f;

		public bool IsSoiled => base.master.FlushesUsed > 0;

		public StatesInstance(Toilet master)
			: base(master)
		{
			activeUseChores = new List<Chore>();
		}

		public int GetFlushesRemaining()
		{
			return base.master.maxFlushes - base.master.FlushesUsed;
		}

		public bool RequiresDirtDelivery()
		{
			if (base.master.storage.IsEmpty())
			{
				return true;
			}
			Tag tag = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
			if (!base.master.storage.Has(tag))
			{
				return true;
			}
			float amountAvailable = base.master.storage.GetAmountAvailable(tag);
			if (!(amountAvailable >= base.master.manualdeliverykg.capacity) && !IsSoiled)
			{
				return true;
			}
			return false;
		}

		public float MassPerFlush()
		{
			return base.master.solidWastePerUse.mass;
		}

		public float DirtUsedPerFlush()
		{
			return base.master.dirtUsedPerFlush;
		}

		public bool IsToxicSandRemoved()
		{
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			return base.master.storage.FindFirst(tag) == null;
		}

		public void CreateCleanChore()
		{
			if (cleanChore != null)
			{
				cleanChore.Cancel("dupe");
			}
			ToiletWorkableClean component = base.master.GetComponent<ToiletWorkableClean>();
			cleanChore = new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, component, null, run_until_complete: true, OnCleanComplete, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
		}

		public void CancelCleanChore()
		{
			if (cleanChore != null)
			{
				cleanChore.Cancel("Cancelled");
				cleanChore = null;
			}
		}

		private void DropFromStorage(Tag tag)
		{
			ListPool<GameObject, Toilet>.PooledList pooledList = ListPool<GameObject, Toilet>.Allocate();
			base.master.storage.Find(tag, pooledList);
			foreach (GameObject item in pooledList)
			{
				base.master.storage.Drop(item);
			}
			pooledList.Recycle();
		}

		private void OnCleanComplete(Chore chore)
		{
			cleanChore = null;
			Tag tag = GameTagExtensions.Create(base.master.solidWastePerUse.elementID);
			Tag tag2 = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
			DropFromStorage(tag);
			DropFromStorage(tag2);
			base.master.meter.SetPositionPercent((float)base.master.FlushesUsed / (float)base.master.maxFlushes);
		}

		public void Flush()
		{
			Worker worker = base.master.GetComponent<ToiletWorkableUse>().worker;
			base.master.Flush(worker);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Toilet>
	{
		public class ReadyStates : State
		{
			public State idle;

			public State inuse;

			public State flush;
		}

		public State needsdirt;

		public State empty;

		public State notoperational;

		public State ready;

		public State earlyclean;

		public State earlyWaitingForClean;

		public State full;

		public State fullWaitingForClean;

		private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
		{
			"full_pre",
			"full"
		};

		public IntParameter flushes = new IntParameter(0);

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = needsdirt;
			root.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, needsdirt, (StatesInstance smi) => smi.RequiresDirtDelivery()).EventTransition(GameHashes.OperationalChanged, notoperational, (StatesInstance smi) => !smi.Get<Operational>().IsOperational);
			needsdirt.Enter(delegate(StatesInstance smi)
			{
				if (smi.RequiresDirtDelivery())
				{
					smi.master.manualdeliverykg.RequestDelivery();
				}
			}).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => !smi.RequiresDirtDelivery());
			ready.ParamTransition(flushes, full, (StatesInstance smi, int p) => smi.GetFlushesRemaining() <= 0).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Toilet).ToggleRecurringChore(CreateUrgentUseChore)
				.ToggleRecurringChore(CreateBreakUseChore)
				.ToggleTag(GameTags.Usable)
				.EventHandler(GameHashes.Flush, delegate(StatesInstance smi, object data)
				{
					smi.Flush();
				});
			earlyclean.PlayAnims((StatesInstance smi) => FULL_ANIMS).OnAnimQueueComplete(earlyWaitingForClean);
			earlyWaitingForClean.Enter(delegate(StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsToxicSandRemoved());
			full.PlayAnims((StatesInstance smi) => FULL_ANIMS).OnAnimQueueComplete(fullWaitingForClean);
			fullWaitingForClean.Enter(delegate(StatesInstance smi)
			{
				smi.CreateCleanChore();
			}).Exit(delegate(StatesInstance smi)
			{
				smi.CancelCleanChore();
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying)
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable)
				.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => smi.IsToxicSandRemoved())
				.Enter(delegate(StatesInstance smi)
				{
					smi.Schedule(smi.monsterSpawnTime, delegate
					{
						smi.master.SpawnMonster();
					});
				});
			empty.PlayAnim("off").Enter("ClearFlushes", delegate(StatesInstance smi)
			{
				smi.master.FlushesUsed = 0;
			}).GoTo(needsdirt);
			notoperational.EventTransition(GameHashes.OperationalChanged, needsdirt, (StatesInstance smi) => smi.Get<Operational>().IsOperational).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable);
		}

		private Chore CreateUrgentUseChore(StatesInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderFull);
			chore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing);
			return chore;
		}

		private Chore CreateBreakUseChore(StatesInstance smi)
		{
			Chore chore = CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
			chore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull);
			chore.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Hygiene);
			return chore;
		}

		private Chore CreateUseChore(StatesInstance smi, ChoreType choreType)
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

	[SerializeField]
	public SpawnInfo solidWastePerUse;

	[SerializeField]
	public float solidWasteTemperature;

	[SerializeField]
	public SpawnInfo gasWasteWhenFull;

	[SerializeField]
	public int maxFlushes = 15;

	[SerializeField]
	public string diseaseId;

	[SerializeField]
	public int diseasePerFlush;

	[SerializeField]
	public int diseaseOnDupePerFlush;

	[SerializeField]
	public float dirtUsedPerFlush = 13f;

	[Serialize]
	public int _flushesUsed = 0;

	private MeterController meter;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private ManualDeliveryKG manualdeliverykg;

	private static readonly EventSystem.IntraObjectHandler<Toilet> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Toilet>(delegate(Toilet component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public int FlushesUsed
	{
		get
		{
			return _flushesUsed;
		}
		set
		{
			_flushesUsed = value;
			base.smi.sm.flushes.Set(value, base.smi);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Toilets.Add(this);
		Components.BasicBuildings.Add(this);
		base.smi.StartSM();
		ToiletWorkableUse component = GetComponent<ToiletWorkableUse>();
		component.trackUses = true;
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
		meter.SetPositionPercent((float)FlushesUsed / (float)maxFlushes);
		FlushesUsed = _flushesUsed;
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.BasicBuildings.Remove(this);
		Components.Toilets.Remove(this);
	}

	public bool IsUsable()
	{
		return base.smi.HasTag(GameTags.Usable);
	}

	public void Flush(Worker worker)
	{
		FlushesUsed++;
		meter.SetPositionPercent((float)FlushesUsed / (float)maxFlushes);
		float aggregate_temperature = 0f;
		Tag tag = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
		storage.ConsumeAndGetDisease(tag, base.smi.DirtUsedPerFlush(), out var amount_consumed, out var disease_info, out aggregate_temperature);
		byte index = Db.Get().Diseases.GetIndex(diseaseId);
		float mass = base.smi.MassPerFlush() + amount_consumed;
		Element element = ElementLoader.FindElementByHash(solidWastePerUse.elementID);
		GameObject gameObject = element.substance.SpawnResource(base.transform.GetPosition(), mass, solidWasteTemperature, index, diseasePerFlush, prevent_merge: true);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.AddDisease(disease_info.idx, disease_info.count, "Toilet.Flush");
		storage.Store(gameObject);
		PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
		component2.AddDisease(index, diseaseOnDupePerFlush, "Toilet.Flush");
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format(DUPLICANTS.DISEASES.ADDED_POPFX, Db.Get().Diseases[index].Name, diseasePerFlush + diseaseOnDupePerFlush), base.transform, Vector3.up);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (base.smi.GetCurrentState() != base.smi.sm.full && base.smi.IsSoiled && base.smi.cleanChore == null)
		{
			Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("status_item_toilet_needs_emptying", UI.USERMENUACTIONS.CLEANTOILET.NAME, delegate
			{
				base.smi.GoTo(base.smi.sm.earlyclean);
			}, Action.NumActions, null, null, null, UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP));
		}
	}

	private void SpawnMonster()
	{
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), base.smi.transform.GetPosition(), Grid.SceneLayer.Creatures);
		gameObject.SetActive(value: true);
	}

	public List<Descriptor> RequirementDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
		string arg = component.requestedItemTag.ProperName();
		float mass = base.smi.DirtUsedPerFlush();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		list.Add(item);
		return list;
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(solidWastePerUse.elementID);
		string arg = element.tag.ProperName();
		float mass = base.smi.MassPerFlush() + base.smi.DirtUsedPerFlush();
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTED_TOILET, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(solidWasteTemperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_TOILET, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(solidWasteTemperature))));
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
}
