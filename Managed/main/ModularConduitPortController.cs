using System.Collections.Generic;
using KSerialization;
using STRINGS;

public class ModularConduitPortController : GameStateMachine<ModularConduitPortController, ModularConduitPortController.Instance, IStateMachineTarget, ModularConduitPortController.Def>
{
	public class Def : BaseDef
	{
	}

	public enum Mode
	{
		Unload,
		Both,
		Load
	}

	private class OnStates : State
	{
		public State idle;

		public State unloading;

		public State unloading_pst;

		public State loading;

		public State loading_pst;

		public State loaded;

		public State loaded_pst;
	}

	public new class Instance : GameInstance, INToggleSideScreenControl
	{
		[Serialize]
		private Mode selectedMode = Mode.Both;

		public Mode SelectedMode => selectedMode;

		public string SidescreenTitleKey => "STRINGS.UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TITLE";

		public List<LocString> Options => UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.LABELS;

		public List<LocString> Tooltips => UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TOOLTIPS;

		public string Description => UI.UISIDESCREENS.MODULAR_CONDUIT_PORT_SIDE_SCREEN.TOOLTIPS[(int)selectedMode];

		public int SelectedOption => (int)selectedMode;

		public int QueuedOption => (int)selectedMode;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public ConduitType GetConduitType()
		{
			return GetComponent<IConduitConsumer>().ConduitType;
		}

		public void SetUnloading(bool isUnloading)
		{
			base.sm.isUnloading.Set(isUnloading, this);
		}

		public void SetLoading(bool isLoading)
		{
			base.sm.isLoading.Set(isLoading, this);
		}

		public void SetRocket(bool hasRocket)
		{
			base.sm.hasRocket.Set(hasRocket, this);
		}

		public void QueueSelectedOption(int option)
		{
			selectedMode = (Mode)option;
		}
	}

	private State off;

	private OnStates on;

	public BoolParameter isUnloading;

	public BoolParameter isLoading;

	public BoolParameter hasRocket;

	private static StatusItem idleStatusItem;

	private static StatusItem unloadingStatusItem;

	private static StatusItem loadingStatusItem;

	private static StatusItem loadedStatusItem;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = off;
		InitializeStatusItems();
		off.PlayAnim("off", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, on, (Instance smi) => smi.GetComponent<Operational>().IsOperational);
		on.DefaultState(on.idle).EventTransition(GameHashes.OperationalChanged, off, (Instance smi) => !smi.GetComponent<Operational>().IsOperational);
		on.idle.PlayAnim("idle", KAnim.PlayMode.Loop).ParamTransition(isUnloading, on.unloading, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsTrue).ParamTransition(isLoading, on.loading, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsTrue)
			.ParamTransition(hasRocket, on.loaded, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsTrue)
			.ToggleStatusItem(idleStatusItem);
		on.unloading.PlayAnim("unloading_pre").QueueAnim("unloading_loop", loop: true).ParamTransition(isUnloading, on.unloading_pst, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsFalse)
			.ToggleStatusItem(unloadingStatusItem);
		on.unloading_pst.PlayAnim("unloading_pst").OnAnimQueueComplete(on.idle);
		on.loading.PlayAnim("loading_pre").QueueAnim("loading_loop", loop: true).ParamTransition(isLoading, on.loading_pst, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsFalse)
			.ToggleStatusItem(loadingStatusItem);
		on.loading_pst.PlayAnim("loading_pst").OnAnimQueueComplete(on.idle);
		on.loaded.PlayAnim("finished", KAnim.PlayMode.Loop).ParamTransition(hasRocket, on.idle, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsFalse).ParamTransition(isUnloading, on.idle, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsTrue)
			.ParamTransition(isLoading, on.idle, GameStateMachine<ModularConduitPortController, Instance, IStateMachineTarget, Def>.IsTrue)
			.ToggleStatusItem(loadedStatusItem);
	}

	private static void InitializeStatusItems()
	{
		if (idleStatusItem == null)
		{
			idleStatusItem = new StatusItem("ROCKET_PORT_IDLE", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			unloadingStatusItem = new StatusItem("ROCKET_PORT_UNLOADING", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			loadingStatusItem = new StatusItem("ROCKET_PORT_LOADING", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			loadedStatusItem = new StatusItem("ROCKET_PORT_LOADED", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
		}
	}
}
