using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Fabricator")]
public class Fabricator : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}
}
