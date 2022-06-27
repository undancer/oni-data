using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct InfoInternal : IDisposable
	{
		private int m_ApiVersion;

		private Status m_Status;

		private IntPtr m_UserId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Platform;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_RichText;

		private int m_RecordsCount;

		private IntPtr m_Records;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ProductName;

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

		public Status Status
		{
			get
			{
				Status target = Helper.GetDefault<Status>();
				Helper.TryMarshalGet(m_Status, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Status, value);
			}
		}

		public EpicAccountId UserId
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_UserId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_UserId, value);
			}
		}

		public string ProductId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductId, value);
			}
		}

		public string ProductVersion
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductVersion, value);
			}
		}

		public string Platform
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Platform, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Platform, value);
			}
		}

		public string RichText
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_RichText, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_RichText, value);
			}
		}

		public DataRecordInternal[] Records
		{
			get
			{
				DataRecordInternal[] target = Helper.GetDefault<DataRecordInternal[]>();
				Helper.TryMarshalGet(m_Records, out target, m_RecordsCount);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Records, value, out m_RecordsCount);
			}
		}

		public string ProductName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ProductName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ProductName, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Records);
		}
	}
}
