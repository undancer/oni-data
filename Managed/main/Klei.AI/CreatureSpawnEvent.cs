using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class CreatureSpawnEvent : GameplayEvent<CreatureSpawnEvent.StatesInstance>
	{
		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, CreatureSpawnEvent>.GameplayEventStateMachineInstance
		{
			[Serialize]
			private List<Vector3> spawnPositions = new List<Vector3>();

			[Serialize]
			private string creatureID;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, CreatureSpawnEvent creatureEvent)
				: base(master, eventInstance, creatureEvent)
			{
			}

			private void PickCreatureToSpawn()
			{
				creatureID = CreatureSpawnEventIDs.GetRandom();
			}

			private void PickSpawnLocations()
			{
				Telepad random = Components.Telepads.Items.GetRandom();
				Vector3 position = random.transform.GetPosition();
				int num = 100;
				ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
				GameScenePartitioner.Instance.GatherEntries((int)position.x - num / 2, (int)position.y - num / 2, num, num, GameScenePartitioner.Instance.plants, pooledList);
				foreach (ScenePartitionerEntry item in pooledList)
				{
					KPrefabID kPrefabID = (KPrefabID)item.obj;
					if (!kPrefabID.GetComponent<TreeBud>())
					{
						base.smi.spawnPositions.Add(kPrefabID.transform.GetPosition());
					}
				}
				pooledList.Recycle();
			}

			public void InitializeEvent()
			{
				PickCreatureToSpawn();
				PickSpawnLocations();
			}

			public void EndEvent()
			{
				creatureID = null;
				spawnPositions.Clear();
			}

			public void SpawnCreature()
			{
				if (spawnPositions.Count > 0)
				{
					Vector3 random = spawnPositions.GetRandom();
					GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(creatureID), random);
					gameObject.SetActive(value: true);
				}
			}
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, CreatureSpawnEvent>
		{
			public State initialize_event;

			public State spawn_season;

			public State start;

			public override void InitializeStates(out BaseState default_state)
			{
				default_state = initialize_event;
				base.serializable = SerializeType.Both_DEPRECATED;
				initialize_event.Enter(delegate(StatesInstance smi)
				{
					smi.InitializeEvent();
					smi.GoTo(spawn_season);
				});
				start.DoNothing();
				spawn_season.Update(delegate(StatesInstance smi, float dt)
				{
					smi.SpawnCreature();
				}, UpdateRate.SIM_4000ms).Exit(delegate(StatesInstance smi)
				{
					smi.EndEvent();
				});
			}

			public override GameplayEventPopupData GenerateEventPopupData(StatesInstance smi)
			{
				GameplayEventPopupData gameplayEventPopupData = new GameplayEventPopupData(smi.gameplayEvent);
				gameplayEventPopupData.location = GAMEPLAY_EVENTS.LOCATIONS.PRINTING_POD;
				gameplayEventPopupData.whenDescription = GAMEPLAY_EVENTS.TIMES.NOW;
				return gameplayEventPopupData;
			}
		}

		public const string ID = "HatchSpawnEvent";

		public const float UPDATE_TIME = 4f;

		public const float NUM_TO_SPAWN = 10f;

		public const float duration = 40f;

		public static List<string> CreatureSpawnEventIDs = new List<string>
		{
			"Hatch",
			"Squirrel",
			"Puft",
			"Crab",
			"Drecko",
			"Mole",
			"LightBug",
			"Pacu"
		};

		public CreatureSpawnEvent()
			: base("HatchSpawnEvent", 0, 0)
		{
			popupTitle = GAMEPLAY_EVENTS.EVENT_TYPES.CREATURE_SPAWN.NAME;
			popupDescription = GAMEPLAY_EVENTS.EVENT_TYPES.CREATURE_SPAWN.DESCRIPTION;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
