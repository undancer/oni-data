using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class BonusEvent : GameplayEvent<BonusEvent.StatesInstance>
	{
		public enum TriggerType
		{
			None,
			NewBuilding,
			UseBuilding,
			WorkableComplete,
			AchievementUnlocked
		}

		public delegate bool ConditionFn(GameplayEventData data);

		public class GameplayEventData
		{
			public GameHashes eventTrigger;

			public BuildingComplete building;

			public Workable workable;

			public Worker worker;
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, BonusEvent>
		{
			public class ActiveStates : State
			{
				public State notify;

				public State seenNotification;
			}

			public TargetParameter chosen;

			public State load;

			public State waitNewBuilding;

			public State waitUseBuilding;

			public State waitForAchievement;

			public State waitforWorkables;

			public State immediate;

			public ActiveStates active;

			public State ending;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = load;
				base.serializable = SerializeType.Both_DEPRECATED;
				load.Enter(AssignPreSelectedMinionIfNeeded).Transition(waitNewBuilding, (StatesInstance smi) => smi.gameplayEvent.triggerType == TriggerType.NewBuilding).Transition(waitUseBuilding, (StatesInstance smi) => smi.gameplayEvent.triggerType == TriggerType.UseBuilding)
					.Transition(waitforWorkables, (StatesInstance smi) => smi.gameplayEvent.triggerType == TriggerType.WorkableComplete)
					.Transition(waitForAchievement, (StatesInstance smi) => smi.gameplayEvent.triggerType == TriggerType.AchievementUnlocked)
					.Transition(immediate, (StatesInstance smi) => smi.gameplayEvent.triggerType == TriggerType.None);
				waitNewBuilding.EventHandlerTransition(GameHashes.NewBuilding, active, BuildingEventTrigger);
				waitUseBuilding.EventHandlerTransition(GameHashes.UseBuilding, active, BuildingEventTrigger);
				waitforWorkables.EventHandlerTransition(GameHashes.UseBuilding, active, WorkableEventTrigger);
				immediate.Enter(delegate(StatesInstance smi)
				{
					GameObject x = smi.sm.chosen.Get(smi);
					if (x == null)
					{
						x = smi.gameplayEvent.GetRandomMinionPrioritizeFiltered().gameObject;
						smi.sm.chosen.Set(x, smi);
					}
				}).GoTo(active);
				active.Enter(delegate(StatesInstance smi)
				{
					smi.sm.chosen.Get(smi).GetComponent<Effects>().Add(smi.gameplayEvent.effect, should_save: true);
				}).Enter(delegate(StatesInstance smi)
				{
					MonitorStart(chosen, smi);
				}).Exit(delegate(StatesInstance smi)
				{
					MonitorStop(chosen, smi);
				})
					.ScheduleGoTo((StatesInstance smi) => GetEffect(smi)?.duration ?? 0f, ending)
					.DefaultState(active.notify)
					.OnTargetLost(chosen, ending)
					.Target(chosen)
					.TagTransition(GameTags.Dead, ending);
				active.notify.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventNotification(GenerateEventPopupData(smi)));
				active.seenNotification.Enter(delegate(StatesInstance smi)
				{
					smi.eventInstance.seenNotification = true;
				});
				ending.ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				GameObject gameObject = smi.sm.chosen.Get(smi);
				if (gameObject == null)
				{
					DebugUtil.LogErrorArgs("Minion not set for " + smi.gameplayEvent.Id);
					return null;
				}
				Effect effect = GetEffect(smi);
				if (effect == null)
				{
					return null;
				}
				gameplayEventPopupData.focus = gameObject.transform;
				gameplayEventPopupData.minions = new GameObject[1]
				{
					gameObject
				};
				gameplayEventPopupData.SetTextParameter("dupe", gameObject.GetProperName());
				if (smi.building != null)
				{
					gameplayEventPopupData.SetTextParameter("building", UI.FormatAsLink(smi.building.GetProperName(), smi.building.GetProperName().ToUpper()));
				}
				GameplayEventPopupData.PopupOption popupOption = gameplayEventPopupData.AddDefaultOption(delegate
				{
					smi.GoTo(smi.sm.active.seenNotification);
				});
				string text = GAMEPLAY_EVENTS.BONUS_EVENT_DESCRIPTION;
				text = text.Replace("{effects}", Effect.CreateTooltip(effect, showDuration: false, " ", showHeader: false));
				text = text.Replace("{durration}", GameUtil.GetFormattedCycles(effect.duration));
				foreach (AttributeModifier selfModifier in effect.SelfModifiers)
				{
					Attribute attribute = Db.Get().Attributes.TryGet(selfModifier.AttributeId);
					string str = string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, attribute.Name, selfModifier.GetFormattedString());
					str = str + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_TOTAL, GameUtil.GetFormattedCycles(effect.duration));
					Sprite sprite = Assets.GetSprite(attribute.uiFullColourSprite);
					popupOption.AddPositiveIcon(sprite, str, 1.75f);
				}
				return gameplayEventPopupData;
			}

			private void AssignPreSelectedMinionIfNeeded(StatesInstance smi)
			{
				if (smi.gameplayEvent.preSelectMinion && smi.sm.chosen.Get(smi) == null)
				{
					smi.sm.chosen.Set(smi.gameplayEvent.GetRandomMinionPrioritizeFiltered().gameObject, smi);
					smi.timesTriggered = 0;
				}
			}

			private bool IsCorrectMinion(StatesInstance smi, GameplayEventData gameplayEventData)
			{
				if (smi.gameplayEvent.preSelectMinion && smi.sm.chosen.Get(smi) != gameplayEventData.worker.gameObject)
				{
					if (GameUtil.GetCurrentTimeInCycles() - smi.lastTriggered > 5f && smi.PercentageUntilTriggered() < 0.5f)
					{
						smi.sm.chosen.Set(gameplayEventData.worker.gameObject, smi);
						smi.timesTriggered = 0;
						return true;
					}
					return false;
				}
				return true;
			}

			private bool OtherConditionsAreSatisfied(StatesInstance smi, GameplayEventData gameplayEventData)
			{
				if (smi.gameplayEvent.roomRestrictions != null)
				{
					Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(gameplayEventData.worker.gameObject);
					if (roomOfGameObject == null)
					{
						return false;
					}
					if (!smi.gameplayEvent.roomRestrictions.Contains(roomOfGameObject.roomType))
					{
						return false;
					}
					if (smi.gameplayEvent.roomHasOwnable)
					{
						bool flag = false;
						foreach (Ownables owner in roomOfGameObject.GetOwners())
						{
							if (owner.gameObject == gameplayEventData.worker.gameObject)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return false;
						}
					}
				}
				if (smi.gameplayEvent.extraCondition != null && !smi.gameplayEvent.extraCondition(gameplayEventData))
				{
					return false;
				}
				return true;
			}

			private bool IncrementAndTrigger(StatesInstance smi, GameplayEventData gameplayEventData)
			{
				smi.timesTriggered++;
				smi.lastTriggered = GameUtil.GetCurrentTimeInCycles();
				if (smi.timesTriggered < smi.gameplayEvent.numTimesToTrigger)
				{
					return false;
				}
				smi.building = gameplayEventData.building;
				smi.sm.chosen.Set(gameplayEventData.worker.gameObject, smi);
				return true;
			}

			private bool BuildingEventTrigger(StatesInstance smi, object data)
			{
				GameplayEventData gameplayEventData = data as GameplayEventData;
				if (gameplayEventData == null)
				{
					return false;
				}
				AssignPreSelectedMinionIfNeeded(smi);
				if (gameplayEventData.building == null)
				{
					return false;
				}
				if (smi.gameplayEvent.buildingTrigger.Count > 0 && !smi.gameplayEvent.buildingTrigger.Contains(gameplayEventData.building.PrefabID()))
				{
					return false;
				}
				if (!OtherConditionsAreSatisfied(smi, gameplayEventData))
				{
					return false;
				}
				if (!IsCorrectMinion(smi, gameplayEventData))
				{
					return false;
				}
				return IncrementAndTrigger(smi, gameplayEventData);
			}

			private bool WorkableEventTrigger(StatesInstance smi, object data)
			{
				GameplayEventData gameplayEventData = data as GameplayEventData;
				if (gameplayEventData == null)
				{
					return false;
				}
				AssignPreSelectedMinionIfNeeded(smi);
				if (smi.gameplayEvent.workableType.Count > 0 && !smi.gameplayEvent.workableType.Contains(gameplayEventData.workable.GetType()))
				{
					return false;
				}
				if (!OtherConditionsAreSatisfied(smi, gameplayEventData))
				{
					return false;
				}
				if (!IsCorrectMinion(smi, gameplayEventData))
				{
					return false;
				}
				return IncrementAndTrigger(smi, gameplayEventData);
			}

			private bool ChosenMinionDied(StatesInstance smi, object data)
			{
				return smi.sm.chosen.Get(smi) == data as GameObject;
			}

			private Effect GetEffect(StatesInstance smi)
			{
				GameObject gameObject = smi.sm.chosen.Get(smi);
				if (gameObject == null)
				{
					return null;
				}
				EffectInstance effectInstance = gameObject.GetComponent<Effects>().Get(smi.gameplayEvent.effect);
				if (effectInstance == null)
				{
					Debug.LogWarning($"Effect {smi.gameplayEvent.effect} not found on {gameObject} in BonusEvent");
					return null;
				}
				return effectInstance.effect;
			}
		}

		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, BonusEvent>.GameplayEventStateMachineInstance
		{
			[Serialize]
			public int timesTriggered;

			[Serialize]
			public float lastTriggered;

			public BuildingComplete building;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, BonusEvent bonusEvent)
				: base(master, eventInstance, bonusEvent)
			{
				lastTriggered = GameUtil.GetCurrentTimeInCycles();
			}

			public float PercentageUntilTriggered()
			{
				return (float)timesTriggered / (float)base.smi.gameplayEvent.numTimesToTrigger;
			}
		}

		public const int PRE_SELECT_MINION_TIMEOUT = 5;

		public string effect;

		public bool preSelectMinion;

		public int numTimesToTrigger;

		public TriggerType triggerType = TriggerType.None;

		public HashSet<Tag> buildingTrigger;

		public HashSet<Type> workableType;

		public HashSet<RoomType> roomRestrictions;

		public ConditionFn extraCondition;

		public bool roomHasOwnable = false;

		public BonusEvent(string id, string overrideEffect = null, int numTimesAllowed = 1, bool preSelectMinion = false, int priority = 0)
			: base(id, priority, 0)
		{
			popupTitle = Strings.Get("STRINGS.GAMEPLAY_EVENTS.BONUS." + id.ToUpper() + ".NAME");
			popupDescription = Strings.Get("STRINGS.GAMEPLAY_EVENTS.BONUS." + id.ToUpper() + ".DESCRIPTION");
			effect = ((overrideEffect != null) ? overrideEffect : id);
			base.numTimesAllowed = numTimesAllowed;
			this.preSelectMinion = preSelectMinion;
			popupAnimFileName = id.ToLower() + "_kanim";
			AddPrecondition(GameplayEventPreconditions.Instance.LiveMinions());
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}

		public BonusEvent TriggerOnNewBuilding(int triggerCount, params string[] buildings)
		{
			DebugUtil.DevAssert(triggerType == TriggerType.None, "Only one trigger per event");
			triggerType = TriggerType.NewBuilding;
			buildingTrigger = new HashSet<Tag>(buildings.ToTagList());
			numTimesToTrigger = triggerCount;
			return this;
		}

		public BonusEvent TriggerOnUseBuilding(int triggerCount, params string[] buildings)
		{
			DebugUtil.DevAssert(triggerType == TriggerType.None, "Only one trigger per event");
			triggerType = TriggerType.UseBuilding;
			buildingTrigger = new HashSet<Tag>(buildings.ToTagList());
			numTimesToTrigger = triggerCount;
			return this;
		}

		public BonusEvent TriggerOnWorkableComplete(int triggerCount, params Type[] types)
		{
			DebugUtil.DevAssert(triggerType == TriggerType.None, "Only one trigger per event");
			triggerType = TriggerType.WorkableComplete;
			workableType = new HashSet<Type>(types);
			numTimesToTrigger = triggerCount;
			return this;
		}

		public BonusEvent SetExtraCondition(ConditionFn extraCondition)
		{
			this.extraCondition = extraCondition;
			return this;
		}

		public BonusEvent SetRoomConstraints(bool hasOwnableInRoom, params RoomType[] types)
		{
			roomHasOwnable = hasOwnableInRoom;
			roomRestrictions = ((types == null) ? null : new HashSet<RoomType>(types));
			return this;
		}

		public string GetEffectTooltip(Effect effect)
		{
			return effect.Name + "\n\n" + Effect.CreateTooltip(effect, showDuration: true);
		}

		public override Sprite GetDisplaySprite()
		{
			Effect effect = Db.Get().effects.Get(this.effect);
			if (effect.SelfModifiers.Count > 0)
			{
				Attribute attribute = Db.Get().Attributes.TryGet(effect.SelfModifiers[0].AttributeId);
				return Assets.GetSprite(attribute.uiFullColourSprite);
			}
			return null;
		}

		public override string GetDisplayString()
		{
			Effect effect = Db.Get().effects.Get(this.effect);
			if (effect.SelfModifiers.Count > 0)
			{
				Attribute attribute = Db.Get().Attributes.TryGet(effect.SelfModifiers[0].AttributeId);
				return attribute.Name;
			}
			return null;
		}
	}
}
