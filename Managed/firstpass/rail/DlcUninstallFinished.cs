namespace rail
{
	public class DlcUninstallFinished : EventBase
	{
		public RailDlcID dlc_id = new RailDlcID();

		public new RailResult result;
	}
}
