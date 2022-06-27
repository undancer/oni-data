using System;
using System.Collections.Generic;
using UnityEngine;

public class AcousticDisturbance
{
	private static readonly HashedString[] PreAnims = new HashedString[2] { "grid_pre", "grid_loop" };

	private static readonly HashedString PostAnim = "grid_pst";

	private static float distanceDelay = 0.25f;

	private static float duration = 3f;

	private static HashSet<int> cellsInRange = new HashSet<int>();

	public static void Emit(object data, int EmissionRadius)
	{
		GameObject gameObject = (GameObject)data;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		Vector2 vector = gameObject.transform.GetPosition();
		int num = Grid.PosToCell(vector);
		int num2 = EmissionRadius * EmissionRadius;
		cellsInRange = GameUtil.CollectCellsBreadthFirst(num, (int cell) => !Grid.Solid[cell], EmissionRadius);
		DrawVisualEffect(num, cellsInRange);
		for (int i = 0; i < liveMinionIdentities.Count; i++)
		{
			MinionIdentity minionIdentity = liveMinionIdentities[i];
			if (!(minionIdentity.gameObject != gameObject.gameObject))
			{
				continue;
			}
			Vector2 vector2 = minionIdentity.transform.GetPosition();
			if (Vector2.SqrMagnitude(vector - vector2) <= (float)num2)
			{
				int item = Grid.PosToCell(vector2);
				if (cellsInRange.Contains(item) && minionIdentity.GetSMI<StaminaMonitor.Instance>().IsSleeping())
				{
					minionIdentity.Trigger(-527751701, data);
					minionIdentity.Trigger(1621815900, data);
				}
			}
		}
		cellsInRange.Clear();
	}

	private static void DrawVisualEffect(int center_cell, HashSet<int> cells)
	{
		SoundEvent.PlayOneShot(GlobalResources.Instance().AcousticDisturbanceSound, Grid.CellToPos(center_cell));
		foreach (int cell in cells)
		{
			int gridDistance = GetGridDistance(cell, center_cell);
			GameScheduler.Instance.Schedule("radialgrid_pre", distanceDelay * (float)gridDistance, SpawnEffect, cell);
		}
	}

	private static void SpawnEffect(object data)
	{
		Grid.SceneLayer layer = Grid.SceneLayer.InteriorWall;
		int cell = (int)data;
		KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("radialgrid_kanim", Grid.CellToPosCCC(cell, layer), null, update_looping_sounds_position: false, layer);
		kBatchedAnimController.destroyOnAnimComplete = false;
		kBatchedAnimController.Play(PreAnims, KAnim.PlayMode.Loop);
		GameScheduler.Instance.Schedule("radialgrid_loop", duration, DestroyEffect, kBatchedAnimController);
	}

	private static void DestroyEffect(object data)
	{
		KBatchedAnimController obj = (KBatchedAnimController)data;
		obj.destroyOnAnimComplete = true;
		obj.Play(PostAnim);
	}

	private static int GetGridDistance(int cell, int center_cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		Vector2I vector2I2 = Grid.CellToXY(center_cell);
		Vector2I vector2I3 = vector2I - vector2I2;
		return Math.Abs(vector2I3.x) + Math.Abs(vector2I3.y);
	}
}
