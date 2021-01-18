using Klei.AI;
using KSerialization;
using UnityEngine;

public class CoughMonitor : GameStateMachine<CoughMonitor, CoughMonitor.Instance, IStateMachineTarget, CoughMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public float lastCoughTime;

		[Serialize]
		public float lastConsumeTime;

		[Serialize]
		public float amountConsumed;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public Reactable GetReactable()
		{
			EmoteReactable emoteReactable = new SelfEmoteReactable(base.master.gameObject, "BadAirCough", Db.Get().ChoreTypes.Cough, "anim_slimelungcough_kanim", 0f, 0f).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react_small",
				finishcb = FinishedCoughing
			});
			emoteReactable.preventChoreInterruption = true;
			return emoteReactable;
		}

		private void FinishedCoughing(GameObject cougher)
		{
			cougher.GetComponent<Effects>().Add("ContaminatedLungs", should_save: true);
			base.sm.shouldCough.Set(value: false, base.smi);
			base.smi.lastCoughTime = GameClock.Instance.GetTimeInCycles();
		}
	}

	private const float amountToCough = 1f;

	private const float decayRate = 0.05f;

	private const float coughInterval = 0.1f;

	public State idle;

	public State coughing;

	public BoolParameter shouldCough = new BoolParameter(default_value: false);

	public override void InitializeStates(out BaseState default_state)
	{
		base.serializable = SerializeType.ParamsOnly;
		default_state = idle;
		idle.EventHandler(GameHashes.PoorAirQuality, OnBreatheDirtyAir).ParamTransition(shouldCough, coughing, (Instance smi, bool bShouldCough) => bShouldCough);
		coughing.ToggleStatusItem(Db.Get().DuplicantStatusItems.Coughing).ToggleReactable((Instance smi) => smi.GetReactable()).ParamTransition(shouldCough, idle, (Instance smi, bool bShouldCough) => !bShouldCough);
	}

	private void OnBreatheDirtyAir(Instance smi, object data)
	{
		float timeInCycles = GameClock.Instance.GetTimeInCycles();
		if (!(timeInCycles > 0.1f) || !(timeInCycles - smi.lastCoughTime <= 0.1f))
		{
			Sim.MassConsumedCallback massConsumedCallback = (Sim.MassConsumedCallback)data;
			float num = ((smi.lastConsumeTime <= 0f) ? 0f : (timeInCycles - smi.lastConsumeTime));
			smi.lastConsumeTime = timeInCycles;
			smi.amountConsumed -= 0.05f * num;
			smi.amountConsumed = Mathf.Max(smi.amountConsumed, 0f);
			smi.amountConsumed += massConsumedCallback.mass;
			if (smi.amountConsumed >= 1f)
			{
				shouldCough.Set(value: true, smi);
				smi.lastConsumeTime = 0f;
				smi.amountConsumed = 0f;
			}
		}
	}
}
