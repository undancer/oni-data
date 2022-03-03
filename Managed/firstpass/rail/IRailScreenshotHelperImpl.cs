using System;

namespace rail
{
	public class IRailScreenshotHelperImpl : RailObject, IRailScreenshotHelper
	{
		internal IRailScreenshotHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailScreenshotHelperImpl()
		{
		}

		public virtual IRailScreenshot CreateScreenshotWithRawData(byte[] rgb_data, uint len, uint width, uint height)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailScreenshotHelper_CreateScreenshotWithRawData(swigCPtr_, rgb_data, len, width, height);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailScreenshotImpl(intPtr);
			}
			return null;
		}

		public virtual IRailScreenshot CreateScreenshotWithLocalImage(string image_file, string thumbnail_file)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailScreenshotHelper_CreateScreenshotWithLocalImage(swigCPtr_, image_file, thumbnail_file);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailScreenshotImpl(intPtr);
			}
			return null;
		}

		public virtual void AsyncTakeScreenshot(string user_data)
		{
			RAIL_API_PINVOKE.IRailScreenshotHelper_AsyncTakeScreenshot(swigCPtr_, user_data);
		}

		public virtual void HookScreenshotHotKey(bool hook)
		{
			RAIL_API_PINVOKE.IRailScreenshotHelper_HookScreenshotHotKey(swigCPtr_, hook);
		}

		public virtual bool IsScreenshotHotKeyHooked()
		{
			return RAIL_API_PINVOKE.IRailScreenshotHelper_IsScreenshotHotKeyHooked(swigCPtr_);
		}
	}
}
