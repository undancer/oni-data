using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/ResearchEntry")]
public class ResearchEntry : KMonoBehaviour
{
	[Header("Labels")]
	[SerializeField]
	private LocText researchName;

	[Header("Transforms")]
	[SerializeField]
	private Transform progressBarContainer;

	[SerializeField]
	private Transform lineContainer;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject iconPanel;

	[SerializeField]
	private GameObject iconPrefab;

	[SerializeField]
	private GameObject linePrefab;

	[SerializeField]
	private GameObject progressBarPrefab;

	[Header("Graphics")]
	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image titleBG;

	[SerializeField]
	private Image borderHighlight;

	[SerializeField]
	private Image filterHighlight;

	[SerializeField]
	private Image filterLowlight;

	[SerializeField]
	private Sprite hoverBG;

	[SerializeField]
	private Sprite completedBG;

	[Header("Colors")]
	[SerializeField]
	private Color defaultColor = Color.blue;

	[SerializeField]
	private Color completedColor = Color.yellow;

	[SerializeField]
	private Color pendingColor = Color.magenta;

	[SerializeField]
	private Color completedHeaderColor = Color.grey;

	[SerializeField]
	private Color incompleteHeaderColor = Color.grey;

	[SerializeField]
	private Color pendingHeaderColor = Color.grey;

	private Sprite defaultBG;

	[MyCmpGet]
	private KToggle toggle;

	private ResearchScreen researchScreen;

	private Dictionary<Tech, UILineRenderer> techLineMap;

	private Tech targetTech;

	private bool isOn = true;

	private Coroutine fadeRoutine;

	public Color activeLineColor;

	public Color inactiveLineColor;

	public int lineThickness_active = 6;

	public int lineThickness_inactive = 2;

	public Material StandardUIMaterial;

	private Dictionary<string, GameObject> progressBarsByResearchTypeID = new Dictionary<string, GameObject>();

	public static readonly string UnlockedTechKey = "UnlockedTech";

	private Dictionary<string, object> unlockedTechMetric = new Dictionary<string, object> { { UnlockedTechKey, null } };

	protected override void OnSpawn()
	{
		base.OnSpawn();
		techLineMap = new Dictionary<Tech, UILineRenderer>();
		BG.color = defaultColor;
		foreach (Tech item in targetTech.requiredTech)
		{
			float num = targetTech.width / 2f + 18f;
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			if (item.center.y > targetTech.center.y + 2f)
			{
				vector = new Vector2(0f, 20f);
				vector2 = new Vector2(0f, -20f);
			}
			else if (item.center.y < targetTech.center.y - 2f)
			{
				vector = new Vector2(0f, -20f);
				vector2 = new Vector2(0f, 20f);
			}
			UILineRenderer component = Util.KInstantiateUI(linePrefab, lineContainer.gameObject, force_active: true).GetComponent<UILineRenderer>();
			float num2 = 32f;
			component.Points = new Vector2[4]
			{
				new Vector2(0f, 0f) + vector,
				new Vector2(0f - num2, 0f) + vector,
				new Vector2(0f - num2, item.center.y - targetTech.center.y) + vector2,
				new Vector2(0f - (targetTech.center.x - num - (item.center.x + num)) + 2f, item.center.y - targetTech.center.y) + vector2
			};
			component.LineThickness = lineThickness_inactive;
			component.color = inactiveLineColor;
			techLineMap.Add(item, component);
		}
		QueueStateChanged(isSelected: false);
		if (targetTech == null)
		{
			return;
		}
		foreach (TechInstance item2 in Research.Instance.GetResearchQueue())
		{
			if (item2.tech == targetTech)
			{
				QueueStateChanged(isSelected: true);
			}
		}
	}

