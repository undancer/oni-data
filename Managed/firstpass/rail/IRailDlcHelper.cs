using System.Collections.Generic;

namespace rail
{
	public interface IRailDlcHelper
	{
		RailResult AsyncQueryIsOwnedDlcsOnServer(List<RailDlcID> dlc_ids, string user_data);

		RailResult AsyncCheckAllDlcsStateReady(string user_data);

		bool IsDlcInstalled(RailDlcID dlc_id, out string installed_path);

		bool IsDlcInstalled(RailDlcID dlc_id);

		bool IsOwnedDlc(RailDlcID dlc_id);

		uint GetDlcCount();

		bool GetDlcInfo(uint index, RailDlcInfo dlc_info);

		bool AsyncInstallDlc(RailDlcID dlc_id, string user_data);

		bool AsyncRemoveDlc(RailDlcID dlc_id, string user_data);
	}
}
