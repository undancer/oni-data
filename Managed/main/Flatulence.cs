using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class Flatulence : StateMachineComponent<Flatulence.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Flatulence, object>.GameInstance
	{
		public StatesInstance(Flatulence master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Flatulence>
	{
		public State idle;

		public State emit;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.TagTransition(GameTags.Dead, null);
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
			float mu = TRAITS.FLATULENCE_EMIT_INTERVAL_MAX - TRAITS.FLATULENCE_EMIT_INTERVAL_MIN;
			float a = Util.GaussianRandom(mu);
			a = Mathf.Max(a, TRAITS.FLATULENCE_EMIT_INTERVAL_MIN);
			return Mathf.Min(a, TRAITS.FLATULENCE_EMIT_INTERVAL_MAX);
		}
	}

	private const float EmitMass = 0.1f;

	private const SimHashes EmitElement = SimHashes.Methane;

	private const float EmissionRadius = 1.5f;

	private const float MaxDistanceSq = 2.25f;

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
		float value = Db.Get().Amounts.Temperature.Lookup(this).value;
		Equippable equippable = GetComponent<SuitEquipper>().IsWearingAirtightSuit();
		if (equippable != null)
		{
			equippable.GetComponent<Storage>().AddGasChunk(SimHashes.Methane, 0.1f, value, byte.MaxValue, 0, keep_zero_mass: false);
		}
		else
		{
			Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
			Vector2 a = gameObject.transform.GetPosition();
			for (int i = 0; i < liveMinionIdentities.Count; i++)
			{
				MinionIdentity minionIdentity = liveMinionIdentities[i];
				if (minionIdentity.gameObject != gameObject.gameObject)
				{
					Vector2 b = minionIdentity.transform.GetPosition();
					float num = Vector2.SqrMagnitude(a - b);
					if (num <= 2.25f)
					{
						minionIdentity.Trigger(508119890, Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String);
						minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
					}
				}
			}
			int gameCell = Grid.PosToCell(gameObject.transform.GetPosition());
			SimMessages.AddRemoveSubstance(gameCell, SimHashes.Methane, CellEventLogger.Instance.ElementConsumerSimUpdate, 0.1f, value, byte.MaxValue, 0);
			KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("odor_fx_kanim", gameObject.transform.GetPosition(), gameObject.transform, update_looping_sounds_position: true);
			kBatchedAnimController.Play(WorkLoopAnims);
			kBatchedAnimController.destroyOnAnimComplete = true;
		}
		GameObject gameObject2 = gameObject;
		bool flag = SoundEvent.ObjectIsSelectedAndVisible(gameObject2);
		Vector3 vector = gameObject2.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
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
