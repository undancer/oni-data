using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SimpleMassStatusItem")]
public class SimpleMassStatusItem : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OreMass, base.gameObject);
	}
}
