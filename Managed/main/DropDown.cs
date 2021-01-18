using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/DropDown")]
public class DropDown : KMonoBehaviour
{
	public GameObject targetDropDownContainer;

	public LocText selectedLabel;

	public KButton openButton;

	public Transform contentContainer;

	public GameObject scrollRect;

	public RectTransform dropdownAlignmentTarget;

	public GameObject rowEntryPrefab;

	public bool addEmptyRow = true;

	public object targetData = null;

	private List<IListableOption> entries = new List<IListableOption>();

	private Action<IListableOption, object> onEntrySelectedAction;

	private Action<DropDownEntry, object> rowRefreshAction;

	public Dictionary<IListableOption, GameObject> rowLookup = new Dictionary<IListableOption, GameObject>();

	private Func<IListableOption, IListableOption, object, int> sortFunction;

	private GameObject emptyRow;

	private string emptyRowLabel;

	private Sprite emptyRowSprite;

	private bool built = false;

	private bool displaySelectedValueWhenClosed = true;

	private const int ROWS_BEFORE_SCROLL = 8;

	private KCanvasScaler canvasScaler;

	public bool open
	{
		get;
		private set;
	}

	public List<IListableOption> Entries => entries;

	public void Initialize(IEnumerable<IListableOption> contentKeys, Action<IListableOption, object> onEntrySelectedAction, Func<IListableOption, IListableOption, object, int> sortFunction = null, Action<DropDownEntry, object> refreshAction = null, bool displaySelectedValueWhenClosed = true, object targetData = null)
	{
		this.targetData = targetData;
		this.sortFunction = sortFunction;
		this.onEntrySelectedAction = onEntrySelectedAction;
		this.displaySelectedValueWhenClosed = displaySelectedValueWhenClosed;
		rowRefreshAction = refreshAction;
		ChangeContent(contentKeys);
		openButton.ClearOnClick();
		openButton.onClick += delegate
		{
			OnClick();
		};
		canvasScaler = GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>();
	}

	public void CustomizeEmptyRow(string txt, Sprite icon)
	{
		emptyRowLabel = txt;
		emptyRowSprite = icon;
	}

	public void OnClick()
	{
		if (!open)
		{
			Open();
		}
		else
		{
			Close();
		}
	}

	public void ChangeContent(IEnumerable<IListableOption> contentKeys)
	{
		entries.Clear();
		foreach (IListableOption contentKey in contentKeys)
		{
			entries.Add(contentKey);
		}
		built = false;
	}

	private void Update()
	{
		if (open && (Input.GetMouseButtonDown(0) || Input.GetAxis("Mouse ScrollWheel") != 0f))
		{
			float canvasScale = canvasScaler.GetCanvasScale();
			if (scrollRect.rectTransform().GetPosition().x + scrollRect.rectTransform().sizeDelta.x * canvasScale < KInputManager.GetMousePos().x || scrollRect.rectTransform().GetPosition().x > KInputManager.GetMousePos().x || scrollRect.rectTransform().GetPosition().y - scrollRect.rectTransform().sizeDelta.y * canvasScale > KInputManager.GetMousePos().y || scrollRect.rectTransform().GetPosition().y < KInputManager.GetMousePos().y)
			{
				Close();
			}
		}
	}

	private void Build(List<IListableOption> contentKeys)
	{
		built = true;
		for (int num = contentContainer.childCount - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(contentContainer.GetChild(num));
		}
		rowLookup.Clear();
		if (addEmptyRow)
		{
			emptyRow = Util.KInstantiateUI(rowEntryPrefab, contentContainer.gameObject, force_active: true);
			emptyRow.GetComponent<KButton>().onClick += delegate
			{
				onEntrySelectedAction(null, targetData);
				if (displaySelectedValueWhenClosed)
				{
					selectedLabel.text = emptyRowLabel ?? ((string)UI.DROPDOWN.NONE);
				}
				Close();
			};
			string text = emptyRowLabel ?? ((string)UI.DROPDOWN.NONE);
			emptyRow.GetComponent<DropDownEntry>().label.text = text;
			if (emptyRowSprite != null)
			{
				emptyRow.GetComponent<DropDownEntry>().image.sprite = emptyRowSprite;
			}
		}
		for (int i = 0; i < contentKeys.Count; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(rowEntryPrefab, contentContainer.gameObject, force_active: true);
			IListableOption id = contentKeys[i];
			gameObject.GetComponent<DropDownEntry>().entryData = id;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				onEntrySelectedAction(id, targetData);
				if (displaySelectedValueWhenClosed)
				{
					selectedLabel.text = id.GetProperName();
				}
				Close();
			};
			rowLookup.Add(id, gameObject);
		}
		RefreshEntries();
		Close();
		scrollRect.gameObject.transform.SetParent(targetDropDownContainer.transform);
		scrollRect.gameObject.SetActive(value: false);
	}

	private void RefreshEntries()
	{
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			DropDownEntry component = item.Value.GetComponent<DropDownEntry>();
			component.label.text = item.Key.GetProperName();
			if (component.portrait != null && item.Key is IAssignableIdentity)
			{
				component.portrait.SetIdentityObject(item.Key as IAssignableIdentity);
			}
		}
		if (sortFunction != null)
		{
			entries.Sort((IListableOption a, IListableOption b) => sortFunction(a, b, targetData));
			for (int i = 0; i < entries.Count; i++)
			{
				rowLookup[entries[i]].transform.SetAsFirstSibling();
			}
			if (emptyRow != null)
			{
				emptyRow.transform.SetAsFirstSibling();
			}
		}
		foreach (KeyValuePair<IListableOption, GameObject> item2 in rowLookup)
		{
			DropDownEntry component2 = item2.Value.GetComponent<DropDownEntry>();
			rowRefreshAction(component2, targetData);
		}
		if (emptyRow != null)
		{
			rowRefreshAction(emptyRow.GetComponent<DropDownEntry>(), targetData);
		}
	}

	protected override void OnCleanUp()
	{
		Util.KDestroyGameObject(scrollRect);
		base.OnCleanUp();
	}

	public void Open()
	{
		if (open)
		{
			return;
		}
		if (!built)
		{
			Build(entries);
		}
		else
		{
			RefreshEntries();
		}
		open = true;
		scrollRect.gameObject.SetActive(value: true);
		scrollRect.rectTransform().localScale = Vector3.one;
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			item.Value.SetActive(value: true);
		}
		scrollRect.rectTransform().sizeDelta = new Vector2(scrollRect.rectTransform().sizeDelta.x, 32f * (float)Mathf.Min(contentContainer.childCount, 8));
		Vector3 position = dropdownAlignmentTarget.TransformPoint(dropdownAlignmentTarget.rect.x, dropdownAlignmentTarget.rect.y, 0f);
		Vector2 v = new Vector2(Mathf.Min(0f, (float)Screen.width - (position.x + rowEntryPrefab.GetComponent<LayoutElement>().minWidth)), 0f - Mathf.Min(0f, position.y - scrollRect.rectTransform().sizeDelta.y));
		position += (Vector3)v;
		scrollRect.rectTransform().SetPosition(position);
	}

	public void Close()
	{
		if (!open)
		{
			return;
		}
		open = false;
		foreach (KeyValuePair<IListableOption, GameObject> item in rowLookup)
		{
			item.Value.SetActive(value: false);
		}
		scrollRect.SetActive(value: false);
	}
}
