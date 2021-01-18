using UnityEngine;
using UnityEngine.EventSystems;

public class DialogPanel : MonoBehaviour, IDeselectHandler, IEventSystemHandler
{
	public bool destroyOnDeselect = true;

	public void OnDeselect(BaseEventData eventData)
	{
		if (destroyOnDeselect)
		{
			foreach (Transform item in base.transform)
			{
				Util.KDestroyGameObject(item.gameObject);
			}
		}
		base.gameObject.SetActive(value: false);
	}
}
