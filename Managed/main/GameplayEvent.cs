using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{base.Id}")]
public abstract class GameplayEvent : Resource, IComparable<GameplayEvent>
{
	public enum Occurance
	{
		Once,
		Infinity
	}

	public const int INFINITE = -1;

	public int numTimesAllowed = -1;

	public bool allowMultipleEventInstances;

	public int durration;

	public int warning;

	protected int basePriority;

	protected int calculatedPriority;

	public List<GameplayEventPrecondition> preconditions;

	public List<GameplayEventMinionFilter> minionFilters;

	public List<HashedString> successEvents;

	public List<HashedString> failureEvents;

	public string popupTitle;

	public string popupDescription;

	public HashedString popupAnimFileName;

	public HashedString popupBackgroundFileName;

	public Color32 popupBackgroundTint;

	public List<Tag> tags;

	public int importance { get; private set; }

	public virtual bool IsAllowed()
	{
		if (WillNeverRunAgain())
		{
			return false;
		}
		if (!allowMultipleEventInstances && GameplayEventManager.Instance.IsGameplayEventActive(this))
		{
			return false;
		}
		foreach (GameplayEventPrecondition precondition in preconditions)
		{
			if (precondition.required && !precondition.condition())
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool WillNeverRunAgain()
	{
		if (numTimesAllowed != -1)
		{
			return GameplayEventManager.Instance.NumberOfPastEvents(Id) >= numTimesAllowed;
		}
		return false;
	}

	public int GetCashedPriority()
	{
		return calculatedPriority;
	}

	public virtual int CalculatePriority()
	{
		calculatedPriority = basePriority + CalculateBoost();
		return calculatedPriority;
	}

	public int CalculateBoost()
	{
		int num = 0;
		foreach (GameplayEventPrecondition precondition in preconditions)
		{
			if (!precondition.required && precondition.condition())
			{
				num += precondition.priorityModifier;
			}
		}
		return num;
	}

	public GameplayEvent AddPrecondition(GameplayEventPrecondition precondition)
	{
		precondition.required = true;
		preconditions.Add(precondition);
		return this;
	}

	public GameplayEvent AddPriorityBoost(GameplayEventPrecondition precondition, int priorityBoost)
	{
		precondition.required = false;
		precondition.priorityModifier = priorityBoost;
		preconditions.Add(precondition);
		return this;
	}

	public GameplayEvent AddMinionFilter(GameplayEventMinionFilter filter)
	{
		minionFilters.Add(filter);
		return this;
	}

	public GameplayEvent TrySpawnEventOnSuccess(HashedString evt)
	{
		successEvents.Add(evt);
		return this;
	}

	public GameplayEvent TrySpawnEventOnFailure(HashedString evt)
	{
		failureEvents.Add(evt);
		return this;
	}

	public GameplayEvent SetVisuals(HashedString backgroundSpriteName, HashedString animFileName, Color32 tint)
	{
		popupAnimFileName = animFileName;
		popupBackgroundFileName = backgroundSpriteName;
		popupBackgroundTint = tint;
		return this;
	}

	public GameplayEvent SetVisuals(HashedString backgroundSpriteName, HashedString animFileName)
	{
		popupAnimFileName = animFileName;
		popupBackgroundFileName = backgroundSpriteName;
		return this;
	}

	public virtual Sprite GetDisplaySprite()
	{
		return null;
	}

	public virtual string GetDisplayString()
	{
		return null;
	}

	public MinionIdentity GetRandomFilteredMinion()
	{
		List<MinionIdentity> list = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		foreach (GameplayEventMinionFilter filter in minionFilters)
		{
			list.RemoveAll((MinionIdentity x) => !filter.filter(x));
		}
		if (list.Count != 0)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	public MinionIdentity GetRandomMinionPrioritizeFiltered()
	{
		MinionIdentity randomFilteredMinion = GetRandomFilteredMinion();
		if (!(randomFilteredMinion == null))
		{
			return randomFilteredMinion;
		}
		return Components.LiveMinionIdentities.Items[UnityEngine.Random.Range(0, Components.LiveMinionIdentities.Items.Count)];
	}

	public int CompareTo(GameplayEvent other)
	{
		return -GetCashedPriority().CompareTo(other.GetCashedPriority());
	}

	public GameplayEvent(string id, int priority, int importance)
		: base(id)
	{
		tags = new List<Tag>();
		basePriority = priority;
		preconditions = new List<GameplayEventPrecondition>();
		minionFilters = new List<GameplayEventMinionFilter>();
		successEvents = new List<HashedString>();
		failureEvents = new List<HashedString>();
		this.importance = importance;
		popupAnimFileName = id;
	}

	public abstract StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance);

	public GameplayEventInstance CreateInstance(int worldId)
	{
		GameplayEventInstance gameplayEventInstance = new GameplayEventInstance(this, worldId);
		if (tags != null)
		{
			gameplayEventInstance.tags.AddRange(tags);
		}
		return gameplayEventInstance;
	}
}
public abstract class GameplayEvent<StateMachineInstanceType> : GameplayEvent where StateMachineInstanceType : StateMachine.Instance
{
	public GameplayEvent(string id, int priority = 0, int importance = 0)
		: base(id, priority, importance)
	{
	}
}
