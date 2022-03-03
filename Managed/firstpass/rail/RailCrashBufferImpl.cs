using System;

namespace rail
{
	public class RailCrashBufferImpl : RailObject, RailCrashBuffer
	{
		internal RailCrashBufferImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~RailCrashBufferImpl()
		{
		}

		public virtual string GetData()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailCrashBuffer_GetData(swigCPtr_));
		}

		public virtual uint GetBufferLength()
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_GetBufferLength(swigCPtr_);
		}

		public virtual uint GetValidLength()
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_GetValidLength(swigCPtr_);
		}

		public virtual uint SetData(string data, uint length, uint offset)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_SetData__SWIG_0(swigCPtr_, data, length, offset);
		}

		public virtual uint SetData(string data, uint length)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_SetData__SWIG_1(swigCPtr_, data, length);
		}

		public virtual uint AppendData(string data, uint length)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_AppendData(swigCPtr_, data, length);
		}
	}
}
