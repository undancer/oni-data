using System;
using System.Collections;
using UnityEngine;

public static class EnterTemporalTearSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(Sequence());
	}

	private static IEnumerator Sequence()
	{
		CameraController.Instance.FadeOut();
		yield return new WaitForSecondsRealtime(2f);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		VideoScreen screen = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
		screen.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.ReachedDistantPlanet.shortVideoName), unskippable: true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		screen.QueueVictoryVideoLoop(queue: true, Db.Get().ColonyAchievements.ReachedDistantPlanet.messageBody, Db.Get().ColonyAchievements.ReachedDistantPlanet.Id, Db.Get().ColonyAchievements.ReachedDistantPlanet.loopVideoName);
		screen.OnStop = (System.Action)Delegate.Combine(screen.OnStop, (System.Action)delegate
		{
			StoryMessageScreen.HideInterface(hide: false);
			CameraController.Instance.FadeIn();
			CameraController.Instance.SetWorldInteractive(state: true);
			HoverTextScreen.Instance.Show();
			CameraController.Instance.SetOverrideZoomSpeed(1f);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
			RootMenu.Instance.canTogglePauseScreen = true;
		});
	}
}
