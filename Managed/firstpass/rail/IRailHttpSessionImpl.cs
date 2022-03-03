using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailHttpSessionImpl : RailObject, IRailHttpSession, IRailComponent
	{
		internal IRailHttpSessionImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailHttpSessionImpl()
		{
		}

		public virtual RailResult SetRequestMethod(RailHttpSessionMethod method)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestMethod(swigCPtr_, (int)method);
		}

		public virtual RailResult SetParameters(List<RailKeyValue> parameters)
		{
			IntPtr intPtr = ((parameters == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (parameters != null)
			{
				RailConverter.Csharp2Cpp(parameters, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetParameters(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
		}

		public virtual RailResult SetPostBodyContent(string body_content)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetPostBodyContent(swigCPtr_, body_content);
		}

		public virtual RailResult SetRequestTimeOut(uint timeout_secs)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestTimeOut(swigCPtr_, timeout_secs);
		}

		public virtual RailResult SetRequestHeaders(List<string> headers)
		{
			IntPtr intPtr = ((headers == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (headers != null)
			{
				RailConverter.Csharp2Cpp(headers, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_SetRequestHeaders(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual RailResult AsyncSendRequest(string url, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailHttpSession_AsyncSendRequest(swigCPtr_, url, user_data);
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
