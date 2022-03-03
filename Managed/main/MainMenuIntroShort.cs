using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MainMenuIntroShort")]
public class MainMenuIntroShort : KMonoBehaviour
{
	[SerializeField]
	private bool alwaysPlay;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		string @string = KPlayerPrefs.GetString("PlayShortOnLaunch", "");
		if (!string.IsNullOrEmpty(MainMenu.Instance.IntroShortName) && @string != MainMenu.Instance.IntroShortName)
		{
			VideoScreen component = KScreenManager.AddChild(FrontEndManager.Instance.gameObject, ScreenPrefabs.Instance.VideoScreen.gameObject).GetComponent<VideoScreen>();
			component.PlayVideo(Assets.GetVideo(MainMenu.Instance.IntroShortName), unskippable: false, AudioMixerSnapshots.Get().MainMenuVideoPlayingSnapshot);
			component.OnStop = (System.Action)Delegate.Combine(component.OnStop, (System.Action)delegate
			{
				KPlayerPrefs.SetString("PlayShortOnLaunch", MainMenu.Instance.IntroShortName);
				base.gameObject.SetActive(value: false);
			});
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
