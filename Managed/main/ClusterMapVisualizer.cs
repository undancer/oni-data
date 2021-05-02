using System.Collections.Generic;
using UnityEngine;

public class ClusterMapVisualizer : KMonoBehaviour
{
	public KBatchedAnimController animControllerPrefab;

	public KBatchedAnimController peekControllerPrefab;

	public Transform nameTarget;

	public AlertVignette alertVignette;

	public bool doesTransitionAnimation = false;

	[HideInInspector]
	public Transform animContainer;

	private ClusterGridEntity entity;

	private ClusterMapPathDrawer pathDrawer;

	private ClusterMapPath mapPath;

	private List<KBatchedAnimController> animControllers;

	private bool isSelected = false;

	private ClusterRevealLevel lastRevealLevel = ClusterRevealLevel.Hidden;

	public void Init(ClusterGridEntity entity, ClusterMapPathDrawer pathDrawer)
	{
		this.entity = entity;
		this.pathDrawer = pathDrawer;
		animControllers = new List<KBatchedAnimController>();
		if (animContainer == null)
		{
			GameObject gameObject = new GameObject("AnimContainer", typeof(RectTransform));
			RectTransform component = GetComponent<RectTransform>();
			RectTransform component2 = gameObject.GetComponent<RectTransform>();
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (doesTransitionAnimation)
		{
			ClusterMapTravelAnimator.StatesInstance statesInstance = new ClusterMapTravelAnimator.StatesInstance(this, entity);
			statesInstance.StartSM();
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
			GetFirstAnim().SetSymbolVisiblity("selected", selected);
		}
	}

	public void PlayAnim(string animName, KAnim.PlayMode playMode)
	{
		GetFirstAnim().Play(animName, playMode);
	}

	public KBatchedAnimController GetFirstAnim()
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
					kBatchedAnimController.AnimFiles = new KAnimFile[1]
					{
						animConfig.animFile
					};
					kBatchedAnimController.initialMode = KAnim.PlayMode.Loop;
					kBatchedAnimController.initialAnim = animConfig.initialAnim;
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
			Color color = (isSelected ? ClusterMapScreen.Instance.rocketSelectedPathColor : (entity.ShowPath() ? ClusterMapScreen.Instance.rocketPathColor : new Color(0f, 0f, 0f, 0f)));
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
