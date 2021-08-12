using UnityEngine;

public class CellSelectionInstantiator : MonoBehaviour
{
	public GameObject CellSelectionPrefab;

	private void Awake()
	{
		GameObject gameObject = Util.KInstantiate(CellSelectionPrefab, null, "WorldSelectionCollider");
		GameObject obj = Util.KInstantiate(CellSelectionPrefab, null, "WorldSelectionCollider");
		CellSelectionObject component = gameObject.GetComponent<CellSelectionObject>();
		(component.alternateSelectionObject = obj.GetComponent<CellSelectionObject>()).alternateSelectionObject = component;
	}
}
