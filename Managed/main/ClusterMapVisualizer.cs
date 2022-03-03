using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class ClusterMapVisualizer : KMonoBehaviour
{
	private class UpdateXPositionParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public Transform transform;

			public EventInstance ev;

			public PARAMETER_ID parameterId;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateXPositionParameter()
			: base("Starmap_Position_X")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.transform = sound.transform;
			entry.ev = sound.ev;
			entry.parameterId = sound.description.GetParameterId(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				if (!(entry.transform == null))
				{
					EventInstance ev = entry.ev;
					ev.setParameterByID(entry.parameterId, entry.transform.GetPosition().x / (float)Screen.width);
				}
			}
		}

		public override void Remove(Sound sound)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ev.handle == sound.ev.handle)
				{
					entries.RemoveAt(i);
					break;
				}
			}
		}
	}

	private class UpdateYPositionParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public Transform transform;

			public EventInstance ev;

			public PARAMETER_ID parameterId;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateYPositionParameter()
			: base("Starmap_Position_Y")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.transform = sound.transform;
			entry.ev = sound.ev;
			entry.parameterId = sound.description.GetParameterId(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				if (!(entry.transform == null))
				{
					EventInstance ev = entry.ev;
					ev.setParameterByID(entry.parameterId, entry.transform.GetPosition().y / (float)Screen.height);
				}
			}
		}

		public override void Remove(Sound sound)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ev.handle == sound.ev.handle)
				{
					entries.RemoveAt(i);
					break;
				}
			}
		}
	}

	private class UpdateZoomPercentageParameter : LoopingSoundParameterUpdater
	{
		private struct Entry
		{
			public Transform transform;

			public EventInstance ev;

			public PARAMETER_ID parameterId;
		}

		private List<Entry> entries = new List<Entry>();

		public UpdateZoomPercentageParameter()
			: base("Starmap_Zoom_Percentage")
		{
		}

		public override void Add(Sound sound)
		{
			Entry entry = default(Entry);
			entry.ev = sound.ev;
			entry.parameterId = sound.description.GetParameterId(base.parameter);
			Entry item = entry;
			entries.Add(item);
		}

		public override void Update(float dt)
		{
			foreach (Entry entry in entries)
			{
				EventInstance ev = entry.ev;
				ev.setParameterByID(entry.parameterId, ClusterMapScreen.Instance.CurrentZoomPercentage());
			}
		}

		public override void Remove(Sound sound)
		{
			for (int i = 0; i < entries.Count; i++)
			{
				if (entries[i].ev.handle == sound.ev.handle)
				{
					entries.RemoveAt(i);
					break;
				}
			}
		}
	}

	public KBatchedAnimController animControllerPrefab;

	public KBatchedAnimController peekControllerPrefab;

	public Transform nameTarget;

	public AlertVignette alertVignette;

	public bool doesTransitionAnimation;

	[HideInInspector]
	public Transform animContainer;

	private ClusterGridEntity entity;

	private ClusterMapPathDrawer pathDrawer;

	private ClusterMapPath mapPath;

	private List<KBatchedAnimController> animControllers;

	private bool isSelected;

	private ClusterRevealLevel lastRevealLevel;

	public void Init(ClusterGridEntity entity, ClusterMapPathDrawer pathDrawer)
	{
		this.entity = entity;
		this.pathDrawer = pathDrawer;
		animControllers = new List<KBatchedAnimController>();
		if (animContainer == null)
		{
			GameObject obj = new GameObject("AnimContainer", typeof(RectTransform));
			RectTransform component = GetComponent<RectTransform>();
			RectTransform component2 = obj.GetComponent<RectTransform>();
			component2.SetParent(component, worldPositionStays: false);
			component2.SetLocalPosition(new Vector3(0f, 0f, 0f));
			component2.sizeDelta = component.sizeDelta;
			component2.localScale = Vector3.one;
			animContainer = component2;
		}
		Vector3 position = ClusterGrid.Instance.GetPosition(entity);
		this.rectTransform().SetLocalPosition(position);
		RefreshPathDrawing();
		entity.Subscribe(543433792, OnClusterDestinationChanged);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (doesTransitionAnimation)
		{
			new ClusterMapTravelAnimator.StatesInstance(this, entity).StartSM();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (entity != null)
		{
			if (entity is Clustercraft)
			{
				new ClusterMapRocketAnimator.StatesInstance(this, entity).StartSM();
			}
			else if (entity is BallisticClusterGridEntity)
			{
				new ClusterMapBallisticAnimator.StatesInstance(this, entity).StartSM();
			}
			else if (entity.Layer == EntityLayer.FX)
			{
				new ClusterMapFXAnimator.StatesInstance(this, entity).StartSM();
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (entity != null)
		{
			entity.Unsubscribe(543433792, OnClusterDestinationChanged);
		}
		base.OnCleanUp();
	}

	private void OnClusterDestinationChanged(object data)
	{
		RefreshPathDrawing();
	}

	public void Select(bool selected)
	{
		if (animControllers != null && animControllers.Count != 0)
		{
			if (!selected == isSelected)
			{
				isSelected = selected;
				RefreshPathDrawing();
			}
			GetFirstAnimController().SetSymbolVisiblity("selected", selected);
		}
	}

	public void PlayAnim(string animName, KAnim.PlayMode playMode)
	{
		GetFirstAnimController().Play(animName, playMode);
	}

	public KBatchedAnimController GetFirstAnimController()
	{
		return animControllers[0];
	}

	public void Show(ClusterRevealLevel level)
	{
		if (!entity.IsVisible)
		{
			level = ClusterRevealLevel.Hidden;
		}
		if (level == lastRevealLevel)
		{
			return;
		}
		lastRevealLevel = level;
		switch (level)
		{
		case ClusterRevealLevel.Hidden:
			base.gameObject.SetActive(value: false);
			break;
		case ClusterRevealLevel.Peeked:
		{
			ClearAnimControllers();
			KBatchedAnimController kBatchedAnimController2 = Object.Instantiate(peekControllerPrefab, animContainer);
			kBatchedAnimController2.gameObject.SetActive(value: true);
			animControllers.Add(kBatchedAnimController2);
			base.gameObject.SetActive(value: true);
			break;
		}
		case ClusterRevealLevel.Visible:
			ClearAnimControllers();
			if (animControllerPrefab != null && entity.AnimConfigs != null)
			{
				foreach (ClusterGridEntity.AnimConfig animConfig in entity.AnimConfigs)
				{
					KBatchedAnimController kBatchedAnimController = Object.Instantiate(animControllerPrefab, animContainer);
					kBatchedAnimController.AnimFiles = new KAnimFile[1] { animConfig.animFile };
					kBatchedAnimController.initialMode = animConfig.playMode;
					kBatchedAnimController.initialAnim = animConfig.initialAnim;
					kBatchedAnimController.Offset = animConfig.animOffset;
					kBatchedAnimController.gameObject.AddComponent<LoopingSounds>();
					if (!string.IsNullOrEmpty(animConfig.symbolSwapTarget) && !string.IsNullOrEmpty(animConfig.symbolSwapSymbol))
					{
						kBatchedAnimController.GetComponent<SymbolOverrideController>().AddSymbolOverride(source_symbol: kBatchedAnimController.AnimFiles[0].GetData().build.GetSymbol(animConfig.symbolSwapSymbol), target_symbol: animConfig.symbolSwapTarget);
					}
					kBatchedAnimController.gameObject.SetActive(value: true);
					animControllers.Add(kBatchedAnimController);
				}
			}
			base.gameObject.SetActive(value: true);
			break;
		}
	}

	public void RefreshPathDrawing()
	{
		if (entity == null)
		{
			return;
		}
		ClusterTraveler component = entity.GetComponent<ClusterTraveler>();
		if (component == null)
		{
			return;
		}
		List<AxialI> list = ((entity.IsVisible && component.IsTraveling()) ? component.CurrentPath : null);
		if (list != null && list.Count > 0)
		{
			if (mapPath == null)
			{
				mapPath = pathDrawer.AddPath();
			}
			mapPath.SetPoints(ClusterMapPathDrawer.GetDrawPathList(base.transform.GetLocalPosition(), list));
			Color color = (isSelected ? ClusterMapScreen.Instance.rocketSelectedPathColor : ((!entity.ShowPath()) ? new Color(0f, 0f, 0f, 0f) : ClusterMapScreen.Instance.rocketPathColor));
			mapPath.SetColor(color);
		}
		else if (mapPath != null)
		{
			Util.KDestroyGameObject(mapPath);
			mapPath = null;
		}
	}

	public void SetAnimRotation(float rotation)
	{
		animContainer.localRotation = Quaternion.Euler(0f, 0f, rotation);
	}

	public float GetPathAngle()
	{
		if (mapPath == null)
		{
			return 0f;
		}
		return mapPath.GetRotationForNextSegment();
	}

	private void ClearAnimControllers()
	{
		if (animControllers == null)
		{
			return;
		}
		foreach (KBatchedAnimController animController in animControllers)
		{
			Util.KDestroyGameObject(animController.gameObject);
		}
		animControllers.Clear();
	}
}
