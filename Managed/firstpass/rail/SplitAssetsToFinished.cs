namespace rail
{
	public class SplitAssetsToFinished : EventBase
	{
		public ulong source_asset;

		public uint to_quantity;

		public ulong split_to_asset_id;
	}
}
