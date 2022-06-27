using UnityEngine;

public class URLOpenFunction : MonoBehaviour
{
	[SerializeField]
	private KButton triggerButton;

	[SerializeField]
	private string fixedURL;

	private void Start()
	{
		if (triggerButton != null)
		{
			triggerButton.ClearOnClick();
			triggerButton.onClick += delegate
			{
				OpenUrl(fixedURL);
			};
		}
	}

	public void OpenUrl(string url)
	{
		App.OpenWebURL(url);
	}
}
