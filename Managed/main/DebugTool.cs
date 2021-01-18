using System.Collections.Generic;
using UnityEngine;

public class DebugTool : DragTool
{
	public enum Type
	{
		Dig,
		Heat,
		Cool,
		ReplaceSubstance,
		FillReplaceSubstance,
		AddPressure,
		RemovePressure,
		PaintPlant,
		Clear,
		AddSelection,
		RemoveSelection,
		Deconstruct,
		Destroy,
		Sample,
		StoreSubstance
	}

	public static DebugTool Instance;

	public Type type;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	public void Activate(Type type)
	{
		this.type = type;
		Activate();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		PlayerController.Instance.ToolDeactivated(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		switch (type)
		{
		case Type.Dig:
			SimMessages.Dig(cell);
			break;
		case Type.Heat:
			SimMessages.ModifyEnergy(cell, 10000f, 10000f, SimMessages.EnergySourceID.DebugHeat);
			break;
		case Type.Cool:
			SimMessages.ModifyEnergy(cell, -10000f, 10000f, SimMessages.EnergySourceID.DebugCool);
			break;
		case Type.AddPressure:
			SimMessages.ModifyMass(cell, 10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 293f, SimHashes.Oxygen);
			break;
		case Type.RemovePressure:
			SimMessages.ModifyMass(cell, -10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 0f, SimHashes.Oxygen);
			break;
		case Type.ReplaceSubstance:
			DoReplaceSubstance(cell);
			break;
		case Type.FillReplaceSubstance:
		{
			GameUtil.FloodFillNext.Clear();
			GameUtil.FloodFillVisited.Clear();
			SimHashes elem_hash = Grid.Element[cell].id;
			GameUtil.FloodFillConditional(cell, delegate(int check_cell)
			{
				bool result = false;
				if (Grid.Element[check_cell].id == elem_hash)
				{
					result = true;
					DoReplaceSubstance(check_cell);
				}
				return result;
			}, GameUtil.FloodFillVisited);
			break;
		}
		case Type.Clear:
			ClearCell(cell);
			break;
		case Type.AddSelection:
			DebugBaseTemplateButton.Instance.AddToSelection(cell);
			break;
		case Type.RemoveSelection:
			DebugBaseTemplateButton.Instance.RemoveFromSelection(cell);
			break;
		case Type.Destroy:
			DestroyCell(cell);
			break;
		case Type.Deconstruct:
			DeconstructCell(cell);
			break;
		case Type.Sample:
			DebugPaintElementScreen.Instance.SampleCell(cell);
			break;
		case Type.StoreSubstance:
			DoStoreSubstance(cell);
			break;
		case Type.PaintPlant:
			break;
		}
	}

	public void DoReplaceSubstance(int cell)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			return;
		}
		Element element = (DebugPaintElementScreen.Instance.paintElement.isOn ? ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element) : ElementLoader.elements[Grid.ElementIdx[cell]]);
		if (element == null)
		{
			element = ElementLoader.FindElementByHash(SimHashes.Vacuum);
		}
		byte b = (DebugPaintElementScreen.Instance.paintDisease.isOn ? DebugPaintElementScreen.Instance.diseaseIdx : Grid.DiseaseIdx[cell]);
		float num = (DebugPaintElementScreen.Instance.paintTemperature.isOn ? DebugPaintElementScreen.Instance.temperature : Grid.Temperature[cell]);
		float num2 = (DebugPaintElementScreen.Instance.paintMass.isOn ? DebugPaintElementScreen.Instance.mass : Grid.Mass[cell]);
		int num3 = (DebugPaintElementScreen.Instance.paintDiseaseCount.isOn ? DebugPaintElementScreen.Instance.diseaseCount : Grid.DiseaseCount[cell]);
		if (num == -1f)
		{
			num = element.defaultValues.temperature;
		}
		if (num2 == -1f)
		{
			num2 = element.defaultValues.mass;
		}
		if (DebugPaintElementScreen.Instance.affectCells.isOn)
		{
			SimMessages.ReplaceElement(cell, element.id, CellEventLogger.Instance.DebugTool, num2, num, b, num3);
			if (DebugPaintElementScreen.Instance.set_prevent_fow_reveal)
			{
				Grid.Visible[cell] = 0;
				Grid.PreventFogOfWarReveal[cell] = true;
			}
			else if (DebugPaintElementScreen.Instance.set_allow_fow_reveal && Grid.PreventFogOfWarReveal[cell])
			{
				Grid.PreventFogOfWarReveal[cell] = false;
			}
		}
		if (!DebugPaintElementScreen.Instance.affectBuildings.isOn)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(Grid.Objects[cell, 1]);
		list.Add(Grid.Objects[cell, 2]);
		list.Add(Grid.Objects[cell, 9]);
		list.Add(Grid.Objects[cell, 16]);
		list.Add(Grid.Objects[cell, 12]);
		list.Add(Grid.Objects[cell, 16]);
		list.Add(Grid.Objects[cell, 26]);
		foreach (GameObject item in list)
		{
			if (item != null)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				if (num > 0f)
				{
					component.Temperature = num;
				}
				if (num3 > 0 && b != byte.MaxValue)
				{
					component.ModifyDiseaseCount(int.MinValue, "DebugTool.DoReplaceSubstance");
					component.AddDisease(b, num3, "DebugTool.DoReplaceSubstance");
				}
			}
		}
	}

	public void DeconstructCell(int cell)
	{
		bool instantBuildMode = DebugHandler.InstantBuildMode;
		DebugHandler.InstantBuildMode = true;
		DeconstructTool.Instance.DeconstructCell(cell);
		if (!instantBuildMode)
		{
			DebugHandler.InstantBuildMode = false;
		}
	}

	public void DestroyCell(int cell)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(Grid.Objects[cell, 2]);
		list.Add(Grid.Objects[cell, 1]);
		list.Add(Grid.Objects[cell, 12]);
		list.Add(Grid.Objects[cell, 16]);
		list.Add(Grid.Objects[cell, 0]);
		list.Add(Grid.Objects[cell, 26]);
		foreach (GameObject item in list)
		{
			if (item != null)
			{
				Object.Destroy(item);
			}
		}
		ClearCell(cell);
		if (ElementLoader.elements[Grid.ElementIdx[cell]].id == SimHashes.Void)
		{
			SimMessages.ReplaceElement(cell, SimHashes.Void, CellEventLogger.Instance.DebugTool, 0f, 0f);
		}
		else
		{
			SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 0f, 0f);
		}
	}

	public void ClearCell(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		ListPool<ScenePartitionerEntry, DebugTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, DebugTool>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
			Pickupable pickupable = scenePartitionerEntry.obj as Pickupable;
			if (pickupable != null && pickupable.GetComponent<MinionBrain>() == null)
			{
				Util.KDestroyGameObject(pickupable.gameObject);
			}
		}
		pooledList.Recycle();
	}

	public void DoStoreSubstance(int cell)
	{
		if (!Grid.IsValidBuildingCell(cell))
		{
			return;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject == null)
		{
			return;
		}
		Storage component = gameObject.GetComponent<Storage>();
		if (!(component == null))
		{
			Element element = (DebugPaintElementScreen.Instance.paintElement.isOn ? ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element) : ElementLoader.elements[Grid.ElementIdx[cell]]);
			if (element == null)
			{
				element = ElementLoader.FindElementByHash(SimHashes.Vacuum);
			}
			byte disease_idx = (DebugPaintElementScreen.Instance.paintDisease.isOn ? DebugPaintElementScreen.Instance.diseaseIdx : Grid.DiseaseIdx[cell]);
			float num = (DebugPaintElementScreen.Instance.paintTemperature.isOn ? DebugPaintElementScreen.Instance.temperature : element.defaultValues.temperature);
			float num2 = (DebugPaintElementScreen.Instance.paintMass.isOn ? DebugPaintElementScreen.Instance.mass : element.defaultValues.mass);
			if (num == -1f)
			{
				num = element.defaultValues.temperature;
			}
			if (num2 == -1f)
			{
				num2 = element.defaultValues.mass;
			}
			int disease_count = (DebugPaintElementScreen.Instance.paintDiseaseCount.isOn ? DebugPaintElementScreen.Instance.diseaseCount : 0);
			if (element.IsGas)
			{
				component.AddGasChunk(element.id, num2, num, disease_idx, disease_count, keep_zero_mass: false);
			}
			else if (element.IsLiquid)
			{
				component.AddLiquid(element.id, num2, num, disease_idx, disease_count);
			}
			else if (element.IsSolid)
			{
				component.AddOre(element.id, num2, num, disease_idx, disease_count);
			}
		}
	}
}
