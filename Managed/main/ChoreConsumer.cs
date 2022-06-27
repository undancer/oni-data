using System;
using System.Collections.Generic;
using System.Diagnostics;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ChoreConsumer")]
public class ChoreConsumer : KMonoBehaviour, IPersonalPriorityManager
{
	private struct BehaviourPrecondition
	{
		public Func<object, bool> cb;

		public object arg;
	}

	public class PreconditionSnapshot
	{
		public List<Chore.Precondition.Context> succeededContexts = new List<Chore.Precondition.Context>();

		public List<Chore.Precondition.Context> failedContexts = new List<Chore.Precondition.Context>();

		public bool doFailedContextsNeedSorting = true;

		public void CopyTo(PreconditionSnapshot snapshot)
		{
			snapshot.Clear();
			snapshot.succeededContexts.AddRange(succeededContexts);
			snapshot.failedContexts.AddRange(failedContexts);
			snapshot.doFailedContextsNeedSorting = true;
		}

		public void Clear()
		{
			succeededContexts.Clear();
			failedContexts.Clear();
			doFailedContextsNeedSorting = true;
		}
	}

	public struct PriorityInfo
	{
		public int priority;
	}

	public const int DEFAULT_PERSONAL_CHORE_PRIORITY = 3;

	public const int MIN_PERSONAL_PRIORITY = 0;

	public const int MAX_PERSONAL_PRIORITY = 5;

	public const int PRIORITY_DISABLED = 0;

	public const int PRIORITY_VERYLOW = 1;

	public const int PRIORITY_LOW = 2;

	public const int PRIORITY_FLAT = 3;

	public const int PRIORITY_HIGH = 4;

	public const int PRIORITY_VERYHIGH = 5;

	[MyCmpAdd]
	public ChoreProvider choreProvider;

	[MyCmpAdd]
	public ChoreDriver choreDriver;

	[MyCmpGet]
	public Navigator navigator;

	[MyCmpGet]
	public MinionResume resume;

	[MyCmpAdd]
	private User user;

	public System.Action choreRulesChanged;

	public bool debug;

	private List<ChoreProvider> providers = new List<ChoreProvider>();

	private List<Urge> urges = new List<Urge>();

	public ChoreTable choreTable;

	private ChoreTable.Instance choreTableInstance;

	public ChoreConsumerState consumerState;

	private Dictionary<Tag, BehaviourPrecondition> behaviourPreconditions = new Dictionary<Tag, BehaviourPrecondition>();

	private PreconditionSnapshot preconditionSnapshot = new PreconditionSnapshot();

	private PreconditionSnapshot lastSuccessfulPreconditionSnapshot = new PreconditionSnapshot();

	[Serialize]
	private Dictionary<HashedString, PriorityInfo> choreGroupPriorities = new Dictionary<HashedString, PriorityInfo>();

	private Dictionary<HashedString, int> choreTypePriorities = new Dictionary<HashedString, int>();

	private List<HashedString> traitDisabledChoreGroups = new List<HashedString>();

	private List<HashedString> userDisabledChoreGroups = new List<HashedString>();

	private int stationaryReach = -1;

	public List<ChoreProvider> GetProviders()
	{
		return providers;
	}

	public PreconditionSnapshot GetLastPreconditionSnapshot()
	{
		return preconditionSnapshot;
	}

	public List<Chore.Precondition.Context> GetSuceededPreconditionContexts()
	{
		return lastSuccessfulPreconditionSnapshot.succeededContexts;
	}

	public List<Chore.Precondition.Context> GetFailedPreconditionContexts()
	{
		return lastSuccessfulPreconditionSnapshot.failedContexts;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (ChoreGroupManager.instance != null)
		{
			foreach (KeyValuePair<Tag, int> item in ChoreGroupManager.instance.DefaultChorePermission)
			{
				bool flag = false;
				foreach (HashedString userDisabledChoreGroup in userDisabledChoreGroups)
				{
					if (userDisabledChoreGroup.HashValue == item.Key.GetHashCode())
					{
						flag = true;
						break;
					}
				}
				if (!flag && item.Value == 0)
				{
					userDisabledChoreGroups.Add(new HashedString(item.Key.GetHashCode()));
				}
			}
		}
		providers.Add(choreProvider);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KPrefabID component = GetComponent<KPrefabID>();
		if (choreTable != null)
		{
			choreTableInstance = new ChoreTable.Instance(choreTable, component);
		}
		foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
		{
			int personalPriority = GetPersonalPriority(resource);
			UpdateChoreTypePriorities(resource, personalPriority);
			SetPermittedByUser(resource, personalPriority != 0);
		}
		consumerState = new ChoreConsumerState(this);
	}

