using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen
{
	[DebuggerDisplay("{item.item.Name}")]
	public class StatusItemEntry : IRenderEveryTick
	{
		private enum FadeStage
		{
			IN,
			WAIT,
			OUT
		}

		public StatusItemGroup.Entry item;

		public StatusItemCategory category;

		private LayoutElement spacerLayout;

		private GameObject widget;

		private ToolTip toolTip;

		private TextStyleSetting tooltipStyle;

		public Action<StatusItemEntry> onDestroy;

		private Image image;

		private LocText text;

		private KButton button;

		public Color color;

		public TextStyleSetting style;

		private FadeStage fadeStage;

		private float fade;

		private float fadeInTime;

		private float fadeOutTime = 1.8f;

		public Image GetImage => image;

		public StatusItemEntry(StatusItemGroup.Entry item, StatusItemCategory category, GameObject status_item_prefab, Transform parent, TextStyleSetting tooltip_style, Color color, TextStyleSetting style, bool skip_fade, Action<StatusItemEntry> onDestroy)
		{
			this.item = item;
			this.category = category;
			tooltipStyle = tooltip_style;
			this.onDestroy = onDestroy;
			this.color = color;
			this.style = style;
			widget = Util.KInstantiateUI(status_item_prefab, parent.gameObject);
			text = widget.GetComponentInChildren<LocText>(includeInactive: true);
			SetTextStyleSetting.ApplyStyle(text, style);
			toolTip = widget.GetComponentInChildren<ToolTip>(includeInactive: true);
			image = widget.GetComponentInChildren<Image>(includeInactive: true);
			item.SetIcon(image);
			widget.SetActive(value: true);
			toolTip.OnToolTip = OnToolTip;
			button = widget.GetComponentInChildren<KButton>();
			if (item.item.statusItemClickCallback != null)
			{
				button.onClick += OnClick;
			}
			else
			{
				button.enabled = false;
			}
			fadeStage = (skip_fade ? FadeStage.WAIT : FadeStage.IN);
			SimAndRenderScheduler.instance.Add(this);
			Refresh();
			SetColor();
		}

		internal void SetSprite(TintedSprite sprite)
		{
			if (sprite != null)
			{
				image.sprite = sprite.sprite;
			}
		}

		public int GetIndex()
		{
			return widget.transform.GetSiblingIndex();
		}

		public void SetIndex(int index)
		{
			widget.transform.SetSiblingIndex(index);
		}

		public void RenderEveryTick(float dt)
		{
			switch (fadeStage)
			{
			case FadeStage.IN:
			{
				fade = Mathf.Min(fade + Time.deltaTime / fadeInTime, 1f);
				float num2 = fade;
				SetColor(num2);
				if (fade >= 1f)
				{
					fadeStage = FadeStage.WAIT;
				}
				break;
			}
			case FadeStage.OUT:
			{
				float num = fade;
				SetColor(num);
				fade = Mathf.Max(fade - Time.deltaTime / fadeOutTime, 0f);
				if (fade <= 0f)
				{
					Destroy(immediate: true);
				}
				break;
			}
			case FadeStage.WAIT:
				break;
			}
		}

		private string OnToolTip()
		{
			item.ShowToolTip(toolTip, tooltipStyle);
			return "";
		}

		private void OnClick()
		{
			item.OnClick();
		}

		public void Refresh()
		{
			string name = item.GetName();
			if (name != text.text)
			{
				text.text = name;
				SetColor();
			}
		}

		private void SetColor(float alpha = 1f)
		{
			Color color = new Color(this.color.r, this.color.g, this.color.b, alpha);
			image.color = color;
			text.color = color;
		}

		public void Destroy(bool immediate)
		{
			if (immediate)
			{
				if (onDestroy != null)
				{
					onDestroy(this);
				}
				SimAndRenderScheduler.instance.Remove(this);
				toolTip.OnToolTip = null;
				UnityEngine.Object.Destroy(widget);
			}
			else
			{
				fade = 0.5f;
				fadeStage = FadeStage.OUT;
			}
		}
	}

	public GameObject attributesLabelTemplate;

	public GameObject attributesLabelButtonTemplate;

	public GameObject DescriptionContainerTemplate;

	private DescriptionContainer descriptionContainer;

	public GameObject StampContainerTemplate;

	public GameObject StampPrefab;

	public GameObject VitalsPanelTemplate;

	public Sprite DefaultPortraitIcon;

	public Text StatusPanelCurrentActionLabel;

	public GameObject StatusItemPrefab;

	public Sprite statusWarningIcon;

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private GameObject storagePanel;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private GameObject stressPanel;

	private DetailsPanelDrawer stressDrawer;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public TextStyleSetting StatusItemStyle_Main;

	public TextStyleSetting StatusItemStyle_Other;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_bad = new Color(244f / 255f, 74f / 255f, 71f / 255f);

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private bool TargetIsMinion;

	private List<StatusItemEntry> statusItems = new List<StatusItemEntry>();

	private List<StatusItemEntry> oldStatusItems = new List<StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private Action<object> onStorageChangeDelegate;

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	public SimpleInfoScreen()
	{
		onStorageChangeDelegate = OnStorageChange;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		statusItemPanel.scalerMask.hoverLock = true;
		statusItemsFolder = statusItemPanel.Content.gameObject;
		vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		vitalsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		vitalsContainer = Util.KInstantiateUI(VitalsPanelTemplate, vitalsPanel.Content.gameObject).GetComponent<MinionVitalsPanel>();
		fertilityPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		fertilityPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject gameObject = infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(DescriptionContainerTemplate, gameObject);
		storagePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		stressDrawer = new DetailsPanelDrawer(attributesLabelTemplate, stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		stampContainer = Util.KInstantiateUI(StampContainerTemplate, gameObject);
		Subscribe(-1514841199, OnRefreshDataDelegate);
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		Subscribe(target, -1697596308, onStorageChangeDelegate);
		Subscribe(target, -1197125120, onStorageChangeDelegate);
		RefreshStorage();
		Subscribe(target, 1059811075, OnBreedingChanceChanged);
		RefreshBreedingChance();
		vitalsPanel.SetTitle((target.GetComponent<WiltCondition>() == null) ? UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION : UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS);
		KSelectable component = target.GetComponent<KSelectable>();
		if (component != null)
		{
			StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
			if (statusItemGroup != null)
			{
				statusItemGroup.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Combine(statusItemGroup.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(OnAddStatusItem));
				statusItemGroup.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Combine(statusItemGroup.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(OnRemoveStatusItem));
				foreach (StatusItemGroup.Entry item in statusItemGroup)
				{
					if (item.category != null && item.category.Id == "Main")
					{
						DoAddStatusItem(item, item.category);
					}
				}
				foreach (StatusItemGroup.Entry item2 in statusItemGroup)
				{
					if (item2.category == null || item2.category.Id != "Main")
					{
						DoAddStatusItem(item2, item2.category);
					}
				}
			}
		}
		statusItemPanel.gameObject.SetActive(value: true);
		statusItemPanel.scalerMask.UpdateSize();
		Refresh(force: true);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
		if (target != null)
		{
			Unsubscribe(target, -1697596308, onStorageChangeDelegate);
			Unsubscribe(target, -1197125120, onStorageChangeDelegate);
			Unsubscribe(target, 1059811075, OnBreedingChanceChanged);
		}
		KSelectable component = target.GetComponent<KSelectable>();
		if (!(component != null))
		{
			return;
		}
		StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
		if (statusItemGroup == null)
		{
			return;
		}
		statusItemGroup.OnAddStatusItem = (Action<StatusItemGroup.Entry, StatusItemCategory>)Delegate.Remove(statusItemGroup.OnAddStatusItem, new Action<StatusItemGroup.Entry, StatusItemCategory>(OnAddStatusItem));
		statusItemGroup.OnRemoveStatusItem = (Action<StatusItemGroup.Entry, bool>)Delegate.Remove(statusItemGroup.OnRemoveStatusItem, new Action<StatusItemGroup.Entry, bool>(OnRemoveStatusItem));
		foreach (StatusItemEntry statusItem in statusItems)
		{
			statusItem.Destroy(immediate: true);
		}
		statusItems.Clear();
		foreach (StatusItemEntry oldStatusItem in oldStatusItems)
		{
			oldStatusItem.onDestroy = null;
			oldStatusItem.Destroy(immediate: true);
		}
		oldStatusItems.Clear();
	}

	private void OnStorageChange(object data)
	{
		RefreshStorage();
	}

	private void OnBreedingChanceChanged(object data)
	{
		RefreshBreedingChance();
	}

	private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category)
	{
		DoAddStatusItem(status_item, category);
	}

	private void DoAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category, bool show_immediate = false)
	{
		GameObject gameObject = statusItemsFolder;
		Color color = ((status_item.item.notificationType != NotificationType.BadMinor && status_item.item.notificationType != NotificationType.Bad && status_item.item.notificationType != NotificationType.DuplicantThreatening) ? statusItemTextColor_regular : statusItemTextColor_bad);
		TextStyleSetting style = ((category == Db.Get().StatusItemCategories.Main) ? StatusItemStyle_Main : StatusItemStyle_Other);
		StatusItemEntry statusItemEntry = new StatusItemEntry(status_item, category, StatusItemPrefab, gameObject.transform, ToolTipStyle_Property, color, style, show_immediate, OnStatusItemDestroy);
		statusItemEntry.SetSprite(status_item.item.sprite);
		if (category != null)
		{
			int num = -1;
			foreach (StatusItemEntry item in oldStatusItems.FindAll((StatusItemEntry e) => e.category == category))
			{
				num = item.GetIndex();
				item.Destroy(immediate: true);
				oldStatusItems.Remove(item);
			}
			if (category == Db.Get().StatusItemCategories.Main)
			{
				num = 0;
			}
			if (num != -1)
			{
				statusItemEntry.SetIndex(num);
			}
		}
		statusItems.Add(statusItemEntry);
	}

	private void OnRemoveStatusItem(StatusItemGroup.Entry status_item, bool immediate = false)
	{
		DoRemoveStatusItem(status_item, immediate);
	}

	private void DoRemoveStatusItem(StatusItemGroup.Entry status_item, bool destroy_immediate = false)
	{
		for (int i = 0; i < statusItems.Count; i++)
		{
			if (statusItems[i].item.item == status_item.item)
			{
				StatusItemEntry statusItemEntry = statusItems[i];
				statusItems.RemoveAt(i);
				oldStatusItems.Add(statusItemEntry);
				statusItemEntry.Destroy(destroy_immediate);
				break;
			}
		}
	}

	private void OnStatusItemDestroy(StatusItemEntry item)
	{
		oldStatusItems.Remove(item);
	}

	private void Update()
	{
		Refresh();
	}

	private void OnRefreshData(object obj)
	{
		Refresh();
	}

	public void Refresh(bool force = false)
	{
		if (selectedTarget != lastTarget || force)
		{
			lastTarget = selectedTarget;
			if (selectedTarget != null)
			{
				SetPanels(selectedTarget);
				SetStamps(selectedTarget);
			}
		}
		int count = statusItems.Count;
		statusItemPanel.gameObject.SetActive(count > 0);
		for (int i = 0; i < count; i++)
		{
			statusItems[i].Refresh();
		}
		if (vitalsContainer.isActiveAndEnabled)
		{
			vitalsContainer.Refresh();
		}
		RefreshStress();
		RefreshStorage();
	}

	private void SetPanels(GameObject target)
	{
		MinionIdentity component = target.GetComponent<MinionIdentity>();
		Amounts amounts = target.GetAmounts();
		PrimaryElement component2 = target.GetComponent<PrimaryElement>();
		BuildingComplete component3 = target.GetComponent<BuildingComplete>();
		BuildingUnderConstruction component4 = target.GetComponent<BuildingUnderConstruction>();
		CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
		InfoDescription component6 = target.GetComponent<InfoDescription>();
		Edible component7 = target.GetComponent<Edible>();
		attributeLabels.ForEach(delegate(LocText x)
		{
			UnityEngine.Object.Destroy(x.gameObject);
		});
		attributeLabels.Clear();
		vitalsPanel.gameObject.SetActive(amounts != null);
		string text = "";
		string text2 = "";
		if (amounts != null)
		{
			vitalsContainer.selectedEntity = selectedTarget;
			Uprootable component8 = selectedTarget.gameObject.GetComponent<Uprootable>();
			if (component8 != null)
			{
				vitalsPanel.gameObject.SetActive(component8.GetPlanterStorage != null);
			}
			if (selectedTarget.gameObject.GetComponent<WiltCondition>() != null)
			{
				vitalsPanel.gameObject.SetActive(value: true);
			}
		}
		if ((bool)component)
		{
			text = "";
		}
		else if ((bool)component6)
		{
			text = component6.description;
		}
		else if (component3 != null)
		{
			text = component3.Def.Effect;
			text2 = component3.Def.Desc;
		}
		else if (component4 != null)
		{
			text = component4.Def.Effect;
			text2 = component4.Def.Desc;
		}
		else if (component7 != null)
		{
			EdiblesManager.FoodInfo foodInfo = component7.FoodInfo;
			text += string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit));
		}
		else if (component5 != null)
		{
			text = component5.element.FullDescription(addHardnessColor: false);
		}
		else if (component2 != null)
		{
			Element element = ElementLoader.FindElementByHash(component2.ElementID);
			text = ((element != null) ? element.FullDescription(addHardnessColor: false) : "");
		}
		List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(target, simpleInfoScreen: true);
		bool flag = gameObjectEffects.Count > 0;
		descriptionContainer.gameObject.SetActive(flag);
		descriptionContainer.descriptors.gameObject.SetActive(flag);
		if (flag)
		{
			descriptionContainer.descriptors.SetDescriptors(gameObjectEffects);
		}
		descriptionContainer.description.text = text;
		descriptionContainer.flavour.text = text2;
		infoPanel.gameObject.SetActive(component == null);
		descriptionContainer.gameObject.SetActive(infoPanel.activeSelf);
		descriptionContainer.flavour.gameObject.SetActive(text2 != "" && text2 != "\n");
		if (vitalsPanel.gameObject.activeSelf && amounts.Count == 0)
		{
			vitalsPanel.gameObject.SetActive(value: false);
		}
	}

	private void RefreshBreedingChance()
	{
		if (selectedTarget == null)
		{
			fertilityPanel.gameObject.SetActive(value: false);
			return;
		}
		FertilityMonitor.Instance sMI = selectedTarget.GetSMI<FertilityMonitor.Instance>();
		if (sMI == null)
		{
			fertilityPanel.gameObject.SetActive(value: false);
			return;
		}
		int num = 0;
		foreach (FertilityMonitor.BreedingChance breedingChance in sMI.breedingChances)
		{
			List<FertilityModifier> forTag = Db.Get().FertilityModifiers.GetForTag(breedingChance.egg);
			if (forTag.Count > 0)
			{
				string text = "";
				foreach (FertilityModifier item in forTag)
				{
					text += string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_MOD_FORMAT, item.GetTooltip());
				}
				fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f), text));
			}
			else
			{
				fertilityPanel.SetLabel("breeding_" + num++, string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f)), string.Format(UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, breedingChance.egg.ProperName(), GameUtil.GetFormattedPercent(breedingChance.weight * 100f)));
			}
		}
		fertilityPanel.Commit();
	}

	private void RefreshStorage()
	{
		if (selectedTarget == null)
		{
			storagePanel.gameObject.SetActive(value: false);
			return;
		}
		Storage[] componentsInChildren = selectedTarget.GetComponentsInChildren<Storage>();
		if (componentsInChildren == null)
		{
			storagePanel.gameObject.SetActive(value: false);
			return;
		}
		componentsInChildren = Array.FindAll(componentsInChildren, (Storage n) => n.showInUI);
		if (componentsInChildren.Length == 0)
		{
			storagePanel.gameObject.SetActive(value: false);
			return;
		}
		storagePanel.gameObject.SetActive(value: true);
		string text = ((selectedTarget.GetComponent<MinionIdentity>() != null) ? UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS);
		storagePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = text;
		foreach (KeyValuePair<string, GameObject> storageLabel in storageLabels)
		{
			storageLabel.Value.SetActive(value: false);
		}
		int num = 0;
		Storage[] array = componentsInChildren;
		foreach (Storage storage in array)
		{
			foreach (GameObject item in storage.items)
			{
				if (item == null)
				{
					continue;
				}
				GameObject gameObject = AddOrGetStorageLabel(storageLabels, storagePanel, "storage_" + num);
				num++;
				if (storage.allowUIItemRemoval)
				{
					Transform transform = gameObject.transform.Find("removeAttributeButton");
					if (transform != null)
					{
						KButton component = transform.GetComponent<KButton>();
						component.enabled = true;
						component.gameObject.SetActive(value: true);
						GameObject select_item = item;
						Storage selected_storage = storage;
						component.onClick += delegate
						{
							selected_storage.Drop(select_item);
						};
					}
				}
				PrimaryElement component2 = item.GetComponent<PrimaryElement>();
				Rottable.Instance sMI = item.GetSMI<Rottable.Instance>();
				gameObject.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
				string unitFormattedName = GameUtil.GetUnitFormattedName(item);
				unitFormattedName = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, unitFormattedName, GameUtil.GetFormattedMass(component2.Mass));
				unitFormattedName = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, unitFormattedName, GameUtil.GetFormattedTemperature(component2.Temperature));
				if (sMI != null)
				{
					string text2 = sMI.StateString();
					if (!string.IsNullOrEmpty(text2))
					{
						unitFormattedName += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, text2);
					}
					gameObject.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(sMI.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting);
				}
				if (component2.DiseaseIdx != byte.MaxValue)
				{
					unitFormattedName += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount));
					string formattedDisease = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, color: true);
					gameObject.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting);
				}
				gameObject.GetComponentInChildren<LocText>().text = unitFormattedName;
				KButton component3 = gameObject.GetComponent<KButton>();
				GameObject select_target = item;
				component3.onClick += delegate
				{
					SelectTool.Instance.Select(select_target.GetComponent<KSelectable>());
				};
			}
		}
		if (num == 0)
		{
			AddOrGetStorageLabel(storageLabels, storagePanel, "empty").GetComponentInChildren<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject = null;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
			gameObject.GetComponent<KButton>().ClearOnClick();
			Transform transform = gameObject.transform.Find("removeAttributeButton");
			if (transform != null)
			{
				KButton kButton = transform.FindComponent<KButton>();
				kButton.enabled = false;
				kButton.gameObject.SetActive(value: false);
				kButton.ClearOnClick();
			}
		}
		else
		{
			gameObject = Util.KInstantiate(attributesLabelButtonTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private void RefreshStress()
	{
		MinionIdentity identity = ((selectedTarget != null) ? selectedTarget.GetComponent<MinionIdentity>() : null);
		if (identity == null)
		{
			stressPanel.SetActive(value: false);
			return;
		}
		List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
		stressPanel.SetActive(value: true);
		stressPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.STATS.GROUPNAME_STRESS;
		ReportManager.ReportEntry reportEntry = ReportManager.Instance.TodaysReport.reportEntries.Find((ReportManager.ReportEntry entry) => entry.reportType == ReportManager.ReportType.StressDelta);
		stressDrawer.BeginDrawing();
		float num = 0f;
		stressNotes.Clear();
		int num2 = reportEntry.contextEntries.FindIndex((ReportManager.ReportEntry entry) => entry.context == identity.GetProperName());
		ReportManager.ReportEntry reportEntry2 = ((num2 != -1) ? reportEntry.contextEntries[num2] : null);
		if (reportEntry2 != null)
		{
			reportEntry2.IterateNotes(delegate(ReportManager.ReportEntry.Note note)
			{
				stressNotes.Add(note);
			});
			stressNotes.Sort((ReportManager.ReportEntry.Note a, ReportManager.ReportEntry.Note b) => a.value.CompareTo(b.value));
			for (int i = 0; i < stressNotes.Count; i++)
			{
				stressDrawer.NewLabel(((stressNotes[i].value > 0f) ? UIConstants.ColorPrefixRed : "") + stressNotes[i].note + ": " + Util.FormatTwoDecimalPlace(stressNotes[i].value) + "%" + ((stressNotes[i].value > 0f) ? UIConstants.ColorSuffix : ""));
				num += stressNotes[i].value;
			}
		}
		stressDrawer.NewLabel(((num > 0f) ? UIConstants.ColorPrefixRed : "") + string.Format(UI.DETAILTABS.DETAILS.NET_STRESS, Util.FormatTwoDecimalPlace(num)) + ((num > 0f) ? UIConstants.ColorSuffix : ""));
		stressDrawer.EndDrawing();
	}

	private void ShowAttributes(GameObject target)
	{
		Attributes attributes = target.GetAttributes();
		if (attributes == null)
		{
			return;
		}
		List<AttributeInstance> list = attributes.AttributeTable.FindAll((AttributeInstance a) => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.General);
		if (list.Count <= 0)
		{
			return;
		}
		descriptionContainer.descriptors.gameObject.SetActive(value: true);
		List<Descriptor> list2 = new List<Descriptor>();
		foreach (AttributeInstance item2 in list)
		{
			Descriptor item = new Descriptor($"{item2.Name}: {item2.GetFormattedValue()}", item2.GetAttributeValueTooltip());
			item.IncreaseIndent();
			list2.Add(item);
		}
		descriptionContainer.descriptors.SetDescriptors(list2);
	}

	private void SetStamps(GameObject target)
	{
		for (int i = 0; i < stampContainer.transform.childCount; i++)
		{
			UnityEngine.Object.Destroy(stampContainer.transform.GetChild(i).gameObject);
		}
		_ = target.GetComponent<BuildingComplete>() != null;
	}
}
