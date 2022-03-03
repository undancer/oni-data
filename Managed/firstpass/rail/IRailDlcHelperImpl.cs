using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailDlcHelperImpl : RailObject, IRailDlcHelper
	{
		internal IRailDlcHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailDlcHelperImpl()
		{
		}

		public virtual RailResult AsyncQueryIsOwnedDlcsOnServer(List<RailDlcID> dlc_ids, string user_data)
		{
			IntPtr intPtr = ((dlc_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailDlcID__SWIG_0());
			if (dlc_ids != null)
			{
				RailConverter.Csharp2Cpp(dlc_ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailDlcHelper_AsyncQueryIsOwnedDlcsOnServer(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailDlcID(intPtr);
			}
		}

		public virtual RailResult AsyncCheckAllDlcsStateReady(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailDlcHelper_AsyncCheckAllDlcsStateReady(swigCPtr_, user_data);
		}

		public virtual bool IsDlcInstalled(RailDlcID dlc_id, out string installed_path)
		{
			IntPtr intPtr = ((dlc_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcID__SWIG_0());
			if (dlc_id != null)
			{
				RailConverter.Csharp2Cpp(dlc_id, intPtr);
			}
			IntPtr intPtr2 = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_IsDlcInstalled__SWIG_0(swigCPtr_, intPtr, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailDlcID(intPtr);
				installed_path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr2));
				RAIL_API_PINVOKE.delete_RailString(intPtr2);
			}
		}

		public virtual bool IsDlcInstalled(RailDlcID dlc_id)
		{
			IntPtr intPtr = ((dlc_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcID__SWIG_0());
			if (dlc_id != null)
			{
				RailConverter.Csharp2Cpp(dlc_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_IsDlcInstalled__SWIG_1(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailDlcID(intPtr);
			}
		}

		public virtual bool IsOwnedDlc(RailDlcID dlc_id)
		{
			IntPtr intPtr = ((dlc_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcID__SWIG_0());
			if (dlc_id != null)
			{
				RailConverter.Csharp2Cpp(dlc_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_IsOwnedDlc(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailDlcID(intPtr);
			}
		}

		public virtual uint GetDlcCount()
		{
			return RAIL_API_PINVOKE.IRailDlcHelper_GetDlcCount(swigCPtr_);
		}

		public virtual bool GetDlcInfo(uint index, RailDlcInfo dlc_info)
		{
			IntPtr intPtr = ((dlc_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcInfo__SWIG_0());
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_GetDlcInfo(swigCPtr_, index, intPtr);
			}
			finally
			{
				if (dlc_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, dlc_info);
				}
				RAIL_API_PINVOKE.delete_RailDlcInfo(intPtr);
			}
		}

		public virtual bool AsyncInstallDlc(RailDlcID dlc_id, string user_data)
		{
			IntPtr intPtr = ((dlc_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcID__SWIG_0());
			if (dlc_id != null)
			{
				RailConverter.Csharp2Cpp(dlc_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_AsyncInstallDlc(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailDlcID(intPtr);
			}
		}

		public virtual bool AsyncRemoveDlc(RailDlcID dlc_id, string user_data)
		{
			IntPtr intPtr = ((dlc_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDlcID__SWIG_0());
			if (dlc_id != null)
			{
				RailConverter.Csharp2Cpp(dlc_id, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailDlcHelper_AsyncRemoveDlc(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailDlcID(intPtr);
			}
		}
	}
}
