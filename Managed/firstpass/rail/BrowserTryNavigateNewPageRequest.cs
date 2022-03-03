namespace rail
{
	public class BrowserTryNavigateNewPageRequest : EventBase
	{
		public string url;

		public string target_type;

		public bool is_redirect_request;
	}
}
