using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

public class HandSanitizer : StateMachineComponent<HandSanitizer.SMInstance>, IGameObjectEffectDescriptor, IBasicBuilding
{
	private class WashHandsReactable : WorkableReactable
	{
		public WashHandsReactable(Workable workable, ChoreType chore_type, AllowedDirection allowed_direction = AllowedDirection.Any)
			: base(workable, "WashHands", chore_type, allowed_direction)
		{
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (base.InternalCanBegin(new_reactor, transition))
			{
				HandSanitizer component = workable.GetComponent<HandSanitizer>();
				if (!component.smi.IsReady())
				{
					return false;
				}
				bool flag = new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit);
				if (!component.canSanitizeSuit && flag)
				{
					return false;
				}
				if (component.alwaysUse)
				{
					return true;
				}
				PrimaryElement component2 = new_reactor.GetComponent<PrimaryElement>();
				if (component2 != null)
				{
					return component2.DiseaseIdx != byte.MaxValue;
				}
			}
			return false;
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, HandSanitizer, object>.GameInstance
	{
		public SMInstance(HandSanitizer master)
			: base(master)
		{
		}

		private bool HasSufficientMass()
		{
			bool result = false;
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(base.master.consumedElement);
			if (primaryElement != null)
			{
				result = primaryElement.Mass >= base.master.massConsumedPerUse;
			}
			return result;
		}

		public bool OutputFull()
		{
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(base.master.outputElement);
			if (primaryElement != null)
			{
				return primaryElement.Mass >= (float)base.master.maxUses * base.master.massConsumedPerUse;
			}
			return false;
		}

		public bool IsReady()
		{
			if (!HasSufficientMass())
			{
				return false;
			}
			if (OutputFull())
			{
				return false;
			}
			return true;
		}

		public void DumpOutput()
		{
			Storage component = base.master.GetComponent<Storage>();
			if (base.master.outputElement != SimHashes.Vacuum)
			{
				component.Drop(ElementLoader.FindElementByHash(base.master.outputElement).tag);
			}
		}
	}

	public class States : GameStateMachine<States, SMInstance, HandSanitizer>
	{
		public class ReadyStates : State
		{
			public State free;

			public State occupied;
		}

		public State notready;

		public ReadyStates ready;

		public State notoperational;

		public State full;

