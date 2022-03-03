namespace rail
{
	public interface IRailScreenshotHelper
	{
		IRailScreenshot CreateScreenshotWithRawData(byte[] rgb_data, uint len, uint width, uint height);

		IRailScreenshot CreateScreenshotWithLocalImage(string image_file, string thumbnail_file);

		void AsyncTakeScreenshot(string user_data);

		void HookScreenshotHotKey(bool hook);

		bool IsScreenshotHotKeyHooked();
	}
}
