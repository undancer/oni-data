using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AchievementWidget")]
public class AchievementWidget : KMonoBehaviour
{
	private Color color_dark_red = new Color(24f / 85f, 41f / 255f, 38f / 255f);

	private Color color_gold = new Color(1f, 54f / 85f, 73f / 255f);

	private Color color_dark_grey = new Color(11f / 51f, 11f / 51f, 11f / 51f);

	private Color color_grey = new Color(176f / 255f, 176f / 255f, 176f / 255f);

	[SerializeField]
	private RectTransform sheenTransform;

	public AnimationCurve flourish_iconScaleCurve;

	public AnimationCurve flourish_sheenPositionCurve;

	public KBatchedAnimController[] sparks;

	[SerializeField]
	private RectTransform progressParent;

	[SerializeField]
	private GameObject requirementPrefab;

	[SerializeField]
	private Sprite statusSuccessIcon;

	[SerializeField]
	private Sprite statusFailureIcon;

	private int numRequirementsDisplayed;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle component = GetComponent<MultiToggle>();
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			ExpandAchievement();
		});
	}

	private void Update()
	{
	}

	private void ExpandAchievement()
	{
		if (SaveGame.Instance != null)
		{
			progressParent.gameObject.SetActive(!progressParent.gameObject.activeSelf);
		}
	}

	public void ActivateNewlyAchievedFlourish(float delay = 1f)
	{
		StartCoroutine(Flourish(delay));
	}

	private IEnumerator Flourish(float startDelay)
	{
		SetNeverAchieved();
		if (GetComponent<Canvas>() == null)
		{
			base.gameObject.AddComponent<Canvas>().sortingOrder = 1;
		}
		GetComponent<Canvas>().overrideSorting = true;
		yield return new WaitForSecondsRealtime(startDelay);
		KScrollRect component = base.transform.parent.parent.GetComponent<KScrollRect>();
		float smoothAutoScrollTarget = 1f + base.transform.localPosition.y / component.content.rect.height;
		component.SetSmoothAutoScrollTarget(smoothAutoScrollTarget);
		GameObject icon = GetComponent<HierarchyReferences>().GetReference<Image>("icon").transform.parent.gameObject;
		KBatchedAnimController[] array = sparks;
		foreach (KBatchedAnimController kBatchedAnimController in array)
		{
			if (kBatchedAnimController.transform.parent != icon.transform.parent)
			{
				kBatchedAnimController.GetComponent<KBatchedAnimController>().TintColour = new Color(1f, 0.86f, 0.56f, 1f);
				kBatchedAnimController.transform.SetParent(icon.transform.parent);
				kBatchedAnimController.transform.SetSiblingIndex(icon.transform.GetSiblingIndex());
				kBatchedAnimController.GetComponent<KBatchedAnimCanvasRenderer>().compare = CompareFunction.Always;
			}
		}
		HierarchyReferences component2 = GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = color_dark_red;
		component2.GetReference<Image>("iconBorder").color = color_gold;
		component2.GetReference<Image>("icon").color = color_gold;
		bool colorChanged = false;
		EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("AchievementUnlocked"), Vector3.zero);
		int num = Mathf.RoundToInt(MathUtil.Clamp(1f, 7f, startDelay - startDelay % 1f / 1f)) - 1;
		instance.setParameterByName("num_achievements", num);
		KFMOD.EndOneShot(instance);
		for (float j = 0f; j < 1.2f; j += Time.unscaledDeltaTime)
		{
			icon.transform.localScale = Vector3.one * flourish_iconScaleCurve.Evaluate(j);
			sheenTransform.anchoredPosition = new Vector2(flourish_sheenPositionCurve.Evaluate(j), sheenTransform.anchoredPosition.y);
			if (j > 1f && !colorChanged)
			{
				colorChanged = true;
				array = sparks;
				for (int k = 0; k < array.Length; k++)
				{
					array[k].Play("spark");
				}
				SetAchievedNow();
			}
			yield return 0;
		}
		icon.transform.localScale = Vector3.one;
		for (float j = 0f; j < 0.3f; j += Time.unscaledDeltaTime)
		{
			yield return 0;
		}
		GetComponent<Canvas>().overrideSorting = false;
		base.transform.localScale = Vector3.one;
	}

	public void SetAchievedNow()
	{
		GetComponent<MultiToggle>().ChangeState(1);
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = color_dark_red;
		component.GetReference<Image>("iconBorder").color = color_gold;
		component.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = Color.white;
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
	}

	public void SetAchievedBefore()
	{
		GetComponent<MultiToggle>().ChangeState(1);
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = color_dark_red;
		component.GetReference<Image>("iconBorder").color = color_gold;
		component.GetReference<Image>("icon").color = color_gold;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = Color.white;
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
	}

	public void SetNeverAchieved()
	{
		GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = color_dark_grey;
		component.GetReference<Image>("iconBorder").color = color_grey;
		component.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
	}

	public void SetNotAchieved()
	{
		GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = color_dark_grey;
		component.GetReference<Image>("iconBorder").color = color_grey;
		component.GetReference<Image>("icon").color = color_grey;
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
	}

	public void SetFailed()
	{
		GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = color_dark_grey;
		component.GetReference<Image>("iconBG").SetAlpha(0.5f);
		component.GetReference<Image>("iconBorder").color = color_grey;
		component.GetReference<Image>("iconBorder").SetAlpha(0.5f);
		component.GetReference<Image>("icon").color = color_grey;
		component.GetReference<Image>("icon").SetAlpha(0.5f);
		LocText[] componentsInChildren = GetComponentsInChildren<LocText>();
		foreach (LocText locText in componentsInChildren)
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.25f);
		}
		ConfigureToolTip(GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.FAILED_THIS_COLONY);
	}

	private void ConfigureToolTip(ToolTip tooltip, string status)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(status, null);
		if (SaveGame.Instance != null && !progressParent.gameObject.activeSelf)
		{
			tooltip.AddMultiStringTooltip(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXPAND_TOOLTIP, null);
		}
	}

	public void ShowProgress(ColonyAchievementStatus achievement)
	{
		if (progressParent == null)
		{
			return;
		}
		numRequirementsDisplayed = 0;
		for (int i = 0; i < achievement.Requirements.Count; i++)
		{
			ColonyAchievementRequirement colonyAchievementRequirement = achievement.Requirements[i];
			if (colonyAchievementRequirement is CritterTypesWithTraits)
			{
				ShowCritterChecklist(colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is DupesCompleteChoreInExoSuitForCycles)
			{
				ShowDupesInExoSuitsRequirement(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is DupesVsSolidTransferArmFetch)
			{
				ShowArmsOutPeformingDupesRequirement(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is ProduceXEngeryWithoutUsingYList)
			{
				ShowEngeryWithoutUsing(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is MinimumMorale)
			{
				ShowMinimumMoraleRequirement(achievement.success, colonyAchievementRequirement);
			}
			else
			{
				ShowRequirement(achievement.success, colonyAchievementRequirement);
			}
		}
	}

	private HierarchyReferences GetNextRequirementWidget()
	{
		GameObject gameObject;
		if (progressParent.childCount <= numRequirementsDisplayed)
		{
			gameObject = Util.KInstantiateUI(requirementPrefab, progressParent.gameObject, force_active: true);
		}
		else
		{
			gameObject = progressParent.GetChild(numRequirementsDisplayed).gameObject;
			gameObject.SetActive(value: true);
		}
		numRequirementsDisplayed++;
		return gameObject.GetComponent<HierarchyReferences>();
	}

	private void SetDescription(string str, HierarchyReferences refs)
	{
		refs.GetReference<LocText>("Desc").SetText(str);
	}

	private void SetIcon(Sprite sprite, Color color, HierarchyReferences refs)
	{
		Image reference = refs.GetReference<Image>("Icon");
		reference.sprite = sprite;
		reference.color = color;
		reference.gameObject.SetActive(value: true);
	}

	private void ShowIcon(bool show, HierarchyReferences refs)
	{
		refs.GetReference<Image>("Icon").gameObject.SetActive(show);
	}

	private void ShowRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
		bool flag = req.Success() || succeed;
		bool flag2 = req.Fail();
		if (flag && !flag2)
		{
			SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		else if (flag2)
		{
			SetIcon(statusFailureIcon, Color.red, nextRequirementWidget);
		}
		else
		{
			ShowIcon(show: false, nextRequirementWidget);
		}
		SetDescription(req.GetProgress(flag), nextRequirementWidget);
	}

	private void ShowCritterChecklist(ColonyAchievementRequirement req)
	{
		CritterTypesWithTraits critterTypesWithTraits = req as CritterTypesWithTraits;
		if (req == null)
		{
			return;
		}
		foreach (KeyValuePair<Tag, bool> item in critterTypesWithTraits.critterTypesToCheck)
		{
			HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
			if (item.Value)
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			else
			{
				ShowIcon(show: false, nextRequirementWidget);
			}
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TAME_A_CRITTER, GameTagExtensions.ProperName(item.Key.Name)), nextRequirementWidget);
		}
	}

	private void ShowArmsOutPeformingDupesRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesVsSolidTransferArmFetch dupesVsSolidTransferArmFetch = req as DupesVsSolidTransferArmFetch;
		if (dupesVsSolidTransferArmFetch == null)
		{
			return;
		}
		HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
		if (succeed)
		{
			SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_PERFORMANCE, succeed ? dupesVsSolidTransferArmFetch.numCycles : dupesVsSolidTransferArmFetch.currentCycleCount, dupesVsSolidTransferArmFetch.numCycles), nextRequirementWidget);
		if (!succeed)
		{
			Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
			Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
			int value = 0;
			fetchDupeChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out value);
			int value2 = 0;
			fetchAutomatedChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out value2);
			nextRequirementWidget = GetNextRequirementWidget();
			if ((float)value < (float)value2 * dupesVsSolidTransferArmFetch.percentage)
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			else
			{
				ShowIcon(show: false, nextRequirementWidget);
			}
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_VS_DUPE_FETCHES, "SolidTransferArm", value2, value), nextRequirementWidget);
		}
	}

	private void ShowDupesInExoSuitsRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesCompleteChoreInExoSuitForCycles dupesCompleteChoreInExoSuitForCycles = req as DupesCompleteChoreInExoSuitForCycles;
		if (dupesCompleteChoreInExoSuitForCycles == null)
		{
			return;
		}
		HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
		if (succeed)
		{
			SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		else
		{
			ShowIcon(show: false, nextRequirementWidget);
		}
		SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_CYCLES, succeed ? dupesCompleteChoreInExoSuitForCycles.numCycles : dupesCompleteChoreInExoSuitForCycles.currentCycleStreak, dupesCompleteChoreInExoSuitForCycles.numCycles), nextRequirementWidget);
		if (!succeed)
		{
			nextRequirementWidget = GetNextRequirementWidget();
			int num = dupesCompleteChoreInExoSuitForCycles.GetNumberOfDupesForCycle(GameClock.Instance.GetCycle());
			if (num >= Components.LiveMinionIdentities.Count)
			{
				num = Components.LiveMinionIdentities.Count;
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_THIS_CYCLE, num, Components.LiveMinionIdentities.Count), nextRequirementWidget);
		}
	}

	private void ShowEngeryWithoutUsing(bool succeed, ColonyAchievementRequirement req)
	{
		ProduceXEngeryWithoutUsingYList produceXEngeryWithoutUsingYList = req as ProduceXEngeryWithoutUsingYList;
		if (req == null)
		{
			return;
		}
		HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
		float productionAmount = produceXEngeryWithoutUsingYList.GetProductionAmount(succeed);
		SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.GENERATE_POWER, GameUtil.GetFormattedRoundedJoules(productionAmount), GameUtil.GetFormattedRoundedJoules(produceXEngeryWithoutUsingYList.amountToProduce * 1000f)), nextRequirementWidget);
		if (succeed)
		{
			SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		else
		{
			ShowIcon(show: false, nextRequirementWidget);
		}
		foreach (Tag disallowedBuilding in produceXEngeryWithoutUsingYList.disallowedBuildings)
		{
			nextRequirementWidget = GetNextRequirementWidget();
			if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(disallowedBuilding))
			{
				SetIcon(statusFailureIcon, Color.red, nextRequirementWidget);
			}
			else
			{
				SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			BuildingDef buildingDef = Assets.GetBuildingDef(disallowedBuilding.Name);
			SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_BUILDING, buildingDef.Name), nextRequirementWidget);
		}
	}

	private void ShowMinimumMoraleRequirement(bool success, ColonyAchievementRequirement req)
	{
		MinimumMorale minimumMorale = req as MinimumMorale;
		if (minimumMorale == null)
		{
			return;
		}
		if (success)
		{
			ShowRequirement(success, req);
			return;
		}
		foreach (MinionAssignablesProxy item in Components.MinionAssignablesProxy)
		{
			GameObject targetGameObject = item.GetTargetGameObject();
			if (!(targetGameObject != null) || targetGameObject.HasTag(GameTags.Dead))
			{
				continue;
			}
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
			if (attributeInstance != null)
			{
				HierarchyReferences nextRequirementWidget = GetNextRequirementWidget();
				if (attributeInstance.GetTotalValue() >= (float)minimumMorale.minimumMorale)
				{
					SetIcon(statusSuccessIcon, Color.green, nextRequirementWidget);
				}
				else
				{
					ShowIcon(show: false, nextRequirementWidget);
				}
				SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.MORALE, targetGameObject.GetProperName(), attributeInstance.GetTotalDisplayValue()), nextRequirementWidget);
			}
		}
	}
}