	protected override void OnForcedCleanUp()
	{
		if (consumerState != null)
		{
			consumerState.navigator = null;
		}
		navigator = null;
		base.OnForcedCleanUp();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (choreTableInstance != null)
		{
			choreTableInstance.OnCleanUp(GetComponent<KPrefabID>());
			choreTableInstance = null;
		}
	}

	public bool IsPermittedByUser(ChoreGroup chore_group)
	{
		if (chore_group != null)
		{
			return !userDisabledChoreGroups.Contains(chore_group.IdHash);
		}
		return true;
	}

	public void SetPermittedByUser(ChoreGroup chore_group, bool is_allowed)
	{
		if (is_allowed)
		{
			if (userDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				choreRulesChanged.Signal();
			}
		}
		else if (!userDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			userDisabledChoreGroups.Add(chore_group.IdHash);
			choreRulesChanged.Signal();
		}
	}

	public bool IsPermittedByTraits(ChoreGroup chore_group)
	{
		if (chore_group != null)
		{
			return !traitDisabledChoreGroups.Contains(chore_group.IdHash);
		}
		return true;
	}

	public void SetPermittedByTraits(ChoreGroup chore_group, bool is_enabled)
	{
		if (is_enabled)
		{
			if (traitDisabledChoreGroups.Remove(chore_group.IdHash))
			{
				choreRulesChanged.Signal();
			}
		}
		else if (!traitDisabledChoreGroups.Contains(chore_group.IdHash))
		{
			traitDisabledChoreGroups.Add(chore_group.IdHash);
			choreRulesChanged.Signal();
		}
	}

	private bool ChooseChore(ref Chore.Precondition.Context out_context, List<Chore.Precondition.Context> succeeded_contexts)
	{
		if (succeeded_contexts.Count == 0)
		{
			return false;
		}
		Chore currentChore = choreDriver.GetCurrentChore();
		if (currentChore == null)
		{
			for (int num = succeeded_contexts.Count - 1; num >= 0; num--)
			{
				Chore.Precondition.Context context = succeeded_contexts[num];
				if (context.IsSuccess())
				{
					out_context = context;
					return true;
				}
			}
		}
		else
		{
			int interruptPriority = Db.Get().ChoreTypes.TopPriority.interruptPriority;
			int num2 = ((currentChore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority) ? interruptPriority : currentChore.choreType.interruptPriority);
			for (int num3 = succeeded_contexts.Count - 1; num3 >= 0; num3--)
			{
				Chore.Precondition.Context context2 = succeeded_contexts[num3];
				if (context2.IsSuccess() && ((context2.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority) ? interruptPriority : context2.interruptPriority) > num2 && !currentChore.choreType.interruptExclusion.Overlaps(context2.chore.choreType.tags))
				{
					out_context = context2;
					return true;
				}
			}
		}
		return false;
	}

