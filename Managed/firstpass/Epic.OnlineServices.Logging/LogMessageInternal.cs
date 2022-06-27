using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Logging
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LogMessageInternal : IDisposable
	{
		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Category;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Message;

		private LogLevel m_Level;

		public string Category
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Category, out target);
				return target;
			}
		}

		public string Message
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_Message, out target);
				return target;
			}
		}

		public LogLevel Level
		{
			get
			{
				LogLevel target = Helper.GetDefault<LogLevel>();
				Helper.TryMarshalGet(m_Level, out target);
				return target;
			}
		}

		public void Dispose()
		{
		}
	}
}
