using System.Collections.Generic;
using System.Linq;
using Steamworks;
using STRINGS;

namespace KMod
{
	public class Steam : IDistributionPlatform, SteamUGCService.IClient
	{
		private Mod MakeMod(SteamUGCService.Mod subscribed)
		{
			if (subscribed == null)
			{
				return null;
			}
			if ((SteamUGC.GetItemState(subscribed.fileId) & 4) == 0)
			{
				return null;
			}
			string steamModID = subscribed.fileId.m_PublishedFileId.ToString();
			Label label = default(Label);
			label.id = steamModID;
			label.distribution_platform = Label.DistributionPlatform.Steam;
			label.version = (long)subscribed.lastUpdateTime;
			label.title = subscribed.title;
			Label label2 = label;
			if (!SteamUGC.GetItemInstallInfo(subscribed.fileId, out var _, out var pchFolder, 1024u, out var _))
			{
				Global.Instance.modManager.events.Add(new Event
				{
					event_type = EventType.InstallInfoInaccessible,
					mod = label2
				});
				return null;
			}
			ZipFile zipFile = new ZipFile(pchFolder);
			KModHeader header = KModUtil.GetHeader(zipFile, label2.defaultStaticID, subscribed.title, subscribed.description, devMod: false);
			label2.title = header.title;
			return new Mod(label2, header.staticID, header.description, zipFile, UI.FRONTEND.MODS.TOOLTIPS.MANAGE_STEAM_SUBSCRIPTION, delegate
			{
				App.OpenWebURL("https://steamcommunity.com/sharedfiles/filedetails/?id=" + steamModID);
			});
		}

		public void UpdateMods(IEnumerable<PublishedFileId_t> added, IEnumerable<PublishedFileId_t> updated, IEnumerable<PublishedFileId_t> removed, IEnumerable<SteamUGCService.Mod> loaded_previews)
		{
			foreach (PublishedFileId_t item in added)
			{
				SteamUGCService.Mod mod = SteamUGCService.Instance.FindMod(item);
				if (mod == null)
				{
					DebugUtil.DevAssert(test: false, "SteamUGCService just told us this id was valid!");
					continue;
				}
				Mod mod2 = MakeMod(mod);
				if (mod2 != null)
				{
					Global.Instance.modManager.Subscribe(mod2, this);
				}
			}
			foreach (PublishedFileId_t item2 in updated)
			{
				SteamUGCService.Mod mod3 = SteamUGCService.Instance.FindMod(item2);
				if (mod3 == null)
				{
					DebugUtil.DevAssert(test: false, "SteamUGCService just told us this id was valid!");
					continue;
				}
				Mod mod4 = MakeMod(mod3);
				if (mod4 != null)
				{
					Global.Instance.modManager.Update(mod4, this);
				}
			}
			foreach (PublishedFileId_t item3 in removed)
			{
				Manager modManager = Global.Instance.modManager;
				Label label = default(Label);
				ulong publishedFileId = item3.m_PublishedFileId;
				label.id = publishedFileId.ToString();
				label.distribution_platform = Label.DistributionPlatform.Steam;
				modManager.Unsubscribe(label, this);
			}
			if (added.Count() != 0)
			{
				Global.Instance.modManager.Sanitize(null);
			}
			else
			{
				Global.Instance.modManager.Report(null);
			}
		}
	}
}
