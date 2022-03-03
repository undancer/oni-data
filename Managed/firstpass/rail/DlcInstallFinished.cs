namespace rail
{
	public class DlcInstallFinished : EventBase
	{
		public RailDlcID dlc_id = new RailDlcID();

		public new RailResult result;
	}
}
