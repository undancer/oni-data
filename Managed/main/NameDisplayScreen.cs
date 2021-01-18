using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplayScreen : KScreen
{
	[Serializable]
	public class Entry
	{
		public string Name;

		public GameObject world_go;

		public GameObject display_go;

		public GameObject bars_go;

		public HealthBar healthBar;

		public ProgressBar breathBar;

		public ProgressBar suitBar;

		public ProgressBar suitFuelBar;

		public ProgressBar suitBatteryBar;

		public HierarchyReferences thoughtBubble;

		public HierarchyReferences thoughtBubbleConvo;

		public HierarchyReferences gameplayEventDisplay;

		public HierarchyReferences refs;
	}

	public class TextEntry
	{
		public Guid guid;

		public GameObject display_go;
	}

	[SerializeField]
	private float HideDistance;

	public static NameDisplayScreen Instance;

	public GameObject nameAndBarsPrefab;

	public GameObject barsPrefab;

	public TextStyleSetting ToolTipStyle_Property;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color defaultColor;

	public int fontsize_min = 14;

	public int fontsize_max = 32;

	public float cameraDistance_fontsize_min = 6f;

	public float cameraDistance_fontsize_max = 4f;

	public List<Entry> entries = new List<Entry>();

	public List<TextEntry> textEntries = new List<TextEntry>();

	public bool worldSpace = true;

	private List<KCollider2D> workingList = new List<KCollider2D>();

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UIRegistry.nameDisplayScreen = this;
		Components.Health.Register(OnHealthAdded, null);
		Components.Equipment.Register(OnEquipmentAdded, null);
	}

	private void OnHealthAdded(Health health)
	{
		RegisterComponent(health.gameObject, health);
	}

	private void OnEquipmentAdded(Equipment equipment)
	{
		MinionAssignablesProxy component = equipment.GetComponent<MinionAssignablesProxy>();
		GameObject targetGameObject = component.GetTargetGameObject();
		if ((bool)targetGameObject)
		{
			RegisterComponent(targetGameObject, equipment);
			return;
		}
		Debug.LogWarningFormat("OnEquipmentAdded proxy target {0} was null.", component.TargetInstanceID);
	}

	private bool ShouldShowName(GameObject representedObject)
	{
		CharacterOverlay component = representedObject.GetComponent<CharacterOverlay>();
		return component != null && component.shouldShowName;
	}

	public Guid AddWorldText(string initialText, GameObject prefab)
	{
		TextEntry textEntry = new TextEntry();
		textEntry.guid = Guid.NewGuid();
		textEntry.display_go = Util.KInstantiateUI(prefab, base.gameObject, force_active: true);
		textEntry.display_go.GetComponentInChildren<LocText>().text = initialText;
		textEntries.Add(textEntry);
		return textEntry.guid;
	}

	public GameObject GetWorldText(Guid guid)
	{
		GameObject result = null;
		foreach (TextEntry textEntry in textEntries)
		{
			if (textEntry.guid == guid)
			{
				result = textEntry.display_go;
				break;
			}
		}
		return result;
	}

	public void RemoveWorldText(Guid guid)
	{
		int num = -1;
		for (int i = 0; i < textEntries.Count; i++)
		{
			if (textEntries[i].guid == guid)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			UnityEngine.Object.Destroy(textEntries[num].display_go);
			textEntries.RemoveAt(num);
		}
	}

	public void AddNewEntry(GameObject representedObject)
	{
		Entry entry = new Entry();
		entry.world_go = representedObject;
		GameObject original = (ShouldShowName(representedObject) ? nameAndBarsPrefab : barsPrefab);
		GameObject gameObject = (entry.display_go = Util.KInstantiateUI(original, base.gameObject, force_active: true));
		if (worldSpace)
		{
			entry.display_go.transform.localScale = Vector3.one * 0.01f;
		}
		gameObject.name = representedObject.name + " character overlay";
		entry.Name = representedObject.name;
		entry.refs = gameObject.GetComponent<HierarchyReferences>();
		entries.Add(entry);
		KSelectable component = representedObject.GetComponent<KSelectable>();
		FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
		if (!(component != null))
		{
			return;
		}
		if (component2 != null)
		{
			if (component2.Alignment == FactionManager.FactionID.Friendly || component2.Alignment == FactionManager.FactionID.Duplicant)
			{
				UpdateName(representedObject);
			}
		}
		else
		{
			UpdateName(representedObject);
		}
	}

	public void RegisterComponent(GameObject representedObject, object component, bool force_new_entry = false)
	{
		Entry entry = (force_new_entry ? null : GetEntry(representedObject));
		if (entry == null)
		{
			CharacterOverlay component2 = representedObject.GetComponent<CharacterOverlay>();
			if (component2 != null)
			{
				component2.Register();
				entry = GetEntry(representedObject);
			}
		}
		if (entry == null)
		{
			return;
		}
		Transform reference = entry.refs.GetReference<Transform>("Bars");
		entry.bars_go = reference.gameObject;
		if (component is Health)
		{
			if (!entry.healthBar)
			{
				Health health = (Health)component;
				GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, reference.gameObject);
				gameObject.name = "Health Bar";
				health.healthBar = gameObject.GetComponent<HealthBar>();
				health.healthBar.GetComponent<KSelectable>().entityName = UI.METERS.HEALTH.TOOLTIP;
				health.healthBar.GetComponent<KSelectableHealthBar>().IsSelectable = representedObject.GetComponent<MinionBrain>() != null;
				entry.healthBar = health.healthBar;
				entry.healthBar.autoHide = false;
				gameObject.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
			}
			else
			{
				Debug.LogWarningFormat("Health added twice {0}", component);
			}
		}
		else if (component is OxygenBreather)
		{
			if (!entry.breathBar)
			{
				GameObject gameObject2 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject);
				entry.breathBar = gameObject2.GetComponent<ProgressBar>();
				entry.breathBar.autoHide = false;
				gameObject2.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", ToolTipStyle_Property);
				gameObject2.name = "Breath Bar";
				gameObject2.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
				gameObject2.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
			}
			else
			{
				Debug.LogWarningFormat("OxygenBreather added twice {0}", component);
			}
		}
		else if (component is Equipment)
		{
			if (!entry.suitBar)
			{
				GameObject gameObject3 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject);
				entry.suitBar = gameObject3.GetComponent<ProgressBar>();
				entry.suitBar.autoHide = false;
				gameObject3.name = "Suit Tank Bar";
				gameObject3.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
				gameObject3.GetComponent<KSelectable>().entityName = UI.METERS.BREATH.TOOLTIP;
			}
			else
			{
				Debug.LogWarningFormat("SuitBar added twice {0}", component);
			}
			if (!entry.suitFuelBar)
			{
				GameObject gameObject4 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject);
				entry.suitFuelBar = gameObject4.GetComponent<ProgressBar>();
				entry.suitFuelBar.autoHide = false;
				gameObject4.name = "Suit Fuel Bar";
				gameObject4.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("FuelTankBar");
				gameObject4.GetComponent<KSelectable>().entityName = UI.METERS.FUEL.TOOLTIP;
			}
			else
			{
				Debug.LogWarningFormat("FuelBar added twice {0}", component);
			}
			if (!entry.suitBatteryBar)
			{
				GameObject gameObject5 = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, reference.gameObject);
				entry.suitBatteryBar = gameObject5.GetComponent<ProgressBar>();
				entry.suitBatteryBar.autoHide = false;
				gameObject5.name = "Suit Battery Bar";
				gameObject5.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("BatteryBar");
				gameObject5.GetComponent<KSelectable>().entityName = UI.METERS.BATTERY.TOOLTIP;
			}
			else
			{
				Debug.LogWarningFormat("CoolantBar added twice {0}", component);
			}
		}
		else if (component is ThoughtGraph.Instance)
		{
			if (!entry.thoughtBubble)
			{
				GameObject gameObject6 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubble, entry.display_go);
				entry.thoughtBubble = gameObject6.GetComponent<HierarchyReferences>();
				gameObject6.name = "Thought Bubble";
				GameObject gameObject7 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubbleConvo, entry.display_go);
				entry.thoughtBubbleConvo = gameObject7.GetComponent<HierarchyReferences>();
				gameObject7.name = "Thought Bubble Convo";
			}
			else
			{
				Debug.LogWarningFormat("ThoughtGraph added twice {0}", component);
			}
		}
		else if (component is GameplayEventMonitor.Instance)
		{
			if (!entry.gameplayEventDisplay)
			{
				GameObject gameObject8 = Util.KInstantiateUI(EffectPrefabs.Instance.GameplayEventDisplay, entry.display_go);
				entry.gameplayEventDisplay = gameObject8.GetComponent<HierarchyReferences>();
				gameObject8.name = "Gameplay Event Display";
			}
			else
			{
				Debug.LogWarningFormat("GameplayEventDisplay added twice {0}", component);
			}
		}
	}

	private void LateUpdate()
	{
		if (App.isLoading || App.IsExiting)
		{
			return;
		}
		HashedString x = OverlayModes.None.ID;
		if (OverlayScreen.Instance != null)
		{
			x = OverlayScreen.Instance.GetMode();
		}
		bool flag = !(Camera.main == null) && Camera.main.orthographicSize < HideDistance && x == OverlayModes.None.ID;
		int num = entries.Count;
		int num2 = 0;
		while (num2 < num)
		{
			if (entries[num2].world_go != null)
			{
				Vector3 vector = entries[num2].world_go.transform.GetPosition();
				if (flag && CameraController.Instance.IsVisiblePos(vector))
				{
					RectTransform component = entries[num2].display_go.GetComponent<RectTransform>();
					if (CameraController.Instance != null && CameraController.Instance.followTarget == entries[num2].world_go.transform)
					{
						vector = CameraController.Instance.followTargetPos;
					}
					else
					{
						KAnimControllerBase component2 = entries[num2].world_go.GetComponent<KAnimControllerBase>();
						if (component2 != null)
						{
							vector = component2.GetWorldPivot();
						}
					}
					component.anchoredPosition = (worldSpace ? vector : WorldToScreen(vector));
					entries[num2].display_go.SetActive(value: true);
				}
				else if (entries[num2].display_go.activeSelf)
				{
					entries[num2].display_go.SetActive(value: false);
				}
				if (entries[num2].world_go.HasTag(GameTags.Dead))
				{
					entries[num2].bars_go.SetActive(value: false);
				}
				if (entries[num2].bars_go != null)
				{
					GameObject bars_go = entries[num2].bars_go;
					bars_go.GetComponentsInChildren(includeInactive: false, workingList);
					foreach (KCollider2D working in workingList)
					{
						working.MarkDirty();
					}
				}
				num2++;
			}
			else
			{
				UnityEngine.Object.Destroy(entries[num2].display_go);
				num--;
				entries[num2] = entries[num];
			}
		}
		entries.RemoveRange(num, entries.Count - num);
	}

	public void UpdateName(GameObject representedObject)
	{
		Entry entry = GetEntry(representedObject);
		if (entry == null)
		{
			return;
		}
		KSelectable component = representedObject.GetComponent<KSelectable>();
		entry.display_go.name = component.GetProperName() + " character overlay";
		LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = component.GetProperName();
			if (representedObject.GetComponent<RocketModule>() != null)
			{
				componentInChildren.text = representedObject.GetComponent<RocketModule>().GetParentRocketName();
			}
		}
	}

	public void SetThoughtBubbleDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.thoughtBubble == null))
		{
			ApplyThoughtSprite(entry.thoughtBubble, bubble_sprite, "bubble_sprite");
			ApplyThoughtSprite(entry.thoughtBubble, topic_sprite, "icon_sprite");
			entry.thoughtBubble.GetComponent<KSelectable>().entityName = hover_text;
			entry.thoughtBubble.gameObject.SetActive(bVisible);
		}
	}

	public void SetThoughtBubbleConvoDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite bubble_sprite, Sprite topic_sprite, Sprite mode_sprite)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.thoughtBubble == null))
		{
			ApplyThoughtSprite(entry.thoughtBubbleConvo, bubble_sprite, "bubble_sprite");
			ApplyThoughtSprite(entry.thoughtBubbleConvo, topic_sprite, "icon_sprite");
			ApplyThoughtSprite(entry.thoughtBubbleConvo, mode_sprite, "icon_sprite_mode");
			entry.thoughtBubbleConvo.GetComponent<KSelectable>().entityName = hover_text;
			entry.thoughtBubbleConvo.gameObject.SetActive(bVisible);
		}
	}

	private void ApplyThoughtSprite(HierarchyReferences active_bubble, Sprite sprite, string target)
	{
		Image reference = active_bubble.GetReference<Image>(target);
		reference.sprite = sprite;
	}

	public void SetGameplayEventDisplay(GameObject minion_go, bool bVisible, string hover_text, Sprite sprite)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.gameplayEventDisplay == null))
		{
			entry.gameplayEventDisplay.GetReference<Image>("icon_sprite").sprite = sprite;
			entry.gameplayEventDisplay.GetComponent<KSelectable>().entityName = hover_text;
			entry.gameplayEventDisplay.gameObject.SetActive(bVisible);
		}
	}

	public void SetBreathDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.breathBar == null))
		{
			entry.breathBar.SetUpdateFunc(updatePercentFull);
			entry.breathBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetHealthDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.healthBar == null))
		{
			entry.healthBar.OnChange();
			entry.healthBar.SetUpdateFunc(updatePercentFull);
			if (entry.healthBar.gameObject.activeSelf != bVisible)
			{
				entry.healthBar.gameObject.SetActive(bVisible);
			}
		}
	}

	public void SetSuitTankDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.suitBar == null))
		{
			entry.suitBar.SetUpdateFunc(updatePercentFull);
			entry.suitBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetSuitFuelDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.suitFuelBar == null))
		{
			entry.suitFuelBar.SetUpdateFunc(updatePercentFull);
			entry.suitFuelBar.gameObject.SetActive(bVisible);
		}
	}

	public void SetSuitBatteryDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
	{
		Entry entry = GetEntry(minion_go);
		if (entry != null && !(entry.suitBatteryBar == null))
		{
			entry.suitBatteryBar.SetUpdateFunc(updatePercentFull);
			entry.suitBatteryBar.gameObject.SetActive(bVisible);
		}
	}

	private Entry GetEntry(GameObject worldObject)
	{
		return entries.Find((Entry entry) => entry.world_go == worldObject);
	}
}
