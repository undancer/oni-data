using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class SplashMessageScreen : KMonoBehaviour
{
	public KButton forumButton;

	public KButton confirmButton;

	public LocText bodyText;

	public bool previewInEditor = false;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		forumButton.onClick += delegate
		{
			Application.OpenURL("https://forums.kleientertainment.com/forums/forum/118-oxygen-not-included/");
		};
		confirmButton.onClick += delegate
		{
			base.gameObject.SetActive(value: false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
		};
		bodyText.text = UI.DEVELOPMENTBUILDS.ALPHA.LOADING.BODY;
	}

	private void OnEnable()
	{
		LayoutElement component = confirmButton.GetComponent<LayoutElement>();
		LocText componentInChildren = confirmButton.GetComponentInChildren<LocText>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
	}
}
