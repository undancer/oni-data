using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailHttpResponseImpl : RailObject, IRailHttpResponse, IRailComponent
	{
		internal IRailHttpResponseImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailHttpResponseImpl()
		{
		}

		public virtual int GetHttpResponseCode()
		{
			return RAIL_API_PINVOKE.IRailHttpResponse_GetHttpResponseCode(swigCPtr_);
		}

		public virtual RailResult GetResponseHeaderKeys(List<string> header_keys)
		{
			IntPtr intPtr = ((header_keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailHttpResponse_GetResponseHeaderKeys(swigCPtr_, intPtr);
			}
			finally
			{
				if (header_keys != null)
				{
					RailConverter.Cpp2Csharp(intPtr, header_keys);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
		}

		public virtual string GetResponseHeaderValue(string header_key)
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetResponseHeaderValue(swigCPtr_, header_key));
		}

		public virtual string GetResponseBodyData()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetResponseBodyData(swigCPtr_));
		}

		public virtual uint GetContentLength()
		{
			return RAIL_API_PINVOKE.IRailHttpResponse_GetContentLength(swigCPtr_);
		}

		public virtual string GetContentType()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetContentType(swigCPtr_));
		}

		public virtual string GetContentRange()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetContentRange(swigCPtr_));
		}

		public virtual string GetContentLanguage()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetContentLanguage(swigCPtr_));
		}

		public virtual string GetContentEncoding()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetContentEncoding(swigCPtr_));
		}

		public virtual string GetLastModified()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailHttpResponse_GetLastModified(swigCPtr_));
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
