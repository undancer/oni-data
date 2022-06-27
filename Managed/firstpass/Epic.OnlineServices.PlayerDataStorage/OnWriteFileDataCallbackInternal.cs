using System;

namespace Epic.OnlineServices.PlayerDataStorage
{
	internal delegate WriteResult OnWriteFileDataCallbackInternal(IntPtr messagePtr, IntPtr outDataBuffer, ref uint outDataWritten);
}
