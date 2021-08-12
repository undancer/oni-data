using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Steamworks
{
	public static class CallbackDispatcher
	{
		private static Dictionary<int, List<Callback>> m_registeredCallbacks = new Dictionary<int, List<Callback>>();

		private static Dictionary<int, List<Callback>> m_registeredGameServerCallbacks = new Dictionary<int, List<Callback>>();

		private static Dictionary<ulong, List<CallResult>> m_registeredCallResults = new Dictionary<ulong, List<CallResult>>();

		private static object m_sync = new object();

		private static IntPtr m_pCallbackMsg;

		private static int m_initCount;

		public static bool IsInitialized => m_initCount > 0;

		public static void ExceptionHandler(Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}

		internal static void Initialize()
		{
			lock (m_sync)
			{
				if (m_initCount == 0)
				{
					NativeMethods.SteamAPI_ManualDispatch_Init();
					m_pCallbackMsg = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CallbackMsg_t)));
				}
				m_initCount++;
			}
		}

		internal static void Shutdown()
		{
			lock (m_sync)
			{
				m_initCount--;
				if (m_initCount == 0)
				{
					UnregisterAll();
					Marshal.FreeHGlobal(m_pCallbackMsg);
					m_pCallbackMsg = IntPtr.Zero;
				}
			}
		}

		internal static void Register(Callback cb)
		{
			int callbackIdentity = CallbackIdentities.GetCallbackIdentity(cb.GetCallbackType());
			Dictionary<int, List<Callback>> dictionary = (cb.IsGameServer ? m_registeredGameServerCallbacks : m_registeredCallbacks);
			lock (m_sync)
			{
				if (!dictionary.TryGetValue(callbackIdentity, out var value))
				{
					value = new List<Callback>();
					dictionary.Add(callbackIdentity, value);
				}
				value.Add(cb);
			}
		}

		internal static void Register(SteamAPICall_t asyncCall, CallResult cr)
		{
			lock (m_sync)
			{
				if (!m_registeredCallResults.TryGetValue((ulong)asyncCall, out var value))
				{
					value = new List<CallResult>();
					m_registeredCallResults.Add((ulong)asyncCall, value);
				}
				value.Add(cr);
			}
		}

		internal static void Unregister(Callback cb)
		{
			int callbackIdentity = CallbackIdentities.GetCallbackIdentity(cb.GetCallbackType());
			Dictionary<int, List<Callback>> dictionary = (cb.IsGameServer ? m_registeredGameServerCallbacks : m_registeredCallbacks);
			lock (m_sync)
			{
				if (dictionary.TryGetValue(callbackIdentity, out var value))
				{
					value.Remove(cb);
					if (value.Count == 0)
					{
						dictionary.Remove(callbackIdentity);
					}
				}
			}
		}

		internal static void Unregister(SteamAPICall_t asyncCall, CallResult cr)
		{
			lock (m_sync)
			{
				if (m_registeredCallResults.TryGetValue((ulong)asyncCall, out var value))
				{
					value.Remove(cr);
					if (value.Count == 0)
					{
						m_registeredCallResults.Remove((ulong)asyncCall);
					}
				}
			}
		}

		private static void UnregisterAll()
		{
			List<Callback> list = new List<Callback>();
			List<CallResult> list2 = new List<CallResult>();
			lock (m_sync)
			{
				foreach (KeyValuePair<int, List<Callback>> registeredCallback in m_registeredCallbacks)
				{
					list.AddRange(registeredCallback.Value);
				}
				m_registeredCallbacks.Clear();
				foreach (KeyValuePair<int, List<Callback>> registeredGameServerCallback in m_registeredGameServerCallbacks)
				{
					list.AddRange(registeredGameServerCallback.Value);
				}
				m_registeredGameServerCallbacks.Clear();
				foreach (KeyValuePair<ulong, List<CallResult>> registeredCallResult in m_registeredCallResults)
				{
					list2.AddRange(registeredCallResult.Value);
				}
				m_registeredCallResults.Clear();
				foreach (Callback item in list)
				{
					item.SetUnregistered();
				}
				foreach (CallResult item2 in list2)
				{
					item2.SetUnregistered();
				}
			}
		}

		internal static void RunFrame(bool isGameServer)
		{
			if (!IsInitialized)
			{
				throw new InvalidOperationException("Callback dispatcher is not initialized.");
			}
			HSteamPipe hSteamPipe = (HSteamPipe)(isGameServer ? NativeMethods.SteamGameServer_GetHSteamPipe() : NativeMethods.SteamAPI_GetHSteamPipe());
			NativeMethods.SteamAPI_ManualDispatch_RunFrame(hSteamPipe);
			Dictionary<int, List<Callback>> dictionary = (isGameServer ? m_registeredGameServerCallbacks : m_registeredCallbacks);
			while (NativeMethods.SteamAPI_ManualDispatch_GetNextCallback(hSteamPipe, m_pCallbackMsg))
			{
				CallbackMsg_t callbackMsg_t = (CallbackMsg_t)Marshal.PtrToStructure(m_pCallbackMsg, typeof(CallbackMsg_t));
				try
				{
					if (callbackMsg_t.m_iCallback == 703)
					{
						SteamAPICallCompleted_t steamAPICallCompleted_t = (SteamAPICallCompleted_t)Marshal.PtrToStructure(callbackMsg_t.m_pubParam, typeof(SteamAPICallCompleted_t));
						IntPtr intPtr = Marshal.AllocHGlobal((int)steamAPICallCompleted_t.m_cubParam);
						if (NativeMethods.SteamAPI_ManualDispatch_GetAPICallResult(hSteamPipe, steamAPICallCompleted_t.m_hAsyncCall, intPtr, (int)steamAPICallCompleted_t.m_cubParam, steamAPICallCompleted_t.m_iCallback, out var pbFailed))
						{
							lock (m_sync)
							{
								if (m_registeredCallResults.TryGetValue((ulong)steamAPICallCompleted_t.m_hAsyncCall, out var value))
								{
									m_registeredCallResults.Remove((ulong)steamAPICallCompleted_t.m_hAsyncCall);
									foreach (CallResult item in value)
									{
										item.OnRunCallResult(intPtr, pbFailed, (ulong)steamAPICallCompleted_t.m_hAsyncCall);
										item.SetUnregistered();
									}
								}
							}
						}
						Marshal.FreeHGlobal(intPtr);
					}
					else
					{
						if (!dictionary.TryGetValue(callbackMsg_t.m_iCallback, out var value2))
						{
							continue;
						}
						List<Callback> list;
						lock (m_sync)
						{
							list = new List<Callback>(value2);
						}
						foreach (Callback item2 in list)
						{
							item2.OnRunCallback(callbackMsg_t.m_pubParam);
						}
						continue;
					}
				}
				catch (Exception e)
				{
					ExceptionHandler(e);
				}
				finally
				{
					NativeMethods.SteamAPI_ManualDispatch_FreeLastCallback(hSteamPipe);
				}
			}
		}
	}
}
