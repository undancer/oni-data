using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HealthyGameMessageScreen")]
public class HealthyGameMessageScreen : KMonoBehaviour
{
	public KButton confirmButton;

	public CanvasGroup canvasGroup;

	private float spawnTime;

	private float totalTime = 10f;

	private float fadeTime = 1.5f;

	private bool isFirstUpdate = true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		confirmButton.onClick += delegate
		{
			PlayIntroShort();
		};
		confirmButton.gameObject.SetActive(value: false);
	}

	private void PlayIntroShort()
	{
		string @string = KPlayerPrefs.GetString("PlayShortOnLaunch", "");
		if (!string.IsNullOrEmpty(MainMenu.Instance.IntroShortName) && @string != MainMenu.Instance.IntroShortName)
		{
			VideoScreen component = KScreenManager.AddChild(FrontEndManager.Instance.gameObject, ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
			component.PlayVideo(Assets.GetVideo(MainMenu.Instance.IntroShortName), unskippable: false, AudioMixerSnapshots.Get().MainMenuVideoPlayingSnapshot);
			component.OnStop = (System.Action)Delegate.Combine(component.OnStop, (System.Action)delegate
			{
				KPlayerPrefs.SetString("PlayShortOnLaunch", MainMenu.Instance.IntroShortName);
				if (base.gameObject != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			});
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (!DistributionPlatform.Inst.IsDLCStatusReady())
		{
			return;
		}
		if (isFirstUpdate)
		{
			isFirstUpdate = false;
			spawnTime = Time.unscaledTime;
			return;
		}
		float num = Mathf.Min(Time.unscaledDeltaTime, 71f / (678f * (float)Math.PI));
		float num2 = Time.unscaledTime - spawnTime;
		if (num2 < totalTime - fadeTime)
		{
			canvasGroup.alpha += num * (1f / fadeTime);
		}
		else if (num2 >= totalTime + 0.75f)
		{
			canvasGroup.alpha = 1f;
			confirmButton.gameObject.SetActive(value: true);
		}
		else if (num2 >= totalTime - fadeTime)
		{
			canvasGroup.alpha -= num * (1f / fadeTime);
		}
	}
}
