namespace rail
{
	public interface IRailBrowser : IRailComponent
	{
		bool GetCurrentUrl(out string url);

		bool ReloadWithUrl(string new_url);

		bool ReloadWithUrl();

		void StopLoad();

		bool AddJavascriptEventListener(string event_name);

		bool RemoveAllJavascriptEventListener();

		void AllowNavigateNewPage(bool allow);

		void Close();
	}
}
