namespace rail
{
	public interface IRailBrowserHelper
	{
		IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data, CreateBrowserOptions options, out RailResult result);

		IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data, CreateBrowserOptions options);

		IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data);

		IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data, CreateCustomerDrawBrowserOptions options, out RailResult result);

		IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data, CreateCustomerDrawBrowserOptions options);

		IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data);

		RailResult NavigateWebPage(string url, bool display_in_new_tab);
	}
}
