using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuCategoriesScreen : KIconToggleMenu
{
	private class UserData
	{
		public HashedString category;

		public int depth;

		public PlanScreen.RequirementsState requirementsState;

		public ImageToggleState.State? currentToggleState;
	}

	public Action<HashedString, int> onCategoryClicked;

	[SerializeField]
	public bool modalKeyInputBehaviour;

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	private IList<HashedString> subcategories;

	private Dictionary<HashedString, List<BuildingDef>> categorizedBuildingMap;

	private Dictionary<HashedString, List<HashedString>> categorizedCategoryMap;

	private BuildMenuBuildingsScreen buildingsScreen;

	private HashedString category;

	private IList<BuildMenu.BuildingInfo> buildingInfos;

	private HashedString selectedCategory = HashedString.Invalid;

	public HashedString Category => category;

	public override float GetSortKey()
	{
		return 7f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.onSelect += OnClickCategory;
	}

	public void Configure(HashedString category, int depth, object data, Dictionary<HashedString, List<BuildingDef>> categorized_building_map, Dictionary<HashedString, List<HashedString>> categorized_category_map, BuildMenuBuildingsScreen buildings_screen)
	{
		this.category = category;
		categorizedBuildingMap = categorized_building_map;
		categorizedCategoryMap = categorized_category_map;
		buildingsScreen = buildings_screen;
		List<ToggleInfo> list = new List<ToggleInfo>();
		if (typeof(IList<BuildMenu.BuildingInfo>).IsAssignableFrom(data.GetType()))
		{
			buildingInfos = (IList<BuildMenu.BuildingInfo>)data;
		}
		else if (typeof(IList<BuildMenu.DisplayInfo>).IsAssignableFrom(data.GetType()))
		{
			subcategories = new List<HashedString>();
			foreach (BuildMenu.DisplayInfo item2 in (IList<BuildMenu.DisplayInfo>)data)
			{
				string iconName = item2.iconName;
				string text = HashCache.Get().Get(item2.category).ToUpper();
				text = text.Replace(" ", "");
				ToggleInfo item = new ToggleInfo(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".NAME"), iconName, new UserData
				{
					category = item2.category,
					depth = depth,
					requirementsState = PlanScreen.RequirementsState.Tech
				}, item2.hotkey, Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".TOOLTIP"));
				list.Add(item);
				subcategories.Add(item2.category);
			}
			Setup(list);
			toggles.ForEach(delegate(KToggle to)
			{
				ImageToggleState[] components = to.GetComponents<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in components)
				{
					if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
					{
						imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
					}
				}
				to.GetComponent<KToggle>().soundPlayer.Enabled = false;
			});
		}
		UpdateBuildableStates(skip_flourish: true);
	}

	private void OnClickCategory(ToggleInfo toggle_info)
	{
		UserData userData = (UserData)toggle_info.userData;
		PlanScreen.RequirementsState requirementsState = userData.requirementsState;
		if ((uint)(requirementsState - 2) <= 1u)
		{
			if (selectedCategory != userData.category)
			{
				selectedCategory = userData.category;
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
			}
			else
			{
				selectedCategory = HashedString.Invalid;
				ClearSelection();
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
			}
		}
		else
		{
			selectedCategory = HashedString.Invalid;
			ClearSelection();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
		}
		toggle_info.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: false);
		if (onCategoryClicked != null)
		{
			onCategoryClicked(selectedCategory, userData.depth);
		}
	}

	private void UpdateButtonStates()
	{
		if (toggleInfo == null || toggleInfo.Count <= 0)
		{
			return;
		}
		foreach (ToggleInfo item in toggleInfo)
		{
			UserData userData = (UserData)item.userData;
			HashedString hashedString = userData.category;
			PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(hashedString);
			bool flag = categoryRequirements == PlanScreen.RequirementsState.Tech;
			item.toggle.gameObject.SetActive(!flag);
			switch (categoryRequirements)
			{
			case PlanScreen.RequirementsState.Complete:
			{
				ImageToggleState.State state2 = ((!selectedCategory.IsValid || hashedString != selectedCategory) ? ImageToggleState.State.Inactive : ImageToggleState.State.Active);
				if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state2)
				{
					userData.currentToggleState = state2;
					SetImageToggleState(item.toggle.gameObject, state2);
				}
				break;
			}
			case PlanScreen.RequirementsState.Materials:
			{
				item.toggle.fgImage.SetAlpha(flag ? 0.2509804f : 1f);
				ImageToggleState.State state = ((selectedCategory.IsValid && hashedString == selectedCategory) ? ImageToggleState.State.DisabledActive : ImageToggleState.State.Disabled);
				if (!userData.currentToggleState.HasValue || userData.currentToggleState.GetValueOrDefault() != state)
				{
					userData.currentToggleState = state;
					SetImageToggleState(item.toggle.gameObject, state);
				}
				break;
			}
			}
			item.toggle.fgImage.transform.Find("ResearchIcon").gameObject.gameObject.SetActive(flag);
		}
	}

	private void SetImageToggleState(GameObject target, ImageToggleState.State state)
	{
		ImageToggleState[] components = target.GetComponents<ImageToggleState>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].SetState(state);
		}
	}

	private PlanScreen.RequirementsState GetCategoryRequirements(HashedString category)
	{
		bool flag = true;
		bool flag2 = true;
		List<HashedString> value2;
		if (categorizedBuildingMap.TryGetValue(category, out var value))
		{
			if (value.Count > 0)
			{
				foreach (BuildingDef item in value)
				{
					if (item.ShowInBuildMenu && item.IsAvailable())
					{
						PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(item);
						flag = flag && requirementsState == PlanScreen.RequirementsState.Tech;
						flag2 = flag2 && (requirementsState == PlanScreen.RequirementsState.Materials || requirementsState == PlanScreen.RequirementsState.Tech);
					}
				}
			}
		}
		else if (categorizedCategoryMap.TryGetValue(category, out value2))
		{
			foreach (HashedString item2 in value2)
			{
				PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(item2);
				flag = flag && categoryRequirements == PlanScreen.RequirementsState.Tech;
				flag2 = flag2 && (categoryRequirements == PlanScreen.RequirementsState.Materials || categoryRequirements == PlanScreen.RequirementsState.Tech);
			}
		}
		PlanScreen.RequirementsState result = (flag ? PlanScreen.RequirementsState.Tech : ((!flag2) ? PlanScreen.RequirementsState.Complete : PlanScreen.RequirementsState.Materials));
		if (DebugHandler.InstantBuildMode)
		{
			result = PlanScreen.RequirementsState.Complete;
		}
		return result;
	}

	public void UpdateNotifications(ICollection<HashedString> updated_categories)
	{
		if (toggleInfo == null)
		{
			return;
		}
		UpdateBuildableStates(skip_flourish: false);
		foreach (ToggleInfo item2 in toggleInfo)
		{
			HashedString item = ((UserData)item2.userData).category;
			if (updated_categories.Contains(item))
			{
				item2.toggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(active: true);
			}
		}
	}

	public override void Close()
	{
		base.Close();
		selectedCategory = HashedString.Invalid;
		SetHasFocus(has_focus: false);
		if (buildingInfos != null)
		{
			buildingsScreen.Close();
		}
	}

	[ContextMenu("ForceUpdateBuildableStates")]
	private void ForceUpdateBuildableStates()
	{
		UpdateBuildableStates(skip_flourish: true);
	}

	public void UpdateBuildableStates(bool skip_flourish)
	{
		if (subcategories != null && subcategories.Count > 0)
		{
			UpdateButtonStates();
			{
				foreach (ToggleInfo item in toggleInfo)
				{
					UserData userData = (UserData)item.userData;
					HashedString hashedString = userData.category;
					PlanScreen.RequirementsState categoryRequirements = GetCategoryRequirements(hashedString);
					if (userData.requirementsState == categoryRequirements)
					{
						continue;
					}
					userData.requirementsState = categoryRequirements;
					item.userData = userData;
					if (!skip_flourish)
					{
						item.toggle.ActivateFlourish(state: false);
						string stateName = "NotificationPing";
						if (!item.toggle.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag(stateName))
						{
							item.toggle.gameObject.GetComponent<Animator>().Play(stateName);
							BuildMenu.Instance.PlayNewBuildingSounds();
						}
					}
				}
				return;
			}
		}
		buildingsScreen.UpdateBuildableStates();
	}

	protected override void OnShow(bool show)
	{
		if (buildingInfos != null)
		{
			if (show)
			{
				buildingsScreen.Configure(category, buildingInfos);
				buildingsScreen.Show();
			}
			else
			{
				buildingsScreen.Close();
			}
		}
		base.OnShow(show);
	}

	public override void ClearSelection()
	{
		selectedCategory = HashedString.Invalid;
		base.ClearSelection();
		foreach (KToggle toggle in toggles)
		{
			toggle.isOn = false;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (modalKeyInputBehaviour)
		{
			if (!HasFocus)
			{
				return;
			}
			if (e.TryConsume(Action.Escape))
			{
				Game.Instance.Trigger(288942073);
				return;
			}
			base.OnKeyDown(e);
			if (!e.Consumed)
			{
				Action action = e.GetAction();
				if (action >= Action.BUILD_MENU_START_INTERCEPT)
				{
					e.TryConsume(action);
				}
			}
		}
		else
		{
			base.OnKeyDown(e);
			if (e.Consumed)
			{
				UpdateButtonStates();
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (modalKeyInputBehaviour)
		{
			if (!HasFocus)
			{
				return;
			}
			if (e.TryConsume(Action.Escape))
			{
				Game.Instance.Trigger(288942073);
				return;
			}
			base.OnKeyUp(e);
			if (!e.Consumed)
			{
				Action action = e.GetAction();
				if (action >= Action.BUILD_MENU_START_INTERCEPT)
				{
					e.TryConsume(action);
				}
			}
		}
		else
		{
			base.OnKeyUp(e);
		}
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if (focusIndicator != null)
		{
			focusIndicator.color = (has_focus ? focusedColour : unfocusedColour);
		}
	}
}
