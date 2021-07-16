using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using UnityEngine;

public class KleiMetrics : ThreadedHttps<KleiMetrics>
{
	protected struct PostData
	{
		public string clientKey;

		public Dictionary<string, object> metricData;

		public PostData(string key, Dictionary<string, object> data)
		{
			clientKey = key;
			metricData = data;
		}
	}

	protected struct ExpansionsMetricsData
	{
		public string Name;

		public bool Activated;
	}

	private const string SessionIDKey = "SESSION_ID";

	private const string GameIDKey = "GAME_ID";

	private const string InstallTimeStampKey = "INSTALL_TIMESTAMP";

	private const string UserIDFieldName = "user";

	private const string SessionIDFieldName = "SessionID";

	private const string GameIDFieldName = "GameID";

	private const string InstallTimeStampFieldName = "InstallTimeStamp";

	private const string KleiUserFieldName = "KU";

	private const string StartSessionFieldName = "StartSession";

	private const string EndSessionFieldName = "EndSession";

	private const string EndSessionCrashedFieldName = "EndSessionCrashed";

	private const string SessionStartTimeStampFieldName = "SessionStartTimeStamp";

	private const string SessionTimeFieldName = "SessionTimeSeconds";

	private const string NewGameFieldName = "NewGame";

	private const string EndGameFieldName = "EndGame";

	public const string GameTimeFieldName = "GameTimeSeconds";

	private const string LevelFieldName = "Level";

	public const string BuildBranchName = "Branch";

	public const string BuildFieldName = "Build";

	private const int EDITOR_BUILD_ID = -1;

	private const string HeartBeatFieldName = "HeartBeat";

	private const string HeartBeatTimeOutFieldName = "HeartBeatTimeOut";

	private const string LastUserActionFieldName = "LastUA";

	public const string SaveFolderWriteTest = "SaveFolderWriteTest";

	public const string ExpansionsFieldName = "Expansions";

	private string PlatformUserIDFieldName;

	private static int sessionID = -1;

	private static int gameID = -1;

	private static string installTimeStamp = null;

	private static System.Timers.Timer heartbeatTimer;

	private int HeartBeatInSeconds = 180;

	private int HeartBeatTimeOutInSeconds = 1200;

	private long currentSessionTicks = DateTime.Now.Ticks;

	private float timeSinceLastUserAction;

	private long lastHeartBeatTicks = DateTime.Now.Ticks;

	private long startTimeTicks = DateTime.Now.Ticks;

	private bool shouldEndSession;

	private bool shouldStartSession;

	private bool hasStarted;

	private Dictionary<string, object> userSession = new Dictionary<string, object>();

	private Action<Dictionary<string, object>> SetDynamicSessionVariables;

	private System.Action SetStaticSessionVariables;

	private bool sessionStarted;

	private long sessionStartUtcTicks = DateTime.UtcNow.Ticks;

	public bool isMultiThreaded
	{
		get;
		protected set;
	}

	public bool enabled
	{
		get;
		private set;
	}

	public KleiMetrics()
	{
		LIVE_ENDPOINT = "oni.metrics.klei.com/write";
		serviceName = "KleiMetrics";
		CLIENT_KEY = DistributionPlatform.Inst.MetricsClientKey;
		PlatformUserIDFieldName = DistributionPlatform.Inst.MetricsUserIDField;
		sessionID = -1;
		enabled = !KPrivacyPrefs.instance.disableDataCollection;
		GameID();
		isMultiThreaded = true;
	}

	public void SetEnabled(bool enabled)
	{
		this.enabled = enabled;
	}

	protected string PostMetricData(Dictionary<string, object> data, string debug_source)
	{
		string s = JsonConvert.SerializeObject(new PostData(CLIENT_KEY, data));
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		if (isMultiThreaded)
		{
			PutPacket(bytes);
			return "OK";
		}
		return Send(bytes);
	}

	public static string PlatformUserID()
	{
		DistributionPlatform.User localUser = DistributionPlatform.Inst.LocalUser;
		if (localUser == null)
		{
			return "";
		}
		return localUser.Id.ToString();
	}

	public static string UserID()
	{
		DistributionPlatform.User localUser = DistributionPlatform.Inst.LocalUser;
		if (localUser == null)
		{
			return "";
		}
		return localUser.Id.ToString();
	}

