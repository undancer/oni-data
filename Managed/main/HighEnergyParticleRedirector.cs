using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class HighEnergyParticleRedirector : StateMachineComponent<HighEnergyParticleRedirector.StatesInstance>, IHighEnergyParticleDirection
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, HighEnergyParticleRedirector, object>.GameInstance
	{
		public StatesInstance(HighEnergyParticleRedirector smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, HighEnergyParticleRedirector>
	{
		public State inoperational;

		public State ready;

		public State redirect;

		public State launch;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = inoperational;
			inoperational.PlayAnim("off").TagTransition(GameTags.Operational, ready);
			ready.PlayAnim("on").TagTransition(GameTags.Operational, inoperational, on_remove: true).EventTransition(GameHashes.OnParticleStorageChanged, redirect);
			redirect.PlayAnim("working_pre").QueueAnim("working_loop").QueueAnim("working_pst")
				.ScheduleGoTo((StatesInstance smi) => smi.master.directorDelay, ready)
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.LaunchParticle();
				});
		}
	}

	public static readonly HashedString PORT_ID = "HEPRedirector";

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpReq]
	private HighEnergyParticleStorage storage;

	public float directorDelay;

	[Serialize]
	private EightDirection _direction;

	private EightDirectionController directionController;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>(delegate(HighEnergyParticleRedirector component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>(delegate(HighEnergyParticleRedirector component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	private bool hasLogicWire;

	private bool isLogicActive;

	private static StatusItem infoStatusItem_Logic;

	public EightDirection Direction
	{
		get
		{
			return _direction;
		}
		set
		{
			_direction = value;
			if (directionController != null)
			{
				directionController.SetRotation(45 * EightDirectionUtil.GetDirectionIndex(_direction));
				directionController.controller.enabled = false;
				directionController.controller.enabled = true;
			}
		}
	}

	public bool AllowIncomingParticles
	{
		get
		{
			if (hasLogicWire)
			{
				if (hasLogicWire)
				{
					return isLogicActive;
				}
				return false;
			}
			return true;
		}
	}

	public bool HasLogicWire => hasLogicWire;

	public bool IsLogicActive => isLogicActive;

	private void OnCopySettings(object data)
	{
		HighEnergyParticleRedirector component = ((GameObject)data).GetComponent<HighEnergyParticleRedirector>();
		if (component != null)
		{
			Direction = component.Direction;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
		Subscribe(-801688580, OnLogicValueChangedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HighEnergyParticlePort component = GetComponent<HighEnergyParticlePort>();
		if ((bool)component)
		{
			component.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Combine(component.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(OnParticleCaptureAllowed));
		}
		if (infoStatusItem_Logic == null)
		{
			infoStatusItem_Logic = new StatusItem("HEPRedirectorLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			infoStatusItem_Logic.resolveStringCallback = ResolveInfoStatusItem;
			infoStatusItem_Logic.resolveTooltipCallback = ResolveInfoStatusItemTooltip;
		}
		selectable.AddStatusItem(infoStatusItem_Logic, this);
		directionController = new EightDirectionController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", EightDirectionController.Offset.Infront);
		Direction = Direction;
		base.smi.StartSM();
	}

	private bool OnParticleCaptureAllowed(HighEnergyParticle particle)
	{
		return AllowIncomingParticles;
	}

	private void LaunchParticle()
	{
		if (base.smi.master.storage.Particles < 1f)
		{
			base.smi.master.storage.ConsumeAll();
			return;
		}
		int highEnergyParticleOutputCell = GetComponent<Building>().GetHighEnergyParticleOutputCell();
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab("HighEnergyParticle"), Grid.CellToPosCCC(highEnergyParticleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
		gameObject.SetActive(value: true);
		if (gameObject != null)
		{
			HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
			component.payload = base.smi.master.storage.ConsumeAll();
			component.payload -= 1f;
			component.SetDirection(Direction);
			directionController.PlayAnim("redirector_send");
			directionController.controller.Queue("redirector");
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		HighEnergyParticlePort component = GetComponent<HighEnergyParticlePort>();
		if (component != null)
		{
			component.onParticleCaptureAllowed = (HighEnergyParticlePort.OnParticleCaptureAllowed)Delegate.Remove(component.onParticleCaptureAllowed, new HighEnergyParticlePort.OnParticleCaptureAllowed(OnParticleCaptureAllowed));
		}
	}

	private LogicCircuitNetwork GetNetwork()
	{
		int portCell = GetComponent<LogicPorts>().GetPortCell(PORT_ID);
		return Game.Instance.logicCircuitManager.GetNetworkForCell(portCell);
	}

	private void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == PORT_ID)
		{
			isLogicActive = logicValueChanged.newValue > 0;
			hasLogicWire = GetNetwork() != null;
		}
	}

	private static string ResolveInfoStatusItem(string format_str, object data)
	{
		HighEnergyParticleRedirector highEnergyParticleRedirector = (HighEnergyParticleRedirector)data;
		if (!highEnergyParticleRedirector.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.NORMAL;
		}
		if (highEnergyParticleRedirector.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_STANDBY;
	}

	private static string ResolveInfoStatusItemTooltip(string format_str, object data)
	{
		HighEnergyParticleRedirector highEnergyParticleRedirector = (HighEnergyParticleRedirector)data;
		if (!highEnergyParticleRedirector.HasLogicWire)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.NORMAL;
		}
		if (highEnergyParticleRedirector.IsLogicActive)
		{
			return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_ACTIVE;
		}
		return BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_STANDBY;
	}
}
