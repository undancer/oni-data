using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HotTub : StateMachineComponent<HotTub.StatesInstance>, IGameObjectEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, HotTub>
	{
		public class OffStates : State
		{
			public State draining;

			public FillingStates filling;

			public State too_hot;

			public State awaiting_delivery;
		}

		public class OnStates : State
		{
			public State pre;

			public State relaxing;

			public State relaxing_together;

			public State post;
		}

		public class ReadyStates : State
		{
			public State idle;

			public OnStates on;
		}

		public class FillingStates : State
		{
			public State normal;

			public State too_cold;
		}

		public IntParameter userCount;

		public BoolParameter waterTooCold;

		public State unoperational;

		public OffStates off;

		public ReadyStates ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = ready;
			root.Update(delegate(StatesInstance smi, float dt)
			{
				smi.SapHeatFromWater(dt);
				smi.TestWaterTemperature();
			}, UpdateRate.SIM_4000ms).EventHandler(GameHashes.OnStorageChange, delegate(StatesInstance smi)
			{
				smi.UpdateWaterMeter();
				smi.TestWaterTemperature();
			});
			unoperational.TagTransition(GameTags.Operational, off).PlayAnim("off");
			off.TagTransition(GameTags.Operational, unoperational, on_remove: true).DefaultState(off.filling);
			off.filling.DefaultState(off.filling.normal).Transition(ready, (StatesInstance smi) => smi.master.waterStorage.GetMassAvailable(SimHashes.Water) >= smi.master.hotTubCapacity).PlayAnim("off")
				.Enter(delegate(StatesInstance smi)
				{
					smi.GetComponent<ConduitConsumer>().SetOnState(onState: true);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.GetComponent<ConduitConsumer>().SetOnState(onState: false);
				})
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubFilling, (StatesInstance smi) => smi.master);
			off.filling.normal.ParamTransition(waterTooCold, off.filling.too_cold, GameStateMachine<States, StatesInstance, HotTub, object>.IsTrue);
			off.filling.too_cold.ParamTransition(waterTooCold, off.filling.normal, GameStateMachine<States, StatesInstance, HotTub, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.HotTubWaterTooCold, (StatesInstance smi) => smi.master);
			off.draining.Transition(off.filling, (StatesInstance smi) => smi.master.waterStorage.GetMassAvailable(SimHashes.Water) <= 0f).ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubWaterTooCold, (StatesInstance smi) => smi.master).PlayAnim("off")
				.Enter(delegate(StatesInstance smi)
				{
					smi.GetComponent<ConduitDispenser>().SetOnState(onState: true);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.GetComponent<ConduitDispenser>().SetOnState(onState: false);
				});
			off.too_hot.Transition(ready, (StatesInstance smi) => !smi.IsTubTooHot()).PlayAnim("overheated").ToggleMainStatusItem(Db.Get().BuildingStatusItems.HotTubTooHot, (StatesInstance smi) => smi.master);
			off.awaiting_delivery.EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.HasBleachStone());
			ready.DefaultState(ready.idle).Enter("CreateChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores();
			}).Exit("CancelChore", delegate(StatesInstance smi)
			{
				smi.master.UpdateChores(update: false);
			})
				.TagTransition(GameTags.Operational, unoperational, on_remove: true)
				.ParamTransition(waterTooCold, off.draining, GameStateMachine<States, StatesInstance, HotTub, object>.IsTrue)
				.EventTransition(GameHashes.OnStorageChange, off.awaiting_delivery, (StatesInstance smi) => !smi.HasBleachStone())
				.Transition(off.filling, (StatesInstance smi) => smi.master.waterStorage.IsEmpty())
				.Transition(off.too_hot, (StatesInstance smi) => smi.IsTubTooHot())
				.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
			ready.idle.PlayAnim("on").ParamTransition(userCount, ready.on.pre, (StatesInstance smi, int p) => p > 0);
			ready.on.Enter(delegate(StatesInstance smi)
			{
				smi.SetActive(active: true);
			}).Update(delegate(StatesInstance smi, float dt)
			{
				smi.ConsumeBleachstone(dt);
			}, UpdateRate.SIM_4000ms).Exit(delegate(StatesInstance smi)
			{
				smi.SetActive(active: false);
			});
			ready.on.pre.PlayAnim("working_pre").OnAnimQueueComplete(ready.on.relaxing);
			ready.on.relaxing.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition(userCount, ready.on.post, (StatesInstance smi, int p) => p == 0).ParamTransition(userCount, ready.on.relaxing_together, (StatesInstance smi, int p) => p > 1);
			ready.on.relaxing_together.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition(userCount, ready.on.post, (StatesInstance smi, int p) => p == 0).ParamTransition(userCount, ready.on.relaxing, (StatesInstance smi, int p) => p == 1);
			ready.on.post.PlayAnim("working_pst").OnAnimQueueComplete(ready.idle);
		}

		private string GetRelaxingAnim(StatesInstance smi)
		{
			bool flag = smi.master.occupants.Contains(0);
			bool flag2 = smi.master.occupants.Contains(1);
			if (flag && !flag2)
			{
				return "working_loop_one_p";
			}
			if (flag2 && !flag)
			{
				return "working_loop_two_p";
			}
			return "working_loop_coop_p";
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, HotTub, object>.GameInstance
	{
		private Operational operational;

		public StatesInstance(HotTub smi)
			: base(smi)
		{
			operational = base.master.GetComponent<Operational>();
		}

		public void SetActive(bool active)
		{
			operational.SetActive(operational.IsOperational && active);
		}

		public void UpdateWaterMeter()
		{
			base.smi.master.waterMeter.SetPositionPercent(Mathf.Clamp(base.smi.master.waterStorage.GetMassAvailable(SimHashes.Water) / base.smi.master.hotTubCapacity, 0f, 1f));
		}

		public void UpdateTemperatureMeter(float waterTemp)
		{
			Element element = ElementLoader.GetElement(SimHashes.Water.CreateTag());
			base.smi.master.tempMeter.SetPositionPercent(Mathf.Clamp((waterTemp - base.smi.master.minimumWaterTemperature) / (element.highTemp - base.smi.master.minimumWaterTemperature), 0f, 1f));
		}

		public void TestWaterTemperature()
		{
			Storage waterStorage = base.smi.master.waterStorage;
			GameObject gameObject = waterStorage.FindFirst(new Tag(1836671383));
			float waterTemp = 0f;
			if ((bool)gameObject)
			{
				waterTemp = gameObject.GetComponent<PrimaryElement>().Temperature;
				UpdateTemperatureMeter(waterTemp);
				if (waterTemp < base.smi.master.minimumWaterTemperature)
				{
					base.smi.sm.waterTooCold.Set(value: true, base.smi);
				}
				else
				{
					base.smi.sm.waterTooCold.Set(value: false, base.smi);
				}
			}
			else
			{
				UpdateTemperatureMeter(waterTemp);
				base.smi.sm.waterTooCold.Set(value: false, base.smi);
			}
		}

		public bool IsTubTooHot()
		{
			return base.smi.master.GetComponent<PrimaryElement>().Temperature > base.smi.master.maxOperatingTemperature;
		}

		public bool HasBleachStone()
		{
			Storage waterStorage = base.smi.master.waterStorage;
			GameObject gameObject = waterStorage.FindFirst(new Tag(-839728230));
			return gameObject != null && gameObject.GetComponent<PrimaryElement>().Mass > 0f;
		}

		public void SapHeatFromWater(float dt)
		{
			float num = base.smi.master.waterCoolingRate * dt / (float)base.smi.master.waterStorage.items.Count;
			foreach (GameObject item in base.smi.master.waterStorage.items)
			{
				GameUtil.DeltaThermalEnergy(item.GetComponent<PrimaryElement>(), 0f - num, base.smi.master.minimumWaterTemperature);
				GameUtil.DeltaThermalEnergy(GetComponent<PrimaryElement>(), num, GetComponent<PrimaryElement>().Element.highTemp);
			}
		}

		public void ConsumeBleachstone(float dt)
		{
			base.smi.master.waterStorage.ConsumeIgnoringDisease(new Tag(-839728230), base.smi.master.bleachStoneConsumption * dt);
		}
	}

	public string specificEffect;

	public string trackingEffect;

	public int basePriority;

	public CellOffset[] choreOffsets = new CellOffset[4]
	{
		new CellOffset(-1, 0),
		new CellOffset(1, 0),
		new CellOffset(0, 0),
		new CellOffset(2, 0)
	};

	private HotTubWorkable[] workables;

	private Chore[] chores;

	public HashSet<int> occupants = new HashSet<int>();

	public float waterCoolingRate;

	public float hotTubCapacity = 100f;

	public float minimumWaterTemperature = 0f;

	public float bleachStoneConsumption = 0f;

	public float maxOperatingTemperature = 0f;

	[MyCmpGet]
	public Storage waterStorage;

	private MeterController waterMeter;

	private MeterController tempMeter;

	public float PercentFull => 100f * waterStorage.GetMassAvailable(SimHashes.Water) / hotTubCapacity;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		});
		workables = new HotTubWorkable[choreOffsets.Length];
		chores = new Chore[choreOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), choreOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("HotTubWorkable", pos);
			KSelectable kSelectable = go.AddOrGet<KSelectable>();
			kSelectable.SetName(this.GetProperName());
			kSelectable.IsSelectable = false;
			HotTubWorkable hotTubWorkable = go.AddOrGet<HotTubWorkable>();
			int player_index = i;
			hotTubWorkable.OnWorkableEventCB = (Action<Workable.WorkableEvent>)Delegate.Combine(hotTubWorkable.OnWorkableEventCB, (Action<Workable.WorkableEvent>)delegate(Workable.WorkableEvent ev)
			{
				OnWorkableEvent(player_index, ev);
			});
			workables[i] = hotTubWorkable;
			workables[i].hotTub = this;
		}
		waterMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_water_target", "meter_water", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_water_target");
		base.smi.UpdateWaterMeter();
		tempMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_temperature_target", "meter_temp", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_temperature_target");
		base.smi.TestWaterTemperature();
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		UpdateChores(update: false);
		for (int i = 0; i < workables.Length; i++)
		{
			if ((bool)workables[i])
			{
				Util.KDestroyGameObject(workables[i]);
				workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	private Chore CreateChore(int i)
	{
		Workable workable = workables[i];
		Chore chore = new WorkChore<HotTubWorkable>(Db.Get().ChoreTypes.Relax, workable, null, run_until_complete: true, null, null, schedule_block: Db.Get().ScheduleBlockTypes.Recreation, on_end: OnSocialChoreEnd, allow_in_red_alert: false, ignore_schedule_block: false, only_when_operational: true, override_anims: null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
		chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, workable);
		return chore;
	}

	private void OnSocialChoreEnd(Chore chore)
	{
		if (base.gameObject.HasTag(GameTags.Operational))
		{
			UpdateChores();
		}
	}

	public void UpdateChores(bool update = true)
	{
		for (int i = 0; i < choreOffsets.Length; i++)
		{
			Chore chore = chores[i];
			if (update)
			{
				if (chore?.isComplete ?? true)
				{
					chores[i] = CreateChore(i);
				}
			}
			else if (chore != null)
			{
				chore.Cancel("locator invalidated");
				chores[i] = null;
			}
		}
	}

	public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
	{
		if (ev == Workable.WorkableEvent.WorkStarted)
		{
			occupants.Add(player);
		}
		else
		{
			occupants.Remove(player);
		}
		base.smi.sm.userCount.Set(occupants.Count, base.smi);
	}

	List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(SimHashes.Water);
		list.Add(new Descriptor(BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT.Replace("{element}", element.name).Replace("{amount}", GameUtil.GetFormattedMass(hotTubCapacity)), BUILDINGS.PREFABS.HOTTUB.WATER_REQUIREMENT_TOOLTIP.Replace("{element}", element.name).Replace("{amount}", GameUtil.GetFormattedMass(hotTubCapacity)), Descriptor.DescriptorType.Requirement));
		list.Add(new Descriptor(BUILDINGS.PREFABS.HOTTUB.TEMPERATURE_REQUIREMENT.Replace("{element}", element.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(minimumWaterTemperature)), BUILDINGS.PREFABS.HOTTUB.TEMPERATURE_REQUIREMENT_TOOLTIP.Replace("{element}", element.name).Replace("{temperature}", GameUtil.GetFormattedTemperature(minimumWaterTemperature)), Descriptor.DescriptorType.Requirement));
		list.Add(new Descriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION));
		Effect.AddModifierDescriptions(base.gameObject, list, specificEffect, increase_indent: true);
		return list;
	}
}
