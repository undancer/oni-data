using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Steamworks;
using UnityEngine;

public class SteamUGCService : MonoBehaviour
{
	public class Mod
	{
		public Texture2D previewImage;

		public string title
		{
			get;
			private set;
		}

		public string description
		{
			get;
			private set;
		}

		public PublishedFileId_t fileId
		{
			get;
			private set;
		}

		public ulong lastUpdateTime
		{
			get;
			private set;
		}

		public List<string> tags
		{
			get;
			private set;
		}

		public Mod(SteamUGCDetails_t item, Texture2D previewImage)
		{
			title = item.m_rgchTitle;
			description = item.m_rgchDescription;
			fileId = item.m_nPublishedFileId;
			lastUpdateTime = item.m_rtimeUpdated;
			tags = new List<string>(item.m_rgchTags.Split(','));
			this.previewImage = previewImage;
		}

		public Mod(PublishedFileId_t id)
		{
			title = string.Empty;
			description = string.Empty;
			fileId = id;
			lastUpdateTime = 0uL;
			tags = new List<string>();
			previewImage = null;
		}
	}

	public interface IClient
	{
		void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<Mod> loaded_previews);
	}

	private UGCQueryHandle_t details_query = UGCQueryHandle_t.Invalid;

	private Callback<RemoteStoragePublishedFileSubscribed_t> on_subscribed;

	private Callback<RemoteStoragePublishedFileUpdated_t> on_updated;

	private Callback<RemoteStoragePublishedFileUnsubscribed_t> on_unsubscribed;

	private CallResult<SteamUGCQueryCompleted_t> on_query_completed;

	private HashSet<PublishedFileId_t> downloads = new HashSet<PublishedFileId_t>();

	private HashSet<PublishedFileId_t> queries = new HashSet<PublishedFileId_t>();

	private HashSet<PublishedFileId_t> proxies = new HashSet<PublishedFileId_t>();

	private HashSet<SteamUGCDetails_t> publishes = new HashSet<SteamUGCDetails_t>();

	private HashSet<PublishedFileId_t> removals = new HashSet<PublishedFileId_t>();

	private HashSet<SteamUGCDetails_t> previews = new HashSet<SteamUGCDetails_t>();

	private List<Mod> mods = new List<Mod>();

	private Dictionary<PublishedFileId_t, int> retry_counts = new Dictionary<PublishedFileId_t, int>();

	private static readonly string[] previewFileNames = new string[5]
	{
		"preview.png",
		"Preview.png",
		"PREVIEW.png",
		".png",
		".jpg"
	};

	private List<IClient> clients = new List<IClient>();

	private static SteamUGCService instance;

	private const EItemState DOWNLOADING_MASK = EItemState.k_EItemStateDownloading | EItemState.k_EItemStateDownloadPending;

	private const int RETRY_THRESHOLD = 1000;

	public static SteamUGCService Instance => instance;

	private SteamUGCService()
	{
		on_subscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(OnItemSubscribed);
		on_unsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnItemUnsubscribed);
		on_updated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(OnItemUpdated);
		on_query_completed = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryDetailsCompleted);
		mods = new List<Mod>();
	}

	public static void Initialize()
	{
		if (!(instance != null))
		{
			GameObject gameObject = GameObject.Find("/SteamManager");
			instance = gameObject.GetComponent<SteamUGCService>();
			if (instance == null)
			{
				instance = gameObject.AddComponent<SteamUGCService>();
			}
		}
	}

	public void AddClient(IClient client)
	{
		clients.Add(client);
		ListPool<PublishedFileId_t, SteamUGCService>.PooledList pooledList = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
		foreach (Mod mod in mods)
		{
			pooledList.Add(mod.fileId);
		}
		client.UpdateMods(pooledList, Enumerable.Empty<PublishedFileId_t>(), Enumerable.Empty<PublishedFileId_t>(), Enumerable.Empty<Mod>());
		pooledList.Recycle();
	}

	public void RemoveClient(IClient client)
	{
		clients.Remove(client);
	}

	public void Awake()
	{
		Debug.Assert(instance == null);
		instance = this;
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		if (numSubscribedItems != 0)
		{
			PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
			SteamUGC.GetSubscribedItems(array, numSubscribedItems);
			downloads.UnionWith(array);
		}
	}

	public bool IsSubscribed(PublishedFileId_t item)
	{
		return downloads.Contains(item) || proxies.Contains(item) || queries.Contains(item) || publishes.Any((SteamUGCDetails_t candidate) => candidate.m_nPublishedFileId == item) || mods.Exists((Mod candidate) => candidate.fileId == item);
	}

	public Mod FindMod(PublishedFileId_t item)
	{
		return mods.Find((Mod candidate) => candidate.fileId == item);
	}

	private void OnDestroy()
	{
		Debug.Assert(instance == this);
		instance = null;
	}

	private Texture2D LoadPreviewImage(SteamUGCDetails_t details)
	{
		byte[] array = null;
		if (details.m_hPreviewFile != UGCHandle_t.Invalid)
		{
			SteamRemoteStorage.UGCDownload(details.m_hPreviewFile, 0u);
			array = new byte[details.m_nPreviewFileSize];
			int num = SteamRemoteStorage.UGCRead(details.m_hPreviewFile, array, details.m_nPreviewFileSize, 0u, EUGCReadAction.k_EUGCRead_ContinueReadingUntilFinished);
			if (num != details.m_nPreviewFileSize)
			{
				if (retry_counts[details.m_nPublishedFileId] % 100 == 0)
				{
					Debug.LogFormat("Steam: Preview image load failed");
				}
				array = null;
			}
		}
		if (array == null)
		{
			array = GetBytesFromZip(details.m_nPublishedFileId, previewFileNames, out var _);
		}
		Texture2D texture2D = null;
		if (array != null)
		{
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(array);
		}
		else
		{
			retry_counts[details.m_nPublishedFileId]++;
		}
		return texture2D;
	}

	private void Update()
	{
		if (!SteamManager.Initialized || Game.Instance != null)
		{
			return;
		}
		downloads.ExceptWith(removals);
		publishes.RemoveWhere((SteamUGCDetails_t publish) => removals.Contains(publish.m_nPublishedFileId));
		previews.RemoveWhere((SteamUGCDetails_t publish) => removals.Contains(publish.m_nPublishedFileId));
		proxies.ExceptWith(removals);
		HashSetPool<Mod, SteamUGCService>.PooledHashSet loaded_previews = HashSetPool<Mod, SteamUGCService>.Allocate();
		HashSetPool<PublishedFileId_t, SteamUGCService>.PooledHashSet cancelled_previews = HashSetPool<PublishedFileId_t, SteamUGCService>.Allocate();
		foreach (SteamUGCDetails_t preview in previews)
		{
			Mod mod3 = FindMod(preview.m_nPublishedFileId);
			DebugUtil.DevAssert(mod3 != null, "expect mod with pending preview to be published");
			mod3.previewImage = LoadPreviewImage(preview);
			if (mod3.previewImage != null)
			{
				loaded_previews.Add(mod3);
			}
			else if (1000 < retry_counts[preview.m_nPublishedFileId])
			{
				cancelled_previews.Add(mod3.fileId);
			}
		}
		previews.RemoveWhere((SteamUGCDetails_t publish) => loaded_previews.Any((Mod mod) => mod.fileId == publish.m_nPublishedFileId) || cancelled_previews.Contains(publish.m_nPublishedFileId));
		cancelled_previews.Recycle();
		ListPool<Mod, SteamUGCService>.PooledList pooledList = ListPool<Mod, SteamUGCService>.Allocate();
		HashSetPool<PublishedFileId_t, SteamUGCService>.PooledHashSet published = HashSetPool<PublishedFileId_t, SteamUGCService>.Allocate();
		foreach (SteamUGCDetails_t publish in publishes)
		{
			EItemState itemState = (EItemState)SteamUGC.GetItemState(publish.m_nPublishedFileId);
			if ((itemState & (EItemState.k_EItemStateDownloading | EItemState.k_EItemStateDownloadPending)) == 0)
			{
				Debug.LogFormat("Steam: updating info for mod {0}", publish.m_rgchTitle);
				Mod mod4 = new Mod(publish, LoadPreviewImage(publish));
				pooledList.Add(mod4);
				if (publish.m_hPreviewFile != UGCHandle_t.Invalid && mod4.previewImage == null)
				{
					previews.Add(publish);
				}
				published.Add(publish.m_nPublishedFileId);
			}
		}
		publishes.RemoveWhere((SteamUGCDetails_t publish) => published.Contains(publish.m_nPublishedFileId));
		published.Recycle();
		foreach (PublishedFileId_t proxy in proxies)
		{
			Debug.LogFormat("Steam: proxy mod {0}", proxy);
			pooledList.Add(new Mod(proxy));
		}
		proxies.Clear();
		ListPool<PublishedFileId_t, SteamUGCService>.PooledList pooledList2 = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
		ListPool<PublishedFileId_t, SteamUGCService>.PooledList pooledList3 = ListPool<PublishedFileId_t, SteamUGCService>.Allocate();
		foreach (Mod mod2 in pooledList)
		{
			int num = mods.FindIndex((Mod candidate) => candidate.fileId == mod2.fileId);
			if (num == -1)
			{
				mods.Add(mod2);
				pooledList2.Add(mod2.fileId);
			}
			else
			{
				mods[num] = mod2;
				pooledList3.Add(mod2.fileId);
			}
		}
		pooledList.Recycle();
		bool flag = details_query == UGCQueryHandle_t.Invalid;
		if (pooledList2.Count != 0 || pooledList3.Count != 0 || (flag && removals.Count != 0) || loaded_previews.Count != 0)
		{
			foreach (IClient client in clients)
			{
				IEnumerable<PublishedFileId_t> removed;
				if (!flag)
				{
					removed = Enumerable.Empty<PublishedFileId_t>();
				}
				else
				{
					IEnumerable<PublishedFileId_t> enumerable = removals;
					removed = enumerable;
				}
				client.UpdateMods(pooledList2, pooledList3, removed, loaded_previews);
			}
		}
		pooledList2.Recycle();
		pooledList3.Recycle();
		loaded_previews.Recycle();
		if (flag)
		{
			foreach (PublishedFileId_t removal in removals)
			{
				mods.RemoveAll((Mod candidate) => candidate.fileId == removal);
			}
			removals.Clear();
		}
		foreach (PublishedFileId_t download in downloads)
		{
			EItemState itemState2 = (EItemState)SteamUGC.GetItemState(download);
			if (((itemState2 & EItemState.k_EItemStateInstalled) == 0 || (itemState2 & EItemState.k_EItemStateNeedsUpdate) != 0) && (itemState2 & (EItemState.k_EItemStateDownloading | EItemState.k_EItemStateDownloadPending)) == 0)
			{
				SteamUGC.DownloadItem(download, bHighPriority: false);
			}
		}
		if (details_query == UGCQueryHandle_t.Invalid)
		{
			queries.UnionWith(downloads);
			downloads.Clear();
			if (queries.Count != 0)
			{
				details_query = SteamUGC.CreateQueryUGCDetailsRequest(queries.ToArray(), (uint)queries.Count);
				SteamAPICall_t hAPICall = SteamUGC.SendQueryUGCRequest(details_query);
				on_query_completed.Set(hAPICall);
			}
		}
	}

	private void OnSteamUGCQueryDetailsCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
	{
		switch (pCallback.m_eResult)
		{
		case EResult.k_EResultOK:
		{
			for (uint num = 0u; num < pCallback.m_unNumResultsReturned; num++)
			{
				SteamUGCDetails_t pDetails = default(SteamUGCDetails_t);
				SteamUGC.GetQueryUGCResult(details_query, num, out pDetails);
				if (!removals.Contains(pDetails.m_nPublishedFileId))
				{
					publishes.Add(pDetails);
					retry_counts[pDetails.m_nPublishedFileId] = 0;
				}
				queries.Remove(pDetails.m_nPublishedFileId);
			}
			break;
		}
		case EResult.k_EResultBusy:
		{
			string[] obj2 = new string[5]
			{
				"Steam: [OnSteamUGCQueryDetailsCompleted] - handle: ",
				null,
				null,
				null,
				null
			};
			UGCQueryHandle_t handle = pCallback.m_handle;
			obj2[1] = handle.ToString();
			obj2[2] = " -- Result: ";
			obj2[3] = pCallback.m_eResult.ToString();
			obj2[4] = " Resending";
			Debug.Log(string.Concat(obj2));
			break;
		}
		default:
		{
			string[] obj = new string[10]
			{
				"Steam: [OnSteamUGCQueryDetailsCompleted] - handle: ",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			UGCQueryHandle_t handle = pCallback.m_handle;
			obj[1] = handle.ToString();
			obj[2] = " -- Result: ";
			obj[3] = pCallback.m_eResult.ToString();
			obj[4] = " -- NUm results: ";
			obj[5] = pCallback.m_unNumResultsReturned.ToString();
			obj[6] = " --Total Matching: ";
			obj[7] = pCallback.m_unTotalMatchingResults.ToString();
			obj[8] = " -- cached: ";
			obj[9] = pCallback.m_bCachedData.ToString();
			Debug.Log(string.Concat(obj));
			HashSet<PublishedFileId_t> hashSet = proxies;
			proxies = queries;
			queries = hashSet;
			break;
		}
		}
		SteamUGC.ReleaseQueryUGCRequest(details_query);
		details_query = UGCQueryHandle_t.Invalid;
	}

	private void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback)
	{
		downloads.Add(pCallback.m_nPublishedFileId);
	}

	private void OnItemUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
	{
		downloads.Add(pCallback.m_nPublishedFileId);
	}

	private void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
	{
		removals.Add(pCallback.m_nPublishedFileId);
	}

	public static byte[] GetBytesFromZip(PublishedFileId_t item, string[] filesToExtract, out System.DateTime lastModified, bool getFirstMatch = false)
	{
		byte[] result = null;
		lastModified = System.DateTime.MinValue;
		SteamUGC.GetItemInstallInfo(item, out var _, out var pchFolder, 1024u, out var _);
		try
		{
			lastModified = File.GetLastWriteTimeUtc(pchFolder);
			using MemoryStream memoryStream = new MemoryStream();
			using ZipFile zipFile = ZipFile.Read(pchFolder);
			ZipEntry zipEntry = null;
			foreach (string text in filesToExtract)
			{
				if (text.Length > 4)
				{
					if (zipFile.ContainsEntry(text))
					{
						zipEntry = zipFile[text];
					}
				}
				else
				{
					foreach (ZipEntry entry in zipFile.Entries)
					{
						if (entry.FileName.EndsWith(text))
						{
							zipEntry = entry;
							break;
						}
					}
				}
				if (zipEntry != null)
				{
					break;
				}
			}
			if (zipEntry != null)
			{
				zipEntry.Extract(memoryStream);
				memoryStream.Flush();
				result = memoryStream.ToArray();
			}
		}
		catch (Exception)
		{
		}
		return result;
	}
}
