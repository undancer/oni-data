using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class RadiationEater : StateMachineComponent<RadiationEater.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RadiationEater, object>.GameInstance
	{
		public AttributeModifier radiationEating;

		public StatesInstance(RadiationEater master)
			: base(master)
		{
			radiationEating = new AttributeModifier(Db.Get().Attributes.RadiationRecovery.Id, TRAITS.RADIATION_EATER_RECOVERY, DUPLICANTS.TRAITS.RADIATIONEATER.NAME);
		}

		public void OnEatRads(float radsEaten)
		{
			float delta = Mathf.Abs(radsEaten) * TRAITS.RADS_TO_CALS;
			base.smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.Calories).ApplyDelta(delta);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RadiationEater>
	{
		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleAttributeModifier("Radiation Eating", (StatesInstance smi) => smi.radiationEating).EventHandler(GameHashes.RadiationRecovery, delegate(StatesInstance smi, object data)
			{
				float radsEaten = (float)data;
				smi.OnEatRads(radsEaten);
			});
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
