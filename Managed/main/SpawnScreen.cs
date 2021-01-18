using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpawnScreen")]
public class SpawnScreen : KMonoBehaviour
{
	public GameObject Screen;

	protected override void OnPrefabInit()
	{
		Util.KInstantiateUI(Screen, base.gameObject);
	}
}
