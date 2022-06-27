using System.Collections.Generic;
using UnityEngine;

public class KAnimFile : ScriptableObject
{
	public class Mod
	{
		public byte[] anim;

		public byte[] build;

		public List<Texture2D> textures = new List<Texture2D>();

		public bool IsValid()
		{
			return anim != null;
		}
	}

	public const string ANIM_ROOT_PATH = "Assets/anim";

	[SerializeField]
	private TextAsset animFile;

	[SerializeField]
	private TextAsset buildFile;

	[SerializeField]
	private List<Texture2D> textures = new List<Texture2D>();

	public Mod mod;

	private KAnimFileData data;

	private HashedString _batchTag;

	public string homedirectory = "";

	public bool IsBuildLoaded { get; private set; }

	public bool IsAnimLoaded { get; private set; }

	public byte[] animBytes
	{
		get
		{
			if (mod != null)
			{
				return mod.anim;
			}
			if (!(animFile != null))
			{
				return null;
			}
			return animFile.bytes;
		}
	}

	public byte[] buildBytes
	{
		get
		{
			if (mod != null)
			{
				return mod.build;
			}
			if (!(buildFile != null))
			{
				return null;
			}
			return buildFile.bytes;
		}
	}

	public List<Texture2D> textureList
	{
		get
		{
			if (mod != null)
			{
				return mod.textures;
			}
			return textures;
		}
	}

	public HashedString batchTag
	{
		get
		{
			if (_batchTag.IsValid)
			{
				return _batchTag;
			}
			if (homedirectory == null || homedirectory == "")
			{
				return KAnimBatchManager.NO_BATCH;
			}
			_batchTag = KAnimGroupFile.GetGroupForHomeDirectory(new HashedString(homedirectory));
			return _batchTag;
		}
	}

	public void FinalizeLoading()
	{
		IsBuildLoaded = buildBytes != null;
		IsAnimLoaded = animBytes != null;
		animFile = null;
		buildFile = null;
	}

	public void Initialize(TextAsset anim, TextAsset build, IList<Texture2D> textures)
	{
		animFile = anim;
		buildFile = build;
		this.textures.Clear();
		this.textures.AddRange(textures);
	}

	public KAnimFileData GetData()
	{
		if (data == null)
		{
			KGlobalAnimParser kGlobalAnimParser = KGlobalAnimParser.Get();
			if (kGlobalAnimParser != null)
			{
				data = kGlobalAnimParser.Load(this);
			}
		}
		return data;
	}
}