	private void IncrementSessionCount()
	{
		sessionID = SessionID() + 1;
		KPlayerPrefs.SetInt("SESSION_ID", sessionID);
	}

	public static int SessionID()
	{
		if (sessionID == -1)
		{
			sessionID = KPlayerPrefs.GetInt("SESSION_ID", -1);
		}
		return sessionID;
	}

	private void IncrementGameCount()
	{
		gameID = GameID() + 1;
		SetGameID(gameID);
	}

	public static int GameID()
	{
		if (gameID == -1)
		{
			gameID = KPlayerPrefs.GetInt("GAME_ID", -1);
		}
		return gameID;
	}

	public static void SetGameID(int id)
	{
		KPlayerPrefs.SetInt("GAME_ID", id);
	}

	public static string GetInstallTimeStamp()
	{
		if (installTimeStamp == null)
		{
			installTimeStamp = KPlayerPrefs.GetString("INSTALL_TIMESTAMP", null);
			if (installTimeStamp == null || installTimeStamp == "")
			{
				installTimeStamp = DateTime.UtcNow.Ticks.ToString();
				KPlayerPrefs.SetString("INSTALL_TIMESTAMP", installTimeStamp);
			}
		}
		return installTimeStamp;
	}

	public static string CurrentLevel()
	{
		return null;
	}

	protected static ExpansionsMetricsData[] Expansions()
	{
		List<string> ownedDLCIds = DlcManager.GetOwnedDLCIds();
		ExpansionsMetricsData[] array = new ExpansionsMetricsData[ownedDLCIds.Count];
		for (int i = 0; i < ownedDLCIds.Count; i++)
		{
			array[i] = new ExpansionsMetricsData
			{
				Name = ownedDLCIds[i],
				Activated = DlcManager.IsContentActive(ownedDLCIds[i])
			};
		}
		return array;
	}

	public void SetLastUserAction(long lastUserActionTicks)
	{
		if (enabled && sessionStarted)
		{
			currentSessionTicks = DateTime.Now.Ticks;
			if (shouldEndSession)
			{
				EndSession();
				shouldEndSession = false;
				shouldStartSession = true;
			}
			else if (shouldStartSession && lastUserActionTicks > lastHeartBeatTicks)
			{
				StartSession();
				shouldStartSession = false;
			}
			timeSinceLastUserAction = (float)TimeSpan.FromTicks(currentSessionTicks - lastUserActionTicks).TotalSeconds;
		}
	}

	private void StopHeartBeat()
	{
		if (heartbeatTimer != null)
		{
			heartbeatTimer.Stop();
			heartbeatTimer.Dispose();
			heartbeatTimer = null;
		}
	}

	private void StartHeartBeat()
	{
		if (enabled && sessionStarted)
		{
			StopHeartBeat();
			heartbeatTimer = new System.Timers.Timer(HeartBeatInSeconds * 1000);
			heartbeatTimer.Elapsed += SendHeartBeat;
			heartbeatTimer.AutoReset = true;
			heartbeatTimer.Enabled = true;
			lastHeartBeatTicks = DateTime.Now.Ticks;
		}
	}

	private uint GetSessionTime()
	{
		int num = (int)TimeSpan.FromTicks(currentSessionTicks - startTimeTicks).TotalSeconds;
		if (num < 0)
		{
			Debug.LogWarning("Session time is < 0");
		}
		return (uint)num;
	}

	private void SendHeartBeat(object source, ElapsedEventArgs e)
	{
		if (enabled && sessionStarted)
		{
			Dictionary<string, object> dictionary = GetUserSession();
			dictionary.Add("LastUA", (int)timeSinceLastUserAction);
			if (timeSinceLastUserAction > (float)HeartBeatTimeOutInSeconds)
			{
				dictionary.Add("HeartBeatTimeOut", true);
				heartbeatTimer.Stop();
				shouldEndSession = true;
			}
			long value = DateTime.Now.Ticks - lastHeartBeatTicks;
			dictionary.Add("HeartBeat", (int)TimeSpan.FromTicks(value).TotalSeconds);
			PostMetricData(dictionary, "SendHeartBeat");
			lastHeartBeatTicks = DateTime.Now.Ticks;
		}
	}

