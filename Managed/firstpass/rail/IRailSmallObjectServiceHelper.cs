using System.Collections.Generic;

namespace rail
{
	public interface IRailSmallObjectServiceHelper
	{
		RailResult AsyncDownloadObjects(List<uint> indexes, string user_data);

		RailResult GetObjectContent(uint index, out string content);

		RailResult AsyncQueryObjectState(string user_data);
	}
}
