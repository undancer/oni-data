using System;
using System.Collections.Generic;
using UnityEngine;

public class KAnimGroupFile : ScriptableObject
{
	[Serializable]
	public class Group
	{
		[SerializeField]
		public HashedString id;

		[SerializeField]
		public string commandDirectory = "";

		[SerializeField]
		public List<HashedString> animNames = new List<HashedString>();

		[SerializeField]
		public KAnimBatchGroup.RendererType renderType;

		[SerializeField]
		public int maxVisibleSymbols;

		[SerializeField]
		public int maxGroupSize;

		[SerializeField]
		public HashedString target;

		[SerializeField]
		public HashedString swapTarget;

		[SerializeField]
		public HashedString animTarget;

		[NonSerialized]
		public List<KAnimFile> animFiles = new List<KAnimFile>();

		public Group(HashedString tag)
		{
			id = tag;
		}
	}

	public class GroupFile
	{
		public string groupID
		{
			get;
			set;
		}

		public string commandDirectory
		{
			get;
			set;
		}
	}

	public enum AddModResult
	{
		Added,
		Replaced
	}

	private const string MASTER_GROUP_FILE = "animgrouptags";

	public const int MAX_ANIMS_PER_GROUP = 10;

	private static KAnimGroupFile groupfile;

	private Dictionary<int, KAnimFileData> fileData = new Dictionary<int, KAnimFileData>();

	[SerializeField]
	private List<Group> groups = new List<Group>();

	[SerializeField]
	private List<Pair<HashedString, HashedString>> currentGroup = new List<Pair<HashedString, HashedString>>();

	public static void DestroyInstance()
	{
		groupfile = null;
	}

	public static string GetFilePath(string contentDir = "")
	{
		if (string.IsNullOrEmpty(contentDir))
		{
			return "Assets/anim/base/resources/animgrouptags.asset";
		}
		return "Assets/anim" + $"/{contentDir}/" + "animgrouptags.asset";
	}

	public static KAnimGroupFile GetGroupFile()
	{
		Debug.Assert(groupfile != null, "Cannot GetGroupFile before it is loaded.");
		return groupfile;
	}

	public static Group GetGroup(HashedString tag)
	{
		Debug.Assert(groupfile != null, "GetGroup called before LoadAll called");
		List<Group> list = groupfile.groups;
		Debug.Assert(list != null, list.Count > 0);
		for (int i = 0; i < list.Count; i++)
		{
			Group group = list[i];
			if (group.id == tag || group.target == tag)
			{
				return group;
			}
		}
		return null;
	}

