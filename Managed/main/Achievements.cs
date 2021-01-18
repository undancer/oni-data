using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Achievements")]
public class Achievements : KMonoBehaviour
{
	public void Unlock(string id)
	{
		if ((bool)SteamAchievementService.Instance)
		{
			SteamAchievementService.Instance.Unlock(id);
		}
	}
}