	private void StartThread()
	{
		if (!hasStarted)
		{
			if (isMultiThreaded)
			{
				Start();
			}
			hasStarted = true;
		}
	}

	private void EndThread()
	{
		if (hasStarted)
		{
			if (isMultiThreaded)
			{
				End();
			}
			hasStarted = false;
		}
	}

	public void SetStaticSessionVariable(string name, object var)
	{
		if (userSession.ContainsKey(name))
		{
			userSession[name] = var;
		}
		else
		{
			userSession.Add(name, var);
		}
	}

	public void RemoveStaticSessionVariable(string name)
	{
		if (userSession.ContainsKey(name))
		{
			userSession.Remove(name);
		}
	}

	public void AddDefaultSessionVariables()
	{
		userSession.Clear();
		SetStaticSessionVariable("InstallTimeStamp", GetInstallTimeStamp());
		SetStaticSessionVariable("user", UserID());
		SetStaticSessionVariable("SessionID", SessionID());
		SetStaticSessionVariable("SessionStartTimeStamp", sessionStartUtcTicks.ToString());
		if (KleiAccount.KleiUserID != null)
		{
			SetStaticSessionVariable("KU", KleiAccount.KleiUserID);
		}
		SetStaticSessionVariable("Expansions", Expansions());
	}

