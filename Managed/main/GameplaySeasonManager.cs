using System.Collections.Generic;
using System.Linq;
using Klei.AI;
using KSerialization;

public class GameplaySeasonManager : GameStateMachine<GameplaySeasonManager, GameplaySeasonManager.Instance, IStateMachineTarget, GameplaySeasonManager.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		public List<GameplaySeasonInstance> activeSeasons;

		[MyCmpGet]
		private WorldContainer m_worldContainer;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			activeSeasons = new List<GameplaySeasonInstance>();
		}

		public void Initialize()
		{
			activeSeasons.RemoveAll((GameplaySeasonInstance item) => item.Season == null);
			List<GameplaySeason> list = new List<GameplaySeason>();
			if (m_worldContainer != null)
			{
				ClusterGridEntity component = GetComponent<ClusterGridEntity>();
				foreach (string seasonId in m_worldContainer.GetSeasonIds())
				{
					GameplaySeason gameplaySeason = Db.Get().GameplaySeasons.TryGet(seasonId);
					if (gameplaySeason == null)
					{
						Debug.LogWarning("world " + component.name + " has invalid season " + seasonId);
						continue;
					}
					if (gameplaySeason.type != 0)
					{
						Debug.LogWarning("world " + component.name + " has specified season " + seasonId + ", which is not a world type season");
					}
					list.Add(gameplaySeason);
				}
			}
			else
			{
				Debug.Assert(GetComponent<SaveGame>() != null);
				list = Db.Get().GameplaySeasons.resources.Where((GameplaySeason season) => season.type == GameplaySeason.Type.Cluster).ToList();
			}
			foreach (GameplaySeason item in list)
			{
				if (DlcManager.IsContentActive(item.dlcId) && item.startActive && !SeasonExists(item))
				{
					activeSeasons.Add(item.Instantiate(GetWorldId()));
				}
			}
			foreach (GameplaySeasonInstance item2 in new List<GameplaySeasonInstance>(activeSeasons))
			{
				if (!list.Contains(item2.Season) || !DlcManager.IsContentActive(item2.Season.dlcId))
				{
					activeSeasons.Remove(item2);
				}
			}
		}

		private int GetWorldId()
		{
			if (m_worldContainer != null)
			{
				return m_worldContainer.id;
			}
			return -1;
		}

		public void Update(float dt)
		{
			foreach (GameplaySeasonInstance activeSeason in activeSeasons)
			{
				if (activeSeason.ShouldGenerateEvents() || !(GameUtil.GetCurrentTimeInCycles() > activeSeason.NextEventTime))
				{
					continue;
				}
				for (int i = 0; i < activeSeason.Season.numEventsToStartEachPeriod; i++)
				{
					if (!activeSeason.StartEvent())
					{
						break;
					}
				}
			}
		}

		public void StartNewSeason(GameplaySeason seasonType)
		{
			if (DlcManager.IsContentActive(seasonType.dlcId))
			{
				activeSeasons.Add(seasonType.Instantiate(GetWorldId()));
			}
		}

		public bool SeasonExists(GameplaySeason seasonType)
		{
			return activeSeasons.Find((GameplaySeasonInstance e) => e.Season.IdHash == seasonType.IdHash) != null;
		}
	}

	public override void InitializeStates(out BaseState defaultState)
	{
		defaultState = root;
		root.Enter(delegate(Instance smi)
		{
			smi.Initialize();
		}).Update(delegate(Instance smi, float dt)
		{
			smi.Update(dt);
		}, UpdateRate.SIM_4000ms);
	}
}
