using System.Collections.Generic;
using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Shower")]
public class Shower : Workable, IGameObjectEffectDescriptor
{
	public class ShowerSM : GameStateMachine<ShowerSM, ShowerSM.Instance, Shower>
	{
		public class OperationalState : State
		{
			public State not_ready;

			public State ready;
		}

		public new class Instance : GameInstance
		{
			private Operational operational;

			private ConduitConsumer consumer;

			private ConduitDispenser dispenser;

			public bool IsOperational
			{
				get
				{
					if (operational.IsOperational && consumer.IsConnected)
					{
						return dispenser.IsConnected;
					}
					return false;
				}
			}

			public Instance(Shower master)
				: base(master)
			{
				operational = master.GetComponent<Operational>();
				consumer = master.GetComponent<ConduitConsumer>();
				dispenser = master.GetComponent<ConduitDispenser>();
			}

			public void SetActive(bool active)
			{
				operational.SetActive(active);
			}

			private bool HasSufficientMass()
			{
				bool result = false;
				PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(SimHashes.Water);
				if (primaryElement != null)
				{
					result = primaryElement.Mass >= 5f;
				}
				return result;
			}

			public bool OutputFull()
			{
				PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(SimHashes.DirtyWater);
				if (primaryElement != null)
				{
					return primaryElement.Mass >= 5f;
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
		}

		public State unoperational;

		public OperationalState operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			root.Update(UpdateStatusItems);
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.IsOperational).PlayAnim("off");
			operational.DefaultState(operational.not_ready).EventTransition(GameHashes.OperationalChanged, unoperational, (Instance smi) => !smi.IsOperational);
			operational.not_ready.EventTransition(GameHashes.OnStorageChange, operational.ready, (Instance smi) => smi.IsReady()).PlayAnim("off");
			operational.ready.ToggleChore(CreateShowerChore, operational.not_ready);
		}

		private Chore CreateShowerChore(Instance smi)
		{
			return new WorkChore<Shower>(Db.Get().ChoreTypes.Shower, smi.master, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, Db.Get().ScheduleBlockTypes.Hygiene, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
		}

		private void UpdateStatusItems(Instance smi, float dt)
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

	private ShowerSM.Instance smi;

	public static string SHOWER_EFFECT = "Showered";

	public SimHashes outputTargetElement;

	public float fractionalDiseaseRemoval;

	public int absoluteDiseaseRemoval;

	private SimUtil.DiseaseInfo accumulatedDisease;

	public const float WATER_PER_USE = 5f;

	private static readonly string[] EffectsRemoved = new string[4] { "SoakingWet", "WetFeet", "MinorIrritation", "MajorIrritation" };

	private Shower()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		resetProgressOnStop = true;
		smi = new ShowerSM.Instance(this);
		smi.StartSM();
	}

	protected override void OnStartWork(Worker worker)
	{
		HygieneMonitor.Instance sMI = worker.GetSMI<HygieneMonitor.Instance>();
		base.WorkTimeRemaining = workTime * sMI.GetDirtiness();
		accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
		smi.SetActive(active: true);
		base.OnStartWork(worker);
	}

	protected override void OnStopWork(Worker worker)
	{
		smi.SetActive(active: false);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < EffectsRemoved.Length; i++)
		{
			string effect_id = EffectsRemoved[i];
			component.Remove(effect_id);
		}
		if (!worker.HasTag(GameTags.HasSuitTank))
		{
			worker.GetSMI<GasLiquidExposureMonitor.Instance>()?.ResetExposure();
		}
		component.Add(SHOWER_EFFECT, should_save: true);
		worker.GetSMI<HygieneMonitor.Instance>()?.SetDirtiness(0f);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		PrimaryElement component = worker.GetComponent<PrimaryElement>();
		if (component.DiseaseCount > 0)
		{
			SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
			diseaseInfo.idx = component.DiseaseIdx;
			diseaseInfo.count = Mathf.CeilToInt((float)component.DiseaseCount * (1f - Mathf.Pow(fractionalDiseaseRemoval, dt)) - (float)absoluteDiseaseRemoval);
			SimUtil.DiseaseInfo b = diseaseInfo;
			component.ModifyDiseaseCount(-b.count, "Shower.RemoveDisease");
			accumulatedDisease = SimUtil.CalculateFinalDiseaseInfo(accumulatedDisease, b);
			PrimaryElement primaryElement = GetComponent<Storage>().FindPrimaryElement(outputTargetElement);
			if (primaryElement != null)
			{
				primaryElement.GetComponent<PrimaryElement>().AddDisease(accumulatedDisease.idx, accumulatedDisease.count, "Shower.RemoveDisease");
				accumulatedDisease = SimUtil.DiseaseInfo.Invalid;
			}
		}
		return false;
	}

	protected override void OnAbortWork(Worker worker)
	{
		base.OnAbortWork(worker);
		worker.GetSMI<HygieneMonitor.Instance>()?.SetDirtiness(1f - GetPercentComplete());
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (EffectsRemoved.Length != 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.REMOVESEFFECTSUBTITLE, UI.BUILDINGEFFECTS.TOOLTIPS.REMOVESEFFECTSUBTITLE);
			descriptors.Add(item);
			for (int i = 0; i < EffectsRemoved.Length; i++)
			{
				string text = EffectsRemoved[i];
				string arg = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".NAME");
				string arg2 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text.ToUpper() + ".CAUSE");
				Descriptor item2 = default(Descriptor);
				item2.IncreaseIndent();
				item2.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.REMOVEDEFFECT, arg), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.REMOVEDEFFECT, arg2));
				descriptors.Add(item2);
			}
		}
		Effect.AddModifierDescriptions(base.gameObject, descriptors, SHOWER_EFFECT, increase_indent: true);
		return descriptors;
	}
}
