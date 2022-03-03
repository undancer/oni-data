using System.Collections.Generic;

namespace rail
{
	public class UserSpaceDownloadProgress : EventBase
	{
		public List<RailUserSpaceDownloadProgress> progress = new List<RailUserSpaceDownloadProgress>();

		public uint total_progress;
	}
}
