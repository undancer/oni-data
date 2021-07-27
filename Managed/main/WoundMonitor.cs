public class WoundMonitor : GameStateMachine<WoundMonitor, WoundMonitor.Instance>
{
	public class Wounded : State
	{
		public State light;

		public State medium;

		public State heavy;
	}

	public new class Instance : GameInstance
	{
		public Health health;

		private Worker worker;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			health = master.GetComponent<Health>();
			worker = master.GetComponent<Worker>();
		}

		public void OnHealthChanged(object data)
		{
			float num = (float)data;
			if (health.hitPoints != 0f && num < 0f)
			{
				PlayHitAnimation();
			}
		}

		private void PlayHitAnimation()
		{
			string text = null;
			KBatchedAnimController kBatchedAnimController = base.smi.Get<KBatchedAnimController>();
			if (kBatchedAnimController.CurrentAnim != null)
			{
				text = kBatchedAnimController.CurrentAnim.name;
			}
			KAnim.PlayMode playMode = kBatchedAnimController.PlayMode;
			if (text == null || (!text.Contains("hit") && !text.Contains("2_0") && !text.Contains("2_1") && !text.Contains("2_-1") && !text.Contains("2_-2") && !text.Contains("1_-1") && !text.Contains("1_-2") && !text.Contains("1_1") && !text.Contains("1_2") && !text.Contains("breathe_") && !text.Contains("death_") && !text.Contains("impact")))
			{
				string text2 = "hit";
				AttackChore.StatesInstance sMI = base.gameObject.GetSMI<AttackChore.StatesInstance>();
				if (sMI != null && sMI.GetCurrentState() == sMI.sm.attack)
				{
					text2 = sMI.master.GetHitAnim();
				}
				if (worker.GetComponent<Navigator>().CurrentNavType == NavType.Ladder)
				{
					text2 = "hit_ladder";
				}
				else if (worker.GetComponent<Navigator>().CurrentNavType == NavType.Pole)
				{
					text2 = "hit_pole";
				}
				kBatchedAnimController.Play(text2);
				if (text != null)
				{
					kBatchedAnimController.Queue(text, playMode);
				}
			}
		}

		public void PlayKnockedOverImpactAnimation()
		{
			string text = null;
			KBatchedAnimController kBatchedAnimController = base.smi.Get<KBatchedAnimController>();
			if (kBatchedAnimController.CurrentAnim != null)
			{
				text = kBatchedAnimController.CurrentAnim.name;
			}
			KAnim.PlayMode playMode = kBatchedAnimController.PlayMode;
			if (text == null || (!text.Contains("impact") && !text.Contains("2_0") && !text.Contains("2_1") && !text.Contains("2_-1") && !text.Contains("2_-2") && !text.Contains("1_-1") && !text.Contains("1_-2") && !text.Contains("1_1") && !text.Contains("1_2") && !text.Contains("breathe_") && !text.Contains("death_")))
			{
				string text2 = "impact";
				kBatchedAnimController.Play(text2);
				if (text != null)
				{
					kBatchedAnimController.Queue(text, playMode);
				}
			}
		}

		public void GoToProperHeathState()
		{
			switch (base.smi.health.State)
			{
			case Health.HealthState.Perfect:
				base.smi.GoTo(base.sm.healthy);
				break;
			case Health.HealthState.Critical:
				base.smi.GoTo(base.sm.wounded.heavy);
				break;
			case Health.HealthState.Injured:
				base.smi.GoTo(base.sm.wounded.medium);
				break;
			case Health.HealthState.Scuffed:
				base.smi.GoTo(base.sm.wounded.light);
				break;
			case Health.HealthState.Alright:
				break;
			}
		}

		public bool ShouldExitInfirmary()
		{
			if (health.State == Health.HealthState.Perfect)
			{
				return true;
			}
			return false;
		}

		public void FindAvailableMedicalBed()
		{
			AssignableSlot clinic = Db.Get().AssignableSlots.Clinic;
			Ownables soleOwner = base.gameObject.GetComponent<MinionIdentity>().GetSoleOwner();
			if (soleOwner.GetSlot(clinic).assignable == null)
			{
				soleOwner.AutoAssignSlot(clinic);
			}
		}
	}

	public State healthy;

	public Wounded wounded;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = healthy;
		root.ToggleAnims("anim_hits_kanim").ToggleAnims("anim_impact_kanim", 0f, "EXPANSION1_ID").EventHandler(GameHashes.HealthChanged, delegate(Instance smi, object data)
		{
			smi.OnHealthChanged(data);
		});
		healthy.EventTransition(GameHashes.HealthChanged, wounded, (Instance smi) => smi.health.State != Health.HealthState.Perfect);
		wounded.ToggleUrge(Db.Get().Urges.Heal).Enter(delegate(Instance smi)
		{
			switch (smi.health.State)
			{
			case Health.HealthState.Critical:
				smi.GoTo(wounded.heavy);
				break;
			case Health.HealthState.Injured:
				smi.GoTo(wounded.medium);
				break;
			case Health.HealthState.Scuffed:
				smi.GoTo(wounded.light);
				break;
			}
		}).EventHandler(GameHashes.HealthChanged, delegate(Instance smi)
		{
			smi.GoToProperHeathState();
		});
		wounded.medium.ToggleAnims("anim_loco_wounded_kanim", 1f);
		wounded.heavy.ToggleAnims("anim_loco_wounded_kanim", 3f).Update("LookForAvailableClinic", delegate(Instance smi, float dt)
		{
			smi.FindAvailableMedicalBed();
		}, UpdateRate.SIM_1000ms);
	}
}
