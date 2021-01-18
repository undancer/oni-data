using KSerialization;
using STRINGS;
using UnityEngine;

public class Trap : StateMachineComponent<Trap.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Trap, object>.GameInstance
	{
		private HandleVector<int>.Handle partitionerEntry;

		public StatesInstance(Trap master)
			: base(master)
		{
			partitionerEntry = GameScenePartitioner.Instance.Add("Trap", base.gameObject, Grid.PosToCell(base.gameObject), GameScenePartitioner.Instance.trapsLayer, OnCreatureOnTrap);
		}

		public void OnCreatureOnTrap(object data)
		{
			Storage component = base.master.GetComponent<Storage>();
			if (!component.IsEmpty())
			{
				return;
			}
			Trappable trappable = (Trappable)data;
			if (trappable.HasTag(GameTags.Stored) || trappable.HasTag(GameTags.Trapped) || trappable.HasTag(GameTags.Creatures.Bagged))
			{
				return;
			}
			bool flag = false;
			Tag[] trappableCreatures = base.master.trappableCreatures;
			foreach (Tag tag in trappableCreatures)
			{
				if (trappable.HasTag(tag))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				KPrefabID component2 = trappable.GetComponent<KPrefabID>();
				base.master.contents.Set(component2);
				component.Store(trappable.gameObject, hide_popups: true);
				base.master.SetStoredPosition(trappable.gameObject);
				base.smi.sm.trapTriggered.Trigger(base.smi);
			}
		}

		public override void StopSM(string reason)
		{
			DisableEvents();
			base.StopSM(reason);
		}

		public void DisableEvents()
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry);
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
			base.serializable = false;
			CreateStatusItems();
			ready.OnSignal(trapTriggered, trapping).ToggleStatusItem(statusReady).Exit(delegate(StatesInstance smi)
			{
				smi.DisableEvents();
			});
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

	public Tag[] trappableCreatures;

	public Vector2 trappedOffset = Vector2.zero;

	[Serialize]
	private Ref<KPrefabID> contents;

	public TagSet captureTags = new TagSet();

	private static StatusItem statusReady;

	private static StatusItem statusSprung;

	private void SetStoredPosition(GameObject go)
	{
		Vector3 position = Grid.CellToPosCBC(Grid.PosToCell(base.transform.GetPosition()), Grid.SceneLayer.BuildingBack);
		position.x += trappedOffset.x;
		position.y += trappedOffset.y;
		go.transform.SetPosition(position);
		go.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingBack);
	}

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
		foreach (GameObject item in component.items)
		{
			SetStoredPosition(item);
			KBoxCollider2D component2 = item.GetComponent<KBoxCollider2D>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
		}
		base.smi.StartSM();
		if (!component.IsEmpty())
		{
			KPrefabID component3 = component.items[0].GetComponent<KPrefabID>();
			if (component3 != null)
			{
				contents.Set(component3);
				base.smi.GoTo(base.smi.sm.occupied);
			}
			else
			{
				component.DropAll();
			}
		}
	}

	public KPrefabID GetContents()
	{
		return contents.Get();
	}
}
