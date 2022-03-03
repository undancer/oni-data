namespace rail
{
	public class RailNetworkSessionState
	{
		public RailResult session_error;

		public ushort remote_port;

		public uint packets_in_send_buffer;

		public bool is_connecting;

		public uint bytes_in_send_buffer;

		public bool is_using_relay;

		public bool is_connection_active;

		public uint remote_ip;
	}
}
