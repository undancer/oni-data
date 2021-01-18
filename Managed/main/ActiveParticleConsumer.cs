using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ActiveParticleConsumer : GameStateMachine<ActiveParticleConsumer, ActiveParticleConsumer.Instance, IStateMachineTarget, ActiveParticleConsumer.Def>
{
	public class Def : BaseDef, IGameObjectEffectDescriptor
	{
		public float activeConsumptionRate = 1f;

		public float minParticlesForOperational = 1f;

		public string meterSymbolName = null;

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> list = new List<Descriptor>();
			list.Add(new Descriptor(UI.BUILDINGEFFECTS.ACTIVE_PARTICLE_CONSUMPTION.Replace("{Rate}", GameUtil.GetFormattedHighEnergyParticles(activeConsumptionRate, GameUtil.TimeSlice.PerSecond)), UI.BUILDINGEFFECTS.TOOLTIPS.ACTIVE_PARTICLE_CONSUMPTION.Replace("{Rate}", GameUtil.GetFormattedHighEnergyParticles(activeConsumptionRate, GameUtil.TimeSlice.PerSecond)), Descriptor.DescriptorType.Requirement));
			return list;
		}
	}

	public class OperationalStates : State
	{
		public State waiting;

		public State consuming;
	}

	public new class Instance : GameInstance
	{
		public bool ShowWorkingStatus = false;

		public HighEnergyParticleStorage storage;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			storage = master.GetComponent<HighEnergyParticleStorage>();
		}

		public void Update(float dt)
		{
			float num = storage.ConsumeAndGet(dt * base.def.activeConsumptionRate);
		}
	}

	public static Operational.Flag canConsumeParticlesFlag = new Operational.Flag("canConsumeParticles", Operational.Flag.Type.Requirement);

	public State inoperational;

	public OperationalStates operational;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = inoperational;
		root.Enter(delegate(Instance smi)
		{
			smi.GetComponent<Operational>().SetFlag(canConsumeParticlesFlag, value: false);
		});
		inoperational.EventTransition(GameHashes.OnParticleStorageChanged, operational, IsReady).ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForHighEnergyParticles);
		operational.DefaultState(operational.waiting).EventTransition(GameHashes.OnParticleStorageChanged, inoperational, GameStateMachine<ActiveParticleConsumer, Instance, IStateMachineTarget, Def>.Not(IsReady)).ToggleOperationalFlag(canConsumeParticlesFlag);
		operational.waiting.EventTransition(GameHashes.ActiveChanged, operational.consuming, (Instance smi) => smi.GetComponent<Operational>().IsActive);
		operational.consuming.EventTransition(GameHashes.ActiveChanged, operational.waiting, (Instance smi) => !smi.GetComponent<Operational>().IsActive).Update(delegate(Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_1000ms);
	}

	public bool IsReady(Instance smi)
	{
		return smi.storage.Particles >= smi.def.minParticlesForOperational;
	}
}