	private Dictionary<string, object> GetUserSession()
	{
		Debug.Assert(enabled);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!sessionStarted)
		{
			return dictionary;
		}
		foreach (KeyValuePair<string, object> item in userSession)
		{
			dictionary.Add(item.Key, item.Value);
		}
		dictionary.Add("SessionTimeSeconds", GetSessionTime());
		if (GameID() != -1)
		{
			dictionary.Add("GameID", GameID());
		}
		string text = CurrentLevel();
		if (text != null)
		{
			dictionary.Add("Level", text);
		}
		if (SetDynamicSessionVariables != null)
		{
			try
			{
				SetDynamicSessionVariables(dictionary);
				return dictionary;
			}
			catch (Exception ex)
			{
				Debug.LogError("Dynamic session variables may be set from a thread. " + ex.Message + "\n" + ex.StackTrace);
				return dictionary;
			}
		}
		return dictionary;
	}

	public void SetCallBacks(System.Action setStaticSessionVariables, Action<Dictionary<string, object>> setDynamicSessionVariables)
	{
		SetDynamicSessionVariables = setDynamicSessionVariables;
		SetStaticSessionVariables = setStaticSessionVariables;
	}

	private void SetStartTime()
	{
		sessionStartUtcTicks = DateTime.UtcNow.Ticks;
		startTimeTicks = DateTime.Now.Ticks;
		currentSessionTicks = DateTime.Now.Ticks;
		sessionStarted = true;
	}

	public void StartSession()
	{
		if (!enabled)
		{
			return;
		}
		if (sessionStarted)
		{
			EndSession();
		}
		StartThread();
		SetStartTime();
		IncrementSessionCount();
		AddDefaultSessionVariables();
		if (SetStaticSessionVariables != null)
		{
			SetStaticSessionVariables();
		}
		Dictionary<string, object> dictionary = GetUserSession();
		dictionary.Add("StartSession", true);
		string text = PlatformUserID();
		if (text != null)
		{
			dictionary.Add(PlatformUserIDFieldName, text);
		}
		if (shouldStartSession)
		{
			dictionary.Add("HeartBeatTimeOut", false);
		}
		foreach (KeyValuePair<string, object> hardwareStat in GetHardwareStats())
		{
			dictionary.Add(hardwareStat.Key, hardwareStat.Value);
		}
		PostMetricData(dictionary, "StartSession");
		StartHeartBeat();
	}

	public void EndSession(bool crashed = false)
	{
		if (enabled && sessionStarted)
		{
			Dictionary<string, object> dictionary = GetUserSession();
			dictionary.Add("EndSession", true);
			if (crashed)
			{
				dictionary.Add("EndSessionCrashed", true);
			}
			if (shouldEndSession)
			{
				dictionary.Add("HeartBeatTimeOut", true);
			}
			PostMetricData(dictionary, "EndSession");
			sessionStarted = false;
			StopHeartBeat();
			EndThread();
		}
	}

	public void StartNewGame()
	{
		if (enabled)
		{
			if (!sessionStarted)
			{
				StartSession();
			}
			IncrementGameCount();
			Dictionary<string, object> dictionary = GetUserSession();
			dictionary.Add("NewGame", true);
			PostMetricData(dictionary, "StartNewGame");
		}
	}

	public void EndGame()
	{
		if (enabled && sessionStarted)
		{
			Dictionary<string, object> dictionary = GetUserSession();
			dictionary.Add("EndGame", true);
			PostMetricData(dictionary, "EndGame");
		}
	}

	public void SendEvent(Dictionary<string, object> eventData, string debug_event_name)
	{
		if (!enabled)
		{
			return;
		}
		if (!sessionStarted)
		{
			StartSession();
		}
		Dictionary<string, object> dictionary = GetUserSession();
		foreach (KeyValuePair<string, object> eventDatum in eventData)
		{
			dictionary.Add(eventDatum.Key, eventDatum.Value);
		}
		PostMetricData(dictionary, "SendEvent:" + debug_event_name);
	}

	public bool SendProfileStats()
	{
		if (!enabled)
		{
			return false;
		}
		Dictionary<string, object> data = GetUserSession();
		return ThreadedHttps<KleiMetrics>.Instance.PostMetricData(data, "SendProfileStats") == "OK";
	}

	public static Dictionary<string, object> GetHardwareStats()
	{
		return new Dictionary<string, object>
		{
			{
				"Platform",
				Application.platform.ToString()
			},
			{
				"OSname",
				SystemInfo.operatingSystem
			},
			{
				"OSversion",
				Environment.OSVersion.Version.ToString()
			},
			{
				"CPUmodel",
				SystemInfo.deviceModel
			},
			{
				"CPUdeviceType",
				SystemInfo.deviceType.ToString()
			},
			{
				"CPUarch",
				Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
			},
			{
				"ProcBits",
				(IntPtr.Size == 4) ? 32 : 64
			},
			{
				"CPUcount",
				SystemInfo.processorCount
			},
			{
				"CPUtype",
				SystemInfo.processorType
			},
			{
				"SystemMemoryMegs",
				SystemInfo.systemMemorySize
			},
			{
				"GPUgraphicsDeviceID",
				SystemInfo.graphicsDeviceID
			},
			{
				"GPUname",
				SystemInfo.graphicsDeviceName
			},
			{
				"GPUgraphicsDeviceType",
				SystemInfo.graphicsDeviceType.ToString()
			},
			{
				"GPUgraphicsDeviceVendor",
				SystemInfo.graphicsDeviceVendor
			},
			{
				"GPUgraphicsDeviceVendorID",
				SystemInfo.graphicsDeviceVendorID
			},
			{
				"GPUgraphicsDeviceVersion",
				SystemInfo.graphicsDeviceVersion
			},
			{
				"GPUmemoryMegs",
				SystemInfo.graphicsMemorySize
			},
			{
				"GPUgraphicsMultiThreaded",
				SystemInfo.graphicsMultiThreaded
			},
			{
				"GPUgraphicsShaderLevel",
				SystemInfo.graphicsShaderLevel
			},
			{
				"GPUmaxTextureSize",
				SystemInfo.maxTextureSize
			},
			{
				"GPUnpotSupport",
				SystemInfo.npotSupport.ToString()
			},
			{
				"GPUsupportedRenderTargetCount",
				SystemInfo.supportedRenderTargetCount
			},
			{
				"GPUsupports2DArrayTextures",
				SystemInfo.supports2DArrayTextures
			},
			{
				"GPUsupports3DTextures",
				SystemInfo.supports3DTextures
			},
			{
				"GPUsupportsComputeShaders",
				SystemInfo.supportsComputeShaders
			},
			{
				"GPUsupportsImageEffects",
				true
			},
			{
				"GPUsupportsInstancing",
				SystemInfo.supportsInstancing
			},
			{
				"GPUsupportsRenderToCubemap",
				true
			},
			{
				"GPUsupportsShadows",
				SystemInfo.supportsShadows
			},
			{
				"GPUsupportsSparseTextures",
				SystemInfo.supportsSparseTextures
			}
		};
	}
}
