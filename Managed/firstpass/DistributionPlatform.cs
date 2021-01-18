using System;
using UnityEngine;

public class DistributionPlatform : MonoBehaviour
{
	public interface Implementation
	{
		bool Initialized
		{
			get;
		}

		string Name
		{
			get;
		}

		string Platform
		{
			get;
		}

		string AccountLoginEndpoint
		{
			get;
		}

		string MetricsClientKey
		{
			get;
		}

		string MetricsUserIDField
		{
			get;
		}

		User LocalUser
		{
			get;
		}

		bool IsArchiveBranch
		{
			get;
		}

		bool PurchasedDLC
		{
			get;
		}

		bool IsExpansion1Active
		{
			get;
		}

		string ApplyWordFilter(string text);

		void GetAuthTicket(AuthTicketHandler callback);

		void ToggleDLC();
	}

	public delegate void AuthTicketHandler(byte[] ticket);

	public abstract class UserId
	{
		public abstract ulong ToInt64();
	}

	public abstract class User
	{
		public abstract UserId Id
		{
			get;
		}

		public abstract string Name
		{
			get;
		}
	}

	private static Implementation sImpl;

	public static bool Initialized => Impl.Initialized;

	public static Implementation Inst => Impl;

	private static Implementation Impl => sImpl;

	public static event System.Action onExitRequest;

	public static void Initialize()
	{
		if (sImpl == null)
		{
			sImpl = new GameObject("DistributionPlatform").AddComponent<SteamDistributionPlatform>();
			if (!SteamManager.Initialized)
			{
				Debug.LogError("Steam not initialized in time.");
			}
		}
	}

	public static void RequestExit()
	{
		if (DistributionPlatform.onExitRequest != null)
		{
			DistributionPlatform.onExitRequest();
		}
	}
}
