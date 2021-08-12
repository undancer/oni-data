using UnityEngine;

public class IncapacitationMonitor : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			Health component = master.GetComponent<Health>();
			if ((bool)component)
			{
				component.CanBeIncapacitated = true;
			}
		}

		public void Bleed(float dt, Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(dt * (0f - smi.sm.baseBleedOutSpeed.Get(smi)), smi);
		}

		public void RecoverStamina(float dt, Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(Mathf.Min(dt * smi.sm.baseStaminaRecoverSpeed.Get(smi), smi.sm.maxBleedOutStamina.Get(smi) - smi.sm.bleedOutStamina.Get(smi)), smi);
		}

		public float GetBleedLifeTime(Instance smi)
		{
			return Mathf.Floor(smi.sm.bleedOutStamina.Get(smi) / smi.sm.baseBleedOutSpeed.Get(smi));
		}

		public Death GetCauseOfIncapacitation()
		{
			KPrefabID component = GetComponent<KPrefabID>();
			if (component.HasTag(GameTags.HitByHighEnergyParticle))
			{
				return Db.Get().Deaths.HitByHighEnergyParticle;
			}
			if (component.HasTag(GameTags.RadiationSicknessIncapacitation))
			{
				return Db.Get().Deaths.Radiation;
			}
			if (component.HasTag(GameTags.CaloriesDepleted))
			{
				return Db.Get().Deaths.Starvation;
			}
			if (component.HasTag(GameTags.HitPointsDepleted))
			{
				return Db.Get().Deaths.Slain;
			}
			return Db.Get().Deaths.Generic;
		}
	}

	public State healthy;

	public State start_recovery;

	public State incapacitated;

	public State die;

	private FloatParameter bleedOutStamina = new FloatParameter(120f);

	private FloatParameter baseBleedOutSpeed = new FloatParameter(1f);

	private FloatParameter baseStaminaRecoverSpeed = new FloatParameter(1f);

	private FloatParameter maxBleedOutStamina = new FloatParameter(120f);

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = healthy;
		base.serializable = SerializeType.Both_DEPRECATED;
		healthy.TagTransition(GameTags.CaloriesDepleted, incapacitated).TagTransition(GameTags.HitPointsDepleted, incapacitated).TagTransition(GameTags.HitByHighEnergyParticle, incapacitated)
			.TagTransition(GameTags.RadiationSicknessIncapacitation, incapacitated)
			.Update(delegate(Instance smi, float dt)
			{
				smi.RecoverStamina(dt, smi);
			});
		start_recovery.TagTransition(new Tag[2]
		{
			GameTags.CaloriesDepleted,
			GameTags.HitPointsDepleted
		}, healthy, on_remove: true);
		incapacitated.EventTransition(GameHashes.IncapacitationRecovery, start_recovery).ToggleTag(GameTags.Incapacitated).ToggleRecurringChore((Instance smi) => new BeIncapacitatedChore(smi.master))
			.ParamTransition(bleedOutStamina, die, GameStateMachine<IncapacitationMonitor, Instance, IStateMachineTarget, object>.IsLTEZero)
			.ToggleUrge(Db.Get().Urges.BeIncapacitated)
			.Enter(delegate(Instance smi)
			{
				smi.master.Trigger(-1506500077);
			})
			.Update(delegate(Instance smi, float dt)
			{
				smi.Bleed(dt, smi);
			});
		die.Enter(delegate(Instance smi)
		{
			smi.master.gameObject.GetSMI<DeathMonitor.Instance>().Kill(smi.GetCauseOfIncapacitation());
		});
	}
}
