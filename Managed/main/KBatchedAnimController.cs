using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

[DebuggerDisplay("{name} visible={isVisible} suspendUpdates={suspendUpdates} moving={moving}")]
public class KBatchedAnimController : KAnimControllerBase, KAnimConverter.IAnimConverter
{
	[NonSerialized]
	protected bool _forceRebuild;

	private Vector3 lastPos = Vector3.zero;

	private Vector2I lastChunkXY = KBatchedAnimUpdater.INVALID_CHUNK_ID;

	private KAnimBatch batch;

	public float animScale = 0.005f;

	private bool suspendUpdates;

	private bool visibilityListenerRegistered;

	private bool moving;

	private SymbolOverrideController symbolOverrideController;

	private int symbolOverrideControllerVersion;

	[NonSerialized]
	public KBatchedAnimUpdater.RegistrationState updateRegistrationState = KBatchedAnimUpdater.RegistrationState.Unregistered;

	public Grid.SceneLayer sceneLayer;

	private RectTransform rt;

	private Vector3 screenOffset = new Vector3(0f, 0f, 0f);

	public Matrix2x3 navMatrix = Matrix2x3.identity;

	private CanvasScaler scaler;

	public bool setScaleFromAnim = true;

	public Vector2 animOverrideSize = Vector2.one;

	private Canvas rootCanvas;

	public bool isMovable;

	protected bool forceRebuild
	{
		get
		{
			return _forceRebuild;
		}
		set
		{
			_forceRebuild = value;
		}
	}

	public HashedString batchGroupID { get; private set; }

	public int GetCurrentFrameIndex()
	{
		return curAnimFrameIdx;
	}

	public KBatchedAnimInstanceData GetBatchInstanceData()
	{
		return batchInstanceData;
	}

	public KBatchedAnimController()
	{
		batchInstanceData = new KBatchedAnimInstanceData(this);
	}

	public bool IsActive()
	{
		if (base.isActiveAndEnabled)
		{
			return _enabled;
		}
		return false;
	}

	public bool IsVisible()
	{
		return isVisible;
	}

