using System;

namespace rail
{
	public class IRailZoneServerHelperImpl : RailObject, IRailZoneServerHelper
	{
		internal IRailZoneServerHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailZoneServerHelperImpl()
		{
		}

		public virtual RailZoneID GetPlayerSelectedZoneID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailZoneServerHelper_GetPlayerSelectedZoneID(swigCPtr_);
			RailZoneID railZoneID = new RailZoneID();
			RailConverter.Cpp2Csharp(ptr, railZoneID);
			return railZoneID;
		}

		public virtual RailZoneID GetRootZoneID()
		{
			IntPtr ptr = RAIL_API_PINVOKE.IRailZoneServerHelper_GetRootZoneID(swigCPtr_);
			RailZoneID railZoneID = new RailZoneID();
			RailConverter.Cpp2Csharp(ptr, railZoneID);
			return railZoneID;
		}

		public virtual IRailZoneServer OpenZoneServer(RailZoneID zone_id, out RailResult result)
		{
			IntPtr intPtr = ((zone_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailZoneID__SWIG_0());
			if (zone_id != null)
			{
				RailConverter.Csharp2Cpp(zone_id, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailZoneServerHelper_OpenZoneServer(swigCPtr_, intPtr, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailZoneServerImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailZoneID(intPtr);
			}
		}

		public virtual RailResult AsyncSwitchPlayerSelectedZone(RailZoneID zone_id)
		{
			IntPtr intPtr = ((zone_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailZoneID__SWIG_0());
			if (zone_id != null)
			{
				RailConverter.Csharp2Cpp(zone_id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailZoneServerHelper_AsyncSwitchPlayerSelectedZone(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailZoneID(intPtr);
			}
		}
	}
}
