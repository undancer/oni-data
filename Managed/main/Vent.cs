using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Vent")]
public class Vent : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public enum State
	{
		Invalid,
		Ready,
		Blocked,
		OverPressure,
		Closed
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, Vent, object>.GameInstance
	{
		private Exhaust exhaust;

		public StatesInstance(Vent master)
			: base(master)
		{
			exhaust = master.GetComponent<Exhaust>();
		}

		public bool NeedsExhaust()
		{
			if (exhaust != null && base.master.GetEndPointState() != State.Ready)
			{
				return base.master.endpointType == Endpoint.Source;
			}
			return false;
		}

		public bool Blocked()
		{
			if (base.master.GetEndPointState() == State.Blocked)
			{
				return base.master.endpointType != Endpoint.Source;
			}
			return false;
		}

		public bool OverPressure()
		{
			if (exhaust != null && base.master.GetEndPointState() == State.OverPressure)
			{
				return base.master.endpointType != Endpoint.Source;
			}
			return false;
		}

		public void CheckTransitions()
		{
			if (NeedsExhaust())
			{
				base.smi.GoTo(base.sm.needExhaust);
			}
			else if (base.master.Closed())
			{
				base.smi.GoTo(base.sm.closed);
			}
			else if (Blocked())
			{
				base.smi.GoTo(base.sm.open.blocked);
			}
			else if (OverPressure())
			{
				base.smi.GoTo(base.sm.open.overPressure);
			}
			else
			{
				base.smi.GoTo(base.sm.open.idle);
			}
		}

		public StatusItem SelectStatusItem(StatusItem gas_status_item, StatusItem liquid_status_item)
		{
			if (base.master.conduitType != ConduitType.Gas)
			{
				return liquid_status_item;
			}
			return gas_status_item;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Vent>
	{
		public class OpenState : State
		{
			public State idle;

			public State blocked;

			public State overPressure;
		}

		public OpenState open;

		public State closed;

		public State needExhaust;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = open.idle;
			root.Update("CheckTransitions", delegate(StatesInstance smi, float dt)
			{
				smi.CheckTransitions();
			});
			open.TriggerOnEnter(GameHashes.VentOpen);
			closed.TriggerOnEnter(GameHashes.VentClosed);
			open.blocked.ToggleStatusItem((StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentObstructed, Db.Get().BuildingStatusItems.LiquidVentObstructed));
			open.overPressure.ToggleStatusItem((StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure, Db.Get().BuildingStatusItems.LiquidVentOverPressure));
		}
	}

	private int cell = -1;

	private int sortKey;

	[Serialize]
	public Dictionary<SimHashes, float> lifeTimeVentMass = new Dictionary<SimHashes, float>();

	private StatesInstance smi;

	[SerializeField]
	public ConduitType conduitType = ConduitType.Gas;

	[SerializeField]
	public Endpoint endpointType;

	[SerializeField]
	public float overpressureMass = 1f;

	[NonSerialized]
	public bool showConnectivityIcons = true;

	[NonSerialized]
	[MyCmpGet]
	public Structure structure;

	[NonSerialized]
	[MyCmpGet]
	public Operational operational;

	public int SortKey
	{
		get
		{
			return sortKey;
		}
		set
		{
			sortKey = value;
		}
	}

	public bool IsBlocked => GetEndPointState() != State.Ready;

	public void UpdateVentedMass(SimHashes element, float mass)
	{
		if (!lifeTimeVentMass.ContainsKey(element))
		{
			lifeTimeVentMass.Add(element, mass);
		}
		else
		{
			lifeTimeVentMass[element] += mass;
		}
	}

	public float GetVentedMass(SimHashes element)
	{
		if (lifeTimeVentMass.ContainsKey(element))
		{
			return lifeTimeVentMass[element];
		}
		return 0f;
	}

	public bool Closed()
	{
		bool value = false;
		if (operational.Flags.TryGetValue(LogicOperationalController.LogicOperationalFlag, out value) && !value)
		{
			return true;
		}
		if (operational.Flags.TryGetValue(BuildingEnabledButton.EnabledFlag, out value) && !value)
		{
			return true;
		}
		return false;
	}

	protected override void OnSpawn()
	{
		Building component = GetComponent<Building>();
		cell = component.GetUtilityOutputCell();
		smi = new StatesInstance(this);
		smi.StartSM();
	}

	public State GetEndPointState()
	{
		State result = State.Invalid;
		switch (endpointType)
		{
		case Endpoint.Source:
			result = (IsConnected() ? State.Ready : State.Blocked);
			break;
		case Endpoint.Sink:
		{
			result = State.Ready;
			int num = cell;
			if (!IsValidOutputCell(num))
			{
				result = (Grid.Solid[num] ? State.Blocked : State.OverPressure);
			}
			break;
		}
		}
		return result;
	}

	public bool IsConnected()
	{
		UtilityNetwork networkForCell = Conduit.GetNetworkManager(conduitType).GetNetworkForCell(cell);
		if (networkForCell != null)
		{
			return (networkForCell as FlowUtilityNetwork).HasSinks;
		}
		return false;
	}

	private bool IsValidOutputCell(int output_cell)
	{
		bool result = false;
		if ((structure == null || !structure.IsEntombed() || !Closed()) && !Grid.Solid[output_cell])
		{
			result = Grid.Mass[output_cell] < overpressureMass;
		}
		return result;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		string formattedMass = GameUtil.GetFormattedMass(overpressureMass);
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVER_PRESSURE_MASS, formattedMass), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.OVER_PRESSURE_MASS, formattedMass))
		};
	}
}
