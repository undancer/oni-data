using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class OptionsMenuScreen : KModalButtonMenu
{
	[SerializeField]
	private GameOptionsScreen gameOptionsScreenPrefab;

	[SerializeField]
	private AudioOptionsScreen audioOptionsScreenPrefab;

	[SerializeField]
	private GraphicsOptionsScreen graphicsOptionsScreenPrefab;

	[SerializeField]
	private CreditsScreen creditsScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private MetricsOptionsScreen metricsScreenPrefab;

	[SerializeField]
	private FeedbackScreen feedbackScreenPrefab;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton backButton;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		keepMenuOpen = true;
		buttons = new List<ButtonInfo>
		{
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, Action.NumActions, OnGraphicsOptions),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.AUDIO, Action.NumActions, OnAudioOptions),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GAME, Action.NumActions, OnGameOptions),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.METRICS, Action.NumActions, OnMetrics),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.FEEDBACK, Action.NumActions, OnFeedback),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CREDITS, Action.NumActions, OnCredits)
		};
		closeButton.onClick += Deactivate;
		backButton.onClick += Deactivate;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.OPTIONS_SCREEN.TITLE);
		backButton.transform.SetAsLastSibling();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		GameObject[] array = buttonObjects;
		for (int i = 0; i < array.Length; i++)
		{
			_ = array[i];
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void OnGraphicsOptions()
	{
		ActivateChildScreen(graphicsOptionsScreenPrefab.gameObject);
	}

	private void OnAudioOptions()
	{
		ActivateChildScreen(audioOptionsScreenPrefab.gameObject);
	}

	private void OnGameOptions()
	{
		ActivateChildScreen(gameOptionsScreenPrefab.gameObject);
	}

	private void OnMetrics()
	{
		ActivateChildScreen(metricsScreenPrefab.gameObject);
	}

	private void OnFeedback()
	{
		ActivateChildScreen(feedbackScreenPrefab.gameObject);
	}

	private void OnCredits()
	{
		ActivateChildScreen(creditsScreenPrefab.gameObject);
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}
}
