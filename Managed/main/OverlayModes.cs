using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public abstract class OverlayModes
{
	public class GasConduits : ConduitMode
	{
		public static readonly HashedString ID = "GasConduit";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "GasVent";
		}

		public GasConduits()
			: base(OverlayScreen.GasVentIDs)
		{
		}
	}

	public class LiquidConduits : ConduitMode
	{
		public static readonly HashedString ID = "LiquidConduit";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public LiquidConduits()
			: base(OverlayScreen.LiquidVentIDs)
		{
		}
	}

	public abstract class ConduitMode : Mode
	{
		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private ICollection<Tag> targetIDs;

		private int objectTargetLayer;

		private int conduitTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public ConduitMode(ICollection<Tag> ids)
		{
			objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			targetIDs = ids;
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				if (layerTargets.Contains(item))
				{
					layerTargets.Remove(item);
				}
				partition.Remove(item);
			}
		}

		public override void Disable()
		{
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				float defaultDepth = Mode.GetDefaultDepth(layerTarget);
				Vector3 position = layerTarget.transform.GetPosition();
				position.z = defaultDepth;
				layerTarget.transform.SetPosition(position);
				KBatchedAnimController[] componentsInChildren = layerTarget.GetComponentsInChildren<KBatchedAnimController>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					TriggerResorting(componentsInChildren[i]);
				}
			}
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Disable();
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max, delegate(SaveLoadRoot root)
			{
				if (!(root == null))
				{
					float defaultDepth = Mode.GetDefaultDepth(root);
					Vector3 position2 = root.transform.GetPosition();
					position2.z = defaultDepth;
					root.transform.SetPosition(position2);
					KBatchedAnimController[] componentsInChildren2 = root.GetComponentsInChildren<KBatchedAnimController>();
					for (int j = 0; j < componentsInChildren2.Length; j++)
					{
						TriggerResorting(componentsInChildren2[j]);
					}
				}
			});
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (SaveLoadRoot item in allIntersecting)
			{
				if (item.GetComponent<Conduit>() != null)
				{
					AddTargetIfVisible(item, min, max, layerTargets, conduitTargetLayer);
					continue;
				}
				AddTargetIfVisible(item, min, max, layerTargets, objectTargetLayer, delegate(SaveLoadRoot root)
				{
					Vector3 position = root.transform.GetPosition();
					float z = position.z;
					KPrefabID component3 = root.GetComponent<KPrefabID>();
					if (component3 != null)
					{
						if (component3.HasTag(GameTags.OverlayInFrontOfConduits))
						{
							z = Grid.GetLayerZ((ViewMode() == LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) - 0.2f;
						}
						else if (component3.HasTag(GameTags.OverlayBehindConduits))
						{
							z = Grid.GetLayerZ((ViewMode() == LiquidConduits.ID) ? Grid.SceneLayer.LiquidConduits : Grid.SceneLayer.GasConduits) + 0.2f;
						}
					}
					position.z = z;
					root.transform.SetPosition(position);
					KBatchedAnimController[] componentsInChildren = root.GetComponentsInChildren<KBatchedAnimController>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						TriggerResorting(componentsInChildren[i]);
					}
				});
			}
			GameObject gameObject = null;
			if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
			{
				gameObject = SelectTool.Instance.hover.gameObject;
			}
			connectedNetworks.Clear();
			float num = 1f;
			if (gameObject != null)
			{
				IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
				if (component != null)
				{
					int networkCell = component.GetNetworkCell();
					UtilityNetworkManager<FlowUtilityNetwork, Vent> mgr = ((ViewMode() == LiquidConduits.ID) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem);
					visited.Clear();
					FindConnectedNetworks(networkCell, mgr, connectedNetworks, visited);
					visited.Clear();
					num = ModeUtil.GetHighlightScale();
				}
			}
			Game.ConduitVisInfo conduitVisInfo = ((ViewMode() == LiquidConduits.ID) ? Game.Instance.liquidConduitVisInfo : Game.Instance.gasConduitVisInfo);
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (!(layerTarget == null) && layerTarget.GetComponent<IBridgedNetworkItem>() != null)
				{
					BuildingDef def = layerTarget.GetComponent<Building>().Def;
					Color32 tintColour = ((def.ThermalConductivity == 1f) ? GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayTintName) : ((!(def.ThermalConductivity < 1f)) ? GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayRadiantTintName) : GlobalAssets.Instance.colorSet.GetColorByName(conduitVisInfo.overlayInsulatedTintName)));
					if (connectedNetworks.Count > 0 && (layerTarget.GetComponent<IBridgedNetworkItem>()?.IsConnectedToNetworks(connectedNetworks) ?? false))
					{
						tintColour.r = (byte)((float)(int)tintColour.r * num);
						tintColour.g = (byte)((float)(int)tintColour.g * num);
						tintColour.b = (byte)((float)(int)tintColour.b * num);
					}
					KBatchedAnimController component2 = layerTarget.GetComponent<KBatchedAnimController>();
					component2.TintColour = tintColour;
				}
			}
		}

		private void TriggerResorting(KBatchedAnimController kbac)
		{
			if (kbac.enabled)
			{
				kbac.enabled = false;
				kbac.enabled = true;
			}
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, is_physical_building: false);
				if ((connections & UtilityConnections.Right) != 0)
				{
					FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != 0)
				{
					FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != 0)
				{
					FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != 0)
				{
					FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
				object endpoint = mgr.GetEndpoint(cell);
				if (endpoint != null)
				{
					(endpoint as FlowUtilityNetwork.NetworkItem)?.GameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(networks);
				}
			}
		}
	}

	public class Crop : BasePlantMode
	{
		private struct UpdateCropInfo
		{
			public HarvestDesignatable harvestable;

			public GameObject harvestableUI;

			public UpdateCropInfo(HarvestDesignatable harvestable, GameObject harvestableUI)
			{
				this.harvestable = harvestable;
				this.harvestableUI = harvestableUI;
			}
		}

		public static readonly HashedString ID = "Crop";

		private Canvas uiRoot;

		private List<UpdateCropInfo> updateCropInfo = new List<UpdateCropInfo>();

		private int freeHarvestableNotificationIdx = 0;

		private List<GameObject> harvestableNotificationList = new List<GameObject>();

		private GameObject harvestableNotificationPrefab;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[3]
		{
			new ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropHalted, delegate(KMonoBehaviour h)
			{
				WiltCondition component = h.GetComponent<WiltCondition>();
				return component != null && component.IsWilting();
			}),
			new ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropGrowing, (KMonoBehaviour h) => !(h as HarvestDesignatable).CanBeHarvested()),
			new ColorHighlightCondition((KMonoBehaviour h) => GlobalAssets.Instance.colorSet.cropGrown, (KMonoBehaviour h) => (h as HarvestDesignatable).CanBeHarvested())
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public Crop(Canvas ui_root, GameObject harvestable_notification_prefab)
			: base(OverlayScreen.HarvestableIDs)
		{
			uiRoot = ui_root;
			harvestableNotificationPrefab = harvestable_notification_prefab;
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			list.Add(new LegendEntry(UI.OVERLAYS.CROP.FULLY_GROWN, UI.OVERLAYS.CROP.TOOLTIPS.FULLY_GROWN, GlobalAssets.Instance.colorSet.cropGrown));
			list.Add(new LegendEntry(UI.OVERLAYS.CROP.GROWING, UI.OVERLAYS.CROP.TOOLTIPS.GROWING, GlobalAssets.Instance.colorSet.cropGrowing));
			list.Add(new LegendEntry(UI.OVERLAYS.CROP.GROWTH_HALTED, UI.OVERLAYS.CROP.TOOLTIPS.GROWTH_HALTED, GlobalAssets.Instance.colorSet.cropHalted));
			return list;
		}

		public override void Update()
		{
			updateCropInfo.Clear();
			freeHarvestableNotificationIdx = 0;
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (HarvestDesignatable item in allIntersecting)
			{
				AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
			}
			foreach (HarvestDesignatable layerTarget in layerTargets)
			{
				Vector2I vector2I = Grid.PosToXY(layerTarget.transform.GetPosition());
				if (min <= vector2I && vector2I <= max)
				{
					AddCropUI(layerTarget);
				}
			}
			foreach (UpdateCropInfo item2 in updateCropInfo)
			{
				item2.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(item2.harvestable);
			}
			for (int i = freeHarvestableNotificationIdx; i < harvestableNotificationList.Count; i++)
			{
				if (harvestableNotificationList[i].activeSelf)
				{
					harvestableNotificationList[i].SetActive(value: false);
				}
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Constant, targetLayer);
			base.Update();
		}

		public override void Disable()
		{
			DisableHarvestableUINotifications();
			base.Disable();
		}

		private void DisableHarvestableUINotifications()
		{
			freeHarvestableNotificationIdx = 0;
			foreach (GameObject harvestableNotification in harvestableNotificationList)
			{
				harvestableNotification.SetActive(value: false);
			}
			updateCropInfo.Clear();
		}

		public GameObject GetFreeCropUI()
		{
			GameObject gameObject = null;
			if (freeHarvestableNotificationIdx < harvestableNotificationList.Count)
			{
				gameObject = harvestableNotificationList[freeHarvestableNotificationIdx];
				if (!gameObject.gameObject.activeSelf)
				{
					gameObject.gameObject.SetActive(value: true);
				}
				freeHarvestableNotificationIdx++;
			}
			else
			{
				gameObject = Util.KInstantiateUI(harvestableNotificationPrefab.gameObject, uiRoot.transform.gameObject);
				harvestableNotificationList.Add(gameObject);
				freeHarvestableNotificationIdx++;
			}
			return gameObject;
		}

		private void AddCropUI(HarvestDesignatable harvestable)
		{
			GameObject freeCropUI = GetFreeCropUI();
			UpdateCropInfo item = new UpdateCropInfo(harvestable, freeCropUI);
			Vector3 b = Grid.CellToPos(Grid.PosToCell(harvestable), 0.5f, -1.25f, 0f);
			freeCropUI.GetComponent<RectTransform>().SetPosition(Vector3.up + b);
			updateCropInfo.Add(item);
		}
	}

	public class Harvest : BasePlantMode
	{
		public static readonly HashedString ID = "HarvestWhenReady";

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition((KMonoBehaviour harvestable) => new Color(0.65f, 0.65f, 0.65f, 0.65f), (KMonoBehaviour harvestable) => true)
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Harvest";
		}

		public Harvest()
			: base(OverlayScreen.HarvestableIDs)
		{
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (HarvestDesignatable item in allIntersecting)
			{
				AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Constant, targetLayer);
			base.Update();
		}
	}

	public abstract class BasePlantMode : Mode
	{
		protected UniformGrid<HarvestDesignatable> partition;

		protected HashSet<HarvestDesignatable> layerTargets = new HashSet<HarvestDesignatable>();

		protected ICollection<Tag> targetIDs;

		protected int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public BasePlantMode(ICollection<Tag> ids)
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = LayerMask.GetMask("MaskedOverlay");
			targetIDs = ids;
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<HarvestDesignatable>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				HarvestDesignatable component = item.GetComponent<HarvestDesignatable>();
				if (!(component == null))
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			HarvestDesignatable component = item.GetComponent<HarvestDesignatable>();
			if (!(component == null))
			{
				if (layerTargets.Contains(component))
				{
					layerTargets.Remove(component);
				}
				partition.Remove(component);
			}
		}

		public override void Disable()
		{
			UnregisterSaveLoadListeners();
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			partition.Clear();
			layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
		}
	}

	public class Decor : Mode
	{
		public static readonly HashedString ID = "Decor";

		private UniformGrid<DecorProvider> partition;

		private HashSet<DecorProvider> layerTargets = new HashSet<DecorProvider>();

		private List<DecorProvider> workingTargets = new List<DecorProvider>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition(delegate(KMonoBehaviour dp)
			{
				Color black = Color.black;
				Color b = Color.black;
				if (dp != null)
				{
					int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					float decorForCell = (dp as DecorProvider).GetDecorForCell(cell);
					if (decorForCell > 0f)
					{
						b = GlobalAssets.Instance.colorSet.decorHighlightPositive;
					}
					else if (decorForCell < 0f)
					{
						b = GlobalAssets.Instance.colorSet.decorHighlightNegative;
					}
					else if (dp.GetComponent<MonumentPart>() != null && dp.GetComponent<MonumentPart>().IsMonumentCompleted())
					{
						List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(dp.GetComponent<AttachableBuilding>());
						foreach (GameObject item in attachedNetwork)
						{
							decorForCell = item.GetComponent<DecorProvider>().GetDecorForCell(cell);
							if (decorForCell > 0f)
							{
								b = GlobalAssets.Instance.colorSet.decorHighlightPositive;
								break;
							}
							if (decorForCell < 0f)
							{
								b = GlobalAssets.Instance.colorSet.decorHighlightNegative;
								break;
							}
						}
					}
				}
				return Color.Lerp(black, b, 0.85f);
			}, (KMonoBehaviour dp) => SelectToolHoverTextCard.highlightedObjects.Contains(dp.gameObject))
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Decor";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			list.Add(new LegendEntry(UI.OVERLAYS.DECOR.HIGHDECOR, UI.OVERLAYS.DECOR.TOOLTIPS.HIGHDECOR, GlobalAssets.Instance.colorSet.decorPositive));
			list.Add(new LegendEntry(UI.OVERLAYS.DECOR.LOWDECOR, UI.OVERLAYS.DECOR.TOOLTIPS.LOWDECOR, GlobalAssets.Instance.colorSet.decorNegative));
			return list;
		}

		public Decor()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<DecorProvider>();
			targetIDs.UnionWith(prefabTagsWithComponent);
			Tag[] array = new Tag[5]
			{
				new Tag("Tile"),
				new Tag("MeshTile"),
				new Tag("InsulationTile"),
				new Tag("GasPermeableMembrane"),
				new Tag("CarpetTile")
			};
			Tag[] array2 = array;
			foreach (Tag item in array2)
			{
				targetIDs.Remove(item);
			}
			foreach (Tag gasVentID in OverlayScreen.GasVentIDs)
			{
				targetIDs.Remove(gasVentID);
			}
			foreach (Tag liquidVentID in OverlayScreen.LiquidVentIDs)
			{
				targetIDs.Remove(liquidVentID);
			}
			partition = Mode.PopulatePartition<DecorProvider>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y), workingTargets);
			for (int i = 0; i < workingTargets.Count; i++)
			{
				DecorProvider instance = workingTargets[i];
				AddTargetIfVisible(instance, min, max, layerTargets, targetLayer);
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Conditional, targetLayer);
			workingTargets.Clear();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				DecorProvider component = item.GetComponent<DecorProvider>();
				if (component != null)
				{
					partition.Add(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (item == null || item.gameObject == null)
			{
				return;
			}
			DecorProvider component = item.GetComponent<DecorProvider>();
			if (component != null)
			{
				if (layerTargets.Contains(component))
				{
					layerTargets.Remove(component);
				}
				partition.Remove(component);
			}
		}

		public override void Disable()
		{
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
		}
	}

	public class Disease : Mode
	{
		private struct DiseaseSortInfo
		{
			public float sortkey;

			public Klei.AI.Disease disease;

			public DiseaseSortInfo(Klei.AI.Disease d)
			{
				disease = d;
				sortkey = CalculateHUE(GlobalAssets.Instance.colorSet.GetColorByName(d.overlayColourName));
			}
		}

		private struct UpdateDiseaseInfo
		{
			public DiseaseOverlayWidget ui;

			public AmountInstance valueSrc;

			public UpdateDiseaseInfo(AmountInstance amount_inst, DiseaseOverlayWidget ui)
			{
				this.ui = ui;
				valueSrc = amount_inst;
			}
		}

		public static readonly HashedString ID = "Disease";

		private int cameraLayerMask;

		private int freeDiseaseUI = 0;

		private List<GameObject> diseaseUIList = new List<GameObject>();

		private List<UpdateDiseaseInfo> updateDiseaseInfo = new List<UpdateDiseaseInfo>();

		private HashSet<KMonoBehaviour> layerTargets = new HashSet<KMonoBehaviour>();

		private HashSet<KMonoBehaviour> privateTargets = new HashSet<KMonoBehaviour>();

		private List<KMonoBehaviour> queuedAdds = new List<KMonoBehaviour>();

		private Canvas diseaseUIParent;

		private GameObject diseaseOverlayPrefab;

		private static float CalculateHUE(Color32 colour)
		{
			byte b = Math.Max(colour.r, Math.Max(colour.g, colour.b));
			byte b2 = Math.Min(colour.r, Math.Min(colour.g, colour.b));
			float result = 0f;
			int num = b - b2;
			if (num == 0)
			{
				result = 0f;
			}
			else if (b == colour.r)
			{
				result = (float)(colour.g - colour.b) / (float)num % 6f;
			}
			else if (b == colour.g)
			{
				result = (float)(colour.b - colour.r) / (float)num + 2f;
			}
			else if (b == colour.b)
			{
				result = (float)(colour.r - colour.g) / (float)num + 4f;
			}
			return result;
		}

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Disease";
		}

		public Disease(Canvas diseaseUIParent, GameObject diseaseOverlayPrefab)
		{
			this.diseaseUIParent = diseaseUIParent;
			this.diseaseOverlayPrefab = diseaseOverlayPrefab;
			legendFilters = CreateDefaultFilters();
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
		}

		public override void Enable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disease);
			CameraController.Instance.ToggleColouredOverlayView(enabled: true);
			Camera.main.cullingMask |= cameraLayerMask;
			RegisterSaveLoadListeners();
			foreach (DiseaseSourceVisualizer item in Components.DiseaseSourceVisualizers.Items)
			{
				if (!(item == null))
				{
					item.Show(ViewMode());
				}
			}
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			Dictionary<string, ToolParameterMenu.ToggleState> dictionary = new Dictionary<string, ToolParameterMenu.ToggleState>();
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
			return dictionary;
		}

		public override void OnFiltersChanged()
		{
			Game.Instance.showGasConduitDisease = InFilter(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, legendFilters);
			Game.Instance.showLiquidConduitDisease = InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, legendFilters);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			if (!(item == null))
			{
				KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
				if (!(component == null))
				{
					InfraredVisualizerComponents.ClearOverlayColour(component);
				}
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
		}

		public override void Disable()
		{
			foreach (DiseaseSourceVisualizer item in Components.DiseaseSourceVisualizers.Items)
			{
				if (!(item == null))
				{
					item.Show(None.ID);
				}
			}
			UnregisterSaveLoadListeners();
			Camera.main.cullingMask &= ~cameraLayerMask;
			foreach (KMonoBehaviour layerTarget in layerTargets)
			{
				if (!(layerTarget == null))
				{
					float defaultDepth = Mode.GetDefaultDepth(layerTarget);
					Vector3 position = layerTarget.transform.GetPosition();
					position.z = defaultDepth;
					layerTarget.transform.SetPosition(position);
					KBatchedAnimController component = layerTarget.GetComponent<KBatchedAnimController>();
					component.enabled = false;
					component.enabled = true;
				}
			}
			CameraController.Instance.ToggleColouredOverlayView(enabled: false);
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			Game.Instance.showGasConduitDisease = false;
			Game.Instance.showLiquidConduitDisease = false;
			freeDiseaseUI = 0;
			foreach (UpdateDiseaseInfo item2 in updateDiseaseInfo)
			{
				item2.ui.gameObject.SetActive(value: false);
			}
			updateDiseaseInfo.Clear();
			privateTargets.Clear();
			layerTargets.Clear();
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			List<DiseaseSortInfo> list2 = new List<DiseaseSortInfo>();
			foreach (Klei.AI.Disease resource in Db.Get().Diseases.resources)
			{
				list2.Add(new DiseaseSortInfo(resource));
			}
			list2.Sort((DiseaseSortInfo a, DiseaseSortInfo b) => a.sortkey.CompareTo(b.sortkey));
			foreach (DiseaseSortInfo item in list2)
			{
				list.Add(new LegendEntry(item.disease.Name, item.disease.overlayLegendHovertext.ToString(), GlobalAssets.Instance.colorSet.GetColorByName(item.disease.overlayColourName)));
			}
			return list;
		}

		public GameObject GetFreeDiseaseUI()
		{
			GameObject gameObject = null;
			if (freeDiseaseUI < diseaseUIList.Count)
			{
				gameObject = diseaseUIList[freeDiseaseUI];
				gameObject.gameObject.SetActive(value: true);
				freeDiseaseUI++;
			}
			else
			{
				gameObject = Util.KInstantiateUI(diseaseOverlayPrefab, diseaseUIParent.transform.gameObject);
				diseaseUIList.Add(gameObject);
				freeDiseaseUI++;
			}
			return gameObject;
		}

		private void AddDiseaseUI(MinionIdentity target)
		{
			GameObject gameObject = GetFreeDiseaseUI();
			DiseaseOverlayWidget component = gameObject.GetComponent<DiseaseOverlayWidget>();
			AmountInstance amount_inst = target.GetComponent<Modifiers>().amounts.Get(Db.Get().Amounts.ImmuneLevel);
			UpdateDiseaseInfo item = new UpdateDiseaseInfo(amount_inst, component);
			KAnimControllerBase component2 = target.GetComponent<KAnimControllerBase>();
			Vector3 position = ((component2 != null) ? component2.GetWorldPivot() : (target.transform.GetPosition() + Vector3.down));
			gameObject.GetComponent<RectTransform>().SetPosition(position);
			updateDiseaseInfo.Add(item);
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			using (new KProfiler.Region("UpdateDiseaseCarriers"))
			{
				queuedAdds.Clear();
				foreach (MinionIdentity item in Components.LiveMinionIdentities.Items)
				{
					if (!(item == null))
					{
						Vector2I vector2I = Grid.PosToXY(item.transform.GetPosition());
						if (min <= vector2I && vector2I <= max && !privateTargets.Contains(item))
						{
							AddDiseaseUI(item);
							queuedAdds.Add(item);
						}
					}
				}
				foreach (KMonoBehaviour queuedAdd in queuedAdds)
				{
					privateTargets.Add(queuedAdd);
				}
				queuedAdds.Clear();
			}
			foreach (UpdateDiseaseInfo item2 in updateDiseaseInfo)
			{
				item2.ui.Refresh(item2.valueSrc);
			}
			bool flag = false;
			if (Game.Instance.showLiquidConduitDisease)
			{
				foreach (Tag liquidVentID in OverlayScreen.LiquidVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(liquidVentID))
					{
						OverlayScreen.DiseaseIDs.Add(liquidVentID);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag liquidVentID2 in OverlayScreen.LiquidVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(liquidVentID2))
					{
						OverlayScreen.DiseaseIDs.Remove(liquidVentID2);
						flag = true;
					}
				}
			}
			if (Game.Instance.showGasConduitDisease)
			{
				foreach (Tag gasVentID in OverlayScreen.GasVentIDs)
				{
					if (!OverlayScreen.DiseaseIDs.Contains(gasVentID))
					{
						OverlayScreen.DiseaseIDs.Add(gasVentID);
						flag = true;
					}
				}
			}
			else
			{
				foreach (Tag gasVentID2 in OverlayScreen.GasVentIDs)
				{
					if (OverlayScreen.DiseaseIDs.Contains(gasVentID2))
					{
						OverlayScreen.DiseaseIDs.Remove(gasVentID2);
						flag = true;
					}
				}
			}
			if (flag)
			{
				SetLayerZ(-50f);
			}
		}

		private void SetLayerZ(float offset_z)
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.ClearOutsideViewObjects(layerTargets, min, max, OverlayScreen.DiseaseIDs, delegate(KMonoBehaviour go)
			{
				if (go != null)
				{
					float defaultDepth2 = Mode.GetDefaultDepth(go);
					Vector3 position2 = go.transform.GetPosition();
					position2.z = defaultDepth2;
					go.transform.SetPosition(position2);
					KBatchedAnimController component2 = go.GetComponent<KBatchedAnimController>();
					component2.enabled = false;
					component2.enabled = true;
				}
			});
			Dictionary<Tag, List<SaveLoadRoot>> lists = SaveLoader.Instance.saveManager.GetLists();
			foreach (Tag diseaseID in OverlayScreen.DiseaseIDs)
			{
				if (!lists.TryGetValue(diseaseID, out var value))
				{
					continue;
				}
				foreach (SaveLoadRoot item in value)
				{
					if (!(item == null) && !layerTargets.Contains(item))
					{
						Vector3 position = item.transform.GetPosition();
						if (Grid.IsVisible(Grid.PosToCell(position)) && min <= position && position <= max)
						{
							float defaultDepth = Mode.GetDefaultDepth(item);
							position.z = defaultDepth + offset_z;
							item.transform.SetPosition(position);
							KBatchedAnimController component = item.GetComponent<KBatchedAnimController>();
							component.enabled = false;
							component.enabled = true;
							layerTargets.Add(item);
						}
					}
				}
			}
		}
	}

	public class Logic : Mode
	{
		private struct BridgeInfo
		{
			public int cell;

			public KBatchedAnimController controller;
		}

		private struct EventInfo
		{
			public HandleVector<int>.Handle uiHandle;
		}

		private struct UIInfo
		{
			public GameObject instance;

			public Image image;

			public int cell;

			public int bitDepth;

			public UIInfo(ILogicUIElement ui_elem, LogicModeUI ui_data)
			{
				cell = ui_elem.GetLogicUICell();
				GameObject original = null;
				Sprite sprite = null;
				bitDepth = 1;
				switch (ui_elem.GetLogicPortSpriteType())
				{
				case LogicPortSpriteType.Input:
					original = ui_data.prefab;
					sprite = ui_data.inputSprite;
					break;
				case LogicPortSpriteType.Output:
					original = ui_data.prefab;
					sprite = ui_data.outputSprite;
					break;
				case LogicPortSpriteType.ResetUpdate:
					original = ui_data.prefab;
					sprite = ui_data.resetSprite;
					break;
				case LogicPortSpriteType.ControlInput:
					original = ui_data.controlInputPrefab;
					break;
				case LogicPortSpriteType.RibbonInput:
					original = ui_data.ribbonInputPrefab;
					bitDepth = 4;
					break;
				case LogicPortSpriteType.RibbonOutput:
					original = ui_data.ribbonOutputPrefab;
					bitDepth = 4;
					break;
				}
				instance = Util.KInstantiate(original, Grid.CellToPosCCC(cell, Grid.SceneLayer.Front), Quaternion.identity, GameScreenManager.Instance.worldSpaceCanvas);
				instance.SetActive(value: true);
				image = instance.GetComponent<Image>();
				if (image != null)
				{
					image.raycastTarget = false;
					image.sprite = sprite;
				}
			}

			public void Release()
			{
				Util.KDestroyGameObject(instance);
			}
		}

		public static readonly HashedString ID = "Logic";

		public static HashSet<Tag> HighlightItemIDs = new HashSet<Tag>();

		public static KAnimHashedString RIBBON_WIRE_1_SYMBOL_NAME = "wire1";

		public static KAnimHashedString RIBBON_WIRE_2_SYMBOL_NAME = "wire2";

		public static KAnimHashedString RIBBON_WIRE_3_SYMBOL_NAME = "wire3";

		public static KAnimHashedString RIBBON_WIRE_4_SYMBOL_NAME = "wire4";

		private int conduitTargetLayer;

		private int objectTargetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private UniformGrid<ILogicUIElement> ioPartition;

		private HashSet<ILogicUIElement> ioTargets = new HashSet<ILogicUIElement>();

		private HashSet<ILogicUIElement> workingIOTargets = new HashSet<ILogicUIElement>();

		private HashSet<KBatchedAnimController> wireControllers = new HashSet<KBatchedAnimController>();

		private HashSet<KBatchedAnimController> ribbonControllers = new HashSet<KBatchedAnimController>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private HashSet<BridgeInfo> bridgeControllers = new HashSet<BridgeInfo>();

		private HashSet<BridgeInfo> ribbonBridgeControllers = new HashSet<BridgeInfo>();

		private UniformGrid<SaveLoadRoot> gameObjPartition;

		private HashSet<SaveLoadRoot> gameObjTargets = new HashSet<SaveLoadRoot>();

		private LogicModeUI uiAsset;

		private Dictionary<ILogicUIElement, EventInfo> uiNodes = new Dictionary<ILogicUIElement, EventInfo>();

		private KCompactedVector<UIInfo> uiInfo = new KCompactedVector<UIInfo>();

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Logic";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.INPUT, Color.white, null, Assets.GetSprite("logicInput")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.OUTPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.OUTPUT, Color.white, null, Assets.GetSprite("logicOutput")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.RIBBON_INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_INPUT, Color.white, null, Assets.GetSprite("logic_ribbon_all_in")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.RIBBON_OUTPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.RIBBON_OUTPUT, Color.white, null, Assets.GetSprite("logic_ribbon_all_out")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.RESET_UPDATE, UI.OVERLAYS.LOGIC.TOOLTIPS.RESET_UPDATE, Color.white, null, Assets.GetSprite("logicResetUpdate")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.CONTROL_INPUT, UI.OVERLAYS.LOGIC.TOOLTIPS.CONTROL_INPUT, Color.white, null, Assets.GetSprite("control_input_frame_legend")));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.CIRCUIT_STATUS_HEADER, null, Color.white, null, null, displaySprite: false));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.ONE, null, GlobalAssets.Instance.colorSet.logicOnText));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.ZERO, null, GlobalAssets.Instance.colorSet.logicOffText));
			list.Add(new LegendEntry(UI.OVERLAYS.LOGIC.DISCONNECTED, UI.OVERLAYS.LOGIC.TOOLTIPS.DISCONNECTED, GlobalAssets.Instance.colorSet.logicDisconnected));
			return list;
		}

		public Logic(LogicModeUI ui_asset)
		{
			conduitTargetLayer = LayerMask.NameToLayer("MaskedOverlay");
			objectTargetLayer = LayerMask.NameToLayer("MaskedOverlayBG");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			uiAsset = ui_asset;
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			RegisterSaveLoadListeners();
			gameObjPartition = Mode.PopulatePartition<SaveLoadRoot>(HighlightItemIDs);
			ioPartition = CreateLogicUIPartition();
			GridCompositor.Instance.ToggleMinor(on: true);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Combine(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(OnUIElemRemoved));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterLogicOn);
		}

		public override void Disable()
		{
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			logicCircuitManager.onElemAdded = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager.onElemAdded, new Action<ILogicUIElement>(OnUIElemAdded));
			LogicCircuitManager logicCircuitManager2 = Game.Instance.logicCircuitManager;
			logicCircuitManager2.onElemRemoved = (Action<ILogicUIElement>)Delegate.Remove(logicCircuitManager2.onElemRemoved, new Action<ILogicUIElement>(OnUIElemRemoved));
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterLogicOn);
			foreach (SaveLoadRoot gameObjTarget in gameObjTargets)
			{
				float defaultDepth = Mode.GetDefaultDepth(gameObjTarget);
				Vector3 position = gameObjTarget.transform.GetPosition();
				position.z = defaultDepth;
				gameObjTarget.transform.SetPosition(position);
				gameObjTarget.GetComponent<KBatchedAnimController>().enabled = false;
				gameObjTarget.GetComponent<KBatchedAnimController>().enabled = true;
			}
			Mode.ResetDisplayValues(gameObjTargets);
			Mode.ResetDisplayValues(wireControllers);
			Mode.ResetDisplayValues(ribbonControllers);
			ResetRibbonSymbolTints(ribbonControllers);
			foreach (BridgeInfo bridgeController in bridgeControllers)
			{
				if (bridgeController.controller != null)
				{
					Mode.ResetDisplayValues(bridgeController.controller);
				}
			}
			foreach (BridgeInfo ribbonBridgeController in ribbonBridgeControllers)
			{
				if (ribbonBridgeController.controller != null)
				{
					ResetRibbonTint(ribbonBridgeController.controller);
				}
			}
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			UnregisterSaveLoadListeners();
			foreach (UIInfo data in uiInfo.GetDataList())
			{
				data.Release();
			}
			uiInfo.Clear();
			uiNodes.Clear();
			ioPartition.Clear();
			ioTargets.Clear();
			gameObjPartition.Clear();
			gameObjTargets.Clear();
			wireControllers.Clear();
			ribbonControllers.Clear();
			bridgeControllers.Clear();
			ribbonBridgeControllers.Clear();
			GridCompositor.Instance.ToggleMinor(on: false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (HighlightItemIDs.Contains(saveLoadTag))
			{
				gameObjPartition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				if (gameObjTargets.Contains(item))
				{
					gameObjTargets.Remove(item);
				}
				gameObjPartition.Remove(item);
			}
		}

		private void OnUIElemAdded(ILogicUIElement elem)
		{
			ioPartition.Add(elem);
		}

		private void OnUIElemRemoved(ILogicUIElement elem)
		{
			ioPartition.Remove(elem);
			if (ioTargets.Contains(elem))
			{
				ioTargets.Remove(elem);
				FreeUI(elem);
			}
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Tag wire_id = TagManager.Create("LogicWire");
			Tag ribbon_id = TagManager.Create("LogicRibbon");
			Tag bridge_id = TagManager.Create("LogicWireBridge");
			Tag ribbon_bridge_id = TagManager.Create("LogicRibbonBridge");
			Mode.RemoveOffscreenTargets(gameObjTargets, min, max, delegate(SaveLoadRoot root)
			{
				if (!(root == null))
				{
					KPrefabID component10 = root.GetComponent<KPrefabID>();
					if (component10 != null)
					{
						Tag prefabTag = component10.PrefabTag;
						if (prefabTag == wire_id)
						{
							wireControllers.Remove(root.GetComponent<KBatchedAnimController>());
						}
						else if (prefabTag == ribbon_id)
						{
							ResetRibbonTint(root.GetComponent<KBatchedAnimController>());
							ribbonControllers.Remove(root.GetComponent<KBatchedAnimController>());
						}
						else if (prefabTag == bridge_id)
						{
							KBatchedAnimController controller2 = root.GetComponent<KBatchedAnimController>();
							bridgeControllers.RemoveWhere((BridgeInfo x) => x.controller == controller2);
						}
						else if (prefabTag == ribbon_bridge_id)
						{
							KBatchedAnimController controller = root.GetComponent<KBatchedAnimController>();
							ResetRibbonTint(controller);
							ribbonBridgeControllers.RemoveWhere((BridgeInfo x) => x.controller == controller);
						}
						else
						{
							float defaultDepth = Mode.GetDefaultDepth(root);
							Vector3 position2 = root.transform.GetPosition();
							position2.z = defaultDepth;
							root.transform.SetPosition(position2);
							root.GetComponent<KBatchedAnimController>().enabled = false;
							root.GetComponent<KBatchedAnimController>().enabled = true;
						}
					}
				}
			});
			Mode.RemoveOffscreenTargets(ioTargets, workingIOTargets, min, max, FreeUI);
			using (new KProfiler.Region("UpdateLogicOverlay"))
			{
				IEnumerable allIntersecting = gameObjPartition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
				foreach (SaveLoadRoot item2 in allIntersecting)
				{
					if (!(item2 != null))
					{
						continue;
					}
					KPrefabID component = item2.GetComponent<KPrefabID>();
					if (component.PrefabTag == wire_id || component.PrefabTag == bridge_id || component.PrefabTag == ribbon_id || component.PrefabTag == ribbon_bridge_id)
					{
						AddTargetIfVisible(item2, min, max, gameObjTargets, conduitTargetLayer, delegate(SaveLoadRoot root)
						{
							if (!(root == null))
							{
								KPrefabID component5 = root.GetComponent<KPrefabID>();
								if (HighlightItemIDs.Contains(component5.PrefabTag))
								{
									BridgeInfo item;
									if (component5.PrefabTag == wire_id)
									{
										wireControllers.Add(root.GetComponent<KBatchedAnimController>());
									}
									else if (component5.PrefabTag == ribbon_id)
									{
										ribbonControllers.Add(root.GetComponent<KBatchedAnimController>());
									}
									else if (component5.PrefabTag == bridge_id)
									{
										KBatchedAnimController component6 = root.GetComponent<KBatchedAnimController>();
										LogicUtilityNetworkLink component7 = root.GetComponent<LogicUtilityNetworkLink>();
										int networkCell2 = component7.GetNetworkCell();
										HashSet<BridgeInfo> hashSet = bridgeControllers;
										item = new BridgeInfo
										{
											cell = networkCell2,
											controller = component6
										};
										hashSet.Add(item);
									}
									else if (component5.PrefabTag == ribbon_bridge_id)
									{
										KBatchedAnimController component8 = root.GetComponent<KBatchedAnimController>();
										LogicUtilityNetworkLink component9 = root.GetComponent<LogicUtilityNetworkLink>();
										int networkCell3 = component9.GetNetworkCell();
										HashSet<BridgeInfo> hashSet2 = ribbonBridgeControllers;
										item = new BridgeInfo
										{
											cell = networkCell3,
											controller = component8
										};
										hashSet2.Add(item);
									}
								}
							}
						});
						continue;
					}
					AddTargetIfVisible(item2, min, max, gameObjTargets, objectTargetLayer, delegate(SaveLoadRoot root)
					{
						Vector3 position = root.transform.GetPosition();
						float z = position.z;
						KPrefabID component3 = root.GetComponent<KPrefabID>();
						if (component3 != null)
						{
							if (component3.HasTag(GameTags.OverlayInFrontOfConduits))
							{
								z = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) - 0.2f;
							}
							else if (component3.HasTag(GameTags.OverlayBehindConduits))
							{
								z = Grid.GetLayerZ(Grid.SceneLayer.LogicWires) + 0.2f;
							}
						}
						position.z = z;
						root.transform.SetPosition(position);
						KBatchedAnimController component4 = root.GetComponent<KBatchedAnimController>();
						component4.enabled = false;
						component4.enabled = true;
					});
				}
				IEnumerable allIntersecting2 = ioPartition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
				foreach (ILogicUIElement item3 in allIntersecting2)
				{
					if (item3 != null)
					{
						AddTargetIfVisible(item3, min, max, ioTargets, objectTargetLayer, AddUI, (KMonoBehaviour kcmp) => kcmp != null && HighlightItemIDs.Contains(kcmp.GetComponent<KPrefabID>().PrefabTag));
					}
				}
				connectedNetworks.Clear();
				float num = 1f;
				GameObject gameObject = null;
				if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
				{
					gameObject = SelectTool.Instance.hover.gameObject;
				}
				if (gameObject != null)
				{
					IBridgedNetworkItem component2 = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component2 != null)
					{
						int networkCell = component2.GetNetworkCell();
						visited.Clear();
						FindConnectedNetworks(networkCell, Game.Instance.logicCircuitSystem, connectedNetworks, visited);
						visited.Clear();
						num = ModeUtil.GetHighlightScale();
					}
				}
				LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
				Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
				Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
				logicOff.a = (logicOn.a = 0);
				foreach (KBatchedAnimController wireController in wireControllers)
				{
					if (!(wireController == null))
					{
						Color32 tintColour = logicOff;
						LogicCircuitNetwork networkForCell = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(wireController.transform.GetPosition()));
						if (networkForCell != null)
						{
							tintColour = (networkForCell.IsBitActive(0) ? logicOn : logicOff);
						}
						if (connectedNetworks.Count > 0 && (wireController.GetComponent<IBridgedNetworkItem>()?.IsConnectedToNetworks(connectedNetworks) ?? false))
						{
							tintColour.r = (byte)((float)(int)tintColour.r * num);
							tintColour.g = (byte)((float)(int)tintColour.g * num);
							tintColour.b = (byte)((float)(int)tintColour.b * num);
						}
						wireController.TintColour = tintColour;
					}
				}
				foreach (KBatchedAnimController ribbonController in ribbonControllers)
				{
					if (!(ribbonController == null))
					{
						Color32 c = logicOff;
						Color32 c2 = logicOff;
						Color32 c3 = logicOff;
						Color32 c4 = logicOff;
						LogicCircuitNetwork networkForCell2 = logicCircuitManager.GetNetworkForCell(Grid.PosToCell(ribbonController.transform.GetPosition()));
						if (networkForCell2 != null)
						{
							c = (networkForCell2.IsBitActive(0) ? logicOn : logicOff);
							c2 = (networkForCell2.IsBitActive(1) ? logicOn : logicOff);
							c3 = (networkForCell2.IsBitActive(2) ? logicOn : logicOff);
							c4 = (networkForCell2.IsBitActive(3) ? logicOn : logicOff);
						}
						if (connectedNetworks.Count > 0 && (ribbonController.GetComponent<IBridgedNetworkItem>()?.IsConnectedToNetworks(connectedNetworks) ?? false))
						{
							c.r = (byte)((float)(int)c.r * num);
							c.g = (byte)((float)(int)c.g * num);
							c.b = (byte)((float)(int)c.b * num);
							c2.r = (byte)((float)(int)c2.r * num);
							c2.g = (byte)((float)(int)c2.g * num);
							c2.b = (byte)((float)(int)c2.b * num);
							c3.r = (byte)((float)(int)c3.r * num);
							c3.g = (byte)((float)(int)c3.g * num);
							c3.b = (byte)((float)(int)c3.b * num);
							c4.r = (byte)((float)(int)c4.r * num);
							c4.g = (byte)((float)(int)c4.g * num);
							c4.b = (byte)((float)(int)c4.b * num);
						}
						ribbonController.SetSymbolTint(RIBBON_WIRE_1_SYMBOL_NAME, c);
						ribbonController.SetSymbolTint(RIBBON_WIRE_2_SYMBOL_NAME, c2);
						ribbonController.SetSymbolTint(RIBBON_WIRE_3_SYMBOL_NAME, c3);
						ribbonController.SetSymbolTint(RIBBON_WIRE_4_SYMBOL_NAME, c4);
					}
				}
				foreach (BridgeInfo bridgeController in bridgeControllers)
				{
					if (!(bridgeController.controller == null))
					{
						Color32 tintColour2 = logicOff;
						LogicCircuitNetwork networkForCell3 = logicCircuitManager.GetNetworkForCell(bridgeController.cell);
						if (networkForCell3 != null)
						{
							tintColour2 = (networkForCell3.IsBitActive(0) ? logicOn : logicOff);
						}
						if (connectedNetworks.Count > 0 && (bridgeController.controller.GetComponent<IBridgedNetworkItem>()?.IsConnectedToNetworks(connectedNetworks) ?? false))
						{
							tintColour2.r = (byte)((float)(int)tintColour2.r * num);
							tintColour2.g = (byte)((float)(int)tintColour2.g * num);
							tintColour2.b = (byte)((float)(int)tintColour2.b * num);
						}
						bridgeController.controller.TintColour = tintColour2;
					}
				}
				foreach (BridgeInfo ribbonBridgeController in ribbonBridgeControllers)
				{
					if (!(ribbonBridgeController.controller == null))
					{
						Color32 c5 = logicOff;
						Color32 c6 = logicOff;
						Color32 c7 = logicOff;
						Color32 c8 = logicOff;
						LogicCircuitNetwork networkForCell4 = logicCircuitManager.GetNetworkForCell(ribbonBridgeController.cell);
						if (networkForCell4 != null)
						{
							c5 = (networkForCell4.IsBitActive(0) ? logicOn : logicOff);
							c6 = (networkForCell4.IsBitActive(1) ? logicOn : logicOff);
							c7 = (networkForCell4.IsBitActive(2) ? logicOn : logicOff);
							c8 = (networkForCell4.IsBitActive(3) ? logicOn : logicOff);
						}
						if (connectedNetworks.Count > 0 && (ribbonBridgeController.controller.GetComponent<IBridgedNetworkItem>()?.IsConnectedToNetworks(connectedNetworks) ?? false))
						{
							c5.r = (byte)((float)(int)c5.r * num);
							c5.g = (byte)((float)(int)c5.g * num);
							c5.b = (byte)((float)(int)c5.b * num);
							c6.r = (byte)((float)(int)c6.r * num);
							c6.g = (byte)((float)(int)c6.g * num);
							c6.b = (byte)((float)(int)c6.b * num);
							c7.r = (byte)((float)(int)c7.r * num);
							c7.g = (byte)((float)(int)c7.g * num);
							c7.b = (byte)((float)(int)c7.b * num);
							c8.r = (byte)((float)(int)c8.r * num);
							c8.g = (byte)((float)(int)c8.g * num);
							c8.b = (byte)((float)(int)c8.b * num);
						}
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_1_SYMBOL_NAME, c5);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_2_SYMBOL_NAME, c6);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_3_SYMBOL_NAME, c7);
						ribbonBridgeController.controller.SetSymbolTint(RIBBON_WIRE_4_SYMBOL_NAME, c8);
					}
				}
			}
			UpdateUI();
		}

		private void UpdateUI()
		{
			Color32 logicOn = GlobalAssets.Instance.colorSet.logicOn;
			Color32 logicOff = GlobalAssets.Instance.colorSet.logicOff;
			Color32 logicDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
			logicOff.a = (logicOn.a = byte.MaxValue);
			foreach (UIInfo data in uiInfo.GetDataList())
			{
				LogicCircuitNetwork networkForCell = Game.Instance.logicCircuitManager.GetNetworkForCell(data.cell);
				Color32 c = logicDisconnected;
				LogicControlInputUI component = data.instance.GetComponent<LogicControlInputUI>();
				if (component != null)
				{
					component.SetContent(networkForCell);
				}
				else if (data.bitDepth == 4)
				{
					LogicRibbonDisplayUI component2 = data.instance.GetComponent<LogicRibbonDisplayUI>();
					if (component2 != null)
					{
						component2.SetContent(networkForCell);
					}
				}
				else if (data.bitDepth == 1)
				{
					if (networkForCell != null)
					{
						c = (networkForCell.IsBitActive(0) ? logicOn : logicOff);
					}
					if (data.image.color != c)
					{
						data.image.color = c;
					}
				}
			}
		}

		private void AddUI(ILogicUIElement ui_elem)
		{
			if (!uiNodes.ContainsKey(ui_elem))
			{
				HandleVector<int>.Handle uiHandle = uiInfo.Allocate(new UIInfo(ui_elem, uiAsset));
				uiNodes.Add(ui_elem, new EventInfo
				{
					uiHandle = uiHandle
				});
			}
		}

		private void FreeUI(ILogicUIElement item)
		{
			if (item != null && uiNodes.TryGetValue(item, out var value))
			{
				uiInfo.GetData(value.uiHandle).Release();
				uiInfo.Free(value.uiHandle);
				uiNodes.Remove(item);
			}
		}

		protected UniformGrid<ILogicUIElement> CreateLogicUIPartition()
		{
			UniformGrid<ILogicUIElement> uniformGrid = new UniformGrid<ILogicUIElement>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
			ReadOnlyCollection<ILogicUIElement> visElements = logicCircuitManager.GetVisElements();
			foreach (ILogicUIElement item in visElements)
			{
				if (item != null)
				{
					uniformGrid.Add(item);
				}
			}
			return uniformGrid;
		}

		private bool IsBitActive(int value, int bit)
		{
			return (value & (1 << bit)) > 0;
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, is_physical_building: false);
				if ((connections & UtilityConnections.Right) != 0)
				{
					FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != 0)
				{
					FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != 0)
				{
					FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != 0)
				{
					FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
			}
		}

		private void ResetRibbonSymbolTints<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T target in targets)
			{
				if (!((UnityEngine.Object)target == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
					ResetRibbonTint(component);
				}
			}
		}

		private void ResetRibbonTint(KBatchedAnimController kbac)
		{
			if (kbac != null)
			{
				kbac.SetSymbolTint(RIBBON_WIRE_1_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(RIBBON_WIRE_2_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(RIBBON_WIRE_3_SYMBOL_NAME, Color.white);
				kbac.SetSymbolTint(RIBBON_WIRE_4_SYMBOL_NAME, Color.white);
			}
		}
	}

	public enum BringToFrontLayerSetting
	{
		None,
		Constant,
		Conditional
	}

	public class ColorHighlightCondition
	{
		public Func<KMonoBehaviour, Color> highlight_color;

		public Func<KMonoBehaviour, bool> highlight_condition;

		public ColorHighlightCondition(Func<KMonoBehaviour, Color> highlight_color, Func<KMonoBehaviour, bool> highlight_condition)
		{
			this.highlight_color = highlight_color;
			this.highlight_condition = highlight_condition;
		}
	}

	public class None : Mode
	{
		public static readonly HashedString ID = HashedString.Invalid;

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}
	}

	public class PathProber : Mode
	{
		public static readonly HashedString ID = "PathProber";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Off";
		}
	}

	public class Oxygen : Mode
	{
		public static readonly HashedString ID = "Oxygen";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Oxygen";
		}

		public override void Enable()
		{
			base.Enable();
			int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
			int mask = LayerMask.GetMask("MaskedOverlay");
			SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
		}

		public override void Disable()
		{
			base.Disable();
			SelectTool.Instance.ClearLayerMask();
		}
	}

	public class Light : Mode
	{
		public static readonly HashedString ID = "Light";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Lights";
		}
	}

	public class Priorities : Mode
	{
		public static readonly HashedString ID = "Priorities";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Priorities";
		}
	}

	public class ThermalConductivity : Mode
	{
		public static readonly HashedString ID = "ThermalConductivity";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}

	public class HeatFlow : Mode
	{
		public static readonly HashedString ID = "HeatFlow";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "HeatFlow";
		}
	}

	public class Rooms : Mode
	{
		public static readonly HashedString ID = "Rooms";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Rooms";
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			List<LegendEntry> list = new List<LegendEntry>();
			List<RoomType> list2 = new List<RoomType>(Db.Get().RoomTypes.resources);
			list2.Sort((RoomType a, RoomType b) => a.sortKey.CompareTo(b.sortKey));
			foreach (RoomType item in list2)
			{
				string text = item.GetCriteriaString();
				if (item.effects != null && item.effects.Length != 0)
				{
					text = text + "\n\n" + item.GetRoomEffectsString();
				}
				list.Add(new LegendEntry(item.Name + "\n" + item.effect, text, GlobalAssets.Instance.colorSet.GetColorByName(item.category.colorName)));
			}
			return list;
		}
	}

	public abstract class Mode
	{
		public Dictionary<string, ToolParameterMenu.ToggleState> legendFilters;

		private static List<KMonoBehaviour> workingTargets = new List<KMonoBehaviour>();

		public static void Clear()
		{
			workingTargets.Clear();
		}

		public abstract HashedString ViewMode();

		public virtual void Enable()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Disable()
		{
		}

		public virtual List<LegendEntry> GetCustomLegendData()
		{
			return null;
		}

		public virtual Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			return null;
		}

		public virtual void OnFiltersChanged()
		{
		}

		public virtual void DisableOverlay()
		{
		}

		public abstract string GetSoundName();

		protected bool InFilter(string layer, Dictionary<string, ToolParameterMenu.ToggleState> filter)
		{
			return (filter.ContainsKey(ToolParameterMenu.FILTERLAYERS.ALL) && filter[ToolParameterMenu.FILTERLAYERS.ALL] == ToolParameterMenu.ToggleState.On) || (filter.ContainsKey(layer) && filter[layer] == ToolParameterMenu.ToggleState.On);
		}

		public void RegisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister += OnSaveLoadRootRegistered;
			saveManager.onUnregister += OnSaveLoadRootUnregistered;
		}

		public void UnregisterSaveLoadListeners()
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			saveManager.onRegister -= OnSaveLoadRootRegistered;
			saveManager.onUnregister -= OnSaveLoadRootUnregistered;
		}

		protected virtual void OnSaveLoadRootRegistered(SaveLoadRoot root)
		{
		}

		protected virtual void OnSaveLoadRootUnregistered(SaveLoadRoot root)
		{
		}

		protected void ProcessExistingSaveLoadRoots()
		{
			foreach (KeyValuePair<Tag, List<SaveLoadRoot>> list in SaveLoader.Instance.saveManager.GetLists())
			{
				foreach (SaveLoadRoot item in list.Value)
				{
					OnSaveLoadRootRegistered(item);
				}
			}
		}

		protected static UniformGrid<T> PopulatePartition<T>(ICollection<Tag> tags) where T : IUniformGridObject
		{
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
			UniformGrid<T> uniformGrid = new UniformGrid<T>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			foreach (Tag tag in tags)
			{
				List<SaveLoadRoot> value = null;
				if (!lists.TryGetValue(tag, out value))
				{
					continue;
				}
				foreach (SaveLoadRoot item in value)
				{
					T component = item.GetComponent<T>();
					if (component != null)
					{
						uniformGrid.Add(component);
					}
				}
			}
			return uniformGrid;
		}

		protected static void ResetDisplayValues<T>(ICollection<T> targets) where T : MonoBehaviour
		{
			foreach (T target in targets)
			{
				if (!((UnityEngine.Object)target == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						ResetDisplayValues(component);
					}
				}
			}
		}

		protected static void ResetDisplayValues(KBatchedAnimController controller)
		{
			controller.SetLayer(0);
			controller.HighlightColour = Color.clear;
			controller.TintColour = Color.white;
			controller.SetLayer(controller.GetComponent<KPrefabID>().defaultLayer);
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, Vector2I min, Vector2I max, Action<T> on_removed = null) where T : KMonoBehaviour
		{
			ClearOutsideViewObjects(targets, min, max, null, delegate(T cmp)
			{
				if ((UnityEngine.Object)cmp != (UnityEngine.Object)null)
				{
					KBatchedAnimController component = cmp.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						ResetDisplayValues(component);
					}
					if (on_removed != null)
					{
						on_removed(cmp);
					}
				}
			});
			workingTargets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, Vector2I vis_min, Vector2I vis_max, ICollection<Tag> item_ids, Action<T> on_remove) where T : KMonoBehaviour
		{
			workingTargets.Clear();
			foreach (T target in targets)
			{
				if ((UnityEngine.Object)target == (UnityEngine.Object)null)
				{
					continue;
				}
				Vector2I vector2I = Grid.PosToXY(target.transform.GetPosition());
				if (!(vis_min <= vector2I) || !(vector2I <= vis_max) || target.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
				{
					workingTargets.Add(target);
					continue;
				}
				KPrefabID component = target.GetComponent<KPrefabID>();
				if (item_ids != null && !item_ids.Contains(component.PrefabTag) && target.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
				{
					workingTargets.Add(target);
				}
			}
			foreach (T workingTarget in workingTargets)
			{
				if (!((UnityEngine.Object)workingTarget == (UnityEngine.Object)null))
				{
					on_remove?.Invoke(workingTarget);
					targets.Remove(workingTarget);
				}
			}
			workingTargets.Clear();
		}

		protected static void RemoveOffscreenTargets<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null, Func<T, bool> special_clear_condition = null) where T : IUniformGridObject
		{
			ClearOutsideViewObjects(targets, working_targets, vis_min, vis_max, delegate(T cmp)
			{
				if (cmp != null && on_removed != null)
				{
					on_removed(cmp);
				}
			});
			if (special_clear_condition == null)
			{
				return;
			}
			working_targets.Clear();
			foreach (T target in targets)
			{
				if (special_clear_condition(target))
				{
					working_targets.Add(target);
				}
			}
			foreach (T working_target in working_targets)
			{
				if (working_target != null)
				{
					if (on_removed != null)
					{
						on_removed(working_target);
					}
					targets.Remove(working_target);
				}
			}
			working_targets.Clear();
		}

		protected static void ClearOutsideViewObjects<T>(ICollection<T> targets, ICollection<T> working_targets, Vector2I vis_min, Vector2I vis_max, Action<T> on_removed = null) where T : IUniformGridObject
		{
			working_targets.Clear();
			foreach (T target in targets)
			{
				if (target != null)
				{
					Vector2 vector = target.PosMin();
					Vector2 vector2 = target.PosMin();
					if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || (float)vis_max.x < vector.x || (float)vis_max.y < vector.y)
					{
						working_targets.Add(target);
					}
				}
			}
			foreach (T working_target in working_targets)
			{
				if (working_target != null)
				{
					on_removed?.Invoke(working_target);
					targets.Remove(working_target);
				}
			}
			working_targets.Clear();
		}

		protected static float GetDefaultDepth(KMonoBehaviour cmp)
		{
			BuildingComplete component = cmp.GetComponent<BuildingComplete>();
			if (component != null)
			{
				return Grid.GetLayerZ(component.Def.SceneLayer);
			}
			return Grid.GetLayerZ(Grid.SceneLayer.Creatures);
		}

		protected void UpdateHighlightTypeOverlay<T>(Vector2I min, Vector2I max, ICollection<T> targets, ICollection<Tag> item_ids, ColorHighlightCondition[] highlights, BringToFrontLayerSetting bringToFrontSetting, int layer) where T : KMonoBehaviour
		{
			foreach (T target in targets)
			{
				if ((UnityEngine.Object)target == (UnityEngine.Object)null)
				{
					continue;
				}
				Vector3 position = target.transform.GetPosition();
				int cell = Grid.PosToCell(position);
				if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell) || !(min <= position) || !(position <= max))
				{
					continue;
				}
				KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
				if (component == null)
				{
					continue;
				}
				int layer2 = 0;
				Color32 highlightColour = Color.clear;
				if (highlights != null)
				{
					foreach (ColorHighlightCondition colorHighlightCondition in highlights)
					{
						if (colorHighlightCondition.highlight_condition(target))
						{
							highlightColour = colorHighlightCondition.highlight_color(target);
							layer2 = layer;
							break;
						}
					}
				}
				switch (bringToFrontSetting)
				{
				case BringToFrontLayerSetting.Constant:
					component.SetLayer(layer);
					break;
				case BringToFrontLayerSetting.Conditional:
					component.SetLayer(layer2);
					break;
				}
				component.HighlightColour = highlightColour;
			}
		}

		protected void DisableHighlightTypeOverlay<T>(ICollection<T> targets) where T : KMonoBehaviour
		{
			Color32 highlightColour = Color.clear;
			foreach (T target in targets)
			{
				if (!((UnityEngine.Object)target == (UnityEngine.Object)null))
				{
					KBatchedAnimController component = target.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.HighlightColour = highlightColour;
						component.SetLayer(0);
					}
				}
			}
			targets.Clear();
		}

		protected void AddTargetIfVisible<T>(T instance, Vector2I vis_min, Vector2I vis_max, ICollection<T> targets, int layer, Action<T> on_added = null, Func<KMonoBehaviour, bool> should_add = null) where T : IUniformGridObject
		{
			if (instance.Equals(null))
			{
				return;
			}
			Vector2 vector = instance.PosMin();
			Vector2 vector2 = instance.PosMax();
			if (vector2.x < (float)vis_min.x || vector2.y < (float)vis_min.y || vector.x > (float)vis_max.x || vector.y > (float)vis_max.y || targets.Contains(instance))
			{
				return;
			}
			bool flag = false;
			for (int i = (int)vector.y; (float)i <= vector2.y; i++)
			{
				for (int j = (int)vector.x; (float)j <= vector2.x; j++)
				{
					int num = Grid.XYToCell(j, i);
					if ((Grid.IsValidCell(num) && Grid.Visible[num] > 20 && Grid.WorldIdx[num] == ClusterManager.Instance.activeWorldId) || !PropertyTextures.IsFogOfWarEnabled)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = true;
			KMonoBehaviour kMonoBehaviour = instance as KMonoBehaviour;
			if (kMonoBehaviour != null && should_add != null)
			{
				flag2 = should_add(kMonoBehaviour);
			}
			if (!flag2)
			{
				return;
			}
			if (kMonoBehaviour != null)
			{
				KBatchedAnimController component = kMonoBehaviour.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					component.SetLayer(layer);
				}
			}
			targets.Add(instance);
			on_added?.Invoke(instance);
		}
	}

	public class ModeUtil
	{
		public static float GetHighlightScale()
		{
			return Mathf.SmoothStep(0.5f, 1f, Mathf.Abs(Mathf.Sin(Time.unscaledTime * 4f)));
		}
	}

	public class Power : Mode
	{
		private struct UpdatePowerInfo
		{
			public KMonoBehaviour item;

			public LocText powerLabel;

			public LocText unitLabel;

			public Generator generator;

			public IEnergyConsumer consumer;

			public UpdatePowerInfo(KMonoBehaviour item, LocText power_label, LocText unit_label, Generator g, IEnergyConsumer c)
			{
				this.item = item;
				powerLabel = power_label;
				unitLabel = unit_label;
				generator = g;
				consumer = c;
			}
		}

		private struct UpdateBatteryInfo
		{
			public Battery battery;

			public BatteryUI ui;

			public UpdateBatteryInfo(Battery battery, BatteryUI ui)
			{
				this.battery = battery;
				this.ui = ui;
			}
		}

		public static readonly HashedString ID = "Power";

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private List<UpdatePowerInfo> updatePowerInfo = new List<UpdatePowerInfo>();

		private List<UpdateBatteryInfo> updateBatteryInfo = new List<UpdateBatteryInfo>();

		private Canvas powerLabelParent;

		private LocText powerLabelPrefab;

		private Vector3 powerLabelOffset;

		private BatteryUI batteryUIPrefab;

		private Vector3 batteryUIOffset;

		private Vector3 batteryUITransformerOffset;

		private Vector3 batteryUISmallTransformerOffset;

		private int freePowerLabelIdx = 0;

		private int freeBatteryUIIdx = 0;

		private List<LocText> powerLabels = new List<LocText>();

		private List<BatteryUI> batteryUIList = new List<BatteryUI>();

		private UniformGrid<SaveLoadRoot> partition;

		private List<SaveLoadRoot> queuedAdds = new List<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private HashSet<SaveLoadRoot> privateTargets = new HashSet<SaveLoadRoot>();

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Power";
		}

		public Power(Canvas powerLabelParent, LocText powerLabelPrefab, BatteryUI batteryUIPrefab, Vector3 powerLabelOffset, Vector3 batteryUIOffset, Vector3 batteryUITransformerOffset, Vector3 batteryUISmallTransformerOffset)
		{
			this.powerLabelParent = powerLabelParent;
			this.powerLabelPrefab = powerLabelPrefab;
			this.batteryUIPrefab = batteryUIPrefab;
			this.powerLabelOffset = powerLabelOffset;
			this.batteryUIOffset = batteryUIOffset;
			this.batteryUITransformerOffset = batteryUITransformerOffset;
			this.batteryUISmallTransformerOffset = batteryUISmallTransformerOffset;
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
		}

		public override void Enable()
		{
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(OverlayScreen.WireIDs);
			GridCompositor.Instance.ToggleMinor(on: true);
		}

		public override void Disable()
		{
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			privateTargets.Clear();
			queuedAdds.Clear();
			DisablePowerLabels();
			DisableBatteryUIs();
			GridCompositor.Instance.ToggleMinor(on: false);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (OverlayScreen.WireIDs.Contains(saveLoadTag))
			{
				partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				if (layerTargets.Contains(item))
				{
					layerTargets.Remove(item);
				}
				partition.Remove(item);
			}
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			using (new KProfiler.Region("UpdatePowerOverlay"))
			{
				IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
				foreach (SaveLoadRoot item in allIntersecting)
				{
					AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
				}
				connectedNetworks.Clear();
				float num = 1f;
				GameObject gameObject = null;
				if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
				{
					gameObject = SelectTool.Instance.hover.gameObject;
				}
				if (gameObject != null)
				{
					IBridgedNetworkItem component = gameObject.GetComponent<IBridgedNetworkItem>();
					if (component != null)
					{
						int networkCell = component.GetNetworkCell();
						visited.Clear();
						FindConnectedNetworks(networkCell, Game.Instance.electricalConduitSystem, connectedNetworks, visited);
						visited.Clear();
						num = ModeUtil.GetHighlightScale();
					}
				}
				CircuitManager circuitManager = Game.Instance.circuitManager;
				foreach (SaveLoadRoot layerTarget in layerTargets)
				{
					if (layerTarget == null)
					{
						continue;
					}
					IBridgedNetworkItem component2 = layerTarget.GetComponent<IBridgedNetworkItem>();
					if (component2 != null)
					{
						KMonoBehaviour kMonoBehaviour = component2 as KMonoBehaviour;
						KBatchedAnimController component3 = kMonoBehaviour.GetComponent<KBatchedAnimController>();
						int networkCell2 = component2.GetNetworkCell();
						UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(networkCell2);
						ushort circuitID = ((networkForCell != null) ? ((ushort)networkForCell.id) : ushort.MaxValue);
						float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitID);
						float maxSafeWattageForCircuit = circuitManager.GetMaxSafeWattageForCircuit(circuitID);
						maxSafeWattageForCircuit += POWER.FLOAT_FUDGE_FACTOR;
						float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
						Color32 tintColour = ((wattsUsedByCircuit <= 0f) ? GlobalAssets.Instance.colorSet.powerCircuitUnpowered : ((wattsUsedByCircuit > maxSafeWattageForCircuit) ? GlobalAssets.Instance.colorSet.powerCircuitOverloading : ((!(wattsNeededWhenActive > maxSafeWattageForCircuit) || !(maxSafeWattageForCircuit > 0f) || !(wattsUsedByCircuit / maxSafeWattageForCircuit >= 0.75f)) ? GlobalAssets.Instance.colorSet.powerCircuitSafe : GlobalAssets.Instance.colorSet.powerCircuitStraining)));
						if (connectedNetworks.Count > 0 && component2.IsConnectedToNetworks(connectedNetworks))
						{
							tintColour.r = (byte)((float)(int)tintColour.r * num);
							tintColour.g = (byte)((float)(int)tintColour.g * num);
							tintColour.b = (byte)((float)(int)tintColour.b * num);
						}
						component3.TintColour = tintColour;
					}
				}
			}
			queuedAdds.Clear();
			using (new KProfiler.Region("BatteryUI"))
			{
				foreach (Battery item2 in Components.Batteries.Items)
				{
					Vector2I vector2I = Grid.PosToXY(item2.transform.GetPosition());
					if (min <= vector2I && vector2I <= max && item2.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
					{
						SaveLoadRoot component4 = item2.GetComponent<SaveLoadRoot>();
						if (!privateTargets.Contains(component4))
						{
							AddBatteryUI(item2);
							queuedAdds.Add(component4);
						}
					}
				}
				foreach (Generator item3 in Components.Generators.Items)
				{
					Vector2I vector2I2 = Grid.PosToXY(item3.transform.GetPosition());
					if (!(min <= vector2I2) || !(vector2I2 <= max) || item3.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
					{
						continue;
					}
					SaveLoadRoot component5 = item3.GetComponent<SaveLoadRoot>();
					if (!privateTargets.Contains(component5))
					{
						privateTargets.Add(component5);
						if (item3.GetComponent<PowerTransformer>() == null)
						{
							AddPowerLabels(item3);
						}
					}
				}
				foreach (EnergyConsumer item4 in Components.EnergyConsumers.Items)
				{
					Vector2I vector2I3 = Grid.PosToXY(item4.transform.GetPosition());
					if (min <= vector2I3 && vector2I3 <= max && item4.GetMyWorldId() == ClusterManager.Instance.activeWorldId)
					{
						SaveLoadRoot component6 = item4.GetComponent<SaveLoadRoot>();
						if (!privateTargets.Contains(component6))
						{
							privateTargets.Add(component6);
							AddPowerLabels(item4);
						}
					}
				}
			}
			foreach (SaveLoadRoot queuedAdd in queuedAdds)
			{
				privateTargets.Add(queuedAdd);
			}
			queuedAdds.Clear();
			UpdatePowerLabels();
		}

		private LocText GetFreePowerLabel()
		{
			LocText locText = null;
			if (freePowerLabelIdx < powerLabels.Count)
			{
				locText = powerLabels[freePowerLabelIdx];
				freePowerLabelIdx++;
			}
			else
			{
				locText = Util.KInstantiateUI<LocText>(powerLabelPrefab.gameObject, powerLabelParent.transform.gameObject);
				powerLabels.Add(locText);
				freePowerLabelIdx++;
			}
			return locText;
		}

		private void UpdatePowerLabels()
		{
			foreach (UpdatePowerInfo item2 in updatePowerInfo)
			{
				KMonoBehaviour item = item2.item;
				LocText powerLabel = item2.powerLabel;
				LocText unitLabel = item2.unitLabel;
				Generator generator = item2.generator;
				IEnergyConsumer consumer = item2.consumer;
				if (item2.item == null || item2.item.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
				{
					powerLabel.gameObject.SetActive(value: false);
					continue;
				}
				powerLabel.gameObject.SetActive(value: true);
				if (generator != null && consumer == null)
				{
					int num = int.MaxValue;
					ManualGenerator component = generator.GetComponent<ManualGenerator>();
					if (component == null)
					{
						generator.GetComponent<Operational>();
						num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
					}
					else
					{
						num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
					}
					powerLabel.text = ((num != 0) ? ("+" + num) : num.ToString());
					BuildingEnabledButton component2 = item.GetComponent<BuildingEnabledButton>();
					Color color3 = (unitLabel.color = (powerLabel.color = ((component2 != null && !component2.IsEnabled) ? GlobalAssets.Instance.colorSet.powerBuildingDisabled : GlobalAssets.Instance.colorSet.powerGenerator)));
					BuildingCellVisualizer component3 = generator.GetComponent<BuildingCellVisualizer>();
					if (component3 != null)
					{
						Image outputIcon = component3.GetOutputIcon();
						if (outputIcon != null)
						{
							outputIcon.color = color3;
						}
					}
				}
				if (consumer != null)
				{
					BuildingEnabledButton component4 = item.GetComponent<BuildingEnabledButton>();
					Color color4 = ((component4 != null && !component4.IsEnabled) ? GlobalAssets.Instance.colorSet.powerBuildingDisabled : GlobalAssets.Instance.colorSet.powerConsumer);
					int num2 = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
					string text = num2.ToString();
					powerLabel.text = ((num2 != 0) ? ("-" + text) : text);
					powerLabel.color = color4;
					unitLabel.color = color4;
					Image inputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
					if (inputIcon != null)
					{
						inputIcon.color = color4;
					}
				}
			}
			foreach (UpdateBatteryInfo item3 in updateBatteryInfo)
			{
				item3.ui.SetContent(item3.battery);
			}
		}

		private void AddPowerLabels(KMonoBehaviour item)
		{
			if (item.gameObject.GetMyWorldId() != ClusterManager.Instance.activeWorldId)
			{
				return;
			}
			IEnergyConsumer componentInChildren = item.gameObject.GetComponentInChildren<IEnergyConsumer>();
			Generator componentInChildren2 = item.gameObject.GetComponentInChildren<Generator>();
			if (componentInChildren == null && !(componentInChildren2 != null))
			{
				return;
			}
			float num = -10f;
			if (componentInChildren2 != null)
			{
				LocText freePowerLabel = GetFreePowerLabel();
				freePowerLabel.gameObject.SetActive(value: true);
				freePowerLabel.gameObject.name = item.gameObject.name + "power label";
				LocText component = freePowerLabel.transform.GetChild(0).GetComponent<LocText>();
				component.gameObject.SetActive(value: true);
				freePowerLabel.enabled = true;
				component.enabled = true;
				Vector3 a = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel.rectTransform.SetPosition(a + powerLabelOffset + Vector3.up * (num * 0.02f));
				if (componentInChildren != null && componentInChildren.PowerCell == componentInChildren2.PowerCell)
				{
					num -= 15f;
				}
				SetToolTip(freePowerLabel, UI.OVERLAYS.POWER.WATTS_GENERATED);
				updatePowerInfo.Add(new UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, null));
			}
			if (componentInChildren != null && componentInChildren.GetType() != typeof(Battery))
			{
				LocText freePowerLabel2 = GetFreePowerLabel();
				LocText component2 = freePowerLabel2.transform.GetChild(0).GetComponent<LocText>();
				freePowerLabel2.gameObject.SetActive(value: true);
				component2.gameObject.SetActive(value: true);
				freePowerLabel2.gameObject.name = item.gameObject.name + "power label";
				freePowerLabel2.enabled = true;
				component2.enabled = true;
				Vector3 a2 = Grid.CellToPos(componentInChildren.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel2.rectTransform.SetPosition(a2 + powerLabelOffset + Vector3.up * (num * 0.02f));
				SetToolTip(freePowerLabel2, UI.OVERLAYS.POWER.WATTS_CONSUMED);
				updatePowerInfo.Add(new UpdatePowerInfo(item, freePowerLabel2, component2, null, componentInChildren));
			}
		}

		private void DisablePowerLabels()
		{
			freePowerLabelIdx = 0;
			foreach (LocText powerLabel in powerLabels)
			{
				powerLabel.gameObject.SetActive(value: false);
			}
			updatePowerInfo.Clear();
		}

		private void AddBatteryUI(Battery bat)
		{
			BatteryUI freeBatteryUI = GetFreeBatteryUI();
			freeBatteryUI.SetContent(bat);
			Vector3 b = Grid.CellToPos(bat.PowerCell, 0.5f, 0f, 0f);
			bool flag = bat.powerTransformer != null;
			float num = 1f;
			Rotatable component = bat.GetComponent<Rotatable>();
			if (component != null && component.GetVisualizerFlipX())
			{
				num = -1f;
			}
			Vector3 b2 = batteryUIOffset;
			if (flag)
			{
				int widthInCells = bat.GetComponent<Building>().Def.WidthInCells;
				b2 = ((widthInCells == 2) ? batteryUISmallTransformerOffset : batteryUITransformerOffset);
			}
			b2.x *= num;
			freeBatteryUI.GetComponent<RectTransform>().SetPosition(Vector3.up + b + b2);
			updateBatteryInfo.Add(new UpdateBatteryInfo(bat, freeBatteryUI));
		}

		private void SetToolTip(LocText label, string text)
		{
			ToolTip component = label.GetComponent<ToolTip>();
			if (component != null)
			{
				component.toolTip = text;
			}
		}

		private void DisableBatteryUIs()
		{
			freeBatteryUIIdx = 0;
			foreach (BatteryUI batteryUI in batteryUIList)
			{
				batteryUI.gameObject.SetActive(value: false);
			}
			updateBatteryInfo.Clear();
		}

		private BatteryUI GetFreeBatteryUI()
		{
			BatteryUI batteryUI = null;
			if (freeBatteryUIIdx < batteryUIList.Count)
			{
				batteryUI = batteryUIList[freeBatteryUIIdx];
				batteryUI.gameObject.SetActive(value: true);
				freeBatteryUIIdx++;
			}
			else
			{
				batteryUI = Util.KInstantiateUI<BatteryUI>(batteryUIPrefab.gameObject, powerLabelParent.transform.gameObject);
				batteryUIList.Add(batteryUI);
				freeBatteryUIIdx++;
			}
			return batteryUI;
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell != null)
			{
				networks.Add(networkForCell);
				UtilityConnections connections = mgr.GetConnections(cell, is_physical_building: false);
				if ((connections & UtilityConnections.Right) != 0)
				{
					FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Left) != 0)
				{
					FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Up) != 0)
				{
					FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
				}
				if ((connections & UtilityConnections.Down) != 0)
				{
					FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
				}
			}
		}
	}

	public class Radiation : Mode
	{
		public static readonly HashedString ID = "Radiation";

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Radiation";
		}
	}

	public class SolidConveyor : Mode
	{
		public static readonly HashedString ID = "SolidConveyor";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs = OverlayScreen.SolidConveyorIDs;

		private Color32 tint_color = new Color32(201, 201, 201, 0);

		private HashSet<UtilityNetwork> connectedNetworks = new HashSet<UtilityNetwork>();

		private List<int> visited = new List<int>();

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "LiquidVent";
		}

		public SolidConveyor()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			partition = Mode.PopulatePartition<SaveLoadRoot>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Enable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				if (layerTargets.Contains(item))
				{
					layerTargets.Remove(item);
				}
				partition.Remove(item);
			}
		}

		public override void Disable()
		{
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Disable();
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (SaveLoadRoot item in allIntersecting)
			{
				AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
			}
			GameObject gameObject = null;
			if (SelectTool.Instance != null && SelectTool.Instance.hover != null)
			{
				gameObject = SelectTool.Instance.hover.gameObject;
			}
			connectedNetworks.Clear();
			float num = 1f;
			if (gameObject != null)
			{
				SolidConduit component = gameObject.GetComponent<SolidConduit>();
				if (component != null)
				{
					int cell = Grid.PosToCell(component);
					UtilityNetworkManager<FlowUtilityNetwork, SolidConduit> solidConduitSystem = Game.Instance.solidConduitSystem;
					visited.Clear();
					FindConnectedNetworks(cell, solidConduitSystem, connectedNetworks, visited);
					visited.Clear();
					num = ModeUtil.GetHighlightScale();
				}
			}
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (layerTarget == null)
				{
					continue;
				}
				Color32 tintColour = tint_color;
				SolidConduit component2 = layerTarget.GetComponent<SolidConduit>();
				if (component2 != null)
				{
					if (connectedNetworks.Count > 0 && IsConnectedToNetworks(component2, connectedNetworks))
					{
						tintColour.r = (byte)((float)(int)tintColour.r * num);
						tintColour.g = (byte)((float)(int)tintColour.g * num);
						tintColour.b = (byte)((float)(int)tintColour.b * num);
					}
					KBatchedAnimController component3 = layerTarget.GetComponent<KBatchedAnimController>();
					component3.TintColour = tintColour;
				}
			}
		}

		public bool IsConnectedToNetworks(SolidConduit conduit, ICollection<UtilityNetwork> networks)
		{
			UtilityNetwork network = conduit.GetNetwork();
			return networks.Contains(network);
		}

		private void FindConnectedNetworks(int cell, IUtilityNetworkMgr mgr, ICollection<UtilityNetwork> networks, List<int> visited)
		{
			if (visited.Contains(cell))
			{
				return;
			}
			visited.Add(cell);
			UtilityNetwork networkForCell = mgr.GetNetworkForCell(cell);
			if (networkForCell == null)
			{
				return;
			}
			networks.Add(networkForCell);
			UtilityConnections connections = mgr.GetConnections(cell, is_physical_building: false);
			if ((connections & UtilityConnections.Right) != 0)
			{
				FindConnectedNetworks(Grid.CellRight(cell), mgr, networks, visited);
			}
			if ((connections & UtilityConnections.Left) != 0)
			{
				FindConnectedNetworks(Grid.CellLeft(cell), mgr, networks, visited);
			}
			if ((connections & UtilityConnections.Up) != 0)
			{
				FindConnectedNetworks(Grid.CellAbove(cell), mgr, networks, visited);
			}
			if ((connections & UtilityConnections.Down) != 0)
			{
				FindConnectedNetworks(Grid.CellBelow(cell), mgr, networks, visited);
			}
			object endpoint = mgr.GetEndpoint(cell);
			if (endpoint == null)
			{
				return;
			}
			FlowUtilityNetwork.NetworkItem networkItem = endpoint as FlowUtilityNetwork.NetworkItem;
			if (networkItem != null)
			{
				GameObject gameObject = networkItem.GameObject;
				if (gameObject != null)
				{
					gameObject.GetComponent<IBridgedNetworkItem>()?.AddNetworks(networks);
				}
			}
		}
	}

	public class Sound : Mode
	{
		public static readonly HashedString ID = "Sound";

		private UniformGrid<NoisePolluter> partition;

		private HashSet<NoisePolluter> layerTargets = new HashSet<NoisePolluter>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition(delegate(KMonoBehaviour np)
			{
				Color black = Color.black;
				Color b = Color.black;
				float t = 0.8f;
				if (np != null)
				{
					float num = 0f;
					int cell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
					num = (np as NoisePolluter).GetNoiseForCell(cell);
					if (num < 36f)
					{
						t = 1f;
						b = new Color(0.4f, 0.4f, 0.4f);
					}
				}
				return Color.Lerp(black, b, t);
			}, delegate(KMonoBehaviour np)
			{
				List<GameObject> highlightedObjects = SelectToolHoverTextCard.highlightedObjects;
				bool result = false;
				for (int i = 0; i < highlightedObjects.Count; i++)
				{
					if (highlightedObjects[i] != null && highlightedObjects[i] == np.gameObject)
					{
						result = true;
						break;
					}
				}
				return result;
			})
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Sound";
		}

		public Sound()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			targetIDs.UnionWith(prefabTagsWithComponent);
		}

		public override void Enable()
		{
			RegisterSaveLoadListeners();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<NoisePolluter>();
			targetIDs.UnionWith(prefabTagsWithComponent);
			partition = Mode.PopulatePartition<NoisePolluter>(targetIDs);
			Camera.main.cullingMask |= cameraLayerMask;
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (NoisePolluter item in allIntersecting)
			{
				AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Conditional, targetLayer);
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			Tag saveLoadTag = item.GetComponent<KPrefabID>().GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				partition.Add(component);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				NoisePolluter component = item.GetComponent<NoisePolluter>();
				if (layerTargets.Contains(component))
				{
					layerTargets.Remove(component);
				}
				partition.Remove(component);
			}
		}

		public override void Disable()
		{
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			UnregisterSaveLoadListeners();
			partition.Clear();
			layerTargets.Clear();
		}
	}

	public class Suit : Mode
	{
		public static readonly HashedString ID = "Suit";

		private UniformGrid<SaveLoadRoot> partition;

		private HashSet<SaveLoadRoot> layerTargets = new HashSet<SaveLoadRoot>();

		private ICollection<Tag> targetIDs;

		private List<GameObject> uiList = new List<GameObject>();

		private int freeUiIdx;

		private int targetLayer;

		private int cameraLayerMask;

		private int selectionMask;

		private Canvas uiParent;

		private GameObject overlayPrefab;

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "SuitRequired";
		}

		public Suit(Canvas ui_parent, GameObject overlay_prefab)
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			selectionMask = cameraLayerMask;
			targetIDs = OverlayScreen.SuitIDs;
			uiParent = ui_parent;
			overlayPrefab = overlay_prefab;
		}

		public override void Enable()
		{
			partition = new UniformGrid<SaveLoadRoot>(Grid.WidthInCells, Grid.HeightInCells, 8, 8);
			ProcessExistingSaveLoadRoots();
			RegisterSaveLoadListeners();
			Camera.main.cullingMask |= cameraLayerMask;
			SelectTool.Instance.SetLayerMask(selectionMask);
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Enable();
		}

		public override void Disable()
		{
			UnregisterSaveLoadListeners();
			Mode.ResetDisplayValues(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			SelectTool.Instance.ClearLayerMask();
			partition.Clear();
			partition = null;
			layerTargets.Clear();
			for (int i = 0; i < uiList.Count; i++)
			{
				uiList[i].SetActive(value: false);
			}
			GridCompositor.Instance.ToggleMinor(on: false);
			base.Disable();
		}

		protected override void OnSaveLoadRootRegistered(SaveLoadRoot item)
		{
			KPrefabID component = item.GetComponent<KPrefabID>();
			Tag saveLoadTag = component.GetSaveLoadTag();
			if (targetIDs.Contains(saveLoadTag))
			{
				partition.Add(item);
			}
		}

		protected override void OnSaveLoadRootUnregistered(SaveLoadRoot item)
		{
			if (!(item == null) && !(item.gameObject == null))
			{
				if (layerTargets.Contains(item))
				{
					layerTargets.Remove(item);
				}
				partition.Remove(item);
			}
		}

		private GameObject GetFreeUI()
		{
			GameObject gameObject = null;
			if (freeUiIdx >= uiList.Count)
			{
				gameObject = Util.KInstantiateUI(overlayPrefab, uiParent.transform.gameObject);
				uiList.Add(gameObject);
			}
			else
			{
				gameObject = uiList[freeUiIdx++];
			}
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(value: true);
			}
			return gameObject;
		}

		public override void Update()
		{
			freeUiIdx = 0;
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			IEnumerable allIntersecting = partition.GetAllIntersecting(new Vector2(min.x, min.y), new Vector2(max.x, max.y));
			foreach (SaveLoadRoot item in allIntersecting)
			{
				AddTargetIfVisible(item, min, max, layerTargets, targetLayer);
			}
			foreach (SaveLoadRoot layerTarget in layerTargets)
			{
				if (layerTarget == null)
				{
					continue;
				}
				KBatchedAnimController component = layerTarget.GetComponent<KBatchedAnimController>();
				component.TintColour = Color.white;
				bool flag = false;
				if (layerTarget.GetComponent<KPrefabID>().HasTag(GameTags.Suit))
				{
					flag = true;
				}
				else
				{
					SuitLocker component2 = layerTarget.GetComponent<SuitLocker>();
					if (component2 != null)
					{
						flag = component2.GetStoredOutfit() != null;
					}
				}
				if (flag)
				{
					GameObject freeUI = GetFreeUI();
					freeUI.GetComponent<RectTransform>().SetPosition(layerTarget.transform.GetPosition());
				}
			}
			for (int i = freeUiIdx; i < uiList.Count; i++)
			{
				if (uiList[i].activeSelf)
				{
					uiList[i].SetActive(value: false);
				}
			}
		}
	}

	public class Temperature : Mode
	{
		public static readonly HashedString ID = "Temperature";

		public List<LegendEntry> temperatureLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.MAXHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(227f / 255f, 7f / 51f, 11f / 85f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(251f / 255f, 83f / 255f, 16f / 51f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 169f / 255f, 12f / 85f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(239f / 255f, 1f, 0f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.TEMPERATE, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(59f / 255f, 254f / 255f, 74f / 255f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.COLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(31f / 255f, 161f / 255f, 1f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYCOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(43f / 255f, 203f / 255f, 1f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(128f / 255f, 254f / 255f, 0.9411765f))
		};

		public List<LegendEntry> heatFlowLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.HEATFLOW.HEATING, UI.OVERLAYS.HEATFLOW.TOOLTIPS.HEATING, new Color(232f / 255f, 22f / 85f, 38f / 255f)),
			new LegendEntry(UI.OVERLAYS.HEATFLOW.NEUTRAL, UI.OVERLAYS.HEATFLOW.TOOLTIPS.NEUTRAL, new Color(79f / 255f, 79f / 255f, 79f / 255f)),
			new LegendEntry(UI.OVERLAYS.HEATFLOW.COOLING, UI.OVERLAYS.HEATFLOW.TOOLTIPS.COOLING, new Color(64f / 255f, 161f / 255f, 77f / 85f))
		};

		public List<LegendEntry> expandedTemperatureLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.MAXHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(227f / 255f, 7f / 51f, 11f / 85f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMEHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(251f / 255f, 83f / 255f, 16f / 51f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYHOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(1f, 169f / 255f, 12f / 85f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.HOT, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(239f / 255f, 1f, 0f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.TEMPERATE, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(59f / 255f, 254f / 255f, 74f / 255f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.COLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(31f / 255f, 161f / 255f, 1f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.VERYCOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(43f / 255f, 203f / 255f, 1f)),
			new LegendEntry(UI.OVERLAYS.TEMPERATURE.EXTREMECOLD, UI.OVERLAYS.TEMPERATURE.TOOLTIPS.TEMPERATURE, new Color(128f / 255f, 254f / 255f, 0.9411765f))
		};

		public List<LegendEntry> stateChangeLegend = new List<LegendEntry>
		{
			new LegendEntry(UI.OVERLAYS.STATECHANGE.HIGHPOINT, UI.OVERLAYS.STATECHANGE.TOOLTIPS.HIGHPOINT, new Color(227f / 255f, 7f / 51f, 11f / 85f)),
			new LegendEntry(UI.OVERLAYS.STATECHANGE.STABLE, UI.OVERLAYS.STATECHANGE.TOOLTIPS.STABLE, new Color(59f / 255f, 254f / 255f, 74f / 255f)),
			new LegendEntry(UI.OVERLAYS.STATECHANGE.LOWPOINT, UI.OVERLAYS.STATECHANGE.TOOLTIPS.LOWPOINT, new Color(128f / 255f, 254f / 255f, 0.9411765f))
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "Temperature";
		}

		public Temperature()
		{
			legendFilters = CreateDefaultFilters();
		}

		public override void Enable()
		{
			base.Enable();
			int num = SimDebugView.Instance.temperatureThresholds.Length - 1;
			for (int i = 0; i < temperatureLegend.Count; i++)
			{
				temperatureLegend[i].colour = GlobalAssets.Instance.colorSet.GetColorByName(SimDebugView.Instance.temperatureThresholds[num - i].colorName);
				temperatureLegend[i].desc_arg = GameUtil.GetFormattedTemperature(SimDebugView.Instance.temperatureThresholds[num - i].value);
			}
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			Dictionary<string, ToolParameterMenu.ToggleState> dictionary = new Dictionary<string, ToolParameterMenu.ToggleState>();
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE, ToolParameterMenu.ToggleState.On);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.HEATFLOW, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.STATECHANGE, ToolParameterMenu.ToggleState.Off);
			return dictionary;
		}

		public override List<LegendEntry> GetCustomLegendData()
		{
			return Game.Instance.temperatureOverlayMode switch
			{
				Game.TemperatureOverlayModes.AbsoluteTemperature => temperatureLegend, 
				Game.TemperatureOverlayModes.HeatFlow => heatFlowLegend, 
				Game.TemperatureOverlayModes.AdaptiveTemperature => expandedTemperatureLegend, 
				Game.TemperatureOverlayModes.StateChange => stateChangeLegend, 
				_ => temperatureLegend, 
			};
		}

		public override void OnFiltersChanged()
		{
			if (InFilter(ToolParameterMenu.FILTERLAYERS.HEATFLOW, legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.HeatFlow;
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.ABSOLUTETEMPERATURE, legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AbsoluteTemperature;
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.ADAPTIVETEMPERATURE, legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.AdaptiveTemperature;
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.STATECHANGE, legendFilters))
			{
				Game.Instance.temperatureOverlayMode = Game.TemperatureOverlayModes.StateChange;
			}
			switch (Game.Instance.temperatureOverlayMode)
			{
			case Game.TemperatureOverlayModes.AbsoluteTemperature:
				Infrared.Instance.SetMode(Infrared.Mode.Infrared);
				CameraController.Instance.ToggleColouredOverlayView(enabled: true);
				break;
			case Game.TemperatureOverlayModes.AdaptiveTemperature:
				Infrared.Instance.SetMode(Infrared.Mode.Infrared);
				CameraController.Instance.ToggleColouredOverlayView(enabled: true);
				break;
			case Game.TemperatureOverlayModes.HeatFlow:
				Infrared.Instance.SetMode(Infrared.Mode.Disabled);
				CameraController.Instance.ToggleColouredOverlayView(enabled: false);
				break;
			case Game.TemperatureOverlayModes.StateChange:
				Infrared.Instance.SetMode(Infrared.Mode.Disabled);
				CameraController.Instance.ToggleColouredOverlayView(enabled: false);
				break;
			}
		}

		public override void Disable()
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			CameraController.Instance.ToggleColouredOverlayView(enabled: false);
			base.Disable();
		}
	}

	public class TileMode : Mode
	{
		public static readonly HashedString ID = "TileMode";

		private HashSet<PrimaryElement> layerTargets = new HashSet<PrimaryElement>();

		private HashSet<Tag> targetIDs = new HashSet<Tag>();

		private int targetLayer;

		private int cameraLayerMask;

		private ColorHighlightCondition[] highlightConditions = new ColorHighlightCondition[1]
		{
			new ColorHighlightCondition(delegate(KMonoBehaviour primary_element)
			{
				Color result = Color.black;
				if (primary_element != null)
				{
					Element element = (primary_element as PrimaryElement).Element;
					result = element.substance.uiColour;
				}
				return result;
			}, (KMonoBehaviour primary_element) => primary_element.gameObject.GetComponent<KBatchedAnimController>().IsVisible())
		};

		public override HashedString ViewMode()
		{
			return ID;
		}

		public override string GetSoundName()
		{
			return "SuitRequired";
		}

		public TileMode()
		{
			targetLayer = LayerMask.NameToLayer("MaskedOverlay");
			cameraLayerMask = LayerMask.GetMask("MaskedOverlay", "MaskedOverlayBG");
			legendFilters = CreateDefaultFilters();
		}

		public override void Enable()
		{
			base.Enable();
			List<Tag> prefabTagsWithComponent = Assets.GetPrefabTagsWithComponent<PrimaryElement>();
			targetIDs.UnionWith(prefabTagsWithComponent);
			Camera.main.cullingMask |= cameraLayerMask;
			int defaultLayerMask = SelectTool.Instance.GetDefaultLayerMask();
			int mask = LayerMask.GetMask("MaskedOverlay");
			SelectTool.Instance.SetLayerMask(defaultLayerMask | mask);
		}

		public override void Update()
		{
			Grid.GetVisibleExtents(out var min, out var max);
			Mode.RemoveOffscreenTargets(layerTargets, min, max);
			int height = max.y - min.y;
			int width = max.x - min.x;
			Extents extents = new Extents(min.x, min.y, width, height);
			List<ScenePartitionerEntry> list = new List<ScenePartitionerEntry>();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, list);
			foreach (ScenePartitionerEntry item in list)
			{
				Pickupable pickupable = (Pickupable)item.obj;
				PrimaryElement component = pickupable.gameObject.GetComponent<PrimaryElement>();
				if (component != null)
				{
					TryAddObject(component, min, max);
				}
			}
			list.Clear();
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, list);
			foreach (ScenePartitionerEntry item2 in list)
			{
				BuildingComplete buildingComplete = (BuildingComplete)item2.obj;
				PrimaryElement component2 = buildingComplete.gameObject.GetComponent<PrimaryElement>();
				if (component2 != null && buildingComplete.gameObject.layer == 0)
				{
					TryAddObject(component2, min, max);
				}
			}
			UpdateHighlightTypeOverlay(min, max, layerTargets, targetIDs, highlightConditions, BringToFrontLayerSetting.Conditional, targetLayer);
		}

		private void TryAddObject(PrimaryElement pe, Vector2I min, Vector2I max)
		{
			Element element = pe.Element;
			foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tileOverlayFilter))
				{
					AddTargetIfVisible(pe, min, max, layerTargets, targetLayer);
					break;
				}
			}
		}

		public override void Disable()
		{
			base.Disable();
			DisableHighlightTypeOverlay(layerTargets);
			Camera.main.cullingMask &= ~cameraLayerMask;
			layerTargets.Clear();
			SelectTool.Instance.ClearLayerMask();
		}

		public override Dictionary<string, ToolParameterMenu.ToggleState> CreateDefaultFilters()
		{
			Dictionary<string, ToolParameterMenu.ToggleState> dictionary = new Dictionary<string, ToolParameterMenu.ToggleState>();
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.METAL, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.BUILDABLE, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.FILTER, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.ORGANICS, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.FARMABLE, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.GAS, ToolParameterMenu.ToggleState.Off);
			dictionary.Add(ToolParameterMenu.FILTERLAYERS.LIQUID, ToolParameterMenu.ToggleState.Off);
			return dictionary;
		}

		public override void OnFiltersChanged()
		{
			Game.Instance.tileOverlayFilters.Clear();
			if (InFilter(ToolParameterMenu.FILTERLAYERS.METAL, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Metal);
				Game.Instance.tileOverlayFilters.Add(GameTags.RefinedMetal);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.BUILDABLE, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.BuildableRaw);
				Game.Instance.tileOverlayFilters.Add(GameTags.BuildableProcessed);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.FILTER, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Filter);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.LIQUIFIABLE, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Liquifiable);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.LIQUID, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Liquid);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.CONSUMABLEORE, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.ConsumableOre);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.ORGANICS, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Organics);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.FARMABLE, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Farmable);
				Game.Instance.tileOverlayFilters.Add(GameTags.Agriculture);
			}
			if (InFilter(ToolParameterMenu.FILTERLAYERS.GAS, legendFilters))
			{
				Game.Instance.tileOverlayFilters.Add(GameTags.Breathable);
				Game.Instance.tileOverlayFilters.Add(GameTags.Unbreathable);
			}
			DisableHighlightTypeOverlay(layerTargets);
			layerTargets.Clear();
			Game.Instance.ForceOverlayUpdate();
		}
	}
}
