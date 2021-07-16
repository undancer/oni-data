using System;
using System.Collections.Generic;
using KSerialization;

public class GameplayEventManager : KMonoBehaviour
{
	public static GameplayEventManager Instance;

	public Notifier notifier;

	[Serialize]
	private List<GameplayEventInstance> activeEvents = new List<GameplayEventInstance>();

	[Serialize]
	private Dictionary<HashedString, int> pastEvents = new Dictionary<HashedString, int>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		notifier = GetComponent<Notifier>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.ScheduleNextFrame("GameplayEventManager", delegate
		{
			RestoreEvents();
		});
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}

	private void RestoreEvents()
	{
		activeEvents.RemoveAll((GameplayEventInstance x) => Db.Get().GameplayEvents.TryGet(x.eventID) == null);
		foreach (GameplayEventInstance activeEvent in activeEvents)
		{
			StartEventInstance(activeEvent);
		}
	}

	public bool IsGameplayEventActive(GameplayEvent eventType)
	{
		return activeEvents.Find((GameplayEventInstance e) => e.eventID == eventType.IdHash) != null;
	}

	public bool IsGameplayEventRunningWithTag(Tag tag)
	{
		foreach (GameplayEventInstance activeEvent in activeEvents)
		{
			if (activeEvent.tags.Contains(tag))
			{
				return true;
			}
		}
		return false;
	}

	public void GetActiveEventsOfType<T>(int worldID, ref List<GameplayEventInstance> results) where T : GameplayEvent
	{
		foreach (GameplayEventInstance activeEvent in activeEvents)
		{
			if (activeEvent.worldId == worldID && activeEvent.gameplayEvent as T != null)
			{
				results.Add(activeEvent);
			}
		}
	}

	private GameplayEventInstance CreateGameplayEvent(GameplayEvent gameplayEvent, int worldId)
	{
		return gameplayEvent.CreateInstance(worldId);
	}

	public GameplayEventInstance GetGameplayEventInstance(HashedString eventID)
	{
		return activeEvents.Find((GameplayEventInstance e) => e.eventID == eventID);
	}

	public GameplayEventInstance StartNewEvent(GameplayEvent eventType, int worldId = -1)
	{
		GameplayEventInstance gameplayEventInstance = CreateGameplayEvent(eventType, worldId);
		StartEventInstance(gameplayEventInstance);
		activeEvents.Add(gameplayEventInstance);
		pastEvents.TryGetValue(gameplayEventInstance.eventID, out var value);
		pastEvents[gameplayEventInstance.eventID] = value + 1;
		return gameplayEventInstance;
	}

	private void StartEventInstance(GameplayEventInstance gameplayEventInstance)
	{
		StateMachine.Instance instance = gameplayEventInstance.PrepareEvent(this);
		instance.OnStop = (Action<string, StateMachine.Status>)Delegate.Combine(instance.OnStop, (Action<string, StateMachine.Status>)delegate
		{
			activeEvents.Remove(gameplayEventInstance);
		});
		gameplayEventInstance.StartEvent();
	}

	public int NumberOfPastEvents(HashedString eventID)
	{
		pastEvents.TryGetValue(eventID, out var value);
		return value;
	}
}
