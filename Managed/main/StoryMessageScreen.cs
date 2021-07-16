using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class StoryMessageScreen : KScreen
{
	private const float ALPHA_SPEED = 0.01f;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private KButton button;

	[SerializeField]
	[EventRef]
	private string dialogSound;

	[SerializeField]
	private LocText titleLabel;

	[SerializeField]
	private LocText bodyLabel;

	private const float expandedHeight = 300f;

	[SerializeField]
	private GameObject content;

	public bool restoreInterfaceOnClose = true;

	public System.Action OnClose;

	private bool startFade;

	public string title
	{
		set
		{
			titleLabel.SetText(value);
		}
	}

	public string body
	{
		set
		{
			bodyLabel.SetText(value);
		}
	}

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HideInterface(hide: true);
		CameraController.Instance.FadeOut(0.5f);
	}

	private IEnumerator ExpandPanel()
	{
		content.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(0.25f);
		float height = 0f;
		while (height < 299f)
		{
			height = Mathf.Lerp(dialog.rectTransform().sizeDelta.y, 300f, Time.unscaledDeltaTime * 15f);
			dialog.rectTransform().sizeDelta = new Vector2(dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		CameraController.Instance.FadeOut(0.5f);
		yield return null;
	}

	private IEnumerator CollapsePanel()
	{
		float height = 300f;
		while (height > 0f)
		{
			height = Mathf.Lerp(dialog.rectTransform().sizeDelta.y, -1f, Time.unscaledDeltaTime * 15f);
			dialog.rectTransform().sizeDelta = new Vector2(dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		content.gameObject.SetActive(value: false);
		if (OnClose != null)
		{
			OnClose();
			OnClose = null;
		}
		Deactivate();
		yield return null;
	}

	public static void HideInterface(bool hide)
	{
		SelectTool.Instance.Select(null, skipSound: true);
		NotificationScreen.Instance.Show(!hide);
		OverlayMenu.Instance.Show(!hide);
		if (PlanScreen.Instance != null)
		{
			PlanScreen.Instance.Show(!hide);
		}
		if (BuildMenu.Instance != null)
		{
			BuildMenu.Instance.Show(!hide);
		}
		ManagementMenu.Instance.Show(!hide);
		ToolMenu.Instance.Show(!hide);
		ToolMenu.Instance.PriorityScreen.Show(!hide);
		ColonyDiagnosticScreen.Instance.Show(!hide);
		PinnedResourcesPanel.Instance.Show(!hide);
		TopLeftControlScreen.Instance.Show(!hide);
		if (WorldSelector.Instance != null)
		{
			WorldSelector.Instance.Show(!hide);
		}
		DateTime.Instance.Show(!hide);
		if (BuildWatermark.Instance != null)
		{
			BuildWatermark.Instance.Show(!hide);
		}
		PopFXManager.Instance.Show(!hide);
	}

	public void Update()
	{
		if (startFade)
		{
			Color color = bg.color;
			color.a -= 0.01f;
			if (color.a <= 0f)
			{
				color.a = 0f;
			}
			bg.color = color;
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		SelectTool.Instance.Select(null);
		button.onClick += delegate
		{
			StartCoroutine(CollapsePanel());
		};
		dialog.GetComponent<KScreen>().Show(show: false);
		startFade = false;
		CameraController.Instance.DisableUserCameraControl = true;
		KFMOD.PlayUISound(dialogSound);
		dialog.GetComponent<KScreen>().Activate();
		dialog.GetComponent<KScreen>().SetShouldFadeIn(bShouldFade: true);
		dialog.GetComponent<KScreen>().Show();
		MusicManager.instance.PlaySong("Music_Victory_01_Message");
		StartCoroutine(ExpandPanel());
	}

	protected override void OnDeactivate()
	{
		IsActive();
		base.OnDeactivate();
		MusicManager.instance.StopSong("Music_Victory_01_Message");
		if (restoreInterfaceOnClose)
		{
			CameraController.Instance.DisableUserCameraControl = false;
			CameraController.Instance.FadeIn();
			HideInterface(hide: false);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			StartCoroutine(CollapsePanel());
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}
}
