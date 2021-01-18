using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Clinic")]
public class Clinic : Workable, IGameObjectEffectDescriptor, ISingleSliderControl, ISliderControl
{
	public class ClinicSM : GameStateMachine<ClinicSM, ClinicSM.Instance, Clinic>
	{
		public class OperationalStates : State
		{
			public State idle;

			public HealingStates healing;
		}

		public class HealingStates : State
		{
			public State undoctored;

			public State doctored;

			public State newlyDoctored;
		}

		public new class Instance : GameInstance
		{
			private WorkChore<DoctorChoreWorkable> doctorChore;

			public Instance(Clinic master)
				: base(master)
			{
			}

			public void StartDoctorChore()
			{
				if (base.master.IsValidEffect(base.master.doctoredHealthEffect) || base.master.IsValidEffect(base.master.doctoredDiseaseEffect))
				{
					doctorChore = new WorkChore<DoctorChoreWorkable>(Db.Get().ChoreTypes.Doctor, base.smi.master, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: true, PriorityScreen.PriorityClass.basic, 5, ignore_building_assignment: true);
					WorkChore<DoctorChoreWorkable> workChore = doctorChore;
					workChore.onComplete = (Action<Chore>)Delegate.Combine(workChore.onComplete, (Action<Chore>)delegate
					{
						base.smi.GoTo(base.smi.sm.operational.healing.newlyDoctored);
					});
				}
			}

			public void StopDoctorChore()
			{
				if (doctorChore != null)
				{
					doctorChore.Cancel("StopDoctorChore");
					doctorChore = null;
				}
			}

			public bool HasEffect(string effect)
			{
				bool result = false;
				if (base.master.IsValidEffect(effect))
				{
					result = base.smi.master.worker.GetComponent<Effects>().HasEffect(effect);
				}
				return result;
			}

			public EffectInstance StartEffect(string effect, bool should_save)
			{
				if (base.master.IsValidEffect(effect))
				{
					Worker worker = base.smi.master.worker;
					if (worker != null)
					{
						Effects component = worker.GetComponent<Effects>();
						if (!component.HasEffect(effect))
						{
							return component.Add(effect, should_save);
						}
					}
				}
				return null;
			}

			public void StopEffect(string effect)
			{
				if (!base.master.IsValidEffect(effect))
				{
					return;
				}
				Worker worker = base.smi.master.worker;
				if (worker != null)
				{
					Effects component = worker.GetComponent<Effects>();
					if (component.HasEffect(effect))
					{
						component.Remove(effect);
					}
				}
			}
		}

		public State unoperational;

