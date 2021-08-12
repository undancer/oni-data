using System.Diagnostics;
using Steamworks;
using UnityEngine;

public class SteamAchievementService : MonoBehaviour
{
	private Callback<UserStatsReceived_t> cbUserStatsReceived;

	private Callback<UserStatsStored_t> cbUserStatsStored;

	private Callback<UserAchievementStored_t> cbUserAchievementStored;

	private bool setupComplete;

	private static SteamAchievementService instance;

	public static SteamAchievementService Instance => instance;

	public static void Initialize()
	{
		if (instance == null)
		{
			GameObject gameObject = GameObject.Find("/SteamManager");
			instance = gameObject.GetComponent<SteamAchievementService>();
			if (instance == null)
			{
				instance = gameObject.AddComponent<SteamAchievementService>();
			}
		}
	}

	public void Awake()
	{
		setupComplete = false;
		Debug.Assert(instance == null);
		instance = this;
	}

	private void OnDestroy()
	{
		Debug.Assert(instance == this);
		instance = null;
	}

	private void Update()
	{
		if (SteamManager.Initialized && !(Game.Instance != null) && !setupComplete && DistributionPlatform.Initialized)
		{
			Setup();
		}
	}

	private void Setup()
	{
		cbUserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		cbUserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		cbUserAchievementStored = Callback<UserAchievementStored_t>.Create(OnUserAchievementStored);
		setupComplete = true;
		RefreshStats();
	}

	private void RefreshStats()
	{
		SteamUserStats.RequestCurrentStats();
	}

	private void OnUserStatsReceived(UserStatsReceived_t data)
	{
		if (data.m_eResult != EResult.k_EResultOK)
		{
			DebugUtil.LogWarningArgs("OnUserStatsReceived", data.m_eResult, data.m_steamIDUser);
		}
	}

	private void OnUserStatsStored(UserStatsStored_t data)
	{
		if (data.m_eResult != EResult.k_EResultOK)
		{
			DebugUtil.LogWarningArgs("OnUserStatsStored", data.m_eResult);
		}
	}

	private void OnUserAchievementStored(UserAchievementStored_t data)
	{
	}

	public void Unlock(string achievement_id)
	{
		bool flag = SteamUserStats.SetAchievement(achievement_id);
		Debug.LogFormat("SetAchievement {0} {1}", achievement_id, flag);
		bool flag2 = SteamUserStats.StoreStats();
		Debug.LogFormat("StoreStats {0}", flag2);
	}

	[Conditional("UNITY_EDITOR")]
	[ContextMenu("Reset All Achievements")]
	private void ResetAllAchievements()
	{
		bool flag = SteamUserStats.ResetAllStats(bAchievementsToo: true);
		Debug.LogFormat("ResetAllStats {0}", flag);
		if (flag)
		{
			RefreshStats();
		}
	}
}
