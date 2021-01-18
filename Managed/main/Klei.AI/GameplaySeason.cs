using System.Collections.Generic;
using System.Diagnostics;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public class GameplaySeason : Resource
	{
		public enum Type
		{
			World,
			Cluster
		}

		public const float DEFAULT_PERCENTAGE_RANDOMIZED_EVENT_START = 0.05f;

		public const float PERCENTAGE_WARNING = 0.4f;

		public const float USE_DEFAULT = -1f;

		public const int INFINITE = -1;

		public float period;

		public bool synchronizedToPeriod;

		public MathUtil.MinMax randomizedEventStartTime;

		public int finishAfterNumEvents = -1;

		public bool startActive;

		public int numEventsToStartEachPeriod;

		public float minCycle;

		public float maxCycle;

		public List<GameplayEvent> events;

		public Type type;

		public string dlcId;

		public GameplaySeason(string id, Type type, string dlcId, float period, bool synchronizedToPeriod, float randomizedEventStartTime = -1f, bool alwaysLoad = false, int finishAfterNumEvents = -1, float minCycle = 0f, float maxCycle = float.PositiveInfinity, int numEventsToStartEachPeriod = 1)
			: base(id)
		{
			this.type = type;
			this.dlcId = dlcId;
			this.period = period;
			this.synchronizedToPeriod = synchronizedToPeriod;
			Debug.Assert(period > 0f, "Season " + id + "'s Period cannot be 0 or negative");
			if (randomizedEventStartTime == -1f)
			{
				this.randomizedEventStartTime = new MathUtil.MinMax(-0.05f * period, 0.05f * period);
			}
			else
			{
				this.randomizedEventStartTime = new MathUtil.MinMax(0f - randomizedEventStartTime, randomizedEventStartTime);
				DebugUtil.DevAssert((this.randomizedEventStartTime.max - this.randomizedEventStartTime.min) * 0.4f < period, $"Season {id} randomizedEventStartTime is greater than {0.4f}% of its period.");
			}
			startActive = alwaysLoad;
			this.finishAfterNumEvents = finishAfterNumEvents;
			this.minCycle = minCycle;
			this.maxCycle = maxCycle;
			events = new List<GameplayEvent>();
			this.numEventsToStartEachPeriod = numEventsToStartEachPeriod;
		}

		public GameplaySeason AddEvent(GameplayEvent evt)
		{
			events.Add(evt);
			return this;
		}

		public GameplaySeasonInstance Instantiate(int worldId)
		{
			return new GameplaySeasonInstance(this, worldId);
		}
	}
}
