using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public abstract class Callback
	{
		public abstract bool IsGameServer
		{
			get;
		}

		internal abstract Type GetCallbackType();

		internal abstract void OnRunCallback(IntPtr pvParam);

		internal abstract void SetUnregistered();
	}
	public sealed class Callback<T> : Callback, IDisposable
	{
		public delegate void DispatchDelegate(T param);

		private bool m_bGameServer;

		private bool m_bIsRegistered;

		private bool m_bDisposed;

		public override bool IsGameServer => m_bGameServer;

		private event DispatchDelegate m_Func;

		public static Callback<T> Create(DispatchDelegate func)
		{
			return new Callback<T>(func);
		}

		public static Callback<T> CreateGameServer(DispatchDelegate func)
		{
			return new Callback<T>(func, bGameServer: true);
		}

		public Callback(DispatchDelegate func, bool bGameServer = false)
		{
			m_bGameServer = bGameServer;
			Register(func);
		}

		~Callback()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!m_bDisposed)
			{
				GC.SuppressFinalize(this);
				if (m_bIsRegistered)
				{
					Unregister();
				}
				m_bDisposed = true;
			}
		}

		public void Register(DispatchDelegate func)
		{
			if (func == null)
			{
				throw new Exception("Callback function must not be null.");
			}
			if (m_bIsRegistered)
			{
				Unregister();
			}
			this.m_Func = func;
			CallbackDispatcher.Register(this);
			m_bIsRegistered = true;
		}

		public void Unregister()
		{
			CallbackDispatcher.Unregister(this);
			m_bIsRegistered = false;
		}

		internal override Type GetCallbackType()
		{
			return typeof(T);
		}

		internal override void OnRunCallback(IntPtr pvParam)
		{
			try
			{
				this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)));
			}
			catch (Exception e)
			{
				CallbackDispatcher.ExceptionHandler(e);
			}
		}

		internal override void SetUnregistered()
		{
			m_bIsRegistered = false;
		}
	}
}
