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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		TriggerLoadingMusic();
		UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(value: false);
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			File.Delete(WorldGen.SIM_SAVE_FILENAME);
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
			MusicManager.instance.StopSong("Music_TitleTheme");
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot);
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
		base.OnKeyDown(e);
	}
}
