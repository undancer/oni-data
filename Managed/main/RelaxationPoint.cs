using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RelaxationPoint")]
public class RelaxationPoint : Workable, IGameObjectEffectDescriptor
{
	public class RelaxationPointSM : GameStateMachine<RelaxationPointSM, RelaxationPointSM.Instance, RelaxationPoint>
	{
		public new class Instance : GameInstance
		{
			public Instance(RelaxationPoint master)
				: base(master)
			{
			}
		}

		public State unoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.EventTransition(GameHashes.OperationalChanged, operational, (Instance smi) => smi.GetComponent<Operational>().IsOperational).PlayAnim("off");
			operational.ToggleChore((Instance smi) => smi.master.CreateWorkChore(), unoperational);
		}
	}

	[MyCmpGet]
	private RoomTracker roomTracker;

	[Serialize]
	protected float stopStressingValue;

	public float stressModificationValue;

	public float roomStressModificationValue;

	private RelaxationPointSM.Instance smi;

	private static Effect stressReductionEffect;

	private static Effect roomStressReductionEffect;

	public RelaxationPoint()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
		showProgressBar = false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		lightEfficiencyBonus = false;
		GetComponent<KPrefabID>().AddTag(TagManager.Create("RelaxationPoint", MISC.TAGS.RELAXATION_POINT));
		if (stressReductionEffect == null)
		{
			stressReductionEffect = CreateEffect();
			roomStressReductionEffect = CreateRoomEffect();
		}
	}

	public Effect CreateEffect()
	{
		Effect effect = new Effect("StressReduction", DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, stressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION.NAME);
		effect.Add(modifier);
		return effect;
	}

	public Effect CreateRoomEffect()
	{
		Effect effect = new Effect("RoomRelaxationEffect", DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.TOOLTIP, 0f, show_in_ui: true, trigger_floating_text: false, is_bad: false);
		AttributeModifier modifier = new AttributeModifier(Db.Get().Amounts.Stress.deltaAttribute.Id, roomStressModificationValue / 600f, DUPLICANTS.MODIFIERS.STRESSREDUCTION_CLINIC.NAME);
		effect.Add(modifier);
		return effect;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new RelaxationPointSM.Instance(this);
		smi.StartSM();
		SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (roomTracker != null && roomTracker.room != null && roomTracker.room.roomType == Db.Get().RoomTypes.MassageClinic)
		{
			worker.GetComponent<Effects>().Add(roomStressReductionEffect, should_save: false);
		}
		else
		{
			worker.GetComponent<Effects>().Add(stressReductionEffect, should_save: false);
		}
		GetComponent<Operational>().SetActive(value: true);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (Db.Get().Amounts.Stress.Lookup(worker.gameObject).value <= stopStressingValue)
		{
			return true;
		}
		base.OnWorkTick(worker, dt);
		return false;
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetComponent<Effects>().Remove(stressReductionEffect);
		worker.GetComponent<Effects>().Remove(roomStressReductionEffect);
		GetComponent<Operational>().SetActive(value: false);
		base.OnStopWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	protected virtual WorkChore<RelaxationPoint> CreateWorkChore()
	{
		return new WorkChore<RelaxationPoint>(Db.Get().ChoreTypes.Relax, this, null, run_until_complete: false, null, null, null, allow_in_red_alert: false);
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STRESSREDUCEDPERMINUTE, GameUtil.GetFormattedPercent(stressModificationValue / 600f * 60f)));
		descriptors.Add(item);
		return descriptors;
	}
}
