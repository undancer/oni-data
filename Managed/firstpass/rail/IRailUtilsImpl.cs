using System;

namespace rail
{
	public class IRailUtilsImpl : RailObject, IRailUtils
	{
		internal IRailUtilsImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailUtilsImpl()
		{
		}

		public virtual uint GetTimeCountSinceGameLaunch()
		{
			return RAIL_API_PINVOKE.IRailUtils_GetTimeCountSinceGameLaunch(swigCPtr_);
		}

		public virtual uint GetTimeCountSinceComputerLaunch()
		{
			return RAIL_API_PINVOKE.IRailUtils_GetTimeCountSinceComputerLaunch(swigCPtr_);
		}

		public virtual uint GetTimeFromServer()
		{
			return RAIL_API_PINVOKE.IRailUtils_GetTimeFromServer(swigCPtr_);
		}

		public virtual RailResult AsyncGetImageData(string image_path, uint scale_to_width, uint scale_to_height, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUtils_AsyncGetImageData(swigCPtr_, image_path, scale_to_width, scale_to_height, user_data);
		}

		public virtual void GetErrorString(RailResult result, out string error_string)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				RAIL_API_PINVOKE.IRailUtils_GetErrorString(swigCPtr_, (int)result, intPtr);
			}
			finally
			{
				error_string = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult DirtyWordsFilter(string words, bool replace_sensitive, RailDirtyWordsCheckResult check_result)
		{
			IntPtr intPtr = ((check_result == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailDirtyWordsCheckResult__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUtils_DirtyWordsFilter(swigCPtr_, words, replace_sensitive, intPtr);
			}
			finally
			{
				if (check_result != null)
				{
					RailConverter.Cpp2Csharp(intPtr, check_result);
				}
				RAIL_API_PINVOKE.delete_RailDirtyWordsCheckResult(intPtr);
			}
		}

		public virtual EnumRailPlatformType GetRailPlatformType()
		{
			return (EnumRailPlatformType)RAIL_API_PINVOKE.IRailUtils_GetRailPlatformType(swigCPtr_);
		}

		public virtual RailResult GetLaunchAppParameters(EnumRailLaunchAppType app_type, out string parameter)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUtils_GetLaunchAppParameters(swigCPtr_, (int)app_type, intPtr);
			}
			finally
			{
				parameter = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult GetPlatformLanguageCode(out string language_code)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUtils_GetPlatformLanguageCode(swigCPtr_, intPtr);
			}
			finally
			{
				language_code = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}

		public virtual RailResult SetWarningMessageCallback(RailWarningMessageCallbackFunction callback)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUtils_SetWarningMessageCallback(swigCPtr_, callback);
		}

		public virtual RailResult GetCountryCodeOfCurrentLoggedInIP(out string country_code)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUtils_GetCountryCodeOfCurrentLoggedInIP(swigCPtr_, intPtr);
			}
			finally
			{
				country_code = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
		}
	}
}
