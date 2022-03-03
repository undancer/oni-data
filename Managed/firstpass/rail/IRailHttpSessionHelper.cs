namespace rail
{
	public interface IRailHttpSessionHelper
	{
		IRailHttpSession CreateHttpSession();

		IRailHttpResponse CreateHttpResponse(string http_response_data);
	}
}
