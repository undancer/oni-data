using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class WorldSelector : KScreen, ISim4000ms
{
	public static WorldSelector Instance;

	public Dictionary<int, MultiToggle> worldRows;

	public TextStyleSetting titleTextSetting;

	public TextStyleSetting bodyTextSetting;

	public GameObject worldRowPrefab;

	public GameObject worldRowContainer;

	private Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion> previousWorldDiagnosticStatus = new Dictionary<int, ColonyDiagnostic.DiagnosticResult.Opinion>();

	private Dictionary<int, List<GameObject>> worldStatusIcons = new Dictionary<int, List<GameObject>>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			Deactivate();
			return;
		}
		base.OnSpawn();
		worldRows = new Dictionary<int, MultiToggle>();
		SpawnToggles();
		RefreshToggles();
		Game.Instance.Subscribe(1983128072, delegate
		{
			RefreshToggles();
		});
		Game.Instance.Subscribe(-521212405, delegate
		{
			RefreshToggles();
		});
		Game.Instance.Subscribe(880851192, delegate
		{
			SortRows();
		});
		ClusterManager.Instance.Subscribe(-1280433810, delegate(object data)
		{
			AddWorld(data);
		});
		ClusterManager.Instance.Subscribe(-1078710002, delegate(object data)
		{
			RemoveWorld(data);
		});
	}

	private void SpawnToggles()
	{
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			Util.KDestroyGameObject(worldRow.Value);
		}
		worldRows.Clear();
		foreach (int item in ClusterManager.Instance.GetWorldIDsSorted())
		{
			MultiToggle component = Util.KInstantiateUI(worldRowPrefab, worldRowContainer).GetComponent<MultiToggle>();
			worldRows.Add(item, component);
			previousWorldDiagnosticStatus.Add(item, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
			int id = item;
			component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
			{
				OnWorldRowClicked(id);
			});
			component.GetComponentInChildren<AlertVignette>().worldID = item;
		}
	}

	private void AddWorld(object data)
	{
		int num = (int)data;
		MultiToggle component = Util.KInstantiateUI(worldRowPrefab, worldRowContainer).GetComponent<MultiToggle>();
		worldRows.Add(num, component);
		previousWorldDiagnosticStatus.Add(num, ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
		int id = num;
		component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
		{
			OnWorldRowClicked(id);
		});
		component.GetComponentInChildren<AlertVignette>().worldID = num;
		RefreshToggles();
	}

	private void RemoveWorld(object data)
	{
		int key = (int)data;
		if (worldRows.TryGetValue(key, out var value))
		{
			value.DeleteObject();
		}
		worldRows.Remove(key);
		previousWorldDiagnosticStatus.Remove(key);
		RefreshToggles();
	}

	public void OnWorldRowClicked(int id)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(id);
		if (world != null && world.IsDiscovered)
		{
			CameraController.Instance.ActiveWorldStarWipe(id);
		}
	}

	private void RefreshToggles()
	{
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Key);
			ClusterGridEntity component = world.GetComponent<ClusterGridEntity>();
			HierarchyReferences component2 = worldRow.Value.GetComponent<HierarchyReferences>();
			if (world != null)
			{
				component2.GetReference<Image>("Icon").sprite = component.GetUISprite();
				component2.GetReference<LocText>("Label").SetText(world.GetComponent<ClusterGridEntity>().Name);
			}
			else
			{
				component2.GetReference<Image>("Icon").sprite = Assets.GetSprite("unknown_far");
			}
			if (worldRow.Key == CameraController.Instance.cameraActiveCluster)
			{
				worldRow.Value.ChangeState(1);
				worldRow.Value.gameObject.SetActive(value: true);
			}
			else if (world != null && world.IsDiscovered)
			{
				worldRow.Value.ChangeState(0);
				worldRow.Value.gameObject.SetActive(value: true);
			}
			else
			{
				worldRow.Value.ChangeState(0);
				worldRow.Value.gameObject.SetActive(value: false);
			}
			RefreshToggleTooltips();
			worldRow.Value.GetComponentInChildren<AlertVignette>().worldID = worldRow.Key;
		}
		RefreshWorldStatus();
		SortRows();
	}

	private void RefreshWorldStatus()
	{
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			if (!worldStatusIcons.ContainsKey(worldRow.Key))
			{
				worldStatusIcons.Add(worldRow.Key, new List<GameObject>());
			}
			foreach (GameObject item in worldStatusIcons[worldRow.Key])
			{
				Util.KDestroyGameObject(item);
			}
			LocText reference = worldRow.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("StatusLabel");
			reference.SetText(ClusterManager.Instance.GetWorld(worldRow.Key).GetStatus());
			reference.color = ColonyDiagnosticScreen.GetDiagnosticIndicationColor(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(worldRow.Key));
		}
	}

	private void RefreshToggleTooltips()
	{
		int num = 0;
		List<int> discoveredAsteroidIDsSorted = ClusterManager.Instance.GetDiscoveredAsteroidIDsSorted();
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			ClusterGridEntity component = ClusterManager.Instance.GetWorld(worldRow.Key).GetComponent<ClusterGridEntity>();
			ToolTip component2 = worldRow.Value.GetComponent<ToolTip>();
			component2.ClearMultiStringTooltip();
			WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Key);
			if (world != null)
			{
				component2.AddMultiStringTooltip(component.Name, titleTextSetting);
				if (!world.IsModuleInterior)
				{
					int num2 = discoveredAsteroidIDsSorted.IndexOf(world.id);
					if (num2 != -1 && num2 <= 9)
					{
						component2.AddMultiStringTooltip(" ", bodyTextSetting);
						if (KInputManager.currentControllerIsGamepad)
						{
							component2.AddMultiStringTooltip(UI.FormatAsHotkey(GameUtil.GetActionString(IdxToHotkeyAction(num2))), bodyTextSetting);
						}
						else
						{
							component2.AddMultiStringTooltip(UI.FormatAsHotkey("[" + GameUtil.GetActionString(IdxToHotkeyAction(num2)) + "]"), bodyTextSetting);
						}
					}
				}
			}
			else
			{
				component2.AddMultiStringTooltip(UI.CLUSTERMAP.UNKNOWN_DESTINATION, titleTextSetting);
			}
			if (ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(world.id) < ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
			{
				component2.AddMultiStringTooltip(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultTooltip(world.id), bodyTextSetting);
			}
			num++;
		}
	}

	private void SortRows()
	{
		List<KeyValuePair<int, MultiToggle>> list = worldRows.ToList();
		list.Sort(delegate(KeyValuePair<int, MultiToggle> x, KeyValuePair<int, MultiToggle> y)
		{
			float num2 = (ClusterManager.Instance.GetWorld(x.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(x.Key).DiscoveryTimestamp);
			float value = (ClusterManager.Instance.GetWorld(y.Key).IsModuleInterior ? float.PositiveInfinity : ClusterManager.Instance.GetWorld(y.Key).DiscoveryTimestamp);
			return num2.CompareTo(value);
		});
		for (int i = 0; i < list.Count; i++)
		{
			list[i].Value.transform.SetSiblingIndex(i);
		}
		foreach (KeyValuePair<int, MultiToggle> item in list)
		{
			item.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.zero;
			item.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.right * 24f;
			WorldContainer world = ClusterManager.Instance.GetWorld(item.Key);
			if (world.ParentWorldId == world.id || world.ParentWorldId == ClusterManager.INVALID_WORLD_IDX)
			{
				continue;
			}
			int num = -1;
			foreach (KeyValuePair<int, MultiToggle> item2 in list)
			{
				if (item2.Key == world.ParentWorldId)
				{
					num = item2.Value.gameObject.transform.GetSiblingIndex();
					item.Value.gameObject.transform.SetSiblingIndex(num + 1);
					item.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.right * 32f;
					item.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Status").anchoredPosition = Vector2.right * -8f;
					break;
				}
			}
		}
	}

	private Action IdxToHotkeyAction(int idx)
	{
		switch (idx)
		{
		case 0:
			return Action.SwitchActiveWorld1;
		case 1:
			return Action.SwitchActiveWorld2;
		case 2:
			return Action.SwitchActiveWorld3;
		case 3:
			return Action.SwitchActiveWorld4;
		case 4:
			return Action.SwitchActiveWorld5;
		case 5:
			return Action.SwitchActiveWorld6;
		case 6:
			return Action.SwitchActiveWorld7;
		case 7:
			return Action.SwitchActiveWorld8;
		case 8:
			return Action.SwitchActiveWorld9;
		case 9:
			return Action.SwitchActiveWorld10;
		default:
			Debug.LogError("Action must be a SwitchActiveWorld Action");
			return Action.SwitchActiveWorld1;
		}
	}

	public void Sim4000ms(float dt)
	{
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			ColonyDiagnostic.DiagnosticResult.Opinion worldDiagnosticResult = ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(worldRow.Key);
			ColonyDiagnosticScreen.SetIndication(worldDiagnosticResult, worldRow.Value.GetComponent<HierarchyReferences>().GetReference("Indicator").gameObject);
			if (previousWorldDiagnosticStatus[worldRow.Key] > worldDiagnosticResult && ClusterManager.Instance.activeWorldId != worldRow.Key)
			{
				TriggerVisualNotification(worldRow.Key, worldDiagnosticResult);
			}
			previousWorldDiagnosticStatus[worldRow.Key] = worldDiagnosticResult;
		}
		RefreshWorldStatus();
		RefreshToggleTooltips();
	}

	public void TriggerVisualNotification(int worldID, ColonyDiagnostic.DiagnosticResult.Opinion result)
	{
		foreach (KeyValuePair<int, MultiToggle> worldRow in worldRows)
		{
			if (worldRow.Key == worldID)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsInactive[result]));
				if (worldRow.Value.gameObject.activeInHierarchy)
				{
					worldRow.Value.StartCoroutine(VisualNotificationRoutine(worldRow.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject, worldRow.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indicator"), worldRow.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Spacer").gameObject));
				}
			}
		}
	}

	private IEnumerator VisualNotificationRoutine(GameObject contentGameObject, RectTransform indicator, GameObject spacer)
	{
		spacer.GetComponent<NotificationAnimator>().Begin(startOffset: false);
		Vector2 defaultIndicatorSize2 = new Vector2(8f, 8f);
		float bounceDuration = 1.5f;
		for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize2 + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
			yield return 0;
		}
		for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize2 + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
			yield return 0;
		}
		for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
		{
			indicator.sizeDelta = defaultIndicatorSize2 + Vector2.one * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
			yield return 0;
		}
		defaultIndicatorSize2 = (indicator.sizeDelta = new Vector2(8f, 8f));
		contentGameObject.rectTransform().localPosition = Vector2.zero;
	}
}
