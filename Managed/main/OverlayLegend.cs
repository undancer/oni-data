using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OverlayLegend : KScreen
{
	[Serializable]
	public class OverlayInfoUnit
	{
		public Sprite icon;

		public string description;

		public string tooltip;

		public Color color;

		public Color fontColor;

		public object formatData;

		public object tooltipFormatData;

		public bool sliceIcon;

		public OverlayInfoUnit(Sprite icon, string description, Color color, Color fontColor, object formatData = null, bool sliceIcon = false)
		{
			this.icon = icon;
			this.description = description;
			this.color = color;
			this.fontColor = fontColor;
			this.formatData = formatData;
			this.sliceIcon = sliceIcon;
		}
	}

	[Serializable]
	public class OverlayInfo
	{
		public string name;

		public HashedString mode;

		public List<OverlayInfoUnit> infoUnits;

		public List<GameObject> diagrams;

		public bool isProgrammaticallyPopulated;
	}

	public static OverlayLegend Instance;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private Sprite emptySprite;

	[SerializeField]
	private List<OverlayInfo> overlayInfoList;

	[SerializeField]
	private GameObject unitPrefab;

	[SerializeField]
	private GameObject activeUnitsParent;

	[SerializeField]
	private GameObject diagramsParent;

	[SerializeField]
	private GameObject inactiveUnitsParent;

	[SerializeField]
	private GameObject toolParameterMenuPrefab;

	private ToolParameterMenu filterMenu;

	private OverlayModes.Mode currentMode;

	private List<GameObject> inactiveUnitObjs;

	private List<GameObject> activeUnitObjs;

	private List<GameObject> activeDiagrams = new List<GameObject>();

	[ContextMenu("Set all fonts color")]
	public void SetAllFontsColor()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if (overlayInfo.infoUnits[i].fontColor == Color.clear)
				{
					overlayInfo.infoUnits[i].fontColor = Color.white;
				}
			}
		}
	}

	[ContextMenu("Set all tooltips")]
	public void SetAllTooltips()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			string text = overlayInfo.name;
			text = text.Replace("NAME", "");
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				string description = overlayInfo.infoUnits[i].description;
				description = description.Replace(text, "");
				description = text + "TOOLTIPS." + description;
				overlayInfo.infoUnits[i].tooltip = description;
			}
		}
	}

	[ContextMenu("Set Sliced for empty icons")]
	public void SetSlicedForEmptyIcons()
	{
		foreach (OverlayInfo overlayInfo in overlayInfoList)
		{
			for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
			{
				if (overlayInfo.infoUnits[i].icon == emptySprite)
				{
					overlayInfo.infoUnits[i].sliceIcon = true;
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Instance == null)
		{
			Instance = this;
			activeUnitObjs = new List<GameObject>();
			inactiveUnitObjs = new List<GameObject>();
			foreach (OverlayInfo overlayInfo in overlayInfoList)
			{
				overlayInfo.name = Strings.Get(overlayInfo.name);
				for (int i = 0; i < overlayInfo.infoUnits.Count; i++)
				{
					overlayInfo.infoUnits[i].description = Strings.Get(overlayInfo.infoUnits[i].description);
					if (!string.IsNullOrEmpty(overlayInfo.infoUnits[i].tooltip))
					{
						overlayInfo.infoUnits[i].tooltip = Strings.Get(overlayInfo.infoUnits[i].tooltip);
					}
				}
			}
			GetComponent<LayoutElement>().minWidth = (DlcManager.FeatureClusterSpaceEnabled() ? 322 : 288);
			ClearLegend();
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	protected override void OnLoadLevel()
	{
		Instance = null;
		activeDiagrams.Clear();
		UnityEngine.Object.Destroy(base.gameObject);
		base.OnLoadLevel();
	}

	private void SetLegend(OverlayInfo overlayInfo)
	{
		if (overlayInfo == null)
		{
			ClearLegend();
			return;
		}
		if (!overlayInfo.isProgrammaticallyPopulated && (overlayInfo.infoUnits == null || overlayInfo.infoUnits.Count == 0))
		{
			ClearLegend();
			return;
		}
		Show();
		title.text = overlayInfo.name;
		if (overlayInfo.isProgrammaticallyPopulated)
		{
			PopulateGeneratedLegend(overlayInfo);
		}
		else
		{
			PopulateOverlayInfoUnits(overlayInfo);
		}
	}

	public void SetLegend(OverlayModes.Mode mode, bool refreshing = false)
	{
		if (currentMode == null || !(currentMode.ViewMode() == mode.ViewMode()) || refreshing)
		{
			ClearLegend();
			OverlayInfo legend = overlayInfoList.Find((OverlayInfo ol) => ol.mode == mode.ViewMode());
			currentMode = mode;
			SetLegend(legend);
		}
	}

	public GameObject GetFreeUnitObject()
	{
		GameObject gameObject = null;
		if (inactiveUnitObjs.Count == 0)
		{
			inactiveUnitObjs.Add(Util.KInstantiateUI(unitPrefab, inactiveUnitsParent));
		}
		gameObject = inactiveUnitObjs[0];
		inactiveUnitObjs.RemoveAt(0);
		activeUnitObjs.Add(gameObject);
		return gameObject;
	}

	private void RemoveActiveObjects()
	{
		while (activeUnitObjs.Count > 0)
		{
			activeUnitObjs[0].transform.Find("Icon").GetComponent<Image>().enabled = false;
			activeUnitObjs[0].GetComponentInChildren<LocText>().enabled = false;
			activeUnitObjs[0].transform.SetParent(inactiveUnitsParent.transform);
			activeUnitObjs[0].SetActive(value: false);
			inactiveUnitObjs.Add(activeUnitObjs[0]);
			activeUnitObjs.RemoveAt(0);
		}
	}

	public void ClearLegend()
	{
		RemoveActiveObjects();
		for (int i = 0; i < activeDiagrams.Count; i++)
		{
			if (activeDiagrams[i] != null)
			{
				UnityEngine.Object.Destroy(activeDiagrams[i]);
			}
		}
		activeDiagrams.Clear();
		Vector2 sizeDelta = diagramsParent.GetComponent<RectTransform>().sizeDelta;
		sizeDelta.y = 0f;
		diagramsParent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
		Show(show: false);
	}

	public OverlayInfo GetOverlayInfo(OverlayModes.Mode mode)
	{
		for (int i = 0; i < overlayInfoList.Count; i++)
		{
			if (overlayInfoList[i].mode == mode.ViewMode())
			{
				return overlayInfoList[i];
			}
		}
		return null;
	}

	private void PopulateOverlayInfoUnits(OverlayInfo overlayInfo, bool isRefresh = false)
	{
		if (overlayInfo.infoUnits != null && overlayInfo.infoUnits.Count > 0)
		{
			activeUnitsParent.SetActive(value: true);
			foreach (OverlayInfoUnit infoUnit in overlayInfo.infoUnits)
			{
				GameObject freeUnitObject = GetFreeUnitObject();
				if (infoUnit.icon != null)
				{
					Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
					component.gameObject.SetActive(value: true);
					component.sprite = infoUnit.icon;
					component.color = infoUnit.color;
					component.enabled = true;
					component.type = (infoUnit.sliceIcon ? Image.Type.Sliced : Image.Type.Simple);
				}
				else
				{
					freeUnitObject.transform.Find("Icon").gameObject.SetActive(value: false);
				}
				if (!string.IsNullOrEmpty(infoUnit.description))
				{
					LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
					componentInChildren.text = string.Format(infoUnit.description, infoUnit.formatData);
					componentInChildren.color = infoUnit.fontColor;
					componentInChildren.enabled = true;
				}
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				if (!string.IsNullOrEmpty(infoUnit.tooltip))
				{
					component2.toolTip = string.Format(infoUnit.tooltip, infoUnit.tooltipFormatData);
					component2.enabled = true;
				}
				else
				{
					component2.enabled = false;
				}
				freeUnitObject.SetActive(value: true);
				freeUnitObject.transform.SetParent(activeUnitsParent.transform);
			}
		}
		else
		{
			activeUnitsParent.SetActive(value: false);
		}
		if (isRefresh)
		{
			return;
		}
		if (overlayInfo.diagrams != null && overlayInfo.diagrams.Count > 0)
		{
			diagramsParent.SetActive(value: true);
			{
				foreach (GameObject diagram in overlayInfo.diagrams)
				{
					GameObject item = Util.KInstantiateUI(diagram, diagramsParent);
					activeDiagrams.Add(item);
				}
				return;
			}
		}
		diagramsParent.SetActive(value: false);
	}

	private void PopulateGeneratedLegend(OverlayInfo info, bool isRefresh = false)
	{
		if (isRefresh)
		{
			RemoveActiveObjects();
		}
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info, isRefresh);
		}
		List<LegendEntry> customLegendData = currentMode.GetCustomLegendData();
		if (customLegendData != null)
		{
			activeUnitsParent.SetActive(value: true);
			foreach (LegendEntry item in customLegendData)
			{
				GameObject freeUnitObject = GetFreeUnitObject();
				Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
				component.gameObject.SetActive(item.displaySprite);
				component.sprite = item.sprite;
				component.color = item.colour;
				component.enabled = true;
				component.type = Image.Type.Simple;
				LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
				componentInChildren.text = item.name;
				componentInChildren.color = Color.white;
				componentInChildren.enabled = true;
				ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
				component2.enabled = item.desc != null || item.desc_arg != null;
				component2.toolTip = ((item.desc_arg == null) ? item.desc : string.Format(item.desc, item.desc_arg));
				freeUnitObject.SetActive(value: true);
				freeUnitObject.transform.SetParent(activeUnitsParent.transform);
			}
		}
		else
		{
			activeUnitsParent.SetActive(value: false);
		}
		if (!isRefresh && currentMode.legendFilters != null)
		{
			GameObject gameObject = Util.KInstantiateUI(toolParameterMenuPrefab, diagramsParent);
			activeDiagrams.Add(gameObject);
			diagramsParent.SetActive(value: true);
			filterMenu = gameObject.GetComponent<ToolParameterMenu>();
			filterMenu.PopulateMenu(currentMode.legendFilters);
			filterMenu.onParametersChanged += OnFiltersChanged;
			OnFiltersChanged();
		}
	}

	private void OnFiltersChanged()
	{
		currentMode.OnFiltersChanged();
		PopulateGeneratedLegend(GetOverlayInfo(currentMode), isRefresh: true);
		Game.Instance.ForceOverlayUpdate();
	}

	private void DisableOverlay()
	{
		filterMenu.onParametersChanged -= OnFiltersChanged;
		filterMenu.ClearMenu();
		filterMenu.gameObject.SetActive(value: false);
		filterMenu = null;
	}

	private void PopulateNoiseLegend(OverlayInfo info)
	{
		if (info.infoUnits != null && info.infoUnits.Count > 0)
		{
			PopulateOverlayInfoUnits(info);
		}
		string[] names = Enum.GetNames(typeof(AudioEventManager.NoiseEffect));
		Array values = Enum.GetValues(typeof(AudioEventManager.NoiseEffect));
		Color[] dbColours = SimDebugView.dbColours;
		for (int i = 0; i < names.Length; i++)
		{
			GameObject freeUnitObject = GetFreeUnitObject();
			Image component = freeUnitObject.transform.Find("Icon").GetComponent<Image>();
			component.gameObject.SetActive(value: true);
			component.sprite = Assets.instance.LegendColourBox;
			component.color = ((i == 0) ? new Color(1f, 1f, 1f, 0.7f) : Color.Lerp(dbColours[i * 2], dbColours[Mathf.Min(dbColours.Length - 1, i * 2 + 1)], 0.5f));
			component.enabled = true;
			component.type = Image.Type.Simple;
			string text = names[i].ToUpper();
			int num = (int)values.GetValue(i);
			int num2 = (int)values.GetValue(i);
			LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
			componentInChildren.text = string.Concat(Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.NAMES." + text), " ", string.Format(UI.OVERLAYS.NOISE_POLLUTION.RANGE, num));
			componentInChildren.color = Color.white;
			componentInChildren.enabled = true;
			ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
			component2.enabled = true;
			component2.toolTip = string.Format(Strings.Get("STRINGS.UI.OVERLAYS.NOISE_POLLUTION.TOOLTIPS." + text), num, num2);
			freeUnitObject.SetActive(value: true);
			freeUnitObject.transform.SetParent(activeUnitsParent.transform);
		}
	}
}
