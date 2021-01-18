using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MoveTarget")]
public class MoveTarget : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.gameObject.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
	}
}
