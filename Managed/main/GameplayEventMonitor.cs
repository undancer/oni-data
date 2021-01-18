using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayEventMonitor : GameStateMachine<GameplayEventMonitor, GameplayEventMonitor.Instance, IStateMachineTarget, GameplayEventMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public class ActiveState : State
	{
		public State unseenEvents;

		public State seenAllEvents;
	}

	public new class Instance : GameInstance
	{
		public List<GameplayEventInstance> events = new List<GameplayEventInstance>();

		public GameplayEventFX.Instance fx;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this);
		}

		public void OnMonitorStart(object data)
		{
			GameplayEventInstance gameplayEventInstance = data as GameplayEventInstance;
			if (!events.Contains(gameplayEventInstance))
			{
				events.Add(gameplayEventInstance);
				gameplayEventInstance.RegisterMonitorCallback(base.gameObject);
			}
			base.smi.sm.UpdateFX(base.smi);
			base.smi.sm.UpdateEventDisplay(base.smi);
		}

		public void OnMonitorEnd(object data)
		{
			GameplayEventInstance gameplayEventInstance = data as GameplayEventInstance;
			if (events.Contains(gameplayEventInstance))
			{
				events.Remove(gameplayEventInstance);
				gameplayEventInstance.UnregisterMonitorCallback(base.gameObject);
			}
			base.smi.sm.UpdateFX(base.smi);
			base.smi.sm.UpdateEventDisplay(base.smi);
			if (events.Count == 0)
			{
				base.smi.GoTo(base.sm.idle);
			}
		}

		public void OnSelect(object data)
		{
			if (!(bool)data)
			{
				return;
			}
			foreach (GameplayEventInstance @event in events)
			{
				if (!@event.seenNotification && @event.GetEventPopupData != null)
				{
					@event.seenNotification = true;
					GameplayEventInstance.ShowEventPopup(@event.GetEventPopupData());
					break;
				}
			}
			if (UnseenCount() == 0)
			{
				base.smi.GoTo(base.sm.activeState.seenAllEvents);
			}
		}

		public int UnseenCount()
		{
			return events.Count((GameplayEventInstance evt) => !evt.seenNotification);
		}
	}

	public State idle;

	public ActiveState activeState;

	public override void InitializeStates(out BaseState default_state)
	{
		base.InitializeStates(out default_state);
		default_state = idle;
		root.EventHandler(GameHashes.GameplayEventMonitorStart, delegate(Instance smi, object data)
		{
			smi.OnMonitorStart(data);
		}).EventHandler(GameHashes.GameplayEventMonitorEnd, delegate(Instance smi, object data)
		{
			smi.OnMonitorEnd(data);
		}).EventHandler(GameHashes.GameplayEventMonitorChanged, delegate(Instance smi, object data)
		{
			UpdateFX(smi);
		});
		idle.EventTransition(GameHashes.GameplayEventMonitorStart, activeState, HasEvents).Enter(UpdateEventDisplay);
		activeState.DefaultState(activeState.unseenEvents);
		activeState.unseenEvents.ToggleFX(CreateFX).EventHandler(GameHashes.SelectObject, delegate(Instance smi, object data)
		{
			smi.OnSelect(data);
		}).EventTransition(GameHashes.GameplayEventMonitorChanged, activeState.seenAllEvents, SeenAll);
		activeState.seenAllEvents.EventTransition(GameHashes.GameplayEventMonitorStart, activeState.unseenEvents, GameStateMachine<GameplayEventMonitor, Instance, IStateMachineTarget, Def>.Not(SeenAll)).Enter(UpdateEventDisplay);
	}

	private bool HasEvents(Instance smi)
	{
		return smi.events.Count > 0;
	}

	private bool SeenAll(Instance smi)
	{
		return smi.UnseenCount() == 0;
	}

	private void UpdateFX(Instance smi)
	{
		if (smi.fx != null)
		{
			smi.fx.sm.notificationCount.Set(smi.UnseenCount(), smi.fx);
		}
	}

	private GameplayEventFX.Instance CreateFX(Instance smi)
	{
		if (!smi.isMasterNull)
		{
			smi.fx = new GameplayEventFX.Instance(smi.master, new Vector3(0f, 0f, -0.1f));
			return smi.fx;
		}
		return null;
	}

	public void UpdateEventDisplay(Instance smi)
	{
		if (smi.events.Count == 0 || smi.UnseenCount() > 0)
		{
			NameDisplayScreen.Instance.SetGameplayEventDisplay(smi.master.gameObject, bVisible: false, null, null);
			return;
		}
		int num = -1;
		GameplayEvent gameplayEvent = null;
		foreach (GameplayEventInstance @event in smi.events)
		{
			Sprite displaySprite = @event.gameplayEvent.GetDisplaySprite();
			if (@event.gameplayEvent.importance > num && displaySprite != null)
			{
				num = @event.gameplayEvent.importance;
				gameplayEvent = @event.gameplayEvent;
			}
		}
		if (gameplayEvent != null)
		{
			NameDisplayScreen.Instance.SetGameplayEventDisplay(smi.master.gameObject, bVisible: true, gameplayEvent.GetDisplayString(), gameplayEvent.GetDisplaySprite());
		}
	}
}
