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

	public Dictionary<MultiToggle, int> worldRows;

	public TextStyleSetting titleTextSetting;

	public TextStyleSetting bodyTextSetting;

	public GameObject worldRowPrefab;

	public GameObject worldRowContainer;

	private Dictionary<int, ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion> previousWorldDiagnosticStatus = new Dictionary<int, ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		if (!DlcManager.IsExpansion1Active())
		{
			Deactivate();
			return;
		}
		base.OnSpawn();
		worldRows = new Dictionary<MultiToggle, int>();
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
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			Util.KDestroyGameObject(worldRow.Key);
		}
		worldRows.Clear();
		foreach (int worldID in ClusterManager.Instance.GetWorldIDs())
		{
			GameObject gameObject = Util.KInstantiateUI(worldRowPrefab, worldRowContainer);
			MultiToggle component = gameObject.GetComponent<MultiToggle>();
			worldRows.Add(component, worldID);
			previousWorldDiagnosticStatus.Add(worldID, ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
			int id = worldID;
			component.onClick = (System.Action)Delegate.Combine(component.onClick, (System.Action)delegate
			{
				OnWorldRowClicked(id);
			});
			component.GetComponentInChildren<AlertVignette>().worldID = worldID;
		}
	}

	private void AddWorld(object data)
	{
		int num = (int)data;
		GameObject gameObject = Util.KInstantiateUI(worldRowPrefab, worldRowContainer);
		MultiToggle component = gameObject.GetComponent<MultiToggle>();
		worldRows.Add(component, num);
		previousWorldDiagnosticStatus.Add(num, ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal);
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
		int world_id = (int)data;
		foreach (KeyValuePair<MultiToggle, int> item in worldRows.Where((KeyValuePair<MultiToggle, int> kvp) => kvp.Value == world_id).ToList())
		{
			worldRows.Remove(item.Key);
			item.Key.DeleteObject();
		}
		previousWorldDiagnosticStatus.Remove(world_id);
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
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Value);
			ClusterGridEntity component = world.GetComponent<ClusterGridEntity>();
			HierarchyReferences component2 = worldRow.Key.GetComponent<HierarchyReferences>();
			if (world != null)
			{
				component2.GetReference<Image>("Icon").sprite = component.GetUISprite();
				component2.GetReference<LocText>("Label").SetText(world.GetComponent<ClusterGridEntity>().Name);
			}
			else
			{
				component2.GetReference<Image>("Icon").sprite = Assets.GetSprite("unknown_far");
			}
			if (worldRow.Value == CameraController.Instance.cameraActiveCluster)
			{
				worldRow.Key.ChangeState(1);
				worldRow.Key.gameObject.SetActive(value: true);
			}
			else if (world != null && world.IsDiscovered)
			{
				worldRow.Key.ChangeState(0);
				worldRow.Key.gameObject.SetActive(value: true);
			}
			else
			{
				worldRow.Key.ChangeState(0);
				worldRow.Key.gameObject.SetActive(value: false);
			}
			RefreshToggleTooltips();
			worldRow.Key.GetComponentInChildren<AlertVignette>().worldID = worldRow.Value;
		}
		SortRows();
	}

	private void RefreshToggleTooltips()
	{
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Value);
			ClusterGridEntity component = world.GetComponent<ClusterGridEntity>();
			ToolTip component2 = worldRow.Key.GetComponent<ToolTip>();
			component2.ClearMultiStringTooltip();
			WorldContainer world2 = ClusterManager.Instance.GetWorld(worldRow.Value);
			if (world2 != null)
			{
				component2.AddMultiStringTooltip(component.Name, titleTextSetting);
				if (!world2.IsModuleInterior)
				{
					component2.AddMultiStringTooltip(" ", bodyTextSetting);
					component2.AddMultiStringTooltip(UI.FormatAsHotkey("[" + GameUtil.GetActionString(IdxToHotkeyAction(num2)) + "]"), bodyTextSetting);
					num2++;
				}
			}
			else
			{
				component2.AddMultiStringTooltip(UI.CLUSTERMAP.UNKNOWN_DESTINATION, titleTextSetting);
			}
			if (ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(world2.id) < ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion.Normal)
			{
				component2.AddMultiStringTooltip(ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultTooltip(world2.id), bodyTextSetting);
			}
			num++;
		}
	}

	private void SortRows()
	{
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(worldRow.Value);
			if (world.IsModuleInterior)
			{
				worldRow.Key.transform.SetAsLastSibling();
			}
		}
		foreach (KeyValuePair<MultiToggle, int> worldRow2 in worldRows)
		{
			worldRow2.Key.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.zero;
			WorldContainer world2 = ClusterManager.Instance.GetWorld(worldRow2.Value);
			if (world2.ParentWorldId == world2.id || world2.ParentWorldId == ClusterManager.INVALID_WORLD_IDX)
			{
				continue;
			}
			int num = -1;
			foreach (KeyValuePair<MultiToggle, int> worldRow3 in worldRows)
			{
				if (worldRow3.Value == world2.ParentWorldId)
				{
					num = worldRow3.Key.gameObject.transform.GetSiblingIndex();
					worldRow2.Key.gameObject.transform.SetSiblingIndex(num + 1);
					worldRow2.Key.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indent").anchoredPosition = Vector2.right * 32f;
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
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion worldDiagnosticResult = ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResult(worldRow.Value);
			ColonyDiagnosticScreen.SetIndication(worldDiagnosticResult, worldRow.Key.GetComponent<HierarchyReferences>().GetReference("Indicator").gameObject);
			if (previousWorldDiagnosticStatus[worldRow.Value] > worldDiagnosticResult && ClusterManager.Instance.activeWorldId != worldRow.Value)
			{
				TriggerVisualNotification(worldRow.Value, worldDiagnosticResult);
			}
			previousWorldDiagnosticStatus[worldRow.Value] = worldDiagnosticResult;
		}
		RefreshToggleTooltips();
	}

	public void TriggerVisualNotification(int worldID, ColonyDiagnosticUtility.ColonyDiagnostic.DiagnosticResult.Opinion result)
	{
		foreach (KeyValuePair<MultiToggle, int> worldRow in worldRows)
		{
			if (worldRow.Value == worldID)
			{
				KFMOD.PlayUISound(GlobalAssets.GetSound(ColonyDiagnosticScreen.notificationSoundsInactive[result]));
				if (worldRow.Key.gameObject.activeInHierarchy)
				{
					worldRow.Key.StartCoroutine(VisualNotificationRoutine(worldRow.Key.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content").gameObject, worldRow.Key.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Indicator")));
				}
			}
		}
	}

	private IEnumerator VisualNotificationRoutine(GameObject contentGameObject, RectTransform indicator)
	{
		Vector2 defaultIndicatorSize = new Vector2(indicator.sizeDelta.x, indicator.sizeDelta.y);
		float bounceDuration = 1.5f;
		for (float k = 0f; k < bounceDuration; k += Time.unscaledDeltaTime)
		{
			contentGameObject.rectTransform().localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (k / bounceDuration)) * 108f);
			indicator.sizeDelta = defaultIndicatorSize + defaultIndicatorSize * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (k / bounceDuration))));
			yield return 0;
		}
		for (float j = 0f; j < bounceDuration; j += Time.unscaledDeltaTime)
		{
			contentGameObject.rectTransform().localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (j / bounceDuration)) * 80f);
			indicator.sizeDelta = defaultIndicatorSize + defaultIndicatorSize * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (j / bounceDuration))));
			yield return 0;
		}
		for (float i = 0f; i < bounceDuration; i += Time.unscaledDeltaTime)
		{
			contentGameObject.rectTransform().localPosition = Vector2.left * Mathf.RoundToInt(Mathf.Sin((float)Math.PI * (i / bounceDuration)) * 60f);
			indicator.sizeDelta = defaultIndicatorSize + defaultIndicatorSize * Mathf.RoundToInt(Mathf.Sin(6f * ((float)Math.PI * (i / bounceDuration))));
			yield return 0;
		}
		indicator.sizeDelta = defaultIndicatorSize;
		contentGameObject.rectTransform().localPosition = Vector2.zero;
	}
}
