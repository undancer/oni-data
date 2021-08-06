using System;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDay")]
public class TimeOfDay : KMonoBehaviour, ISaveLoadable
{
	public enum TimeRegion
	{
		Invalid,
		Day,
		Night
	}

	[Serialize]
	private float scale;

	private TimeRegion timeRegion;

	private EventInstance nightLPEvent;

	public static TimeOfDay Instance;

	private bool isEclipse;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		timeRegion = GetCurrentTimeRegion();
		UpdateSunlightIntensity();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		UpdateVisuals();
	}

	public TimeRegion GetCurrentTimeRegion()
	{
		if (GameClock.Instance.IsNighttime())
		{
			return TimeRegion.Night;
		}
		return TimeRegion.Day;
	}

	private void Update()
	{
		UpdateVisuals();
		UpdateAudio();
	}

	private void UpdateVisuals()
	{
		float num = 0.875f;
		float num2 = 0.2f;
		float num3 = 1f;
		float b = 0f;
		if (GameClock.Instance.GetCurrentCycleAsPercentage() >= num)
		{
			b = num3;
		}
		scale = Mathf.Lerp(scale, b, Time.deltaTime * num2);
		float y = UpdateSunlightIntensity();
		Shader.SetGlobalVector("_TimeOfDay", new Vector4(scale, y, 0f, 0f));
	}

	private void UpdateAudio()
	{
		TimeRegion currentTimeRegion = GetCurrentTimeRegion();
		if (currentTimeRegion != timeRegion)
		{
			TriggerSoundChange(currentTimeRegion);
			timeRegion = currentTimeRegion;
			Trigger(1791086652);
		}
	}

	public void Sim4000ms(float dt)
	{
		UpdateSunlightIntensity();
	}

	public void SetEclipse(bool eclipse)
	{
		isEclipse = eclipse;
	}

	private float UpdateSunlightIntensity()
	{
		float daytimeDurationInPercentage = GameClock.Instance.GetDaytimeDurationInPercentage();
		float num = GameClock.Instance.GetCurrentCycleAsPercentage() / daytimeDurationInPercentage;
		if (num >= 1f || isEclipse)
		{
			num = 0f;
		}
		float num2 = Mathf.Sin(num * (float)Math.PI);
		Game.Instance.currentFallbackSunlightIntensity = num2 * 80000f;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			worldContainer.currentSunlightIntensity = num2 * (float)worldContainer.sunlight;
			worldContainer.currentCosmicIntensity = worldContainer.cosmicRadiation;
		}
		return num2;
	}

	private void TriggerSoundChange(TimeRegion new_region)
	{
		switch (new_region)
		{
		case TimeRegion.Day:
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NightStartedMigrated);
			if (MusicManager.instance.SongIsPlaying("Stinger_Loop_Night"))
			{
				MusicManager.instance.StopSong("Stinger_Loop_Night");
			}
			MusicManager.instance.PlaySong("Stinger_Day");
			MusicManager.instance.PlayDynamicMusic();
			break;
		case TimeRegion.Night:
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().NightStartedMigrated);
			MusicManager.instance.PlaySong("Stinger_Loop_Night");
			break;
		}
	}

	public void SetScale(float new_scale)
	{
		scale = new_scale;
	}
}