	public void SetSymbolScale(KAnimHashedString symbol_name, float scale)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(GetBatchGroupID()).GetSymbol(symbol_name);
		if (symbol != null)
		{
			base.symbolInstanceGpuData.SetSymbolScale(symbol.symbolIndexInSourceBuild, scale);
			SuspendUpdates(suspend: false);
			SetDirty();
		}
	}

	public void SetSymbolTint(KAnimHashedString symbol_name, Color color)
	{
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(GetBatchGroupID()).GetSymbol(symbol_name);
		if (symbol != null)
		{
			base.symbolInstanceGpuData.SetSymbolTint(symbol.symbolIndexInSourceBuild, color);
			SuspendUpdates(suspend: false);
			SetDirty();
		}
	}

	public Vector2I GetCellXY()
	{
		Vector3 positionIncludingOffset = base.PositionIncludingOffset;
		if (Grid.CellSizeInMeters == 0f)
		{
			return new Vector2I((int)positionIncludingOffset.x, (int)positionIncludingOffset.y);
		}
		return Grid.PosToXY(positionIncludingOffset);
	}

	public float GetZ()
	{
		return base.transform.GetPosition().z;
	}

	public string GetName()
	{
		return base.name;
	}

	public override KAnim.Anim GetAnim(int index)
	{
		if (!batchGroupID.IsValid || !(batchGroupID != KAnimBatchManager.NO_BATCH))
		{
			Debug.LogError(base.name + " batch not ready");
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(batchGroupID);
		Debug.Assert(batchGroupData != null);
		return batchGroupData.GetAnim(index);
	}

	private void Initialize()
	{
		if (batchGroupID.IsValid && batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			DeRegister();
			Register();
		}
	}

	private void OnMovementStateChanged(bool is_moving)
	{
		if (is_moving != moving)
		{
			moving = is_moving;
			SetDirty();
			ConfigureUpdateListener();
		}
	}

	private static void OnMovementStateChanged(Transform transform, bool is_moving)
	{
		transform.GetComponent<KBatchedAnimController>().OnMovementStateChanged(is_moving);
	}

	private void SetBatchGroup(KAnimFileData kafd)
	{
		if (!batchGroupID.IsValid || kafd == null || !(batchGroupID == kafd.batchTag))
		{
			DebugUtil.Assert(!batchGroupID.IsValid, "Should only be setting the batch group once.");
			DebugUtil.Assert(kafd != null, "Null anim data!! For", base.name);
			base.curBuild = kafd.build;
			DebugUtil.Assert(base.curBuild != null, "Null build for anim!! ", base.name, kafd.name);
			KAnimGroupFile.Group group = KAnimGroupFile.GetGroup(base.curBuild.batchTag);
			HashedString hashedString = kafd.build.batchTag;
			if (group.renderType == KAnimBatchGroup.RendererType.DontRender || group.renderType == KAnimBatchGroup.RendererType.AnimOnly)
			{
				bool isValid = group.swapTarget.IsValid;
				HashedString id = group.id;
				Debug.Assert(isValid, "Invalid swap target fro group [" + id.ToString() + "]");
				hashedString = group.swapTarget;
			}
			batchGroupID = hashedString;
			base.symbolInstanceGpuData = new SymbolInstanceGpuData(KAnimBatchManager.instance.GetBatchGroupData(batchGroupID).maxSymbolsPerBuild);
			base.symbolOverrideInfoGpuData = new SymbolOverrideInfoGpuData(KAnimBatchManager.instance.GetBatchGroupData(batchGroupID).symbolFrameInstances.Count);
			if (!batchGroupID.IsValid || batchGroupID == KAnimBatchManager.NO_BATCH)
			{
				Debug.LogError("Batch is not ready: " + base.name);
			}
		}
	}

	public void LoadAnims()
	{
		if (!KAnimBatchManager.Instance().isReady)
		{
			Debug.LogError("KAnimBatchManager is not ready when loading anim:" + base.name);
		}
		if (animFiles.Length == 0)
		{
			DebugUtil.Assert(test: false, "KBatchedAnimController has no anim files:" + base.name);
		}
		if (animFiles[0].buildBytes == null)
		{
			DebugUtil.LogErrorArgs(base.gameObject, $"First anim file needs to be the build file but {animFiles[0].GetData().name} doesn't have an associated build");
		}
		overrideAnims.Clear();
		anims.Clear();
		SetBatchGroup(animFiles[0].GetData());
		for (int i = 0; i < animFiles.Length; i++)
		{
			AddAnims(animFiles[i]);
		}
		forceRebuild = true;
		if (layering != null)
		{
			layering.HideSymbols();
		}
		if (usingNewSymbolOverrideSystem)
		{
			DebugUtil.Assert(GetComponent<SymbolOverrideController>() != null);
		}
	}

	public void SwapAnims(KAnimFile[] anims)
	{
		if (batchGroupID.IsValid)
		{
			DeRegister();
			batchGroupID = HashedString.Invalid;
		}
		base.AnimFiles = anims;
		LoadAnims();
		Register();
	}

	public void UpdateAnim(float dt)
	{
		if (batch != null && base.transform.hasChanged)
		{
			base.transform.hasChanged = false;
			if (batch != null && batch.group.maxGroupSize == 1 && lastPos.z != base.transform.GetPosition().z)
			{
				batch.OverrideZ(base.transform.GetPosition().z);
			}
			Vector3 vector = (lastPos = base.PositionIncludingOffset);
			if (visibilityType != VisibilityType.Always && KAnimBatchManager.ControllerToChunkXY(this) != lastChunkXY && lastChunkXY != KBatchedAnimUpdater.INVALID_CHUNK_ID)
			{
				DeRegister();
				Register();
			}
			SetDirty();
		}
		if (batchGroupID == KAnimBatchManager.NO_BATCH || !IsActive())
		{
			return;
		}
		if (!forceRebuild && (mode == KAnim.PlayMode.Paused || stopped || curAnim == null || (mode == KAnim.PlayMode.Once && curAnim != null && (elapsedTime > curAnim.totalTime || curAnim.totalTime <= 0f) && animQueue.Count == 0)))
		{
			SuspendUpdates(suspend: true);
		}
		if (!isVisible && !forceRebuild)
		{
			if (visibilityType == VisibilityType.OffscreenUpdate && !stopped && mode != KAnim.PlayMode.Paused)
			{
				SetElapsedTime(elapsedTime + dt * playSpeed);
			}
			return;
		}
		curAnimFrameIdx = GetFrameIdx(elapsedTime, absolute: true);
		if (eventManagerHandle.IsValid() && aem != null)
		{
			float num = aem.GetElapsedTime(eventManagerHandle);
			if ((int)((elapsedTime - num) * 100f) != 0)
			{
				UpdateAnimEventSequenceTime();
			}
		}
		UpdateFrame(elapsedTime);
		if (!stopped && mode != KAnim.PlayMode.Paused)
		{
			SetElapsedTime(elapsedTime + dt * playSpeed);
		}
		forceRebuild = false;
	}

	protected override void UpdateFrame(float t)
	{
		base.previousFrame = base.currentFrame;
		if (!stopped || forceRebuild)
		{
			if (curAnim != null && (mode == KAnim.PlayMode.Loop || elapsedTime <= GetDuration() || forceRebuild))
			{
				base.currentFrame = curAnim.GetFrameIdx(mode, elapsedTime);
				if (base.currentFrame != base.previousFrame || forceRebuild)
				{
					SetDirty();
				}
			}
			else
			{
				TriggerStop();
			}
			if (!stopped && mode == KAnim.PlayMode.Loop && base.currentFrame == 0)
			{
				AnimEnter(curAnim.hash);
			}
		}
		if (synchronizer != null)
		{
			synchronizer.SyncTime();
		}
	}

	public override void TriggerStop()
	{
		if (animQueue.Count > 0)
		{
			StartQueuedAnim();
		}
		else if (curAnim != null && mode == KAnim.PlayMode.Once)
		{
			base.currentFrame = curAnim.numFrames - 1;
			Stop();
			base.gameObject.Trigger(-1061186183);
			if (destroyOnAnimComplete)
			{
				DestroySelf();
			}
		}
	}

	public override void UpdateHidden()
	{
		for (int i = 0; i < base.curBuild.symbols.Length; i++)
		{
			KAnim.Build.Symbol symbol = base.curBuild.symbols[i];
			bool is_visible = !hiddenSymbols.Contains(symbol.hash);
			base.symbolInstanceGpuData.SetVisible(i, is_visible);
		}
		SetDirty();
	}

	public int GetMaxVisible()
	{
		return maxSymbols;
	}

	public HashedString GetBatchGroupID(bool isEditorWindow = false)
	{
		Debug.Assert(isEditorWindow || animFiles == null || animFiles.Length == 0 || (batchGroupID.IsValid && batchGroupID != KAnimBatchManager.NO_BATCH));
		return batchGroupID;
	}

	public int GetLayer()
	{
		return base.gameObject.layer;
	}

	public KAnimBatch GetBatch()
	{
		return batch;
	}

	public void SetBatch(KAnimBatch new_batch)
	{
		batch = new_batch;
		if (materialType == KAnimBatchGroup.MaterialType.UI)
		{
			KBatchedAnimCanvasRenderer kBatchedAnimCanvasRenderer = GetComponent<KBatchedAnimCanvasRenderer>();
			if (kBatchedAnimCanvasRenderer == null && new_batch != null)
			{
				kBatchedAnimCanvasRenderer = base.gameObject.AddComponent<KBatchedAnimCanvasRenderer>();
			}
			if (kBatchedAnimCanvasRenderer != null)
			{
				kBatchedAnimCanvasRenderer.SetBatch(this);
			}
		}
	}

	public int GetCurrentNumFrames()
	{
		if (curAnim == null)
		{
			return 0;
		}
		return curAnim.numFrames;
	}

	public int GetFirstFrameIndex()
	{
		if (curAnim == null)
		{
			return -1;
		}
		return curAnim.firstFrameIdx;
	}

	private Canvas GetRootCanvas()
	{
		Canvas canvas = null;
		if (rt == null)
		{
			return null;
		}
		RectTransform component = rt.parent.GetComponent<RectTransform>();
		while (component != null)
		{
			canvas = component.GetComponent<Canvas>();
			if (canvas != null && canvas.isRootCanvas)
			{
				return canvas;
			}
			component = component.parent.GetComponent<RectTransform>();
		}
		return null;
	}

	public override Matrix2x3 GetTransformMatrix()
	{
		Vector3 vector = base.PositionIncludingOffset;
		vector.z = 0f;
		Vector2 scale = new Vector2(animScale * animWidth, (0f - animScale) * animHeight);
		if (materialType == KAnimBatchGroup.MaterialType.UI)
		{
			rt = GetComponent<RectTransform>();
			if (rootCanvas == null)
			{
				rootCanvas = GetRootCanvas();
			}
			if (scaler == null && rootCanvas != null)
			{
				scaler = rootCanvas.GetComponent<CanvasScaler>();
			}
			if (rootCanvas == null)
			{
				screenOffset.x = Screen.width / 2;
				screenOffset.y = Screen.height / 2;
			}
			else
			{
				screenOffset.x = rootCanvas.rectTransform().rect.width / 2f;
				screenOffset.y = rootCanvas.rectTransform().rect.height / 2f;
			}
			float num = 1f;
			if (scaler != null)
			{
				num = 1f / scaler.scaleFactor;
			}
			vector = (rt.localToWorldMatrix.MultiplyPoint(rt.pivot) + offset) * num - screenOffset;
			float num2 = animWidth * animScale;
			float num3 = animHeight * animScale;
			if (setScaleFromAnim && curAnim != null)
			{
				num2 *= rt.rect.size.x / curAnim.unScaledSize.x;
				num3 *= rt.rect.size.y / curAnim.unScaledSize.y;
			}
			else
			{
				num2 *= rt.rect.size.x / animOverrideSize.x;
				num3 *= rt.rect.size.y / animOverrideSize.y;
			}
			scale = new Vector3(rt.lossyScale.x * num2 * num, (0f - rt.lossyScale.y) * num3 * num, rt.lossyScale.z * num);
			pivot = rt.pivot;
		}
		Matrix2x3 matrix2x = Matrix2x3.Scale(scale);
		Matrix2x3 matrix2x2 = Matrix2x3.Scale(new Vector2(flipX ? (-1f) : 1f, flipY ? (-1f) : 1f));
		if (rotation != 0f)
		{
			Matrix2x3 matrix2x3 = Matrix2x3.Translate(-pivot);
			Matrix2x3 matrix2x4 = Matrix2x3.Rotate(rotation * ((float)Math.PI / 180f));
			Matrix2x3 matrix2x5 = Matrix2x3.Translate(pivot) * matrix2x4 * matrix2x3;
			return Matrix2x3.TRS(vector, base.transform.rotation, base.transform.localScale) * matrix2x5 * matrix2x * navMatrix * matrix2x2;
		}
		return Matrix2x3.TRS(vector, base.transform.rotation, base.transform.localScale) * matrix2x * navMatrix * matrix2x2;
	}

	public override Matrix4x4 GetSymbolTransform(HashedString symbol, out bool symbolVisible)
	{
		if (curAnimFrameIdx != -1 && batch != null)
		{
			Matrix2x3 symbolLocalTransform = GetSymbolLocalTransform(symbol, out symbolVisible);
			if (symbolVisible)
			{
				return (Matrix4x4)GetTransformMatrix() * (Matrix4x4)symbolLocalTransform;
			}
		}
		symbolVisible = false;
		return default(Matrix4x4);
	}

	public override Matrix2x3 GetSymbolLocalTransform(HashedString symbol, out bool symbolVisible)
	{
		if (curAnimFrameIdx != -1 && batch != null)
		{
			KAnim.Anim.Frame frame = batch.group.data.GetFrame(curAnimFrameIdx);
			if (frame != KAnim.Anim.Frame.InvalidFrame)
			{
				for (int i = 0; i < frame.numElements; i++)
				{
					int num = frame.firstElementIdx + i;
					if (num < batch.group.data.frameElements.Count)
					{
						KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[num];
						if (frameElement.symbol == symbol)
						{
							symbolVisible = true;
							return frameElement.transform;
						}
					}
				}
			}
		}
		symbolVisible = false;
		return Matrix2x3.identity;
	}

	public override void SetLayer(int layer)
	{
		if (layer != base.gameObject.layer)
		{
			base.SetLayer(layer);
			DeRegister();
			base.gameObject.layer = layer;
			Register();
		}
	}

	public override void SetDirty()
	{
		if (batch != null)
		{
			batch.SetDirty(this);
		}
	}

	protected override void OnStartQueuedAnim()
	{
		SuspendUpdates(suspend: false);
	}

	protected override void OnAwake()
	{
		LoadAnims();
		if (visibilityType == VisibilityType.Default)
		{
			visibilityType = ((materialType == KAnimBatchGroup.MaterialType.UI) ? VisibilityType.Always : visibilityType);
		}
		symbolOverrideController = GetComponent<SymbolOverrideController>();
		UpdateHidden();
		hasEnableRun = false;
	}

	protected override void OnStart()
	{
		if (batch == null)
		{
			Initialize();
		}
		if (visibilityType == VisibilityType.Always || visibilityType == VisibilityType.OffscreenUpdate)
		{
			ConfigureUpdateListener();
		}
		CellChangeMonitor instance = Singleton<CellChangeMonitor>.Instance;
		if (instance != null)
		{
			instance.RegisterMovementStateChanged(base.transform, OnMovementStateChanged);
			moving = instance.IsMoving(base.transform);
		}
		symbolOverrideController = GetComponent<SymbolOverrideController>();
		SetDirty();
	}

	protected override void OnStop()
	{
		SetDirty();
	}

	private void OnEnable()
	{
		if (_enabled)
		{
			Enable();
		}
	}

	protected override void Enable()
	{
		if (!hasEnableRun)
		{
			hasEnableRun = true;
			if (batch == null)
			{
				Initialize();
			}
			SetDirty();
			SuspendUpdates(suspend: false);
			ConfigureVisibilityListener(enabled: true);
			if (!stopped && curAnim != null && mode != KAnim.PlayMode.Paused && !eventManagerHandle.IsValid())
			{
				StartAnimEventSequence();
			}
		}
	}

	private void OnDisable()
	{
		Disable();
	}

	protected override void Disable()
	{
		if (!App.IsExiting && !KMonoBehaviour.isLoadingScene && hasEnableRun)
		{
			hasEnableRun = false;
			SuspendUpdates(suspend: true);
			if (batch != null)
			{
				DeRegister();
			}
			ConfigureVisibilityListener(enabled: false);
			StopAnimEventSequence();
		}
	}

	protected override void OnDestroy()
	{
		if (!App.IsExiting)
		{
			Singleton<CellChangeMonitor>.Instance?.UnregisterMovementStateChanged(base.transform, OnMovementStateChanged);
			Singleton<KBatchedAnimUpdater>.Instance?.UpdateUnregister(this);
			isVisible = false;
			DeRegister();
			stopped = true;
			StopAnimEventSequence();
			batchInstanceData = null;
			batch = null;
			base.OnDestroy();
		}
	}

	public void SetBlendValue(float value)
	{
		batchInstanceData.SetBlend(value);
		SetDirty();
	}

	public bool ApplySymbolOverrides()
	{
		if (symbolOverrideController != null)
		{
			if (symbolOverrideControllerVersion != symbolOverrideController.version || symbolOverrideController.applySymbolOverridesEveryFrame)
			{
				symbolOverrideControllerVersion = symbolOverrideController.version;
				symbolOverrideController.ApplyOverrides();
			}
			symbolOverrideController.ApplyAtlases();
			return true;
		}
		return false;
	}

	public void SetSymbolOverrides(int symbol_start_idx, int symbol_num_frames, int atlas_idx, KBatchGroupData source_data, int source_start_idx, int source_num_frames)
	{
		base.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_start_idx, symbol_num_frames, atlas_idx, source_data, source_start_idx, source_num_frames);
	}

	public void SetSymbolOverride(int symbol_idx, ref KAnim.Build.SymbolFrameInstance symbol_frame_instance)
	{
		base.symbolOverrideInfoGpuData.SetSymbolOverrideInfo(symbol_idx, ref symbol_frame_instance);
	}

	protected override void Register()
	{
		if (IsActive() && batch == null && batchGroupID.IsValid && batchGroupID != KAnimBatchManager.NO_BATCH)
		{
			lastChunkXY = KAnimBatchManager.ControllerToChunkXY(this);
			KAnimBatchManager.Instance().Register(this);
			forceRebuild = true;
			SetDirty();
		}
	}

	protected override void DeRegister()
	{
		if (batch != null)
		{
			batch.Deregister(this);
		}
	}

	private void ConfigureUpdateListener()
	{
		if ((IsActive() && !suspendUpdates && isVisible) || moving || visibilityType == VisibilityType.OffscreenUpdate || visibilityType == VisibilityType.Always)
		{
			Singleton<KBatchedAnimUpdater>.Instance.UpdateRegister(this);
		}
		else
		{
			Singleton<KBatchedAnimUpdater>.Instance.UpdateUnregister(this);
		}
	}

	protected override void SuspendUpdates(bool suspend)
	{
		suspendUpdates = suspend;
		ConfigureUpdateListener();
	}

	public void SetVisiblity(bool is_visible)
	{
		if (is_visible != isVisible)
		{
			isVisible = is_visible;
			if (is_visible)
			{
				SuspendUpdates(suspend: false);
				SetDirty();
				UpdateAnimEventSequenceTime();
			}
			else
			{
				SuspendUpdates(suspend: true);
				SetDirty();
			}
		}
	}

	private void ConfigureVisibilityListener(bool enabled)
	{
		if (visibilityType != VisibilityType.Always && visibilityType != VisibilityType.OffscreenUpdate)
		{
			if (enabled)
			{
				RegisterVisibilityListener();
			}
			else
			{
				UnregisterVisibilityListener();
			}
		}
	}

	protected override void RefreshVisibilityListener()
	{
		if (visibilityListenerRegistered)
		{
			ConfigureVisibilityListener(enabled: false);
			ConfigureVisibilityListener(enabled: true);
		}
	}

	private void RegisterVisibilityListener()
	{
		DebugUtil.Assert(!visibilityListenerRegistered);
		Singleton<KBatchedAnimUpdater>.Instance.VisibilityRegister(this);
		visibilityListenerRegistered = true;
	}

	private void UnregisterVisibilityListener()
	{
		DebugUtil.Assert(visibilityListenerRegistered);
		Singleton<KBatchedAnimUpdater>.Instance.VisibilityUnregister(this);
		visibilityListenerRegistered = false;
	}

	public void SetSceneLayer(Grid.SceneLayer layer)
	{
		float layerZ = Grid.GetLayerZ(layer);
		sceneLayer = layer;
		Vector3 position = base.transform.GetPosition();
		position.z = layerZ;
		base.transform.SetPosition(position);
		DeRegister();
		Register();
	}
}