	public void SetTech(Tech newTech)
	{
		if (newTech == null)
		{
			Debug.LogError("The research provided is null!");
		}
		else
		{
			if (targetTech == newTech)
			{
				return;
			}
			foreach (ResearchType type in Research.Instance.researchTypes.Types)
			{
				if (newTech.costsByResearchTypeID.ContainsKey(type.id) && newTech.costsByResearchTypeID[type.id] > 0f)
				{
					GameObject gameObject = Util.KInstantiateUI(progressBarPrefab, progressBarContainer.gameObject, force_active: true);
					Image image = gameObject.GetComponentsInChildren<Image>()[2];
					Image component = gameObject.transform.Find("Icon").GetComponent<Image>();
					image.color = type.color;
					component.sprite = type.sprite;
					progressBarsByResearchTypeID[type.id] = gameObject;
				}
			}
			if (researchScreen == null)
			{
				researchScreen = base.transform.parent.GetComponentInParent<ResearchScreen>();
			}
			if (newTech.IsComplete())
			{
				ResearchCompleted(notify: false);
			}
			targetTech = newTech;
			researchName.text = targetTech.Name;
			string text = "";
			foreach (TechItem unlockedItem in targetTech.unlockedItems)
			{
				HierarchyReferences component2 = GetFreeIcon().GetComponent<HierarchyReferences>();
				if (text != "")
				{
					text += ", ";
				}
				text += unlockedItem.Name;
				component2.GetReference<KImage>("Icon").sprite = unlockedItem.UISprite();
				component2.GetReference<KImage>("Background");
				component2.GetReference<KImage>("DLCOverlay").gameObject.SetActive(!DlcManager.IsValidForVanilla(unlockedItem.dlcIds));
				string text2 = $"{unlockedItem.Name}\n{unlockedItem.description}";
				if (!DlcManager.IsValidForVanilla(unlockedItem.dlcIds))
				{
					text2 += RESEARCH.MESSAGING.DLC.EXPANSION1;
				}
				component2.GetComponent<ToolTip>().toolTip = text2;
			}
			text = string.Format(UI.RESEARCHSCREEN_UNLOCKSTOOLTIP, text);
			researchName.GetComponent<ToolTip>().toolTip = $"{targetTech.Name}\n{targetTech.desc}\n\n{text}";
			toggle.ClearOnClick();
			toggle.onClick += OnResearchClicked;
			toggle.onPointerEnter += delegate
			{
				researchScreen.TurnEverythingOff();
				OnHover(entered: true, targetTech);
			};
			toggle.soundPlayer.AcceptClickCondition = () => !targetTech.IsComplete();
			toggle.onPointerExit += delegate
			{
				researchScreen.TurnEverythingOff();
			};
		}
	}

	public void SetEverythingOff()
	{
		if (!isOn)
		{
			return;
		}
		borderHighlight.gameObject.SetActive(value: false);
		foreach (KeyValuePair<Tech, UILineRenderer> item in techLineMap)
		{
			item.Value.LineThickness = lineThickness_inactive;
			item.Value.color = inactiveLineColor;
		}
		isOn = false;
	}

	public void SetEverythingOn()
	{
		if (isOn)
		{
			return;
		}
		UpdateProgressBars();
		borderHighlight.gameObject.SetActive(value: true);
		foreach (KeyValuePair<Tech, UILineRenderer> item in techLineMap)
		{
			item.Value.LineThickness = lineThickness_active;
			item.Value.color = activeLineColor;
		}
		base.transform.SetAsLastSibling();
		isOn = true;
	}

	public void OnHover(bool entered, Tech hoverSource)
	{
		SetEverythingOn();
		foreach (Tech item in targetTech.requiredTech)
		{
			ResearchEntry entry = researchScreen.GetEntry(item);
			if (entry != null)
			{
				entry.OnHover(entered, targetTech);
			}
		}
	}

