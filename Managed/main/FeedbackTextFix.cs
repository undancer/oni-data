using Steamworks;
using UnityEngine;

public class FeedbackTextFix : MonoBehaviour
{
	public string newKey;

	public LocText locText;

	private void Awake()
	{
		if (!DistributionPlatform.Initialized || !SteamUtils.IsSteamRunningOnSteamDeck())
		{
			Object.DestroyImmediate(this);
		}
		else
		{
			locText.key = newKey;
		}
	}
}
