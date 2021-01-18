using System;
using System.Collections;
using UnityEngine;

public static class ThrivingSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(Sequence());
	}

	private static IEnumerator Sequence()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		CameraController.Instance.SetWorldInteractive(state: false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.Thriving.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS");
		Vector3 cameraBiasUp = Vector3.up * 5f;
		GameObject cameraTaget = null;
		foreach (Telepad pad in Components.Telepads)
		{
			if (pad != null)
			{
				cameraTaget = pad.gameObject;
			}
		}
		CameraController.Instance.FadeOut(1f, 2f);
		yield return new WaitForSecondsRealtime(1f);
		CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 10f, playSound: false);
		CameraController.Instance.SetOverrideZoomSpeed(10f);
		yield return new WaitForSecondsRealtime(0.4f);
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(playSound: false);
		}
		SpeedControlScreen.Instance.SetSpeed(1);
		CameraController.Instance.SetOverrideZoomSpeed(0.05f);
		CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 20f, playSound: false);
		CameraController.Instance.FadeIn(0f, 2f);
		foreach (MinionIdentity minion in Components.LiveMinionIdentities)
		{
			if (minion != null)
			{
				minion.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
				new EmoteChore(minion.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6]
				{
					"cheer_pre",
					"cheer_loop",
					"cheer_pst",
					"cheer_pre",
					"cheer_loop",
					"cheer_pst"
				}, null);
			}
		}
		yield return new WaitForSecondsRealtime(0.5f);
		yield return new WaitForSecondsRealtime(3f);
		GameObject cameraTaget2 = null;
		foreach (ComplexFabricator pad2 in Components.ComplexFabricators)
		{
			if (pad2 != null)
			{
				cameraTaget2 = pad2.gameObject;
			}
		}
		if (cameraTaget2 == null)
		{
			foreach (Generator pad3 in Components.Generators)
			{
				if (pad3 != null)
				{
					cameraTaget2 = pad3.gameObject;
				}
			}
		}
		if (cameraTaget2 == null)
		{
			foreach (Fabricator pad4 in Components.Fabricators)
			{
				if (pad4 != null)
				{
					cameraTaget2 = pad4.gameObject;
				}
			}
		}
		if (cameraTaget2 != null)
		{
			CameraController.Instance.FadeOut(1f, 2f);
			yield return new WaitForSecondsRealtime(1f);
			CameraController.Instance.SetTargetPos(cameraTaget2.transform.position + cameraBiasUp, 10f, playSound: false);
			CameraController.Instance.SetOverrideZoomSpeed(10f);
			yield return new WaitForSecondsRealtime(0.4f);
			CameraController.Instance.SetOverrideZoomSpeed(0.1f);
			CameraController.Instance.SetTargetPos(cameraTaget2.transform.position + cameraBiasUp, 20f, playSound: false);
			CameraController.Instance.FadeIn(0f, 2f);
			foreach (MinionIdentity minion3 in Components.LiveMinionIdentities)
			{
				if (minion3 != null)
				{
					minion3.GetComponent<Facing>().Face(cameraTaget2.transform.position.x);
					new EmoteChore(minion3.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6]
					{
						"cheer_pre",
						"cheer_loop",
						"cheer_pst",
						"cheer_pre",
						"cheer_loop",
						"cheer_pst"
					}, null);
				}
			}
			yield return new WaitForSecondsRealtime(0.5f);
			yield return new WaitForSecondsRealtime(3f);
		}
		GameObject cameraTaget3 = null;
		foreach (MonumentPart part in Components.MonumentParts)
		{
			if (part.IsMonumentCompleted())
			{
				cameraTaget3 = part.gameObject;
			}
		}
		CameraController.Instance.FadeOut(1f, 2f);
		yield return new WaitForSecondsRealtime(1f);
		CameraController.Instance.SetTargetPos(cameraTaget3.transform.position, 15f, playSound: false);
		CameraController.Instance.SetOverrideZoomSpeed(10f);
		yield return new WaitForSecondsRealtime(0.4f);
		CameraController.Instance.FadeIn(0f, 2f);
		foreach (MinionIdentity minion2 in Components.LiveMinionIdentities)
		{
			if (minion2 != null)
			{
				minion2.GetComponent<Facing>().Face(cameraTaget3.transform.position.x);
				new EmoteChore(minion2.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6]
				{
					"cheer_pre",
					"cheer_loop",
					"cheer_pst",
					"cheer_pre",
					"cheer_loop",
					"cheer_pst"
				}, null);
			}
		}
		yield return new WaitForSecondsRealtime(0.5f);
		CameraController.Instance.SetOverrideZoomSpeed(0.075f);
		CameraController.Instance.SetTargetPos(cameraTaget3.transform.position, 25f, playSound: false);
		yield return new WaitForSecondsRealtime(5f);
		CameraController.Instance.FadeOut();
		MusicManager.instance.StopSong("Music_Victory_02_NIS");
		AudioMixer.instance.Stop(Db.Get().ColonyAchievements.Thriving.victoryNISSnapshot);
		yield return new WaitForSecondsRealtime(2f);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		VideoScreen screen = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
		screen.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.Thriving.shortVideoName), unskippable: true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		screen.QueueVictoryVideoLoop(queue: true, Db.Get().ColonyAchievements.Thriving.messageBody, Db.Get().ColonyAchievements.Thriving.Id, Db.Get().ColonyAchievements.Thriving.loopVideoName);
		screen.OnStop = (System.Action)Delegate.Combine(screen.OnStop, (System.Action)delegate
		{
			StoryMessageScreen.HideInterface(hide: false);
			CameraController.Instance.FadeIn();
			CameraController.Instance.SetWorldInteractive(state: true);
			CameraController.Instance.SetOverrideZoomSpeed(1f);
			HoverTextScreen.Instance.Show();
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
			RootMenu.Instance.canTogglePauseScreen = true;
		});
	}
}
