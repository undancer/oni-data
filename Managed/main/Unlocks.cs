using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using ProcGen;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Unlocks")]
public class Unlocks : KMonoBehaviour
{
	private class MetaUnlockCategory
	{
		public string metaCollectionID;

		public string mesaCollectionID;

		public int mesaUnlockCount;

		public MetaUnlockCategory(string metaCollectionID, string mesaCollectionID, int mesaUnlockCount)
		{
			this.metaCollectionID = metaCollectionID;
			this.mesaCollectionID = mesaCollectionID;
			this.mesaUnlockCount = mesaUnlockCount;
		}
	}

	private const int FILE_IO_RETRY_ATTEMPTS = 5;

	private List<string> unlocked = new List<string>();

	private List<MetaUnlockCategory> MetaUnlockCategories = new List<MetaUnlockCategory>
	{
		new MetaUnlockCategory("dimensionalloreMeta", "dimensionallore", 4)
	};

	public Dictionary<string, string[]> lockCollections = new Dictionary<string, string[]>
	{
		{
			"emails",
			new string[22]
			{
				"email_thermodynamiclaws", "email_security2", "email_pens2", "email_atomiconrecruitment", "email_devonsblog", "email_researchgiant", "email_thejanitor", "email_newemployee", "email_timeoffapproved", "email_security3",
				"email_preliminarycalculations", "email_hollandsdog", "email_temporalbowupdate", "email_retemporalbowupdate", "email_memorychip", "email_arthistoryrequest", "email_AIcontrol", "email_AIcontrol2", "email_friendlyemail", "email_AIcontrol3",
				"email_AIcontrol4", "email_engineeringcandidate"
			}
		},
		{
			"journals",
			new string[33]
			{
				"journal_timesarrowthoughts", "journal_A046_1", "journal_B835_1", "journal_sunflowerseeds", "journal_B327_1", "journal_B556_1", "journal_employeeprocessing", "journal_B327_2", "journal_A046_2", "journal_elliesbirthday1",
				"journal_B835_2", "journal_ants", "journal_pipedream", "journal_B556_2", "journal_movedrats", "journal_B835_3", "journal_A046_3", "journal_B556_3", "journal_B327_3", "journal_B835_4",
				"journal_cleanup", "journal_A046_4", "journal_B327_4", "journal_revisitednumbers", "journal_B556_4", "journal_B835_5", "journal_elliesbirthday2", "journal_B111_1", "journal_revisitednumbers2", "journal_timemusings",
				"journal_evil", "journal_timesorder", "journal_inspace"
			}
		},
		{
			"researchnotes",
			new string[18]
			{
				"notes_clonedrats", "notes_agriculture1", "notes_husbandry1", "notes_hibiscus3", "notes_husbandry2", "notes_agriculture2", "notes_geneticooze", "notes_agriculture3", "notes_husbandry3", "notes_memoryimplantation",
				"notes_husbandry4", "notes_agriculture4", "notes_neutronium", "notes_firstsuccess", "notes_neutroniumapplications", "notes_teleportation", "notes_AI", "cryotank_warning"
			}
		},
		{
			"misc",
			new string[6] { "misc_newsecurity", "misc_mailroometiquette", "misc_unattendedcultures", "misc_politerequest", "misc_casualfriday", "misc_dishbot" }
		},
		{
			"dimensionallore",
			new string[6] { "notes_clonedrabbits", "notes_clonedraccoons", "journal_movedrabbits", "journal_movedraccoons", "journal_strawberries", "journal_shrimp" }
		},
		{
			"dimensionalloreMeta",
			new string[1] { "log9" }
		},
		{
			"space",
			new string[4] { "display_spaceprop1", "notice_pilot", "journal_inspace", "notes_firstcolony" }
		}
	};

