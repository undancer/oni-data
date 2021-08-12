using System;
using System.Collections;
using UnityEngine;

public static class ArtifactSequence
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
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.CollectedArtifacts.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS");
		Vector3 cameraBiasUp = Vector3.up * 5f;
		GameObject cameraTaget = null;
		foreach (Telepad telepad in Components.Telepads)
		{
			if (telepad != null)
			{
				cameraTaget = telepad.gameObject;
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
		foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
		{
			if (liveMinionIdentity != null)
			{
				liveMinionIdentity.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
				new EmoteChore(liveMinionIdentity.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
			}
		}
		yield return new WaitForSecondsRealtime(0.5f);
		yield return new WaitForSecondsRealtime(3f);
		cameraTaget = null;
		foreach (ArtifactAnalysisStationWorkable artifactAnalysisStation in Components.ArtifactAnalysisStations)
		{
			cameraTaget = artifactAnalysisStation.gameObject;
		}
		if (cameraTaget != null)
		{
			CameraController.Instance.FadeOut(1f, 2f);
			yield return new WaitForSecondsRealtime(1f);
			CameraController.Instance.SetTargetPos(cameraTaget.transform.position + cameraBiasUp, 10f, playSound: false);
			CameraController.Instance.SetOverrideZoomSpeed(10f);
			yield return new WaitForSecondsRealtime(0.4f);
			CameraController.Instance.SetOverrideZoomSpeed(0.1f);
			CameraController.Instance.SetTargetPos(cameraTaget.transform.position + cameraBiasUp, 20f, playSound: false);
			CameraController.Instance.FadeIn(0f, 2f);
			foreach (MinionIdentity liveMinionIdentity2 in Components.LiveMinionIdentities)
			{
				if (liveMinionIdentity2 != null)
				{
					liveMinionIdentity2.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
					new EmoteChore(liveMinionIdentity2.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
				}
			}
			yield return new WaitForSecondsRealtime(0.5f);
			yield return new WaitForSecondsRealtime(3f);
		}
		cameraTaget = null;
		foreach (SpaceArtifact spaceArtifact in Components.SpaceArtifacts)
		{
			if (spaceArtifact != null && spaceArtifact.HasTag(GameTags.CharmedArtifact))
			{
				cameraTaget = spaceArtifact.gameObject;
			}
		}
		CameraController.Instance.FadeOut(1f, 2f);
		yield return new WaitForSecondsRealtime(1f);
		CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 15f, playSound: false);
		CameraController.Instance.SetOverrideZoomSpeed(10f);
		yield return new WaitForSecondsRealtime(0.4f);
		CameraController.Instance.FadeIn(0f, 2f);
		foreach (MinionIdentity liveMinionIdentity3 in Components.LiveMinionIdentities)
		{
			if (liveMinionIdentity3 != null)
			{
				liveMinionIdentity3.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
				new EmoteChore(liveMinionIdentity3.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[6] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
			}
		}
		yield return new WaitForSecondsRealtime(0.5f);
		CameraController.Instance.SetOverrideZoomSpeed(0.075f);
		CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 25f, playSound: false);
		yield return new WaitForSecondsRealtime(5f);
		CameraController.Instance.FadeOut();
		MusicManager.instance.StopSong("Music_Victory_02_NIS");
		AudioMixer.instance.Stop(Db.Get().ColonyAchievements.CollectedArtifacts.victoryNISSnapshot);
		yield return new WaitForSecondsRealtime(2f);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(playSound: false);
		}
		VideoScreen component = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
		component.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.CollectedArtifacts.shortVideoName), unskippable: true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		component.QueueVictoryVideoLoop(queue: true, Db.Get().ColonyAchievements.CollectedArtifacts.messageBody, Db.Get().ColonyAchievements.CollectedArtifacts.Id, Db.Get().ColonyAchievements.CollectedArtifacts.loopVideoName);
		component.OnStop = (System.Action)Delegate.Combine(component.OnStop, (System.Action)delegate
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
