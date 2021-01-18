using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/DoctorStation")]
public class DoctorStation : Workable
{
	public class States : GameStateMachine<States, StatesInstance, DoctorStation>
	{
		public class OperationalStates : State
		{
			public State not_ready;

			public ReadyStates ready;
		}

		public class ReadyStates : State
		{
			public State idle;

			public PatientStates has_patient;
		}

		public class PatientStates : State
		{
			public State waiting;

			public State being_treated;
		}

		public State unoperational;

		public OperationalStates operational;

		public BoolParameter hasSupplies;

		public BoolParameter hasPatient;

		public BoolParameter hasDoctor;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = false;
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (StatesInstance smi) => smi.master.operational.IsOperational);
			operational.EventTransition(GameHashes.OperationalChanged, operational, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(operational.not_ready);
			operational.not_ready.ParamTransition(hasSupplies, operational.ready, (StatesInstance smi, bool p) => p);
			operational.ready.DefaultState(operational.ready.idle).ToggleRecurringChore(CreatePatientChore).ParamTransition(hasSupplies, operational.not_ready, (StatesInstance smi, bool p) => !p);
			operational.ready.idle.ParamTransition(hasPatient, operational.ready.has_patient, (StatesInstance smi, bool p) => p);
			operational.ready.has_patient.ParamTransition(hasPatient, operational.ready.idle, (StatesInstance smi, bool p) => !p).DefaultState(operational.ready.has_patient.waiting).ToggleRecurringChore(CreateDoctorChore);
			operational.ready.has_patient.waiting.ParamTransition(hasDoctor, operational.ready.has_patient.being_treated, (StatesInstance smi, bool p) => p);
			operational.ready.has_patient.being_treated.ParamTransition(hasDoctor, operational.ready.has_patient.waiting, (StatesInstance smi, bool p) => !p).Enter(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: true);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.GetComponent<Operational>().SetActive(value: false);
			});
		}

		private Chore CreatePatientChore(StatesInstance smi)
		{
			WorkChore<DoctorStation> workChore = new WorkChore<DoctorStation>(Db.Get().ChoreTypes.GetDoctored, smi.master, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.personalNeeds);
			workChore.AddPrecondition(TreatmentAvailable, smi.master);
			workChore.AddPrecondition(DoctorAvailable, smi.master);
			return workChore;
		}

		private Chore CreateDoctorChore(StatesInstance smi)
		{
			DoctorStationDoctorWorkable component = smi.master.GetComponent<DoctorStationDoctorWorkable>();
			return new WorkChore<DoctorStationDoctorWorkable>(Db.Get().ChoreTypes.Doctor, component, null, run_until_complete: true, null, null, null, allow_in_red_alert: false, null, ignore_schedule_block: false, only_when_operational: true, null, is_preemptable: false, allow_in_context_menu: true, allow_prioritization: false, PriorityScreen.PriorityClass.high);
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, DoctorStation, object>.GameInstance
	{
		public StatesInstance(DoctorStation master)
			: base(master)
		{
		}
	}

	private static readonly EventSystem.IntraObjectHandler<DoctorStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<DoctorStation>(delegate(DoctorStation component, object data)
	{
		component.OnStorageChange(data);
	});

	[MyCmpReq]
	public Storage storage;

	[MyCmpReq]
	public Operational operational;

	private DoctorStationDoctorWorkable doctor_workable;

	[SerializeField]
	public Tag supplyTag;

	private Dictionary<HashedString, Tag> treatments_available = new Dictionary<HashedString, Tag>();

	private StatesInstance smi;

	public static readonly Chore.Precondition TreatmentAvailable;

	public static readonly Chore.Precondition DoctorAvailable;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		doctor_workable = GetComponent<DoctorStationDoctorWorkable>();
		SetWorkTime(float.PositiveInfinity);
		smi = new StatesInstance(this);
		smi.StartSM();
		OnStorageChange();
		Subscribe(-1697596308, OnStorageChangeDelegate);
	}

	protected override void OnCleanUp()
	{
		Prioritizable.RemoveRef(base.gameObject);
		if (smi != null)
		{
			smi.StopSM("OnCleanUp");
			smi = null;
		}
		base.OnCleanUp();
	}

	private void OnStorageChange(object data = null)
	{
		treatments_available.Clear();
		foreach (GameObject item in storage.items)
		{
			if (item.HasTag(GameTags.MedicalSupplies))
			{
				Tag tag = item.PrefabID();
				if (tag == "IntermediateCure")
				{
					AddTreatment("SlimeSickness", tag);
				}
				if (tag == "AdvancedCure")
				{
					AddTreatment("ZombieSickness", tag);
				}
			}
		}
		bool value = treatments_available.Count > 0;
		smi.sm.hasSupplies.Set(value, smi);
	}

	private void AddTreatment(string id, Tag tag)
	{
		if (!treatments_available.ContainsKey(id))
		{
			treatments_available.Add(id, tag);
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		smi.sm.hasPatient.Set(value: true, smi);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		smi.sm.hasPatient.Set(value: false, smi);
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	public void SetHasDoctor(bool has)
	{
		smi.sm.hasDoctor.Set(has, smi);
	}

	public void CompleteDoctoring()
	{
		if ((bool)base.worker)
		{
			CompleteDoctoring(base.worker.gameObject);
		}
	}

	private void CompleteDoctoring(GameObject target)
	{
		Sicknesses sicknesses = target.GetSicknesses();
		if (sicknesses == null)
		{
			return;
		}
		bool flag = false;
		foreach (SicknessInstance item in sicknesses)
		{
			if (treatments_available.TryGetValue(item.Sickness.id, out var value))
			{
				Game.Instance.savedInfo.curedDisease = true;
				item.Cure();
				storage.ConsumeIgnoringDisease(value, 1f);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogWarningFormat(base.gameObject, "Failed to treat any disease for {0}", target);
		}
	}

	public bool IsDoctorAvailable(GameObject target)
	{
		if (!string.IsNullOrEmpty(doctor_workable.requiredSkillPerk))
		{
			MinionResume component = target.GetComponent<MinionResume>();
			if (!MinionResume.AnyOtherMinionHasPerk(doctor_workable.requiredSkillPerk, component))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsTreatmentAvailable(GameObject target)
	{
		Sicknesses sicknesses = target.GetSicknesses();
		if (sicknesses != null)
		{
			foreach (SicknessInstance item in sicknesses)
			{
				if (treatments_available.TryGetValue(item.Sickness.id, out var _))
				{
					return true;
				}
			}
		}
		return false;
	}

	static DoctorStation()
	{
		Chore.Precondition precondition = new Chore.Precondition
		{
			id = "TreatmentAvailable",
			description = DUPLICANTS.CHORES.PRECONDITIONS.TREATMENT_AVAILABLE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return ((DoctorStation)data).IsTreatmentAvailable(context.consumerState.gameObject);
			}
		};
		TreatmentAvailable = precondition;
		precondition = new Chore.Precondition
		{
			id = "DoctorAvailable",
			description = DUPLICANTS.CHORES.PRECONDITIONS.DOCTOR_AVAILABLE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return ((DoctorStation)data).IsDoctorAvailable(context.consumerState.gameObject);
			}
		};
		DoctorAvailable = precondition;
	}
}
