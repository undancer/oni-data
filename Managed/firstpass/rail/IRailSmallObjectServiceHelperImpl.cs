using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailSmallObjectServiceHelperImpl : RailObject, IRailSmallObjectServiceHelper
	{
		internal IRailSmallObjectServiceHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailSmallObjectServiceHelperImpl()
		{
		}

		public virtual RailResult AsyncDownloadObjects(List<uint> indexes, string user_data)
		{
			IntPtr intPtr = ((indexes == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayuint32_t__SWIG_0());
			if (indexes != null)
			{
				RailConverter.Csharp2Cpp(indexes, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_AsyncDownloadObjects(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayuint32_t(intPtr);
			}
		}

		public virtual RailResult GetObjectContent(uint index, out string content)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_GetObjectContent(swigCPtr_, index, intPtr);
			}
			finally
			{
				content = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult AsyncQueryObjectState(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSmallObjectServiceHelper_AsyncQueryObjectState(swigCPtr_, user_data);
		}
	}
}
