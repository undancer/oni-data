using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SandboxToolParameterMenu : KScreen
{
	public class SelectorValue
	{
		public class SearchFilter
		{
			public string Name;

			public Func<object, bool> condition;

			public SearchFilter parentFilter;

			public Tuple<Sprite, Color> icon;

			public SearchFilter(string Name, Func<object, bool> condition, SearchFilter parentFilter = null, Tuple<Sprite, Color> icon = null)
			{
				this.Name = Name;
				this.condition = condition;
				this.parentFilter = parentFilter;
				this.icon = icon;
			}
		}

		public GameObject row;

		public List<KeyValuePair<object, GameObject>> optionButtons;

		public KButton button;

		public object[] options;

		public Action<object> onValueChanged;

		public Func<object, string> getOptionName;

		public Func<string, object, bool> filterOptionFunction;

		public Func<object, Tuple<Sprite, Color>> getOptionSprite;

		public SearchFilter[] filters;

		public List<SearchFilter> activeFilters = new List<SearchFilter>();

		public SearchFilter currentFilter;

		public string labelText;

		public SelectorValue(object[] options, Action<object> onValueChanged, Func<object, string> getOptionName, Func<string, object, bool> filterOptionFunction, Func<object, Tuple<Sprite, Color>> getOptionSprite, string labelText, SearchFilter[] filters = null)
		{
			this.options = options;
			this.onValueChanged = onValueChanged;
			this.getOptionName = getOptionName;
			this.filterOptionFunction = filterOptionFunction;
			this.getOptionSprite = getOptionSprite;
			this.filters = filters;
			this.labelText = labelText;
		}

		public bool runCurrentFilter(object obj)
		{
			if (currentFilter == null)
			{
				return true;
			}
			if (!currentFilter.condition(obj))
			{
				return false;
			}
			return true;
		}
	}

	public class SliderValue
	{
		public GameObject row;

		public string bottomSprite;

		public string topSprite;

		public float slideMinValue;

		public float slideMaxValue;

		public float clampValueLow;

		public float clampValueHigh;

		public string unitString;

		public Action<float> onValueChanged;

		public string tooltip;

		public int roundToDecimalPlaces;

		public string labelText;

		public KSlider slider;

		public KNumberInputField inputField;

		public SliderValue(float slideMinValue, float slideMaxValue, string bottomSprite, string topSprite, string unitString, string tooltip, string labelText, Action<float> onValueChanged, int decimalPlaces = 0)
		{
			this.slideMinValue = slideMinValue;
			this.slideMaxValue = slideMaxValue;
			this.bottomSprite = bottomSprite;
			this.topSprite = topSprite;
			this.unitString = unitString;
			this.onValueChanged = onValueChanged;
			this.tooltip = tooltip;
			roundToDecimalPlaces = decimalPlaces;
			this.labelText = labelText;
			clampValueLow = slideMinValue;
			clampValueHigh = slideMaxValue;
		}

		public void SetRange(float min, float max, bool resetCurrentValue = true)
		{
			slideMinValue = min;
			slideMaxValue = max;
			slider.minValue = slideMinValue;
			slider.maxValue = slideMaxValue;
			inputField.currentValue = slideMinValue + (slideMaxValue - slideMinValue) / 2f;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (resetCurrentValue)
			{
				slider.value = slideMinValue + (slideMaxValue - slideMinValue) / 2f;
				onValueChanged(slideMinValue + (slideMaxValue - slideMinValue) / 2f);
			}
		}

		public void SetValue(float value, bool runOnValueChanged = true)
		{
			value = Mathf.Clamp(value, clampValueLow, clampValueHigh);
			slider.value = value;
			inputField.currentValue = value;
			if (runOnValueChanged)
			{
				onValueChanged(value);
			}
			RefreshDisplay();
		}

		public void RefreshDisplay()
		{
			inputField.SetDisplayValue(((roundToDecimalPlaces == 0) ? ((float)Mathf.RoundToInt(inputField.currentValue)) : inputField.currentValue).ToString());
		}
	}

	public static SandboxToolParameterMenu instance;

	public SandboxSettings settings;

	[SerializeField]
	private GameObject sliderPropertyPrefab;

	[SerializeField]
	private GameObject selectorPropertyPrefab;

	private List<GameObject> inputFields = new List<GameObject>();

	private Dictionary<Tag, List<KPrefabID>> items;

	public SelectorValue elementSelector;

	public SliderValue brushRadiusSlider = new SliderValue(1f, 10f, "dash", "circle_hard", "", UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.BRUSH_SIZE.NAME, delegate(float value)
	{
		instance.settings.SetIntSetting("SandboxTools.BrushSize", Mathf.Clamp(Mathf.RoundToInt(value), 1, 50));
	});

	public SliderValue noiseScaleSlider = new SliderValue(0f, 1f, "little", "lots", "", UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE_SCALE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE_SCALE.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandboxTools.NoiseScale", value);
	}, 2);

	public SliderValue noiseDensitySlider = new SliderValue(1f, 20f, "little", "lots", "", UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE_SCALE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.BRUSH_NOISE_DENSITY.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandboxTools.NoiseDensity", value);
	}, 2);

	public SliderValue massSlider = new SliderValue(0.1f, 1000f, "action_pacify", "status_item_plant_solid", UI.UNITSUFFIXES.MASS.KILOGRAM, UI.SANDBOXTOOLS.SETTINGS.MASS.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.MASS.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandboxTools.Mass", Mathf.Clamp(value, 0.001f, 9999f));
	}, 2);

	public SliderValue temperatureSlider = new SliderValue(150f, 500f, "cold", "hot", GameUtil.GetTemperatureUnitSuffix(), UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandbosTools.Temperature", Mathf.Clamp(GameUtil.GetTemperatureConvertedToKelvin(value), 1f, 9999f));
	});

	public SliderValue temperatureAdditiveSlider = new SliderValue(-15f, 15f, "cold", "hot", GameUtil.GetTemperatureUnitSuffix(), UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE_ADDITIVE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.TEMPERATURE_ADDITIVE.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandbosTools.TemperatureAdditive", GameUtil.GetTemperatureConvertedToKelvin(value));
	});

	public SliderValue radiationAdditiveSlider = new SliderValue(-100f, 1000f, "little", "lots", UI.UNITSUFFIXES.RADIATION.RADS, UI.SANDBOXTOOLS.SETTINGS.RADIATION_ADDITIVE.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.RADIATION_ADDITIVE.NAME, delegate(float value)
	{
		instance.settings.SetFloatSetting("SandbosTools.RadiationAdditive", value);
	});

	public SelectorValue diseaseSelector;

	public SliderValue diseaseCountSlider = new SliderValue(0f, 10000f, "status_item_barren", "germ", UI.UNITSUFFIXES.DISEASE.UNITS, UI.SANDBOXTOOLS.SETTINGS.DISEASE_COUNT.TOOLTIP, UI.SANDBOXTOOLS.SETTINGS.DISEASE_COUNT.NAME, delegate(float value)
	{
		instance.settings.SetIntSetting("SandboxTools.DiseaseCount", Mathf.RoundToInt(value));
	});

	public SelectorValue entitySelector;

	public static void DestroyInstance()
	{
		instance = null;
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ConfigureSettings();
		activateOnSpawn = true;
		base.ConsumeMouseScroll = true;
	}

	private void ConfigureSettings()
	{
		massSlider.clampValueLow = 0.001f;
		massSlider.clampValueHigh = 10000f;
		temperatureAdditiveSlider.clampValueLow = -9999f;
		temperatureAdditiveSlider.clampValueHigh = 9999f;
		temperatureSlider.clampValueLow = -458f;
		temperatureSlider.clampValueHigh = 9999f;
		brushRadiusSlider.clampValueLow = 1f;
		brushRadiusSlider.clampValueHigh = 50f;
		diseaseCountSlider.clampValueHigh = 1000000f;
		diseaseCountSlider.slideMaxValue = 1000000f;
		settings = new SandboxSettings();
		SandboxSettings sandboxSettings = settings;
		sandboxSettings.OnChangeElement = (Action<bool>)Delegate.Combine(sandboxSettings.OnChangeElement, (Action<bool>)delegate(bool forceElementDefaults)
		{
			int num = settings.GetIntSetting("SandboxTools.SelectedElement");
			if (num >= ElementLoader.elements.Count)
			{
				num = 0;
			}
			Element element = ElementLoader.elements[num];
			elementSelector.button.GetComponentInChildren<LocText>().text = element.name + " (" + element.GetStateString() + ")";
			Tuple<Sprite, Color> uISprite = Def.GetUISprite(element);
			elementSelector.button.GetComponentsInChildren<Image>()[1].sprite = uISprite.first;
			elementSelector.button.GetComponentsInChildren<Image>()[1].color = uISprite.second;
			SetAbsoluteTemperatureSliderRange(element);
			massSlider.SetRange(0.1f, Mathf.Min(element.maxMass * 2f, massSlider.clampValueHigh), resetCurrentValue: false);
			if (forceElementDefaults)
			{
				temperatureSlider.SetValue(GameUtil.GetConvertedTemperature(element.defaultValues.temperature, roundOutput: true));
				massSlider.SetValue(element.defaultValues.mass);
			}
		});
		SandboxSettings sandboxSettings2 = settings;
		sandboxSettings2.OnChangeMass = (System.Action)Delegate.Combine(sandboxSettings2.OnChangeMass, (System.Action)delegate
		{
			massSlider.SetValue(settings.GetFloatSetting("SandboxTools.Mass"), runOnValueChanged: false);
		});
		SandboxSettings sandboxSettings3 = settings;
		sandboxSettings3.OnChangeDisease = (System.Action)Delegate.Combine(sandboxSettings3.OnChangeDisease, (System.Action)delegate
		{
			Disease disease = Db.Get().Diseases.TryGet(instance.settings.GetStringSetting("SandboxTools.SelectedDisease"));
			if (disease == null)
			{
				disease = Db.Get().Diseases.Get("FoodPoisoning");
			}
			diseaseSelector.button.GetComponentInChildren<LocText>().text = disease.Name;
			diseaseSelector.button.GetComponentsInChildren<Image>()[1].sprite = Assets.GetSprite("germ");
			diseaseCountSlider.SetRange(0f, 1000000f, resetCurrentValue: false);
		});
		SandboxSettings sandboxSettings4 = settings;
		sandboxSettings4.OnChangeDiseaseCount = (System.Action)Delegate.Combine(sandboxSettings4.OnChangeDiseaseCount, (System.Action)delegate
		{
			diseaseCountSlider.SetValue(settings.GetIntSetting("SandboxTools.DiseaseCount"), runOnValueChanged: false);
		});
		SandboxSettings sandboxSettings5 = settings;
		sandboxSettings5.OnChangeEntity = (System.Action)Delegate.Combine(sandboxSettings5.OnChangeEntity, (System.Action)delegate
		{
			string stringSetting = instance.settings.GetStringSetting("SandboxTools.SelectedEntity");
			GameObject gameObject = Assets.TryGetPrefab(stringSetting);
			if (gameObject == null)
			{
				settings.ForceDefaultStringSetting("SandboxTools.SelectedEntity");
			}
			else
			{
				entitySelector.button.GetComponentInChildren<LocText>().text = gameObject.GetProperName();
				Tuple<Sprite, Color> tuple = ((!(stringSetting == MinionConfig.ID)) ? Def.GetUISprite(stringSetting) : new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white));
				if (tuple != null)
				{
					entitySelector.button.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
					entitySelector.button.GetComponentsInChildren<Image>()[1].color = tuple.second;
				}
			}
		});
		SandboxSettings sandboxSettings6 = settings;
		sandboxSettings6.OnChangeBrushSize = (System.Action)Delegate.Combine(sandboxSettings6.OnChangeBrushSize, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is BrushTool)
			{
				(PlayerController.Instance.ActiveTool as BrushTool).SetBrushSize(settings.GetIntSetting("SandboxTools.BrushSize"));
			}
		});
		SandboxSettings sandboxSettings7 = settings;
		sandboxSettings7.OnChangeNoiseScale = (System.Action)Delegate.Combine(sandboxSettings7.OnChangeNoiseScale, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is SandboxSprinkleTool)
			{
				(PlayerController.Instance.ActiveTool as SandboxSprinkleTool).SetBrushSize(settings.GetIntSetting("SandboxTools.BrushSize"));
			}
		});
		SandboxSettings sandboxSettings8 = settings;
		sandboxSettings8.OnChangeNoiseDensity = (System.Action)Delegate.Combine(sandboxSettings8.OnChangeNoiseDensity, (System.Action)delegate
		{
			if (PlayerController.Instance.ActiveTool is SandboxSprinkleTool)
			{
				(PlayerController.Instance.ActiveTool as SandboxSprinkleTool).SetBrushSize(settings.GetIntSetting("SandboxTools.BrushSize"));
			}
		});
		SandboxSettings sandboxSettings9 = settings;
		sandboxSettings9.OnChangeTemperature = (System.Action)Delegate.Combine(sandboxSettings9.OnChangeTemperature, (System.Action)delegate
		{
			temperatureSlider.SetValue(GameUtil.GetConvertedTemperature(settings.GetFloatSetting("SandbosTools.Temperature")), runOnValueChanged: false);
		});
		SandboxSettings sandboxSettings10 = settings;
		sandboxSettings10.OnChangeAdditiveTemperature = (System.Action)Delegate.Combine(sandboxSettings10.OnChangeAdditiveTemperature, (System.Action)delegate
		{
			temperatureAdditiveSlider.SetValue(GameUtil.GetConvertedTemperature(settings.GetFloatSetting("SandbosTools.TemperatureAdditive"), roundOutput: true), runOnValueChanged: false);
		});
		Game.Instance.Subscribe(999382396, OnTemperatureUnitChanged);
		SandboxSettings sandboxSettings11 = settings;
		sandboxSettings11.OnChangeAdditiveRadiation = (System.Action)Delegate.Combine(sandboxSettings11.OnChangeAdditiveRadiation, (System.Action)delegate
		{
			radiationAdditiveSlider.SetValue(settings.GetFloatSetting("SandbosTools.RadiationAdditive"), runOnValueChanged: false);
		});
	}

	public void DisableParameters()
	{
		elementSelector.row.SetActive(value: false);
		entitySelector.row.SetActive(value: false);
		brushRadiusSlider.row.SetActive(value: false);
		noiseScaleSlider.row.SetActive(value: false);
		noiseDensitySlider.row.SetActive(value: false);
		massSlider.row.SetActive(value: false);
		temperatureAdditiveSlider.row.SetActive(value: false);
		temperatureSlider.row.SetActive(value: false);
		radiationAdditiveSlider.row.SetActive(value: false);
		diseaseCountSlider.row.SetActive(value: false);
		diseaseSelector.row.SetActive(value: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConfigureElementSelector();
		ConfigureDiseaseSelector();
		ConfigureEntitySelector();
		SpawnSelector(entitySelector);
		SpawnSelector(elementSelector);
		SpawnSlider(brushRadiusSlider);
		SpawnSlider(noiseScaleSlider);
		SpawnSlider(noiseDensitySlider);
		SpawnSlider(massSlider);
		SpawnSlider(temperatureSlider);
		SpawnSlider(temperatureAdditiveSlider);
		SpawnSlider(radiationAdditiveSlider);
		SpawnSelector(diseaseSelector);
		SpawnSlider(diseaseCountSlider);
		if (instance == null)
		{
			instance = this;
			base.gameObject.SetActive(value: false);
			settings.RestorePrefs();
		}
	}

	private void ConfigureElementSelector()
	{
		Func<object, bool> condition = (object element) => (element as Element).IsSolid;
		Func<object, bool> condition2 = (object element) => (element as Element).IsLiquid;
		Func<object, bool> condition3 = (object element) => (element as Element).IsGas;
		List<Element> commonElements = new List<Element>();
		Func<object, bool> condition4 = (object element) => commonElements.Contains(element as Element);
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Oxygen));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Water));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Vacuum));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Dirt));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SandStone));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Cuprite));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Steel));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Algae));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.CrudeOil));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.CarbonDioxide));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Sand));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.SlimeMold));
		commonElements.Insert(0, ElementLoader.FindElementByHash(SimHashes.Granite));
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (!element.disabled)
			{
				list.Add(element);
			}
		}
		list.Sort((Element a, Element b) => a.name.CompareTo(b.name));
		object[] options = list.ToArray();
		elementSelector = new SelectorValue(options, delegate(object element)
		{
			settings.SetIntSetting("SandboxTools.SelectedElement", ((Element)element).idx);
		}, (object element) => (element as Element).name + " (" + (element as Element).GetStateString() + ")", (string filterString, object option) => ((option as Element).name.ToUpper() + (option as Element).GetStateString().ToUpper()).Contains(filterString.ToUpper()) ? true : false, (object element) => Def.GetUISprite(element as Element), UI.SANDBOXTOOLS.SETTINGS.ELEMENT.NAME, new SelectorValue.SearchFilter[4]
		{
			new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.COMMON, condition4),
			new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.SOLID, condition, null, Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.SandStone))),
			new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.LIQUID, condition2, null, Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Water))),
			new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.GAS, condition3, null, Def.GetUISprite(ElementLoader.FindElementByHash(SimHashes.Oxygen)))
		});
	}

	private void ConfigureEntitySelector()
	{
		List<SelectorValue.SearchFilter> list = new List<SelectorValue.SearchFilter>();
		SelectorValue.SearchFilter item = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.FOOD, delegate(object entity)
		{
			string idString = (entity as KPrefabID).PrefabID().ToString();
			return !(entity as KPrefabID).HasTag(GameTags.Egg) && EdiblesManager.GetAllFoodTypes().Find((EdiblesManager.FoodInfo match) => match.Id == idString) != null;
		}, null, Def.GetUISprite(Assets.GetPrefab("MushBar")));
		list.Add(item);
		SelectorValue.SearchFilter item2 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.COMETS, (object entity) => (entity as KPrefabID).HasTag(GameTags.Comet), null, Def.GetUISprite(Assets.GetPrefab(CopperCometConfig.ID)));
		list.Add(item2);
		SelectorValue.SearchFilter item3 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.SPECIAL, (object entity) => (entity as KPrefabID).PrefabID().Name == MinionConfig.ID, null, new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white));
		list.Add(item3);
		SelectorValue.SearchFilter searchFilter = null;
		SelectorValue.SearchFilter searchFilter2 = null;
		searchFilter = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE, (object entity) => false, null, Def.GetUISprite(Assets.GetPrefab("Hatch")));
		list.Add(searchFilter);
		List<Tag> list2 = new List<Tag>();
		foreach (GameObject item8 in Assets.GetPrefabsWithTag("CreatureBrain".ToTag()))
		{
			CreatureBrain brain = item8.GetComponent<CreatureBrain>();
			if (!list2.Contains(brain.species))
			{
				Tuple<Sprite, Color> icon = new Tuple<Sprite, Color>(CodexCache.entries[brain.species.ToString().ToUpper()].icon, CodexCache.entries[brain.species.ToString().ToUpper()].iconColor);
				list2.Add(brain.species);
				SelectorValue.SearchFilter item4 = new SelectorValue.SearchFilter(Strings.Get("STRINGS.CREATURES.FAMILY_PLURAL." + brain.species.ToString().ToUpper()), delegate(object entity)
				{
					CreatureBrain component2 = Assets.GetPrefab((entity as KPrefabID).PrefabID()).GetComponent<CreatureBrain>();
					return (entity as KPrefabID).HasTag("CreatureBrain".ToString()) && component2.species == brain.species;
				}, searchFilter, icon);
				list.Add(item4);
			}
		}
		searchFilter2 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.CREATURE_EGG, (object entity) => (entity as KPrefabID).HasTag(GameTags.Egg), searchFilter, Def.GetUISprite(Assets.GetPrefab("HatchEgg")));
		list.Add(searchFilter2);
		SelectorValue.SearchFilter item5 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.EQUIPMENT, delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject4 = (entity as KPrefabID).gameObject;
			return gameObject4 != null && gameObject4.GetComponent<Equippable>() != null;
		}, null, Def.GetUISprite(Assets.GetPrefab("Funky_Vest")));
		list.Add(item5);
		SelectorValue.SearchFilter searchFilter3 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.PLANTS, delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject3 = (entity as KPrefabID).gameObject;
			return gameObject3 != null && (gameObject3.GetComponent<Harvestable>() != null || gameObject3.GetComponent<WiltCondition>() != null);
		}, null, Def.GetUISprite(Assets.GetPrefab("PrickleFlower")));
		list.Add(searchFilter3);
		SelectorValue.SearchFilter item6 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.SEEDS, delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject2 = (entity as KPrefabID).gameObject;
			return gameObject2 != null && gameObject2.GetComponent<PlantableSeed>() != null;
		}, searchFilter3, Def.GetUISprite(Assets.GetPrefab("PrickleFlowerSeed")));
		list.Add(item6);
		SelectorValue.SearchFilter item7 = new SelectorValue.SearchFilter(UI.SANDBOXTOOLS.FILTERS.ENTITIES.INDUSTRIAL_PRODUCTS, delegate(object entity)
		{
			if ((entity as KPrefabID).gameObject == null)
			{
				return false;
			}
			GameObject gameObject = (entity as KPrefabID).gameObject;
			return gameObject != null && (gameObject.HasTag(GameTags.IndustrialIngredient) || gameObject.HasTag(GameTags.IndustrialProduct) || gameObject.HasTag(GameTags.Medicine) || gameObject.HasTag(GameTags.MedicalSupplies));
		}, null, Def.GetUISprite(Assets.GetPrefab("BasicCure")));
		list.Add(item7);
		List<KPrefabID> list3 = new List<KPrefabID>();
		foreach (KPrefabID prefab2 in Assets.Prefabs)
		{
			foreach (SelectorValue.SearchFilter item9 in list)
			{
				if (item9.condition(prefab2))
				{
					list3.Add(prefab2);
					break;
				}
			}
		}
		object[] options = list3.ToArray();
		entitySelector = new SelectorValue(options, delegate(object entity)
		{
			settings.SetStringSetting("SandboxTools.SelectedEntity", (entity as KPrefabID).PrefabID().Name);
		}, (object entity) => (entity as KPrefabID).GetProperName(), null, delegate(object entity)
		{
			GameObject prefab = Assets.GetPrefab((entity as KPrefabID).PrefabTag);
			if (prefab != null)
			{
				if (prefab.PrefabID() == MinionConfig.ID)
				{
					return new Tuple<Sprite, Color>(Assets.GetSprite("ui_duplicant_portrait_placeholder"), Color.white);
				}
				KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
				if (component != null && component.AnimFiles.Length != 0 && component.AnimFiles[0] != null)
				{
					return Def.GetUISprite(prefab);
				}
			}
			return null;
		}, UI.SANDBOXTOOLS.SETTINGS.SPAWN_ENTITY.NAME, list.ToArray());
	}

	private void ConfigureDiseaseSelector()
	{
		object[] options = Db.Get().Diseases.resources.ToArray();
		diseaseSelector = new SelectorValue(options, delegate(object disease)
		{
			settings.SetStringSetting("SandboxTools.SelectedDisease", ((Disease)disease).Id);
		}, (object disease) => (disease as Disease).Name, null, (object disease) => new Tuple<Sprite, Color>(Assets.GetSprite("germ"), GlobalAssets.Instance.colorSet.GetColorByName((disease as Disease).overlayColourName)), UI.SANDBOXTOOLS.SETTINGS.DISEASE.NAME);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		if (PlayerController.Instance.ActiveTool != null && instance != null)
		{
			RefreshDisplay();
		}
	}

	public void RefreshDisplay()
	{
		brushRadiusSlider.row.SetActive(PlayerController.Instance.ActiveTool is BrushTool);
		if (PlayerController.Instance.ActiveTool is BrushTool)
		{
			brushRadiusSlider.SetValue(settings.GetIntSetting("SandboxTools.BrushSize"));
		}
		massSlider.SetValue(settings.GetFloatSetting("SandboxTools.Mass"));
		radiationAdditiveSlider.SetValue(settings.GetFloatSetting("SandbosTools.RadiationAdditive"));
		RefreshTemperatureUnitDisplays();
		temperatureSlider.SetValue(GameUtil.GetConvertedTemperature(settings.GetFloatSetting("SandbosTools.Temperature"), roundOutput: true));
		temperatureAdditiveSlider.SetValue(GameUtil.GetConvertedTemperature(settings.GetFloatSetting("SandbosTools.TemperatureAdditive"), roundOutput: true));
		diseaseCountSlider.SetValue(settings.GetIntSetting("SandboxTools.DiseaseCount"));
	}

	private void OnTemperatureUnitChanged(object unit)
	{
		int num = settings.GetIntSetting("SandboxTools.SelectedElement");
		if (num >= ElementLoader.elements.Count)
		{
			num = 0;
		}
		Element absoluteTemperatureSliderRange = ElementLoader.elements[num];
		SetAbsoluteTemperatureSliderRange(absoluteTemperatureSliderRange);
		temperatureAdditiveSlider.SetValue(5f);
	}

	private void SetAbsoluteTemperatureSliderRange(Element element)
	{
		float temperature = Mathf.Max(element.lowTemp - 10f, 1f);
		float temperature2 = ((!element.IsGas) ? Mathf.Min(9999f, element.highTemp + 10f) : Mathf.Min(9999f, element.highTemp + 10f, element.defaultValues.temperature + 100f));
		temperature = GameUtil.GetConvertedTemperature(temperature, roundOutput: true);
		temperature2 = GameUtil.GetConvertedTemperature(temperature2, roundOutput: true);
		temperatureSlider.SetRange(temperature, temperature2, resetCurrentValue: false);
	}

	private void RefreshTemperatureUnitDisplays()
	{
		temperatureSlider.unitString = GameUtil.GetTemperatureUnitSuffix();
		temperatureSlider.row.GetComponent<HierarchyReferences>().GetReference<LocText>("UnitLabel").text = temperatureSlider.unitString;
		temperatureAdditiveSlider.unitString = GameUtil.GetTemperatureUnitSuffix();
		temperatureAdditiveSlider.row.GetComponent<HierarchyReferences>().GetReference<LocText>("UnitLabel").text = temperatureSlider.unitString;
	}

	private GameObject SpawnSelector(SelectorValue selector)
	{
		GameObject gameObject = Util.KInstantiateUI(selectorPropertyPrefab, base.gameObject, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		GameObject panel = component.GetReference("ScrollPanel").gameObject;
		GameObject gameObject2 = component.GetReference("Content").gameObject;
		InputField filterInputField = component.GetReference<InputField>("Filter");
		component.GetReference<LocText>("Label").SetText(selector.labelText);
		Game.Instance.Subscribe(1174281782, delegate
		{
			if (panel.activeSelf)
			{
				panel.SetActive(value: false);
			}
		});
		KButton reference = component.GetReference<KButton>("Button");
		reference.onClick += delegate
		{
			panel.SetActive(!panel.activeSelf);
			if (panel.activeSelf)
			{
				panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
				filterInputField.ActivateInputField();
				filterInputField.onValueChanged.Invoke(filterInputField.text);
			}
		};
		GameObject gameObject3 = component.GetReference("optionPrefab").gameObject;
		selector.row = gameObject;
		selector.optionButtons = new List<KeyValuePair<object, GameObject>>();
		GameObject clearFilterButton = Util.KInstantiateUI(gameObject3, gameObject2);
		clearFilterButton.GetComponentInChildren<LocText>().text = UI.SANDBOXTOOLS.FILTERS.BACK;
		clearFilterButton.GetComponentsInChildren<Image>()[1].enabled = false;
		clearFilterButton.GetComponent<KButton>().onClick += delegate
		{
			selector.currentFilter = null;
			selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
			{
				if (test.Key is SelectorValue.SearchFilter)
				{
					test.Value.SetActive((test.Key as SelectorValue.SearchFilter).parentFilter == null);
				}
				else
				{
					test.Value.SetActive(value: false);
				}
			});
			clearFilterButton.SetActive(value: false);
			panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
			filterInputField.text = "";
			filterInputField.onValueChanged.Invoke(filterInputField.text);
		};
		if (selector.filters != null)
		{
			SelectorValue.SearchFilter[] filters = selector.filters;
			foreach (SelectorValue.SearchFilter filter in filters)
			{
				GameObject gameObject4 = Util.KInstantiateUI(gameObject3, gameObject2);
				gameObject4.SetActive(filter.parentFilter == null);
				gameObject4.GetComponentInChildren<LocText>().text = filter.Name;
				if (filter.icon != null)
				{
					gameObject4.GetComponentsInChildren<Image>()[1].sprite = filter.icon.first;
					gameObject4.GetComponentsInChildren<Image>()[1].color = filter.icon.second;
				}
				gameObject4.GetComponent<KButton>().onClick += delegate
				{
					selector.currentFilter = filter;
					clearFilterButton.SetActive(value: true);
					selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
					{
						if (!(test.Key is SelectorValue.SearchFilter))
						{
							test.Value.SetActive(selector.runCurrentFilter(test.Key));
						}
						else if ((test.Key as SelectorValue.SearchFilter).parentFilter == null)
						{
							test.Value.SetActive(value: false);
						}
						else
						{
							test.Value.SetActive((test.Key as SelectorValue.SearchFilter).parentFilter == filter);
						}
					});
					panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
				};
				selector.optionButtons.Add(new KeyValuePair<object, GameObject>(filter, gameObject4));
			}
		}
		object[] options = selector.options;
		foreach (object option in options)
		{
			GameObject gameObject5 = Util.KInstantiateUI(gameObject3, gameObject2, force_active: true);
			gameObject5.GetComponentInChildren<LocText>().text = selector.getOptionName(option);
			gameObject5.GetComponent<KButton>().onClick += delegate
			{
				selector.onValueChanged(option);
				panel.SetActive(value: false);
			};
			Tuple<Sprite, Color> tuple = selector.getOptionSprite(option);
			gameObject5.GetComponentsInChildren<Image>()[1].sprite = tuple.first;
			gameObject5.GetComponentsInChildren<Image>()[1].color = tuple.second;
			selector.optionButtons.Add(new KeyValuePair<object, GameObject>(option, gameObject5));
			if (option is SelectorValue.SearchFilter)
			{
				gameObject5.SetActive((option as SelectorValue.SearchFilter).parentFilter == null);
			}
			else
			{
				gameObject5.SetActive(value: false);
			}
		}
		selector.button = reference;
		filterInputField.onValueChanged.AddListener(delegate(string filterString)
		{
			if (!clearFilterButton.activeSelf && !string.IsNullOrEmpty(filterString))
			{
				clearFilterButton.SetActive(value: true);
			}
			new List<KeyValuePair<object, GameObject>>();
			bool flag = selector.optionButtons.Find((KeyValuePair<object, GameObject> match) => match.Key is SelectorValue.SearchFilter).Key != null;
			if (string.IsNullOrEmpty(filterString))
			{
				if (!flag)
				{
					selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
					{
						test.Value.SetActive(value: true);
					});
				}
				else
				{
					selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
					{
						if (test.Key is SelectorValue.SearchFilter && ((SelectorValue.SearchFilter)test.Key).parentFilter == null)
						{
							test.Value.SetActive(value: true);
						}
						else
						{
							test.Value.SetActive(value: false);
						}
					});
				}
			}
			else
			{
				selector.optionButtons.ForEach(delegate(KeyValuePair<object, GameObject> test)
				{
					if (test.Key is SelectorValue.SearchFilter)
					{
						test.Value.SetActive(((SelectorValue.SearchFilter)test.Key).Name.ToUpper().Contains(filterString.ToUpper()));
					}
					else
					{
						test.Value.SetActive(selector.getOptionName(test.Key).ToUpper().Contains(filterString.ToUpper()));
					}
				});
			}
			if (selector.filterOptionFunction != null)
			{
				object[] options2 = selector.options;
				object option2 = default(object);
				for (int j = 0; j < options2.Length; j++)
				{
					option2 = options2[j];
					foreach (KeyValuePair<object, GameObject> item in selector.optionButtons.FindAll((KeyValuePair<object, GameObject> match) => match.Key == option2))
					{
						if (string.IsNullOrEmpty(filterString))
						{
							item.Value.SetActive(value: false);
						}
						else
						{
							item.Value.SetActive(selector.filterOptionFunction(filterString, option2));
						}
					}
				}
			}
			panel.GetComponent<KScrollRect>().verticalNormalizedPosition = 1f;
		});
		inputFields.Add(filterInputField.gameObject);
		panel.SetActive(value: false);
		return gameObject;
	}

	private GameObject SpawnSlider(SliderValue value)
	{
		GameObject gameObject = Util.KInstantiateUI(sliderPropertyPrefab, base.gameObject, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("BottomIcon").sprite = Assets.GetSprite(value.bottomSprite);
		component.GetReference<Image>("TopIcon").sprite = Assets.GetSprite(value.topSprite);
		component.GetReference<LocText>("Label").SetText(value.labelText);
		KSlider slider = component.GetReference<KSlider>("Slider");
		KNumberInputField inputField = component.GetReference<KNumberInputField>("InputField");
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(value.tooltip);
		slider.minValue = value.slideMinValue;
		slider.maxValue = value.slideMaxValue;
		inputField.minValue = value.clampValueLow;
		inputField.maxValue = value.clampValueHigh;
		inputFields.Add(inputField.gameObject);
		value.slider = slider;
		inputField.decimalPlaces = value.roundToDecimalPlaces;
		value.inputField = inputField;
		value.row = gameObject;
		slider.onReleaseHandle += delegate
		{
			float value2 = Mathf.Round(slider.value * Mathf.Pow(10f, value.roundToDecimalPlaces)) / Mathf.Pow(10f, value.roundToDecimalPlaces);
			slider.value = value2;
			inputField.currentValue = Mathf.Round(slider.value * Mathf.Pow(10f, value.roundToDecimalPlaces)) / Mathf.Pow(10f, value.roundToDecimalPlaces);
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onDrag += delegate
		{
			float num2 = Mathf.Round(slider.value * Mathf.Pow(10f, value.roundToDecimalPlaces)) / Mathf.Pow(10f, value.roundToDecimalPlaces);
			slider.value = num2;
			inputField.currentValue = num2;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		slider.onMove += delegate
		{
			float num = Mathf.Round(slider.value * Mathf.Pow(10f, value.roundToDecimalPlaces)) / Mathf.Pow(10f, value.roundToDecimalPlaces);
			slider.value = num;
			inputField.currentValue = num;
			inputField.SetDisplayValue(inputField.currentValue.ToString());
			if (value.onValueChanged != null)
			{
				value.onValueChanged(slider.value);
			}
		};
		inputField.onEndEdit += delegate
		{
			float currentValue = inputField.currentValue;
			currentValue = Mathf.Round(currentValue * Mathf.Pow(10f, value.roundToDecimalPlaces)) / Mathf.Pow(10f, value.roundToDecimalPlaces);
			inputField.SetDisplayValue(currentValue.ToString());
			slider.value = currentValue;
			if (value.onValueChanged != null)
			{
				value.onValueChanged(currentValue);
			}
		};
		component.GetReference<LocText>("UnitLabel").text = value.unitString;
		return gameObject;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (CheckBlockedInput())
		{
			if (!e.Consumed)
			{
				e.Consumed = true;
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool CheckBlockedInput()
	{
		bool result = false;
		if (UnityEngine.EventSystems.EventSystem.current != null)
		{
			GameObject currentSelectedGameObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != null)
			{
				foreach (GameObject inputField in inputFields)
				{
					if (currentSelectedGameObject == inputField.gameObject)
					{
						return true;
					}
				}
				return result;
			}
		}
		return result;
	}
}