	public static HashedString GetGroupForHomeDirectory(HashedString homedirectory)
	{
		List<Pair<HashedString, HashedString>> list = groupfile.currentGroup;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].first == homedirectory)
			{
				return list[i].second;
			}
		}
		return default(HashedString);
	}

	public List<Group> GetData()
	{
		return groups;
	}

	public void Reset()
	{
		groups = new List<Group>();
		currentGroup = new List<Pair<HashedString, HashedString>>();
	}

	private int AddGroup(AnimCommandFile akf, GroupFile gf, KAnimFile file)
	{
		bool flag = akf.IsSwap(file);
		HashedString groupId = new HashedString(gf.groupID);
		int num = groups.FindIndex((Group t) => t.id == groupId);
		if (num == -1)
		{
			num = groups.Count;
			Group group = new Group(groupId);
			group.commandDirectory = akf.directory;
			group.maxGroupSize = akf.MaxGroupSize;
			group.renderType = akf.RendererType;
			if (groups.FindIndex((Group t) => t.commandDirectory == group.commandDirectory) == -1)
			{
				if (flag)
				{
					if (!string.IsNullOrEmpty(akf.TargetBuild))
					{
						group.target = new HashedString(akf.TargetBuild);
					}
					if (group.renderType != KAnimBatchGroup.RendererType.DontRender)
					{
						group.renderType = KAnimBatchGroup.RendererType.DontRender;
						group.swapTarget = new HashedString(akf.SwapTargetBuild);
					}
				}
				if (akf.Type == AnimCommandFile.ConfigType.AnimOnly)
				{
					group.target = new HashedString(akf.TargetBuild);
					group.renderType = KAnimBatchGroup.RendererType.AnimOnly;
					group.animTarget = new HashedString(akf.AnimTargetBuild);
					group.swapTarget = new HashedString(akf.SwapTargetBuild);
				}
			}
			groups.Add(group);
		}
		return num;
	}

	public bool AddAnimFile(GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		Debug.Assert(gf != null);
		Debug.Assert(file != null, gf.groupID);
		Debug.Assert(akf != null, gf.groupID);
		int groupIndex = AddGroup(akf, gf, file);
		return AddFile(groupIndex, file);
	}

	private bool AddFile(int groupIndex, KAnimFile file)
	{
		if (!groups[groupIndex].animNames.Contains(file.name))
		{
			Pair<HashedString, HashedString> pair = new Pair<HashedString, HashedString>(file.homedirectory, groups[groupIndex].id);
			bool flag = false;
			for (int i = 0; i < currentGroup.Count; i++)
			{
				if (currentGroup[i].first == file.homedirectory)
				{
					currentGroup[i] = pair;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				currentGroup.Add(pair);
			}
			groups[groupIndex].animFiles.Add(file);
			groups[groupIndex].animNames.Add(file.name);
			return true;
		}
		return false;
	}

	public AddModResult AddAnimMod(GroupFile gf, AnimCommandFile akf, KAnimFile file)
	{
		Debug.Assert(gf != null);
		Debug.Assert(file != null, gf.groupID);
		Debug.Assert(akf != null, gf.groupID);
		int index = AddGroup(akf, gf, file);
		string name = file.GetData().name;
		int num = groups[index].animFiles.FindIndex((KAnimFile candidate) => candidate != null && candidate.GetData().name == name);
		if (num == -1)
		{
			groups[index].animFiles.Add(file);
			groups[index].animNames.Add(file.GetData().name);
			return AddModResult.Added;
		}
		groups[index].animFiles[num].mod = file.mod;
		return AddModResult.Replaced;
	}

	public static void LoadGroupResourceFile()
	{
		groupfile = (KAnimGroupFile)Resources.Load("animgrouptags", typeof(KAnimGroupFile));
	}

	public static void LoadAll()
	{
		groupfile.Load();
	}

	public static void MapNamesToAnimFiles(Dictionary<HashedString, KAnimFile> animTable)
	{
		groupfile.DoMapNamesToAnimFiles(animTable);
	}

	private void DoMapNamesToAnimFiles(Dictionary<HashedString, KAnimFile> animTable)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			groups[i].animFiles = new List<KAnimFile>();
			for (int j = 0; j < groups[i].animNames.Count; j++)
			{
				HashedString key = groups[i].animNames[j];
				KAnimFile value = null;
				animTable.TryGetValue(key, out value);
				if (value != null)
				{
					groups[i].animFiles.Add(value);
				}
			}
		}
	}

	private void Load()
	{
		fileData.Clear();
		for (int i = 0; i < groups.Count; i++)
		{
			if (!groups[i].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", i);
			}
			KBatchGroupData kBatchGroupData = null;
			kBatchGroupData = ((!groups[i].target.IsValid) ? KAnimBatchManager.Instance().GetBatchGroupData(groups[i].id) : KAnimBatchManager.Instance().GetBatchGroupData(groups[i].target));
			HashedString batchTag = groups[i].id;
			if (groups[i].renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				if (!groups[i].swapTarget.IsValid)
				{
					continue;
				}
				kBatchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(groups[i].swapTarget);
				batchTag = groups[i].swapTarget;
			}
			for (int j = 0; j < groups[i].animFiles.Count; j++)
			{
				KAnimFile kAnimFile = groups[i].animFiles[j];
				if (kAnimFile != null && kAnimFile.buildBytes != null && !fileData.ContainsKey(kAnimFile.GetInstanceID()))
				{
					if (kAnimFile.buildBytes.Length == 0)
					{
						Debug.LogWarning("Build File [" + kAnimFile.GetData().name + "] has 0 bytes");
						continue;
					}
					HashedString hash = new HashedString(kAnimFile.name);
					HashCache.Get().Add(hash.HashValue, kAnimFile.name);
					KAnimFileData file = KGlobalAnimParser.Get().GetFile(kAnimFile);
					file.maxVisSymbolFrames = 0;
					file.batchTag = batchTag;
					file.buildIndex = KGlobalAnimParser.ParseBuildData(kBatchGroupData, hash, new FastReader(kAnimFile.buildBytes), kAnimFile.textureList);
					fileData.Add(kAnimFile.GetInstanceID(), file);
				}
			}
		}
		for (int k = 0; k < groups.Count; k++)
		{
			if (groups[k].renderType != KAnimBatchGroup.RendererType.AnimOnly)
			{
				continue;
			}
			KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(groups[k].swapTarget);
			KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[k].animTarget);
			for (int l = 0; l < batchGroupData.builds.Count; l++)
			{
				KAnim.Build build = batchGroupData.builds[l];
				if (build == null || build.symbols == null)
				{
					continue;
				}
				for (int m = 0; m < build.symbols.Length; m++)
				{
					KAnim.Build.Symbol symbol = build.symbols[m];
					if (symbol != null && symbol.hash.IsValid() && batchGroupData2.GetFirstIndex(symbol.hash) == -1)
					{
						KAnim.Build.Symbol symbol2 = new KAnim.Build.Symbol();
						symbol2.build = build;
						symbol2.hash = symbol.hash;
						symbol2.path = symbol.path;
						symbol2.colourChannel = symbol.colourChannel;
						symbol2.flags = symbol.flags;
						symbol2.firstFrameIdx = batchGroupData2.symbolFrameInstances.Count;
						symbol2.numFrames = symbol.numFrames;
						symbol2.symbolIndexInSourceBuild = batchGroupData2.frameElementSymbols.Count;
						for (int n = 0; n < symbol2.numFrames; n++)
						{
							KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData.GetSymbolFrameInstance(n + symbol.firstFrameIdx);
							KAnim.Build.SymbolFrameInstance item = default(KAnim.Build.SymbolFrameInstance);
							item.symbolFrame = symbolFrameInstance.symbolFrame;
							item.buildImageIdx = -1;
							item.symbolIdx = batchGroupData2.GetSymbolCount();
							batchGroupData2.symbolFrameInstances.Add(item);
						}
						batchGroupData2.AddBuildSymbol(symbol2);
					}
				}
			}
		}
		for (int num = 0; num < groups.Count; num++)
		{
			if (!groups[num].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", num);
			}
			if (groups[num].renderType == KAnimBatchGroup.RendererType.DontRender)
			{
				continue;
			}
			KBatchGroupData kBatchGroupData2 = null;
			if (groups[num].animTarget.IsValid)
			{
				kBatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num].animTarget);
				if (kBatchGroupData2 == null)
				{
					Debug.LogErrorFormat("Anim group is null for [{0}] -> [{1}]", groups[num].id, groups[num].animTarget);
				}
			}
			else
			{
				kBatchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num].id);
				if (kBatchGroupData2 == null)
				{
					Debug.LogErrorFormat("Anim group is null for [{0}]", groups[num].id);
				}
			}
			for (int num2 = 0; num2 < groups[num].animFiles.Count; num2++)
			{
				KAnimFile kAnimFile2 = groups[num].animFiles[num2];
				if (!(kAnimFile2 != null) || kAnimFile2.animBytes == null)
				{
					continue;
				}
				if (kAnimFile2.animBytes.Length == 0)
				{
					Debug.LogWarning("Anim File [" + kAnimFile2.GetData().name + "] has 0 bytes");
					continue;
				}
				if (!fileData.ContainsKey(kAnimFile2.GetInstanceID()))
				{
					KAnimFileData file2 = KGlobalAnimParser.Get().GetFile(kAnimFile2);
					file2.maxVisSymbolFrames = 0;
					file2.batchTag = groups[num].id;
					fileData.Add(kAnimFile2.GetInstanceID(), file2);
				}
				HashedString fileNameHash = new HashedString(kAnimFile2.name);
				FastReader reader = new FastReader(kAnimFile2.animBytes);
				KAnimFileData animFile = fileData[kAnimFile2.GetInstanceID()];
				KGlobalAnimParser.ParseAnimData(kBatchGroupData2, fileNameHash, reader, animFile);
			}
		}
		for (int num3 = 0; num3 < groups.Count; num3++)
		{
			if (!groups[num3].id.IsValid)
			{
				Debug.LogErrorFormat("Group invalid groupIndex [{0}]", num3);
			}
			KBatchGroupData kBatchGroupData3 = null;
			if (groups[num3].target.IsValid)
			{
				kBatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num3].target);
				if (kBatchGroupData3 == null)
				{
					Debug.LogErrorFormat("Group is null for  [{0}] target [{1}]", groups[num3].id, groups[num3].target);
				}
			}
			else
			{
				kBatchGroupData3 = KAnimBatchManager.Instance().GetBatchGroupData(groups[num3].id);
				if (kBatchGroupData3 == null)
				{
					Debug.LogErrorFormat("Group is null for [{0}]", groups[num3].id);
				}
			}
			KGlobalAnimParser.PostParse(kBatchGroupData3);
		}
	}
}
