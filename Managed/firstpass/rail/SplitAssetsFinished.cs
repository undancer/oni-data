namespace rail
{
	public class SplitAssetsFinished : EventBase
	{
		public ulong source_asset;

		public uint to_quantity;

		public ulong new_asset_id;
	}
}
