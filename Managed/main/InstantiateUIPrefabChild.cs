using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/InstantiateUIPrefabChild")]
public class InstantiateUIPrefabChild : KMonoBehaviour
{
	public GameObject[] prefabs;

	public bool InstantiateOnAwake = true;

	private bool alreadyInstantiated;

	public bool setAsFirstSibling;

	protected override void OnPrefabInit()
	{
		if (InstantiateOnAwake)
		{
			Instantiate();
		}
	}

	public void Instantiate()
	{
		if (alreadyInstantiated)
		{
			Debug.LogWarning(base.gameObject.name + "trying to instantiate UI prefabs multiple times.");
			return;
		}
		alreadyInstantiated = true;
		GameObject[] array = prefabs;
		foreach (GameObject gameObject in array)
		{
			if (!(gameObject == null))
			{
				Vector3 vector = gameObject.rectTransform().anchoredPosition;
				GameObject gameObject2 = Object.Instantiate(gameObject);
				gameObject2.transform.SetParent(base.transform);
				gameObject2.rectTransform().anchoredPosition = vector;
				gameObject2.rectTransform().localScale = Vector3.one;
				if (setAsFirstSibling)
				{
					gameObject2.transform.SetAsFirstSibling();
				}
			}
		}
	}
}
