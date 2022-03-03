using System;

namespace rail
{
	public class IRailThirdPartyAccountLoginHelperImpl : RailObject, IRailThirdPartyAccountLoginHelper
	{
		internal IRailThirdPartyAccountLoginHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailThirdPartyAccountLoginHelperImpl()
		{
		}

		public virtual RailResult AsyncAutoLogin(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailThirdPartyAccountLoginHelper_AsyncAutoLogin(swigCPtr_, user_data);
		}

		public virtual RailResult AsyncLogin(RailThirdPartyAccountLoginOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailThirdPartyAccountLoginOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailThirdPartyAccountLoginHelper_AsyncLogin(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailThirdPartyAccountLoginOptions(intPtr);
			}
		}

		public virtual RailResult GetAccountInfo(RailThirdPartyAccountInfo account_info)
		{
			IntPtr intPtr = ((account_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailThirdPartyAccountInfo__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailThirdPartyAccountLoginHelper_GetAccountInfo(swigCPtr_, intPtr);
			}
			finally
			{
				if (account_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, account_info);
				}
				RAIL_API_PINVOKE.delete_RailThirdPartyAccountInfo(intPtr);
			}
		}
	}
}