		public OperationalStates operational;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = false;
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational).Enter(delegate(Instance smi)
			{
				smi.master.GetComponent<Assignable>().Unassign();
			});
			operational.DefaultState(operational.idle).EventTransition(GameHashes.OperationalChanged, unoperational, (Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).EventTransition(GameHashes.AssigneeChanged, unoperational)
				.ToggleRecurringChore((Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.Heal, allow_prioritization: false, allow_in_red_alert: true, PriorityScreen.PriorityClass.personalNeeds), (Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.HealCritical, allow_prioritization: false, allow_in_red_alert: true, PriorityScreen.PriorityClass.personalNeeds), (Instance smi) => !string.IsNullOrEmpty(smi.master.healthEffect))
				.ToggleRecurringChore((Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.RestDueToDisease, allow_prioritization: false, allow_in_red_alert: true, PriorityScreen.PriorityClass.personalNeeds, ignore_schedule_block: true), (Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect))
				.ToggleRecurringChore((Instance smi) => smi.master.CreateWorkChore(Db.Get().ChoreTypes.SleepDueToDisease, allow_prioritization: false, allow_in_red_alert: true, PriorityScreen.PriorityClass.personalNeeds, ignore_schedule_block: true), (Instance smi) => !string.IsNullOrEmpty(smi.master.diseaseEffect));
			operational.idle.WorkableStartTransition((Instance smi) => smi.master, operational.healing);
			operational.healing.DefaultState(operational.healing.undoctored).WorkableStopTransition((Instance smi) => smi.GetComponent<Clinic>(), operational.idle).Enter(delegate(Instance smi)
			{
				smi.master.GetComponent<Operational>().SetActive(value: true);
			})
				.Exit(delegate(Instance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(value: false);
				});
			operational.healing.undoctored.Enter(delegate(Instance smi)
			{
				smi.StartEffect(smi.master.healthEffect, should_save: false);
				smi.StartEffect(smi.master.diseaseEffect, should_save: false);
				bool flag = false;
				if (smi.master.worker != null)
				{
					flag = smi.HasEffect(smi.master.doctoredHealthEffect) || smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredPlaceholderEffect);
				}
				if (smi.master.AllowDoctoring())
				{
					if (flag)
					{
						smi.GoTo(operational.healing.doctored);
					}
					else
					{
						smi.StartDoctorChore();
					}
				}
			}).Exit(delegate(Instance smi)
			{
				smi.StopEffect(smi.master.healthEffect);
				smi.StopEffect(smi.master.diseaseEffect);
				smi.StopDoctorChore();
			});
			operational.healing.newlyDoctored.Enter(delegate(Instance smi)
			{
				smi.StartEffect(smi.master.doctoredDiseaseEffect, should_save: true);
				smi.StartEffect(smi.master.doctoredHealthEffect, should_save: true);
				smi.GoTo(operational.healing.doctored);
			});
			operational.healing.doctored.Enter(delegate(Instance smi)
			{
				Effects component3 = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredPlaceholderEffect))
				{
					EffectInstance effectInstance4 = component3.Get(smi.master.doctoredPlaceholderEffect);
					EffectInstance effectInstance5 = smi.StartEffect(smi.master.doctoredDiseaseEffect, should_save: true);
					if (effectInstance5 != null)
					{
						float num2 = effectInstance4.effect.duration - effectInstance4.timeRemaining;
						effectInstance5.timeRemaining = effectInstance5.effect.duration - num2;
					}
					EffectInstance effectInstance6 = smi.StartEffect(smi.master.doctoredHealthEffect, should_save: true);
					if (effectInstance6 != null)
					{
						float num3 = effectInstance4.effect.duration - effectInstance4.timeRemaining;
						effectInstance6.timeRemaining = effectInstance6.effect.duration - num3;
					}
					component3.Remove(smi.master.doctoredPlaceholderEffect);
				}
			}).ScheduleGoTo(delegate(Instance smi)
			{
				Effects component2 = smi.master.worker.GetComponent<Effects>();
				float num = smi.master.doctorVisitInterval;
				if (smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance3 = component2.Get(smi.master.doctoredHealthEffect);
					num = Mathf.Min(num, effectInstance3.GetTimeRemaining());
				}
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect))
				{
					EffectInstance effectInstance3 = component2.Get(smi.master.doctoredDiseaseEffect);
					num = Mathf.Min(num, effectInstance3.GetTimeRemaining());
				}
				return num;
			}, operational.healing.undoctored).Exit(delegate(Instance smi)
			{
				Effects component = smi.master.worker.GetComponent<Effects>();
				if (smi.HasEffect(smi.master.doctoredDiseaseEffect) || smi.HasEffect(smi.master.doctoredHealthEffect))
				{
					EffectInstance effectInstance = component.Get(smi.master.doctoredDiseaseEffect);
					if (effectInstance == null)
					{
						effectInstance = component.Get(smi.master.doctoredHealthEffect);
					}
					EffectInstance effectInstance2 = smi.StartEffect(smi.master.doctoredPlaceholderEffect, should_save: true);
					effectInstance2.timeRemaining = effectInstance2.effect.duration - (effectInstance.effect.duration - effectInstance.timeRemaining);
					component.Remove(smi.master.doctoredDiseaseEffect);
					component.Remove(smi.master.doctoredHealthEffect);
				}
			});
		}
	}

	[MyCmpReq]
	private Assignable assignable;

	private static readonly string[] EffectsRemoved = new string[1]
	{
		"SoreBack"
	};

	private const int MAX_RANGE = 10;

	private const float CHECK_RANGE_INTERVAL = 10f;

	public float doctorVisitInterval = 300f;

	public KAnimFile[] workerInjuredAnims;

	public KAnimFile[] workerDiseasedAnims;

	public string diseaseEffect;

	public string healthEffect;

	public string doctoredDiseaseEffect;

	public string doctoredHealthEffect;

	public string doctoredPlaceholderEffect;

	private ClinicSM.Instance clinicSMI;

	public static readonly Chore.Precondition IsOverSicknessThreshold = new Chore.Precondition
	{
		id = "IsOverSicknessThreshold",
		description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
		fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((Clinic)data).IsHealthBelowThreshold(context.consumerState.gameObject);
		}
	};

	[Serialize]
	private float sicknessSliderValue = 70f;

	public float MedicalAttentionMinimum => sicknessSliderValue / 100f;

	string ISliderControl.SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TITLE";

	string ISliderControl.SliderUnits => UI.UNITSUFFIXES.PERCENT;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = false;
		assignable.subSlots = new AssignableSlot[1]
		{
			Db.Get().AssignableSlots.MedicalBed
		};
		assignable.AddAutoassignPrecondition(CanAutoAssignTo);
		assignable.AddAssignPrecondition(CanManuallyAssignTo);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		Components.Clinics.Add(this);
		SetWorkTime(float.PositiveInfinity);
		clinicSMI = new ClinicSM.Instance(this);
		clinicSMI.StartSM();
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		Components.Clinics.Remove(this);
		base.OnCleanUp();
	}

	private KAnimFile[] GetAppropriateOverrideAnims(Worker worker)
	{
		KAnimFile[] result = null;
		if (!worker.GetSMI<WoundMonitor.Instance>().ShouldExitInfirmary())
		{
			result = workerInjuredAnims;
		}
		else if (workerDiseasedAnims != null && IsValidEffect(diseaseEffect) && worker.GetSMI<SicknessMonitor.Instance>().IsSick())
		{
			result = workerDiseasedAnims;
		}
		return result;
	}

	public override AnimInfo GetAnim(Worker worker)
	{
		overrideAnims = GetAppropriateOverrideAnims(worker);
		return base.GetAnim(worker);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		worker.GetComponent<Effects>().Add("Sleep", should_save: false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		KAnimFile[] appropriateOverrideAnims = GetAppropriateOverrideAnims(worker);
		if (appropriateOverrideAnims == null || appropriateOverrideAnims != overrideAnims)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove("Sleep");
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		assignable.Unassign();
		base.OnCompleteWork(worker);
		Effects component = worker.GetComponent<Effects>();
		for (int i = 0; i < EffectsRemoved.Length; i++)
		{
			string effect_id = EffectsRemoved[i];
			component.Remove(effect_id);
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	private Chore CreateWorkChore(ChoreType chore_type, bool allow_prioritization, bool allow_in_red_alert, PriorityScreen.PriorityClass priority_class, bool ignore_schedule_block = false)
	{
		bool allow_prioritization2 = allow_prioritization;
		return new WorkChore<Clinic>(chore_type, this, null, run_until_complete: true, null, null, null, allow_in_red_alert, null, ignore_schedule_block, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization2, priority_class, 5, ignore_building_assignment: false, add_to_daily_report: false);
	}

	private bool CanAutoAssignTo(MinionAssignablesProxy worker)
	{
		bool flag = false;
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			if (IsValidEffect(healthEffect))
			{
				Health component = minionIdentity.GetComponent<Health>();
				if (component != null && component.hitPoints < component.maxHitPoints)
				{
					flag = true;
				}
			}
			if (!flag && IsValidEffect(diseaseEffect))
			{
				flag = minionIdentity.GetComponent<MinionModifiers>().sicknesses.Count > 0;
			}
		}
		return flag;
	}

	private bool CanManuallyAssignTo(MinionAssignablesProxy worker)
	{
		bool result = false;
		MinionIdentity minionIdentity = worker.target as MinionIdentity;
		if (minionIdentity != null)
		{
			result = IsHealthBelowThreshold(minionIdentity.gameObject);
		}
		return result;
	}

	private bool IsHealthBelowThreshold(GameObject minion)
	{
		Health health = ((minion != null) ? minion.GetComponent<Health>() : null);
		if (health != null)
		{
			float num = health.hitPoints / health.maxHitPoints;
			if (health != null)
			{
				return num < MedicalAttentionMinimum;
			}
		}
		return false;
	}

	private bool IsValidEffect(string effect)
	{
		if (effect != null)
		{
			return effect != "";
		}
		return false;
	}

	private bool AllowDoctoring()
	{
		if (!IsValidEffect(doctoredDiseaseEffect))
		{
			return IsValidEffect(doctoredHealthEffect);
		}
		return true;
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (IsValidEffect(healthEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, descriptors, healthEffect);
		}
		if (diseaseEffect != healthEffect && IsValidEffect(diseaseEffect))
		{
			Effect.AddModifierDescriptions(base.gameObject, descriptors, diseaseEffect);
		}
		if (AllowDoctoring())
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.DOCTORING, UI.BUILDINGEFFECTS.TOOLTIPS.DOCTORING);
			descriptors.Add(item);
			if (IsValidEffect(doctoredHealthEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, descriptors, doctoredHealthEffect, increase_indent: true);
			}
			if (doctoredDiseaseEffect != doctoredHealthEffect && IsValidEffect(doctoredDiseaseEffect))
			{
				Effect.AddModifierDescriptions(base.gameObject, descriptors, doctoredDiseaseEffect, increase_indent: true);
			}
		}
		return descriptors;
	}

	int ISliderControl.SliderDecimalPlaces(int index)
	{
		return 0;
	}

	float ISliderControl.GetSliderMin(int index)
	{
		return 0f;
	}

	float ISliderControl.GetSliderMax(int index)
	{
		return 100f;
	}

	float ISliderControl.GetSliderValue(int index)
	{
		return sicknessSliderValue;
	}

	void ISliderControl.SetSliderValue(float percent, int index)
	{
		if (percent != sicknessSliderValue)
		{
			sicknessSliderValue = Mathf.RoundToInt(percent);
			Game.Instance.Trigger(875045922);
		}
	}

	string ISliderControl.GetSliderTooltip()
	{
		return string.Format(Strings.Get("STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP"), sicknessSliderValue);
	}

	string ISliderControl.GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.MEDICALCOTSIDESCREEN.TOOLTIP";
	}
}
