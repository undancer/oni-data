using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct BeginPlayerSessionOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private BeginPlayerSessionOptionsAccountIdInternal m_AccountId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		private UserControllerType m_ControllerType;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_ServerIp;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_GameSessionId;

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

		public BeginPlayerSessionOptionsAccountIdInternal AccountId
		{
			get
			{
				BeginPlayerSessionOptionsAccountIdInternal target = Helper.GetDefault<BeginPlayerSessionOptionsAccountIdInternal>();
				Helper.TryMarshalGet(m_AccountId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AccountId, value);
			}
		}

		public string DisplayName
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_DisplayName, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_DisplayName, value);
			}
		}

		public UserControllerType ControllerType
		{
			get
			{
				UserControllerType target = Helper.GetDefault<UserControllerType>();
				Helper.TryMarshalGet(m_ControllerType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ControllerType, value);
			}
		}

		public string ServerIp
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_ServerIp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ServerIp, value);
			}
		}

		public string GameSessionId
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_GameSessionId, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_GameSessionId, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_AccountId);
		}
	}
}
