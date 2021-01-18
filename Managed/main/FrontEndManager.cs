using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FrontEndManager")]
public class FrontEndManager : KMonoBehaviour
{
	public static FrontEndManager Instance;

	public static bool firstInit = true;

	public GameObject[] SpawnOnLoadScreens;

	public GameObject[] SpawnOnLaunchScreens;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
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
		if (SpawnOnLaunchScreens == null || SpawnOnLoadScreens.Length == 0)
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
