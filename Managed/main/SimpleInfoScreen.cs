using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen, ISim4000ms, ISim1000ms
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

	private RocketSimpleInfoPanel rocketSimpleInfoPanel;

	private SpacePOISimpleInfoPanel spaceSimpleInfoPOIPanel;

	[SerializeField]
	private HierarchyReferences processConditionHeader;

	[SerializeField]
	private GameObject processConditionRow;

	private CollapsibleDetailContentPanel statusItemPanel;

	private CollapsibleDetailContentPanel vitalsPanel;

	private CollapsibleDetailContentPanel fertilityPanel;

	private CollapsibleDetailContentPanel rocketStatusContainer;

	private CollapsibleDetailContentPanel worldLifePanel;

	private CollapsibleDetailContentPanel worldElementsPanel;

	private CollapsibleDetailContentPanel worldBiomesPanel;

	private CollapsibleDetailContentPanel worldGeysersPanel;

	private CollapsibleDetailContentPanel spacePOIPanel;

	private CollapsibleDetailContentPanel worldTraitsPanel;

	[SerializeField]
	public GameObject iconLabelRow;

	[SerializeField]
	public GameObject bigIconLabelRow;

	private Dictionary<Tag, GameObject> lifeformRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> biomeRows = new Dictionary<Tag, GameObject>();

	private Dictionary<Tag, GameObject> geyserRows = new Dictionary<Tag, GameObject>();

	private List<GameObject> worldTraitRows = new List<GameObject>();

	[SerializeField]
	public GameObject spacerRow;

	private GameObject infoPanel;

	private GameObject stampContainer;

	private MinionVitalsPanel vitalsContainer;

	private GameObject InfoFolder;

	private GameObject statusItemsFolder;

	public GameObject TextContainerPrefab;

	private GameObject processConditionContainer;

	private GameObject stressPanel;

	private DetailsPanelDrawer stressDrawer;

	private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();

	public TextStyleSetting ToolTipStyle_Property;

	public TextStyleSetting StatusItemStyle_Main;

	public TextStyleSetting StatusItemStyle_Other;

	public Color statusItemTextColor_regular = Color.black;

	public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);

	private GameObject lastTarget;

	private bool TargetIsMinion;

	private List<StatusItemEntry> statusItems = new List<StatusItemEntry>();

	private List<StatusItemEntry> oldStatusItems = new List<StatusItemEntry>();

	private List<LocText> attributeLabels = new List<LocText>();

	private List<GameObject> processConditionRows = new List<GameObject>();

	private Action<object> onStorageChangeDelegate;

	private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>(delegate(SimpleInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});

	public GameObject StoragePanel { get; private set; }

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
		processConditionContainer = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.PROCESS_CONDITIONS.NAME;
		statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		statusItemPanel.Content.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
		statusItemPanel.HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
		statusItemPanel.scalerMask.hoverLock = true;
		statusItemsFolder = statusItemPanel.Content.gameObject;
		spaceSimpleInfoPOIPanel = new SpacePOISimpleInfoPanel(this);
		spacePOIPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		rocketSimpleInfoPanel = new RocketSimpleInfoPanel(this);
		rocketStatusContainer = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		rocketStatusContainer.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ROCKET);
		vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		vitalsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
		vitalsContainer = Util.KInstantiateUI(VitalsPanelTemplate, vitalsPanel.Content.gameObject).GetComponent<MinionVitalsPanel>();
		fertilityPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		fertilityPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
		infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
		GameObject parent = infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject;
		descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(DescriptionContainerTemplate, parent);
		worldLifePanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		worldLifePanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_LIFE);
		worldTraitsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		worldTraitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.SIMPLEINFO.GROUPNAME_WORLDTRAITS;
		worldElementsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		worldElementsPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ELEMENTS);
		worldGeysersPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		worldGeysersPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_GEYSERS);
		worldBiomesPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		worldBiomesPanel.SetTitle(UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES);
		StoragePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject);
		stressDrawer = new DetailsPanelDrawer(attributesLabelTemplate, stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		stampContainer = Util.KInstantiateUI(StampContainerTemplate, parent);
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
		RefreshWorld();
		spaceSimpleInfoPOIPanel.Refresh(spacePOIPanel, selectedTarget);
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
		Color color = ((status_item.item.notificationType == NotificationType.BadMinor || status_item.item.notificationType == NotificationType.Bad || status_item.item.notificationType == NotificationType.DuplicantThreatening) ? ((Color)GlobalAssets.Instance.colorSet.statusItemBad) : ((status_item.item.notificationType != NotificationType.Event) ? statusItemTextColor_regular : ((Color)GlobalAssets.Instance.colorSet.statusItemEvent)));
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
		rocketSimpleInfoPanel.Refresh(rocketStatusContainer, selectedTarget);
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
		IProcessConditionSet component8 = target.GetComponent<IProcessConditionSet>();
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
			Uprootable component9 = selectedTarget.gameObject.GetComponent<Uprootable>();
			if (component9 != null)
			{
				vitalsPanel.gameObject.SetActive(component9.GetPlanterStorage != null);
			}
			if (selectedTarget.gameObject.GetComponent<WiltCondition>() != null)
			{
				vitalsPanel.gameObject.SetActive(value: true);
			}
		}
		if (component8 != null)
		{
			processConditionContainer.SetActive(value: true);
			RefreshProcessConditions();
		}
		else
		{
			processConditionContainer.SetActive(value: false);
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
		bool flag2 = text.IsNullOrWhiteSpace() && text2.IsNullOrWhiteSpace() && !flag;
		infoPanel.gameObject.SetActive(component == null && !flag2);
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
			StoragePanel.gameObject.SetActive(value: false);
			return;
		}
		IStorage[] componentsInChildren = selectedTarget.GetComponentsInChildren<IStorage>();
		if (componentsInChildren == null)
		{
			StoragePanel.gameObject.SetActive(value: false);
			return;
		}
		componentsInChildren = Array.FindAll(componentsInChildren, (IStorage n) => n.ShouldShowInUI());
		if (componentsInChildren.Length == 0)
		{
			StoragePanel.gameObject.SetActive(value: false);
			return;
		}
		StoragePanel.gameObject.SetActive(value: true);
		string text = ((selectedTarget.GetComponent<MinionIdentity>() != null) ? UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS : UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS);
		StoragePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = text;
		foreach (KeyValuePair<string, GameObject> storageLabel in storageLabels)
		{
			storageLabel.Value.SetActive(value: false);
		}
		int num = 0;
		IStorage[] array = componentsInChildren;
		foreach (IStorage storage in array)
		{
			ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();
			foreach (GameObject item in storage.GetItems())
			{
				if (item == null)
				{
					continue;
				}
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (component != null && component.Mass == 0f)
				{
					continue;
				}
				Rottable.Instance sMI = item.GetSMI<Rottable.Instance>();
				HighEnergyParticleStorage component2 = item.GetComponent<HighEnergyParticleStorage>();
				string text2 = "";
				pooledList.Clear();
				if (component != null && component2 == null)
				{
					text2 = GameUtil.GetUnitFormattedName(item);
					text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text2, GameUtil.GetFormattedMass(component.Mass));
					text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, text2, GameUtil.GetFormattedTemperature(component.Temperature));
				}
				if (component2 != null)
				{
					text2 = ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME;
					text2 = string.Format(UI.DETAILTABS.DETAILS.CONTENTS_MASS, text2, GameUtil.GetFormattedHighEnergyParticles(component2.Particles));
				}
				if (sMI != null)
				{
					string text3 = sMI.StateString();
					if (!string.IsNullOrEmpty(text3))
					{
						text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, text3);
					}
					pooledList.Add(new Tuple<string, TextStyleSetting>(sMI.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting));
				}
				if (component.DiseaseIdx != byte.MaxValue)
				{
					text2 += string.Format(UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, GameUtil.GetFormattedDisease(component.DiseaseIdx, component.DiseaseCount));
					string formattedDisease = GameUtil.GetFormattedDisease(component.DiseaseIdx, component.DiseaseCount, color: true);
					pooledList.Add(new Tuple<string, TextStyleSetting>(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting));
				}
				GameObject gameObject = AddOrGetStorageLabel(storageLabels, StoragePanel, "storage_" + num);
				num++;
				gameObject.GetComponentInChildren<LocText>().text = text2;
				gameObject.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
				foreach (Tuple<string, TextStyleSetting> item2 in pooledList)
				{
					gameObject.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(item2.first, item2.second);
				}
				KButton component3 = gameObject.GetComponent<KButton>();
				GameObject select_target = item;
				component3.onClick += delegate
				{
					SelectTool.Instance.Select(select_target.GetComponent<KSelectable>());
				};
				if (!storage.allowUIItemRemoval)
				{
					continue;
				}
				Transform transform = gameObject.transform.Find("removeAttributeButton");
				if (transform != null)
				{
					KButton component4 = transform.GetComponent<KButton>();
					component4.enabled = true;
					component4.gameObject.SetActive(value: true);
					GameObject select_item = item;
					IStorage selected_storage = storage;
					component4.onClick += delegate
					{
						selected_storage.Drop(select_item);
					};
				}
			}
			pooledList.Recycle();
		}
		if (num == 0)
		{
			AddOrGetStorageLabel(storageLabels, StoragePanel, "empty").GetComponentInChildren<LocText>().text = UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
		}
	}

	private void CreateWorldTraitRow()
	{
		GameObject gameObject = Util.KInstantiateUI(iconLabelRow, worldTraitsPanel.Content.gameObject, force_active: true);
		worldTraitRows.Add(gameObject);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").gameObject.SetActive(value: false);
		component.GetReference<LocText>("ValueLabel").gameObject.SetActive(value: false);
	}

	private void RefreshWorld()
	{
		WorldContainer worldContainer = ((selectedTarget == null) ? null : selectedTarget.GetComponent<WorldContainer>());
		AsteroidGridEntity asteroidGridEntity = ((selectedTarget == null) ? null : selectedTarget.GetComponent<AsteroidGridEntity>());
		bool flag = worldContainer != null && asteroidGridEntity != null;
		worldBiomesPanel.gameObject.SetActive(flag);
		worldGeysersPanel.gameObject.SetActive(flag);
		worldTraitsPanel.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		foreach (KeyValuePair<Tag, GameObject> biomeRow in biomeRows)
		{
			biomeRow.Value.SetActive(value: false);
		}
		if (worldContainer.Biomes != null)
		{
			foreach (string biome in worldContainer.Biomes)
			{
				Sprite biomeSprite = GameUtil.GetBiomeSprite(biome);
				if (!biomeRows.ContainsKey(biome))
				{
					biomeRows.Add(biome, Util.KInstantiateUI(bigIconLabelRow, worldBiomesPanel.Content.gameObject, force_active: true));
					HierarchyReferences component = biomeRows[biome].GetComponent<HierarchyReferences>();
					component.GetReference<Image>("Icon").sprite = biomeSprite;
					component.GetReference<LocText>("NameLabel").SetText(UI.FormatAsLink(Strings.Get("STRINGS.SUBWORLDS." + biome.ToUpper() + ".NAME"), "BIOME" + biome.ToUpper()));
					component.GetReference<LocText>("DescriptionLabel").SetText(Strings.Get("STRINGS.SUBWORLDS." + biome.ToUpper() + ".DESC"));
				}
				biomeRows[biome].SetActive(value: true);
			}
		}
		else
		{
			worldBiomesPanel.gameObject.SetActive(value: false);
		}
		List<Tag> list = new List<Tag>();
		Geyser[] array = UnityEngine.Object.FindObjectsOfType<Geyser>();
		foreach (Geyser geyser in array)
		{
			if (geyser.GetMyWorldId() == worldContainer.id)
			{
				list.Add(geyser.PrefabID());
			}
		}
		list.AddRange(SaveGame.Instance.worldGenSpawner.GetUnspawnedWithType<Geyser>(worldContainer.id));
		list.AddRange(SaveGame.Instance.worldGenSpawner.GetSpawnersWithTag("OilWell", worldContainer.id, includeSpawned: true));
		foreach (KeyValuePair<Tag, GameObject> geyserRow in geyserRows)
		{
			geyserRow.Value.SetActive(value: false);
		}
		foreach (Tag item in list)
		{
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(item);
			if (!geyserRows.ContainsKey(item))
			{
				geyserRows.Add(item, Util.KInstantiateUI(iconLabelRow, worldGeysersPanel.Content.gameObject, force_active: true));
				HierarchyReferences component2 = geyserRows[item].GetComponent<HierarchyReferences>();
				component2.GetReference<Image>("Icon").sprite = uISprite.first;
				component2.GetReference<Image>("Icon").color = uISprite.second;
				component2.GetReference<LocText>("NameLabel").SetText(Assets.GetPrefab(item).GetProperName());
				component2.GetReference<LocText>("ValueLabel").gameObject.SetActive(value: false);
			}
			geyserRows[item].SetActive(value: true);
		}
		Tag key = "NoGeysers";
		if (!geyserRows.ContainsKey(key))
		{
			geyserRows.Add(key, Util.KInstantiateUI(iconLabelRow, worldGeysersPanel.Content.gameObject, force_active: true));
			HierarchyReferences component3 = geyserRows[key].GetComponent<HierarchyReferences>();
			component3.GetReference<Image>("Icon").sprite = Assets.GetSprite("icon_action_cancel");
			component3.GetReference<LocText>("NameLabel").SetText(UI.DETAILTABS.SIMPLEINFO.NO_GEYSERS);
			component3.GetReference<LocText>("ValueLabel").gameObject.SetActive(value: false);
		}
		geyserRows[key].gameObject.SetActive(list.Count == 0);
		List<string> worldTraitIds = worldContainer.WorldTraitIds;
		if (worldTraitIds == null)
		{
			return;
		}
		for (int j = 0; j < worldTraitIds.Count; j++)
		{
			if (j > worldTraitRows.Count - 1)
			{
				CreateWorldTraitRow();
			}
			WorldTrait cachedTrait = SettingsCache.GetCachedTrait(worldTraitIds[j], assertMissingTrait: false);
			if (cachedTrait != null)
			{
				worldTraitRows[j].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(Strings.Get(cachedTrait.name));
				worldTraitRows[j].AddOrGet<ToolTip>().SetSimpleTooltip(Strings.Get(cachedTrait.description));
			}
			else
			{
				worldTraitRows[j].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.MISSING_TRAIT);
				worldTraitRows[j].AddOrGet<ToolTip>().SetSimpleTooltip("");
			}
		}
		for (int k = 0; k < worldTraitRows.Count; k++)
		{
			worldTraitRows[k].SetActive(k < worldTraitIds.Count);
		}
		if (worldTraitIds.Count == 0)
		{
			if (worldTraitRows.Count < 1)
			{
				CreateWorldTraitRow();
			}
			worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel").SetText(WORLD_TRAITS.NO_TRAITS.NAME_SHORTHAND);
			worldTraitRows[0].AddOrGet<ToolTip>().SetSimpleTooltip(WORLD_TRAITS.NO_TRAITS.DESCRIPTION);
			worldTraitRows[0].SetActive(value: true);
		}
	}

	private void RefreshProcessConditions()
	{
		foreach (GameObject processConditionRow in processConditionRows)
		{
			Util.KDestroyGameObject(processConditionRow);
		}
		processConditionRows.Clear();
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			if (selectedTarget.GetComponent<LaunchableRocket>() != null)
			{
				RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
				RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
				RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
			}
			else
			{
				RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
			}
		}
		else if (selectedTarget.GetComponent<LaunchPad>() != null || selectedTarget.GetComponent<RocketProcessConditionDisplayTarget>() != null)
		{
			RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketFlight);
			RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
			RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
			RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
		}
		else
		{
			RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
		}
	}

	private void RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType conditionType)
	{
		List<ProcessCondition> conditionSet = selectedTarget.GetComponent<IProcessConditionSet>().GetConditionSet(conditionType);
		if (conditionSet.Count == 0)
		{
			return;
		}
		HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(processConditionHeader.gameObject, processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, force_active: true);
		hierarchyReferences.GetReference<LocText>("Label").text = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper());
		hierarchyReferences.GetComponent<ToolTip>().toolTip = Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper() + "_TOOLTIP");
		processConditionRows.Add(hierarchyReferences.gameObject);
		List<ProcessCondition> list = new List<ProcessCondition>();
		foreach (ProcessCondition condition in conditionSet)
		{
			if (condition.ShowInUI() && (condition.GetType() == typeof(RequireAttachedComponent) || list.Find((ProcessCondition match) => match.GetType() == condition.GetType()) == null))
			{
				list.Add(condition);
				GameObject gameObject = Util.KInstantiateUI(processConditionRow, processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, force_active: true);
				processConditionRows.Add(gameObject);
				ConditionListSideScreen.SetRowState(gameObject, condition);
			}
		}
	}

	public GameObject AddOrGetStorageLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
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

	public void Sim1000ms(float dt)
	{
		if (selectedTarget != null && selectedTarget.GetComponent<IProcessConditionSet>() != null)
		{
			RefreshProcessConditions();
		}
	}

	public void Sim4000ms(float dt)
	{
		RefreshWorld();
		spaceSimpleInfoPOIPanel.Refresh(spacePOIPanel, selectedTarget);
	}
}
