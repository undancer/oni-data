namespace rail
{
	public interface IRailBrowserRender : IRailComponent
	{
		bool GetCurrentUrl(out string url);

		bool ReloadWithUrl(string new_url);

		bool ReloadWithUrl();

		void StopLoad();

		bool AddJavascriptEventListener(string event_name);

		bool RemoveAllJavascriptEventListener();

		void AllowNavigateNewPage(bool allow);

		void Close();

		void UpdateCustomDrawWindowPos(int content_offset_x, int content_offset_y, uint content_window_width, uint content_window_height);

		void SetBrowserActive(bool active);

		void GoBack();

		void GoForward();

		bool ExecuteJavascript(string event_name, string event_value);

		void DispatchWindowsMessage(uint window_msg, uint w_param, uint l_param);

		void DispatchMouseMessage(EnumRailMouseActionType button_action, uint user_define_mouse_key, uint x_pos, uint y_pos);

		void MouseWheel(int delta, uint user_define_mouse_key, uint x_pos, uint y_pos);

		void SetFocus(bool has_focus);

		void KeyDown(uint key_code);

		void KeyUp(uint key_code);

		void KeyChar(uint key_code, bool is_uinchar);
	}
}
