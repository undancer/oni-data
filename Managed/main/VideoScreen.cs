using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : KModalScreen
{
	public static VideoScreen Instance;

	[SerializeField]
	private VideoPlayer videoPlayer;

	[SerializeField]
	private Slideshow slideshow;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton proceedButton;

	[SerializeField]
	private RectTransform overlayContainer;

	[SerializeField]
	private List<VideoOverlay> overlayPrefabs;

	private RawImage screen;

	private RenderTexture renderTexture;

	private string activeAudioSnapshot;

	[SerializeField]
	private Image fadeOverlay;

	private EventInstance audioHandle;

	private bool victoryLoopQueued = false;

	private string victoryLoopMessage = "";

	private string victoryLoopClip = "";

	private bool videoSkippable = true;

	public System.Action OnStop;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
		closeButton.onClick += delegate
		{
			Stop();
		};
		proceedButton.onClick += delegate
		{
			Stop();
		};
		videoPlayer.isLooping = false;
		videoPlayer.loopPointReached += delegate
		{
			if (victoryLoopQueued)
			{
				StartCoroutine(SwitchToVictoryLoop());
			}
			else if (!videoPlayer.isLooping)
			{
				Stop();
			}
		};
		Instance = this;
		Show(show: false);
	}

	protected override void OnShow(bool show)
	{
		base.transform.SetAsLastSibling();
		base.OnShow(show);
		screen = videoPlayer.gameObject.GetComponent<RawImage>();
	}

	public void DisableAllMedia()
	{
		overlayContainer.gameObject.SetActive(value: false);
		videoPlayer.gameObject.SetActive(value: false);
		slideshow.gameObject.SetActive(value: false);
	}

	public void PlaySlideShow(Sprite[] sprites)
	{
		Show();
		DisableAllMedia();
		slideshow.updateType = SlideshowUpdateType.preloadedSprites;
		slideshow.gameObject.SetActive(value: true);
		slideshow.SetSprites(sprites);
		slideshow.SetPaused(state: false);
	}

	public void PlaySlideShow(string[] files)
	{
		Show();
		DisableAllMedia();
		slideshow.updateType = SlideshowUpdateType.loadOnDemand;
		slideshow.gameObject.SetActive(value: true);
		slideshow.SetFiles(files, 0);
		slideshow.SetPaused(state: false);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(Action.Escape))
		{
			if (slideshow.gameObject.activeSelf && e.TryConsume(Action.Escape))
			{
				Stop();
				return;
			}
			if (e.TryConsume(Action.Escape))
			{
				if (videoSkippable)
				{
					Stop();
				}
				return;
			}
		}
		base.OnKeyDown(e);
	}

	public void PlayVideo(VideoClip clip, bool unskippable = false, string overrideAudioSnapshot = "", bool showProceedButton = false)
	{
		for (int i = 0; i < overlayContainer.childCount; i++)
		{
			UnityEngine.Object.Destroy(overlayContainer.GetChild(i).gameObject);
		}
		Show();
		videoPlayer.isLooping = false;
		activeAudioSnapshot = (string.IsNullOrEmpty(overrideAudioSnapshot) ? AudioMixerSnapshots.Get().TutorialVideoPlayingSnapshot : overrideAudioSnapshot);
		AudioMixer.instance.Start(activeAudioSnapshot);
		DisableAllMedia();
		videoPlayer.gameObject.SetActive(value: true);
		renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
		screen.texture = renderTexture;
		videoPlayer.targetTexture = renderTexture;
		videoPlayer.clip = clip;
		videoPlayer.Play();
		if (audioHandle.isValid())
		{
			KFMOD.EndOneShot(audioHandle);
			audioHandle.clearHandle();
		}
		audioHandle = KFMOD.BeginOneShot(GlobalAssets.GetSound("vid_" + clip.name), Vector3.zero);
		KFMOD.EndOneShot(audioHandle);
		videoSkippable = !unskippable;
		closeButton.gameObject.SetActive(videoSkippable);
		proceedButton.gameObject.SetActive(showProceedButton && videoSkippable);
	}

	public void QueueVictoryVideoLoop(bool queue, string message = "", string victoryAchievement = "", string loopVideo = "")
	{
		victoryLoopQueued = queue;
		victoryLoopMessage = message;
		victoryLoopClip = loopVideo;
		OnStop = (System.Action)Delegate.Combine(OnStop, (System.Action)delegate
		{
			RetireColonyUtility.SaveColonySummaryData();
			MainMenu.ActivateRetiredColoniesScreenFromData(base.transform.parent.gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
		});
	}

	public void SetOverlayText(string overlayTemplate, List<string> strings)
	{
		VideoOverlay videoOverlay = null;
		foreach (VideoOverlay overlayPrefab in overlayPrefabs)
		{
			if (overlayPrefab.name == overlayTemplate)
			{
				videoOverlay = overlayPrefab;
				break;
			}
		}
		DebugUtil.Assert(videoOverlay != null, "Could not find a template named ", overlayTemplate);
		VideoOverlay videoOverlay2 = Util.KInstantiateUI<VideoOverlay>(videoOverlay.gameObject, overlayContainer.gameObject, force_active: true);
		videoOverlay2.SetText(strings);
		overlayContainer.gameObject.SetActive(value: true);
	}

	private IEnumerator SwitchToVictoryLoop()
	{
		victoryLoopQueued = false;
		Color color = fadeOverlay.color;
		for (float j = 0f; j < 1f; j += Time.unscaledDeltaTime)
		{
			fadeOverlay.color = new Color(color.r, color.g, color.b, j);
			yield return 0;
		}
		fadeOverlay.color = new Color(color.r, color.g, color.b, 1f);
		MusicManager.instance.PlaySong("Music_Victory_03_StoryAndSummary");
		MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 1f);
		closeButton.gameObject.SetActive(value: true);
		proceedButton.gameObject.SetActive(value: true);
		SetOverlayText("VictoryEnd", new List<string>
		{
			victoryLoopMessage
		});
		videoPlayer.clip = Assets.GetVideo(victoryLoopClip);
		videoPlayer.isLooping = true;
		videoPlayer.Play();
		proceedButton.gameObject.SetActive(value: true);
		yield return new WaitForSecondsRealtime(1f);
		for (float i = 1f; i >= 0f; i -= Time.unscaledDeltaTime)
		{
			fadeOverlay.color = new Color(color.r, color.g, color.b, i);
			yield return 0;
		}
		fadeOverlay.color = new Color(color.r, color.g, color.b, 0f);
	}

	public void Stop()
	{
		videoPlayer.Stop();
		screen.texture = null;
		videoPlayer.targetTexture = null;
		AudioMixer.instance.Stop(activeAudioSnapshot);
		audioHandle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		if (OnStop != null)
		{
			OnStop();
		}
		Show(show: false);
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (audioHandle.isValid())
		{
			audioHandle.getTimelinePosition(out var position);
			double num = videoPlayer.time * 1000.0;
			if ((double)position - num > 33.0)
			{
				videoPlayer.frame++;
			}
			else if (num - (double)position > 33.0)
			{
				videoPlayer.frame--;
			}
		}
	}
}
