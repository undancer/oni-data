using UnityEngine;

public class URLOpenFunction : MonoBehaviour
{
	public void OpenUrl(string url)
	{
		App.OpenWebURL(url);
	}
}
