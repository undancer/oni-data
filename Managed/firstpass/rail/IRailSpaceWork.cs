using System.Collections.Generic;

namespace rail
{
	public interface IRailSpaceWork : IRailComponent
	{
		void Close();

		SpaceWorkID GetSpaceWorkID();

		bool Editable();

		RailResult StartSync(string user_data);

		RailResult GetSyncProgress(RailSpaceWorkSyncProgress progress);

		RailResult CancelSync();

		RailResult GetWorkLocalFolder(out string path);

		RailResult AsyncUpdateMetadata(string user_data);

		RailResult GetName(out string name);

		RailResult GetDescription(out string description);

		RailResult GetUrl(out string url);

		uint GetCreateTime();

		uint GetLastUpdateTime();

		ulong GetWorkFileSize();

		RailResult GetTags(List<string> tags);

		RailResult GetPreviewImage(out string path);

		RailResult GetVersion(out string version);

		ulong GetDownloadCount();

		ulong GetSubscribedCount();

		EnumRailSpaceWorkShareLevel GetShareLevel();

		ulong GetScore();

		RailResult GetMetadata(string key, out string value);

		EnumRailSpaceWorkRateValue GetMyVote();

		bool IsFavorite();

		bool IsSubscribed();

		RailResult SetName(string name);

		RailResult SetDescription(string description);

		RailResult SetTags(List<string> tags);

		RailResult SetPreviewImage(string path_filename);

		RailResult SetVersion(string version);

		RailResult SetShareLevel(EnumRailSpaceWorkShareLevel level);

		RailResult SetShareLevel();

		RailResult SetMetadata(string key, string value);

		RailResult SetContentFromFolder(string path);

		RailResult GetAllMetadata(List<RailKeyValue> metadata);

		RailResult GetAdditionalPreviewUrls(List<string> preview_urls);

		RailResult GetAssociatedSpaceWorks(List<SpaceWorkID> ids);

		RailResult GetLanguages(List<string> languages);

		RailResult RemoveMetadata(string key);

		RailResult SetAdditionalPreviews(List<string> local_paths);

		RailResult SetAssociatedSpaceWorks(List<SpaceWorkID> ids);

		RailResult SetLanguages(List<string> languages);

		RailResult GetPreviewUrl(out string url, uint scaling);

		RailResult GetPreviewUrl(out string url);

		RailResult GetVoteDetail(List<RailSpaceWorkVoteDetail> vote_details);

		RailResult GetUploaderIDs(List<RailID> uploader_ids);

		RailResult SetUpdateOptions(RailSpaceWorkUpdateOptions options);

		RailResult GetStatistic(EnumRailSpaceWorkStatistic stat_type, out ulong value);

		RailResult RemovePreviewImage();

		uint GetState();

		RailResult AddAssociatedGameIDs(List<RailGameID> game_ids);

		RailResult RemoveAssociatedGameIDs(List<RailGameID> game_ids);

		RailResult GetAssociatedGameIDs(List<RailGameID> game_ids);

		RailResult GetLocalVersion(out string version);
	}
}
