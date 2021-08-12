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
				GameObject gameObject8 = (GameObject)data;
				return str.Replace("{0}", gameObject8.name);
			};
			LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			LowBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject7 = (GameObject)data;
				return str.Replace("{0}", gameObject7.name);
			};
			LowBatteryNoCharge = new StatusItem("LowBatteryNoCharge", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			LowBatteryNoCharge.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject6 = (GameObject)data;
				return str.Replace("{0}", gameObject6.name);
			};
			DeadBattery = new StatusItem("DeadBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			DeadBattery.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject5 = (GameObject)data;
				return str.Replace("{0}", gameObject5.name);
			};
			DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			DustBinFull.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject4 = (GameObject)data;
				return str.Replace("{0}", gameObject4.name);
			};
			Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Working.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject3 = (GameObject)data;
				return str.Replace("{0}", gameObject3.name);
			};
			MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			MovingToChargeStation.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject2 = (GameObject)data;
				return str.Replace("{0}", gameObject2.name);
			};
			UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			UnloadingStorage.resolveStringCallback = delegate(string str, object data)
			{
				GameObject gameObject = (GameObject)data;
				return str.Replace("{0}", gameObject.name);
			};
			ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactPositive.resolveStringCallback = (string str, object data) => str;
			ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactNegative.resolveStringCallback = (string str, object data) => str;
		}
	}
}
