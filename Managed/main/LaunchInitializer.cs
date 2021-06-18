using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	private const string BASE_BUILD_PREFIX = "MD";

	private const string EXPANSION1_BUILD_PREFIX = "EX1 S13";

	public GameObject[] SpawnPrefabs;

	[SerializeField]
	private int numWaitFrames = 1;

	public static string BuildPrefix()
	{
		return DlcManager.IsExpansion1Active() ? "EX1 S13" : "MD";
	}

	private void Update()
	{
		if (numWaitFrames > Time.renderedFrameCount)
		{
			return;
		}
		if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAFloat))
		{
			Debug.LogError("Machine does not support RGBAFloat32");
		}
		GraphicsOptionsScreen.SetSettingsFromPrefs();
		Util.ApplyInvariantCultureToThread(Thread.CurrentThread);
		Debug.Log("Current date: " + System.DateTime.Now.ToString());
		Debug.Log("preview Build: " + 468097u);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		KPlayerPrefs.instance.Load();
		DistributionPlatform.Initialize();
		Debug.Log("DistributionPlatform initialized.");
		Debug.Log("preview Build: " + BuildPrefix() + "-" + 468097u);
		Debug.Log($"EXPANSION1 installed: {DlcManager.IsExpansion1Installed()}  active: {DlcManager.IsExpansion1Active()}");
		KFMOD.Initialize();
		for (int i = 0; i < SpawnPrefabs.Length; i++)
		{
			GameObject gameObject = SpawnPrefabs[i];
			if (gameObject != null)
			{
				Util.KInstantiate(gameObject, base.gameObject);
			}
		}
		DeleteLingeringFiles();
		base.enabled = false;
	}

	private static void DeleteLingeringFiles()
	{
		string[] array = new string[3]
		{
			"fmod.log",
			"load_stats_0.json",
			"OxygenNotIncluded_Data/output_log.txt"
		};
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		string[] array2 = array;
		foreach (string path in array2)
		{
			string path2 = Path.Combine(directoryName, path);
			try
			{
				if (File.Exists(path2))
				{
					File.Delete(path2);
				}
			}
			catch (Exception obj)
			{
				Debug.LogWarning(obj);
			}
		}
	}
}
