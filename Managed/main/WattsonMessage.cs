using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WattsonMessage : KScreen
{
	public class Tuning : TuningData<Tuning>
	{
		public float initialOrthographicSize;
	}

	private const float STARTTIME = 0.1f;

	private const float ENDTIME = 6.6f;

	private const float ALPHA_SPEED = 0.01f;

	private const float expandedHeight = 300f;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private LocText message;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private KButton button;

	[SerializeField]
	[EventRef]
	private string dialogSound;

	private List<KScreen> hideScreensWhileActive = new List<KScreen>();

	private bool startFade;

	private List<SchedulerHandle> scheduleHandles = new List<SchedulerHandle>();

	private static readonly HashedString[] WorkLoopAnims = new HashedString[2]
	{
		"working_pre",
		"working_loop"
	};

	private int birthsComplete;

	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game.Instance.Subscribe(-122303817, OnNewBaseCreated);
		if (DlcManager.IsExpansion1Active())
		{
			message.SetText(UI.WELCOMEMESSAGEBODY_SPACEDOUT);
		}
		else
		{
			message.SetText(UI.WELCOMEMESSAGEBODY);
		}
	}

	private IEnumerator ExpandPanel()
	{
		yield return new WaitForSecondsRealtime(0.2f);
		float height = 0f;
		while (height < 299f)
		{
			height = Mathf.Lerp(dialog.rectTransform().sizeDelta.y, 300f, Time.unscaledDeltaTime * 15f);
			dialog.rectTransform().sizeDelta = new Vector2(dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		yield return null;
	}

	private IEnumerator CollapsePanel()
	{
		float height = 300f;
		while (height > 1f)
		{
			height = Mathf.Lerp(dialog.rectTransform().sizeDelta.y, 0f, Time.unscaledDeltaTime * 15f);
			dialog.rectTransform().sizeDelta = new Vector2(dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		Deactivate();
		yield return null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		hideScreensWhileActive.Add(NotificationScreen.Instance);
		hideScreensWhileActive.Add(OverlayMenu.Instance);
		if (PlanScreen.Instance != null)
		{
			hideScreensWhileActive.Add(PlanScreen.Instance);
		}
		if (BuildMenu.Instance != null)
		{
			hideScreensWhileActive.Add(BuildMenu.Instance);
		}
		hideScreensWhileActive.Add(ManagementMenu.Instance);
		hideScreensWhileActive.Add(ToolMenu.Instance);
		hideScreensWhileActive.Add(ToolMenu.Instance.PriorityScreen);
		hideScreensWhileActive.Add(PinnedResourcesPanel.Instance);
		hideScreensWhileActive.Add(TopLeftControlScreen.Instance);
		hideScreensWhileActive.Add(DateTime.Instance);
		hideScreensWhileActive.Add(BuildWatermark.Instance);
		hideScreensWhileActive.Add(BuildWatermark.Instance);
		hideScreensWhileActive.Add(ColonyDiagnosticScreen.Instance);
		if (WorldSelector.Instance != null)
		{
			hideScreensWhileActive.Add(WorldSelector.Instance);
		}
		foreach (KScreen item in hideScreensWhileActive)
		{
			item.Show(show: false);
		}
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
		Debug.Log("WattsonMessage OnActivate");
		base.OnActivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().IntroNIS);
		AudioMixer.instance.activeNIS = true;
		button.onClick += delegate
		{
			StartCoroutine(CollapsePanel());
		};
		dialog.GetComponent<KScreen>().Show(show: false);
		startFade = false;
		GameObject telepad = GameUtil.GetTelepad(0);
		if (telepad != null)
		{
			KAnimControllerBase kac = telepad.GetComponent<KAnimControllerBase>();
			kac.Play(WorkLoopAnims, KAnim.PlayMode.Loop);
			for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
			{
				int idx = i + 1;
				MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
				minionIdentity.gameObject.transform.SetPosition(new Vector3(telepad.transform.GetPosition().x + (float)idx - 1.5f, telepad.transform.GetPosition().y, minionIdentity.gameObject.transform.GetPosition().z));
				GameObject gameObject = minionIdentity.gameObject;
				ChoreProvider chore_provider = gameObject.GetComponent<ChoreProvider>();
				EmoteChore chorePre = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[1]
				{
					"portalbirth_pre_" + idx
				}, KAnim.PlayMode.Loop);
				UIScheduler.Instance.Schedule("DupeBirth", (float)idx * 0.5f, delegate
				{
					chorePre.Cancel("Done looping");
					EmoteChore emoteChore = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[1]
					{
						"portalbirth_" + idx
					}, null);
					emoteChore.onComplete = (Action<Chore>)Delegate.Combine(emoteChore.onComplete, (Action<Chore>)delegate
					{
						birthsComplete++;
						if (birthsComplete == Components.LiveMinionIdentities.Count - 1 && IsActive())
						{
							PauseAndShowMessage();
						}
					});
				});
			}
			UIScheduler.Instance.Schedule("Welcome", 6.6f, delegate
			{
				kac.Play(new HashedString[2]
				{
					"working_pst",
					"idle"
				});
			});
			CameraController.Instance.DisableUserCameraControl = true;
		}
		else
		{
			Debug.LogWarning("Failed to spawn telepad - does the starting base template lack a 'Headquarters' ?");
			PauseAndShowMessage();
		}
		scheduleHandles.Add(UIScheduler.Instance.Schedule("GoHome", 0.1f, delegate
		{
			CameraController.Instance.SetOrthographicsSize(TuningData<Tuning>.Get().initialOrthographicSize);
			CameraController.Instance.CameraGoHome(0.5f);
			startFade = true;
			MusicManager.instance.PlaySong("Music_WattsonMessage");
		}));
	}

	protected void PauseAndShowMessage()
	{
		SpeedControlScreen.Instance.Pause(playSound: false);
		StartCoroutine(ExpandPanel());
		KFMOD.PlayUISound(dialogSound);
		dialog.GetComponent<KScreen>().Activate();
		dialog.GetComponent<KScreen>().SetShouldFadeIn(bShouldFade: true);
		dialog.GetComponent<KScreen>().Show();
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().IntroNIS);
		AudioMixer.instance.StartPersistentSnapshots();
		MusicManager.instance.StopSong("Music_WattsonMessage");
		MusicManager.instance.PlayDynamicMusic();
		AudioMixer.instance.activeNIS = false;
		DemoTimer.Instance.CountdownActive = true;
		SpeedControlScreen.Instance.Unpause(playSound: false);
		CameraController.Instance.DisableUserCameraControl = false;
		foreach (SchedulerHandle scheduleHandle in scheduleHandles)
		{
			scheduleHandle.ClearScheduler();
		}
		UIScheduler.Instance.Schedule("fadeInUI", 0.5f, delegate
		{
			foreach (KScreen item in hideScreensWhileActive)
			{
				if (!(item == null))
				{
					item.SetShouldFadeIn(bShouldFade: true);
					item.Show();
				}
			}
			CameraController.Instance.SetMaxOrthographicSize(20f);
			Game.Instance.StartDelayedInitialSave();
			UIScheduler.Instance.Schedule("InitialScreenshot", 1f, delegate
			{
				Game.Instance.timelapser.InitialScreenshot();
			});
			GameScheduler.Instance.Schedule("BasicTutorial", 1.5f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Basics);
			});
			GameScheduler.Instance.Schedule("WelcomeTutorial", 2f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Welcome);
			});
			GameScheduler.Instance.Schedule("DiggingTutorial", 420f, delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Digging);
			});
		});
		Game.Instance.SetGameStarted();
		if (TopLeftControlScreen.Instance != null)
		{
			TopLeftControlScreen.Instance.RefreshName();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape))
		{
			CameraController.Instance.CameraGoHome();
			Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	private void OnNewBaseCreated(object data)
	{
		base.gameObject.SetActive(value: true);
	}
}
