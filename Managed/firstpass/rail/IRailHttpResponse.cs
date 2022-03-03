using System.Collections.Generic;

namespace rail
{
	public interface IRailHttpResponse : IRailComponent
	{
		int GetHttpResponseCode();

		RailResult GetResponseHeaderKeys(List<string> header_keys);

		string GetResponseHeaderValue(string header_key);

		string GetResponseBodyData();

		uint GetContentLength();

		string GetContentType();

		string GetContentRange();

		string GetContentLanguage();

		string GetContentEncoding();

		string GetLastModified();
	}
}
