using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class FoodFightEvent : GameplayEvent<FoodFightEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, FoodFightEvent>.GameplayEventStateMachineInstance
		{
			public List<FoodFightChore> chores;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, FoodFightEvent foodEvent)
				: base(master, eventInstance, foodEvent)
			{
			}

			public void CreateChores(StatesInstance smi)
			{
				chores = new List<FoodFightChore>();
				List<Room> list = Game.Instance.roomProber.rooms.FindAll((Room match) => match.roomType == Db.Get().RoomTypes.MessHall || match.roomType == Db.Get().RoomTypes.GreatHall);
				if (list == null || list.Count == 0)
				{
					return;
				}
				List<GameObject> buildingsOnFloor = list[UnityEngine.Random.Range(0, list.Count)].GetBuildingsOnFloor();
				for (int i = 0; i < Math.Min(Components.LiveMinionIdentities.Count, buildingsOnFloor.Count); i++)
				{
					MinionIdentity master = Components.LiveMinionIdentities[i];
					GameObject gameObject = buildingsOnFloor[UnityEngine.Random.Range(0, buildingsOnFloor.Count)];
					GameObject locator = ChoreHelpers.CreateLocator("FoodFightLocator", gameObject.transform.position);
					FoodFightChore foodFightChore = new FoodFightChore(master, locator);
					buildingsOnFloor.Remove(gameObject);
					foodFightChore.onExit = (Action<Chore>)Delegate.Combine(foodFightChore.onExit, (Action<Chore>)delegate
					{
						Util.KDestroyGameObject(locator);
					});
					chores.Add(foodFightChore);
				}
			}

			public void ClearChores()
			{
				if (chores != null)
				{
					for (int num = chores.Count - 1; num >= 0; num--)
					{
						if (chores[num] != null)
						{
							chores[num].Cancel("end");
						}
					}
				}
				chores = null;
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, FoodFightEvent>
		{
			public class WarmupStates : State
			{
				public State wait;

				public State start;
			}

			public State planning;

			public WarmupStates warmup;

			public State partying;

			public State ending;

			public State canceled;

			public override void InitializeStates(out BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = planning;
				base.serializable = SerializeType.Both_DEPRECATED;
				root.Exit(delegate(StatesInstance smi)
				{
					smi.ClearChores();
				});
				planning.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventNotification(GenerateEventPopupData(smi)));
				warmup.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventChosenNotification(GenerateEventPopupData(smi)));
				warmup.wait.ScheduleGoTo(60f, warmup.start);
				warmup.start.Enter(delegate(StatesInstance smi)
				{
					smi.CreateChores(smi);
				}).Update(delegate(StatesInstance smi, float data)
				{
					int num = 0;
					foreach (FoodFightChore chore in smi.chores)
					{
						if (chore.smi.IsInsideState(chore.smi.sm.waitForParticipants))
						{
							num++;
						}
					}
					if (num >= smi.chores.Count || smi.timeinstate > 30f)
					{
						foreach (FoodFightChore chore2 in smi.chores)
						{
							chore2.gameObject.Trigger(-2043101269);
						}
						smi.GoTo(partying);
					}
				}, UpdateRate.RENDER_1000ms);
				partying.ToggleNotification((StatesInstance smi) => new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.UNDERWAY, NotificationType.Good, (List<Notification> a, object b) => GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.UNDERWAY_TOOLTIP)).ScheduleGoTo(60f, ending);
				ending.ReturnSuccess();
				canceled.DoNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardCancelledNotification(GenerateEventPopupData(smi))).Enter(delegate
				{
					foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
					{
						liveMinionIdentity.GetComponent<Effects>().Add("NoFunAllowed", should_save: true);
					}
				}).ReturnFailure();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.location = GAMEPLAY_EVENTS.LOCATIONS.PRINTING_POD;
				gameplayEventPopupData.whenDescription = string.Format(GAMEPLAY_EVENTS.TIMES.IN_CYCLES, 0.1f);
				gameplayEventPopupData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.ACCEPT_OPTION_NAME).callback = delegate
				{
					smi.GoTo(smi.sm.warmup.wait);
				};
				gameplayEventPopupData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.REJECT_OPTION_NAME).callback = delegate
				{
					smi.GoTo(smi.sm.canceled);
				};
				return gameplayEventPopupData;
			}
		}

		public const float FUTURE_TIME = 60f;

		public const float DURATION = 60f;

		public FoodFightEvent()
			: base("FoodFight", 0, 0)
		{
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.FOOD_FIGHT.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
