using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class ChorePreconditions
{
	private static ChorePreconditions _instance;

	public Chore.Precondition IsPreemptable;

	public Chore.Precondition HasUrge;

	public Chore.Precondition IsValid;

	public Chore.Precondition IsPermitted;

	public Chore.Precondition IsAssignedtoMe;

	public Chore.Precondition IsInMyWorld;

	public Chore.Precondition IsInMyParentWorld;

	public Chore.Precondition IsCellNotInMyWorld;

	public Chore.Precondition IsInMyRoom;

	public Chore.Precondition IsPreferredAssignable;

	public Chore.Precondition IsPreferredAssignableOrUrgentBladder;

	public Chore.Precondition IsNotTransferArm;

	public Chore.Precondition HasSkillPerk;

	public Chore.Precondition IsMinion;

	public Chore.Precondition IsMoreSatisfyingEarly;

	public Chore.Precondition IsMoreSatisfyingLate;

	public Chore.Precondition IsChattable;

	public Chore.Precondition IsNotRedAlert;

	public Chore.Precondition IsScheduledTime;

	public Chore.Precondition CanMoveTo;

	public Chore.Precondition CanMoveToCell;

	public Chore.Precondition CanPickup;

	public Chore.Precondition IsAwake;

	public Chore.Precondition IsStanding;

	public Chore.Precondition IsMoving;

	public Chore.Precondition IsOffLadder;

	public Chore.Precondition NotInTube;

	public Chore.Precondition ConsumerHasTrait;

	public Chore.Precondition IsOperational;

	public Chore.Precondition IsNotMarkedForDeconstruction;

	public Chore.Precondition IsNotMarkedForDisable;

	public Chore.Precondition IsFunctional;

	public Chore.Precondition IsOverrideTargetNullOrMe;

	public Chore.Precondition NotChoreCreator;

	public Chore.Precondition IsGettingMoreStressed;

	public Chore.Precondition IsAllowedByAutomation;

	public Chore.Precondition HasTag;

	public Chore.Precondition CheckBehaviourPrecondition;

	public Chore.Precondition CanDoWorkerPrioritizable;

	public Chore.Precondition IsExclusivelyAvailableWithOtherChores;

	public Chore.Precondition IsBladderFull;

	public Chore.Precondition IsBladderNotFull;

	public Chore.Precondition NoDeadBodies;

	public Chore.Precondition IsNotARobot;

	public Chore.Precondition NotCurrentlyPeeing;

	public static ChorePreconditions instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChorePreconditions();
			}
			return _instance;
		}
	}

	public static void DestroyInstance()
	{
		_instance = null;
	}

	public ChorePreconditions()
	{
		Chore.Precondition isPreemptable = new Chore.Precondition
		{
			id = "IsPreemptable",
			sortOrder = 1,
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREEMPTABLE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.isAttemptingOverride || context.chore.CanPreempt(context) || context.chore.driver == null;
			}
		};
		IsPreemptable = isPreemptable;
		isPreemptable = (HasUrge = new Chore.Precondition
		{
			id = "HasUrge",
			description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_URGE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.chore.choreType.urge == null)
				{
					return true;
				}
				foreach (Urge urge in context.consumerState.consumer.GetUrges())
				{
					if (context.chore.SatisfiesUrge(urge))
					{
						return true;
					}
				}
				return false;
			}
		});
		isPreemptable = (IsValid = new Chore.Precondition
		{
			id = "IsValid",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_VALID,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.chore.IsValid();
			}
		});
		isPreemptable = (IsPermitted = new Chore.Precondition
		{
			id = "IsPermitted",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PERMITTED,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.consumer.IsPermittedOrEnabled(context.choreTypeForPermission, context.chore);
			}
		});
		isPreemptable = (IsAssignedtoMe = new Chore.Precondition
		{
			id = "IsAssignedToMe",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_ASSIGNED_TO_ME,
			sortOrder = 10,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Assignable assignable2 = (Assignable)data;
				IAssignableIdentity component = context.consumerState.gameObject.GetComponent<IAssignableIdentity>();
				return component != null && assignable2.IsAssignedTo(component);
			}
		});
		isPreemptable = (IsInMyWorld = new Chore.Precondition
		{
			id = "IsInMyWorld",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_WORLD,
			sortOrder = -1,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return !context.chore.isNull && context.chore.gameObject.IsMyWorld(context.consumerState.gameObject);
			}
		});
		isPreemptable = (IsInMyParentWorld = new Chore.Precondition
		{
			id = "IsInMyParentWorld",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_WORLD,
			sortOrder = -1,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return !context.chore.isNull && context.chore.gameObject.IsMyParentWorld(context.consumerState.gameObject);
			}
		});
		isPreemptable = (IsCellNotInMyWorld = new Chore.Precondition
		{
			id = "IsCellNotInMyWorld",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_CELL_NOT_IN_MY_WORLD,
			sortOrder = -1,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (!context.chore.isNull)
				{
					int num = (int)data;
					return !Grid.IsValidCell(num) || Grid.WorldIdx[num] != context.consumerState.gameObject.GetMyWorldId();
				}
				return false;
			}
		});
		isPreemptable = (IsInMyRoom = new Chore.Precondition
		{
			id = "IsInMyRoom",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_ROOM,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				int cell2 = (int)data;
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell2);
				Room room = null;
				if (cavityForCell != null)
				{
					room = cavityForCell.room;
				}
				if (room != null)
				{
					if (!(context.consumerState.ownable != null))
					{
						Room room2 = null;
						FetchChore fetchChore = context.chore as FetchChore;
						if (fetchChore != null && fetchChore.destination != null)
						{
							CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(fetchChore.destination));
							if (cavityForCell2 != null)
							{
								room2 = cavityForCell2.room;
							}
							if (room2 != null)
							{
								return room2 == room;
							}
							return false;
						}
						if (context.chore is WorkChore<Tinkerable>)
						{
							CavityInfo cavityForCell3 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((context.chore as WorkChore<Tinkerable>).gameObject));
							if (cavityForCell3 != null)
							{
								room2 = cavityForCell3.room;
							}
							if (room2 != null)
							{
								return room2 == room;
							}
							return false;
						}
						return false;
					}
					foreach (Ownables owner in room.GetOwners())
					{
						if (owner.gameObject == context.consumerState.gameObject)
						{
							return true;
						}
					}
				}
				return false;
			}
		});
		isPreemptable = (IsPreferredAssignable = new Chore.Precondition
		{
			id = "IsPreferredAssignable",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE,
			sortOrder = 10,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Assignable assignable = (Assignable)data;
				return Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable.slot).Contains(assignable) ? true : false;
			}
		});
		isPreemptable = (IsPreferredAssignableOrUrgentBladder = new Chore.Precondition
		{
			id = "IsPreferredAssignableOrUrgent",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER,
			sortOrder = 10,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Assignable candidate = (Assignable)data;
				if (Game.Instance.assignmentManager.IsPreferredAssignable(context.consumerState.assignables, candidate))
				{
					return true;
				}
				PeeChoreMonitor.Instance sMI2 = context.consumerState.gameObject.GetSMI<PeeChoreMonitor.Instance>();
				return sMI2?.IsInsideState(sMI2.sm.critical) ?? false;
			}
		});
		isPreemptable = (IsNotTransferArm = new Chore.Precondition
		{
			id = "IsNotTransferArm",
			description = "",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return !context.consumerState.hasSolidTransferArm;
			}
		});
		isPreemptable = (HasSkillPerk = new Chore.Precondition
		{
			id = "HasSkillPerk",
			description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_SKILL_PERK,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				MinionResume resume2 = context.consumerState.resume;
				if (!resume2)
				{
					return false;
				}
				if (data is SkillPerk)
				{
					SkillPerk perk = data as SkillPerk;
					return resume2.HasPerk(perk);
				}
				if (data is HashedString)
				{
					HashedString perkId = (HashedString)data;
					return resume2.HasPerk(perkId);
				}
				if (data is string)
				{
					HashedString perkId2 = (string)data;
					return resume2.HasPerk(perkId2);
				}
				return false;
			}
		});
		isPreemptable = (IsMinion = new Chore.Precondition
		{
			id = "IsMinion",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MINION,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				MinionResume resume = context.consumerState.resume;
				return resume != null;
			}
		});
		isPreemptable = (IsMoreSatisfyingEarly = new Chore.Precondition
		{
			id = "IsMoreSatisfyingEarly",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING,
			sortOrder = -2,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.isAttemptingOverride)
				{
					return true;
				}
				if (context.skipMoreSatisfyingEarlyPrecondition)
				{
					return true;
				}
				if (context.consumerState.selectable.IsSelected)
				{
					return true;
				}
				Chore currentChore3 = context.consumerState.choreDriver.GetCurrentChore();
				if (currentChore3 != null)
				{
					if (context.masterPriority.priority_class != currentChore3.masterPriority.priority_class)
					{
						return context.masterPriority.priority_class > currentChore3.masterPriority.priority_class;
					}
					if (context.consumerState.consumer != null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore3.choreType))
					{
						return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore3.choreType);
					}
					if (context.masterPriority.priority_value != currentChore3.masterPriority.priority_value)
					{
						return context.masterPriority.priority_value > currentChore3.masterPriority.priority_value;
					}
					return context.priority > currentChore3.choreType.priority;
				}
				return true;
			}
		});
		isPreemptable = (IsMoreSatisfyingLate = new Chore.Precondition
		{
			id = "IsMoreSatisfyingLate",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING,
			sortOrder = 10000,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.isAttemptingOverride)
				{
					return true;
				}
				if (!context.consumerState.selectable.IsSelected)
				{
					return true;
				}
				Chore currentChore2 = context.consumerState.choreDriver.GetCurrentChore();
				if (currentChore2 != null)
				{
					if (context.masterPriority.priority_class != currentChore2.masterPriority.priority_class)
					{
						return context.masterPriority.priority_class > currentChore2.masterPriority.priority_class;
					}
					if (context.consumerState.consumer != null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType))
					{
						return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType);
					}
					if (context.masterPriority.priority_value != currentChore2.masterPriority.priority_value)
					{
						return context.masterPriority.priority_value > currentChore2.masterPriority.priority_value;
					}
					return context.priority > currentChore2.choreType.priority;
				}
				return true;
			}
		});
		isPreemptable = (IsChattable = new Chore.Precondition
		{
			id = "CanChat",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_CHAT,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				KMonoBehaviour kMonoBehaviour2 = (KMonoBehaviour)data;
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				if (context.consumerState.navigator == null)
				{
					return false;
				}
				return !(kMonoBehaviour2 == null) && context.consumerState.navigator.CanReach(kMonoBehaviour2.GetComponent<Chattable>());
			}
		});
		isPreemptable = (IsNotRedAlert = new Chore.Precondition
		{
			id = "IsNotRedAlert",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_RED_ALERT,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority || !context.chore.gameObject.GetMyWorld().IsRedAlert();
			}
		});
		isPreemptable = (IsScheduledTime = new Chore.Precondition
		{
			id = "IsScheduledTime",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_SCHEDULED_TIME,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.chore.gameObject.GetMyWorld().IsRedAlert())
				{
					return true;
				}
				ScheduleBlockType type = (ScheduleBlockType)data;
				return context.consumerState.scheduleBlock?.IsAllowed(type) ?? true;
			}
		});
		isPreemptable = (CanMoveTo = new Chore.Precondition
		{
			id = "CanMoveTo",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)data;
				if (kMonoBehaviour == null)
				{
					return false;
				}
				IApproachable approachable = (IApproachable)kMonoBehaviour;
				if (context.consumerState.consumer.GetNavigationCost(approachable, out var cost2))
				{
					context.cost += cost2;
					return true;
				}
				return false;
			}
		});
		isPreemptable = (CanMoveToCell = new Chore.Precondition
		{
			id = "CanMoveToCell",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				int cell = (int)data;
				if (!Grid.IsValidCell(cell))
				{
					return false;
				}
				if (context.consumerState.consumer.GetNavigationCost(cell, out var cost))
				{
					context.cost += cost;
					return true;
				}
				return false;
			}
		});
		isPreemptable = (CanPickup = new Chore.Precondition
		{
			id = "CanPickup",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_PICKUP,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Pickupable pickupable = (Pickupable)data;
				if (pickupable == null)
				{
					return false;
				}
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				if (pickupable.HasTag(GameTags.StoredPrivate))
				{
					return false;
				}
				return pickupable.CouldBePickedUpByMinion(context.consumerState.gameObject) && context.consumerState.consumer.CanReach(pickupable);
			}
		});
		isPreemptable = (IsAwake = new Chore.Precondition
		{
			id = "IsAwake",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_AWAKE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				StaminaMonitor.Instance sMI = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>();
				return !sMI.IsInsideState(sMI.sm.sleepy.sleeping);
			}
		});
		isPreemptable = (IsStanding = new Chore.Precondition
		{
			id = "IsStanding",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_STANDING,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				return !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType == NavType.Floor;
			}
		});
		isPreemptable = (IsMoving = new Chore.Precondition
		{
			id = "IsMoving",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MOVING,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				return !(context.consumerState.navigator == null) && context.consumerState.navigator.IsMoving();
			}
		});
		isPreemptable = (IsOffLadder = new Chore.Precondition
		{
			id = "IsOffLadder",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OFF_LADDER,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				return !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType != NavType.Ladder && context.consumerState.navigator.CurrentNavType != NavType.Pole;
			}
		});
		isPreemptable = (NotInTube = new Chore.Precondition
		{
			id = "NotInTube",
			description = DUPLICANTS.CHORES.PRECONDITIONS.NOT_IN_TUBE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				return !(context.consumerState.navigator == null) && context.consumerState.navigator.CurrentNavType != NavType.Tube;
			}
		});
		isPreemptable = (ConsumerHasTrait = new Chore.Precondition
		{
			id = "ConsumerHasTrait",
			description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_TRAIT,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				string trait_id = (string)data;
				Traits traits = context.consumerState.traits;
				return !(traits == null) && traits.HasTrait(trait_id);
			}
		});
		isPreemptable = (IsOperational = new Chore.Precondition
		{
			id = "IsOperational",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OPERATIONAL,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Operational operational2 = data as Operational;
				return operational2.IsOperational;
			}
		});
		isPreemptable = (IsNotMarkedForDeconstruction = new Chore.Precondition
		{
			id = "IsNotMarkedForDeconstruction",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Deconstructable deconstructable = data as Deconstructable;
				return deconstructable == null || !deconstructable.IsMarkedForDeconstruction();
			}
		});
		isPreemptable = (IsNotMarkedForDisable = new Chore.Precondition
		{
			id = "IsNotMarkedForDisable",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DISABLE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				BuildingEnabledButton buildingEnabledButton = data as BuildingEnabledButton;
				return buildingEnabledButton == null || (buildingEnabledButton.IsEnabled && !buildingEnabledButton.WaitingForDisable);
			}
		});
		isPreemptable = (IsFunctional = new Chore.Precondition
		{
			id = "IsFunctional",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_FUNCTIONAL,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Operational operational = data as Operational;
				return operational.IsFunctional;
			}
		});
		isPreemptable = (IsOverrideTargetNullOrMe = new Chore.Precondition
		{
			id = "IsOverrideTargetNullOrMe",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_OVERRIDE_TARGET_NULL_OR_ME,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.isAttemptingOverride || context.chore.overrideTarget == null || context.chore.overrideTarget == context.consumerState.consumer;
			}
		});
		isPreemptable = (NotChoreCreator = new Chore.Precondition
		{
			id = "NotChoreCreator",
			description = DUPLICANTS.CHORES.PRECONDITIONS.NOT_CHORE_CREATOR,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				GameObject y = (GameObject)data;
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				return (!(context.consumerState.gameObject == y)) ? true : false;
			}
		});
		isPreemptable = (IsGettingMoreStressed = new Chore.Precondition
		{
			id = "IsGettingMoreStressed",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_GETTING_MORE_STRESSED,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
				return amountInstance.GetDelta() > 0f;
			}
		});
		isPreemptable = (IsAllowedByAutomation = new Chore.Precondition
		{
			id = "IsAllowedByAutomation",
			description = DUPLICANTS.CHORES.PRECONDITIONS.IS_ALLOWED_BY_AUTOMATION,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Automatable automatable = (Automatable)data;
				return automatable.AllowedByAutomation(context.consumerState.hasSolidTransferArm);
			}
		});
		isPreemptable = (HasTag = new Chore.Precondition
		{
			id = "HasTag",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Tag tag2 = (Tag)data;
				return context.consumerState.prefabid.HasTag(tag2);
			}
		});
		isPreemptable = (CheckBehaviourPrecondition = new Chore.Precondition
		{
			id = "CheckBehaviourPrecondition",
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				Tag tag = (Tag)data;
				return context.consumerState.consumer.RunBehaviourPrecondition(tag);
			}
		});
		isPreemptable = (CanDoWorkerPrioritizable = new Chore.Precondition
		{
			id = "CanDoWorkerPrioritizable",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CAN_DO_RECREATION,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				if (context.consumerState.consumer == null)
				{
					return false;
				}
				IWorkerPrioritizable workerPrioritizable = data as IWorkerPrioritizable;
				if (workerPrioritizable == null)
				{
					return false;
				}
				int priority = 0;
				if (workerPrioritizable.GetWorkerPriority(context.consumerState.worker, out priority))
				{
					context.consumerPriority += priority;
					return true;
				}
				return false;
			}
		});
		isPreemptable = (IsExclusivelyAvailableWithOtherChores = new Chore.Precondition
		{
			id = "IsExclusivelyAvailableWithOtherChores",
			description = DUPLICANTS.CHORES.PRECONDITIONS.EXCLUSIVELY_AVAILABLE,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				List<Chore> list = (List<Chore>)data;
				foreach (Chore item in list)
				{
					if (item != context.chore && item.driver != null)
					{
						return false;
					}
				}
				return true;
			}
		});
		isPreemptable = (IsBladderFull = new Chore.Precondition
		{
			id = "IsBladderFull",
			description = DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_FULL,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return (context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>()?.NeedsToPee() ?? false) ? true : false;
			}
		});
		isPreemptable = (IsBladderNotFull = new Chore.Precondition
		{
			id = "IsBladderNotFull",
			description = DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_NOT_FULL,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return (!(context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>()?.NeedsToPee() ?? false)) ? true : false;
			}
		});
		isPreemptable = (NoDeadBodies = new Chore.Precondition
		{
			id = "NoDeadBodies",
			description = DUPLICANTS.CHORES.PRECONDITIONS.NO_DEAD_BODIES,
			fn = delegate
			{
				return Components.LiveMinionIdentities.Count == Components.MinionIdentities.Count;
			}
		});
		isPreemptable = (IsNotARobot = new Chore.Precondition
		{
			id = "NoRobots",
			description = DUPLICANTS.CHORES.PRECONDITIONS.NOT_A_ROBOT,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				return context.consumerState.gameObject.GetComponent<MinionResume>() != null;
			}
		});
		isPreemptable = (NotCurrentlyPeeing = new Chore.Precondition
		{
			id = "NotCurrentlyPeeing",
			description = DUPLICANTS.CHORES.PRECONDITIONS.CURRENTLY_PEEING,
			fn = delegate(ref Chore.Precondition.Context context, object data)
			{
				bool result = true;
				Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
				if (currentChore != null)
				{
					string id = currentChore.choreType.Id;
					result = id != Db.Get().ChoreTypes.BreakPee.Id && id != Db.Get().ChoreTypes.Pee.Id;
				}
				return result;
			}
		});
		base._002Ector();
	}
}
