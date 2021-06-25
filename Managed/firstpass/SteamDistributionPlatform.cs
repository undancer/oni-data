using System;
using Steamworks;
using UnityEngine;

internal class SteamDistributionPlatform : MonoBehaviour, DistributionPlatform.Implementation
{
	public class SteamUserId : DistributionPlatform.UserId
	{
		private CSteamID mSteamId;

		public SteamUserId(CSteamID id)
		{
			mSteamId = id;
		}

		public override string ToString()
		{
			return mSteamId.ToString();
		}

		public override ulong ToInt64()
		{
			return mSteamId.m_SteamID;
		}
	}

	public class SteamUser : DistributionPlatform.User
	{
		private SteamUserId mId;

		private string mName;

		public override DistributionPlatform.UserId Id => mId;

		public override string Name => mName;

		public SteamUser(CSteamID id, string name)
		{
			mId = new SteamUserId(id);
			mName = name;
		}
	}

	private SteamUser mLocalUser;

	public bool Initialized => SteamManager.Initialized;

	public string Name => "Steam";

	public string Platform => "Steam";

	public string AccountLoginEndpoint => "/login/LoginViaSteam";

	public string MetricsClientKey => "2Ehpf6QcWdCXV8eqbbiJBkrqD6xc8waX";

	public string MetricsUserIDField => "SteamUserID";

	public DistributionPlatform.User LocalUser
	{
		get
		{
			if (mLocalUser == null)
			{
				InitializeLocalUser();
			}
			return mLocalUser;
		}
	}

	public bool IsArchiveBranch
	{
		get
		{
			SteamApps.GetCurrentBetaName(out var pchName, 100);
			Debug.Log("Checking which steam branch we're on. Got: [" + pchName + "]");
			if (!(pchName == "default"))
			{
				return !(pchName == "release");
			}
			return false;
		}
	}

	public bool PurchasedDLC
	{
		get
		{
			bool purchasedDLC = false;
			if (SteamManager.Initialized)
			{
				GetAuthTicket(delegate(byte[] ticket)
				{
					CSteamID steamID = Steamworks.SteamUser.GetSteamID();
					Steamworks.SteamUser.BeginAuthSession(ticket, ticket.Length, steamID);
					EUserHasLicenseForAppResult eUserHasLicenseForAppResult = Steamworks.SteamUser.UserHasLicenseForApp(steamID, new AppId_t(1452490u));
					purchasedDLC = eUserHasLicenseForAppResult == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense;
					Steamworks.SteamUser.EndAuthSession(steamID);
				});
			}
			return purchasedDLC;
		}
	}

	public bool IsExpansion1Active => false;

	public string ApplyWordFilter(string text)
	{
		return text;
	}

	public void GetAuthTicket(DistributionPlatform.AuthTicketHandler handler)
	{
		uint pcbTicket = 0u;
		byte[] array = new byte[2048];
		Steamworks.SteamUser.GetAuthSessionTicket(array, array.Length, out pcbTicket);
		byte[] array2 = new byte[pcbTicket];
		if (0 < pcbTicket)
		{
			Array.Copy(array, array2, pcbTicket);
		}
		handler(array2);
	}

	public void ToggleDLC()
	{
		Debug.Log("Steam: Toggling DLC");
		if (PurchasedDLC)
		{
			if (IsExpansion1Active)
			{
				SteamApps.UninstallDLC(new AppId_t(1452490u));
				Debug.Log("Switching to base game");
			}
			else
			{
				SteamApps.InstallDLC(new AppId_t(1452490u));
				Debug.Log("Switching to Spaced Out");
			}
			SteamApps.MarkContentCorrupt(bMissingFilesOnly: false);
			Application.OpenURL("steam://rungameid/" + 457140u);
			App.Quit();
		}
	}

	private void InitializeLocalUser()
	{
		if (SteamManager.Initialized)
		{
			CSteamID steamID = Steamworks.SteamUser.GetSteamID();
			string personaName = SteamFriends.GetPersonaName();
			mLocalUser = new SteamUser(steamID, personaName);
		}
	}
}
