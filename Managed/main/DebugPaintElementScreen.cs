using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPaintElementScreen : KScreen
{
	private struct ElemDisplayInfo
	{
		public SimHashes id;

		public string displayStr;
	}

	[Header("Current State")]
	public SimHashes element;

	[NonSerialized]
	public float mass = 1000f;

	[NonSerialized]
	public float temperature = -1f;

	[NonSerialized]
	public bool set_prevent_fow_reveal;

	[NonSerialized]
	public bool set_allow_fow_reveal;

	[NonSerialized]
	public int diseaseCount;

	public byte diseaseIdx;

	[Header("Popup Buttons")]
	[SerializeField]
	private KButton elementButton;

	[SerializeField]
	private KButton diseaseButton;

	[Header("Popup Menus")]
	[SerializeField]
	private KPopupMenu elementPopup;

	[SerializeField]
	private KPopupMenu diseasePopup;

	[Header("Value Inputs")]
	[SerializeField]
	private TMP_InputField massPressureInput;

	[SerializeField]
	private TMP_InputField temperatureInput;

	[SerializeField]
	private TMP_InputField diseaseCountInput;

	[SerializeField]
	private TMP_InputField filterInput;

	[Header("Tool Buttons")]
	[SerializeField]
	private KButton paintButton;

	[SerializeField]
	private KButton fillButton;

	[SerializeField]
	private KButton sampleButton;

	[SerializeField]
	private KButton spawnButton;

	[SerializeField]
	private KButton storeButton;

	[Header("Parameter Toggles")]
	public Toggle paintElement;

	public Toggle paintMass;

	public Toggle paintTemperature;

	public Toggle paintDisease;

	public Toggle paintDiseaseCount;

	public Toggle affectBuildings;

	public Toggle affectCells;

	public Toggle paintPreventFOWReveal;

	public Toggle paintAllowFOWReveal;

	private List<TMP_InputField> inputFields = new List<TMP_InputField>();

	private List<string> options_list = new List<string>();

	private string filter;

	public static DebugPaintElementScreen Instance
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
		Instance = this;
		SetupLocText();
		inputFields.Add(massPressureInput);
		inputFields.Add(temperatureInput);
		inputFields.Add(diseaseCountInput);
		inputFields.Add(filterInput);
		foreach (TMP_InputField inputField in inputFields)
		{
			inputField.onFocus = (System.Action)Delegate.Combine(inputField.onFocus, (System.Action)delegate
			{
				base.isEditing = true;
			});
			inputField.onEndEdit.AddListener(delegate
			{
				base.isEditing = false;
			});
		}
		base.gameObject.SetActive(value: false);
		activateOnSpawn = true;
		base.ConsumeMouseScroll = true;
	}

	private void SetupLocText()
	{
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("Title").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TITLE;
		component.GetReference<LocText>("ElementLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
		component.GetReference<LocText>("MassLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.MASS_KG;
		component.GetReference<LocText>("TemperatureLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.TEMPERATURE_KELVIN;
		component.GetReference<LocText>("DiseaseLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
		component.GetReference<LocText>("DiseaseCountLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE_COUNT;
		component.GetReference<LocText>("AddFoWMaskLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ADD_FOW_MASK;
		component.GetReference<LocText>("RemoveFoWMaskLabel").text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.REMOVE_FOW_MASK;
		elementButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.ELEMENT;
		diseaseButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.DISEASE;
		paintButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.PAINT;
		fillButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.FILL;
		spawnButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.SPAWN_ALL;
		sampleButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.SAMPLE;
		storeButton.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.STORE;
		affectBuildings.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.BUILDINGS;
		affectCells.transform.parent.GetComponentsInChildren<LocText>()[0].text = UI.DEBUG_TOOLS.PAINT_ELEMENTS_SCREEN.CELLS;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		element = SimHashes.Ice;
		diseaseIdx = byte.MaxValue;
		ConfigureElements();
		List<string> list = new List<string>();
		list.Insert(0, "None");
		foreach (Disease resource in Db.Get().Diseases.resources)
		{
			list.Add(resource.Name);
		}
		diseasePopup.SetOptions(list.ToArray());
		KPopupMenu kPopupMenu = diseasePopup;
		kPopupMenu.OnSelect = (Action<string, int>)Delegate.Combine(kPopupMenu.OnSelect, new Action<string, int>(OnSelectDisease));
		SelectDiseaseOption(diseaseIdx);
		paintButton.onClick += OnClickPaint;
		fillButton.onClick += OnClickFill;
		sampleButton.onClick += OnClickSample;
		storeButton.onClick += OnClickStore;
		if (SaveGame.Instance.worldGenSpawner.SpawnsRemain())
		{
			spawnButton.onClick += OnClickSpawn;
		}
		KPopupMenu kPopupMenu2 = elementPopup;
		kPopupMenu2.OnSelect = (Action<string, int>)Delegate.Combine(kPopupMenu2.OnSelect, new Action<string, int>(OnSelectElement));
		elementButton.onClick += elementPopup.OnClick;
		diseaseButton.onClick += diseasePopup.OnClick;
	}

	private void FilterElements(string filterValue)
	{
		if (string.IsNullOrEmpty(filterValue))
		{
			foreach (KButtonMenu.ButtonInfo button in elementPopup.GetButtons())
			{
				button.uibutton.gameObject.SetActive(value: true);
			}
			return;
		}
		filterValue = filter.ToLower();
		foreach (KButtonMenu.ButtonInfo button2 in elementPopup.GetButtons())
		{
			button2.uibutton.gameObject.SetActive(button2.text.ToLower().Contains(filterValue));
		}
	}

	private void ConfigureElements()
	{
		if (filter != null)
		{
			filter = filter.ToLower();
		}
		List<ElemDisplayInfo> list = new List<ElemDisplayInfo>();
		ElemDisplayInfo item;
		foreach (Element element2 in ElementLoader.elements)
		{
			if (element2.name != "Element Not Loaded" && element2.substance != null && element2.substance.showInEditor && (string.IsNullOrEmpty(filter) || element2.name.ToLower().Contains(filter)))
			{
				item = new ElemDisplayInfo
				{
					id = element2.id,
					displayStr = element2.name + " (" + element2.GetStateString() + ")"
				};
				list.Add(item);
			}
		}
		list.Sort((ElemDisplayInfo a, ElemDisplayInfo b) => a.displayStr.CompareTo(b.displayStr));
		if (string.IsNullOrEmpty(filter))
		{
			SimHashes[] array = new SimHashes[6]
			{
				SimHashes.SlimeMold,
				SimHashes.Vacuum,
				SimHashes.Dirt,
				SimHashes.CarbonDioxide,
				SimHashes.Water,
				SimHashes.Oxygen
			};
			for (int i = 0; i < array.Length; i++)
			{
				Element element = ElementLoader.FindElementByHash(array[i]);
				item = new ElemDisplayInfo
				{
					id = element.id,
					displayStr = element.name + " (" + element.GetStateString() + ")"
				};
				list.Insert(0, item);
			}
		}
		options_list = new List<string>();
		List<string> list2 = new List<string>();
		foreach (ElemDisplayInfo item2 in list)
		{
			list2.Add(item2.displayStr);
			options_list.Add(item2.id.ToString());
		}
		elementPopup.SetOptions(list2);
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].id == this.element)
			{
				elementPopup.SelectOption(list2[j], j);
			}
		}
		elementPopup.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0f, 1f);
	}

	private void OnClickSpawn()
	{
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			worldContainer.SetDiscovered(reveal_surface: true);
		}
		SaveGame.Instance.worldGenSpawner.SpawnEverything();
		spawnButton.GetComponent<KButton>().isInteractable = false;
	}

	private void OnClickPaint()
	{
		OnChangeMassPressure();
		OnChangeTemperature();
		OnDiseaseCountChange();
		OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.ReplaceSubstance);
	}

	private void OnClickStore()
	{
		OnChangeMassPressure();
		OnChangeTemperature();
		OnDiseaseCountChange();
		OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.StoreSubstance);
	}

	private void OnClickSample()
	{
		OnChangeMassPressure();
		OnChangeTemperature();
		OnDiseaseCountChange();
		OnChangeFOWReveal();
		DebugTool.Instance.Activate(DebugTool.Type.Sample);
	}

	private void OnClickFill()
	{
		OnChangeMassPressure();
		OnChangeTemperature();
		OnDiseaseCountChange();
		DebugTool.Instance.Activate(DebugTool.Type.FillReplaceSubstance);
	}

	private void OnSelectElement(string str, int index)
	{
		element = (SimHashes)Enum.Parse(typeof(SimHashes), options_list[index]);
		elementButton.GetComponentInChildren<LocText>().text = str;
	}

	private void OnSelectElement(SimHashes element)
	{
		this.element = element;
		elementButton.GetComponentInChildren<LocText>().text = ElementLoader.FindElementByHash(element).name;
	}

	private void OnSelectDisease(string str, int index)
	{
		diseaseIdx = byte.MaxValue;
		for (int i = 0; i < Db.Get().Diseases.Count; i++)
		{
			if (Db.Get().Diseases[i].Name == str)
			{
				diseaseIdx = (byte)i;
			}
		}
		SelectDiseaseOption(diseaseIdx);
	}

	private void SelectDiseaseOption(int diseaseIdx)
	{
		if (diseaseIdx == 255)
		{
			diseaseButton.GetComponentInChildren<LocText>().text = "None";
			return;
		}
		string name = Db.Get().Diseases[diseaseIdx].Name;
		diseaseButton.GetComponentInChildren<LocText>().text = name;
	}

	private void OnChangeFOWReveal()
	{
		if (paintPreventFOWReveal.isOn)
		{
			paintAllowFOWReveal.isOn = false;
		}
		if (paintAllowFOWReveal.isOn)
		{
			paintPreventFOWReveal.isOn = false;
		}
		set_prevent_fow_reveal = paintPreventFOWReveal.isOn;
		set_allow_fow_reveal = paintAllowFOWReveal.isOn;
	}

	public void OnChangeMassPressure()
	{
		float num;
		try
		{
			num = Convert.ToSingle(massPressureInput.text);
		}
		catch
		{
			num = -1f;
		}
		mass = num;
	}

	public void OnChangeTemperature()
	{
		float num;
		try
		{
			num = Convert.ToSingle(temperatureInput.text);
		}
		catch
		{
			num = -1f;
		}
		temperature = num;
	}

	public void OnDiseaseCountChange()
	{
		int num;
		try
		{
			num = Convert.ToInt32(diseaseCountInput.text);
		}
		catch
		{
			num = 0;
		}
		diseaseCount = num;
	}

	public void OnElementsFilterEdited(string new_filter)
	{
		filter = (string.IsNullOrEmpty(filterInput.text) ? null : filterInput.text);
		FilterElements(filter);
	}

	public void SampleCell(int cell)
	{
		massPressureInput.text = (Grid.Pressure[cell] * 0.010000001f).ToString();
		temperatureInput.text = Grid.Temperature[cell].ToString();
		OnSelectElement(ElementLoader.GetElementID(Grid.Element[cell].tag));
		OnChangeMassPressure();
		OnChangeTemperature();
	}
}
