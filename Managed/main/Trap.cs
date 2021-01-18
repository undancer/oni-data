using KSerialization;
using STRINGS;
using UnityEngine;

public class Trap : StateMachineComponent<Trap.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Trap, object>.GameInstance
	{
		public StatesInstance(Trap master)
			: base(master)
		{
		}

		public void OnTrapTriggered(object data)
		{
			GameObject gameObject = (GameObject)data;
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			base.master.contents.Set(component);
			base.smi.sm.trapTriggered.Trigger(base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Trap>
	{
		public class OccupiedStates : State
		{
			public State idle;
		}

		public State ready;

		public State trapping;

		public State finishedUsing;

		public State destroySelf;

		public Signal trapTriggered;

		public OccupiedStates occupied;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = ready;
			base.serializable = SerializeType.Never;
			CreateStatusItems();
			ready.EventHandler(GameHashes.TrapTriggered, delegate(StatesInstance smi, object data)
			{
				smi.OnTrapTriggered(data);
			}).OnSignal(trapTriggered, trapping).ToggleStatusItem(statusReady);
			trapping.PlayAnim("working_pre").OnAnimQueueComplete(occupied);
			occupied.ToggleTag(GameTags.Trapped).ToggleStatusItem(statusSprung, (StatesInstance smi) => smi).DefaultState(occupied.idle)
				.EventTransition(GameHashes.OnStorageChange, finishedUsing, (StatesInstance smi) => smi.master.GetComponent<Storage>().IsEmpty());
			occupied.idle.PlayAnim("working_loop", KAnim.PlayMode.Loop);
			finishedUsing.PlayAnim("working_pst").OnAnimQueueComplete(destroySelf);
			destroySelf.Enter(delegate(StatesInstance smi)
			{
				Util.KDestroyGameObject(smi.master.gameObject);
			});
		}
	}

	[Serialize]
	private Ref<KPrefabID> contents;

	public TagSet captureTags = new TagSet();

	private static StatusItem statusReady;

	private static StatusItem statusSprung;

	private static void CreateStatusItems()
	{
		if (statusSprung == null)
		{
			statusReady = new StatusItem("Ready", BUILDING.STATUSITEMS.CREATURE_TRAP.READY.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.READY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			statusSprung = new StatusItem("Sprung", BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.NAME, BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID);
			statusSprung.resolveTooltipCallback = delegate(string str, object obj)
			{
				StatesInstance statesInstance = (StatesInstance)obj;
				return string.Format(str, statesInstance.master.contents.Get().GetProperName());
			};
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		contents = new Ref<KPrefabID>();
		CreateStatusItems();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Storage component = GetComponent<Storage>();
		base.smi.StartSM();
		if (!component.IsEmpty())
		{
			KPrefabID component2 = component.items[0].GetComponent<KPrefabID>();
			if (component2 != null)
			{
				contents.Set(component2);
				base.smi.GoTo(base.smi.sm.occupied);
			}
			else
			{
				component.DropAll();
			}
		}
	}
}
