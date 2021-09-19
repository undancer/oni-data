using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexScreen : KScreen
{
	public enum PlanCategory
	{
		Home,
		Tips,
		MyLog,
		Investigations,
		Emails,
		Journals,
		ResearchNotes,
		Creatures,
		Plants,
		Food,
		Tech,
		Diseases,
		Roles,
		Buildings,
		Elements
	}

	public enum HistoryDirection
	{
		Back,
		Forward,
		Up,
		NewArticle
	}

	public class HistoryEntry
	{
		public string id;

		public Vector3 position;

		public string name;

		public HistoryEntry(string entry, Vector3 pos, string articleName)
		{
			id = entry;
			position = pos;
			name = articleName;
		}
	}

	private string _activeEntryID;

	private Dictionary<Type, UIGameObjectPool> ContentUIPools = new Dictionary<Type, UIGameObjectPool>();

	private Dictionary<Type, GameObject> ContentPrefabs = new Dictionary<Type, GameObject>();

	private List<GameObject> categoryHeaders = new List<GameObject>();

	private Dictionary<CodexEntry, GameObject> entryButtons = new Dictionary<CodexEntry, GameObject>();

	private Dictionary<SubEntry, GameObject> subEntryButtons = new Dictionary<SubEntry, GameObject>();

	private UIGameObjectPool contentContainerPool;

	[SerializeField]
	private KScrollRect displayScrollRect;

	[SerializeField]
	private RectTransform scrollContentPane;

	private bool editingSearch;

	private List<HistoryEntry> history = new List<HistoryEntry>();

	private int currentHistoryIdx;

	[Header("Hierarchy")]
	[SerializeField]
	private Transform navigatorContent;

	[SerializeField]
	private Transform displayPane;

	[SerializeField]
	private Transform contentContainers;

	[SerializeField]
	private Transform widgetPool;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private TMP_InputField searchInputField;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private LocText backButton;

	[SerializeField]
	private KButton backButtonButton;

	[SerializeField]
	private KButton fwdButtonButton;

	[SerializeField]
	private LocText currentLocationText;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject prefabNavigatorEntry;

	[SerializeField]
	private GameObject prefabCategoryHeader;

	[SerializeField]
	private GameObject prefabContentContainer;

	[SerializeField]
	private GameObject prefabTextWidget;

	[SerializeField]
	private GameObject prefabTextWithTooltipWidget;

	[SerializeField]
	private GameObject prefabImageWidget;

	[SerializeField]
	private GameObject prefabDividerLineWidget;

	[SerializeField]
	private GameObject prefabSpacer;

	[SerializeField]
	private GameObject prefabLargeSpacer;

	[SerializeField]
	private GameObject prefabLabelWithIcon;

	[SerializeField]
	private GameObject prefabLabelWithLargeIcon;

	[SerializeField]
	private GameObject prefabContentLocked;

	[SerializeField]
	private GameObject prefabVideoWidget;

	[SerializeField]
	private GameObject prefabIndentedLabelWithIcon;

	[SerializeField]
	private GameObject prefabRecipePanel;

	[Header("Text Styles")]
	[SerializeField]
	private TextStyleSetting textStyleTitle;

	[SerializeField]
	private TextStyleSetting textStyleSubtitle;

	[SerializeField]
	private TextStyleSetting textStyleBody;

	[SerializeField]
	private TextStyleSetting textStyleBodyWhite;

	private Dictionary<CodexTextStyle, TextStyleSetting> textStyles = new Dictionary<CodexTextStyle, TextStyleSetting>();

	private List<CodexEntry> searchResults = new List<CodexEntry>();

	private List<SubEntry> subEntrySearchResults = new List<SubEntry>();

	private Coroutine scrollToTargetRoutine;

	public string activeEntryID
	{
		get
		{
			return _activeEntryID;
		}
		private set
		{
			_activeEntryID = value;
		}
	}

	protected override void OnActivate()
	{
		base.ConsumeMouseScroll = true;
		base.OnActivate();
		closeButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		clearSearchButton.onClick += delegate
		{
			searchInputField.text = "";
		};
		if (string.IsNullOrEmpty(activeEntryID))
		{
			ChangeArticle("HOME");
		}
		searchInputField.onValueChanged.AddListener(delegate(string value)
		{
			FilterSearch(value);
		});
		TMP_InputField tMP_InputField = searchInputField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			editingSearch = true;
		});
		searchInputField.onEndEdit.AddListener(delegate
		{
			editingSearch = false;
		});
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (editingSearch)
		{
			e.Consumed = true;
		}
		base.OnKeyDown(e);
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	private void CodexScreenInit()
	{
		textStyles[CodexTextStyle.Title] = textStyleTitle;
		textStyles[CodexTextStyle.Subtitle] = textStyleSubtitle;
		textStyles[CodexTextStyle.Body] = textStyleBody;
		textStyles[CodexTextStyle.BodyWhite] = textStyleBodyWhite;
		SetupPrefabs();
		PopulatePools();
		CategorizeEntries();
		FilterSearch("");
		backButtonButton.onClick += HistoryStepBack;
		backButtonButton.soundPlayer.AcceptClickCondition = () => currentHistoryIdx > 0;
		fwdButtonButton.onClick += HistoryStepForward;
		fwdButtonButton.soundPlayer.AcceptClickCondition = () => currentHistoryIdx < history.Count - 1;
		Game.Instance.Subscribe(1594320620, delegate
		{
			if (base.gameObject.activeSelf)
			{
				FilterSearch(searchInputField.text);
				if (!string.IsNullOrEmpty(activeEntryID))
				{
					ChangeArticle(activeEntryID);
				}
			}
		});
	}

	private void SetupPrefabs()
	{
		contentContainerPool = new UIGameObjectPool(prefabContentContainer);
		contentContainerPool.disabledElementParent = widgetPool;
		ContentPrefabs[typeof(CodexText)] = prefabTextWidget;
		ContentPrefabs[typeof(CodexTextWithTooltip)] = prefabTextWithTooltipWidget;
		ContentPrefabs[typeof(CodexImage)] = prefabImageWidget;
		ContentPrefabs[typeof(CodexDividerLine)] = prefabDividerLineWidget;
		ContentPrefabs[typeof(CodexSpacer)] = prefabSpacer;
		ContentPrefabs[typeof(CodexLabelWithIcon)] = prefabLabelWithIcon;
		ContentPrefabs[typeof(CodexLabelWithLargeIcon)] = prefabLabelWithLargeIcon;
		ContentPrefabs[typeof(CodexContentLockedIndicator)] = prefabContentLocked;
		ContentPrefabs[typeof(CodexLargeSpacer)] = prefabLargeSpacer;
		ContentPrefabs[typeof(CodexVideo)] = prefabVideoWidget;
		ContentPrefabs[typeof(CodexIndentedLabelWithIcon)] = prefabIndentedLabelWithIcon;
		ContentPrefabs[typeof(CodexRecipePanel)] = prefabRecipePanel;
	}

	private List<CodexEntry> FilterSearch(string input)
	{
		searchResults.Clear();
		subEntrySearchResults.Clear();
		input = input.ToLower();
		foreach (KeyValuePair<string, CodexEntry> entry in CodexCache.entries)
		{
			bool flag = false;
			string[] dlcIds = entry.Value.GetDlcIds();
			for (int i = 0; i < dlcIds.Length; i++)
			{
				if (DlcManager.IsContentActive(dlcIds[i]))
				{
					flag = true;
					break;
				}
			}
			string[] forbiddenDLCs = entry.Value.GetForbiddenDLCs();
			for (int j = 0; j < forbiddenDLCs.Length; j++)
			{
				if (DlcManager.IsContentActive(forbiddenDLCs[j]))
				{
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			if (input == "")
			{
				if (!entry.Value.searchOnly)
				{
					searchResults.Add(entry.Value);
				}
			}
			else if (input == entry.Value.name.ToLower() || input.Contains(entry.Value.name.ToLower()) || entry.Value.name.ToLower().Contains(input))
			{
				searchResults.Add(entry.Value);
			}
		}
		foreach (KeyValuePair<string, SubEntry> subEntry in CodexCache.subEntries)
		{
			if (input == subEntry.Value.name.ToLower() || input.Contains(subEntry.Value.name.ToLower()) || subEntry.Value.name.ToLower().Contains(input))
			{
				subEntrySearchResults.Add(subEntry.Value);
			}
		}
		FilterEntries(input != "");
		return searchResults;
	}

	private bool HasUnlockedCategoryEntries(string entryID)
	{
		foreach (ContentContainer contentContainer in CodexCache.entries[entryID].contentContainers)
		{
			if (string.IsNullOrEmpty(contentContainer.lockID) || Game.Instance.unlocks.IsUnlocked(contentContainer.lockID))
			{
				return true;
			}
		}
		return false;
	}

	private void FilterEntries(bool allowOpenCategories = true)
	{
		foreach (KeyValuePair<CodexEntry, GameObject> entryButton in entryButtons)
		{
			entryButton.Value.SetActive(searchResults.Contains(entryButton.Key) && HasUnlockedCategoryEntries(entryButton.Key.id));
		}
		foreach (KeyValuePair<SubEntry, GameObject> subEntryButton in subEntryButtons)
		{
			subEntryButton.Value.SetActive(subEntrySearchResults.Contains(subEntryButton.Key));
		}
		foreach (GameObject categoryHeader in categoryHeaders)
		{
			bool flag = false;
			Transform transform = categoryHeader.transform.Find("Content");
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).gameObject.activeSelf)
				{
					flag = true;
				}
			}
			categoryHeader.SetActive(flag);
			if (allowOpenCategories)
			{
				if (flag)
				{
					ToggleCategoryOpen(categoryHeader, open: true);
				}
			}
			else
			{
				ToggleCategoryOpen(categoryHeader, open: false);
			}
		}
	}

	private void ToggleCategoryOpen(GameObject header, bool open)
	{
		header.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").ChangeState(open ? 1 : 0);
		header.GetComponent<HierarchyReferences>().GetReference("Content").gameObject.SetActive(open);
	}

	private void PopulatePools()
	{
		foreach (KeyValuePair<Type, GameObject> contentPrefab in ContentPrefabs)
		{
			UIGameObjectPool uIGameObjectPool = new UIGameObjectPool(contentPrefab.Value);
			uIGameObjectPool.disabledElementParent = widgetPool;
			ContentUIPools[contentPrefab.Key] = uIGameObjectPool;
		}
	}

	private GameObject NewCategoryHeader(KeyValuePair<string, CodexEntry> entryKVP, Dictionary<string, GameObject> categories)
	{
		if (entryKVP.Value.category == "")
		{
			entryKVP.Value.category = "Root";
		}
		GameObject categoryHeader = Util.KInstantiateUI(prefabCategoryHeader, navigatorContent.gameObject, force_active: true);
		GameObject categoryContent = categoryHeader.GetComponent<HierarchyReferences>().GetReference("Content").gameObject;
		categories.Add(entryKVP.Value.category, categoryContent);
		LocText reference = categoryHeader.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
		if (CodexCache.entries.ContainsKey(entryKVP.Value.category))
		{
			reference.text = CodexCache.entries[entryKVP.Value.category].name;
		}
		else
		{
			reference.text = Strings.Get("STRINGS.UI.CODEX.CATEGORYNAMES." + entryKVP.Value.category.ToUpper());
		}
		categoryHeaders.Add(categoryHeader);
		categoryContent.SetActive(value: false);
		categoryHeader.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("ExpandToggle").onClick = delegate
		{
			ToggleCategoryOpen(categoryHeader, !categoryContent.activeSelf);
		};
		return categoryHeader;
	}

	private void CategorizeEntries()
	{
		string text = "";
		_ = navigatorContent.gameObject;
		Dictionary<string, GameObject> dictionary = new Dictionary<string, GameObject>();
		List<Tuple<string, CodexEntry>> list = new List<Tuple<string, CodexEntry>>();
		foreach (KeyValuePair<string, CodexEntry> entry in CodexCache.entries)
		{
			if (string.IsNullOrEmpty(entry.Value.sortString))
			{
				entry.Value.sortString = UI.StripLinkFormatting(Strings.Get(entry.Value.title));
			}
			list.Add(new Tuple<string, CodexEntry>(entry.Key, entry.Value));
		}
		list.Sort((Tuple<string, CodexEntry> a, Tuple<string, CodexEntry> b) => string.Compare(a.second.sortString, b.second.sortString));
		for (int i = 0; i < list.Count; i++)
		{
			Tuple<string, CodexEntry> tuple = list[i];
			text = tuple.second.category;
			if (text == "" || text == "Root")
			{
				text = "Root";
			}
			if (!dictionary.ContainsKey(text))
			{
				NewCategoryHeader(new KeyValuePair<string, CodexEntry>(tuple.first, tuple.second), dictionary);
			}
			GameObject gameObject = Util.KInstantiateUI(prefabNavigatorEntry, dictionary[text], force_active: true);
			string id = tuple.second.id;
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				ChangeArticle(id);
			};
			if (string.IsNullOrEmpty(tuple.second.name))
			{
				tuple.second.name = Strings.Get(tuple.second.title);
			}
			gameObject.GetComponentInChildren<LocText>().text = tuple.second.name;
			entryButtons.Add(tuple.second, gameObject);
			foreach (SubEntry subEntry in tuple.second.subEntries)
			{
				GameObject gameObject2 = Util.KInstantiateUI(prefabNavigatorEntry, dictionary[text], force_active: true);
				string subEntryId = subEntry.id;
				gameObject2.GetComponent<KButton>().onClick += delegate
				{
					ChangeArticle(subEntryId);
				};
				if (string.IsNullOrEmpty(subEntry.name))
				{
					subEntry.name = Strings.Get(subEntry.title);
				}
				gameObject2.GetComponentInChildren<LocText>().text = subEntry.name;
				subEntryButtons.Add(subEntry, gameObject2);
				CodexCache.subEntries.Add(subEntry.id, subEntry);
			}
		}
		foreach (KeyValuePair<string, CodexEntry> entry2 in CodexCache.entries)
		{
			if (CodexCache.entries.ContainsKey(entry2.Value.category) && CodexCache.entries.ContainsKey(CodexCache.entries[entry2.Value.category].category))
			{
				entry2.Value.searchOnly = true;
			}
		}
		List<KeyValuePair<string, GameObject>> list2 = new List<KeyValuePair<string, GameObject>>();
		foreach (KeyValuePair<string, GameObject> item in dictionary)
		{
			list2.Add(item);
		}
		list2.Sort((KeyValuePair<string, GameObject> a, KeyValuePair<string, GameObject> b) => string.Compare(a.Value.name, b.Value.name));
		for (int j = 0; j < list2.Count; j++)
		{
			list2[j].Value.transform.parent.SetSiblingIndex(j);
		}
		SetupCategory(dictionary, "PLANTS");
		SetupCategory(dictionary, "CREATURES");
		SetupCategory(dictionary, "NOTICES");
		SetupCategory(dictionary, "RESEARCHNOTES");
		SetupCategory(dictionary, "JOURNALS");
		SetupCategory(dictionary, "EMAILS");
		SetupCategory(dictionary, "INVESTIGATIONS");
		SetupCategory(dictionary, "MYLOG");
		SetupCategory(dictionary, "LESSONS");
		SetupCategory(dictionary, "Root");
	}

	private static void SetupCategory(Dictionary<string, GameObject> categories, string category_name)
	{
		if (categories.ContainsKey(category_name))
		{
			categories[category_name].transform.parent.SetAsFirstSibling();
		}
	}

	public void ChangeArticle(string id, bool playClickSound = false, Vector3 targetPosition = default(Vector3), HistoryDirection historyMovement = HistoryDirection.NewArticle)
	{
		Debug.Assert(id != null);
		if (playClickSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
		}
		if (contentContainerPool == null)
		{
			CodexScreenInit();
		}
		string text = "";
		SubEntry subEntry = null;
		if (!CodexCache.entries.ContainsKey(id))
		{
			subEntry = null;
			subEntry = CodexCache.FindSubEntry(id);
			if (subEntry != null && !subEntry.disabled)
			{
				id = subEntry.parentEntryID.ToUpper();
				text = UI.StripLinkFormatting(subEntry.name);
			}
			else
			{
				id = "PAGENOTFOUND";
			}
		}
		if (CodexCache.entries[id].disabled)
		{
			id = "PAGENOTFOUND";
		}
		if (string.IsNullOrEmpty(text))
		{
			text = UI.StripLinkFormatting(CodexCache.entries[id].name);
		}
		ICodexWidget codexWidget = null;
		CodexCache.entries[id].GetFirstWidget();
		RectTransform rectTransform = null;
		if (subEntry != null)
		{
			foreach (ContentContainer contentContainer2 in CodexCache.entries[id].contentContainers)
			{
				if (contentContainer2 == subEntry.contentContainers[0])
				{
					codexWidget = contentContainer2.content[0];
					break;
				}
			}
		}
		int num = 0;
		string text2 = "";
		while (contentContainers.transform.childCount > 0)
		{
			while (!string.IsNullOrEmpty(text2) && CodexCache.entries[activeEntryID].contentContainers[num].lockID == text2)
			{
				num++;
			}
			GameObject gameObject = contentContainers.transform.GetChild(0).gameObject;
			int num2 = 0;
			while (gameObject.transform.childCount > 0)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
				Type key;
				if (gameObject2.name == "PrefabContentLocked")
				{
					text2 = CodexCache.entries[activeEntryID].contentContainers[num].lockID;
					key = typeof(CodexContentLockedIndicator);
				}
				else
				{
					key = CodexCache.entries[activeEntryID].contentContainers[num].content[num2].GetType();
				}
				ContentUIPools[key].ClearElement(gameObject2);
				num2++;
			}
			contentContainerPool.ClearElement(contentContainers.transform.GetChild(0).gameObject);
			num++;
		}
		bool flag = CodexCache.entries[id] is CategoryEntry;
		activeEntryID = id;
		if (CodexCache.entries[id].contentContainers == null)
		{
			CodexCache.entries[id].CreateContentContainerCollection();
		}
		bool flag2 = false;
		string text3 = "";
		for (int i = 0; i < CodexCache.entries[id].contentContainers.Count; i++)
		{
			ContentContainer contentContainer = CodexCache.entries[id].contentContainers[i];
			GameObject gameObject3;
			if (!string.IsNullOrEmpty(contentContainer.lockID) && !Game.Instance.unlocks.IsUnlocked(contentContainer.lockID))
			{
				if (text3 != contentContainer.lockID)
				{
					gameObject3 = contentContainerPool.GetFreeElement(contentContainers.gameObject, forceActive: true).gameObject;
					ConfigureContentContainer(contentContainer, gameObject3, flag && flag2);
					text3 = contentContainer.lockID;
					_ = ContentUIPools[typeof(CodexContentLockedIndicator)].GetFreeElement(gameObject3, forceActive: true).gameObject;
				}
				continue;
			}
			gameObject3 = contentContainerPool.GetFreeElement(contentContainers.gameObject, forceActive: true).gameObject;
			ConfigureContentContainer(contentContainer, gameObject3, flag && flag2);
			flag2 = !flag2;
			if (contentContainer.content == null)
			{
				continue;
			}
			foreach (ICodexWidget item in contentContainer.content)
			{
				GameObject gameObject4 = null;
				gameObject4 = ContentUIPools[item.GetType()].GetFreeElement(gameObject3, forceActive: true).gameObject;
				item.Configure(gameObject4, displayPane, textStyles);
				if (item == codexWidget)
				{
					rectTransform = gameObject4.rectTransform();
				}
			}
		}
		string text4 = "";
		string text5 = id;
		int num3 = 0;
		while (text5 != CodexCache.FormatLinkID("HOME") && num3 < 10)
		{
			num3++;
			if (text5 != null)
			{
				text4 = ((!(text5 != id)) ? text4.Insert(0, CodexCache.entries[text5].name) : text4.Insert(0, CodexCache.entries[text5].name + " > "));
				text5 = CodexCache.entries[text5].parentId;
			}
			else
			{
				text5 = CodexCache.entries[CodexCache.FormatLinkID("HOME")].id;
				text4 = text4.Insert(0, CodexCache.entries[text5].name + " > ");
			}
		}
		currentLocationText.text = ((text4 == "") ? ("<b>" + UI.StripLinkFormatting(CodexCache.entries["HOME"].name) + "</b>") : text4);
		if (history.Count == 0)
		{
			history.Add(new HistoryEntry(id, Vector3.zero, text));
			currentHistoryIdx = 0;
		}
		else
		{
			switch (historyMovement)
			{
			case HistoryDirection.Back:
				history[currentHistoryIdx].position = displayPane.transform.localPosition;
				currentHistoryIdx--;
				break;
			case HistoryDirection.Forward:
				history[currentHistoryIdx].position = displayPane.transform.localPosition;
				currentHistoryIdx++;
				break;
			case HistoryDirection.Up:
			case HistoryDirection.NewArticle:
			{
				if (currentHistoryIdx == history.Count - 1)
				{
					history.Add(new HistoryEntry(activeEntryID, Vector3.zero, text));
					history[currentHistoryIdx].position = displayPane.transform.localPosition;
					currentHistoryIdx++;
					break;
				}
				for (int num4 = history.Count - 1; num4 > currentHistoryIdx; num4--)
				{
					history.RemoveAt(num4);
				}
				history.Add(new HistoryEntry(activeEntryID, Vector3.zero, text));
				history[history.Count - 2].position = displayPane.transform.localPosition;
				currentHistoryIdx++;
				break;
			}
			}
		}
		if (currentHistoryIdx > 0)
		{
			backButtonButton.GetComponent<Image>().color = Color.black;
			backButton.text = UI.FormatAsLink(string.Format(UI.CODEX.BACK_BUTTON, UI.StripLinkFormatting(CodexCache.entries[history[history.Count - 2].id].name)), CodexCache.entries[history[history.Count - 2].id].id);
			backButtonButton.GetComponent<ToolTip>().toolTip = string.Format(UI.CODEX.BACK_BUTTON_TOOLTIP, history[currentHistoryIdx - 1].name);
		}
		else
		{
			backButtonButton.GetComponent<Image>().color = Color.grey;
			backButton.text = UI.StripLinkFormatting(GameUtil.ColourizeString(Color.grey, string.Format(UI.CODEX.BACK_BUTTON, CodexCache.entries["HOME"].name)));
			backButtonButton.GetComponent<ToolTip>().toolTip = UI.CODEX.BACK_BUTTON_NO_HISTORY_TOOLTIP;
		}
		if (currentHistoryIdx < history.Count - 1)
		{
			fwdButtonButton.GetComponent<Image>().color = Color.black;
			fwdButtonButton.GetComponent<ToolTip>().toolTip = string.Format(UI.CODEX.FORWARD_BUTTON_TOOLTIP, history[currentHistoryIdx + 1].name);
		}
		else
		{
			fwdButtonButton.GetComponent<Image>().color = Color.grey;
			fwdButtonButton.GetComponent<ToolTip>().toolTip = UI.CODEX.FORWARD_BUTTON_NO_HISTORY_TOOLTIP;
		}
		if (targetPosition != Vector3.zero)
		{
			if (scrollToTargetRoutine != null)
			{
				StopCoroutine(scrollToTargetRoutine);
			}
			scrollToTargetRoutine = StartCoroutine(ScrollToTarget(targetPosition));
		}
		else if (rectTransform != null)
		{
			if (scrollToTargetRoutine != null)
			{
				StopCoroutine(scrollToTargetRoutine);
			}
			scrollToTargetRoutine = StartCoroutine(ScrollToTarget(rectTransform));
		}
		else
		{
			displayScrollRect.content.SetLocalPosition(Vector3.zero);
		}
	}

	private void HistoryStepBack()
	{
		if (currentHistoryIdx != 0)
		{
			ChangeArticle(history[currentHistoryIdx - 1].id, playClickSound: false, history[currentHistoryIdx - 1].position, HistoryDirection.Back);
		}
	}

	private void HistoryStepForward()
	{
		if (currentHistoryIdx != history.Count - 1)
		{
			ChangeArticle(history[currentHistoryIdx + 1].id, playClickSound: false, history[currentHistoryIdx + 1].position, HistoryDirection.Forward);
		}
	}

	private void HistoryStepUp()
	{
		if (!string.IsNullOrEmpty(CodexCache.entries[activeEntryID].parentId))
		{
			ChangeArticle(CodexCache.entries[activeEntryID].parentId, playClickSound: false, default(Vector3), HistoryDirection.Up);
		}
	}

	private IEnumerator ScrollToTarget(RectTransform targetWidgetTransform)
	{
		yield return 0;
		displayScrollRect.content.SetLocalPosition(Vector3.down * (displayScrollRect.content.InverseTransformPoint(targetWidgetTransform.GetPosition()).y + 12f));
	}

	private IEnumerator ScrollToTarget(Vector3 position)
	{
		yield return 0;
		displayScrollRect.content.SetLocalPosition(position);
	}

	private void ConfigureContentContainer(ContentContainer container, GameObject containerGameObject, bool bgColor = false)
	{
		container.go = containerGameObject;
		LayoutGroup component = containerGameObject.GetComponent<LayoutGroup>();
		if (component != null)
		{
			UnityEngine.Object.DestroyImmediate(component);
		}
		if (Game.Instance.unlocks.IsUnlocked(container.lockID) || string.IsNullOrEmpty(container.lockID))
		{
			switch (container.contentLayout)
			{
			case ContentContainer.ContentLayout.Horizontal:
			{
				component = containerGameObject.AddComponent<HorizontalLayoutGroup>();
				component.childAlignment = TextAnchor.MiddleLeft;
				HorizontalOrVerticalLayoutGroup obj2 = component as HorizontalOrVerticalLayoutGroup;
				bool childForceExpandHeight = ((component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
				obj2.childForceExpandHeight = childForceExpandHeight;
				(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
				break;
			}
			case ContentContainer.ContentLayout.Vertical:
			{
				component = containerGameObject.AddComponent<VerticalLayoutGroup>();
				HorizontalOrVerticalLayoutGroup obj = component as HorizontalOrVerticalLayoutGroup;
				bool childForceExpandHeight = ((component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
				obj.childForceExpandHeight = childForceExpandHeight;
				(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
				break;
			}
			case ContentContainer.ContentLayout.Grid:
				component = containerGameObject.AddComponent<GridLayoutGroup>();
				(component as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				(component as GridLayoutGroup).constraintCount = 4;
				(component as GridLayoutGroup).cellSize = new Vector2(128f, 180f);
				(component as GridLayoutGroup).spacing = new Vector2(6f, 6f);
				break;
			case ContentContainer.ContentLayout.GridTwoColumn:
				component = containerGameObject.AddComponent<GridLayoutGroup>();
				(component as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				(component as GridLayoutGroup).constraintCount = 2;
				(component as GridLayoutGroup).cellSize = new Vector2(264f, 32f);
				(component as GridLayoutGroup).spacing = new Vector2(0f, 12f);
				break;
			case ContentContainer.ContentLayout.GridTwoColumnTall:
				component = containerGameObject.AddComponent<GridLayoutGroup>();
				(component as GridLayoutGroup).constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				(component as GridLayoutGroup).constraintCount = 2;
				(component as GridLayoutGroup).cellSize = new Vector2(264f, 64f);
				(component as GridLayoutGroup).spacing = new Vector2(0f, 12f);
				break;
			}
		}
		else
		{
			component = containerGameObject.AddComponent<VerticalLayoutGroup>();
			HorizontalOrVerticalLayoutGroup obj3 = component as HorizontalOrVerticalLayoutGroup;
			bool childForceExpandHeight = ((component as HorizontalOrVerticalLayoutGroup).childForceExpandWidth = false);
			obj3.childForceExpandHeight = childForceExpandHeight;
			(component as HorizontalOrVerticalLayoutGroup).spacing = 8f;
		}
	}
}
