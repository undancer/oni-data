using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class ReceptacleMonitor : StateMachineComponent<ReceptacleMonitor.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, ReceptacleMonitor, object>.GameInstance
	{
		public StatesInstance(ReceptacleMonitor master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, ReceptacleMonitor>
	{
		public ObjectParameter<SingleEntityReceptacle> receptacle;

		public State wild;

		public State inoperational;

		public State operational;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = wild;
			base.serializable = true;
			wild.TriggerOnEnter(GameHashes.ReceptacleOperational);
			inoperational.TriggerOnEnter(GameHashes.ReceptacleInoperational);
			operational.TriggerOnEnter(GameHashes.ReceptacleOperational);
		}
	}

	private bool replanted;

	public bool Replanted => replanted;

	WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[1]
	{
		WiltCondition.Condition.Receptacle
	};

	public string WiltStateString
	{
		get
		{
			string text = "";
			if (base.smi.IsInsideState(base.smi.sm.inoperational))
			{
				text += CREATURES.STATUSITEMS.RECEPTACLEINOPERATIONAL.NAME;
			}
			return text;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public PlantablePlot GetReceptacle()
	{
		return (PlantablePlot)base.smi.sm.receptacle.Get(base.smi);
	}

	public void SetReceptacle(PlantablePlot plot = null)
	{
		if (plot == null)
		{
			base.smi.sm.receptacle.Set(null, base.smi);
			replanted = false;
		}
		else
		{
			base.smi.sm.receptacle.Set(plot, base.smi);
			replanted = true;
		}
	}

	public void Sim1000ms(float dt)
	{
		if (base.smi.sm.receptacle.Get(base.smi) == null)
		{
			base.smi.GoTo(base.smi.sm.wild);
			return;
		}
		Operational component = base.smi.sm.receptacle.Get(base.smi).GetComponent<Operational>();
		if (component == null)
		{
			base.smi.GoTo(base.smi.sm.operational);
		}
		else if (component.IsOperational)
		{
			base.smi.GoTo(base.smi.sm.operational);
		}
		else
		{
			base.smi.GoTo(base.smi.sm.inoperational);
		}
	}

	public bool HasReceptacle()
	{
		return !base.smi.IsInsideState(base.smi.sm.wild);
	}

	public bool HasOperationalReceptacle()
	{
		return base.smi.IsInsideState(base.smi.sm.operational);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_RECEPTACLE, UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_RECEPTACLE, Descriptor.DescriptorType.Requirement)
		};
	}
}
