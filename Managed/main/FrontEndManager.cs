using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FrontEndManager")]
public class FrontEndManager : KMonoBehaviour
{
	public static FrontEndManager Instance;

	public static bool firstInit = true;

	public GameObject mainMenuVanilla;

	public GameObject mainMenuExpansion1;

	public GameObject[] SpawnOnLoadScreens;

	public GameObject[] SpawnOnLaunchScreens;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		string highestActiveDlcId = DlcManager.GetHighestActiveDlcId();
		if (highestActiveDlcId == null || (highestActiveDlcId != null && highestActiveDlcId.Length == 0) || !(highestActiveDlcId == "EXPANSION1_ID"))
		{
			Util.KInstantiateUI(mainMenuVanilla, base.gameObject, force_active: true);
		}
		else
		{
			Util.KInstantiateUI(mainMenuExpansion1, base.gameObject, force_active: true);
		}
		GameObject[] spawnOnLoadScreens;
		if (SpawnOnLoadScreens != null && SpawnOnLoadScreens.Length != 0)
		{
			spawnOnLoadScreens = SpawnOnLoadScreens;
			foreach (GameObject gameObject in spawnOnLoadScreens)
			{
				if (gameObject != null)
				{
					Util.KInstantiateUI(gameObject, base.gameObject, force_active: true);
				}
			}
		}
		if (!firstInit)
		{
			return;
		}
		firstInit = false;
		if (SpawnOnLaunchScreens == null || SpawnOnLaunchScreens.Length == 0)
		{
			return;
		}
		spawnOnLoadScreens = SpawnOnLaunchScreens;
		foreach (GameObject gameObject2 in spawnOnLoadScreens)
		{
			if (gameObject2 != null)
			{
				Util.KInstantiateUI(gameObject2, base.gameObject, force_active: true);
			}
		}
	}

	private void LateUpdate()
	{
		if (Debug.developerConsoleVisible)
		{
			Debug.developerConsoleVisible = false;
		}
		KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(0, 0), new Vector2I(9999, 9999));
		KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
		KAnimBatchManager.Instance().Render();
	}
}