	private void OnResearchClicked()
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null && activeResearch.tech != targetTech)
		{
			researchScreen.CancelResearch();
		}
		Research.Instance.SetActiveResearch(targetTech, clearQueue: true);
		if (DebugHandler.InstantBuildMode)
		{
			Research.Instance.CompleteQueue();
		}
		UpdateProgressBars();
	}

	private void OnResearchCanceled()
	{
		if (!targetTech.IsComplete())
		{
			toggle.ClearOnClick();
			toggle.onClick += OnResearchClicked;
			researchScreen.CancelResearch();
			Research.Instance.CancelResearch(targetTech);
		}
	}

	public void QueueStateChanged(bool isSelected)
	{
		if (isSelected)
		{
			if (!targetTech.IsComplete())
			{
				toggle.isOn = true;
				BG.color = pendingColor;
				titleBG.color = pendingHeaderColor;
				toggle.ClearOnClick();
				toggle.onClick += OnResearchCanceled;
			}
			else
			{
				toggle.isOn = false;
			}
			foreach (KeyValuePair<string, GameObject> item in progressBarsByResearchTypeID)
			{
				item.Value.transform.GetChild(0).GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			Image[] componentsInChildren = iconPanel.GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = StandardUIMaterial;
			}
			return;
		}
		if (targetTech.IsComplete())
		{
			toggle.isOn = false;
			BG.color = completedColor;
			titleBG.color = completedHeaderColor;
			defaultColor = completedColor;
			toggle.ClearOnClick();
			foreach (KeyValuePair<string, GameObject> item2 in progressBarsByResearchTypeID)
			{
				item2.Value.transform.GetChild(0).GetComponentsInChildren<Image>()[1].color = Color.white;
			}
			Image[] componentsInChildren = iconPanel.GetComponentsInChildren<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].material = StandardUIMaterial;
			}
			return;
		}
		toggle.isOn = false;
		BG.color = defaultColor;
		titleBG.color = incompleteHeaderColor;
		toggle.ClearOnClick();
		toggle.onClick += OnResearchClicked;
		foreach (KeyValuePair<string, GameObject> item3 in progressBarsByResearchTypeID)
		{
			item3.Value.transform.GetChild(0).GetComponentsInChildren<Image>()[1].color = new Color(0.52156866f, 0.52156866f, 0.52156866f);
		}
	}

	public void UpdateFilterState(bool state)
	{
		bool flag = state;
		filterLowlight.gameObject.SetActive(!flag);
	}

	public void SetPercentage(float percent)
	{
	}

	public void UpdateProgressBars()
	{
		foreach (KeyValuePair<string, GameObject> item in progressBarsByResearchTypeID)
		{
			Transform child = item.Value.transform.GetChild(0);
			float num = 0f;
			if (targetTech.IsComplete())
			{
				num = 1f;
				child.GetComponentInChildren<LocText>().text = targetTech.costsByResearchTypeID[item.Key] + "/" + targetTech.costsByResearchTypeID[item.Key];
			}
			else
			{
				TechInstance orAdd = Research.Instance.GetOrAdd(targetTech);
				if (orAdd == null)
				{
					continue;
				}
				child.GetComponentInChildren<LocText>().text = orAdd.progressInventory.PointsByTypeID[item.Key] + "/" + targetTech.costsByResearchTypeID[item.Key];
				num = orAdd.progressInventory.PointsByTypeID[item.Key] / targetTech.costsByResearchTypeID[item.Key];
			}
			child.GetComponentsInChildren<Image>()[2].fillAmount = num;
			child.GetComponent<ToolTip>().SetSimpleTooltip(Research.Instance.researchTypes.GetResearchType(item.Key).description);
		}
	}

	private GameObject GetFreeIcon()
	{
		GameObject obj = Util.KInstantiateUI(iconPrefab, iconPanel);
		obj.SetActive(value: true);
		return obj;
	}

	private Image GetFreeLine()
	{
		return Util.KInstantiateUI<Image>(linePrefab.gameObject, base.gameObject);
	}

	public void ResearchCompleted(bool notify = true)
	{
		BG.color = completedColor;
		titleBG.color = completedHeaderColor;
		defaultColor = completedColor;
		if (notify)
		{
			unlockedTechMetric[UnlockedTechKey] = targetTech.Id;
			ThreadedHttps<KleiMetrics>.Instance.SendEvent(unlockedTechMetric, "ResearchCompleted");
		}
		toggle.ClearOnClick();
		if (notify)
		{
			ResearchCompleteMessage message = new ResearchCompleteMessage(targetTech);
			MusicManager.instance.PlaySong("Stinger_ResearchComplete");
			Messenger.Instance.QueueMessage(message);
		}
	}
}
