using System;
using System.IO;
using ProcGenGame;
using UnityEngine;

public class WorldGenScreen : NewGameFlowScreen
{
	[MyCmpReq]
	private OfflineWorldGen offlineWorldGen;

	public static WorldGenScreen Instance;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (MainMenu.Instance != null)
		{
			MainMenu.Instance.StopAmbience();
		}
		TriggerLoadingMusic();
		UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(value: false);
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			for (int i = 0; File.Exists(WorldGen.GetSIMSaveFilename(i)); i++)
			{
				File.Delete(WorldGen.GetSIMSaveFilename(i));
			}
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs(ex.ToString());
		}
		offlineWorldGen.Generate();
	}

	private void TriggerLoadingMusic()
	{
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MainMenu.Instance.StopMainMenuMusic();
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
			MusicManager.instance.PlaySong("Music_FrontEnd");
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			e.TryConsume(Action.Escape);
		}
		if (!e.Consumed)
		{
			e.TryConsume(Action.MouseRight);
		}
		base.OnKeyDown(e);
	}
}
