using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailUserSpaceHelperImpl : RailObject, IRailUserSpaceHelper
	{
		internal IRailUserSpaceHelperImpl(IntPtr cPtr)
		{
			swigCPtr_ = cPtr;
		}

		~IRailUserSpaceHelperImpl()
		{
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_0(swigCPtr_, offset, max_works, (int)type, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_1(swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
		}

		public virtual RailResult AsyncGetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMySubscribedWorks__SWIG_2(swigCPtr_, offset, max_works, (int)type);
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_0(swigCPtr_, offset, max_works, (int)type, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_1(swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr);
			}
		}

		public virtual RailResult AsyncGetMyFavoritesWorks(uint offset, uint max_works, EnumRailSpaceWorkType type)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncGetMyFavoritesWorks__SWIG_2(swigCPtr_, offset, max_works, (int)type);
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, RailQueryWorkFileOptions options, string user_data)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_0(swigCPtr_, intPtr, offset, max_works, (int)order_by, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
			}
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, RailQueryWorkFileOptions options)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_1(swigCPtr_, intPtr, offset, max_works, (int)order_by, intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
			}
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_2(swigCPtr_, intPtr, offset, max_works, (int)order_by);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
			}
		}

		public virtual RailResult AsyncQuerySpaceWorks(RailSpaceWorkFilter filter, uint offset, uint max_works)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorks__SWIG_3(swigCPtr_, intPtr, offset, max_works);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkFilter(intPtr);
			}
		}

		public virtual RailResult AsyncSubscribeSpaceWorks(List<SpaceWorkID> ids, bool subscribe, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncSubscribeSpaceWorks(swigCPtr_, intPtr, subscribe, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}

		public virtual IRailSpaceWork OpenSpaceWork(SpaceWorkID id)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailUserSpaceHelper_OpenSpaceWork(swigCPtr_, intPtr);
				return (intPtr2 == IntPtr.Zero) ? null : new IRailSpaceWorkImpl(intPtr2);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
		}

		public virtual IRailSpaceWork CreateSpaceWork(EnumRailSpaceWorkType type)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailUserSpaceHelper_CreateSpaceWork(swigCPtr_, (int)type);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailSpaceWorkImpl(intPtr);
			}
			return null;
		}

		public virtual RailResult GetMySubscribedWorks(uint offset, uint max_works, EnumRailSpaceWorkType type, QueryMySubscribedSpaceWorksResult result)
		{
			IntPtr intPtr = ((result == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_QueryMySubscribedSpaceWorksResult__SWIG_0());
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_GetMySubscribedWorks(swigCPtr_, offset, max_works, (int)type, intPtr);
			}
			finally
			{
				if (result != null)
				{
					RailConverter.Cpp2Csharp(intPtr, result);
				}
				RAIL_API_PINVOKE.delete_QueryMySubscribedSpaceWorksResult(intPtr);
			}
		}

		public virtual uint GetMySubscribedWorksCount(EnumRailSpaceWorkType type, out RailResult result)
		{
			return RAIL_API_PINVOKE.IRailUserSpaceHelper_GetMySubscribedWorksCount(swigCPtr_, (int)type, out result);
		}

		public virtual RailResult AsyncRemoveSpaceWork(SpaceWorkID id, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncRemoveSpaceWork(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
		}

		public virtual RailResult AsyncModifyFavoritesWorks(List<SpaceWorkID> ids, EnumRailModifyFavoritesSpaceWorkType modify_flag, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncModifyFavoritesWorks(swigCPtr_, intPtr, (int)modify_flag, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}

		public virtual RailResult AsyncVoteSpaceWork(SpaceWorkID id, EnumRailSpaceWorkVoteValue vote, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncVoteSpaceWork(swigCPtr_, intPtr, (int)vote, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
		}

		public virtual RailResult AsyncSearchSpaceWork(RailSpaceWorkSearchFilter filter, RailQueryWorkFileOptions options, List<EnumRailSpaceWorkType> types, uint offset, uint max_works, EnumRailSpaceWorkOrderBy order_by, string user_data)
		{
			IntPtr intPtr = ((filter == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSpaceWorkSearchFilter__SWIG_0());
			if (filter != null)
			{
				RailConverter.Csharp2Cpp(filter, intPtr);
			}
			IntPtr intPtr2 = ((options == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailQueryWorkFileOptions__SWIG_0());
			if (options != null)
			{
				RailConverter.Csharp2Cpp(options, intPtr2);
			}
			IntPtr intPtr3 = ((types == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayEnumRailSpaceWorkType__SWIG_0());
			if (types != null)
			{
				RailConverter.Csharp2Cpp(types, intPtr3);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncSearchSpaceWork(swigCPtr_, intPtr, intPtr2, intPtr3, offset, max_works, (int)order_by, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSpaceWorkSearchFilter(intPtr);
				RAIL_API_PINVOKE.delete_RailQueryWorkFileOptions(intPtr2);
				RAIL_API_PINVOKE.delete_RailArrayEnumRailSpaceWorkType(intPtr3);
			}
		}

		public virtual RailResult AsyncRateSpaceWork(SpaceWorkID id, EnumRailSpaceWorkRateValue mark, string user_data)
		{
			IntPtr intPtr = ((id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_SpaceWorkID__SWIG_0());
			if (id != null)
			{
				RailConverter.Csharp2Cpp(id, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncRateSpaceWork(swigCPtr_, intPtr, (int)mark, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_SpaceWorkID(intPtr);
			}
		}

		public virtual RailResult AsyncQuerySpaceWorksInfo(List<SpaceWorkID> ids, string user_data)
		{
			IntPtr intPtr = ((ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArraySpaceWorkID__SWIG_0());
			if (ids != null)
			{
				RailConverter.Csharp2Cpp(ids, intPtr);
			}
			try
			{
				return (RailResult)RAIL_API_PINVOKE.IRailUserSpaceHelper_AsyncQuerySpaceWorksInfo(swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArraySpaceWorkID(intPtr);
			}
		}
	}
}
