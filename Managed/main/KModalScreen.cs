using System;
using UnityEngine;
using UnityEngine.UI;

public class KModalScreen : KScreen
{
	private bool shown;

	public bool pause = true;

	private RectTransform backgroundRectTransform;

	public const float SCREEN_SORT_KEY = 100f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		backgroundRectTransform = MakeScreenModal(this);
	}

	public static RectTransform MakeScreenModal(KScreen screen)
	{
		screen.ConsumeMouseScroll = true;
		screen.activateOnSpawn = true;
		GameObject gameObject = new GameObject("background");
		gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
		gameObject.AddComponent<CanvasRenderer>();
		Image image = gameObject.AddComponent<Image>();
		image.color = new Color32(0, 0, 0, 160);
		image.raycastTarget = true;
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.SetParent(screen.transform);
		ResizeBackground(component);
		return component;
	}

	public static void ResizeBackground(RectTransform rectTransform)
	{
		rectTransform.SetAsFirstSibling();
		rectTransform.SetLocalPosition(Vector3.zero);
		rectTransform.localScale = Vector3.one;
		KCanvasScaler componentInParent = rectTransform.GetComponentInParent<KCanvasScaler>();
		float num = 1f;
		num = ((!(componentInParent != null)) ? rectTransform.lossyScale.x : componentInParent.GetCanvasScale());
		rectTransform.localScale = new Vector3(1f / num, 1f / num, 1f / num);
		rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (CameraController.Instance != null)
		{
			CameraController.Instance.DisableUserCameraControl = true;
		}
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (CameraController.Instance != null)
		{
			CameraController.Instance.DisableUserCameraControl = false;
		}
		Trigger(476357528);
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
	}

	private void OnResize()
	{
		ResizeBackground(backgroundRectTransform);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnActivate()
	{
		OnShow(show: true);
	}

	protected override void OnDeactivate()
	{
		OnShow(show: false);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (pause && SpeedControlScreen.Instance != null)
		{
			if (show && !shown)
			{
				SpeedControlScreen.Instance.Pause(playSound: false);
			}
			else if (!show && shown)
			{
				SpeedControlScreen.Instance.Unpause(playSound: false);
			}
			shown = show;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (Game.Instance != null && (e.TryConsume(Action.TogglePause) || e.TryConsume(Action.CycleSpeed)))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
		}
		if (!e.Consumed && e.TryConsume(Action.Escape))
		{
			Deactivate();
		}
		if (!e.Consumed)
		{
			KScrollRect componentInChildren = GetComponentInChildren<KScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.OnKeyDown(e);
			}
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			KScrollRect componentInChildren = GetComponentInChildren<KScrollRect>();
			if (componentInChildren != null)
			{
				componentInChildren.OnKeyUp(e);
			}
		}
		e.Consumed = true;
	}
}
