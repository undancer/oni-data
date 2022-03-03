namespace rail
{
	public class BrowserDamageRectNeedsPaintRequest : EventBase
	{
		public uint update_bgra_height;

		public uint scroll_x_pos;

		public string bgra_data;

		public uint update_bgra_width;

		public float page_scale_factor;

		public int update_offset_y;

		public int update_offset_x;

		public int offset_x;

		public int offset_y;

		public uint bgra_height;

		public uint scroll_y_pos;

		public uint bgra_width;
	}
}
