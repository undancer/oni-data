using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameplayEventInfoScreen : KModalScreen
{
	[SerializeField]
	private float baseCharacterScale = 0.0057f;

	[FormerlySerializedAs("midgroundPrefab")]
	[FormerlySerializedAs("mid")]
	[Header("Prefabs")]
	[SerializeField]
	private GameObject animPrefab;

	[SerializeField]
	private GameObject optionPrefab;

	[SerializeField]
	private GameObject optionIconPrefab;

	[SerializeField]
	private GameObject optionTextPrefab;

	[Header("Groups")]
	[SerializeField]
	private Transform midgroundGroup;

	[SerializeField]
	private GameObject timeGroup;

	[SerializeField]
	private GameObject buttonsGroup;

	[SerializeField]
	private GameObject chainGroup;

	[Header("Text")]
	[SerializeField]
	private LocText eventHeader;

	[SerializeField]
	private LocText eventTimeLabel;

	[SerializeField]
	private LocText eventLocationLabel;

	[SerializeField]
	private LocText eventDescriptionLabel;

	[SerializeField]
	private bool loadMinionFromPersonalities;

	[SerializeField]
	private LocText chainCount;

	[Header("Button Colour Styles")]
	[SerializeField]
	private ColorStyleSetting neutralButtonSetting;

	[SerializeField]
	private ColorStyleSetting badButtonSetting;

	[SerializeField]
	private ColorStyleSetting goodButtonSetting;

	[Header("Backgrounds")]
	[SerializeField]
	private Image foregroundImage2;

	[SerializeField]
	private Image foregroundImage1;

	[SerializeField]
	private Image backgroundImage1;

	[SerializeField]
	private Image backgroundImage2;

	private List<KBatchedAnimController> createdAnimations = new List<KBatchedAnimController>();

	public override bool IsModal()
	{
		return true;
	}

	public void SetEventData(GameplayEventPopupData data)
	{
		data.FinalizeText();
		eventHeader.text = string.Format(UI.GAMEPLAY_EVENT_INFO_SCREEN.TITLE, data.title);
		eventDescriptionLabel.text = data.description;
		eventLocationLabel.text = data.location;
		eventTimeLabel.text = data.whenDescription;
		if (data.location.IsNullOrWhiteSpace() && data.location.IsNullOrWhiteSpace())
		{
			timeGroup.gameObject.SetActive(value: false);
		}
		if (data.options.Count == 0)
		{
			data.AddDefaultOption();
		}
		SetEventDataOptions(data);
		SetEventDataVisuals(data);
	}

	private void SetEventDataOptions(GameplayEventPopupData data)
	{
		foreach (GameplayEventPopupData.PopupOption option in data.options)
		{
			GameObject gameObject = Util.KInstantiateUI(optionPrefab, buttonsGroup);
			gameObject.name = "Option: " + option.mainText;
			KButton component = gameObject.GetComponent<KButton>();
			component.isInteractable = option.allowed;
			component.onClick += delegate
			{
				if (option.callback != null)
				{
					option.callback();
				}
				Deactivate();
			};
			if (!option.tooltip.IsNullOrWhiteSpace())
			{
				gameObject.GetComponent<ToolTip>().SetSimpleTooltip(option.tooltip);
			}
			else
			{
				gameObject.GetComponent<ToolTip>().enabled = false;
			}
			foreach (GameplayEventPopupData.PopupOptionIcon informationIcon in option.informationIcons)
			{
				CreateOptionIcon(gameObject, informationIcon);
			}
			Util.KInstantiateUI(optionTextPrefab, gameObject).GetComponent<LocText>().text = ((option.description == null) ? ("<b>" + option.mainText + "</b>") : ("<b>" + option.mainText + "</b>\n<i>(" + option.description + ")</i>"));
			foreach (GameplayEventPopupData.PopupOptionIcon consequenceIcon in option.consequenceIcons)
			{
				CreateOptionIcon(gameObject, consequenceIcon);
			}
			gameObject.SetActive(value: true);
		}
	}

	private void CreateOptionIcon(GameObject option, GameplayEventPopupData.PopupOptionIcon optionIcon)
	{
		GameObject gameObject = Util.KInstantiateUI(optionIconPrefab, option);
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(optionIcon.tooltip);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Mask");
		Image reference2 = component.GetReference<Image>("Border");
		Image reference3 = component.GetReference<Image>("Icon");
		if (optionIcon.sprite != null)
		{
			reference3.transform.localScale *= optionIcon.scale;
		}
		Color32 c = Color.white;
		switch (optionIcon.containerType)
		{
		case GameplayEventPopupData.PopupOptionIcon.ContainerType.Neutral:
			reference.sprite = Assets.GetSprite("container_fill_neutral");
			reference2.sprite = Assets.GetSprite("container_border_neutral");
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("knob");
			}
			c = GlobalAssets.Instance.colorSet.eventNeutral;
			break;
		case GameplayEventPopupData.PopupOptionIcon.ContainerType.Positive:
			reference.sprite = Assets.GetSprite("container_fill_positive");
			reference2.sprite = Assets.GetSprite("container_border_positive");
			reference3.rectTransform.localPosition += Vector3.down * 1f;
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("icon_positive");
			}
			c = GlobalAssets.Instance.colorSet.eventPositive;
			break;
		case GameplayEventPopupData.PopupOptionIcon.ContainerType.Negative:
			reference.sprite = Assets.GetSprite("container_fill_negative");
			reference2.sprite = Assets.GetSprite("container_border_negative");
			reference3.rectTransform.localPosition += Vector3.up * 1f;
			c = GlobalAssets.Instance.colorSet.eventNegative;
			if (optionIcon.sprite == null)
			{
				optionIcon.sprite = Assets.GetSprite("cancel");
			}
			break;
		case GameplayEventPopupData.PopupOptionIcon.ContainerType.Information:
			reference.sprite = Assets.GetSprite("requirements");
			reference2.enabled = false;
			break;
		}
		reference.color = c;
		reference3.sprite = optionIcon.sprite;
		if (optionIcon.sprite == null)
		{
			reference3.gameObject.SetActive(value: false);
		}
	}

	private void SetEventDataVisuals(GameplayEventPopupData data)
	{
		createdAnimations.ForEach(delegate(KBatchedAnimController x)
		{
			Object.Destroy(x);
		});
		createdAnimations.Clear();
		Sprite sprite = Assets.GetSprite(data.backgroundFileName);
		if (sprite != null)
		{
			backgroundImage1.sprite = sprite;
			backgroundImage1.color = data.backgroundTint;
		}
		else
		{
			backgroundImage1.sprite = Assets.GetSprite("event_bg_01");
			DebugUtil.LogWarningArgs("No background set for '" + data.title + "'");
		}
		KAnimFile anim = Assets.GetAnim(data.animFileName);
		if (anim == null)
		{
			Debug.LogWarning("Event " + data.title + " has no anim data");
			return;
		}
		KBatchedAnimController component = CreateAnimLayer(midgroundGroup, anim, "event").transform.GetComponent<KBatchedAnimController>();
		if (data.minions != null)
		{
			for (int i = 0; i < data.minions.Length; i++)
			{
				if (data.minions[i] == null)
				{
					DebugUtil.LogWarningArgs($"GameplayEventInfoScreen unable to display minion {i}");
				}
				string s = $"dupe{i + 1:D2}";
				if (component.HasAnimation(s))
				{
					CreateAnimLayer(midgroundGroup, anim, s, data.minions[i]);
				}
			}
		}
		if (data.artifact != null)
		{
			string s2 = "artifact";
			if (component.HasAnimation(s2))
			{
				CreateAnimLayer(midgroundGroup, anim, s2, null, data.artifact);
			}
		}
	}

	private GameObject CreateAnimLayer(Transform parent, KAnimFile animFile, HashedString animName, GameObject minion = null, GameObject artifact = null, string targetSymbol = null)
	{
		GameObject gameObject = Object.Instantiate(animPrefab, parent);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		createdAnimations.Add(component);
		component.AnimFiles = new KAnimFile[4]
		{
			Assets.GetAnim("body_comp_default_kanim"),
			Assets.GetAnim("head_swap_kanim"),
			Assets.GetAnim("body_swap_kanim"),
			animFile
		};
		if (minion != null)
		{
			SymbolOverrideController component2 = component.GetComponent<SymbolOverrideController>();
			if (loadMinionFromPersonalities)
			{
				component.GetComponent<UIDupeSymbolOverride>().Apply(minion.GetComponent<MinionIdentity>());
			}
			else
			{
				SymbolOverrideController.SymbolEntry[] getSymbolOverrides = minion.GetComponent<SymbolOverrideController>().GetSymbolOverrides;
				for (int i = 0; i < getSymbolOverrides.Length; i++)
				{
					SymbolOverrideController.SymbolEntry symbolEntry = getSymbolOverrides[i];
					component2.AddSymbolOverride(symbolEntry.targetSymbol, symbolEntry.sourceSymbol, symbolEntry.priority);
				}
			}
			MinionConfig.ConfigureSymbols(gameObject);
		}
		if (artifact != null)
		{
			SymbolOverrideController component3 = component.GetComponent<SymbolOverrideController>();
			KBatchedAnimController component4 = artifact.GetComponent<KBatchedAnimController>();
			string initialAnim = component4.initialAnim;
			initialAnim = initialAnim.Replace("idle_", "artifact_");
			initialAnim = initialAnim.Replace("_loop", "");
			KAnim.Build.Symbol symbol = component4.AnimFiles[0].GetData().build.GetSymbol(initialAnim);
			if (symbol != null)
			{
				component3.AddSymbolOverride("snapTo_artifact", symbol);
			}
		}
		if (targetSymbol != null)
		{
			gameObject.AddOrGet<KBatchedAnimTracker>().symbol = targetSymbol;
		}
		gameObject.SetActive(value: true);
		component.Play(animName, KAnim.PlayMode.Loop);
		component.animScale = baseCharacterScale;
		return gameObject;
	}
}
