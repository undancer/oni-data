using System.Collections.Generic;
using Klei.AI;
using STRINGS;

public class RadiationMonitor : GameStateMachine<RadiationMonitor, RadiationMonitor.Instance>
{
	public class SickStates : State
	{
		public State waiting;

		public State vomiting;
	}

	public new class Instance : GameInstance
	{
		public Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = GetComponent<Effects>();
		}
	}

	public const float BASE_ABSORBTION_RATE = 1f;

	public FloatParameter radiationExposure;

	public BoolParameter isSick;

	public State idle;

	public SickStates sick;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		root.Update(CheckRadiationLevel, UpdateRate.SIM_1000ms);
		idle.DoNothing().ParamTransition(isSick, sick, GameStateMachine<RadiationMonitor, Instance, IStateMachineTarget, object>.IsTrue);
		sick.DefaultState(sick.waiting).ParamTransition(isSick, idle, GameStateMachine<RadiationMonitor, Instance, IStateMachineTarget, object>.IsFalse).ToggleAnims("anim_loco_wounded_kanim", 2f);
		sick.waiting.ScheduleGoTo(60f, sick.vomiting);
		sick.vomiting.ToggleChore(CreateVomitChore, sick.waiting);
	}

	private Chore CreateVomitChore(Instance smi)
	{
		Notification notification = new Notification(DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => string.Concat(DUPLICANTS.STATUSITEMS.RADIATIONVOMITING.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)));
		return new VomitChore(Db.Get().ChoreTypes.Vomit, smi.master, Db.Get().DuplicantStatusItems.Vomiting, notification);
	}

	private static void RadiationRecovery(Instance smi, float dt)
	{
		smi.sm.radiationExposure.Delta(Db.Get().Attributes.RadiationRecovery.Lookup(smi.gameObject).GetTotalValue() * dt, smi);
	}

	private static void CheckRadiationLevel(Instance smi, float dt)
	{
		RadiationRecovery(smi, dt);
		int num = Grid.PosToCell(smi.gameObject);
		if (Grid.IsValidCell(num))
		{
			float delta_value = Grid.Radiation[num] * 1f * (1f - Db.Get().Attributes.RadiationResistance.Lookup(smi.gameObject).GetTotalValue());
			smi.sm.radiationExposure.DeltaClamp(delta_value, 0f, 10000f, smi);
		}
		bool value = false;
		if (smi.sm.radiationExposure.Get(smi) >= 10000f)
		{
			smi.GetComponent<Health>().Incapacitate(GameTags.RadiationSicknessIncapacitation);
		}
		else if (smi.sm.radiationExposure.Get(smi) >= 8000f)
		{
			value = true;
			smi.effects.Add(Db.Get().effects.Get("RadiationExposureExtreme"), should_save: true);
		}
		else if (smi.sm.radiationExposure.Get(smi) >= 5000f)
		{
			value = true;
			smi.effects.Add(Db.Get().effects.Get("RadiationExposureMajor"), should_save: true);
		}
		else if (smi.sm.radiationExposure.Get(smi) >= 2000f)
		{
			value = true;
			smi.effects.Add(Db.Get().effects.Get("RadiationExposureMinor"), should_save: true);
		}
		smi.sm.isSick.Set(value, smi);
	}
}
