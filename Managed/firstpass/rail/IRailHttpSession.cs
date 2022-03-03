using System.Collections.Generic;

namespace rail
{
	public interface IRailHttpSession : IRailComponent
	{
		RailResult SetRequestMethod(RailHttpSessionMethod method);

		RailResult SetParameters(List<RailKeyValue> parameters);

		RailResult SetPostBodyContent(string body_content);

		RailResult SetRequestTimeOut(uint timeout_secs);

		RailResult SetRequestHeaders(List<string> headers);

		RailResult AsyncSendRequest(string url, string user_data);
	}
}
