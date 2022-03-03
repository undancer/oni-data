using System.Collections.Generic;
using UnityEngine;

public class ReorderableBuilding : KMonoBehaviour
{
	public enum MoveSource
	{
		Push,
		Pull
	}

	public string templateBuildingID = "UnconstructedRocketModule";

	private bool cancelShield;

	private bool reorderingAnimUnderway;

	private KBatchedAnimController animController;

	public List<SelectModuleCondition> buildConditions = new List<SelectModuleCondition>();

	private KBatchedAnimController reorderArmController;

	private KAnimLink m_animLink;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	private string reorderSound = "RocketModuleSwitchingArm_moving_LP";

	private static List<ReorderableBuilding> toBeRemoved = new List<ReorderableBuilding>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		animController = GetComponent<KBatchedAnimController>();
		Subscribe(2127324410, OnCancel);
		GameObject gameObject = new GameObject();
		gameObject.name = "ReorderArm";
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.SetLocalPosition(Vector3.up * Grid.CellSizeInMeters * ((float)GetComponent<Building>().Def.HeightInCells / 2f - 0.5f));
		gameObject.transform.SetPosition(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.BuildingBack)));
		gameObject.SetActive(value: false);
		reorderArmController = gameObject.AddComponent<KBatchedAnimController>();
		reorderArmController.AnimFiles = new KAnimFile[1] { Assets.GetAnim("rocket_module_switching_arm_kanim") };
		reorderArmController.initialAnim = "off";
		gameObject.SetActive(value: true);
		int cell = Grid.PosToCell(gameObject);
		ShowReorderArm(Grid.IsValidCell(cell));
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			LaunchPad currentPad = component.CraftInterface.CurrentPad;
			if (currentPad != null)
			{
				m_animLink = new KAnimLink(currentPad.GetComponent<KAnimControllerBase>(), reorderArmController);
			}
		}
		if (m_animLink == null)
		{
			m_animLink = new KAnimLink(GetComponent<KAnimControllerBase>(), reorderArmController);
		}
	}

	private void OnCancel(object data)
	{
		if (GetComponent<BuildingUnderConstruction>() != null && !cancelShield && !toBeRemoved.Contains(this))
		{
			toBeRemoved.Add(this);
		}
	}

	public GameObject AddModule(BuildingDef def, IList<Tag> buildMaterials)
	{
		if (Assets.GetPrefab(GetComponent<KPrefabID>().PrefabID()).GetComponent<ReorderableBuilding>().buildConditions.Find((SelectModuleCondition match) => match is TopOnly) != null || def.BuildingComplete.GetComponent<ReorderableBuilding>().buildConditions.Find((SelectModuleCondition match) => match is EngineOnBottom) != null)
		{
			return AddModuleBelow(def, buildMaterials);
		}
		return AddModuleAbove(def, buildMaterials);
	}

	private GameObject AddModuleAbove(BuildingDef def, IList<Tag> buildMaterials)
	{
		BuildingAttachPoint component = GetComponent<BuildingAttachPoint>();
		if (component == null)
		{
			return null;
		}
		BuildingAttachPoint.HardPoint hardPoint = component.points[0];
		int cell = Grid.OffsetCell(Grid.PosToCell(base.gameObject), hardPoint.position);
		int heightInCells = def.HeightInCells;
		if (hardPoint.attachedBuilding != null)
		{
			if (!hardPoint.attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(heightInCells))
			{
				return null;
			}
			hardPoint.attachedBuilding.GetComponent<ReorderableBuilding>().MoveVertical(heightInCells);
		}
		return AddModuleCommon(def, buildMaterials, cell);
	}

	private GameObject AddModuleBelow(BuildingDef def, IList<Tag> buildMaterials)
	{
		int cell = Grid.PosToCell(base.gameObject);
		int heightInCells = def.HeightInCells;
		if (!CanMoveVertically(heightInCells))
		{
			return null;
		}
		MoveVertical(heightInCells);
		return AddModuleCommon(def, buildMaterials, cell);
	}

	private GameObject AddModuleCommon(BuildingDef def, IList<Tag> buildMaterials, int cell)
	{
		GameObject result = ((!DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild)) ? def.TryPlace(null, Grid.CellToPosCBC(cell, def.SceneLayer), Orientation.Neutral, buildMaterials) : def.Build(cell, Orientation.Neutral, null, buildMaterials, 273.15f, playsound: true, GameClock.Instance.GetTime()));
		RebuildNetworks();
		RocketSpecificPostAdd(result, cell);
		return result;
	}

	private void RocketSpecificPostAdd(GameObject obj, int cell)
	{
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		RocketModuleCluster component2 = obj.GetComponent<RocketModuleCluster>();
		if (component != null && component2 != null)
		{
			component.CraftInterface.AddModule(component2);
		}
	}

	public void RemoveModule()
	{
		BuildingAttachPoint component = GetComponent<BuildingAttachPoint>();
		AttachableBuilding attachableBuilding = null;
		if (component != null)
		{
			attachableBuilding = component.points[0].attachedBuilding;
		}
		int heightInCells = GetComponent<Building>().Def.HeightInCells;
		if (GetComponent<Deconstructable>() != null)
		{
			GetComponent<Deconstructable>().CompleteWork(null);
		}
		if (GetComponent<BuildingUnderConstruction>() != null)
		{
			this.DeleteObject();
		}
		Building component2 = GetComponent<Building>();
		component2.Def.UnmarkArea(Grid.PosToCell(this), component2.Orientation, component2.Def.ObjectLayer, base.gameObject);
		if (attachableBuilding != null)
		{
			attachableBuilding.GetComponent<ReorderableBuilding>().MoveVertical(-heightInCells);
		}
	}

	public void LateUpdate()
	{
		cancelShield = false;
		ProcessToBeRemoved();
		if (!reorderingAnimUnderway)
		{
			return;
		}
		float num = 10f;
		if (Mathf.Abs(animController.Offset.y) < Time.unscaledDeltaTime * num)
		{
			animController.Offset = new Vector3(animController.Offset.x, 0f, animController.Offset.z);
			reorderingAnimUnderway = false;
			string text = GetComponent<Building>().Def.WidthInCells + "x" + GetComponent<Building>().Def.HeightInCells + "_ungrab";
			if (!reorderArmController.HasAnimation(text))
			{
				text = "3x3_ungrab";
			}
			reorderArmController.Play(text);
			reorderArmController.Queue("off");
			loopingSounds.StopSound(GlobalAssets.GetSound(reorderSound));
		}
		else if (animController.Offset.y > 0f)
		{
			animController.Offset = new Vector3(animController.Offset.x, animController.Offset.y - Time.unscaledDeltaTime * num, animController.Offset.z);
		}
		else if (animController.Offset.y < 0f)
		{
			animController.Offset = new Vector3(animController.Offset.x, animController.Offset.y + Time.unscaledDeltaTime * num, animController.Offset.z);
		}
		reorderArmController.Offset = animController.Offset;
	}

	private static void ProcessToBeRemoved()
	{
		if (toBeRemoved.Count > 0)
		{
			toBeRemoved.Sort((ReorderableBuilding a, ReorderableBuilding b) => (a.transform.position.y >= b.transform.position.y) ? 1 : (-1));
			for (int i = 0; i < toBeRemoved.Count; i++)
			{
				toBeRemoved[i].RemoveModule();
			}
			toBeRemoved.Clear();
		}
	}

	public void MoveVertical(int amount)
	{
		if (amount == 0)
		{
			return;
		}
		cancelShield = true;
		List<GameObject> buildings = new List<GameObject>();
		buildings.Add(base.gameObject);
		AttachableBuilding.GetAttachedAbove(GetComponent<AttachableBuilding>(), ref buildings);
		if (amount > 0)
		{
			buildings.Reverse();
		}
		foreach (GameObject item in buildings)
		{
			UnmarkBuilding(item, null);
			TransformExtensions.SetPosition(position: Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(item), 0, amount), CellAlignment.Bottom, Grid.SceneLayer.BuildingFront), transform: item.transform);
			MarkBuilding(item, null);
			item.GetComponent<ReorderableBuilding>().ApplyAnimOffset(-amount);
		}
		if (amount <= 0)
		{
			return;
		}
		foreach (GameObject item2 in buildings)
		{
			item2.GetComponent<AttachableBuilding>().RegisterWithAttachPoint(register: true);
		}
	}

	public void SwapWithAbove(bool selectOnComplete = true)
	{
		BuildingAttachPoint component = GetComponent<BuildingAttachPoint>();
		if (!(component == null) && !(component.points[0].attachedBuilding == null))
		{
			int num = Grid.PosToCell(base.gameObject);
			UnmarkBuilding(base.gameObject, null);
			AttachableBuilding attachedBuilding = component.points[0].attachedBuilding;
			BuildingAttachPoint component2 = attachedBuilding.GetComponent<BuildingAttachPoint>();
			AttachableBuilding aboveBuilding = ((component2 != null) ? component2.points[0].attachedBuilding : null);
			UnmarkBuilding(attachedBuilding.gameObject, aboveBuilding);
			Building component3 = attachedBuilding.GetComponent<Building>();
			int cell = num;
			attachedBuilding.transform.SetPosition(Grid.CellToPos(cell, CellAlignment.Bottom, Grid.SceneLayer.BuildingFront));
			MarkBuilding(attachedBuilding.gameObject, null);
			int cell2 = Grid.OffsetCell(num, 0, component3.Def.HeightInCells);
			base.transform.SetPosition(Grid.CellToPos(cell2, CellAlignment.Bottom, Grid.SceneLayer.BuildingFront));
			MarkBuilding(base.gameObject, aboveBuilding);
			RebuildNetworks();
			ApplyAnimOffset(-component3.Def.HeightInCells);
			Building component4 = GetComponent<Building>();
			component3.GetComponent<ReorderableBuilding>().ApplyAnimOffset(component4.Def.HeightInCells);
			if (selectOnComplete)
			{
				SelectTool.Instance.Select(component4.GetComponent<KSelectable>());
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (GetComponent<BuildingUnderConstruction>() == null && !this.HasTag(GameTags.RocketInSpace))
		{
			RemoveModule();
		}
		if (m_animLink != null)
		{
			m_animLink.Unregister();
		}
		base.OnCleanUp();
	}

	private void ApplyAnimOffset(float amount)
	{
		animController.Offset = new Vector3(animController.Offset.x, animController.Offset.y + amount, animController.Offset.z);
		reorderArmController.Offset = animController.Offset;
		string text = GetComponent<Building>().Def.WidthInCells + "x" + GetComponent<Building>().Def.HeightInCells + "_grab";
		if (!reorderArmController.HasAnimation(text))
		{
			text = "3x3_grab";
		}
		reorderArmController.Play(text);
		reorderArmController.onAnimComplete += StartReorderingAnim;
	}

	private void StartReorderingAnim(HashedString data)
	{
		loopingSounds.StartSound(GlobalAssets.GetSound(reorderSound));
		reorderingAnimUnderway = true;
		reorderArmController.onAnimComplete -= StartReorderingAnim;
	}

	public void SwapWithBelow(bool selectOnComplete = true)
	{
		if (!(GetComponent<AttachableBuilding>() == null) && !(GetComponent<AttachableBuilding>().GetAttachedTo() == null))
		{
			GetComponent<AttachableBuilding>().GetAttachedTo().GetComponent<ReorderableBuilding>().SwapWithAbove(!selectOnComplete);
			if (selectOnComplete)
			{
				SelectTool.Instance.Select(GetComponent<KSelectable>());
			}
		}
	}

	public bool CanMoveVertically(int moveAmount, GameObject ignoreBuilding = null)
	{
		if (moveAmount == 0)
		{
			return true;
		}
		BuildingAttachPoint component = GetComponent<BuildingAttachPoint>();
		AttachableBuilding component2 = GetComponent<AttachableBuilding>();
		if (moveAmount > 0)
		{
			if (component != null && component.points[0].attachedBuilding != null && component.points[0].attachedBuilding.gameObject != ignoreBuilding && !component.points[0].attachedBuilding.GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount))
			{
				return false;
			}
		}
		else if (component2 != null)
		{
			BuildingAttachPoint attachedTo = component2.GetAttachedTo();
			if (attachedTo != null && attachedTo.gameObject != ignoreBuilding && !component2.GetAttachedTo().GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount))
			{
				return false;
			}
		}
		CellOffset[] occupiedOffsets = GetOccupiedOffsets();
		foreach (CellOffset offset in occupiedOffsets)
		{
			if (!CheckCellClear(Grid.OffsetCell(Grid.OffsetCell(Grid.PosToCell(base.gameObject), offset), 0, moveAmount), base.gameObject))
			{
				return false;
			}
		}
		return true;
	}

	public static bool CheckCellClear(int checkCell, GameObject ignoreObject = null)
	{
		if (!Grid.IsValidCell(checkCell) || !Grid.IsValidBuildingCell(checkCell) || Grid.Solid[checkCell] || Grid.WorldIdx[checkCell] == ClusterManager.INVALID_WORLD_IDX)
		{
			return false;
		}
		if (Grid.Objects[checkCell, 1] != null && Grid.Objects[checkCell, 1] != ignoreObject && Grid.Objects[checkCell, 1].GetComponent<ReorderableBuilding>() == null)
		{
			return false;
		}
		return true;
	}

	public GameObject ConvertModule(BuildingDef toModule, IList<Tag> materials)
	{
		int cell = Grid.PosToCell(base.gameObject);
		int num = toModule.HeightInCells - GetComponent<Building>().Def.HeightInCells;
		base.gameObject.GetComponent<Building>();
		BuildingAttachPoint component = base.gameObject.GetComponent<BuildingAttachPoint>();
		GameObject gameObject = null;
		if (component != null && component.points[0].attachedBuilding != null)
		{
			gameObject = component.points[0].attachedBuilding.gameObject;
			component.points[0].attachedBuilding = null;
			Components.BuildingAttachPoints.Remove(component);
		}
		UnmarkBuilding(base.gameObject, null);
		if (materials == null)
		{
			materials = toModule.DefaultElements();
		}
		if (num != 0 && gameObject != null)
		{
			gameObject.GetComponent<ReorderableBuilding>().MoveVertical(num);
		}
		if (!DebugHandler.InstantBuildMode && !toModule.IsValidPlaceLocation(base.gameObject, cell, Orientation.Neutral, out var fail_reason))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, fail_reason, base.transform);
			if (num != 0 && gameObject != null)
			{
				num *= -1;
				gameObject.GetComponent<ReorderableBuilding>().MoveVertical(num);
			}
			return null;
		}
		GameObject gameObject2 = null;
		gameObject2 = ((!DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild)) ? toModule.TryPlace(base.gameObject, Grid.CellToPosCBC(cell, toModule.SceneLayer), Orientation.Neutral, materials) : toModule.Build(cell, Orientation.Neutral, null, materials, 273.15f, playsound: true, GameClock.Instance.GetTime()));
		RocketModuleCluster component2 = GetComponent<RocketModuleCluster>();
		RocketModuleCluster component3 = gameObject2.GetComponent<RocketModuleCluster>();
		if (component2 != null && component3 != null)
		{
			component2.CraftInterface.AddModule(component3);
		}
		Util.KDestroyGameObject(base.gameObject);
		return gameObject2;
	}

	private CellOffset[] GetOccupiedOffsets()
	{
		OccupyArea component = GetComponent<OccupyArea>();
		if (component != null)
		{
			return component.OccupiedCellsOffsets;
		}
		return GetComponent<BuildingUnderConstruction>().Def.PlacementOffsets;
	}

	public bool CanChangeModule()
	{
		string text = "";
		text = ((!(GetComponent<BuildingUnderConstruction>() != null)) ? GetComponent<Building>().Def.PrefabID : GetComponent<BuildingUnderConstruction>().Def.PrefabID);
		RocketModuleCluster component = GetComponent<RocketModuleCluster>();
		if (component != null)
		{
			if (component.CraftInterface != null)
			{
				if (component.CraftInterface.GetComponent<Clustercraft>().Status != 0)
				{
					return false;
				}
			}
			else if (component.conditionManager != null && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(component.conditionManager).state != 0)
			{
				return false;
			}
		}
		if (text != templateBuildingID)
		{
			return text != Assets.GetBuildingDef(templateBuildingID).BuildingUnderConstruction.PrefabID();
		}
		return false;
	}

	public bool CanRemoveModule()
	{
		return true;
	}

	public bool CanSwapUp(bool alsoCheckAboveCanSwapDown = true)
	{
		BuildingAttachPoint component = GetComponent<BuildingAttachPoint>();
		if (component == null)
		{
			return false;
		}
		if (GetComponent<AttachableBuilding>() == null || GetComponent<RocketEngineCluster>() != null)
		{
			return false;
		}
		AttachableBuilding attachedBuilding = component.points[0].attachedBuilding;
		if (attachedBuilding == null)
		{
			return false;
		}
		if (attachedBuilding.GetComponent<BuildingAttachPoint>() == null || attachedBuilding.HasTag(GameTags.NoseRocketModule))
		{
			return false;
		}
		if (!CanMoveVertically(attachedBuilding.GetComponent<Building>().Def.HeightInCells, attachedBuilding.gameObject))
		{
			return false;
		}
		if (alsoCheckAboveCanSwapDown && !attachedBuilding.GetComponent<ReorderableBuilding>().CanSwapDown(alsoCheckBelowCanSwapUp: false))
		{
			return false;
		}
		return true;
	}

	public bool CanSwapDown(bool alsoCheckBelowCanSwapUp = true)
	{
		if (base.gameObject.HasTag(GameTags.NoseRocketModule))
		{
			return false;
		}
		AttachableBuilding component = GetComponent<AttachableBuilding>();
		if (component == null)
		{
			return false;
		}
		BuildingAttachPoint attachedTo = component.GetAttachedTo();
		if (attachedTo == null)
		{
			return false;
		}
		if (GetComponent<BuildingAttachPoint>() == null)
		{
			return false;
		}
		if (attachedTo.GetComponent<AttachableBuilding>() == null || attachedTo.GetComponent<RocketEngineCluster>() != null)
		{
			return false;
		}
		if (!CanMoveVertically(attachedTo.GetComponent<Building>().Def.HeightInCells * -1, attachedTo.gameObject))
		{
			return false;
		}
		if (alsoCheckBelowCanSwapUp && !attachedTo.GetComponent<ReorderableBuilding>().CanSwapUp(alsoCheckAboveCanSwapDown: false))
		{
			return false;
		}
		return true;
	}

	public void ShowReorderArm(bool show)
	{
		if (reorderArmController != null)
		{
			reorderArmController.gameObject.SetActive(show);
		}
	}

	private static void RebuildNetworks()
	{
		Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
		Game.Instance.gasConduitSystem.ForceRebuildNetworks();
		Game.Instance.liquidConduitSystem.ForceRebuildNetworks();
		Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
		Game.Instance.solidConduitSystem.ForceRebuildNetworks();
	}

	private static void UnmarkBuilding(GameObject go, AttachableBuilding aboveBuilding)
	{
		int cell = Grid.PosToCell(go);
		Building component = go.GetComponent<Building>();
		component.Def.UnmarkArea(cell, component.Orientation, component.Def.ObjectLayer, go);
		AttachableBuilding component2 = go.GetComponent<AttachableBuilding>();
		if (component2 != null)
		{
			component2.RegisterWithAttachPoint(register: false);
		}
		if (aboveBuilding != null)
		{
			aboveBuilding.RegisterWithAttachPoint(register: false);
		}
		RocketModule component3 = go.GetComponent<RocketModule>();
		if (component3 != null)
		{
			component3.DeregisterComponents();
		}
		RocketConduitSender[] components = go.GetComponents<RocketConduitSender>();
		if (components.Length != 0)
		{
			RocketConduitSender[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RemoveConduitPortFromNetwork();
			}
		}
		RocketConduitReceiver[] components2 = go.GetComponents<RocketConduitReceiver>();
		if (components2.Length != 0)
		{
			RocketConduitReceiver[] array2 = components2;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].RemoveConduitPortFromNetwork();
			}
		}
	}

	private static void MarkBuilding(GameObject go, AttachableBuilding aboveBuilding)
	{
		int cell = Grid.PosToCell(go);
		Building component = go.GetComponent<Building>();
		component.Def.MarkArea(cell, component.Orientation, component.Def.ObjectLayer, go);
		if (component.GetComponent<OccupyArea>() != null)
		{
			component.GetComponent<OccupyArea>().UpdateOccupiedArea();
		}
		LogicPorts component2 = component.GetComponent<LogicPorts>();
		if ((bool)component2 && go.GetComponent<BuildingComplete>() != null)
		{
			component2.OnMove();
		}
		component.GetComponent<AttachableBuilding>().RegisterWithAttachPoint(register: true);
		if (aboveBuilding != null)
		{
			aboveBuilding.RegisterWithAttachPoint(register: true);
		}
		RocketModule component3 = go.GetComponent<RocketModule>();
		if (component3 != null)
		{
			component3.RegisterComponents();
		}
		VerticalModuleTiler component4 = go.GetComponent<VerticalModuleTiler>();
		if (component4 != null)
		{
			component4.PostReorderMove();
		}
		RocketConduitSender[] components = go.GetComponents<RocketConduitSender>();
		if (components.Length != 0)
		{
			RocketConduitSender[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddConduitPortToNetwork();
			}
		}
		RocketConduitReceiver[] components2 = go.GetComponents<RocketConduitReceiver>();
		if (components2.Length != 0)
		{
			RocketConduitReceiver[] array2 = components2;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].AddConduitPortToNetwork();
			}
		}
	}
}
