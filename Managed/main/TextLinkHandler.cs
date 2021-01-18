using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextLinkHandler : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
{
	private static TextLinkHandler hoveredText;

	[MyCmpGet]
	private LocText text;

	private bool hoverLink;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != 0 || !this.text.AllowLinks)
		{
			return;
		}
		int num = TMP_TextUtilities.FindIntersectingLink(this.text, KInputManager.GetMousePos(), null);
		if (num == -1)
		{
			return;
		}
		string text = CodexCache.FormatLinkID(this.text.textInfo.linkInfo[num].GetLinkID());
		if (!CodexCache.entries.ContainsKey(text))
		{
			SubEntry subEntry = CodexCache.FindSubEntry(text);
			if (subEntry == null || subEntry.disabled)
			{
				text = "PAGENOTFOUND";
			}
		}
		else if (CodexCache.entries[text].disabled)
		{
			text = "PAGENOTFOUND";
		}
		if (!ManagementMenu.Instance.codexScreen.gameObject.activeInHierarchy)
		{
			ManagementMenu.Instance.ToggleCodex();
		}
		ManagementMenu.Instance.codexScreen.ChangeArticle(text, playClickSound: true);
	}

	private void Update()
	{
		CheckMouseOver();
		if (hoveredText == this && text.AllowLinks)
		{
			PlayerController.Instance.ActiveTool.SetLinkCursor(hoverLink);
		}
	}

	private void OnEnable()
	{
		CheckMouseOver();
	}

	private void OnDisable()
	{
		ClearState();
	}

	private void Awake()
	{
		text = GetComponent<LocText>();
		if (text.AllowLinks && !text.raycastTarget)
		{
			text.raycastTarget = true;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetMouseOver();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ClearState();
	}

	private void ClearState()
	{
		if (!(this == null) && !Equals(null) && hoveredText == this)
		{
			if (hoverLink && PlayerController.Instance != null && PlayerController.Instance.ActiveTool != null)
			{
				PlayerController.Instance.ActiveTool.SetLinkCursor(set: false);
			}
			hoveredText = null;
			hoverLink = false;
		}
	}

	public void CheckMouseOver()
	{
		if (!(text == null))
		{
			if (TMP_TextUtilities.FindIntersectingLink(text, KInputManager.GetMousePos(), null) != -1)
			{
				SetMouseOver();
				hoverLink = true;
			}
			else if (hoveredText == this)
			{
				hoverLink = false;
			}
		}
	}

	private void SetMouseOver()
	{
		if (hoveredText != null && hoveredText != this)
		{
			hoveredText.hoverLink = false;
		}
		hoveredText = this;
	}
}
