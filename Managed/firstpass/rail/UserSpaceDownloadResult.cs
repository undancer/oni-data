using System.Collections.Generic;

namespace rail
{
	public class UserSpaceDownloadResult : EventBase
	{
		public uint total_results;

		public List<RailUserSpaceDownloadResult> results = new List<RailUserSpaceDownloadResult>();
	}
}
