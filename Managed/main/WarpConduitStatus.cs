using UnityEngine;

public static class WarpConduitStatus
{
	public static readonly Operational.Flag warpConnectedFlag = new Operational.Flag("warp_conduit_connected", Operational.Flag.Type.Requirement);

	public static void UpdateWarpConduitsOperational(GameObject sender, GameObject receiver)
	{
		bool flag = sender != null && sender.GetComponent<Activatable>().IsActivated;
		bool flag2 = receiver != null && receiver.GetComponent<Activatable>().IsActivated;
		bool value = flag && flag2;
		int num = 0;
		if (flag)
		{
			num++;
		}
		if (flag2)
		{
			num++;
		}
		if (sender != null)
		{
			sender.GetComponent<Operational>().SetFlag(warpConnectedFlag, value);
			if (num != 2)
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
				sender.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num);
			}
			else
			{
				sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
			}
		}
		if (receiver != null)
		{
			receiver.GetComponent<Operational>().SetFlag(warpConnectedFlag, value);
			if (num != 2)
			{
				receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
				receiver.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, num);
			}
			else
			{
				receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
			}
		}
	}
}
