using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/KUISelectable")]
public class KUISelectable : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private GameObject target;

	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	public void SetTarget(GameObject target)
	{
		this.target = target;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (target != null)
		{
			SelectTool.Instance.SetHoverOverride(target.GetComponent<KSelectable>());
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SelectTool.Instance.SetHoverOverride(null);
	}

	private void OnClick()
	{
		if (target != null)
		{
			SelectTool.Instance.Select(target.GetComponent<KSelectable>());
		}
	}

	protected override void OnCmpDisable()
	{
		if (SelectTool.Instance != null)
		{
			SelectTool.Instance.SetHoverOverride(null);
		}
	}
}
