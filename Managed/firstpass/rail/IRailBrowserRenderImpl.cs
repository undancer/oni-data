using System;

namespace rail
{
	public class IRailBrowserRenderImpl : RailObject, IRailBrowserRender, IRailComponent
	{
		internal IRailBrowserRenderImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailBrowserRenderImpl()
		{
		}

		public virtual bool GetCurrentUrl(out string url)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return RAIL_API_PINVOKE.IRailBrowserRender_GetCurrentUrl(swigCPtr_, intPtr);
			}
			finally
			{
				url = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual bool ReloadWithUrl(string new_url)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ReloadWithUrl__SWIG_0(swigCPtr_, new_url);
		}

		public virtual bool ReloadWithUrl()
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ReloadWithUrl__SWIG_1(swigCPtr_);
		}

		public virtual void StopLoad()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_StopLoad(swigCPtr_);
		}

		public virtual bool AddJavascriptEventListener(string event_name)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_AddJavascriptEventListener(swigCPtr_, event_name);
		}

		public virtual bool RemoveAllJavascriptEventListener()
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_RemoveAllJavascriptEventListener(swigCPtr_);
		}

		public virtual void AllowNavigateNewPage(bool allow)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_AllowNavigateNewPage(swigCPtr_, allow);
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_Close(swigCPtr_);
		}

		public virtual void UpdateCustomDrawWindowPos(int content_offset_x, int content_offset_y, uint content_window_width, uint content_window_height)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_UpdateCustomDrawWindowPos(swigCPtr_, content_offset_x, content_offset_y, content_window_width, content_window_height);
		}

		public virtual void SetBrowserActive(bool active)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_SetBrowserActive(swigCPtr_, active);
		}

		public virtual void GoBack()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_GoBack(swigCPtr_);
		}

		public virtual void GoForward()
		{
			RAIL_API_PINVOKE.IRailBrowserRender_GoForward(swigCPtr_);
		}

		public virtual bool ExecuteJavascript(string event_name, string event_value)
		{
			return RAIL_API_PINVOKE.IRailBrowserRender_ExecuteJavascript(swigCPtr_, event_name, event_value);
		}

		public virtual void DispatchWindowsMessage(uint window_msg, uint w_param, uint l_param)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_DispatchWindowsMessage(swigCPtr_, window_msg, w_param, l_param);
		}

		public virtual void DispatchMouseMessage(EnumRailMouseActionType button_action, uint user_define_mouse_key, uint x_pos, uint y_pos)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_DispatchMouseMessage(swigCPtr_, (int)button_action, user_define_mouse_key, x_pos, y_pos);
		}

		public virtual void MouseWheel(int delta, uint user_define_mouse_key, uint x_pos, uint y_pos)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_MouseWheel(swigCPtr_, delta, user_define_mouse_key, x_pos, y_pos);
		}

		public virtual void SetFocus(bool has_focus)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_SetFocus(swigCPtr_, has_focus);
		}

		public virtual void KeyDown(uint key_code)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyDown(swigCPtr_, key_code);
		}

		public virtual void KeyUp(uint key_code)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyUp(swigCPtr_, key_code);
		}

		public virtual void KeyChar(uint key_code, bool is_uinchar)
		{
			RAIL_API_PINVOKE.IRailBrowserRender_KeyChar(swigCPtr_, key_code, is_uinchar);
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
