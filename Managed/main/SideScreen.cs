using UnityEngine;

public class SideScreen : KScreen
{
	[SerializeField]
	private GameObject contentBody;

	public void SetContent(SideScreenContent sideScreenContent, GameObject target)
	{
		if (sideScreenContent.transform.parent != contentBody.transform)
		{
			sideScreenContent.transform.SetParent(contentBody.transform);
		}
		sideScreenContent.SetTarget(target);
	}
}
