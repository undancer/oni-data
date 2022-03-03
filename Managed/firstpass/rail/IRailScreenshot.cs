using System.Collections.Generic;

namespace rail
{
	public interface IRailScreenshot : IRailComponent
	{
		bool SetLocation(string location);

		bool SetUsers(List<RailID> users);

		bool AssociatePublishedFiles(List<SpaceWorkID> work_files);

		RailResult AsyncPublishScreenshot(string work_name, string user_data);
	}
}
