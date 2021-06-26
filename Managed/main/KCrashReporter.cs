using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Klei;
using Newtonsoft.Json;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KCrashReporter : MonoBehaviour
{
	private class Error
	{
		public string game = "simgame";

		public int build = -1;

		public string platform = Environment.OSVersion.ToString();

		public string user = "unknown";

		public ulong steam64_verified = 0uL;

		public string callstack = "";

		public string fullstack = "";

		public string log = "";

		public string summaryline = "";

		public string user_message = "";

		public bool is_server = false;

		public bool is_dedicated = false;

		public string save_hash = "";
	}

	public static string MOST_RECENT_SAVEFILE = null;

	public const string CRASH_REPORTER_SERVER = "http://crashes.klei.ca";

	public const uint MAX_LOGS = 10000000u;

	public static bool ignoreAll = false;

	public static bool debugWasUsed = false;

	public static bool haveActiveMods = false;

	public static uint logCount = 0u;

	public static string error_canvas_name = "ErrorCanvas";

	private static bool disableDeduping = false;

	public static bool hasCrash = false;

	private static readonly Regex failedToLoadModuleRegEx = new Regex("^Failed to load '(.*?)' with error (.*)", RegexOptions.Multiline);

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private GameObject reportErrorPrefab;

	[SerializeField]
	private ConfirmDialogScreen confirmDialogPrefab;

	private GameObject errorScreen = null;

	public static bool terminateOnError = true;

	private static string dataRoot;

	private static readonly string[] IgnoreStrings = new string[3]
	{
		"Releasing render texture whose render buffer is set as Camera's target buffer with Camera.SetTargetBuffers!",
		"The profiler has run out of samples for this frame. This frame will be skipped. Increase the sample limit using Profiler.maxNumberOfSamplesPerFrame",
		"Trying to add Text (LocText) for graphic rebuild while we are already inside a graphic rebuild loop. This is not supported."
	};

	private static HashSet<int> previouslyReportedDevNotifications;

	public static bool hasReportedError
	{
		get;
		private set;
	}

	public static event Action<string> onCrashReported;

	private void OnEnable()
	{
		dataRoot = Application.dataPath;
		Application.logMessageReceived += HandleLog;
		ignoreAll = true;
		string path = Path.Combine(dataRoot, "hashes.json");
		bool flag;
		if (File.Exists(path))
		{
			StringBuilder stringBuilder = new StringBuilder();
			MD5 mD = MD5.Create();
			string value = File.ReadAllText(path);
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
			if (dictionary.Count > 0)
			{
				flag = true;
				foreach (KeyValuePair<string, string> item in dictionary)
				{
					string key = item.Key;
					string value2 = item.Value;
					stringBuilder.Length = 0;
					string path2 = Path.Combine(dataRoot, key);
					using FileStream inputStream = new FileStream(path2, FileMode.Open, FileAccess.Read);
					byte[] array = mD.ComputeHash(inputStream);
					byte[] array2 = array;
					foreach (byte b in array2)
					{
						stringBuilder.AppendFormat("{0:x2}", b);
					}
					string a = stringBuilder.ToString();
					if (a != value2)
					{
						flag = false;
						goto IL_014b;
					}
				}
				goto IL_014b;
			}
			ignoreAll = false;
		}
		else
		{
			ignoreAll = false;
		}
		goto IL_0171;
		IL_014b:
		if (flag)
		{
			ignoreAll = false;
		}
		goto IL_0171;
		IL_0171:
		if (ignoreAll)
		{
			Debug.Log("Ignoring crash due to mismatched hashes.json entries.");
		}
		if (File.Exists("ignorekcrashreporter.txt"))
		{
			ignoreAll = true;
			Debug.Log("Ignoring crash due to ignorekcrashreporter.txt");
		}
		if (Application.isEditor && !GenericGameSettings.instance.enableEditorCrashReporting)
		{
			terminateOnError = false;
		}
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string msg, string stack_trace, LogType type)
	{
		if (++logCount == 10000000)
		{
			DebugUtil.DevLogError("Turning off logging to avoid increasing the file to an unreasonable size, please review the logs as they probably contain spam");
			Debug.DisableLogging();
		}
		if (ignoreAll || Array.IndexOf(IgnoreStrings, msg) != -1 || (msg?.StartsWith("<RI.Hid>") ?? false))
		{
			return;
		}
		if (type == LogType.Exception)
		{
			RestartWarning.ShouldWarn = true;
		}
		if (!(errorScreen == null) || (type != LogType.Exception && type != 0) || (terminateOnError && hasCrash))
		{
			return;
		}
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Pause();
		}
		string text = stack_trace;
		if (string.IsNullOrEmpty(text))
		{
			StackTrace stackTrace = new StackTrace(5, fNeedFileInfo: true);
			text = stackTrace.ToString();
		}
		if (App.isLoading)
		{
			if (!SceneInitializerLoader.deferred_error.IsValid)
			{
				SceneInitializerLoader.DeferredError deferred_error = default(SceneInitializerLoader.DeferredError);
				deferred_error.msg = msg;
				deferred_error.stack_trace = text;
				SceneInitializerLoader.deferred_error = deferred_error;
			}
		}
		else
		{
			ShowDialog(msg, text);
		}
	}

	public bool ShowDialog(string error, string stack_trace)
	{
		if (errorScreen != null)
		{
			return false;
		}
		GameObject gameObject = GameObject.Find(error_canvas_name);
		if (gameObject == null)
		{
			gameObject = new GameObject();
			gameObject.name = error_canvas_name;
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1;
			canvas.sortingOrder = 32767;
			gameObject.AddComponent<GraphicRaycaster>();
		}
		errorScreen = UnityEngine.Object.Instantiate(reportErrorPrefab, Vector3.zero, Quaternion.identity);
		errorScreen.transform.SetParent(gameObject.transform, worldPositionStays: false);
		ReportErrorDialog errorDialog = errorScreen.GetComponentInChildren<ReportErrorDialog>();
		string stackTrace = error + "\n\n" + stack_trace;
		hasCrash = true;
		if (Global.Instance != null && Global.Instance.modManager != null && Global.Instance.modManager.HasCrashableMods())
		{
			Exception ex = DebugUtil.RetrieveLastExceptionLogged();
			StackTrace stackTrace2 = ((ex != null) ? new StackTrace(ex) : new StackTrace(5, fNeedFileInfo: true));
			Global.Instance.modManager.SearchForModsInStackTrace(stackTrace2);
			Global.Instance.modManager.SearchForModsInStackTrace(stack_trace);
			errorDialog.PopupDisableModsDialog(stackTrace, OnQuitToDesktop, (Global.Instance.modManager.IsInDevMode() || !terminateOnError) ? new System.Action(OnCloseErrorDialog) : null);
		}
		else
		{
			errorDialog.PopupSubmitErrorDialog(stackTrace, delegate
			{
				string save_file_hash = null;
				if (MOST_RECENT_SAVEFILE != null)
				{
					save_file_hash = UploadSaveFile(MOST_RECENT_SAVEFILE, stack_trace);
				}
				ReportError(error, stack_trace, save_file_hash, confirmDialogPrefab, errorScreen, errorDialog.UserMessage());
			}, OnQuitToDesktop, terminateOnError ? null : new System.Action(OnCloseErrorDialog));
		}
		return true;
	}

	private void OnCloseErrorDialog()
	{
		UnityEngine.Object.Destroy(errorScreen);
		errorScreen = null;
		hasCrash = false;
		if (SpeedControlScreen.Instance != null)
		{
			SpeedControlScreen.Instance.Unpause();
		}
	}

	private void OnQuitToDesktop()
	{
		App.Quit();
	}

	private static string UploadSaveFile(string save_file, string stack_trace, Dictionary<string, string> metadata = null)
	{
		Debug.Log($"Save_file: {save_file}");
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return "";
		}
		if (save_file != null && File.Exists(save_file))
		{
			using (WebClient webClient = new WebClient())
			{
				Encoding encoding = (webClient.Encoding = Encoding.UTF8);
				byte[] array = File.ReadAllBytes(save_file);
				string text = "----" + System.DateTime.Now.Ticks.ToString("x");
				webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + text);
				string str = "";
				string text2;
				using (SHA1CryptoServiceProvider sHA1CryptoServiceProvider = new SHA1CryptoServiceProvider())
				{
					text2 = BitConverter.ToString(sHA1CryptoServiceProvider.ComputeHash(array)).Replace("-", "");
				}
				str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "hash", text2);
				if (metadata != null)
				{
					string arg = JsonConvert.SerializeObject(metadata);
					str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", text, "metadata", arg);
				}
				str += string.Format("--{0}\r\nContent-Disposition: form-data; name=\"save\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", new object[3]
				{
					text,
					save_file,
					"application/x-spss-sav"
				});
				byte[] bytes = encoding.GetBytes(str);
				string s = $"\r\n--{text}--\r\n";
				byte[] bytes2 = encoding.GetBytes(s);
				byte[] array2 = new byte[bytes.Length + array.Length + bytes2.Length];
				Buffer.BlockCopy(bytes, 0, array2, 0, bytes.Length);
				Buffer.BlockCopy(array, 0, array2, bytes.Length, array.Length);
				Buffer.BlockCopy(bytes2, 0, array2, bytes.Length + array.Length, bytes2.Length);
				Uri address = new Uri("http://crashes.klei.ca/submitSave");
				try
				{
					webClient.UploadData(address, "POST", array2);
					return text2;
				}
				catch (Exception obj)
				{
					Debug.Log(obj);
					return "";
				}
			}
		}
		return "";
	}

	private static string GetUserID()
	{
		if (DistributionPlatform.Initialized)
		{
			return DistributionPlatform.Inst.Name + "ID_" + DistributionPlatform.Inst.LocalUser.Name + "_" + DistributionPlatform.Inst.LocalUser.Id;
		}
		return "LocalUser";
	}

	private static string GetLogContents()
	{
		string path = Util.LogFilePath();
		if (File.Exists(path))
		{
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using StreamReader streamReader = new StreamReader(stream);
				return streamReader.ReadToEnd();
			}
		}
		return "";
	}

	public static void ReportErrorDevNotification(string notification_name, string stack_trace, string details)
	{
		if (previouslyReportedDevNotifications == null)
		{
			previouslyReportedDevNotifications = new HashSet<int>();
		}
		details = "DevNotification: " + notification_name + " - " + details;
		int hashValue = new HashedString(notification_name).HashValue;
		bool hasReportedError = KCrashReporter.hasReportedError;
		if (!previouslyReportedDevNotifications.Contains(hashValue))
		{
			previouslyReportedDevNotifications.Add(hashValue);
			ReportError(notification_name, stack_trace, null, null, null, details);
		}
		KCrashReporter.hasReportedError = hasReportedError;
	}

	public static void ReportError(string msg, string stack_trace, string save_file_hash, ConfirmDialogScreen confirm_prefab, GameObject confirm_parent, string userMessage = "")
	{
		if (ignoreAll)
		{
			return;
		}
		Debug.Log("Reporting error.\n");
		if (msg != null)
		{
			Debug.Log(msg);
		}
		if (stack_trace != null)
		{
			Debug.Log(stack_trace);
		}
		hasReportedError = true;
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			return;
		}
		string obj2;
		using (WebClient webClient = new WebClient())
		{
			webClient.Encoding = Encoding.UTF8;
			if (string.IsNullOrEmpty(msg))
			{
				msg = "No message";
			}
			Match match = failedToLoadModuleRegEx.Match(msg);
			if (match.Success)
			{
				string path = match.Groups[1].ToString();
				string text = match.Groups[2].ToString();
				string fileName = Path.GetFileName(path);
				msg = "Failed to load '" + fileName + "' with error '" + text + "'.";
			}
			if (string.IsNullOrEmpty(stack_trace))
			{
				string arg = LaunchInitializer.BuildPrefix() + "-" + 469369u;
				stack_trace = $"No stack trace {arg}\n\n{msg}";
			}
			List<string> list = new List<string>();
			if (debugWasUsed)
			{
				list.Add("(Debug Used)");
			}
			if (haveActiveMods)
			{
				list.Add("(Mods Active)");
			}
			list.Add(msg);
			string[] array = new string[8]
			{
				"Debug:LogError",
				"UnityEngine.Debug",
				"Output:LogError",
				"DebugUtil:Assert",
				"System.Array",
				"System.Collections",
				"KCrashReporter.Assert",
				"No stack trace."
			};
			string[] array2 = stack_trace.Split('\n');
			foreach (string text2 in array2)
			{
				if (list.Count >= 5)
				{
					break;
				}
				if (string.IsNullOrEmpty(text2))
				{
					continue;
				}
				bool flag = false;
				string[] array3 = array;
				foreach (string value in array3)
				{
					if (text2.StartsWith(value))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list.Add(text2);
				}
			}
			if (userMessage == UI.CRASHSCREEN.BODY.text)
			{
				userMessage = "";
			}
			Error error = new Error();
			error.user = GetUserID();
			error.callstack = stack_trace;
			if (disableDeduping)
			{
				error.callstack = error.callstack + "\n" + Guid.NewGuid().ToString();
			}
			error.fullstack = $"{msg}\n\n{stack_trace}";
			error.build = 469369;
			error.log = GetLogContents();
			error.summaryline = string.Join("\n", list.ToArray());
			error.user_message = userMessage;
			if (!string.IsNullOrEmpty(save_file_hash))
			{
				error.save_hash = save_file_hash;
			}
			if (DistributionPlatform.Initialized)
			{
				error.steam64_verified = DistributionPlatform.Inst.LocalUser.Id.ToInt64();
			}
			string data = JsonConvert.SerializeObject(error);
			string text3 = "";
			Uri address = new Uri("http://crashes.klei.ca/submitCrash");
			Debug.Log("Submitting crash:");
			try
			{
				webClient.UploadStringAsync(address, data);
			}
			catch (Exception obj)
			{
				Debug.Log(obj);
			}
			if (confirm_prefab != null && confirm_parent != null)
			{
				ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)KScreenManager.Instance.StartScreen(confirm_prefab.gameObject, confirm_parent);
				confirmDialogScreen.PopupConfirmDialog(UI.CRASHSCREEN.REPORTEDERROR, null, null);
			}
			obj2 = text3;
		}
		if (KCrashReporter.onCrashReported != null)
		{
			KCrashReporter.onCrashReported(obj2);
		}
	}

	public static void ReportBug(string msg, string save_file, GameObject confirmParent)
	{
		string stack_trace = "Bug Report From: " + GetUserID() + " at " + System.DateTime.Now.ToString();
		string save_file_hash = UploadSaveFile(save_file, stack_trace, new Dictionary<string, string>
		{
			{
				"user",
				GetUserID()
			}
		});
		ReportError(msg, stack_trace, save_file_hash, ScreenPrefabs.Instance.ConfirmDialogScreen, confirmParent);
	}

	public static void Assert(bool condition, string message)
	{
		if (!condition && !hasReportedError)
		{
			StackTrace stackTrace = new StackTrace(1, fNeedFileInfo: true);
			ReportError("ASSERT: " + message, stackTrace.ToString(), null, null, null);
		}
	}

	public static void ReportSimDLLCrash(string msg, string stack_trace, string dmp_filename)
	{
		if (!hasReportedError)
		{
			string save_file_hash = null;
			string text = null;
			string text2 = null;
			if (dmp_filename != null)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dmp_filename);
				text = Path.Combine(Path.GetDirectoryName(dataRoot), dmp_filename);
				text2 = Path.Combine(Path.GetDirectoryName(dataRoot), fileNameWithoutExtension + ".sav");
				File.Move(text, text2);
				save_file_hash = UploadSaveFile(text2, stack_trace, new Dictionary<string, string>
				{
					{
						"user",
						GetUserID()
					}
				});
			}
			ReportError(msg, stack_trace, save_file_hash, null, null);
			if (dmp_filename != null)
			{
				File.Move(text2, text);
			}
		}
	}
}
