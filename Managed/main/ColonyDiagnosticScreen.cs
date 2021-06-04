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

		public ColonyDiagnostic diagnostic;

		public SparkLayer sparkLayer;

		public int worldID;

		private LocText titleLabel;

		private LocText valueLabel;

		private Image indicator;

		private ToolTip tooltip;

		private MultiToggle button;

		private Image image;

		public ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult;

		private Vector2 defaultIndicatorSizeDelta;

		private float timeOfLastNotification = 0f;

		private const float MIN_TIME_BETWEEN_NOTIFICATIONS = 300f;

		private Coroutine activeRoutine = null;

		public GameObject gameObject
		{
			get;
			private set;
		}

		public DiagnosticRow(int worldID, GameObject gameObject, ColonyDiagnostic diagnostic)
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
			defaultIndicatorSizeDelta = Vector2.zero;
		}

		public void Update()
		{
			Color white = Color.white;
			Debug.Assert(diagnostic.LatestResult.opinion != ColonyDiagnostic.DiagnosticResult.Opinion.Unset, $"{diagnostic} criteria returned no opinion. Make sure the DiagnosticResult parameters are used or an opinion result is otherwise set in all of its criteria");
			currentDisplayedResult = diagnostic.LatestResult.opinion;
			white = diagnostic.colors[diagnostic.LatestResult.opinion];
			if (diagnostic.tracker != null)
			{
				Tuple<float, float>[] data = diagnostic.tracker.ChartableData(600f);
				sparkLayer.RefreshLine(data, diagnostic.name);
				sparkLayer.SetColor(white);
			}
			indicator.color = diagnostic.colors[diagnostic.LatestResult.opinion];
			tooltip.SetSimpleTooltip((diagnostic.LatestResult.Message.IsNullOrWhiteSpace() ? UI.COLONY_DIAGNOSTICS.GENERIC_STATUS_NORMAL.text : diagnostic.LatestResult.Message) + "\n\n" + UI.COLONY_DIAGNOSTICS.MUTE_TUTORIAL.text);
			ColonyDiagnostic.PresentationSetting presentationSetting = diagnostic.presentationSetting;
			if (presentationSetting == ColonyDiagnostic.PresentationSetting.AverageValue || presentationSetting != ColonyDiagnostic.PresentationSetting.CurrentValue)
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
			if (white == Constants.NEUTRAL_COLOR)
			{
				white = Color.white;
			}
			titleLabel.color = white;
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
			gameObject.GetComponentInChildren<NotificationAnimator>().Begin(startOffset: false);
			RectTransform indicator = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform;
			defaultIndicatorSizeDelta = Vector2.zero;
			indicator.sizeDelta = defaultIndicatorSizeDelta;
			float bounceDuration = 3f;
			for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
				yield return 0;
			}
			for (float j = 0f; j < bounceDuration; j += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (j / bounceDuration))));
				yield return 0;
			}
			for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
			{
				indicator.sizeDelta = defaultIndicatorSizeDelta + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (i / bounceDuration))));
				yield return 0;
			}
			ResolveNotificationRoutine();
		}

		public void ResolveNotificationRoutine()
		{
			RectTransform rectTransform = gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Indicator").rectTransform;
			rectTransform.sizeDelta = Vector2.zero;
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

	public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsActive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
			"Diagnostic_Active_DuplicantThreatening"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Active_Bad"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Active_Warning"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
			"Diagnostic_Active_Concern"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
			"Diagnostic_Active_Suggestion"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
			"Diagnostic_Active_Tutorial"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Good,
			""
		}
	};

	public static Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string> notificationSoundsInactive = new Dictionary<ColonyDiagnostic.DiagnosticResult.Opinion, string>
	{
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening,
			"Diagnostic_Inactive_DuplicantThreatening"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Bad,
			"Diagnostic_Inactive_Bad"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Warning,
			"Diagnostic_Inactive_Warning"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Concern,
			"Diagnostic_Inactive_Concern"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion,
			"Diagnostic_Inactive_Suggestion"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Tutorial,
			"Diagnostic_Inactive_Tutorial"
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Normal,
			""
		},
		{
			ColonyDiagnostic.DiagnosticResult.Opinion.Good,
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
		AddDiagnostic<BreathabilityDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<FoodDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<StressDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<RadiationDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ReactorDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<FloatingRocketDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<RocketFuelDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<RocketOxidizerDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<FarmDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<ToiletDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<BedDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<IdleDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<TrappedDuplicantDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<EntombedDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<PowerUseDiagnostic>(world, contentContainer, diagnosticRows);
		AddDiagnostic<BatteryDiagnostic>(world, contentContainer, diagnosticRows);
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
		GameObject allChores = AddDiagnostic<AllChoresDiagnostic>(worldID, choreCategory, diagnosticRows);
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
		GameObject allChores2 = AddDiagnostic<AllWorkTimeDiagnostic>(worldID, choreCategory2, diagnosticRows);
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
		ChoreGroupDiagnostic choreGroupDiagnostic = ColonyDiagnosticUtility.Instance.GetChoreGroupDiagnostic(worldID, choreGroup);
		Debug.Assert(choreGroupDiagnostic != null, "Diagnostic of Type " + typeof(ChoreGroupDiagnostic).ToString() + " is null - remember to add it to the AddWorld function in ColonyDiagnosticUtility");
		parentCollection.Add(new DiagnosticRow(worldID, Util.KInstantiateUI(linePrefab, parent, force_active: true), choreGroupDiagnostic));
	}

	private void AddWorkTimeDiagnostic(int worldID, ChoreGroup choreGroup, GameObject parent, List<DiagnosticRow> parentCollection)
	{
		WorkTimeDiagnostic workTimeDiagnostic = ColonyDiagnosticUtility.Instance.GetWorkTimeDiagnostic(worldID, choreGroup);
		Debug.Assert(workTimeDiagnostic != null, "Diagnostic of Type " + typeof(WorkTimeDiagnostic).ToString() + " is null - remember to add it to the AddWorld function in ColonyDiagnosticUtility");
		parentCollection.Add(new DiagnosticRow(worldID, Util.KInstantiateUI(linePrefab, parent, force_active: true), workTimeDiagnostic));
	}

	private GameObject AddDiagnostic<T>(int worldID, GameObject parent, List<DiagnosticRow> parentCollection) where T : ColonyDiagnostic
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

	public static void SetIndication(ColonyDiagnostic.DiagnosticResult.Opinion opinion, GameObject indicatorGameObject)
	{
		Image componentInChildren = indicatorGameObject.GetComponentInChildren<Image>();
		componentInChildren.color = GetDiagnosticIndicationColor(opinion);
	}

	public static Color GetDiagnosticIndicationColor(ColonyDiagnostic.DiagnosticResult.Opinion opinion)
	{
		switch (opinion)
		{
		case ColonyDiagnostic.DiagnosticResult.Opinion.DuplicantThreatening:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Bad:
		case ColonyDiagnostic.DiagnosticResult.Opinion.Warning:
			return Constants.NEGATIVE_COLOR;
		case ColonyDiagnostic.DiagnosticResult.Opinion.Concern:
			return Constants.WARNING_COLOR;
		default:
			return Color.white;
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
			ColonyDiagnostic.DiagnosticResult.Opinion opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Good;
			foreach (DiagnosticRow item in categoryRow.Value)
			{
				opinion = (ColonyDiagnostic.DiagnosticResult.Opinion)Math.Min((int)opinion, (int)UpdateDiagnosticRow(item, text));
			}
			SetIndication(opinion, categoryRow.Key.gameObject.transform.Find("Indicator").gameObject);
			categoryRow.Key.gameObject.GetComponent<ToolTip>().enabled = ((!string.IsNullOrEmpty(text)) ? true : false);
			categoryRow.Key.gameObject.GetComponent<ToolTip>().SetSimpleTooltip(text);
		}
		seeAllButton.GetComponentInChildren<LocText>().SetText(string.Format(UI.DIAGNOSTICS_SCREEN.SEE_ALL, AllDiagnosticsScreen.Instance.GetRowCount()));
	}

	private ColonyDiagnostic.DiagnosticResult.Opinion UpdateDiagnosticRow(DiagnosticRow row, string tooltipString)
	{
		ColonyDiagnostic.DiagnosticResult.Opinion currentDisplayedResult = row.currentDisplayedResult;
		bool activeInHierarchy = row.gameObject.activeInHierarchy;
		row.Update();
		if (row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
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
				SetRowActive(row, row.diagnostic.LatestResult.opinion < ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
				break;
			case ColonyDiagnosticUtility.DisplaySetting.Never:
				SetRowActive(row, active: false);
				break;
			}
			if (row.gameObject.activeInHierarchy && (row.currentDisplayedResult < currentDisplayedResult || (row.currentDisplayedResult < ColonyDiagnostic.DiagnosticResult.Opinion.Normal && !activeInHierarchy)) && row.CheckAllowVisualNotification())
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
