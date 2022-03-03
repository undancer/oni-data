using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailScreenshotImpl : RailObject, IRailScreenshot, IRailComponent
	{
		internal IRailScreenshotImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailScreenshotImpl()
		{
		}

		public virtual bool SetLocation(string location)
		{
			return RAIL_API_PINVOKE.IRailScreenshot_SetLocation(swigCPtr_, location);
		}

		public virtual bool SetUsers(List<RailID> users)
		{
			IntPtr intPtr = ((users == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			if (users != null)
			{
				RailConverter.Csharp2Cpp(users, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailScreenshot_SetUsers(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
		}

		public virtual bool AssociatePublishedFiles(List<SpaceWorkID> work_files)
		{
			IntPtr intPtr = ((work_files == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (work_files != null)
			{
				RailConverter.Csharp2Cpp(work_files, intPtr);
			}
			try
			{
				return RAIL_API_PINVOKE.IRailScreenshot_AssociatePublishedFiles(swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}

		public virtual RailResult AsyncPublishScreenshot(string work_name, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailScreenshot_AsyncPublishScreenshot(swigCPtr_, work_name, user_data);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(swigCPtr_);
		}
	}
}
