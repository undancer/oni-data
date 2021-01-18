using STRINGS;
using UnityEngine;

public class Checkpoint : StateMachineComponent<Checkpoint.SMInstance>
{
	private class CheckpointReactable : Reactable
	{
		private Checkpoint checkpoint;

		private Navigator reactor_navigator;

		private bool rotated = false;

		public CheckpointReactable(Checkpoint checkpoint)
			: base(checkpoint.gameObject, "CheckpointReactable", Db.Get().ChoreTypes.Checkpoint, 1, 1)
		{
			this.checkpoint = checkpoint;
			rotated = gameObject.GetComponent<Rotatable>().IsRotated;
			preventChoreInterruption = false;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if (reactor != null)
			{
				return false;
			}
			if (checkpoint == null)
			{
				Cleanup();
				return false;
			}
			if (!checkpoint.RedLight)
			{
				return false;
			}
			if (rotated)
			{
				return transition.x < 0;
			}
			return transition.x > 0;
		}

		protected override void InternalBegin()
		{
			reactor_navigator = reactor.GetComponent<Navigator>();
			KBatchedAnimController component = reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"), 1f);
			component.Play("idle_pre");
			component.Queue("idle_default", KAnim.PlayMode.Loop);
			checkpoint.OrphanReactable();
			checkpoint.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			if (checkpoint == null || !checkpoint.RedLight || reactor_navigator == null)
			{
				Cleanup();
				return;
			}
			reactor_navigator.AdvancePath(trigger_advance: false);
			if (!reactor_navigator.path.IsValid())
			{
				Cleanup();
				return;
			}
			NavGrid.Transition nextTransition = reactor_navigator.GetNextTransition();
			if (!(rotated ? (nextTransition.x < 0) : (nextTransition.x > 0)))
			{
				Cleanup();
			}
		}

		protected override void InternalEnd()
		{
			if (reactor != null)
			{
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim("anim_idle_distracted_kanim"));
			}
		}

		protected override void InternalCleanup()
		{
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, Checkpoint, object>.GameInstance
	{
		public SMInstance(Checkpoint master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, SMInstance, Checkpoint>
	{
		public BoolParameter redLight;

		public State stop;

		public State go;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = go;
			root.Update("RefreshLight", delegate(SMInstance smi, float dt)
			{
				smi.master.RefreshLight();
			});
			stop.ParamTransition(redLight, go, GameStateMachine<States, SMInstance, Checkpoint, object>.IsFalse).PlayAnim("red_light");
			go.ParamTransition(redLight, stop, GameStateMachine<States, SMInstance, Checkpoint, object>.IsTrue).PlayAnim("green_light");
		}
	}

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private KSelectable selectable;

	private static StatusItem infoStatusItem_Logic;

	private CheckpointReactable reactable;

	public static readonly HashedString PORT_ID = "Checkpoint";

	private bool hasLogicWire;

	private bool hasInputHigh;

	private bool redLight;

	private bool statusDirty = true;

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Checkpoint> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Checkpoint>(delegate(Checkpoint component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private bool RedLightDesiredState => hasLogicWire && !hasInputHigh && operational.IsOperational;

	public bool RedLight => redLight;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-801688580, OnLogicValueChangedDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		base.smi.StartSM();
		if (infoStatusItem_Logic == null)
		{
			infoStatusItem_Logic = new StatusItem("CheckpointLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem_Logic.resolveStringCallback = ResolveInfoStatusItem_Logic;
		}
		Refresh(redLight);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		ClearReactable();
	}

	public void RefreshLight()
	{
		if (redLight != RedLightDesiredState)
		{
			Refresh(RedLightDesiredState);
			statusDirty = true;
		}
		if (statusDirty)
		{
			RefreshStatusItem();
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		LogicPorts component = GetComponent<LogicPorts>();
		int portCell = component.GetPortCell(PORT_ID);
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		return logicCircuitManager.GetNetworkForCell(portCell);
	}

	private static string ResolveInfoStatusItem_Logic(string format_str, object data)
	{
		Checkpoint checkpoint = (Checkpoint)data;
		return checkpoint.RedLight ? BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_CLOSED : BUILDING.STATUSITEMS.CHECKPOINT.LOGIC_CONTROLLED_OPEN;
	}

	private void CreateNewReactable()
	{
		if (reactable == null)
		{
			reactable = new CheckpointReactable(this);
		}
	}

	private void OrphanReactable()
	{
		reactable = null;
	}

	private void ClearReactable()
	{
		if (reactable != null)
		{
			reactable.Cleanup();
			reactable = null;
		}
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			hasInputHigh = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
			hasLogicWire = GetNetwork() != null;
			statusDirty = true;
		}
	}

	private void OnOperationalChanged(object data)
	{
		statusDirty = true;
	}

	private void RefreshStatusItem()
	{
		bool on = operational.IsOperational && hasLogicWire;
		selectable.ToggleStatusItem(infoStatusItem_Logic, on, this);
		statusDirty = false;
	}

	private void Refresh(bool redLightState)
	{
		redLight = redLightState;
		operational.SetActive(operational.IsOperational && redLight);
		base.smi.sm.redLight.Set(redLight, base.smi);
		if (redLight)
		{
			CreateNewReactable();
		}
		else
		{
			ClearReactable();
		}
	}
}
