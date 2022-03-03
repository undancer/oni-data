namespace rail
{
	public class BrowserNeedsPaintRequest : EventBase
	{
		public uint bgra_width;

		public uint scroll_y_pos;

		public string bgra_data;

		public float page_scale_factor;

		public int offset_x;

		public uint scroll_x_pos;

		public uint bgra_height;

		public int offset_y;
	}
}