		public State empty;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = notready;
			root.Update(UpdateStatusItems);
			notoperational.PlayAnim("off").TagTransition(GameTags.Operational, notready);
			notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, ready, (SMInstance smi) => smi.IsReady()).TagTransition(GameTags.Operational, notoperational, on_remove: true);
			ready.DefaultState(ready.free).ToggleReactable((SMInstance smi) => smi.master.reactable = new WashHandsReactable(smi.master.GetComponent<Work>(), Db.Get().ChoreTypes.WashHands, smi.master.GetComponent<DirectionControl>().allowedDirection)).TagTransition(GameTags.Operational, notoperational, on_remove: true);
			ready.free.PlayAnim("on").WorkableStartTransition((SMInstance smi) => smi.GetComponent<Work>(), ready.occupied);
			ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", loop: true).Enter(delegate(SMInstance smi)
			{
				ConduitConsumer component2 = smi.GetComponent<ConduitConsumer>();
				if (component2 != null)
				{
					component2.enabled = false;
				}
			})
				.Exit(delegate(SMInstance smi)
				{
					ConduitConsumer component = smi.GetComponent<ConduitConsumer>();
					if (component != null)
					{
						component.enabled = true;
					}
				})
				.WorkableStopTransition((SMInstance smi) => smi.GetComponent<Work>(), notready);
		}

		private void UpdateStatusItems(SMInstance smi, float dt)
		{
			if (smi.OutputFull())
			{
				smi.master.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, this);
			}
			else
			{
				smi.master.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull);
			}
		}
	}

	[AddComponentMenu("KMonoBehaviour/Workable/Work")]
	public class Work : Workable, IGameObjectEffectDescriptor
	{
		public bool removeIrritation;

		private int diseaseRemoved;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			resetProgressOnStop = true;
			shouldTransferDiseaseWithWorker = false;
			GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater);
			});
		}

		protected override void OnStartWork(Worker worker)
		{
			base.OnStartWork(worker);
			diseaseRemoved = 0;
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			base.OnWorkTick(worker, dt);
			HandSanitizer component = GetComponent<HandSanitizer>();
			Storage component2 = GetComponent<Storage>();
			float massAvailable = component2.GetMassAvailable(component.consumedElement);
			if (massAvailable == 0f)
			{
				return true;
			}
			PrimaryElement component3 = worker.GetComponent<PrimaryElement>();
			float amount = Mathf.Min(component.massConsumedPerUse * dt / workTime, massAvailable);
			int num = Math.Min((int)(dt / workTime * (float)component.diseaseRemovalCount), component3.DiseaseCount);
			diseaseRemoved += num;
			SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
			invalid.idx = component3.DiseaseIdx;
			invalid.count = num;
			component3.ModifyDiseaseCount(-num, "HandSanitizer.OnWorkTick");
			component.maxPossiblyRemoved += num;
			if (component.canSanitizeStorage && (bool)worker.GetComponent<Storage>())
			{
				foreach (GameObject item in worker.GetComponent<Storage>().GetItems())
				{
					PrimaryElement component4 = item.GetComponent<PrimaryElement>();
					if ((bool)component4)
					{
						int num2 = Math.Min((int)(dt / workTime * (float)component.diseaseRemovalCount), component4.DiseaseCount);
						component4.ModifyDiseaseCount(-num2, "HandSanitizer.OnWorkTick");
						component.maxPossiblyRemoved += num2;
					}
				}
			}
			SimUtil.DiseaseInfo disease_info = SimUtil.DiseaseInfo.Invalid;
			component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component.consumedElement).tag, amount, out var amount_consumed, out disease_info, out var aggregate_temperature);
			if (component.outputElement != SimHashes.Vacuum)
			{
				disease_info = SimUtil.CalculateFinalDiseaseInfo(invalid, disease_info);
				component2.AddLiquid(component.outputElement, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count);
			}
			return false;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			base.OnCompleteWork(worker);
			if (removeIrritation && !worker.HasTag(GameTags.HasSuitTank))
			{
				worker.GetSMI<GasLiquidExposureMonitor.Instance>()?.ResetExposure();
			}
		}
	}

	public float massConsumedPerUse = 1f;

	public SimHashes consumedElement = SimHashes.BleachStone;

	public int diseaseRemovalCount = 10000;

	public int maxUses = 10;

	public SimHashes outputElement = SimHashes.Vacuum;

	public bool dumpWhenFull;

	public bool alwaysUse;

	public bool canSanitizeSuit;

	public bool canSanitizeStorage;

	private WorkableReactable reactable;

	private MeterController cleanMeter;

	private MeterController dirtyMeter;

	public Meter.Offset cleanMeterOffset;

	public Meter.Offset dirtyMeterOffset;

	[Serialize]
	public int maxPossiblyRemoved;

	private static readonly EventSystem.IntraObjectHandler<HandSanitizer> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<HandSanitizer>(delegate(HandSanitizer component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.FindOrAddComponent<Workable>();
	}

	private void RefreshMeters()
	{
		float positionPercent = 0f;
		PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(consumedElement);
		float num = (float)maxUses * massConsumedPerUse;
		ConduitConsumer component = GetComponent<ConduitConsumer>();
		if (component != null)
		{
			num = component.capacityKG;
		}
		if (primaryElement != null)
		{
			positionPercent = Mathf.Clamp01(primaryElement.Mass / num);
		}
		float positionPercent2 = 0f;
		PrimaryElement primaryElement2 = GetComponent<Storage>().FindPrimaryElement(outputElement);
		if (primaryElement2 != null)
		{
			positionPercent2 = Mathf.Clamp01(primaryElement2.Mass / ((float)maxUses * massConsumedPerUse));
		}
		cleanMeter.SetPositionPercent(positionPercent);
		dirtyMeter.SetPositionPercent(positionPercent2);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
		cleanMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", cleanMeterOffset, Grid.SceneLayer.NoLayer, "meter_clean_target");
		dirtyMeter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_dirty_target", "meter_dirty", dirtyMeterOffset, Grid.SceneLayer.NoLayer, "meter_dirty_target");
		RefreshMeters();
		Components.HandSanitizers.Add(this);
		Components.BasicBuildings.Add(this);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		DirectionControl component = GetComponent<DirectionControl>();
		component.onDirectionChanged = (Action<WorkableReactable.AllowedDirection>)Delegate.Combine(component.onDirectionChanged, new Action<WorkableReactable.AllowedDirection>(OnDirectionChanged));
		OnDirectionChanged(GetComponent<DirectionControl>().allowedDirection);
	}

	protected override void OnCleanUp()
	{
		Components.BasicBuildings.Remove(this);
		Components.HandSanitizers.Remove(this);
		base.OnCleanUp();
	}

	private void OnDirectionChanged(WorkableReactable.AllowedDirection allowed_direction)
	{
		if (reactable != null)
		{
			reactable.allowedDirection = allowed_direction;
		}
	}

	public List<Descriptor> RequirementDescriptors()
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, ElementLoader.FindElementByHash(consumedElement).name, GameUtil.GetFormattedMass(massConsumedPerUse)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, ElementLoader.FindElementByHash(consumedElement).name, GameUtil.GetFormattedMass(massConsumedPerUse)), Descriptor.DescriptorType.Requirement)
		};
	}

	public List<Descriptor> EffectDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		if (outputElement != SimHashes.Vacuum)
		{
			list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, ElementLoader.FindElementByHash(outputElement).name, GameUtil.GetFormattedMass(massConsumedPerUse))));
		}
		list.Add(new Descriptor(string.Format(UI.BUILDINGEFFECTS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(diseaseRemovalCount)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DISEASECONSUMEDPERUSE, GameUtil.GetFormattedDiseaseAmount(diseaseRemovalCount))));
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(RequirementDescriptors());
		list.AddRange(EffectDescriptors());
		return list;
	}

	private void OnStorageChange(object data)
	{
		if (dumpWhenFull && base.smi.OutputFull())
		{
			base.smi.DumpOutput();
		}
		RefreshMeters();
	}
}
