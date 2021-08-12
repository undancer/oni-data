using UnityEngine;

public class CropTendingMonitor : GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>
{
	public class Def : BaseDef
	{
		public float numCropsTendedPerCycle = 8f;

		public float unsatisfiedTendChance = 0.5f;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}
	}

	private FloatParameter cooldownTimer;

	private State cooldown;

	private State lookingForCrop;

	private State reset;

	private bool InterestedInTendingCrops(Instance smi)
	{
		if (smi.HasTag(GameTags.Creatures.Hungry) && Random.value > smi.def.unsatisfiedTendChance)
		{
			return false;
		}
		return true;
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = cooldown;
		base.serializable = SerializeType.ParamsOnly;
		cooldown.ParamTransition(cooldownTimer, lookingForCrop, (Instance smi, float p) => cooldownTimer.Get(smi) <= 0f && InterestedInTendingCrops(smi)).ParamTransition(cooldownTimer, reset, (Instance smi, float p) => cooldownTimer.Get(smi) <= 0f && !InterestedInTendingCrops(smi)).Update(delegate(Instance smi, float dt)
		{
			cooldownTimer.Delta(0f - dt, smi);
		}, UpdateRate.SIM_1000ms);
		lookingForCrop.ToggleBehaviour(GameTags.Creatures.WantsToTendCrops, (Instance smi) => true, delegate(Instance smi)
		{
			smi.GoTo(reset);
		});
		reset.Exit(delegate(Instance smi)
		{
			cooldownTimer.Set(600f / smi.def.numCropsTendedPerCycle, smi);
		}).GoTo(cooldown);
	}
}
