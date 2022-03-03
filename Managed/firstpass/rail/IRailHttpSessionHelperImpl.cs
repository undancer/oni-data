using System;

namespace rail
{
	public class IRailHttpSessionHelperImpl : RailObject, IRailHttpSessionHelper
	{
		internal IRailHttpSessionHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailHttpSessionHelperImpl()
		{
		}

		public virtual IRailHttpSession CreateHttpSession()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailHttpSessionHelper_CreateHttpSession(swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailHttpSessionImpl(intPtr);
			}
			return null;
		}

		public virtual IRailHttpResponse CreateHttpResponse(string http_response_data)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailHttpSessionHelper_CreateHttpResponse(swigCPtr_, http_response_data);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailHttpResponseImpl(intPtr);
			}
			return null;
		}
	}
}
