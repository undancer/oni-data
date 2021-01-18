using Klei.AI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/CrewListEntry")]
public class CrewListEntry : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerClickHandler
{
	protected MinionIdentity identity;

	protected CrewPortrait portrait;

	public CrewPortrait PortraitPrefab;

	public GameObject crewPortraitParent;

	protected bool mouseOver;

	public Image BorderHighlight;

	public Image BGImage;

	public float lastClickTime;

	public MinionIdentity Identity => identity;

	public void OnPointerEnter(PointerEventData eventData)
	{
		mouseOver = true;
		BGImage.enabled = true;
		BorderHighlight.color = new Color(56f / 85f, 74f / 255f, 121f / 255f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		mouseOver = false;
		BGImage.enabled = false;
		BorderHighlight.color = new Color(0.8f, 0.8f, 0.8f);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool focus = Time.unscaledTime - lastClickTime < 0.3f;
		SelectCrewMember(focus);
		lastClickTime = Time.unscaledTime;
	}

	public virtual void Populate(MinionIdentity _identity)
	{
		identity = _identity;
		if (portrait == null)
		{
			GameObject parent = ((crewPortraitParent != null) ? crewPortraitParent : base.gameObject);
			portrait = Util.KInstantiateUI<CrewPortrait>(PortraitPrefab.gameObject, parent);
			if (crewPortraitParent == null)
			{
				portrait.transform.SetSiblingIndex(2);
			}
		}
		portrait.SetIdentityObject(_identity);
	}

	public virtual void Refresh()
	{
	}

	public void RefreshCrewPortraitContent()
	{
		if (portrait != null)
		{
			portrait.ForceRefresh();
		}
	}

	private string seniorityString()
	{
		return identity.GetAttributes().GetProfessionString();
	}

	public void SelectCrewMember(bool focus)
	{
		if (focus)
		{
			SelectTool.Instance.SelectAndFocus(identity.transform.GetPosition(), identity.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
		}
		else
		{
			SelectTool.Instance.Select(identity.GetComponent<KSelectable>());
		}
	}
}
