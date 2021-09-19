using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RequiresFoundation : KGameObjectComponentManager<RequiresFoundation.Data>, IKComponentManager
{
	public struct Data
	{
		public int cell;

		public int width;

		public int height;

		public BuildLocationRule buildRule;

		public HandleVector<int>.Handle solidPartitionerEntry;

		public HandleVector<int>.Handle buildingPartitionerEntry;

		public bool solid;

		public GameObject go;

		public Action<object> changeCallback;
	}

	public static readonly Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	public HandleVector<int>.Handle Add(GameObject go)
	{
		BuildingDef def = go.GetComponent<Building>().Def;
		int cell = Grid.PosToCell(go.transform.GetPosition());
		Data data = default(Data);
		data.cell = cell;
		data.width = def.WidthInCells;
		data.height = def.HeightInCells;
		data.buildRule = def.BuildLocationRule;
		data.solid = true;
		data.go = go;
		Data data2 = data;
		HandleVector<int>.Handle h = Add(go, data2);
		if (def.ContinuouslyCheckFoundation)
		{
			data2.changeCallback = delegate
			{
				OnSolidChanged(h);
			};
			Rotatable component = data2.go.GetComponent<Rotatable>();
			Orientation orientation = ((component != null) ? component.GetOrientation() : Orientation.Neutral);
			int num = -(def.WidthInCells - 1) / 2;
			int x = def.WidthInCells / 2;
			CellOffset offset = new CellOffset(num, -1);
			CellOffset offset2 = new CellOffset(x, -1);
			if (def.BuildLocationRule == BuildLocationRule.OnCeiling || def.BuildLocationRule == BuildLocationRule.InCorner)
			{
				offset.y = def.HeightInCells;
				offset2.y = def.HeightInCells;
			}
			else if (def.BuildLocationRule == BuildLocationRule.OnWall)
			{
				offset = new CellOffset(num - 1, 0);
				offset2 = new CellOffset(num - 1, def.HeightInCells);
			}
			else if (def.BuildLocationRule == BuildLocationRule.WallFloor)
			{
				offset = new CellOffset(num - 1, -1);
				offset2 = new CellOffset(x, def.HeightInCells - 1);
			}
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offset, orientation);
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(offset2, orientation);
			int cell2 = Grid.OffsetCell(cell, rotatedCellOffset);
			int cell3 = Grid.OffsetCell(cell, rotatedCellOffset2);
			Vector2I vector2I = Grid.CellToXY(cell2);
			Vector2I vector2I2 = Grid.CellToXY(cell3);
			float xmin = Mathf.Min(vector2I.x, vector2I2.x);
			float xmax = Mathf.Max(vector2I.x, vector2I2.x);
			float ymin = Mathf.Min(vector2I.y, vector2I2.y);
			float ymax = Mathf.Max(vector2I.y, vector2I2.y);
			Rect rect = Rect.MinMaxRect(xmin, ymin, xmax, ymax);
			data2.solidPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.solidChangedLayer, data2.changeCallback);
			data2.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.objectLayers[1], data2.changeCallback);
			if (def.BuildLocationRule == BuildLocationRule.BuildingAttachPoint || def.BuildLocationRule == BuildLocationRule.OnFloorOrBuildingAttachPoint)
			{
				AttachableBuilding component2 = data2.go.GetComponent<AttachableBuilding>();
				component2.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(component2.onAttachmentNetworkChanged, data2.changeCallback);
			}
			SetData(h, data2);
			OnSolidChanged(h);
			data2 = GetData(h);
			UpdateSolidState(data2.solid, ref data2, forceUpdate: true);
		}
		return h;
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		Data new_data = GetData(h);
		GameScenePartitioner.Instance.Free(ref new_data.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref new_data.buildingPartitionerEntry);
		AttachableBuilding component = new_data.go.GetComponent<AttachableBuilding>();
		if (!component.IsNullOrDestroyed())
		{
			component.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(component.onAttachmentNetworkChanged, new_data.changeCallback);
		}
		SetData(h, new_data);
	}

	private void OnSolidChanged(HandleVector<int>.Handle h)
	{
		Data new_data = GetData(h);
		SimCellOccupier component = new_data.go.GetComponent<SimCellOccupier>();
		if (!(component == null) && !component.IsReady())
		{
			return;
		}
		Rotatable component2 = new_data.go.GetComponent<Rotatable>();
		Orientation orientation = ((component2 != null) ? component2.GetOrientation() : Orientation.Neutral);
		bool flag = BuildingDef.CheckFoundation(new_data.cell, orientation, new_data.buildRule, new_data.width, new_data.height);
		if (!flag && (new_data.buildRule == BuildLocationRule.BuildingAttachPoint || new_data.buildRule == BuildLocationRule.OnFloorOrBuildingAttachPoint))
		{
			List<GameObject> buildings = new List<GameObject>();
			AttachableBuilding.GetAttachedBelow(new_data.go.GetComponent<AttachableBuilding>(), ref buildings);
			if (buildings.Count > 0)
			{
				Operational component3 = buildings.Last().GetComponent<Operational>();
				if (component3 != null && component3.GetFlag(solidFoundation))
				{
					flag = true;
				}
			}
		}
		UpdateSolidState(flag, ref new_data);
		SetData(h, new_data);
	}

	private void UpdateSolidState(bool is_solid, ref Data data, bool forceUpdate = false)
	{
		if (data.solid != is_solid || forceUpdate)
		{
			data.solid = is_solid;
			Operational component = data.go.GetComponent<Operational>();
			if (component != null)
			{
				component.SetFlag(solidFoundation, is_solid);
			}
			AttachableBuilding component2 = data.go.GetComponent<AttachableBuilding>();
			if (component2 != null)
			{
				List<GameObject> buildings = new List<GameObject>();
				AttachableBuilding.GetAttachedAbove(component2, ref buildings);
				AttachableBuilding.NotifyBuildingsNetworkChanged(buildings);
			}
			data.go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, this);
		}
	}
}
