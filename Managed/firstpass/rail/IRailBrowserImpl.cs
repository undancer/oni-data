using System;

namespace rail
{
	public class IRailBrowserImpl : RailObject, IRailBrowser, IRailComponent
	{
		internal IRailBrowserImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailBrowserImpl()
		{
		}

		public virtual bool GetCurrentUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailBrowser_GetCurrentUrl(swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool ReloadWithUrl(string new_url)
		{
			return RAIL_API_PINVOKE.IRailBrowser_ReloadWithUrl__SWIG_0(swigCPtr_, new_url);
		}

		public virtual bool ReloadWithUrl()
		{
			return RAIL_API_PINVOKE.IRailBrowser_ReloadWithUrl__SWIG_1(swigCPtr_);
		}

		public virtual void StopLoad()
		{
			RAIL_API_PINVOKE.IRailBrowser_StopLoad(swigCPtr_);
		}

		public virtual bool AddJavascriptEventListener(string event_name)
		{
			return RAIL_API_PINVOKE.IRailBrowser_AddJavascriptEventListener(swigCPtr_, event_name);
		}

		public virtual bool RemoveAllJavascriptEventListener()
		{
			return RAIL_API_PINVOKE.IRailBrowser_RemoveAllJavascriptEventListener(swigCPtr_);
		}

		public virtual void AllowNavigateNewPage(bool allow)
		{
			RAIL_API_PINVOKE.IRailBrowser_AllowNavigateNewPage(swigCPtr_, allow);
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailBrowser_Close(swigCPtr_);
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
