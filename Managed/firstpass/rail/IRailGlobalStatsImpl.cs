using System;

namespace rail
{
	public class IRailGlobalStatsImpl : RailObject, IRailGlobalStats, IRailComponent
	{
		internal IRailGlobalStatsImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailGlobalStatsImpl()
		{
		}

		public virtual RailResult AsyncRequestGlobalStats(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalStats_AsyncRequestGlobalStats(swigCPtr_, user_data);
		}

		public virtual RailResult GetGlobalStatValue(string name, out long data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalStats_GetGlobalStatValue__SWIG_0(swigCPtr_, name, out data);
		}

		public virtual RailResult GetGlobalStatValue(string name, out double data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalStats_GetGlobalStatValue__SWIG_1(swigCPtr_, name, out data);
		}

		public virtual RailResult GetGlobalStatValueHistory(string name, long[] global_stats_data, uint data_size, out int num_global_stats)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalStats_GetGlobalStatValueHistory__SWIG_0(swigCPtr_, name, global_stats_data, data_size, out num_global_stats);
		}

		public virtual RailResult GetGlobalStatValueHistory(string name, double[] global_stats_data, uint data_size, out int num_global_stats)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalStats_GetGlobalStatValueHistory__SWIG_1(swigCPtr_, name, global_stats_data, data_size, out num_global_stats);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(swigCPtr_);
		}
	}
}
