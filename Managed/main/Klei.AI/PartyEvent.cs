using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;

namespace Klei.AI
{
	public class PartyEvent : GameplayEvent<PartyEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, PartyEvent>.GameplayEventStateMachineInstance
		{
			private List<Chore> chores;

			public Notification mainNotification;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, PartyEvent partyEvent)
				: base(master, eventInstance, partyEvent)
			{
			}

			public void AddNewChore(Room room)
			{
				List<KPrefabID> list = room.buildings.FindAll((KPrefabID match) => match.HasTag(RoomConstraints.ConstraintTags.RecBuilding));
				if (list.Count == 0)
				{
					DebugUtil.LogWarningArgs("Tried adding a party chore but the room wasn't valid! This probably happened on load? It's because rooms aren't built yet!");
					return;
				}
				int num = 0;
				bool flag = false;
				int locator_cell = Grid.InvalidCell;
				while (num < 20 && !flag)
				{
					num++;
					KPrefabID cmp = list[Random.Range(0, list.Count)];
					locator_cell = Grid.OffsetCell(offset: new CellOffset(Random.Range(-2, 3), 0), cell: Grid.PosToCell(cmp));
					if (Grid.HasDoor[locator_cell] || Game.Instance.roomProber.GetCavityForCell(locator_cell) != room.cavity || chores.Find((Chore match) => Grid.PosToCell(match.target.gameObject) == locator_cell) != null)
					{
						continue;
					}
					flag = true;
					break;
				}
				if (!flag)
				{
					return;
				}
				Vector3 pos = Grid.CellToPosCBC(locator_cell, Grid.SceneLayer.Move);
				GameObject locator = ChoreHelpers.CreateLocator("PartyWorkable", pos);
				PartyPointWorkable partyPointWorkable = locator.AddOrGet<PartyPointWorkable>();
				partyPointWorkable.SetWorkTime(Random.Range(10, 30));
				partyPointWorkable.basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;
				partyPointWorkable.faceTargetWhenWorking = true;
				PartyChore item = new PartyChore(locator.GetComponent<IStateMachineTarget>(), partyPointWorkable, null, null, delegate(Chore data)
				{
					if (chores != null)
					{
						chores.Remove(data);
						Util.KDestroyGameObject(locator);
					}
				});
				chores.Add(item);
			}

			public void ClearChores()
			{
				if (chores != null)
				{
					for (int num = chores.Count - 1; num >= 0; num--)
					{
						if (chores[num] != null)
						{
							Util.KDestroyGameObject(chores[num].gameObject);
						}
					}
				}
				chores = null;
			}

