using UnityEngine;

namespace Database
{
	public class RobotStatusItems : StatusItems
	{
		public StatusItem LowBattery;

		public StatusItem LowBatteryNoCharge;

		public StatusItem DeadBattery;

		public StatusItem CantReachStation;

		public StatusItem DustBinFull;

		public StatusItem Working;

		public StatusItem UnloadingStorage;

		public StatusItem ReactPositive;

		public StatusItem ReactNegative;

		public StatusItem MovingToChargeStation;

		public RobotStatusItems(ResourceSet parent)
			: base("RobotStatusItems", parent)
		{
			CreateStatusItems();
		}

		private void CreateStatusItems()
		{
			CantReachStation = new StatusItem("CantReachStation", "ROBOTS", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			CantReachStation.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go8 = (GameObject)data;
				return str.Replace("{0}", go8.GetProperName());
			};
			LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			LowBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go7 = (GameObject)data;
				return str.Replace("{0}", go7.GetProperName());
			};
			LowBatteryNoCharge = new StatusItem("LowBatteryNoCharge", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			LowBatteryNoCharge.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go6 = (GameObject)data;
				return str.Replace("{0}", go6.GetProperName());
			};
			DeadBattery = new StatusItem("DeadBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			DeadBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go5 = (GameObject)data;
				return str.Replace("{0}", go5.GetProperName());
			};
			DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			DustBinFull.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go4 = (GameObject)data;
				return str.Replace("{0}", go4.GetProperName());
			};
			Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Working.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go3 = (GameObject)data;
				return str.Replace("{0}", go3.GetProperName());
			};
			MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			MovingToChargeStation.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go2 = (GameObject)data;
				return str.Replace("{0}", go2.GetProperName());
			};
			UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			UnloadingStorage.resolveStringCallback = delegate(string str, object data)
			{
				GameObject go = (GameObject)data;
				return str.Replace("{0}", go.GetProperName());
			};
			ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactPositive.resolveStringCallback = (string str, object data) => str;
			ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactNegative.resolveStringCallback = (string str, object data) => str;
		}
	}
}
