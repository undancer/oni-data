using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchScreenSideBar : KScreen
{
	private enum CompletionState
	{
		All,
		Available,
		Completed
	}

	[Header("Containers")]
	[SerializeField]
	private GameObject queueContainer;

	[SerializeField]
	private GameObject projectsContainer;

	[SerializeField]
	private GameObject searchFiltersContainer;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject headerTechTypePrefab;

	[SerializeField]
	private GameObject filterButtonPrefab;

	[SerializeField]
	private GameObject techWidgetRootPrefab;

	[SerializeField]
	private GameObject techWidgetRootAltPrefab;

	[SerializeField]
	private GameObject techItemPrefab;

	[SerializeField]
	private GameObject techWidgetUnlockedItemPrefab;

	[SerializeField]
	private GameObject techWidgetRowPrefab;

	[SerializeField]
	private GameObject techCategoryPrefab;

	[SerializeField]
	private GameObject techCategoryPrefabAlt;

	[Header("Other references")]
	[SerializeField]
	private TMP_InputField searchBox;

	[SerializeField]
	private MultiToggle allFilter;

	[SerializeField]
	private MultiToggle availableFilter;

	[SerializeField]
	private MultiToggle completedFilter;

	[SerializeField]
	private ResearchScreen researchScreen;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private Color evenRowColor;

	[SerializeField]
	private Color oddRowColor;

	private CompletionState completionFilter;

	private Dictionary<string, bool> filterStates = new Dictionary<string, bool>();

	private Dictionary<string, bool> categoryExpanded = new Dictionary<string, bool>();

	private string currentSearchString = "";

	private Dictionary<string, GameObject> queueTechs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> projectTechs = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> projectCategories = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> filterButtons = new Dictionary<string, GameObject>();

	private Dictionary<string, Dictionary<string, GameObject>> projectTechItems = new Dictionary<string, Dictionary<string, GameObject>>();

	private Dictionary<string, List<Tag>> filterPresets = new Dictionary<string, List<Tag>>
	{
		{
			"Oxygen",
			new List<Tag>()
		},
		{
			"Food",
			new List<Tag>()
		},
		{
			"Water",
			new List<Tag>()
		},
		{
			"Power",
			new List<Tag>()
		},
		{
			"Morale",
			new List<Tag>()
		},
		{
			"Ranching",
			new List<Tag>()
		},
		{
			"Filter",
			new List<Tag>()
		},
		{
			"Tile",
			new List<Tag>()
		},
		{
			"Transport",
			new List<Tag>()
		},
		{
			"Automation",
			new List<Tag>()
		},
		{
			"Medicine",
			new List<Tag>()
		},
		{
			"Rocket",
			new List<Tag>()
		}
	};

	private List<GameObject> QueuedActivations = new List<GameObject>();

	private List<GameObject> QueuedDeactivations = new List<GameObject>();

	[SerializeField]
	private int activationPerFrame = 5;

	private bool evenRow = false;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		PopualteProjects();
		PopulateFilterButtons();
		RefreshCategoriesContentExpanded();
		RefreshWidgets();
		searchBox.onValueChanged.AddListener(UpdateCurrentSearch);
		TMP_InputField tMP_InputField = searchBox;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			base.isEditing = true;
		});
		searchBox.onEndEdit.AddListener(delegate
		{
			base.isEditing = false;
		});
		clearSearchButton.onClick += delegate
		{
			searchBox.text = "";
			foreach (KeyValuePair<string, GameObject> filterButton in filterButtons)
			{
				filterStates[filterButton.Key] = false;
				filterButtons[filterButton.Key].GetComponent<MultiToggle>().ChangeState(filterStates[filterButton.Key] ? 1 : 0);
			}
		};
		ConfigCompletionFilters();
		base.ConsumeMouseScroll = true;
		Game.Instance.Subscribe(-107300940, UpdateProjectFilter);
	}

	private void Update()
	{
		for (int i = 0; i < Math.Min(QueuedActivations.Count, activationPerFrame); i++)
		{
			QueuedActivations[i].SetActive(value: true);
		}
		QueuedActivations.RemoveRange(0, Math.Min(QueuedActivations.Count, activationPerFrame));
		for (int j = 0; j < Math.Min(QueuedDeactivations.Count, activationPerFrame); j++)
		{
			QueuedDeactivations[j].SetActive(value: false);
		}
		QueuedDeactivations.RemoveRange(0, Math.Min(QueuedDeactivations.Count, activationPerFrame));
	}

	private void ConfigCompletionFilters()
	{
		MultiToggle multiToggle = allFilter;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SetCompletionFilter(CompletionState.All);
		});
		MultiToggle multiToggle2 = completedFilter;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			SetCompletionFilter(CompletionState.Completed);
		});
		MultiToggle multiToggle3 = availableFilter;
		multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, (System.Action)delegate
		{
			SetCompletionFilter(CompletionState.Available);
		});
		SetCompletionFilter(CompletionState.All);
	}

	private void SetCompletionFilter(CompletionState state)
	{
		completionFilter = state;
		allFilter.GetComponent<MultiToggle>().ChangeState((completionFilter == CompletionState.All) ? 1 : 0);
		completedFilter.GetComponent<MultiToggle>().ChangeState((completionFilter == CompletionState.Completed) ? 1 : 0);
		availableFilter.GetComponent<MultiToggle>().ChangeState((completionFilter == CompletionState.Available) ? 1 : 0);
		UpdateProjectFilter();
	}

	public override float GetSortKey()
	{
		if (base.isEditing)
		{
			return 50f;
		}
		return 21f;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (base.isEditing)
		{
			e.Consumed = true;
		}
		else if (!e.Consumed)
		{
			Vector2 vector = base.transform.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
			if (vector.x >= 0f && vector.x <= base.transform.rectTransform().rect.width && !e.TryConsume(Action.MouseRight) && !e.TryConsume(Action.MouseLeft) && !e.TryConsume(Action.ZoomIn) && !e.TryConsume(Action.ZoomOut))
			{
			}
		}
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		RefreshWidgets();
	}

	private void UpdateCurrentSearch(string newValue)
	{
		if (base.isEditing)
		{
			foreach (KeyValuePair<string, GameObject> filterButton in filterButtons)
			{
				filterStates[filterButton.Key] = false;
				filterButton.Value.GetComponent<MultiToggle>().ChangeState(0);
			}
		}
		currentSearchString = newValue;
		UpdateProjectFilter();
	}

	private void UpdateProjectFilter(object data = null)
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (KeyValuePair<string, GameObject> projectCategory in projectCategories)
		{
			dictionary.Add(projectCategory.Key, value: false);
		}
		RefreshProjectsActive();
		foreach (KeyValuePair<string, GameObject> projectTech in projectTechs)
		{
			if ((projectTech.Value.activeSelf || QueuedActivations.Contains(projectTech.Value)) && !QueuedDeactivations.Contains(projectTech.Value))
			{
				dictionary[Db.Get().Techs.Get(projectTech.Key).category] = true;
				categoryExpanded[Db.Get().Techs.Get(projectTech.Key).category] = true;
			}
		}
		foreach (KeyValuePair<string, bool> item in dictionary)
		{
			ChangeGameObjectActive(projectCategories[item.Key], item.Value);
		}
		RefreshCategoriesContentExpanded();
	}

	private void RefreshProjectsActive()
	{
		foreach (KeyValuePair<string, GameObject> projectTech in projectTechs)
		{
			bool flag = CheckTechPassesFilters(projectTech.Key);
			ChangeGameObjectActive(projectTech.Value, flag);
			researchScreen.GetEntry(Db.Get().Techs.Get(projectTech.Key)).UpdateFilterState(flag);
			foreach (KeyValuePair<string, GameObject> item in projectTechItems[projectTech.Key])
			{
				bool flag2 = CheckTechItemPassesFilters(item.Key);
				HierarchyReferences component = item.Value.GetComponent<HierarchyReferences>();
				component.GetReference<LocText>("Label").color = (flag2 ? Color.white : Color.grey);
				component.GetReference<Image>("Icon").color = (flag2 ? Color.white : new Color(1f, 1f, 1f, 0.5f));
			}
		}
	}

	private void RefreshCategoriesContentExpanded()
	{
		foreach (KeyValuePair<string, GameObject> projectCategory in projectCategories)
		{
			projectCategory.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject.SetActive(categoryExpanded[projectCategory.Key]);
			projectCategory.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(categoryExpanded[projectCategory.Key] ? 1 : 0);
		}
	}

	private void PopualteProjects()
	{
		List<Tuple<Tuple<string, GameObject>, int>> list = new List<Tuple<Tuple<string, GameObject>, int>>();
		for (int i = 0; i < Db.Get().Techs.Count; i++)
		{
			Tech tech = (Tech)Db.Get().Techs.GetResource(i);
			if (!projectCategories.ContainsKey(tech.category))
			{
				string categoryID = tech.category;
				GameObject gameObject = Util.KInstantiateUI(techCategoryPrefabAlt, projectsContainer, force_active: true);
				gameObject.name = categoryID;
				gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + categoryID.ToUpper()));
				categoryExpanded.Add(categoryID, value: false);
				projectCategories.Add(categoryID, gameObject);
				gameObject.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = delegate
				{
					categoryExpanded[categoryID] = !categoryExpanded[categoryID];
					RefreshCategoriesContentExpanded();
				};
			}
			GameObject gameObject2 = SpawnTechWidget(tech.Id, projectCategories[tech.category]);
			list.Add(new Tuple<Tuple<string, GameObject>, int>(new Tuple<string, GameObject>(tech.Id, gameObject2), tech.tier));
			projectTechs.Add(tech.Id, gameObject2);
			gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(tech.desc);
			MultiToggle component = gameObject2.GetComponent<MultiToggle>();
			component.onEnter = (System.Action)Delegate.Combine(component.onEnter, (System.Action)delegate
			{
				researchScreen.TurnEverythingOff();
				researchScreen.GetEntry(tech).OnHover(entered: true, tech);
			});
			MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
			component2.onExit = (System.Action)Delegate.Combine(component2.onExit, (System.Action)delegate
			{
				researchScreen.TurnEverythingOff();
			});
		}
		foreach (KeyValuePair<string, GameObject> projectTech in projectTechs)
		{
			Transform reference = projectCategories[Db.Get().Techs.Get(projectTech.Key).category].GetComponent<HierarchyReferences>().GetReference<Transform>("Content");
			projectTechs[projectTech.Key].transform.SetParent(reference);
		}
		list.Sort((Tuple<Tuple<string, GameObject>, int> a, Tuple<Tuple<string, GameObject>, int> b) => a.second.CompareTo(b.second));
		foreach (Tuple<Tuple<string, GameObject>, int> item in list)
		{
			item.first.second.transform.SetAsLastSibling();
		}
	}

	private void PopulateFilterButtons()
	{
		foreach (KeyValuePair<string, List<Tag>> kvp in filterPresets)
		{
			GameObject gameObject = Util.KInstantiateUI(filterButtonPrefab, searchFiltersContainer, force_active: true);
			filterButtons.Add(kvp.Key, gameObject);
			filterStates.Add(kvp.Key, value: false);
			MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
			LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
			componentInChildren.SetText(Strings.Get("STRINGS.UI.RESEARCHSCREEN.FILTER_BUTTONS." + kvp.Key.ToUpper()));
			MultiToggle multiToggle = toggle;
			multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
			{
				foreach (KeyValuePair<string, GameObject> filterButton in filterButtons)
				{
					if (filterButton.Key != kvp.Key)
					{
						filterStates[filterButton.Key] = false;
						filterButtons[filterButton.Key].GetComponent<MultiToggle>().ChangeState(filterStates[filterButton.Key] ? 1 : 0);
					}
				}
				filterStates[kvp.Key] = !filterStates[kvp.Key];
				toggle.ChangeState(filterStates[kvp.Key] ? 1 : 0);
				if (filterStates[kvp.Key])
				{
					searchBox.text = kvp.Key;
				}
				else
				{
					searchBox.text = "";
				}
			});
		}
	}

	public void RefreshQueue()
	{
	}

	private void RefreshWidgets()
	{
		List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
		foreach (KeyValuePair<string, GameObject> kvp in projectTechs)
		{
			if (Db.Get().Techs.Get(kvp.Key).IsComplete())
			{
				kvp.Value.GetComponent<MultiToggle>().ChangeState(2);
			}
			else if (researchQueue.Find((TechInstance match) => match.tech.Id == kvp.Key) != null)
			{
				kvp.Value.GetComponent<MultiToggle>().ChangeState(1);
			}
			else
			{
				kvp.Value.GetComponent<MultiToggle>().ChangeState(0);
			}
		}
	}

	private void RefreshWidgetProgressBars(string techID, GameObject widget)
	{
		HierarchyReferences component = widget.GetComponent<HierarchyReferences>();
		ResearchPointInventory progressInventory = Research.Instance.GetTechInstance(techID).progressInventory;
		int num = 0;
		for (int i = 0; i < Research.Instance.researchTypes.Types.Count; i++)
		{
			if (Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID.ContainsKey(Research.Instance.researchTypes.Types[i].id) && Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id] > 0f)
			{
				Transform child = component.GetReference<RectTransform>("BarRows").GetChild(1 + num);
				HierarchyReferences component2 = child.GetComponent<HierarchyReferences>();
				float num2 = progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[i].id] / Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id];
				RectTransform rectTransform = component2.GetReference<Image>("Bar").rectTransform;
				rectTransform.sizeDelta = new Vector2(rectTransform.parent.rectTransform().rect.width * num2, rectTransform.sizeDelta.y);
				component2.GetReference<LocText>("Label").SetText(progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[i].id].ToString() + "/" + Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[i].id]);
				num++;
			}
		}
	}

	private GameObject SpawnTechWidget(string techID, GameObject parentContainer)
	{
		GameObject gameObject = Util.KInstantiateUI(techWidgetRootAltPrefab, parentContainer, force_active: true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		gameObject.name = Db.Get().Techs.Get(techID).Name;
		component.GetReference<LocText>("Label").SetText(Db.Get().Techs.Get(techID).Name);
		if (!projectTechItems.ContainsKey(techID))
		{
			projectTechItems.Add(techID, new Dictionary<string, GameObject>());
		}
		RectTransform reference = component.GetReference<RectTransform>("UnlockContainer");
		foreach (TechItem unlockedItem in Db.Get().Techs.Get(techID).unlockedItems)
		{
			GameObject gameObject2 = Util.KInstantiateUI(techItemPrefab, reference.gameObject, force_active: true);
			gameObject2.GetComponentsInChildren<Image>()[1].sprite = unlockedItem.UISprite();
			gameObject2.GetComponentsInChildren<LocText>()[0].SetText(unlockedItem.Name);
			MultiToggle component2 = gameObject2.GetComponent<MultiToggle>();
			component2.onClick = (System.Action)Delegate.Combine(component2.onClick, (System.Action)delegate
			{
				researchScreen.ZoomToTech(techID);
			});
			gameObject2.GetComponentsInChildren<Image>()[0].color = (evenRow ? evenRowColor : oddRowColor);
			evenRow = !evenRow;
			if (!projectTechItems[techID].ContainsKey(unlockedItem.Id))
			{
				projectTechItems[techID].Add(unlockedItem.Id, gameObject2);
			}
		}
		MultiToggle component3 = gameObject.GetComponent<MultiToggle>();
		component3.onClick = (System.Action)Delegate.Combine(component3.onClick, (System.Action)delegate
		{
			researchScreen.ZoomToTech(techID);
		});
		return gameObject;
	}

	private void ChangeGameObjectActive(GameObject target, bool targetActiveState)
	{
		if (target.activeSelf == targetActiveState)
		{
			return;
		}
		if (targetActiveState)
		{
			QueuedActivations.Add(target);
			if (QueuedDeactivations.Contains(target))
			{
				QueuedDeactivations.Remove(target);
			}
		}
		else
		{
			QueuedDeactivations.Add(target);
			if (QueuedActivations.Contains(target))
			{
				QueuedActivations.Remove(target);
			}
		}
	}

	private bool CheckTechItemPassesFilters(string techItemID)
	{
		TechItem techItem = Db.Get().TechItems.Get(techItemID);
		bool flag = true;
		switch (completionFilter)
		{
		case CompletionState.Available:
			flag = flag && !techItem.IsComplete() && techItem.ParentTech.ArePrerequisitesComplete();
			break;
		case CompletionState.Completed:
			flag = flag && techItem.IsComplete();
			break;
		}
		if (!flag)
		{
			return flag;
		}
		flag = flag && ResearchScreen.TechItemPassesSearchFilter(techItemID, currentSearchString);
		foreach (KeyValuePair<string, bool> filterState in filterStates)
		{
		}
		return flag;
	}

	private bool CheckTechPassesFilters(string techID)
	{
		Tech tech = Db.Get().Techs.Get(techID);
		bool flag = true;
		switch (completionFilter)
		{
		case CompletionState.Available:
			flag = flag && !tech.IsComplete() && tech.ArePrerequisitesComplete();
			break;
		case CompletionState.Completed:
			flag = flag && tech.IsComplete();
			break;
		}
		if (!flag)
		{
			return flag;
		}
		flag = flag && ResearchScreen.TechPassesSearchFilter(techID, currentSearchString);
		foreach (KeyValuePair<string, bool> filterState in filterStates)
		{
		}
		return flag;
	}
}
