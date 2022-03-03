namespace rail
{
	public class DlcRefundChanged : EventBase
	{
		public RailDlcID dlc_id = new RailDlcID();

		public EnumRailGameRefundState refund_state;
	}
}
