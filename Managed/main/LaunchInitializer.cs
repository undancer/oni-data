using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
	public const string BUILD_PREFIX = "CS";

	public GameObject[] SpawnPrefabs;

	[SerializeField]
	private int numWaitFrames = 1;

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
		Debug.Log("release Build: CS-" + 447596u);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		KPlayerPrefs.instance.Load();
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
		string[] obj = new string[3]
		{
			"fmod.log",
			"load_stats_0.json",
			"OxygenNotIncluded_Data/output_log.txt"
		};
		string directoryName = Path.GetDirectoryName(Application.dataPath);
		string[] array = obj;
		foreach (string path in array)
		{
			string path2 = Path.Combine(directoryName, path);
			try
			{
				if (File.Exists(path2))
				{
					File.Delete(path2);
				}
			}
			catch (Exception obj2)
			{
				Debug.LogWarning(obj2);
			}
		}
	}
}
