using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class GameplaySeasonInstance : ISaveLoadable
	{
		public const int LIMIT_SELECTION = 5;

		[Serialize]
		public int numStartEvents;

		[Serialize]
		public int worldId;

		[Serialize]
		private readonly string seasonId;

		[Serialize]
		private float nextPeriodTime;

		[Serialize]
		private float randomizedNextTime;

		private bool allEventWillNotRunAgain;

		private GameplaySeason _season;

		public float NextEventTime => nextPeriodTime + randomizedNextTime;

		public GameplaySeason Season
		{
			get
			{
				if (_season == null)
				{
					_season = Db.Get().GameplaySeasons.TryGet(seasonId);
				}
				return _season;
			}
		}

		public GameplaySeasonInstance(GameplaySeason season, int worldId)
		{
			seasonId = season.Id;
			this.worldId = worldId;
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			if (season.synchronizedToPeriod)
			{
				float seasonPeriod = GetSeasonPeriod();
				nextPeriodTime = (Mathf.Floor(currentTimeInCycles / seasonPeriod) + 1f) * seasonPeriod;
			}
			else
			{
				nextPeriodTime = currentTimeInCycles;
			}
			CalculateNextEventTime();
		}

		private void CalculateNextEventTime()
		{
			float seasonPeriod = GetSeasonPeriod();
			randomizedNextTime = Random.Range(Season.randomizedEventStartTime.min, Season.randomizedEventStartTime.max);
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			while (nextPeriodTime < currentTimeInCycles || NextEventTime < Season.minCycle)
			{
				nextPeriodTime += seasonPeriod;
			}
		}

		public bool StartEvent(bool ignorePreconditions = false)
		{
			bool result = false;
			CalculateNextEventTime();
			numStartEvents++;
			List<GameplayEvent> list = (ignorePreconditions ? Season.events : Season.events.Where((GameplayEvent x) => x.IsAllowed()).ToList());
			if (list.Count > 0)
			{
				list.ForEach(delegate(GameplayEvent x)
				{
					x.CalculatePriority();
				});
				list.Sort();
				int maxExclusive = Mathf.Min(list.Count, 5);
				GameplayEvent eventType = list[Random.Range(0, maxExclusive)];
				GameplayEventManager.Instance.StartNewEvent(eventType, worldId);
				result = true;
			}
			allEventWillNotRunAgain = true;
			foreach (GameplayEvent @event in Season.events)
			{
				if (!@event.WillNeverRunAgain())
				{
					allEventWillNotRunAgain = false;
					return result;
				}
			}
			return result;
		}

		private float GetSeasonPeriod()
		{
			return Season.period;
		}

		public bool ShouldGenerateEvents()
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(worldId);
			if (!world.IsDupeVisited && !world.IsRoverVisted)
			{
				return false;
			}
			if ((Season.finishAfterNumEvents != -1 && numStartEvents >= Season.finishAfterNumEvents) || allEventWillNotRunAgain)
			{
				return false;
			}
			float currentTimeInCycles = GameUtil.GetCurrentTimeInCycles();
			if (currentTimeInCycles > Season.minCycle)
			{
				return currentTimeInCycles < Season.maxCycle;
			}
			return false;
		}
	}
}
