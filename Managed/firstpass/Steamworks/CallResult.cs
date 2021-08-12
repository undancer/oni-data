using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public abstract class CallResult
	{
		internal abstract Type GetCallbackType();

		internal abstract void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall);

		internal abstract void SetUnregistered();
	}
	public sealed class CallResult<T> : CallResult, IDisposable
	{
		public delegate void APIDispatchDelegate(T param, bool bIOFailure);

		private SteamAPICall_t m_hAPICall = SteamAPICall_t.Invalid;

		private bool m_bDisposed;

		public SteamAPICall_t Handle => m_hAPICall;

		private event APIDispatchDelegate m_Func;

		public static CallResult<T> Create(APIDispatchDelegate func = null)
		{
			return new CallResult<T>(func);
		}

		public CallResult(APIDispatchDelegate func = null)
		{
			this.m_Func = func;
		}

		~CallResult()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (!m_bDisposed)
			{
				GC.SuppressFinalize(this);
				Cancel();
				m_bDisposed = true;
			}
		}

		public void Set(SteamAPICall_t hAPICall, APIDispatchDelegate func = null)
		{
			if (func != null)
			{
				this.m_Func = func;
			}
			if (this.m_Func == null)
			{
				throw new Exception("CallResult function was null, you must either set it in the CallResult Constructor or via Set()");
			}
			if (m_hAPICall != SteamAPICall_t.Invalid)
			{
				CallbackDispatcher.Unregister(m_hAPICall, this);
			}
			m_hAPICall = hAPICall;
			if (hAPICall != SteamAPICall_t.Invalid)
			{
				CallbackDispatcher.Register(hAPICall, this);
			}
		}

		public bool IsActive()
		{
			return m_hAPICall != SteamAPICall_t.Invalid;
		}

		public void Cancel()
		{
			if (IsActive())
			{
				CallbackDispatcher.Unregister(m_hAPICall, this);
			}
		}

		internal override Type GetCallbackType()
		{
			return typeof(T);
		}

		internal override void OnRunCallResult(IntPtr pvParam, bool bFailed, ulong hSteamAPICall_)
		{
			if ((SteamAPICall_t)hSteamAPICall_ == m_hAPICall)
			{
				try
				{
					this.m_Func((T)Marshal.PtrToStructure(pvParam, typeof(T)), bFailed);
				}
				catch (Exception e)
				{
					CallbackDispatcher.ExceptionHandler(e);
				}
			}
		}

		internal override void SetUnregistered()
		{
			m_hAPICall = SteamAPICall_t.Invalid;
		}
	}
}