			public void UpdateChores(Room room)
			{
				if (room != null)
				{
					if (chores == null)
					{
						chores = new List<Chore>();
					}
					for (int i = chores.Count; i < Components.LiveMinionIdentities.Count * 2; i++)
					{
						AddNewChore(room);
					}
				}
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, PartyEvent>
		{
			public class PlanningStates : State
			{
				public State prepare_entities;

				public State wait_for_input;
			}

			public class WarmupStates : State
			{
				public State wait;

				public State start;
			}

			public TargetParameter roomObject;

			public TargetParameter planner;

			public TargetParameter guest;

			public PlanningStates planning;

			public WarmupStates warmup;

			public State partying;

			public State ending;

			public State canceled;

			public override void InitializeStates(out BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = planning.prepare_entities;
				base.serializable = SerializeType.Both_DEPRECATED;
				root.Enter(PopulateTargetsAndText);
				planning.DefaultState(planning.prepare_entities);
				planning.prepare_entities.Enter(delegate(StatesInstance smi)
				{
					if (planner.Get(smi) != null && guest.Get(smi) != null)
					{
						smi.GoTo(planning.wait_for_input);
					}
					else
					{
						smi.GoTo(ending);
					}
				});
				planning.wait_for_input.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventNotification(GenerateEventPopupData(smi)));
				warmup.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventChosenNotification(GenerateEventPopupData(smi)));
				warmup.wait.ScheduleGoTo(60f, warmup.start);
				warmup.start.Enter(PopulateTargetsAndText).Enter(delegate(StatesInstance smi)
				{
					Room chosenRoom = GetChosenRoom(smi);
					if (chosenRoom == null)
					{
						smi.GoTo(canceled);
					}
					else
					{
						smi.GoTo(partying);
					}
				});
				partying.ToggleNotification((StatesInstance smi) => new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.UNDERWAY, NotificationType.Good, (List<Notification> a, object b) => GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.UNDERWAY_TOOLTIP, null, expires: false, 0f, null, null, roomObject.Get(smi).transform)).Update(delegate(StatesInstance smi, float dt)
				{
					smi.UpdateChores(GetChosenRoom(smi));
				}, UpdateRate.SIM_4000ms).ScheduleGoTo(60f, ending);
				ending.ReturnSuccess();
				canceled.DoNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardCancelledNotification(GenerateEventPopupData(smi))).Enter(delegate(StatesInstance smi)
				{
					if (planner.Get(smi) != null)
					{
						planner.Get(smi).GetComponent<Effects>().Add("NoFunAllowed", should_save: true);
					}
					if (guest.Get(smi) != null)
					{
						guest.Get(smi).GetComponent<Effects>().Add("NoFunAllowed", should_save: true);
					}
				}).ReturnFailure();
			}

			public Room GetChosenRoom(StatesInstance smi)
			{
				return Game.Instance.roomProber.GetRoomOfGameObject(roomObject.Get(smi));
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				Room chosenRoom = GetChosenRoom(smi);
				string location = ((chosenRoom != null) ? chosenRoom.GetProperName() : GAMEPLAY_EVENTS.LOCATIONS.NONE_AVAILABLE.ToString());
				Effect effect = Db.Get().effects.Get("Socialized");
				Effect effect2 = Db.Get().effects.Get("NoFunAllowed");
				gameplayEventPopupData.location = location;
				gameplayEventPopupData.whenDescription = string.Format(GAMEPLAY_EVENTS.TIMES.IN_CYCLES, 0.1f);
				gameplayEventPopupData.minions = new GameObject[2]
				{
					smi.sm.guest.Get(smi),
					smi.sm.planner.Get(smi)
				};
				bool flag = true;
				GameplayEventPopupData.PopupOption popupOption = gameplayEventPopupData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_NAME, GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_DESC);
				popupOption.callback = delegate
				{
					smi.GoTo(smi.sm.warmup.wait);
				};
				popupOption.AddPositiveIcon(Assets.GetSprite("overlay_materials"), Effect.CreateFullTooltip(effect, showDuration: true));
				popupOption.tooltip = GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_DESC;
				if (!flag)
				{
					popupOption.AddInformationIcon("Cake must be built");
					popupOption.allowed = false;
					popupOption.tooltip = GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.ACCEPT_OPTION_INVALID_TOOLTIP;
				}
				GameplayEventPopupData.PopupOption popupOption2 = gameplayEventPopupData.AddOption(GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.REJECT_OPTION_NAME, GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.REJECT_OPTION_DESC);
				popupOption2.callback = delegate
				{
					smi.GoTo(smi.sm.canceled);
				};
				popupOption2.AddNegativeIcon(Assets.GetSprite("overlay_decor"), Effect.CreateFullTooltip(effect2, showDuration: true));
				gameplayEventPopupData.AddDefaultConsiderLaterOption();
				gameplayEventPopupData.SetTextParameter("host", planner.Get(smi).GetProperName());
				gameplayEventPopupData.SetTextParameter("dupe", guest.Get(smi).GetProperName());
				gameplayEventPopupData.SetTextParameter("goodEffect", effect.Name);
				gameplayEventPopupData.SetTextParameter("badEffect", effect2.Name);
				return gameplayEventPopupData;
			}

			public void PopulateTargetsAndText(StatesInstance smi)
			{
				if (roomObject.Get(smi) == null)
				{
					Room room = Game.Instance.roomProber.rooms.Find((Room match) => match.roomType == Db.Get().RoomTypes.RecRoom);
					roomObject.Set(room?.GetPrimaryEntities()[0], smi);
				}
				if (Components.LiveMinionIdentities.Count > 0)
				{
					if (planner.Get(smi) == null)
					{
						MinionIdentity value = Components.LiveMinionIdentities[Random.Range(0, Components.LiveMinionIdentities.Count)];
						planner.Set(value, smi);
					}
					if (guest.Get(smi) == null)
					{
						MinionIdentity value2 = Components.LiveMinionIdentities[Random.Range(0, Components.LiveMinionIdentities.Count)];
						guest.Set(value2, smi);
					}
				}
			}
		}

		public const string cancelEffect = "NoFunAllowed";

		public const float FUTURE_TIME = 60f;

		public const float DURATION = 60f;

		public PartyEvent()
			: base("Party", 0, 0)
		{
			popupAnimFileName = "event_pop_up_assets_kanim";
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.PARTY.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
