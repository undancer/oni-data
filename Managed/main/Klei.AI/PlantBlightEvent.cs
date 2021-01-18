using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class PlantBlightEvent : GameplayEvent<PlantBlightEvent.StatesInstance>
	{
		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, PlantBlightEvent>
		{
			public class RunningStates : State
			{
				public State waiting;

				public State infect;
			}

			public State planning;

			public RunningStates running;

			public State finished;

			public Signal doFinish;

			public FloatParameter nextInfection;

			public override void InitializeStates(out BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = planning;
				base.serializable = SerializeType.Both_DEPRECATED;
				planning.Enter(delegate(StatesInstance smi)
				{
					smi.InfectAPlant(initialInfection: true);
				}).GoTo(running);
				running.ToggleNotification((StatesInstance smi) => GameplayEventInstance.CreateStandardEventNotification(GenerateEventPopupData(smi))).EventHandlerTransition(GameHashes.Uprooted, finished, NoBlightedPlants).DefaultState(running.waiting)
					.OnSignal(doFinish, finished);
				running.waiting.ParamTransition(nextInfection, running.infect, (StatesInstance smi, float p) => p <= 0f).Update(delegate(StatesInstance smi, float dt)
				{
					nextInfection.Delta(0f - dt, smi);
				}, UpdateRate.SIM_4000ms);
				running.infect.Enter(delegate(StatesInstance smi)
				{
					smi.InfectAPlant(initialInfection: false);
				}).GoTo(running.waiting);
				finished.DoNotification((StatesInstance smi) => CreateSuccessNotification(smi, GenerateEventPopupData(smi))).ReturnSuccess();
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				string value = smi.gameplayEvent.targetPlantPrefab.ToTag().ProperName();
				gameplayEventPopupData.location = GAMEPLAY_EVENTS.LOCATIONS.COLONY_WIDE;
				gameplayEventPopupData.whenDescription = GAMEPLAY_EVENTS.TIMES.NOW;
				gameplayEventPopupData.SetTextParameter("plant", value);
				return gameplayEventPopupData;
			}

			private Notification CreateSuccessNotification(StatesInstance smi, GameplayEventPopupData eventPopupData)
			{
				string plantName = smi.gameplayEvent.targetPlantPrefab.ToTag().ProperName();
				return new Notification(GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS.Replace("{plant}", plantName), NotificationType.Neutral, (List<Notification> list, object data) => GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.SUCCESS_TOOLTIP.Replace("{plant}", plantName));
			}

			private bool NoBlightedPlants(StatesInstance smi, object obj)
			{
				GameObject gameObject = (GameObject)obj;
				if (!gameObject.HasTag(GameTags.Blighted))
				{
					return true;
				}
				List<Crop> list = Components.Crops.Items.FindAll((Crop p) => p.name == smi.gameplayEvent.targetPlantPrefab);
				foreach (Crop item in list)
				{
					if (gameObject.gameObject == item.gameObject || !item.HasTag(GameTags.Blighted))
					{
						continue;
					}
					return false;
				}
				return true;
			}
		}

		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, PlantBlightEvent>.GameplayEventStateMachineInstance
		{
			[Serialize]
			private float startTime;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, PlantBlightEvent blightEvent)
				: base(master, eventInstance, blightEvent)
			{
				startTime = Time.time;
			}

			public void InfectAPlant(bool initialInfection)
			{
				if (Time.time - startTime > base.smi.gameplayEvent.infectionDuration)
				{
					base.sm.doFinish.Trigger(base.smi);
					return;
				}
				List<Crop> list = Components.Crops.Items.FindAll((Crop p) => p.name == base.smi.gameplayEvent.targetPlantPrefab);
				if (list.Count == 0)
				{
					base.sm.doFinish.Trigger(base.smi);
					return;
				}
				if (list.Count > 0)
				{
					List<Crop> list2 = new List<Crop>();
					List<Crop> list3 = new List<Crop>();
					foreach (Crop item in list)
					{
						if (item.HasTag(GameTags.Blighted))
						{
							list2.Add(item);
						}
						else
						{
							list3.Add(item);
						}
					}
					if (list2.Count == 0)
					{
						if (initialInfection)
						{
							Crop crop = list3[Random.Range(0, list3.Count)];
							Debug.Log("Blighting a random plant", crop);
							crop.GetComponent<BlightVulnerable>().MakeBlighted();
						}
						else
						{
							base.sm.doFinish.Trigger(base.smi);
						}
					}
					else if (list3.Count > 0)
					{
						Crop crop2 = list2[Random.Range(0, list2.Count)];
						Debug.Log("Spreading blight from a plant", crop2);
						list3.Shuffle();
						foreach (Crop item2 in list3)
						{
							if ((item2.transform.GetPosition() - crop2.transform.GetPosition()).magnitude < 6f)
							{
								item2.GetComponent<BlightVulnerable>().MakeBlighted();
								break;
							}
						}
					}
				}
				base.sm.nextInfection.Set(base.smi.gameplayEvent.incubationDuration, this);
			}
		}

		private const float BLIGHT_DISTANCE = 6f;

		public string targetPlantPrefab;

		public float infectionDuration;

		public float incubationDuration;

		public PlantBlightEvent(string id, string targetPlantPrefab, float infectionDuration, float incubationDuration)
			: base(id, 0, 0)
		{
			this.targetPlantPrefab = targetPlantPrefab;
			this.infectionDuration = infectionDuration;
			this.incubationDuration = incubationDuration;
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.PLANT_BLIGHT.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
