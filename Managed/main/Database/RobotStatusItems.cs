namespace Database
{
	public class RobotStatusItems : StatusItems
	{
		public StatusItem LowBattery;

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
			CantReachStation.resolveStringCallback = (string str, object data) => str;
			LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			LowBattery.resolveStringCallback = (string str, object data) => str;
			DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			DustBinFull.resolveStringCallback = (string str, object data) => str;
			Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			Working.resolveStringCallback = (string str, object data) => str;
			MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			MovingToChargeStation.resolveStringCallback = (string str, object data) => str;
			UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			UnloadingStorage.resolveStringCallback = (string str, object data) => str;
			ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactPositive.resolveStringCallback = (string str, object data) => str;
			ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, showWorldIcon: false);
			ReactNegative.resolveStringCallback = (string str, object data) => str;
		}
	}
}