	public Dictionary<int, string> cycleLocked = new Dictionary<int, string>
	{
		{ 0, "log1" },
		{ 3, "log2" },
		{ 15, "log3" },
		{ 1000, "log4" },
		{ 1500, "log4b" },
		{ 2000, "log5" },
		{ 2500, "log5b" },
		{ 3000, "log6" },
		{ 3500, "log6b" },
		{ 4000, "log7" },
		{ 4001, "log8" }
	};

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnLaunchRocketDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks component, object data)
	{
		component.OnLaunchRocket(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDuplicantDiedDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks component, object data)
	{
		component.OnDuplicantDied(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Unlocks> OnDiscoveredSpaceDelegate = new EventSystem.IntraObjectHandler<Unlocks>(delegate(Unlocks component, object data)
	{
		component.OnDiscoveredSpace(data);
	});

	private static string UnlocksFilename => System.IO.Path.Combine(Util.RootFolder(), "unlocks.json");

	protected override void OnPrefabInit()
	{
		LoadUnlocks();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UnlockCycleCodexes();
		GameClock.Instance.Subscribe(631075836, OnNewDay);
		Subscribe(-1277991738, OnLaunchRocketDelegate);
		Subscribe(282337316, OnDuplicantDiedDelegate);
		Subscribe(-818188514, OnDiscoveredSpaceDelegate);
		Components.LiveMinionIdentities.OnAdd += OnNewDupe;
	}

	public bool IsUnlocked(string unlockID)
	{
		if (string.IsNullOrEmpty(unlockID))
		{
			return false;
		}
		if (DebugHandler.InstantBuildMode)
		{
			return true;
		}
		return unlocked.Contains(unlockID);
	}

	public void Lock(string unlockID)
	{
		if (unlocked.Contains(unlockID))
		{
			unlocked.Remove(unlockID);
			SaveUnlocks();
			Game.Instance.Trigger(1594320620, unlockID);
		}
	}

	public void Unlock(string unlockID)
	{
		if (string.IsNullOrEmpty(unlockID))
		{
			DebugUtil.DevAssert(test: false, "Unlock called with null or empty string");
			return;
		}
		if (!unlocked.Contains(unlockID))
		{
			unlocked.Add(unlockID);
			SaveUnlocks();
			Game.Instance.Trigger(1594320620, unlockID);
			MessageNotification messageNotification = GenerateCodexUnlockNotification(unlockID);
			if (messageNotification != null)
			{
				GetComponent<Notifier>().Add(messageNotification);
			}
		}
		EvalMetaCategories();
	}

	private void EvalMetaCategories()
	{
		foreach (MetaUnlockCategory metaUnlockCategory in MetaUnlockCategories)
		{
			string metaCollectionID = metaUnlockCategory.metaCollectionID;
			string mesaCollectionID = metaUnlockCategory.mesaCollectionID;
			int mesaUnlockCount = metaUnlockCategory.mesaUnlockCount;
			int num = 0;
			string[] array = lockCollections[mesaCollectionID];
			foreach (string unlockID in array)
			{
				if (IsUnlocked(unlockID))
				{
					num++;
				}
			}
			if (num >= mesaUnlockCount)
			{
				UnlockNext(metaCollectionID);
			}
		}
	}

	private void SaveUnlocks()
	{
		if (!Directory.Exists(Util.RootFolder()))
		{
			Directory.CreateDirectory(Util.RootFolder());
		}
		string s = JsonConvert.SerializeObject(unlocked);
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using FileStream fileStream = File.Open(UnlocksFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
				flag = true;
				byte[] bytes = new ASCIIEncoding().GetBytes(s);
				fileStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to save Unlocks attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
	}

	public void LoadUnlocks()
	{
		unlocked.Clear();
		if (!File.Exists(UnlocksFilename))
		{
			return;
		}
		string text = "";
		bool flag = false;
		int num = 0;
		while (!flag && num < 5)
		{
			try
			{
				Thread.Sleep(num * 100);
				using FileStream fileStream = File.Open(UnlocksFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				flag = true;
				ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
				byte[] array = new byte[fileStream.Length];
				if (fileStream.Read(array, 0, array.Length) == fileStream.Length)
				{
					text += aSCIIEncoding.GetString(array);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarningFormat("Failed to load Unlocks attempt {0}: {1}", num + 1, ex.ToString());
			}
			num++;
		}
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		try
		{
			string[] array2 = JsonConvert.DeserializeObject<string[]>(text);
			foreach (string text2 in array2)
			{
				if (!string.IsNullOrEmpty(text2) && !unlocked.Contains(text2))
				{
					unlocked.Add(text2);
				}
			}
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("Error parsing unlocks file [{0}]: {1}", UnlocksFilename, ex2.ToString());
		}
	}

	public string UnlockNext(string collectionID)
	{
		string[] array = lockCollections[collectionID];
		foreach (string text in array)
		{
			if (string.IsNullOrEmpty(text))
			{
				DebugUtil.DevAssertArgs(false, "Found null/empty string in Unlocks collection: ", collectionID);
			}
			else if (!IsUnlocked(text))
			{
				Unlock(text);
				return text;
			}
		}
		return null;
	}

	private MessageNotification GenerateCodexUnlockNotification(string lockID)
	{
		string entryForLock = CodexCache.GetEntryForLock(lockID);
		if (string.IsNullOrEmpty(entryForLock))
		{
			return null;
		}
		string text = null;
		if (CodexCache.FindSubEntry(lockID) != null)
		{
			text = CodexCache.FindSubEntry(lockID).title;
		}
		else if (CodexCache.FindSubEntry(entryForLock) != null)
		{
			text = CodexCache.FindSubEntry(entryForLock).title;
		}
		else if (CodexCache.FindEntry(entryForLock) != null)
		{
			text = CodexCache.FindEntry(entryForLock).title;
		}
		string text2 = UI.FormatAsLink(Strings.Get(text), entryForLock);
		if (!string.IsNullOrEmpty(text))
		{
			ContentContainer contentContainer = CodexCache.FindEntry(entryForLock).contentContainers.Find((ContentContainer match) => match.lockID == lockID);
			if (contentContainer != null)
			{
				foreach (ICodexWidget item in contentContainer.content)
				{
					CodexText codexText = item as CodexText;
					if (codexText != null)
					{
						text2 = text2 + "\n\n" + codexText.text;
					}
				}
			}
			return new MessageNotification(new CodexUnlockedMessage(lockID, text2));
		}
		return null;
	}

	private void UnlockCycleCodexes()
	{
		foreach (KeyValuePair<int, string> item in cycleLocked)
		{
			if (GameClock.Instance.GetCycle() + 1 >= item.Key)
			{
				Unlock(item.Value);
			}
		}
	}

	private void OnNewDay(object data)
	{
		UnlockCycleCodexes();
	}

	private void OnLaunchRocket(object data)
	{
		Unlock("surfacebreach");
		Unlock("firstrocketlaunch");
	}

	private void OnDuplicantDied(object data)
	{
		Unlock("duplicantdeath");
		if (Components.LiveMinionIdentities.Count == 1)
		{
			Unlock("onedupeleft");
		}
	}

	private void OnNewDupe(MinionIdentity minion_identity)
	{
		if (Components.LiveMinionIdentities.Count >= 35)
		{
			Unlock("fulldupecolony");
		}
	}

	private void OnDiscoveredSpace(object data)
	{
		Unlock("surfacebreach");
	}

	public void Sim4000ms(float dt)
	{
		int x = int.MinValue;
		int num = int.MinValue;
		int x2 = int.MaxValue;
		int num2 = int.MaxValue;
		foreach (MinionIdentity item in Components.MinionIdentities.Items)
		{
			if (item == null)
			{
				continue;
			}
			int cell = Grid.PosToCell(item);
			if (Grid.IsValidCell(cell))
			{
				Grid.CellToXY(cell, out var x3, out var y);
				if (y > num)
				{
					num = y;
					x = x3;
				}
				if (y < num2)
				{
					x2 = x3;
					num2 = y;
				}
			}
		}
		if (num != int.MinValue)
		{
			int num3 = num;
			for (int i = 0; i < 30; i++)
			{
				num3++;
				int cell2 = Grid.XYToCell(x, num3);
				if (!Grid.IsValidCell(cell2))
				{
					break;
				}
				if (World.Instance.zoneRenderData.GetSubWorldZoneType(cell2) == SubWorld.ZoneType.Space)
				{
					Unlock("nearingsurface");
					break;
				}
			}
		}
		if (num2 == int.MaxValue)
		{
			return;
		}
		int num4 = num2;
		for (int j = 0; j < 30; j++)
		{
			num4--;
			int num5 = Grid.XYToCell(x2, num4);
			if (Grid.IsValidCell(num5))
			{
				if (World.Instance.zoneRenderData.GetSubWorldZoneType(num5) == SubWorld.ZoneType.ToxicJungle && Grid.Element[num5].id == SimHashes.Magma)
				{
					Unlock("nearingmagma");
					break;
				}
				continue;
			}
			break;
		}
	}
}
