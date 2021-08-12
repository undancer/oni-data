using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.TitleStorage
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryFileListOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_ListOfTags;

		private uint m_ListOfTagsCount;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId target = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet(m_LocalUserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_LocalUserId, value);
			}
		}

		public string[] ListOfTags
		{
			get
			{
				string[] target = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet(m_ListOfTags, out target, m_ListOfTagsCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ListOfTags, value, out m_ListOfTagsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_ListOfTags);
		}
	}
}
