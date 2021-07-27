namespace Epic.OnlineServices.P2P
{
	public enum ConnectionClosedReason
	{
		Unknown,
		ClosedByLocalUser,
		ClosedByPeer,
		TimedOut,
		TooManyConnections,
		InvalidMessage,
		InvalidData,
		ConnectionFailed,
		ConnectionClosed,
		NegotiationFailed,
		UnexpectedError
	}
}
