using System.Collections.Generic;

namespace rail
{
	public class RailGetImageDataResult : EventBase
	{
		public List<byte> image_data = new List<byte>();

		public RailImageDataDescriptor image_data_descriptor = new RailImageDataDescriptor();
	}
}
