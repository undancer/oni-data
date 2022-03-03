using System.Collections.Generic;

namespace rail
{
	public class AsyncListFileResult : EventBase
	{
		public List<RailStreamFileInfo> file_list = new List<RailStreamFileInfo>();

		public uint try_list_file_num;

		public uint all_file_num;

		public uint start_index;
	}
}
