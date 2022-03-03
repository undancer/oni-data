using System;

namespace rail
{
	public class IRailBrowserHelperImpl : RailObject, IRailBrowserHelper
	{
		internal IRailBrowserHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailBrowserHelperImpl()
		{
		}

		public virtual IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data, CreateBrowserOptions options, out RailResult result)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateBrowserOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailBrowserHelper_AsyncCreateBrowser__SWIG_0(swigCPtr_, url, window_width, window_height, user_data, intPtr, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailBrowserImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateBrowserOptions(intPtr);
			}
		}

		public virtual IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data, CreateBrowserOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateBrowserOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailBrowserHelper_AsyncCreateBrowser__SWIG_1(swigCPtr_, url, window_width, window_height, user_data, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailBrowserImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateBrowserOptions(intPtr);
			}
		}

		public virtual IRailBrowser AsyncCreateBrowser(string url, uint window_width, uint window_height, string user_data)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailBrowserHelper_AsyncCreateBrowser__SWIG_2(swigCPtr_, url, window_width, window_height, user_data);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailBrowserImpl(intPtr);
			}
			return null;
		}

		public virtual IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data, CreateCustomerDrawBrowserOptions options, out RailResult result)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateCustomerDrawBrowserOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_0(swigCPtr_, url, user_data, intPtr, out result);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailBrowserRenderImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateCustomerDrawBrowserOptions(intPtr);
			}
		}

		public virtual IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data, CreateCustomerDrawBrowserOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_CreateCustomerDrawBrowserOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_1(swigCPtr_, url, user_data, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailBrowserRenderImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_CreateCustomerDrawBrowserOptions(intPtr);
			}
		}

		public virtual IRailBrowserRender CreateCustomerDrawBrowser(string url, string user_data)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailBrowserHelper_CreateCustomerDrawBrowser__SWIG_2(swigCPtr_, url, user_data);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailBrowserRenderImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult NavigateWebPage(string url, bool display_in_new_tab)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailBrowserHelper_NavigateWebPage(swigCPtr_, url, display_in_new_tab);
		}
	}
}
