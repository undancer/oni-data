using UnityEngine;

public static class WarpConduitStatus
{
	public static readonly Operational.Flag warpConnectedFlag = new Operational.Flag("warp_conduit_connected", Operational.Flag.Type.Requirement);

	public static void UpdateWarpConduitsOperational(GameObject sender, GameObject receiver)
	{
		bool num = sender != null && sender.GetComponent<Activatable>().IsActivated;
		bool flag = receiver != null && receiver.GetComponent<Activatable>().IsActivated;
		bool value = num && flag;
		int num2 = 0;
		if (num)
		{
			num2++;
		}
		if (flag)
		{
			num2++;
		}
		if (sender != null)
		{
			sender.GetComponent<Operational>().SetFlag(warpConnectedFlag, value);
			if (num2 != 2)
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
				sender.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num2);
			}
			else
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
			}
		}
		if (receiver != null)
		{
			receiver.GetComponent<Operational>().SetFlag(warpConnectedFlag, value);
			if (num2 != 2)
			{
				receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
				receiver.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num2);
			}
			else
			{
				receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
			}
		}
	}
}
