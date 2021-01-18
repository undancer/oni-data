using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ColonyDiagnosticScreen : KScreen, ISim4000ms
{
	private class DiagnosticRow
	{
		private const float displayHistoryPeriod = 600f;

		public ColonyDiagnosticUtility.ColonyDiagnostic diagnostic;

		public SparkLayer sparkLayer;

		public int worldID;

		private LocText titleLabel;

		private LocText valueLabel;

		private Image indicator;

		private ToolTip tooltip;

		private MultiToggle button;

		private Image image;

		public ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult;

		private Vector2 defaultIndicatorSizeDelta;

		private float timeOfLastNotification = 0f;

		private const float MIN_TIME_BETWEEN_NOTIFICATIONS = 300f;

		private Coroutine activeRoutine = null;

		public GameObject gameObject
		{
			get;
			private set;
		}

		public DiagnosticRow(int worldID, GameObject gameObject, ColonyDiagnosticUtility.ColonyDiagnostic diagnostic)
		{
			Debug.Assert(diagnostic != null);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			this.worldID = worldID;
			sparkLayer = component.GetReference<SparkLayer>("SparkLayer");
			this.diagnostic = diagnostic;
			titleLabel = component.GetReference<LocText>("TitleLabel");
			valueLabel = component.GetReference<LocText>("ValueLabel");
			indicator = component.GetReference<Image>("Indicator");
			image = component.GetReference<Image>("Image");
			tooltip = gameObject.GetComponent<ToolTip>();
			this.gameObject = gameObject;
			titleLabel.SetText(diagnostic.name);
			sparkLayer.colorRules.setOwnColor = false;
			if (diagnostic.tracker == null)
			{
				sparkLayer.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				sparkLayer.ClearLines();
				Tuple<float, float>[] points = diagnostic.tracker.ChartableData(600f);
				sparkLayer.NewLine(points, diagnostic.name);
			}
			button = gameObject.GetComponent<MultiToggle>();
			MultiToggle multiToggle = button;
			multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
			{
				if (diagnostic.LatestResult.clickThroughTarget == null)
				{
					CameraController.Instance.ActiveWorldStarWipe(diagnostic.worldID);
				}
				else
				{
					SelectTool.Instance.SelectAndFocus(diagnostic.LatestResult.clickThroughTarget.first, (diagnostic.LatestResult.clickThroughTarget.second == null) ? null : diagnostic.LatestResult.clickThroughTarget.second.GetComponent<KSelectable>());
				}
			});
			defaultIndicatorSizeDelta = indicator.rectTransform().sizeDelta;
		}

		public void Update()
		{
			currentDisplayedResult = diagnostic.LatestResult.opinion;
			Color color = diagnostic.colors[diagnostic.LatestResult.opinion];
			if (diagnostic.tracker != null)
			{
				Tuple<float, float>[] data = diagnostic.tracker.ChartableData(600f);
				sparkLayer.RefreshLine(data, diagnostic.name);
				sparkLayer.SetColor(color);
			}
			indicator.color = diagnostic.colors[diagnostic.LatestResult.opinion];
			tooltip.SetSimpleTooltip(diagnostic.LatestResult.Message + "\n\n" + UI.COLONY_DIAGNOSTICS.MUTE_TUTORIAL.text);
			ColonyDiagnosticUtility.ColonyDiagnostic.PresentationSetting presentationSetting = diagnostic.presentationSetting;
			if (presentationSetting == ColonyDiagnosticUtility.ColonyDiagnostic.PresentationSetting.AverageValue || presentationSetting != ColonyDiagnosticUtility.ColonyDiagnostic.PresentationSetting.CurrentValue)
			{
				valueLabel.SetText(diagnostic.GetAverageValueString());
			}
			else
			{
				valueLabel.SetText(diagnostic.GetCurrentValueString());
			}
			if (!string.IsNullOrEmpty(diagnostic.icon))
			{
				image.sprite = Assets.GetSprite(diagnostic.icon);
			}
			if (color == Constants.NEUTRAL_COLOR)
			{
				color = Color.white;
			}
			titleLabel.color = color;
		}

		public bool CheckAllowVisualNotification()
		{
			if (timeOfLastNotification == 0f)
			{
				return true;
			}
			if (GameClock.Instance.GetTime() >= timeOfLastNotification + 300f)
			{
				return true;
			}
			return false;
		}

		public void TriggerVisualNotification()
		{
			if (activeRoutine == null)
			{
				timeOfLastNotification = GameClock.Instance.GetTime();
				KFMOD.PlayUISound(GlobalAssets.GetSound(notificationSoundsActive[currentDisplayedResult]));
				activeRoutine = gameObject.GetComponent<KMonoBehaviour>().StartCoroutine(VisualNotificationRoutine());
			}
		}

		private IEnumerator VisualNotificationRoutine()
		{
			RectTransform indicator = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform;
			defaultIndicatorSizeDelta = new Vector2(0f, 0f);
			float bounceDuration = 1.5f;
			for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
			{
				gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (k / bounceDuration)) * 108f);
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
				yield return 0;
			}
			for (float j = 0f; j < bounceDuration; j += Time.unscaledDeltaTime)
			{
				gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (j / bounceDuration)) * 80f);
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (j / bounceDuration))));
				yield return 0;
			}
			for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
			{
				gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (i / bounceDuration)) * 60f);
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (i / bounceDuration))));
				yield return 0;
			}
			ResolveNotificationRoutine();
		}

		public void ResolveNotificationRoutine()
		{
			RectTransform rectTransform = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform;
			rectTransform.sizeDelta = defaultIndicatorSizeDelta;
			gameObject.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").localPosition = Vector2.zero;
			activeRoutine = null;
		}
	}

	public GameObject linePrefab;

	public GameObject categoryPrefab;

	public static ColonyDiagnosticScreen Instance;

	private List<DiagnosticRow> diagnosticRows = new List<DiagnosticRow>();

	private Dictionary<DiagnosticRow, List<DiagnosticRow>> categoryRows = new Dictionary<DiagnosticRow, List<DiagnosticRow>>();

	public GameObject header;

	public GameObject contentContainer;

	public GameObject rootIndicator;

	public MultiToggle seeAllButton;

	public static Dictionary<ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsActive = new Dictionary<ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Active_Bad"
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Active_Warning"
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Good,
			""
		}
	};

	public static Dictionary<ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsInactive = new Dictionary<ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Inactive_Bad"
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Inactive_Warning"
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Good,
			""
		}
	};

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
		RefreshSingleWorld();
		Game.Instance.Subscribe(1983128072, RefreshSingleWorld);
		MultiToggle multiToggle = seeAllButton;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			AllDiagnosticsScreen.Instance.Show(!AllDiagnosticsScreen.Instance.gameObject.activeSelf);
		});
	}

	private void RefreshSingleWorld(object data = null)
	{
		foreach (DiagnosticRow diagnosticRow in diagnosticRows)
		{
			Util.KDestroyGameObject(diagnosticRow.gameObject);
		}
		diagnosticRows.Clear();
		foreach (KeyValuePair<DiagnosticRow, List<DiagnosticRow>> categoryRow in categoryRows)
		{
			Util.KDestroyGameObject(categoryRow.Key.gameObject.transform.parent.gameObject);
		}
		categoryRows.Clear();
		SpawnTrackerLines(ClusterManager.Instance.activeWorldId);
	}

	private void ToggleCategoryOpen(GameObject category)
	{
		Transform transform = category.transform.Find("Content");
		transform.gameObject.SetActive(!transform.gameObject.activeSelf);
		category.GetComponentInChildren<MultiToggle>().ChangeState(transform.gameObject.activeSelf ? 1 : 0);
	}

	private void SpawnTrackerLines(int world)
	{
		AddDiagnostic<ColonyDiagnosticUtility.BreathabilityDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.FoodDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.StressDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.FloatingRocketDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.RocketFuelDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.RocketOxidizerDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.FarmDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.ToiletDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.BedDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.IdleDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.TrappedDuplicantDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.EntombedDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.PowerUseDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ColonyDiagnosticUtility.BatteryDiagnostic>(world, contentContainer, diagnosticRows);
		List<DiagnosticRow> list = new List<DiagnosticRow>();
		foreach (DiagnosticRow diagnosticRow in diagnosticRows)
		{
			list.Add(diagnosticRow);
		}
		list.Sort((DiagnosticRow a, DiagnosticRow b) => a.diagnostic.name.CompareTo(b.diagnostic.name));
		foreach (DiagnosticRow item in list)
		{
			item.gameObject.transform.SetAsLastSibling();
		}
		list.Clear();
		seeAllButton.transform.SetAsLastSibling();
		RefreshAll();
	}

	private void CreateChoreCategories(GameObject worldRootParent, int worldID)
	{
		GameObject choreCategory = Util.KInstantiateUI(categoryPrefab, worldRootParent, force_active: true);
		GameObject allChores = AddDiagnostic<ColonyDiagnosticUtility.AllChoresDiagnostic>(worldID, choreCategory, diagnosticRows);
		List<DiagnosticRow> list = new List<DiagnosticRow>();
		for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
		{
			AddChoreGroupDiagnostic(worldID, Db.Get().ChoreGroups[i], choreCategory.transform.Find("Content").gameObject, list);
		}
		categoryRows.Add(diagnosticRows.Find((DiagnosticRow match) => match.gameObject == allChores), list);
		allChores.transform.SetSiblingIndex(allChores.transform.parent.childCount - 2);
		allChores.GetComponentInChildren<MultiToggle>().onClick = delegate
		{
			ToggleCategoryOpen(choreCategory);
		};
		ToggleCategoryOpen(choreCategory);
		GameObject choreCategory2 = Util.KInstantiateUI(categoryPrefab, worldRootParent, force_active: true);
		GameObject allChores2 = AddDiagnostic<ColonyDiagnosticUtility.AllWorkTimeDiagnostic>(worldID, choreCategory2, diagnosticRows);
		List<DiagnosticRow> list2 = new List<DiagnosticRow>();
		for (int j = 0; j < Db.Get().ChoreGroups.Count; j++)
		{
			AddWorkTimeDiagnostic(worldID, Db.Get().ChoreGroups[j], choreCategory2.transform.Find("Content").gameObject, list2);
		}
		categoryRows.Add(diagnosticRows.Find((DiagnosticRow match) => match.gameObject == allChores2), list2);
		allChores2.transform.SetSiblingIndex(allChores2.transform.parent.childCount - 2);
		allChores2.GetComponentInChildren<MultiToggle>().onClick = delegate
		{
			ToggleCategoryOpen(choreCategory2);
		};
		ToggleCategoryOpen(choreCategory2);
	}

	private void AddChoreGroupDiagnostic(int worldID, ChoreGroup choreGroup, GameObject parent, List<DiagnosticRow> parentCollection)
	{
		ColonyDiagnosticUtility.ChoreGroupDiagnostic choreGroupDiagnostic = ColonyDiagnosticUtility.Instance.GetChoreGroupDiagnostic(worldID, choreGroup);
		Debug.Assert(choreGroupDiagnostic != null, "Diagnostic of Type " + typeof(ColonyDiagnosticUtility.ChoreGroupDiagnostic).ToString() + " is null - remember to add it to the AddWorld function in ColonyDiagnosticUtility");
		parentCollection.Add(new DiagnosticRow(worldID, Util.KInstantiateUI(linePrefab, parent, force_active: true), choreGroupDiagnostic));
	}

	private void AddWorkTimeDiagnostic(int worldID, ChoreGroup choreGroup, GameObject parent, List<DiagnosticRow> parentCollection)
	{
		ColonyDiagnosticUtility.WorkTimeDiagnostic workTimeDiagnostic = ColonyDiagnosticUtility.Instance.GetWorkTimeDiagnostic(worldID, choreGroup);
		Debug.Assert(workTimeDiagnostic != null, "Diagnostic of Type " + typeof(ColonyDiagnosticUtility.WorkTimeDiagnostic).ToString() + " is null - remember to add it to the AddWorld function in ColonyDiagnosticUtility");
		parentCollection.Add(new DiagnosticRow(worldID, Util.KInstantiateUI(linePrefab, parent, force_active: true), workTimeDiagnostic));
	}

	private GameObject AddDiagnostic<T>(int worldID, GameObject parent, List<DiagnosticRow> parentCollection) where T : ColonyDiagnosticUtility.ColonyDiagnostic
	{
		T diagnostic = ColonyDiagnosticUtility.Instance.GetDiagnostic<T>(worldID);
		if (diagnostic == null)
		{
			return null;
		}
		GameObject gameObject = Util.KInstantiateUI(linePrefab, parent, force_active: true);
		parentCollection.Add(new DiagnosticRow(worldID, gameObject, diagnostic));
		return gameObject;
	}

	public static void SetIndication(ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion pinion, GameObject indicatorGameObject)
	{
		Image componentInChildren = indicatorGameObject.GetComponentInChildren<Image>();
		switch (pinion)
		{
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
			componentInChildren.color = Constants.NEGATIVE_COLOR;
			break;
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
			componentInChildren.color = Constants.WARNING_COLOR;
			break;
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal:
		case ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Good:
			componentInChildren.color = Constants.NEUTRAL_COLOR;
			break;
		}
	}

	public void Sim4000ms(float dt)
	{
		RefreshAll();
	}

	public void RefreshAll()
	{
		string text = "";
		foreach (DiagnosticRow diagnosticRow in diagnosticRows)
		{
			if (diagnosticRow.worldID == ClusterManager.Instance.activeWorldId)
			{
				UpdateDiagnosticRow(diagnosticRow, text);
			}
		}
		SetIndication(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(ClusterManager.Instance.activeWorldId), rootIndicator);
		header.GetComponent<ToolTip>().enabled = ((!string.IsNullOrEmpty(text)) ? true : false);
		header.GetComponent<ToolTip>().SetSimpleTooltip(text);
		foreach (KeyValuePair<DiagnosticRow, List<DiagnosticRow>> categoryRow in categoryRows)
		{
			if (categoryRow.Key.worldID != ClusterManager.Instance.activeWorldId)
			{
				continue;
			}
			text = "";
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion opinion = ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Good;
			foreach (DiagnosticRow item in categoryRow.Value)
			{
				opinion = (ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion)Math.Min((int)opinion, (int)UpdateDiagnosticRow(item, text));
			}
			SetIndication(opinion, categoryRow.Key.gameObject.transform.Find("Indicator").gameObject);
			categoryRow.Key.gameObject.GetComponent<ToolTip>().enabled = ((!string.IsNullOrEmpty(text)) ? true : false);
			categoryRow.Key.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(text);
		}
		seeAllButton.GetComponentInChildren<LocText>().SetText(string.Format(UI.DIAGNOSTICS_SCREEN.SEE_ALL, AllDiagnosticsScreen.Instance.GetRowCount()));
	}

	private ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion UpdateDiagnosticRow(DiagnosticRow row, string tooltipString)
	{
		ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult = row.currentDisplayedResult;
		bool activeInHierarchy = row.gameObject.activeInHierarchy;
		row.Update();
		if (row.diagnostic.LatestResult.opinion < ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
		{
			if (!string.IsNullOrEmpty(tooltipString))
			{
				tooltipString += "\n";
			}
			tooltipString += row.diagnostic.LatestResult.Message;
		}
		if (ColonyDiagnosticUtility.Instance.IsDiagnosticTutorialDisabled(row.diagnostic.id))
		{
			SetRowActive(row, active: false);
		}
		else if (!categoryRows.ContainsKey(row))
		{
			switch (ColonyDiagnosticUtility.Instance.diagnosticDisplaySettings[row.worldID][row.diagnostic.id])
			{
			case ColonyDiagnosticUtility.DisplaySetting.Always:
				SetRowActive(row, active: true);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.AlertOnly:
				SetRowActive(row, row.diagnostic.LatestResult.opinion < ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.Never:
				SetRowActive(row, active: false);
				break;
			}
			if (row.gameObject.activeInHierarchy && (row.currentDisplayedResult < currentDisplayedResult || (row.currentDisplayedResult < ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal && !activeInHierarchy)) && row.CheckAllowVisualNotification())
			{
				row.TriggerVisualNotification();
			}
		}
		return row.diagnostic.LatestResult.opinion;
	}

	private void SetRowActive(DiagnosticRow row, bool active)
	{
		if (row.gameObject.activeSelf != active)
		{
			row.gameObject.SetActive(active);
			row.ResolveNotificationRoutine();
		}
	}
}
