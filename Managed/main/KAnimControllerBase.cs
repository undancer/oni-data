using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class KAnimControllerBase : MonoBehaviour
{
	public struct OverrideAnimFileData
	{
		public float priority;

		public KAnimFile file;
	}

	public struct AnimLookupData
	{
		public int animIndex;
	}

	public struct AnimData
	{
		public HashedString anim;

		public KAnim.PlayMode mode;

		public float speed;

		public float timeOffset;
	}

	public enum VisibilityType
	{
		Default,
		OffscreenUpdate,
		Always
	}

	public delegate void KAnimEvent(HashedString name);

	[NonSerialized]
	public GameObject showWhenMissing;

	[SerializeField]
	public KAnimBatchGroup.MaterialType materialType = KAnimBatchGroup.MaterialType.Default;

	[SerializeField]
	public string initialAnim;

	[SerializeField]
	public KAnim.PlayMode initialMode = KAnim.PlayMode.Once;

	[SerializeField]
	protected KAnimFile[] animFiles = new KAnimFile[0];

	[SerializeField]
	protected Vector3 offset;

	[SerializeField]
	protected Vector3 pivot;

	[SerializeField]
	protected float rotation;

	[SerializeField]
	public bool destroyOnAnimComplete;

	[SerializeField]
	public bool inactiveDisable = false;

	[SerializeField]
	protected bool flipX = false;

	[SerializeField]
	protected bool flipY = false;

	[SerializeField]
	public bool forceUseGameTime = false;

	public string defaultAnim;

	protected KAnim.Anim curAnim;

	protected int curAnimFrameIdx = KAnim.Anim.Frame.InvalidFrame.idx;

	protected int prevAnimFrame = KAnim.Anim.Frame.InvalidFrame.idx;

	public bool usingNewSymbolOverrideSystem;

	protected HandleVector<int>.Handle eventManagerHandle = HandleVector<int>.InvalidHandle;

	protected List<OverrideAnimFileData> overrideAnimFiles = new List<OverrideAnimFileData>();

	protected DeepProfiler DeepProfiler = new DeepProfiler(enable_profiling: false);

	public bool randomiseLoopedOffset = false;

	protected float elapsedTime = 0f;

	protected float playSpeed = 1f;

	protected KAnim.PlayMode mode = KAnim.PlayMode.Once;

	protected bool stopped = true;

	public float animHeight = 1f;

	public float animWidth = 1f;

	protected bool isVisible;

	protected Bounds bounds;

	public Action<Bounds> OnUpdateBounds;

	public Action<Color> OnTintChanged;

	public Action<Color> OnHighlightChanged;

	protected KAnimSynchronizer synchronizer;

	protected KAnimLayering layering;

	[SerializeField]
	protected bool _enabled = true;

	protected bool hasEnableRun = false;

	protected bool hasAwakeRun = false;

	protected KBatchedAnimInstanceData batchInstanceData = null;

	public VisibilityType visibilityType = VisibilityType.Default;

	public Action<GameObject> onDestroySelf;

	[SerializeField]
	protected List<KAnimHashedString> hiddenSymbols = new List<KAnimHashedString>();

	protected Dictionary<HashedString, AnimLookupData> anims = new Dictionary<HashedString, AnimLookupData>();

	protected Dictionary<HashedString, AnimLookupData> overrideAnims = new Dictionary<HashedString, AnimLookupData>();

	protected Queue<AnimData> animQueue = new Queue<AnimData>();

	protected int maxSymbols = 0;

	public Grid.SceneLayer fgLayer = Grid.SceneLayer.NoLayer;

	protected AnimEventManager aem = null;

	private static HashedString snaptoPivot = new HashedString("snapTo_pivot");

	public string debugName
	{
		get;
		private set;
	}

	public KAnim.Build curBuild
	{
		get;
		protected set;
	}

	public new bool enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			if (hasAwakeRun)
			{
				if (_enabled)
				{
					Enable();
				}
				else
				{
					Disable();
				}
			}
		}
	}

	public bool HasBatchInstanceData => batchInstanceData != null;

	public SymbolInstanceGpuData symbolInstanceGpuData
	{
		get;
		protected set;
	}

	public SymbolOverrideInfoGpuData symbolOverrideInfoGpuData
	{
		get;
		protected set;
	}

	public Color32 TintColour
	{
		get
		{
			return batchInstanceData.GetTintColour();
		}
		set
		{
			if (batchInstanceData != null && batchInstanceData.SetTintColour(value))
			{
				SetDirty();
				SuspendUpdates(suspend: false);
				if (OnTintChanged != null)
				{
					OnTintChanged(value);
				}
			}
		}
	}

	public Color32 HighlightColour
	{
		get
		{
			return batchInstanceData.GetHighlightcolour();
		}
		set
		{
			if (batchInstanceData.SetHighlightColour(value))
			{
				SetDirty();
				SuspendUpdates(suspend: false);
				if (OnHighlightChanged != null)
				{
					OnHighlightChanged(value);
				}
			}
		}
	}

	public Color OverlayColour
	{
		get
		{
			return batchInstanceData.GetOverlayColour();
		}
		set
		{
			if (batchInstanceData.SetOverlayColour(value))
			{
				SetDirty();
				SuspendUpdates(suspend: false);
				if (this.OnOverlayColourChanged != null)
				{
					this.OnOverlayColourChanged(value);
				}
			}
		}
	}

	public int previousFrame
	{
		get;
		protected set;
	}

	public int currentFrame
	{
		get;
		protected set;
	}

	public string currentAnim
	{
		get;
		protected set;
	}

	public string currentAnimFile
	{
		get;
		protected set;
	}

	public KAnimHashedString currentAnimFileHash
	{
		get;
		protected set;
	}

	public float PlaySpeedMultiplier
	{
		get;
		set;
	}

	public KAnim.PlayMode PlayMode
	{
		get
		{
			return mode;
		}
		set
		{
			mode = value;
		}
	}

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
		}
	}

	public Vector3 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
			if (layering != null)
			{
				layering.Dirty();
			}
			DeRegister();
			Register();
			RefreshVisibilityListener();
			SetDirty();
		}
	}

	public float Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			if (layering != null)
			{
				layering.Dirty();
			}
			SetDirty();
		}
	}

	public Vector3 Pivot
	{
		get
		{
			return pivot;
		}
		set
		{
			pivot = value;
			if (layering != null)
			{
				layering.Dirty();
			}
			SetDirty();
		}
	}

	public Vector3 PositionIncludingOffset => base.transform.GetPosition() + Offset;

	public KAnim.Anim CurrentAnim => curAnim;

	public KAnimFile[] AnimFiles
	{
		get
		{
			return animFiles;
		}
		set
		{
			DebugUtil.Assert(value.Length != 0, "Controller for " + base.gameObject.name + " has no anim files.");
			DebugUtil.Assert(value[0] != null, "First anim file for " + base.gameObject.name + " needs to be non-null.");
			DebugUtil.Assert(value[0].buildBytes != null, "First anim file for " + base.gameObject.name + " needs to be the build file.");
			for (int i = 0; i < value.Length; i++)
			{
				DebugUtil.Assert(value[i] != null, "Anim file is null", base.name);
			}
			animFiles = new KAnimFile[value.Length];
			for (int j = 0; j < value.Length; j++)
			{
				animFiles[j] = value[j];
			}
		}
	}

	public event Action<Color32> OnOverlayColourChanged;

	public event KAnimEvent onAnimEnter;

	public event KAnimEvent onAnimComplete;

	public event Action<int> onLayerChanged;

	protected KAnimControllerBase()
	{
		previousFrame = -1;
		currentFrame = -1;
		PlaySpeedMultiplier = 1f;
		synchronizer = new KAnimSynchronizer(this);
		layering = new KAnimLayering(this, fgLayer);
		isVisible = true;
	}

	public abstract KAnim.Anim GetAnim(int index);

	public void SetFGLayer(Grid.SceneLayer layer)
	{
		fgLayer = layer;
		GetLayering();
		if (layering != null)
		{
			layering.SetLayer(fgLayer);
		}
	}

	public KAnimBatchGroup.MaterialType GetMaterialType()
	{
		return materialType;
	}

	public Vector3 GetWorldPivot()
	{
		Vector3 position = base.transform.GetPosition();
		KBoxCollider2D component = GetComponent<KBoxCollider2D>();
		if (component != null)
		{
			position.x += component.offset.x;
			position.y += component.offset.y - component.size.y / 2f;
		}
		return position;
	}

	public KAnim.Anim GetCurrentAnim()
	{
		return curAnim;
	}

	public KAnimHashedString GetBuildHash()
	{
		if (curBuild == null)
		{
			return KAnimBatchManager.NO_BATCH;
		}
		return curBuild.fileHash;
	}

	protected float GetDuration()
	{
		if (curAnim != null)
		{
			return (float)curAnim.numFrames / curAnim.frameRate;
		}
		return 0f;
	}

	protected int GetFrameIdxFromOffset(int offset)
	{
		int result = -1;
		if (curAnim != null)
		{
			result = offset + curAnim.firstFrameIdx;
		}
		return result;
	}

	public int GetFrameIdx(float time, bool absolute)
	{
		int result = -1;
		if (curAnim != null)
		{
			result = curAnim.GetFrameIdx(mode, time) + (absolute ? curAnim.firstFrameIdx : 0);
		}
		return result;
	}

	public bool IsStopped()
	{
		return stopped;
	}

	public KAnimSynchronizer GetSynchronizer()
	{
		return synchronizer;
	}

	public KAnimLayering GetLayering()
	{
		if (layering == null && fgLayer != Grid.SceneLayer.NoLayer)
		{
			layering = new KAnimLayering(this, fgLayer);
		}
		return layering;
	}

	public KAnim.PlayMode GetMode()
	{
		return mode;
	}

	public static string GetModeString(KAnim.PlayMode mode)
	{
		return mode switch
		{
			KAnim.PlayMode.Once => "Once", 
			KAnim.PlayMode.Loop => "Loop", 
			KAnim.PlayMode.Paused => "Paused", 
			_ => "Unknown", 
		};
	}

	public float GetPlaySpeed()
	{
		return playSpeed;
	}

	public void SetElapsedTime(float value)
	{
		elapsedTime = value;
	}

	public float GetElapsedTime()
	{
		return elapsedTime;
	}

	protected abstract void SuspendUpdates(bool suspend);

	protected abstract void OnStartQueuedAnim();

	public abstract void SetDirty();

	protected abstract void RefreshVisibilityListener();

	protected abstract void DeRegister();

	protected abstract void Register();

	protected abstract void OnAwake();

	protected abstract void OnStart();

	protected abstract void OnStop();

	protected abstract void Enable();

	protected abstract void Disable();

	protected abstract void UpdateFrame(float t);

	public abstract Matrix2x3 GetTransformMatrix();

	public abstract Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible);

	public abstract void UpdateHidden();

	public abstract void TriggerStop();

	public virtual void SetLayer(int layer)
	{
		if (this.onLayerChanged != null)
		{
			this.onLayerChanged(layer);
		}
	}

	public Vector3 GetPivotSymbolPosition()
	{
		bool symbolVisible = false;
		Matrix4x4 symbolTransform = GetSymbolTransform(snaptoPivot, out symbolVisible);
		Vector3 result = base.transform.GetPosition();
		if (symbolVisible)
		{
			result = new Vector3(symbolTransform[0, 3], symbolTransform[1, 3], symbolTransform[2, 3]);
		}
		return result;
	}

	public virtual Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		symbolVisible = false;
		return Matrix4x4.identity;
	}

	private void Awake()
	{
		if (Global.Instance != null)
		{
			aem = Global.Instance.GetAnimEventManager();
		}
		debugName = base.name;
		SetFGLayer(fgLayer);
		OnAwake();
		if (!string.IsNullOrEmpty(initialAnim))
		{
			SetDirty();
			Play(initialAnim, initialMode);
		}
		hasAwakeRun = true;
	}

	private void Start()
	{
		OnStart();
	}

	protected virtual void OnDestroy()
	{
		animFiles = null;
		curAnim = null;
		curBuild = null;
		synchronizer = null;
		layering = null;
		animQueue = null;
		overrideAnims = null;
		anims = null;
		synchronizer = null;
		layering = null;
		overrideAnimFiles = null;
	}

	protected void AnimEnter(HashedString hashed_name)
	{
		if (this.onAnimEnter != null)
		{
			this.onAnimEnter(hashed_name);
		}
	}

	public void Play(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (!stopped)
		{
			Stop();
		}
		Queue(anim_name, mode, speed, time_offset);
	}

	public void Play(HashedString[] anim_names, KAnim.PlayMode mode = KAnim.PlayMode.Once)
	{
		if (!stopped)
		{
			Stop();
		}
		for (int i = 0; i < anim_names.Length - 1; i++)
		{
			Queue(anim_names[i]);
		}
		Debug.Assert(anim_names.Length != 0, "Play was called with an empty anim array");
		Queue(anim_names[anim_names.Length - 1], mode);
	}

	public void Queue(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		animQueue.Enqueue(new AnimData
		{
			anim = anim_name,
			mode = mode,
			speed = speed,
			timeOffset = time_offset
		});
		this.mode = ((mode != KAnim.PlayMode.Paused) ? KAnim.PlayMode.Once : KAnim.PlayMode.Paused);
		if (aem != null)
		{
			aem.SetMode(eventManagerHandle, this.mode);
		}
		if (animQueue.Count == 1 && stopped)
		{
			StartQueuedAnim();
		}
	}

	public void ClearQueue()
	{
		animQueue.Clear();
	}

	private void Restart(HashedString anim_name, KAnim.PlayMode mode = KAnim.PlayMode.Once, float speed = 1f, float time_offset = 0f)
	{
		if (curBuild == null)
		{
			string[] obj = new string[5]
			{
				"[",
				base.gameObject.name,
				"] Missing build while trying to play anim [",
				null,
				null
			};
			HashedString hashedString = anim_name;
			obj[3] = hashedString.ToString();
			obj[4] = "]";
			Debug.LogWarning(string.Concat(obj), base.gameObject);
			return;
		}
		Queue<AnimData> queue = new Queue<AnimData>();
		queue.Enqueue(new AnimData
		{
			anim = anim_name,
			mode = mode,
			speed = speed,
			timeOffset = time_offset
		});
		while (animQueue.Count > 0)
		{
			queue.Enqueue(animQueue.Dequeue());
		}
		animQueue = queue;
		if (animQueue.Count == 1 && stopped)
		{
			StartQueuedAnim();
		}
	}

	protected void StartQueuedAnim()
	{
		StopAnimEventSequence();
		previousFrame = -1;
		currentFrame = -1;
		SuspendUpdates(suspend: false);
		stopped = false;
		OnStartQueuedAnim();
		AnimData animData = animQueue.Dequeue();
		while (animData.mode == KAnim.PlayMode.Loop && animQueue.Count > 0)
		{
			animData = animQueue.Dequeue();
		}
		if (overrideAnims == null || !overrideAnims.TryGetValue(animData.anim, out var value))
		{
			if (!anims.TryGetValue(animData.anim, out value))
			{
				bool flag = true;
				if (showWhenMissing != null)
				{
					showWhenMissing.SetActive(value: true);
				}
				if (flag)
				{
					TriggerStop();
					return;
				}
			}
			else if (showWhenMissing != null)
			{
				showWhenMissing.SetActive(value: false);
			}
		}
		curAnim = GetAnim(value.animIndex);
		int num = 0;
		if (animData.mode == KAnim.PlayMode.Loop && randomiseLoopedOffset)
		{
			num = UnityEngine.Random.Range(0, curAnim.numFrames - 1);
		}
		prevAnimFrame = -1;
		curAnimFrameIdx = GetFrameIdxFromOffset(num);
		currentFrame = curAnimFrameIdx;
		mode = animData.mode;
		playSpeed = animData.speed * PlaySpeedMultiplier;
		SetElapsedTime((float)num / curAnim.frameRate + animData.timeOffset);
		synchronizer.Sync();
		StartAnimEventSequence();
		AnimEnter(animData.anim);
	}

	public bool GetSymbolVisiblity(KAnimHashedString symbol)
	{
		return !hiddenSymbols.Contains(symbol);
	}

	public void SetSymbolVisiblity(KAnimHashedString symbol, bool is_visible)
	{
		if (is_visible)
		{
			hiddenSymbols.Remove(symbol);
		}
		else if (!hiddenSymbols.Contains(symbol))
		{
			hiddenSymbols.Add(symbol);
		}
		if (curBuild != null)
		{
			UpdateHidden();
		}
	}

	public void AddAnimOverrides(KAnimFile kanim_file, float priority = 0f)
	{
		Debug.Assert(kanim_file != null);
		if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
		{
			SymbolOverrideController component = GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, "Anim overrides containing additional symbols require a symbol override controller.");
			component.AddBuildOverride(kanim_file.GetData());
		}
		overrideAnimFiles.Add(new OverrideAnimFileData
		{
			priority = priority,
			file = kanim_file
		});
		overrideAnimFiles.Sort((OverrideAnimFileData a, OverrideAnimFileData b) => b.priority.CompareTo(a.priority));
		RebuildOverrides(kanim_file);
	}

	public void RemoveAnimOverrides(KAnimFile kanim_file)
	{
		Debug.Assert(kanim_file != null);
		if (kanim_file.GetData().build != null && kanim_file.GetData().build.symbols.Length != 0)
		{
			SymbolOverrideController component = GetComponent<SymbolOverrideController>();
			DebugUtil.Assert(component != null, "Anim overrides containing additional symbols require a symbol override controller.");
			component.TryRemoveBuildOverride(kanim_file.GetData());
		}
		for (int i = 0; i < overrideAnimFiles.Count; i++)
		{
			if (overrideAnimFiles[i].file == kanim_file)
			{
				overrideAnimFiles.RemoveAt(i);
				break;
			}
		}
		RebuildOverrides(kanim_file);
	}

	private void RebuildOverrides(KAnimFile kanim_file)
	{
		bool flag = false;
		overrideAnims.Clear();
		for (int i = 0; i < overrideAnimFiles.Count; i++)
		{
			OverrideAnimFileData overrideAnimFileData = overrideAnimFiles[i];
			KAnimFileData data = overrideAnimFileData.file.GetData();
			for (int j = 0; j < data.animCount; j++)
			{
				KAnim.Anim anim = data.GetAnim(j);
				if (anim.animFile.hashName != data.hashName)
				{
					Debug.LogError($"How did we get an anim from another file? [{data.name}] != [{anim.animFile.name}] for anim [{j}]");
				}
				AnimLookupData value = default(AnimLookupData);
				value.animIndex = anim.index;
				HashedString hashedString = new HashedString(anim.name);
				if (!overrideAnims.ContainsKey(hashedString))
				{
					overrideAnims[hashedString] = value;
				}
				if (curAnim != null && curAnim.hash == hashedString && overrideAnimFileData.file == kanim_file)
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			Restart(curAnim.name, mode, playSpeed);
		}
	}

	public bool HasAnimation(HashedString anim_name)
	{
		if (anims.ContainsKey(anim_name))
		{
			return true;
		}
		if (overrideAnims.ContainsKey(anim_name))
		{
			return true;
		}
		return false;
	}

	public void AddAnims(KAnimFile anim_file)
	{
		KAnimFileData data = anim_file.GetData();
		if (data == null)
		{
			Debug.LogError("AddAnims() Null animfile data");
			return;
		}
		maxSymbols = Mathf.Max(maxSymbols, data.maxVisSymbolFrames);
		for (int i = 0; i < data.animCount; i++)
		{
			KAnim.Anim anim = data.GetAnim(i);
			if (anim.animFile.hashName != data.hashName)
			{
				Debug.LogErrorFormat("How did we get an anim from another file? [{0}] != [{1}] for anim [{2}]", data.name, anim.animFile.name, i);
			}
			anims[anim.hash] = new AnimLookupData
			{
				animIndex = anim.index
			};
		}
		if (usingNewSymbolOverrideSystem && data.buildIndex != -1 && data.build.symbols != null && data.build.symbols.Length != 0)
		{
			SymbolOverrideController component = GetComponent<SymbolOverrideController>();
			component.AddBuildOverride(anim_file.GetData(), -1);
		}
	}

	public void Stop()
	{
		if (curAnim != null)
		{
			StopAnimEventSequence();
		}
		animQueue.Clear();
		stopped = true;
		if (this.onAnimComplete != null)
		{
			this.onAnimComplete((curAnim == null) ? HashedString.Invalid : curAnim.hash);
		}
		OnStop();
	}

	public void StopAndClear()
	{
		if (!stopped)
		{
			Stop();
		}
		bounds.center = Vector3.zero;
		bounds.extents = Vector3.zero;
		if (OnUpdateBounds != null)
		{
			OnUpdateBounds(bounds);
		}
	}

	public float GetPositionPercent()
	{
		return GetElapsedTime() / GetDuration();
	}

	public void SetPositionPercent(float percent)
	{
		if (curAnim != null)
		{
			SetElapsedTime((float)curAnim.numFrames / curAnim.frameRate * percent);
			int frameIdx = curAnim.GetFrameIdx(mode, elapsedTime);
			if (currentFrame != frameIdx)
			{
				SetDirty();
				UpdateAnimEventSequenceTime();
				SuspendUpdates(suspend: false);
			}
		}
	}

	protected void StartAnimEventSequence()
	{
		if (!layering.GetIsForeground() && aem != null)
		{
			eventManagerHandle = aem.PlayAnim(this, curAnim, mode, elapsedTime, visibilityType == VisibilityType.Always);
		}
	}

	protected void UpdateAnimEventSequenceTime()
	{
		if (eventManagerHandle.IsValid() && aem != null)
		{
			aem.SetElapsedTime(eventManagerHandle, elapsedTime);
		}
	}

	protected void StopAnimEventSequence()
	{
		if (eventManagerHandle.IsValid() && aem != null)
		{
			if (!stopped && mode != KAnim.PlayMode.Paused)
			{
				SetElapsedTime(aem.GetElapsedTime(eventManagerHandle));
			}
			aem.StopAnim(eventManagerHandle);
			eventManagerHandle = HandleVector<int>.InvalidHandle;
		}
	}

	protected void DestroySelf()
	{
		if (onDestroySelf != null)
		{
			onDestroySelf(base.gameObject);
		}
		else
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}
}
