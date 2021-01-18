using UnityEngine;

public static class ChoreHelpers
{
	public static GameObject CreateLocator(string name, Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(ApproachableLocator.ID));
		gameObject.name = name;
		gameObject.transform.SetPosition(pos);
		gameObject.gameObject.SetActive(value: true);
		return gameObject;
	}

	public static GameObject CreateSleepLocator(Vector3 pos)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(SleepLocator.ID));
		gameObject.name = "SLeepLocator";
		gameObject.transform.SetPosition(pos);
		gameObject.gameObject.SetActive(value: true);
		return gameObject;
	}

	public static void DestroyLocator(GameObject locator)
	{
		if (locator != null)
		{
			locator.gameObject.DeleteObject();
		}
	}
}
