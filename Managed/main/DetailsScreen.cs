using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DetailsScreen : KTabMenu
{
	[Serializable]
	private struct Screens
	{
		public string name;

		public string displayName;

		public string tooltip;

		public Sprite icon;

		public TargetScreen screen;

		public int displayOrderPriority;

		public bool hideWhenDead;

		public HashedString focusInViewMode;

		[HideInInspector]
		public int tabIdx;
	}

	[Serializable]
	public class SideScreenRef
	{
		public string name;

		public SideScreenContent screenPrefab;

		public Vector2 offset;

		[HideInInspector]
		public SideScreenContent screenInstance;
	}

	public static DetailsScreen Instance;

	[SerializeField]
	private KButton CodexEntryButton;

	[Header("Panels")]
	public Transform UserMenuPanel;

	[Header("Name Editing (disabled)")]
	[SerializeField]
	private KButton CloseButton;

	[Header("Tabs")]
	[SerializeField]
	private EditableTitleBar TabTitle;

	[SerializeField]
	private Screens[] screens;

	[SerializeField]
	private GameObject tabHeaderContainer;

	[Header("Side Screens")]
	[SerializeField]
	private GameObject sideScreenContentBody;

	[SerializeField]
	private GameObject sideScreen;

	[SerializeField]
	private LocText sideScreenTitle;

	[SerializeField]
	private List<SideScreenRef> sideScreens;

	[Header("Secondary Side Screens")]
	[SerializeField]
	private GameObject sideScreen2ContentBody;

	[SerializeField]
	private GameObject sideScreen2;

	[SerializeField]
	private LocText sideScreen2Title;

	private KScreen activeSideScreen2;

	private bool HasActivated;

	private SideScreenContent currentSideScreen;

	private Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = new Dictionary<KScreen, KScreen>();

	private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>(delegate(DetailsScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	private List<KeyValuePair<GameObject, int>> sortedSideScreens = new List<KeyValuePair<GameObject, int>>();

	public GameObject target
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SortScreenOrder();
		base.ConsumeMouseScroll = true;
		Debug.Assert(Instance == null);
		Instance = this;
		UIRegistry.detailsScreen = this;
		DeactivateSideContent();
		Show(show: false);
		Subscribe(Game.Instance.gameObject, -1503271301, OnSelectObject);
	}

	private void OnSelectObject(object data)
	{
		if (data == null)
		{
			previouslyActiveTab = -1;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		CodexEntryButton.onClick += OpenCodexEntry;
		CloseButton.onClick += DeselectAndClose;
		TabTitle.OnNameChanged += OnNameChanged;
		TabTitle.OnStartedEditing += OnStartedEditing;
		sideScreen2.SetActive(value: false);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	private void OnStartedEditing()
	{
		base.isEditing = true;
		KScreenManager.Instance.RefreshStack();
	}

	private void OnNameChanged(string newName)
	{
		base.isEditing = false;
		if (!string.IsNullOrEmpty(newName))
		{
			MinionIdentity component = target.GetComponent<MinionIdentity>();
			UserNameable component2 = target.GetComponent<UserNameable>();
			if (component != null)
			{
				component.SetName(newName);
			}
			else if (component2 != null)
			{
				component2.SetName(newName);
			}
			TabTitle.UpdateRenameTooltip(target);
		}
	}

	protected override void OnDeactivate()
	{
		DeactivateSideContent();
		base.OnDeactivate();
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			DeactivateSideContent();
		}
		else
		{
			MaskSideContent(hide: false);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
		}
		base.OnShow(show);
	}

	protected override void OnCmpDisable()
	{
		DeactivateSideContent();
		base.OnCmpDisable();
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!base.isEditing && target != null && PlayerController.Instance.ConsumeIfNotDragging(e, Action.MouseRight))
		{
			DeselectAndClose();
		}
	}

	private static Component GetComponent(GameObject go, string name)
	{
		Component component = null;
		Type type = Type.GetType(name);
		if (type != null)
		{
			return go.GetComponent(type);
		}
		return go.GetComponent(name);
	}

	private static bool IsExcludedPrefabTag(GameObject go, Tag[] excluded_tags)
	{
		if (excluded_tags == null || excluded_tags.Length == 0)
		{
			return false;
		}
		bool result = false;
		KPrefabID component = go.GetComponent<KPrefabID>();
		foreach (Tag b in excluded_tags)
		{
			if (component.PrefabTag == b)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void UpdateCodexButton()
	{
		string selectedObjectCodexID = GetSelectedObjectCodexID();
		CodexEntryButton.isInteractable = selectedObjectCodexID != "";
		CodexEntryButton.GetComponent<ToolTip>().SetSimpleTooltip(CodexEntryButton.isInteractable ? UI.TOOLTIPS.OPEN_CODEX_ENTRY : UI.TOOLTIPS.NO_CODEX_ENTRY);
	}

	public void OnRefreshData(object obj)
	{
		SetTitle(base.PreviousActiveTab);
		for (int i = 0; i < tabs.Count; i++)
		{
			if (tabs[i].gameObject.activeInHierarchy)
			{
				tabs[i].Trigger(-1514841199, obj);
			}
		}
	}

	public void Refresh(GameObject go)
	{
		if (screens == null)
		{
			return;
		}
		target = go;
		sortedSideScreens.Clear();
		CellSelectionObject component = target.GetComponent<CellSelectionObject>();
		if ((bool)component)
		{
			component.OnObjectSelected(null);
		}
		if (!HasActivated)
		{
			if (screens != null)
			{
				for (int i = 0; i < screens.Length; i++)
				{
					GameObject gameObject = null;
					gameObject = KScreenManager.Instance.InstantiateScreen(screens[i].screen.gameObject, body.gameObject).gameObject;
					screens[i].screen = gameObject.GetComponent<TargetScreen>();
					screens[i].tabIdx = AddTab(screens[i].icon, Strings.Get(screens[i].displayName), screens[i].screen, Strings.Get(screens[i].tooltip));
				}
			}
			base.onTabActivated += OnTabActivated;
			HasActivated = true;
		}
		int num = -1;
		int num2 = 0;
		for (int j = 0; j < screens.Length; j++)
		{
			bool num3 = screens[j].screen.IsValidForTarget(go);
			bool flag = screens[j].hideWhenDead && base.gameObject.HasTag(GameTags.Dead);
			bool flag2 = num3 && !flag;
			SetTabEnabled(screens[j].tabIdx, flag2);
			if (!flag2)
			{
				continue;
			}
			num2++;
			if (num != -1)
			{
				continue;
			}
			if (SimDebugView.Instance.GetMode() != OverlayModes.None.ID)
			{
				if (SimDebugView.Instance.GetMode() == screens[j].focusInViewMode)
				{
					num = j;
				}
			}
			else if (flag2 && previouslyActiveTab >= 0 && previouslyActiveTab < screens.Length && screens[j].name == screens[previouslyActiveTab].name)
			{
				num = screens[j].tabIdx;
			}
		}
		if (num != -1)
		{
			ActivateTab(num);
		}
		else
		{
			ActivateTab(0);
		}
		tabHeaderContainer.gameObject.SetActive((CountTabs() > 1) ? true : false);
		if (sideScreens != null && sideScreens.Count > 0)
		{
			sideScreens.ForEach(delegate(SideScreenRef scn)
			{
				if (!scn.screenPrefab.IsValidForTarget(target))
				{
					if (scn.screenInstance != null && scn.screenInstance.gameObject.activeSelf)
					{
						scn.screenInstance.gameObject.SetActive(value: false);
					}
				}
				else
				{
					if (scn.screenInstance == null)
					{
						scn.screenInstance = Util.KInstantiateUI<SideScreenContent>(scn.screenPrefab.gameObject, sideScreenContentBody);
					}
					if (!sideScreen.activeInHierarchy)
					{
						sideScreen.SetActive(value: true);
					}
					scn.screenInstance.SetTarget(target);
					scn.screenInstance.Show();
					int sideScreenSortOrder = scn.screenInstance.GetSideScreenSortOrder();
					sortedSideScreens.Add(new KeyValuePair<GameObject, int>(scn.screenInstance.gameObject, sideScreenSortOrder));
					if (currentSideScreen == null || !currentSideScreen.gameObject.activeSelf || sideScreenSortOrder > sortedSideScreens.Find((KeyValuePair<GameObject, int> match) => match.Key == currentSideScreen.gameObject).Value)
					{
						currentSideScreen = scn.screenInstance;
					}
					RefreshTitle();
				}
			});
		}
		sortedSideScreens.Sort((KeyValuePair<GameObject, int> x, KeyValuePair<GameObject, int> y) => (x.Value <= y.Value) ? 1 : (-1));
		for (int k = 0; k < sortedSideScreens.Count; k++)
		{
			sortedSideScreens[k].Key.transform.SetSiblingIndex(k);
		}
	}

	public void RefreshTitle()
	{
		if ((bool)currentSideScreen)
		{
			sideScreenTitle.SetText(currentSideScreen.GetTitle());
		}
	}

	private void OnTabActivated(int newTab, int oldTab)
	{
		SetTitle(newTab);
		if (oldTab != -1)
		{
			screens[oldTab].screen.SetTarget(null);
		}
		if (newTab != -1)
		{
			screens[newTab].screen.SetTarget(target);
		}
	}

	public KScreen SetSecondarySideScreen(KScreen secondaryPrefab, string title)
	{
		ClearSecondarySideScreen();
		if (instantiatedSecondarySideScreens.ContainsKey(secondaryPrefab))
		{
			activeSideScreen2 = instantiatedSecondarySideScreens[secondaryPrefab];
			activeSideScreen2.gameObject.SetActive(value: true);
		}
		else
		{
			activeSideScreen2 = KScreenManager.Instance.InstantiateScreen(secondaryPrefab.gameObject, sideScreen2ContentBody);
			activeSideScreen2.Activate();
			instantiatedSecondarySideScreens.Add(secondaryPrefab, activeSideScreen2);
		}
		sideScreen2Title.text = title;
		sideScreen2.SetActive(value: true);
		return activeSideScreen2;
	}

	public void ClearSecondarySideScreen()
	{
		if (activeSideScreen2 != null)
		{
			activeSideScreen2.gameObject.SetActive(value: false);
			activeSideScreen2 = null;
		}
		sideScreen2.SetActive(value: false);
	}

	public void DeactivateSideContent()
	{
		if (SideDetailsScreen.Instance != null && SideDetailsScreen.Instance.gameObject.activeInHierarchy)
		{
			SideDetailsScreen.Instance.Show(show: false);
		}
		if (sideScreens != null && sideScreens.Count > 0)
		{
			sideScreens.ForEach(delegate(SideScreenRef scn)
			{
				if (scn.screenInstance != null)
				{
					scn.screenInstance.ClearTarget();
					scn.screenInstance.Show(show: false);
				}
			});
		}
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
		sideScreen.SetActive(value: false);
	}

	public void MaskSideContent(bool hide)
	{
		if (hide)
		{
			sideScreen.transform.localScale = Vector3.zero;
		}
		else
		{
			sideScreen.transform.localScale = Vector3.one;
		}
	}

	private string GetSelectedObjectCodexID()
	{
		string text = "";
		Debug.Assert(target != null, "Details Screen has no target");
		KSelectable component = target.GetComponent<KSelectable>();
		Debug.Assert(component != null, $"Details Screen target is not a KSelectable {target}");
		CellSelectionObject component2 = component.GetComponent<CellSelectionObject>();
		BuildingUnderConstruction component3 = component.GetComponent<BuildingUnderConstruction>();
		CreatureBrain component4 = component.GetComponent<CreatureBrain>();
		PlantableSeed component5 = component.GetComponent<PlantableSeed>();
		BudUprootedMonitor component6 = component.GetComponent<BudUprootedMonitor>();
		if (component2 != null)
		{
			text = CodexCache.FormatLinkID(component2.element.id.ToString());
		}
		else if (component3 != null)
		{
			text = CodexCache.FormatLinkID(component3.Def.PrefabID);
		}
		else if (component4 != null)
		{
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
			text = text.Replace("BABY", "");
		}
		else if (component5 != null)
		{
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
			text = text.Replace("SEED", "");
		}
		else if (component6 != null)
		{
			if (component6.parentObject.Get() != null)
			{
				text = CodexCache.FormatLinkID(component6.parentObject.Get().PrefabID().ToString());
			}
			else if (component6.GetComponent<TreeBud>() != null)
			{
				text = CodexCache.FormatLinkID(component6.GetComponent<TreeBud>().buddingTrunk.Get().PrefabID().ToString());
			}
		}
		else
		{
			text = CodexCache.FormatLinkID(component.PrefabID().ToString());
		}
		if (CodexCache.entries.ContainsKey(text) || CodexCache.FindSubEntry(text) != null)
		{
			return text;
		}
		return "";
	}

	public void OpenCodexEntry()
	{
		string selectedObjectCodexID = GetSelectedObjectCodexID();
		if (selectedObjectCodexID != "")
		{
			ManagementMenu.Instance.OpenCodexToEntry(selectedObjectCodexID);
		}
	}

	public void DeselectAndClose()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back"));
		SelectTool.Instance.Select(null);
		ClusterMapSelectTool.Instance.Select(null);
		if (!(target == null))
		{
			target = null;
			DeactivateSideContent();
			Show(show: false);
		}
	}

	private void SortScreenOrder()
	{
		Array.Sort(screens, (Screens x, Screens y) => x.displayOrderPriority.CompareTo(y.displayOrderPriority));
	}

	public void UpdatePortrait(GameObject target)
	{
		KSelectable component = target.GetComponent<KSelectable>();
		if (component == null)
		{
			return;
		}
		TabTitle.portrait.ClearPortrait();
		Building component2 = component.GetComponent<Building>();
		if ((bool)component2)
		{
			Sprite sprite = null;
			sprite = component2.Def.GetUISprite();
			if (sprite != null)
			{
				TabTitle.portrait.SetPortrait(sprite);
				return;
			}
		}
		if ((bool)target.GetComponent<MinionIdentity>())
		{
			TabTitle.SetPortrait(component.gameObject);
			return;
		}
		Edible component3 = target.GetComponent<Edible>();
		if (component3 != null)
		{
			Sprite uISpriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component3.GetComponent<KBatchedAnimController>().AnimFiles[0]);
			TabTitle.portrait.SetPortrait(uISpriteFromMultiObjectAnim);
			return;
		}
		PrimaryElement component4 = target.GetComponent<PrimaryElement>();
		if (component4 != null)
		{
			TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(ElementLoader.FindElementByHash(component4.ElementID).substance.anim));
			return;
		}
		CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
		if (component5 != null)
		{
			string animName = (component5.element.IsSolid ? "ui" : component5.element.substance.name);
			Sprite uISpriteFromMultiObjectAnim2 = Def.GetUISpriteFromMultiObjectAnim(component5.element.substance.anim, animName);
			TabTitle.portrait.SetPortrait(uISpriteFromMultiObjectAnim2);
		}
	}

	public bool CompareTargetWith(GameObject compare)
	{
		return target == compare;
	}

	public void SetTitle(int selectedTabIndex)
	{
		UpdateCodexButton();
		if (TabTitle != null)
		{
			TabTitle.SetTitle(target.GetProperName());
			MinionIdentity minionIdentity = null;
			UserNameable x = null;
			if (target != null)
			{
				minionIdentity = target.gameObject.GetComponent<MinionIdentity>();
				x = target.gameObject.GetComponent<UserNameable>();
			}
			if (minionIdentity != null)
			{
				TabTitle.SetSubText(minionIdentity.GetComponent<MinionResume>().GetSkillsSubtitle());
				TabTitle.SetUserEditable(editable: true);
			}
			else if (x != null)
			{
				TabTitle.SetSubText("");
				TabTitle.SetUserEditable(editable: true);
			}
			else
			{
				TabTitle.SetSubText("");
				TabTitle.SetUserEditable(editable: false);
			}
			TabTitle.UpdateRenameTooltip(target);
		}
	}

	public void SetTitle(string title)
	{
		TabTitle.SetTitle(title);
	}

	public TargetScreen GetActiveTab()
	{
		if (previouslyActiveTab >= 0 && previouslyActiveTab < screens.Length)
		{
			return screens[previouslyActiveTab].screen;
		}
		return null;
	}
}
