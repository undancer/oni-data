namespace Epic.OnlineServices.PlayerDataStorage
{
	public delegate WriteResult OnWriteFileDataCallback(WriteFileDataCallbackInfo data, out byte[] outDataBuffer, out uint outDataWritten);
}
