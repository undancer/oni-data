using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductInfoScreen : KScreen
{
	public TitleBar titleBar;

	public GameObject ProductDescriptionPane;

	public LocText productDescriptionText;

	public DescriptorPanel ProductRequirementsPane;

	public DescriptorPanel ProductEffectsPane;

	public GameObject ProductFlavourPane;

	public LocText productFlavourText;

	public RectTransform BGPanel;

	public MaterialSelectionPanel materialSelectionPanelPrefab;

	private Dictionary<string, GameObject> descLabels = new Dictionary<string, GameObject>();

	public MultiToggle sandboxInstantBuildToggle;

	[NonSerialized]
	public MaterialSelectionPanel materialSelectionPanel;

	[NonSerialized]
	public BuildingDef currentDef;

	public System.Action onElementsFullySelected;

	private bool expandedInfo = true;

	private bool configuring;

	private void RefreshScreen()
	{
		if (currentDef != null)
		{
			SetTitle(currentDef);
		}
		else
		{
			ClearProduct();
		}
	}

	public void ClearProduct(bool deactivateTool = true)
	{
		if (!(materialSelectionPanel == null))
		{
			currentDef = null;
			materialSelectionPanel.ClearMaterialToggles();
			if (PlayerController.Instance.ActiveTool == BuildTool.Instance && deactivateTool)
			{
				BuildTool.Instance.Deactivate();
			}
			if (PlayerController.Instance.ActiveTool == UtilityBuildTool.Instance || PlayerController.Instance.ActiveTool == WireBuildTool.Instance)
			{
				ToolMenu.Instance.ClearSelection();
			}
			ClearLabels();
			Show(show: false);
		}
	}

	public new void Awake()
	{
		base.Awake();
		materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(materialSelectionPanelPrefab.gameObject, base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (BuildingGroupScreen.Instance != null)
		{
			BuildingGroupScreen instance = BuildingGroupScreen.Instance;
			instance.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			BuildingGroupScreen instance2 = BuildingGroupScreen.Instance;
			instance2.pointerExitActions = (PointerExitActions)Delegate.Combine(instance2.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		if (PlanScreen.Instance != null)
		{
			PlanScreen instance3 = PlanScreen.Instance;
			instance3.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance3.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			PlanScreen instance4 = PlanScreen.Instance;
			instance4.pointerExitActions = (PointerExitActions)Delegate.Combine(instance4.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		if (BuildMenu.Instance != null)
		{
			BuildMenu instance5 = BuildMenu.Instance;
			instance5.pointerEnterActions = (PointerEnterActions)Delegate.Combine(instance5.pointerEnterActions, new PointerEnterActions(CheckMouseOver));
			BuildMenu instance6 = BuildMenu.Instance;
			instance6.pointerExitActions = (PointerExitActions)Delegate.Combine(instance6.pointerExitActions, new PointerExitActions(CheckMouseOver));
		}
		pointerEnterActions = (PointerEnterActions)Delegate.Combine(pointerEnterActions, new PointerEnterActions(CheckMouseOver));
		pointerExitActions = (PointerExitActions)Delegate.Combine(pointerExitActions, new PointerExitActions(CheckMouseOver));
		base.ConsumeMouseScroll = true;
		sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
		MultiToggle multiToggle = sandboxInstantBuildToggle;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SandboxToolParameterMenu.instance.settings.InstantBuild = !SandboxToolParameterMenu.instance.settings.InstantBuild;
			sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
		});
		sandboxInstantBuildToggle.gameObject.SetActive(Game.Instance.SandboxModeActive);
		Game.Instance.Subscribe(-1948169901, delegate
		{
			sandboxInstantBuildToggle.gameObject.SetActive(Game.Instance.SandboxModeActive);
		});
	}

	public void ConfigureScreen(BuildingDef def)
	{
		configuring = true;
		currentDef = def;
		SetTitle(def);
		SetDescription(def);
		SetEffects(def);
		SetMaterials(def);
		configuring = false;
	}

	private void ExpandInfo(PointerEventData data)
	{
		ToggleExpandedInfo(state: true);
	}

	private void CollapseInfo(PointerEventData data)
	{
		ToggleExpandedInfo(state: false);
	}

	public void ToggleExpandedInfo(bool state)
	{
		expandedInfo = state;
		if (ProductDescriptionPane != null)
		{
			ProductDescriptionPane.SetActive(expandedInfo);
		}
		if (ProductRequirementsPane != null)
		{
			ProductRequirementsPane.gameObject.SetActive(expandedInfo && ProductRequirementsPane.HasDescriptors());
		}
		if (ProductEffectsPane != null)
		{
			ProductEffectsPane.gameObject.SetActive(expandedInfo && ProductEffectsPane.HasDescriptors());
		}
		if (ProductFlavourPane != null)
		{
			ProductFlavourPane.SetActive(expandedInfo);
		}
		if (materialSelectionPanel != null && materialSelectionPanel.CurrentSelectedElement != null)
		{
			materialSelectionPanel.ToggleShowDescriptorPanels(expandedInfo);
		}
	}

	private void CheckMouseOver(PointerEventData data)
	{
		bool state = base.GetMouseOver || (PlanScreen.Instance != null && ((PlanScreen.Instance.isActiveAndEnabled && PlanScreen.Instance.GetMouseOver) || BuildingGroupScreen.Instance.GetMouseOver)) || (BuildMenu.Instance != null && BuildMenu.Instance.isActiveAndEnabled && BuildMenu.Instance.GetMouseOver);
		ToggleExpandedInfo(state);
	}

	private void Update()
	{
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && currentDef != null && materialSelectionPanel.CurrentSelectedElement != null && !MaterialSelector.AllowInsufficientMaterialBuild() && currentDef.Mass[0] > ClusterManager.Instance.activeWorld.worldInventory.GetAmount(materialSelectionPanel.CurrentSelectedElement, includeRelatedWorlds: true))
		{
			materialSelectionPanel.AutoSelectAvailableMaterial();
		}
	}

	private void SetTitle(BuildingDef def)
	{
		titleBar.SetTitle(def.Name);
		bool flag = (PlanScreen.Instance != null && PlanScreen.Instance.isActiveAndEnabled && PlanScreen.Instance.IsDefBuildable(def)) || (BuildMenu.Instance != null && BuildMenu.Instance.isActiveAndEnabled && BuildMenu.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete);
		titleBar.GetComponentInChildren<KImage>().ColorState = ((!flag) ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Active);
	}

	private void SetDescription(BuildingDef def)
	{
		if (def == null || productFlavourText == null)
		{
			return;
		}
		string text = def.Desc;
		Dictionary<Klei.AI.Attribute, float> dictionary = new Dictionary<Klei.AI.Attribute, float>();
		Dictionary<Klei.AI.Attribute, float> dictionary2 = new Dictionary<Klei.AI.Attribute, float>();
		foreach (Klei.AI.Attribute attribute in def.attributes)
		{
			if (!dictionary.ContainsKey(attribute))
			{
				dictionary[attribute] = 0f;
			}
		}
		foreach (AttributeModifier attributeModifier in def.attributeModifiers)
		{
			float value = 0f;
			Klei.AI.Attribute key = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
			dictionary.TryGetValue(key, out value);
			value = (dictionary[key] = value + attributeModifier.Value);
		}
		if (materialSelectionPanel.CurrentSelectedElement != null)
		{
			Element element = ElementLoader.GetElement(materialSelectionPanel.CurrentSelectedElement);
			if (element != null)
			{
				foreach (AttributeModifier attributeModifier2 in element.attributeModifiers)
				{
					float value2 = 0f;
					Klei.AI.Attribute key2 = Db.Get().BuildingAttributes.Get(attributeModifier2.AttributeId);
					dictionary2.TryGetValue(key2, out value2);
					value2 = (dictionary2[key2] = value2 + attributeModifier2.Value);
				}
			}
			else
			{
				PrefabAttributeModifiers component = Assets.TryGetPrefab(materialSelectionPanel.CurrentSelectedElement).GetComponent<PrefabAttributeModifiers>();
				if (component != null)
				{
					foreach (AttributeModifier descriptor in component.descriptors)
					{
						float value3 = 0f;
						Klei.AI.Attribute key3 = Db.Get().BuildingAttributes.Get(descriptor.AttributeId);
						dictionary2.TryGetValue(key3, out value3);
						value3 = (dictionary2[key3] = value3 + descriptor.Value);
					}
				}
			}
		}
		if (dictionary.Count > 0)
		{
			text += "\n\n";
			foreach (KeyValuePair<Klei.AI.Attribute, float> item in dictionary)
			{
				float value4 = 0f;
				dictionary.TryGetValue(item.Key, out value4);
				float value5 = 0f;
				string text2 = "";
				if (dictionary2.TryGetValue(item.Key, out value5))
				{
					value5 = Mathf.Abs(value4 * value5);
					text2 = "(+" + value5 + ")";
				}
				text = text + "\n" + item.Key.Name + ": " + (value4 + value5) + text2;
			}
		}
		productFlavourText.text = text;
	}

	private void SetEffects(BuildingDef def)
	{
		if (productDescriptionText.text != null)
		{
			productDescriptionText.text = $"{def.Effect}";
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def.BuildingComplete);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONREQUIREMENTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONREQUIREMENTS);
			requirementDescriptors.Insert(0, item);
			ProductRequirementsPane.gameObject.SetActive(value: true);
		}
		else
		{
			ProductRequirementsPane.gameObject.SetActive(value: false);
		}
		ProductRequirementsPane.SetDescriptors(requirementDescriptors);
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			Descriptor item2 = default(Descriptor);
			item2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONEFFECTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONEFFECTS);
			effectDescriptors.Insert(0, item2);
			ProductEffectsPane.gameObject.SetActive(value: true);
		}
		else
		{
			ProductEffectsPane.gameObject.SetActive(value: false);
		}
		ProductEffectsPane.SetDescriptors(effectDescriptors);
	}

	public void ClearLabels()
	{
		List<string> list = new List<string>(descLabels.Keys);
		if (list.Count <= 0)
		{
			return;
		}
		foreach (string item in list)
		{
			GameObject gameObject = descLabels[item];
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			descLabels.Remove(item);
		}
	}

	public void SetMaterials(BuildingDef def)
	{
		materialSelectionPanel.gameObject.SetActive(value: true);
		Recipe craftRecipe = def.CraftRecipe;
		materialSelectionPanel.ClearSelectActions();
		materialSelectionPanel.ConfigureScreen(craftRecipe, PlanScreen.Instance.IsDefBuildable, PlanScreen.Instance.GetTooltipForBuildable);
		materialSelectionPanel.ToggleShowDescriptorPanels(show: false);
		materialSelectionPanel.AddSelectAction(RefreshScreen);
		materialSelectionPanel.AddSelectAction(onMenuMaterialChanged);
		materialSelectionPanel.AutoSelectAvailableMaterial();
		ActivateAppropriateTool(def);
	}

	private void onMenuMaterialChanged()
	{
		if (!(currentDef == null))
		{
			ActivateAppropriateTool(currentDef);
			SetDescription(currentDef);
		}
	}

	private void ActivateAppropriateTool(BuildingDef def)
	{
		Debug.Assert(def != null, "def was null");
		bool num;
		if (!(PlanScreen.Instance != null))
		{
			if (!(BuildMenu.Instance != null))
			{
				goto IL_0064;
			}
			num = BuildMenu.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete;
		}
		else
		{
			num = PlanScreen.Instance.IsDefBuildable(def);
		}
		if (num && materialSelectionPanel.AllSelectorsSelected())
		{
			onElementsFullySelected.Signal();
			return;
		}
		goto IL_0064;
		IL_0064:
		if (!MaterialSelector.AllowInsufficientMaterialBuild() && !DebugHandler.InstantBuildMode)
		{
			if (PlayerController.Instance.ActiveTool == BuildTool.Instance)
			{
				BuildTool.Instance.Deactivate();
			}
			PrebuildTool.Instance.Activate(def, PlanScreen.Instance.GetTooltipForBuildable(def));
		}
	}

	public static bool MaterialsMet(Recipe recipe)
	{
		if (recipe == null)
		{
			Debug.LogError("Trying to verify the materials on a null recipe!");
			return false;
		}
		if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
		{
			Debug.LogError("Trying to verify the materials on a recipe with no MaterialCategoryTags!");
			return false;
		}
		bool result = true;
		for (int i = 0; i < recipe.Ingredients.Count; i++)
		{
			if (MaterialSelectionPanel.Filter(recipe.Ingredients[i].tag).kgAvailable < recipe.Ingredients[i].amount)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public void Close()
	{
		if (!configuring)
		{
			ClearProduct();
			Show(show: false);
		}
	}
}
