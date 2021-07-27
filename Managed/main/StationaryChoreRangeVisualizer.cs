using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/StationaryChoreRangeVisualizer")]
public class StationaryChoreRangeVisualizer : KMonoBehaviour
{
	private struct VisData
	{
		public int cell;

		public KBatchedAnimController controller;
	}

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Rotatable rotatable;

	public int x;

	public int y;

	public int width;

	public int height;

	public bool movable;

	public Grid.SceneLayer sceneLayer = Grid.SceneLayer.FXFront;

	public CellOffset vision_offset;

	public Func<int, bool> blocking_cb = Grid.PhysicalBlockingCB;

	public bool blocking_tile_visible = true;

	private static readonly string AnimName = "transferarmgrid_kanim";

	private static readonly HashedString[] PreAnims = new HashedString[2] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private List<VisData> visualizers = new List<VisData>();

	private List<int> newCells = new List<int>();

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnSelectDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnSelect(data);
	});

	private static readonly EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer> OnRotatedDelegate = new EventSystem.IntraObjectHandler<StationaryChoreRangeVisualizer>(delegate(StationaryChoreRangeVisualizer component, object data)
	{
		component.OnRotated(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1503271301, OnSelectDelegate);
		if (movable)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "StationaryChoreRangeVisualizer.OnSpawn");
			Subscribe(-1643076535, OnRotatedDelegate);
		}
	}

	protected override void OnCleanUp()
	{
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		Unsubscribe(-1503271301, OnSelectDelegate);
		Unsubscribe(-1643076535, OnRotatedDelegate);
		ClearVisualizers();
		base.OnCleanUp();
	}

	private void OnSelect(object data)
	{
		if ((bool)data)
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_form"), base.transform.position);
			UpdateVisualizers();
		}
		else
		{
			SoundEvent.PlayOneShot(GlobalAssets.GetSound("RadialGrid_disappear"), base.transform.position);
			ClearVisualizers();
		}
	}

	private void OnRotated(object data)
	{
		UpdateVisualizers();
	}

	private void OnCellChange()
	{
		UpdateVisualizers();
	}

	private void UpdateVisualizers()
	{
		newCells.Clear();
		CellOffset rotatedCellOffset = vision_offset;
		if ((bool)rotatable)
		{
			rotatedCellOffset = rotatable.GetRotatedCellOffset(vision_offset);
		}
		int cell = Grid.PosToCell(base.transform.gameObject);
		Grid.CellToXY(Grid.OffsetCell(cell, rotatedCellOffset), out var num, out var num2);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				CellOffset offset = new CellOffset(this.x + j, this.y + i);
				if ((bool)rotatable)
				{
					offset = rotatable.GetRotatedCellOffset(offset);
				}
				int num3 = Grid.OffsetCell(cell, offset);
				if (Grid.IsValidCell(num3))
				{
					Grid.CellToXY(num3, out var x, out var y);
					if (Grid.TestLineOfSight(num, num2, x, y, blocking_cb, blocking_tile_visible))
					{
						newCells.Add(num3);
					}
				}
			}
		}
		for (int num4 = visualizers.Count - 1; num4 >= 0; num4--)
		{
			if (newCells.Contains(visualizers[num4].cell))
			{
				newCells.Remove(visualizers[num4].cell);
			}
			else
			{
				DestroyEffect(visualizers[num4].controller);
				visualizers.RemoveAt(num4);
			}
		}
		for (int k = 0; k < newCells.Count; k++)
		{
			KBatchedAnimController controller = CreateEffect(newCells[k]);
			visualizers.Add(new VisData
			{
				cell = newCells[k],
				controller = controller
			});
		}
	}

	private void ClearVisualizers()
	{
		for (int i = 0; i < visualizers.Count; i++)
		{
			DestroyEffect(visualizers[i].controller);
		}
		visualizers.Clear();
	}

	private KBatchedAnimController CreateEffect(int cell)
	{
		KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect(AnimName, Grid.CellToPosCCC(cell, sceneLayer), null, update_looping_sounds_position: false, sceneLayer, set_inactive: true);
		kBatchedAnimController.destroyOnAnimComplete = false;
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
		kBatchedAnimController.gameObject.SetActive(value: true);
		kBatchedAnimController.Play(PreAnims, KAnim.PlayMode.Loop);
		return kBatchedAnimController;
	}

	private void DestroyEffect(KBatchedAnimController controller)
	{
		controller.destroyOnAnimComplete = true;
		controller.Play(PostAnim);
	}
}
