using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using TMPro;
using UnityEngine;

public class GlobalAssets : KMonoBehaviour
{
	private static Dictionary<string, string> SoundTable = new Dictionary<string, string>();

	private static HashSet<string> LowPrioritySounds = new HashSet<string>();

	private static HashSet<string> HighPrioritySounds = new HashSet<string>();

	public ColorSet colorSet;

	public ColorSet[] colorSetOptions;

	public static GlobalAssets Instance
	{
		get;
		private set;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		if (SoundTable.Count == 0)
		{
			Bank[] array = null;
			try
			{
				if (RuntimeManager.StudioSystem.getBankList(out array) != 0)
				{
					array = null;
				}
			}
			catch
			{
				array = null;
			}
			if (array != null)
			{
				Bank[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Bank bank = array2[i];
					EventDescription[] array3;
					RESULT eventList = bank.getEventList(out array3);
					string path;
					if (eventList != 0)
					{
						bank.getPath(out path);
						Debug.LogError($"ERROR [{eventList}] loading FMOD events for bank [{path}]");
						continue;
					}
					for (int j = 0; j < array3.Length; j++)
					{
						EventDescription eventDescription = array3[j];
						eventDescription.getPath(out path);
						if (path == null)
						{
							bank.getPath(out path);
							eventDescription.getID(out var id);
							Debug.LogError($"Got a FMOD event with a null path! {eventDescription.ToString()} {id} in bank {path}");
							continue;
						}
						string simpleSoundEventName = Assets.GetSimpleSoundEventName(path);
						simpleSoundEventName = simpleSoundEventName.ToLowerInvariant();
						if (simpleSoundEventName.Length > 0 && !SoundTable.ContainsKey(simpleSoundEventName))
						{
							SoundTable[simpleSoundEventName] = path;
							if (path.ToLower().Contains("lowpriority") || simpleSoundEventName.Contains("lowpriority"))
							{
								LowPrioritySounds.Add(path);
							}
							else if (path.ToLower().Contains("highpriority") || simpleSoundEventName.Contains("highpriority"))
							{
								HighPrioritySounds.Add(path);
							}
						}
					}
				}
			}
		}
		SetDefaults.Initialize();
		GraphicsOptionsScreen.SetColorModeFromPrefs();
		AddColorModeStyles();
		LocString.CreateLocStringKeys(typeof(UI));
		LocString.CreateLocStringKeys(typeof(INPUT));
		LocString.CreateLocStringKeys(typeof(GAMEPLAY_EVENTS));
		LocString.CreateLocStringKeys(typeof(ROOMS));
		LocString.CreateLocStringKeys(typeof(BUILDING.STATUSITEMS), "STRINGS.BUILDING.");
		LocString.CreateLocStringKeys(typeof(BUILDING.DETAILS), "STRINGS.BUILDING.");
		LocString.CreateLocStringKeys(typeof(SETITEMS));
		LocString.CreateLocStringKeys(typeof(COLONY_ACHIEVEMENTS));
		LocString.CreateLocStringKeys(typeof(CREATURES));
		LocString.CreateLocStringKeys(typeof(RESEARCH));
		LocString.CreateLocStringKeys(typeof(DUPLICANTS));
		LocString.CreateLocStringKeys(typeof(ITEMS));
		LocString.CreateLocStringKeys(typeof(ROBOTS));
		LocString.CreateLocStringKeys(typeof(ELEMENTS));
		LocString.CreateLocStringKeys(typeof(MISC));
		LocString.CreateLocStringKeys(typeof(VIDEOS));
		LocString.CreateLocStringKeys(typeof(NAMEGEN));
		LocString.CreateLocStringKeys(typeof(WORLDS));
		LocString.CreateLocStringKeys(typeof(CLUSTER_NAMES));
		LocString.CreateLocStringKeys(typeof(SUBWORLDS));
		LocString.CreateLocStringKeys(typeof(WORLD_TRAITS));
		LocString.CreateLocStringKeys(typeof(INPUT_BINDINGS));
		LocString.CreateLocStringKeys(typeof(LORE));
		LocString.CreateLocStringKeys(typeof(CODEX));
		LocString.CreateLocStringKeys(typeof(SUBWORLDS));
	}

	private void AddColorModeStyles()
	{
		TMP_Style style = new TMP_Style("logic_on", $"<color=#{ColorUtility.ToHtmlStringRGB(colorSet.logicOn)}>", "</color>");
		TMP_StyleSheet.instance.AddStyle(style);
		TMP_Style style2 = new TMP_Style("logic_off", $"<color=#{ColorUtility.ToHtmlStringRGB(colorSet.logicOff)}>", "</color>");
		TMP_StyleSheet.instance.AddStyle(style2);
		TMP_StyleSheet.RefreshStyles();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}

	public static string GetSound(string name, bool force_no_warning = false)
	{
		if (name == null)
		{
			return null;
		}
		name = name.ToLowerInvariant();
		string value = null;
		SoundTable.TryGetValue(name, out value);
		return value;
	}

	public static bool IsLowPriority(string path)
	{
		return LowPrioritySounds.Contains(path);
	}

	public static bool IsHighPriority(string path)
	{
		return HighPrioritySounds.Contains(path);
	}
}