	public bool FindNextChore(ref Chore.Precondition.Context out_context)
	{
		if (debug)
		{
			_ = 0 + 1;
		}
		preconditionSnapshot.Clear();
		consumerState.Refresh();
		if (consumerState.hasSolidTransferArm)
		{
			Debug.Assert(stationaryReach > 0);
			CellOffset offset = Grid.GetOffset(Grid.PosToCell(this));
			Extents extents = new Extents(offset.x, offset.y, stationaryReach);
			ListPool<ScenePartitionerEntry, ChoreConsumer>.PooledList pooledList = ListPool<ScenePartitionerEntry, ChoreConsumer>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.fetchChoreLayer, pooledList);
			foreach (ScenePartitionerEntry item in pooledList)
			{
				if (item.obj == null)
				{
					DebugUtil.Assert(test: false, "FindNextChore found an entry that was null");
					continue;
				}
				if (!(item.obj is FetchChore fetchChore))
				{
					DebugUtil.Assert(test: false, "FindNextChore found an entry that wasn't a FetchChore");
					continue;
				}
				if (fetchChore.target == null)
				{
					DebugUtil.Assert(test: false, "FindNextChore found an entry with a null target");
					continue;
				}
				if (fetchChore.isNull)
				{
					Debug.LogWarning("FindNextChore found an entry that isNull");
					continue;
				}
				int cell = Grid.PosToCell(fetchChore.gameObject);
				if (consumerState.solidTransferArm.IsCellReachable(cell))
				{
					fetchChore.CollectChoresFromGlobalChoreProvider(consumerState, preconditionSnapshot.succeededContexts, preconditionSnapshot.failedContexts, is_attempting_override: false);
				}
			}
			pooledList.Recycle();
		}
		else
		{
			for (int i = 0; i < providers.Count; i++)
			{
				providers[i].CollectChores(consumerState, preconditionSnapshot.succeededContexts, preconditionSnapshot.failedContexts);
			}
		}
		preconditionSnapshot.succeededContexts.Sort();
		List<Chore.Precondition.Context> succeededContexts = preconditionSnapshot.succeededContexts;
		bool num = ChooseChore(ref out_context, succeededContexts);
		if (num)
		{
			preconditionSnapshot.CopyTo(lastSuccessfulPreconditionSnapshot);
		}
		return num;
	}

	public void AddProvider(ChoreProvider provider)
	{
		DebugUtil.Assert(provider != null);
		providers.Add(provider);
	}

	public void RemoveProvider(ChoreProvider provider)
	{
		providers.Remove(provider);
	}

	public void AddUrge(Urge urge)
	{
		DebugUtil.Assert(urge != null);
		urges.Add(urge);
		Trigger(-736698276, urge);
	}

	public void RemoveUrge(Urge urge)
	{
		urges.Remove(urge);
		Trigger(231622047, urge);
	}

	public bool HasUrge(Urge urge)
	{
		return urges.Contains(urge);
	}

	public List<Urge> GetUrges()
	{
		return urges;
	}

	[Conditional("ENABLE_LOGGER")]
	public void Log(string evt, string param)
	{
	}

	public bool IsPermittedOrEnabled(ChoreType chore_type, Chore chore)
	{
		if (chore_type.groups.Length == 0)
		{
			return true;
		}
		bool flag = false;
		bool flag2 = true;
		for (int i = 0; i < chore_type.groups.Length; i++)
		{
			ChoreGroup chore_group = chore_type.groups[i];
			if (!IsPermittedByTraits(chore_group))
			{
				flag2 = false;
			}
			if (IsPermittedByUser(chore_group))
			{
				flag = true;
			}
		}
		return flag && flag2;
	}

	public void SetReach(int reach)
	{
		stationaryReach = reach;
	}

	public bool GetNavigationCost(IApproachable approachable, out int cost)
	{
		if ((bool)navigator)
		{
			cost = navigator.GetNavigationCost(approachable);
			if (cost != -1)
			{
				return true;
			}
		}
		else if (consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			if (consumerState.solidTransferArm.IsCellReachable(cell))
			{
				cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
				return true;
			}
		}
		cost = 0;
		return false;
	}

	public bool GetNavigationCost(int cell, out int cost)
	{
		if ((bool)navigator)
		{
			cost = navigator.GetNavigationCost(cell);
			if (cost != -1)
			{
				return true;
			}
		}
		else if (consumerState.hasSolidTransferArm && consumerState.solidTransferArm.IsCellReachable(cell))
		{
			cost = Grid.GetCellRange(this.NaturalBuildingCell(), cell);
			return true;
		}
		cost = 0;
		return false;
	}

	public bool CanReach(IApproachable approachable)
	{
		if ((bool)navigator)
		{
			return navigator.CanReach(approachable);
		}
		if (consumerState.hasSolidTransferArm)
		{
			int cell = approachable.GetCell();
			return consumerState.solidTransferArm.IsCellReachable(cell);
		}
		return false;
	}

	public bool IsWithinReach(IApproachable approachable)
	{
		if ((bool)navigator)
		{
			if (this == null || base.gameObject == null)
			{
				return false;
			}
			return Grid.IsCellOffsetOf(Grid.PosToCell(this), approachable.GetCell(), approachable.GetOffsets());
		}
		if (consumerState.hasSolidTransferArm)
		{
			return consumerState.solidTransferArm.IsCellReachable(approachable.GetCell());
		}
		return false;
	}

	public void ShowHoverTextOnHoveredItem(Chore.Precondition.Context context, KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		if (context.chore.target.isNull || context.chore.target.gameObject != hover_obj.gameObject)
		{
			return;
		}
		drawer.NewLine();
		drawer.AddIndent();
		drawer.DrawText(context.chore.choreType.Name, hover_text_card.Styles_BodyText.Standard);
		if (!context.IsSuccess())
		{
			Chore.PreconditionInstance preconditionInstance = context.chore.GetPreconditions()[context.failedPreconditionId];
			string text = preconditionInstance.description;
			if (string.IsNullOrEmpty(text))
			{
				text = preconditionInstance.id;
			}
			if (context.chore.driver != null)
			{
				text = text.Replace("{Assignee}", context.chore.driver.GetProperName());
			}
			text = text.Replace("{Selected}", this.GetProperName());
			drawer.DrawText(" (" + text + ")", hover_text_card.Styles_BodyText.Standard);
		}
	}

	public void ShowHoverTextOnHoveredItem(KSelectable hover_obj, HoverTextDrawer drawer, SelectToolHoverTextCard hover_text_card)
	{
		bool flag = false;
		foreach (Chore.Precondition.Context succeededContext in preconditionSnapshot.succeededContexts)
		{
			if (succeededContext.chore.showAvailabilityInHoverText && !succeededContext.chore.target.isNull && !(succeededContext.chore.target.gameObject != hover_obj.gameObject))
			{
				if (!flag)
				{
					drawer.NewLine();
					drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
					flag = true;
				}
				ShowHoverTextOnHoveredItem(succeededContext, hover_obj, drawer, hover_text_card);
			}
		}
		foreach (Chore.Precondition.Context failedContext in preconditionSnapshot.failedContexts)
		{
			if (failedContext.chore.showAvailabilityInHoverText && !failedContext.chore.target.isNull && !(failedContext.chore.target.gameObject != hover_obj.gameObject))
			{
				if (!flag)
				{
					drawer.NewLine();
					drawer.DrawText(DUPLICANTS.CHORES.PRECONDITIONS.HEADER.ToString().Replace("{Selected}", this.GetProperName()), hover_text_card.Styles_BodyText.Standard);
					flag = true;
				}
				ShowHoverTextOnHoveredItem(failedContext, hover_obj, drawer, hover_text_card);
			}
		}
	}

	public int GetPersonalPriority(ChoreType chore_type)
	{
		if (!choreTypePriorities.TryGetValue(chore_type.IdHash, out var value))
		{
			value = 3;
		}
		return Mathf.Clamp(value, 0, 5);
	}

	public int GetPersonalPriority(ChoreGroup group)
	{
		int value = 3;
		if (choreGroupPriorities.TryGetValue(group.IdHash, out var value2))
		{
			value = value2.priority;
		}
		return Mathf.Clamp(value, 0, 5);
	}

	public void SetPersonalPriority(ChoreGroup group, int value)
	{
		if (group.choreTypes != null)
		{
			value = Mathf.Clamp(value, 0, 5);
			if (!choreGroupPriorities.TryGetValue(group.IdHash, out var value2))
			{
				value2.priority = 3;
			}
			choreGroupPriorities[group.IdHash] = new PriorityInfo
			{
				priority = value
			};
			UpdateChoreTypePriorities(group, value);
			SetPermittedByUser(group, value != 0);
		}
	}

	public int GetAssociatedSkillLevel(ChoreGroup group)
	{
		return (int)this.GetAttributes().GetValue(group.attribute.Id);
	}

	private void UpdateChoreTypePriorities(ChoreGroup group, int value)
	{
		ChoreGroups choreGroups = Db.Get().ChoreGroups;
		foreach (ChoreType choreType in group.choreTypes)
		{
			int num = 0;
			foreach (ChoreGroup resource in choreGroups.resources)
			{
				if (resource.choreTypes == null)
				{
					continue;
				}
				foreach (ChoreType choreType2 in resource.choreTypes)
				{
					if (choreType2.IdHash == choreType.IdHash)
					{
						int personalPriority = GetPersonalPriority(resource);
						num = Mathf.Max(num, personalPriority);
					}
				}
			}
			choreTypePriorities[choreType.IdHash] = num;
		}
	}

	public void ResetPersonalPriorities()
	{
	}

	public bool RunBehaviourPrecondition(Tag tag)
	{
		BehaviourPrecondition value = default(BehaviourPrecondition);
		if (!behaviourPreconditions.TryGetValue(tag, out value))
		{
			return false;
		}
		return value.cb(value.arg);
	}

	public void AddBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		DebugUtil.Assert(!behaviourPreconditions.ContainsKey(tag));
		behaviourPreconditions[tag] = new BehaviourPrecondition
		{
			cb = precondition,
			arg = arg
		};
	}

	public void RemoveBehaviourPrecondition(Tag tag, Func<object, bool> precondition, object arg)
	{
		behaviourPreconditions.Remove(tag);
	}

	public bool IsChoreEqualOrAboveCurrentChorePriority<StateMachineType>()
	{
		Chore currentChore = choreDriver.GetCurrentChore();
		if (currentChore == null)
		{
			return true;
		}
		return currentChore.choreType.priority <= choreTable.GetChorePriority<StateMachineType>(this);
	}

	public bool IsChoreGroupDisabled(ChoreGroup chore_group)
	{
		bool result = false;
		Traits component = base.gameObject.GetComponent<Traits>();
		if (component != null && component.IsChoreGroupDisabled(chore_group))
		{
			result = true;
		}
		return result;
	}

	public Dictionary<HashedString, PriorityInfo> GetChoreGroupPriorities()
	{
		return choreGroupPriorities;
	}

	public void SetChoreGroupPriorities(Dictionary<HashedString, PriorityInfo> priorities)
	{
		choreGroupPriorities = priorities;
	}
}
