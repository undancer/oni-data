using System;

namespace rail
{
	public class IRailFloatingWindowImpl : RailObject, IRailFloatingWindow
	{
		internal IRailFloatingWindowImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailFloatingWindowImpl()
		{
		}

		public virtual RailResult AsyncShowRailFloatingWindow(EnumRailWindowType window_type, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_AsyncShowRailFloatingWindow(swigCPtr_, (int)window_type, user_data);
		}

		public virtual RailResult AsyncCloseRailFloatingWindow(EnumRailWindowType window_type, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_AsyncCloseRailFloatingWindow(swigCPtr_, (int)window_type, user_data);
		}

		public virtual RailResult SetNotifyWindowPosition(EnumRailNotifyWindowType window_type, RailWindowLayout layout)
		{
			IntPtr intPtr = ((layout == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailWindowLayout__SWIG_0());
			if (layout != null)
			{
				RailConverter.Csharp2Cpp(layout, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_SetNotifyWindowPosition(swigCPtr_, (int)window_type, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailWindowLayout(intPtr);
			}
		}

		public virtual RailResult AsyncShowStoreWindow(ulong id, RailStoreOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailStoreOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_AsyncShowStoreWindow(swigCPtr_, id, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailStoreOptions(intPtr);
			}
		}

		public virtual bool IsFloatingWindowAvailable()
		{
			return RAIL_API_PINVOKE.IRailFloatingWindow_IsFloatingWindowAvailable(swigCPtr_);
		}

		public virtual RailResult AsyncShowDefaultGameStoreWindow(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_AsyncShowDefaultGameStoreWindow(swigCPtr_, user_data);
		}

		public virtual RailResult SetNotifyWindowEnable(EnumRailNotifyWindowType window_type, bool enable)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFloatingWindow_SetNotifyWindowEnable(swigCPtr_, (int)window_type, enable);
		}
	}
}
