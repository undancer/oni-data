using System.Collections.Generic;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	public class MeteorShowerEvent : GameplayEvent<MeteorShowerEvent.StatesInstance>
	{
		private struct BombardmentInfo
		{
			public string prefab;

			public float weight;
		}

		public class States : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, MeteorShowerEvent>
		{
			public State planning;

			public State running;

			public State finished;

			public Signal doFinish;

			public override void InitializeStates(out BaseState default_state)
			{
				base.InitializeStates(out default_state);
				default_state = planning;
				base.serializable = SerializeType.Both_DEPRECATED;
				planning.GoTo(running);
				running.Update(delegate(StatesInstance smi, float dt)
				{
					smi.Update(dt);
				}).OnSignal(doFinish, finished);
				finished.ReturnSuccess();
			}
		}

		public class StatesInstance : GameplayEventStateMachine<States, StatesInstance, GameplayEventManager, MeteorShowerEvent>.GameplayEventStateMachineInstance
		{
			public GameObject activeMeteorBackground;

			[Serialize]
			private float nextMeteorTime = 0f;

			[Serialize]
			private float timeRemaining;

			[Serialize]
			private int m_worldId;

			public StatesInstance(GameplayEventManager master, GameplayEventInstance eventInstance, MeteorShowerEvent meteorShowerEvent)
				: base(master, eventInstance, meteorShowerEvent)
			{
				timeRemaining = gameplayEvent.duration;
				m_worldId = eventInstance.worldId;
				if (activeMeteorBackground == null)
				{
					activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground);
					WorldContainer world = ClusterManager.Instance.GetWorld(m_worldId);
					float x = (world.maximumBounds.x + world.minimumBounds.x) / 2f;
					float y = world.maximumBounds.y;
					float z = 25f;
					activeMeteorBackground.transform.SetPosition(new Vector3(x, y, z));
					activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
				}
			}

			public override void StopSM(string reason)
			{
				if (activeMeteorBackground != null)
				{
					ParticleSystem component = activeMeteorBackground.GetComponent<ParticleSystem>();
					ParticleSystem.MainModule main = component.main;
					main.stopAction = ParticleSystemStopAction.Destroy;
					component.Stop();
					if (!component.IsAlive())
					{
						Object.Destroy(activeMeteorBackground);
					}
					activeMeteorBackground = null;
				}
				base.StopSM(reason);
			}

			public void Update(float dt)
			{
				nextMeteorTime -= dt;
				while (nextMeteorTime < 0f)
				{
					DoBombardment(gameplayEvent.bombardmentInfo);
					nextMeteorTime += 0.33f;
				}
				timeRemaining -= dt;
				if (timeRemaining <= 0f)
				{
					base.sm.doFinish.Trigger(base.smi);
				}
			}

			private void DoBombardment(List<BombardmentInfo> bombardment_info)
			{
				float num = 0f;
				foreach (BombardmentInfo item in bombardment_info)
				{
					num += item.weight;
				}
				num = Random.Range(0f, num);
				BombardmentInfo bombardmentInfo = bombardment_info[0];
				int num2 = 0;
				while (num - bombardmentInfo.weight > 0f)
				{
					num -= bombardmentInfo.weight;
					bombardmentInfo = bombardment_info[++num2];
				}
				Game.Instance.Trigger(-84771526);
				SpawnBombard(bombardmentInfo.prefab);
			}

			private GameObject SpawnBombard(string prefab)
			{
				WorldContainer world = ClusterManager.Instance.GetWorld(m_worldId);
				float x = (float)world.Width * Random.value + (float)world.WorldOffset.x;
				float y = world.Height + world.WorldOffset.y;
				float layerZ = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
				GameObject gameObject = Util.KInstantiate(position: new Vector3(x, y, layerZ), original: Assets.GetPrefab(prefab), rotation: Quaternion.identity);
				gameObject.SetActive(value: true);
				return gameObject;
			}
		}

		private List<BombardmentInfo> bombardmentInfo;

		private const float SECONDS_PER_METEOR = 0.33f;

		private float duration;

		public MeteorShowerEvent(string id, float duration)
			: base(id, 0, 0)
		{
			this.duration = duration;
			bombardmentInfo = new List<BombardmentInfo>();
			tags.Add(GameTags.SpaceDanger);
		}

		public MeteorShowerEvent AddMeteor(string prefab, float weight)
		{
			bombardmentInfo.Add(new BombardmentInfo
			{
				prefab = prefab,
				weight = weight
			});
			return this;
		}

		public override StateMachine.Instance GetSMI(GameplayEventManager manager, GameplayEventInstance eventInstance)
		{
			return new StatesInstance(manager, eventInstance, this);
		}
	}
}
