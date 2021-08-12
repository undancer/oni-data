using System.Collections.Generic;
using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
	public const int MAXDEPTH = -30000;

	public const int SCREENDEPTH = -1000;

	public GameObject prefab_NewSaveGame;

	public List<GameObject> preloadPrefabs = new List<GameObject>();

	public List<GameObject> prefabs = new List<GameObject>();

	public static SceneInitializer Instance { get; private set; }

	private void Awake()
	{
		Localization.SwapToLocalizedFont();
		Instance = this;
		PreLoadPrefabs();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private void PreLoadPrefabs()
	{
		foreach (GameObject preloadPrefab in preloadPrefabs)
		{
			if (preloadPrefab != null)
			{
				Util.KInstantiate(preloadPrefab, preloadPrefab.transform.GetPosition(), Quaternion.identity, base.gameObject);
			}
		}
	}

	public void NewSaveGamePrefab()
	{
		if (prefab_NewSaveGame != null && SaveGame.Instance == null)
		{
			Util.KInstantiate(prefab_NewSaveGame, base.gameObject);
		}
	}

	public void PostLoadPrefabs()
	{
		foreach (GameObject prefab in prefabs)
		{
			if (prefab != null)
			{
				Util.KInstantiate(prefab, base.gameObject);
			}
		}
	}
}
