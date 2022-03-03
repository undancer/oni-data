using System;

namespace rail
{
	public class IRailInGameCoinImpl : RailObject, IRailInGameCoin
	{
		internal IRailInGameCoinImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailInGameCoinImpl()
		{
		}

		public virtual RailResult AsyncRequestCoinInfo(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameCoin_AsyncRequestCoinInfo(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncPurchaseCoins(RailCoins purchase_info, string user_data)
		{
			IntPtr intPtr = ((purchase_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailCoins__SWIG_0());
			if (purchase_info != null)
			{
				RailConverter.Csharp2Cpp(purchase_info, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailInGameCoin_AsyncPurchaseCoins(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailCoins(intPtr);
			}
		}
	}
}
