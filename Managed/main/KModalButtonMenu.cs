using System;
using UnityEngine;

public class KModalButtonMenu : KButtonMenu
{
	private bool shown;

	[SerializeField]
	private GameObject panelRoot;

	private GameObject childDialog;

	private RectTransform modalBackground;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		modalBackground = KModalScreen.MakeScreenModal(this);
	}

	protected override void OnCmpEnable()
	{
		KModalScreen.ResizeBackground(modalBackground);
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Combine(instance.OnResize, new System.Action(OnResize));
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (childDialog == null)
		{
			Trigger(476357528);
		}
		ScreenResize instance = ScreenResize.Instance;
		instance.OnResize = (System.Action)Delegate.Remove(instance.OnResize, new System.Action(OnResize));
	}

	private void OnResize()
	{
		KModalScreen.ResizeBackground(modalBackground);
	}

	public override bool IsModal()
	{
		return true;
	}

	public override float GetSortKey()
	{
		return 100f;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (SpeedControlScreen.Instance != null)
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
		if (CameraController.Instance != null)
		{
			CameraController.Instance.DisableUserCameraControl = show;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		base.OnKeyUp(e);
		e.Consumed = true;
	}

	public void SetBackgroundActive(bool active)
	{
	}

	protected GameObject ActivateChildScreen(GameObject screenPrefab)
	{
		GameObject result = (childDialog = Util.KInstantiateUI(screenPrefab, base.transform.parent.gameObject));
		result.Subscribe(476357528, Unhide);
		Hide();
		return result;
	}

	private void Hide()
	{
		panelRoot.rectTransform().localScale = Vector3.zero;
	}

	private void Unhide(object data = null)
	{
		panelRoot.rectTransform().localScale = Vector3.one;
		childDialog.Unsubscribe(476357528, Unhide);
		childDialog = null;
	}
}
