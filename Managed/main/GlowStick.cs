using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class GlowStick : StateMachineComponent<GlowStick.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, GlowStick, object>.GameInstance
	{
		[MyCmpAdd]
		private RadiationEmitter _radiationEmitter;

		[MyCmpAdd]
		private Light2D _light2D;

		public AttributeModifier radiationResistance;

		public StatesInstance(GlowStick master)
			: base(master)
		{
			_light2D.Color = Color.green;
			_light2D.Range = 2f;
			_light2D.Angle = 0f;
			_light2D.Direction = LIGHT2D.DEFAULT_DIRECTION;
			_light2D.Offset = new Vector2(0.05f, 0.5f);
			_light2D.shape = LightShape.Circle;
			_light2D.Lux = 500;
			_radiationEmitter.emitRads = 100f;
			_radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			_radiationEmitter.emitRate = 0.5f;
			_radiationEmitter.emitRadiusX = 3;
			_radiationEmitter.emitRadiusY = 3;
			radiationResistance = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, TRAITS.GLOWSTICK_RADIATION_RESISTANCE, DUPLICANTS.TRAITS.GLOWSTICK.NAME);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, GlowStick>
	{
		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleComponent<RadiationEmitter>().ToggleComponent<Light2D>().ToggleAttributeModifier("Radiation Resistance", (StatesInstance smi) => smi.radiationResistance);
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
