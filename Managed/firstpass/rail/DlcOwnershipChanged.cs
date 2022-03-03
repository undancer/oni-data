namespace rail
{
	public class DlcOwnershipChanged : EventBase
	{
		public RailDlcID dlc_id = new RailDlcID();

		public bool is_active;
	}
}
