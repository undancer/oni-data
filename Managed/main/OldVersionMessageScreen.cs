using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class OldVersionMessageScreen : KModalScreen
{
	public KButton forumButton;

	public KButton confirmButton;

	public KButton quitButton;

	public LocText bodyText;

	public bool previewInEditor;

	public RectTransform messageContainer;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		forumButton.onClick += delegate
		{
			App.OpenWebURL("https://forums.kleientertainment.com/forums/topic/140474-previous-update-steam-branch-access/");
		};
		confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(value: false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
		};
		quitButton.onClick += delegate
		{
			App.Quit();
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		messageContainer.sizeDelta = new Vector2(Mathf.Max(384f, (float)Screen.width * 0.25f), messageContainer.sizeDelta.y);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
	}
}
