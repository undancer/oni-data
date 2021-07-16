using Klei.AI;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Stinky : StateMachineComponent<Stinky.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Stinky, object>.GameInstance
	{
		public StatesInstance(Stinky master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Stinky>
	{
		public State idle;

		public State emit;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, null).Enter(delegate(StatesInstance smi)
			{
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", smi.master.gameObject.transform.GetPosition(), smi.master.gameObject.transform, update_looping_sounds_position: true);
				kBatchedAnimController.Play(WorkLoopAnims);
				smi.master.stinkyController = kBatchedAnimController;
			}).Update("StinkyFX", delegate(StatesInstance smi, float dt)
			{
				if (smi.master.stinkyController != null)
				{
					smi.master.stinkyController.Play(WorkLoopAnims);
				}
			}, UpdateRate.SIM_4000ms);
			idle.Enter("ScheduleNextFart", delegate(StatesInstance smi)
			{
				smi.ScheduleGoTo(GetNewInterval(), emit);
			});
			emit.Enter("Fart", delegate(StatesInstance smi)
			{
				smi.master.Emit(smi.master.gameObject);
			}).ToggleExpression(Db.Get().Expressions.Relief).ScheduleGoTo(3f, idle);
		}

		private float GetNewInterval()
		{
			return Mathf.Min(Mathf.Max(Util.GaussianRandom(TRAITS.STINKY_EMIT_INTERVAL_MAX - TRAITS.STINKY_EMIT_INTERVAL_MIN), TRAITS.STINKY_EMIT_INTERVAL_MIN), TRAITS.STINKY_EMIT_INTERVAL_MAX);
		}
	}

	private const float EmitMass = 0.0025000002f;

	private const SimHashes EmitElement = SimHashes.ContaminatedOxygen;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

	private KBatchedAnimController stinkyController;

	private static readonly HashedString[] WorkLoopAnims = new HashedString[3]
	{
		"working_pre",
		"working_loop",
		"working_pst"
	};

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	private void Emit(object data)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 a = gameObject.transform.GetPosition();
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (minionIdentity.gameObject != gameObject.gameObject)
			{
				Vector2 b = minionIdentity.transform.GetPosition();
				if (Vector2.SqrMagnitude(a - b) <= 2.25f)
				{
					minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
					minionIdentity.GetComponent<Effects>().Add("SmelledStinky", should_save: true);
					minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
				}
			}
		}
		SimMessages.AddRemoveSubstance(Grid.PosToCell(gameObject.transform.GetPosition()), temperature: Db.Get().Amounts.Temperature.Lookup(this).value, new_element: SimHashes.ContaminatedOxygen, ev: CellEventLogger.Instance.ElementConsumerSimUpdate, mass: 0.0025000002f, disease_idx: byte.MaxValue, disease_count: 0);
		bool flag = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		Vector3 vector = gameObject.GetComponent<Transform>().GetPosition();
		float volume = 1f;
		if (flag)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
			volume = SoundEvent.GetVolume(flag);
		}
		else
		{
			vector.z = 0f;
		}
		KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), vector, volume);
	}
}
