using System;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
	public class Tuning : TuningData<Tuning>
	{
		public float maxVelocity;

		public float maxVelocityInLiquid;
	}

	private const float Acceleration = -9.8f;

	private static Tag[] LANDS_ON_FAKEFLOOR = new Tag[3]
	{
		GameTags.Minion,
		GameTags.Creatures.Walker,
		GameTags.Creatures.Hoverer
	};

	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, System.Action on_landed = null)
	{
		bool land_on_fake_floors = false;
		KPrefabID component = go.GetComponent<KPrefabID>();
		if (component != null)
		{
			land_on_fake_floors = component.HasAnyTags(LANDS_ON_FAKEFLOOR);
		}
		bool mayLeaveWorld = go.GetComponent<MinionIdentity>() != null;
		return Add(go, new GravityComponent(go.transform, on_landed, initial_velocity, land_on_fake_floors, mayLeaveWorld));
	}

	public override void FixedUpdate(float dt)
	{
		Tuning tuning = TuningData<Tuning>.Get();
		float num = tuning.maxVelocity * tuning.maxVelocity;
		for (int i = 0; i < data.Count; i++)
		{
			GravityComponent value = data[i];
			if (value.elapsedTime < 0f || value.transform == null || IsInCleanupList(value.transform.gameObject))
			{
				continue;
			}
			Vector3 position = value.transform.GetPosition();
			Vector2 vector = position;
			Vector2 vector2 = new Vector2(value.velocity.x, value.velocity.y + -9.8f * dt);
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > num)
			{
				vector2 *= tuning.maxVelocity / Mathf.Sqrt(sqrMagnitude);
			}
			int num2 = Grid.PosToCell(vector);
			bool flag = Grid.IsVisiblyInLiquid(vector - new Vector2(0f, value.bottomYOffset));
			if (flag)
			{
				flag = true;
				float num3 = (float)(value.transform.GetInstanceID() % 1000) / 1000f * 0.25f;
				float num4 = tuning.maxVelocityInLiquid + num3 * tuning.maxVelocityInLiquid;
				if (sqrMagnitude > num4 * num4)
				{
					float num5 = Mathf.Sqrt(sqrMagnitude);
					vector2 = vector2 / num5 * Mathf.Lerp(num5, num3, dt * (5f + 5f * num3));
				}
			}
			value.velocity = vector2;
			value.elapsedTime += dt;
			Vector2 vector3 = vector + vector2 * dt;
			Vector2 pos = vector3;
			pos.y = vector3.y - value.bottomYOffset;
			bool flag2 = Grid.IsVisiblyInLiquid(vector3 + new Vector2(0f, value.bottomYOffset));
			if (!flag && flag2)
			{
				KBatchedAnimController kBatchedAnimController = FXHelpers.CreateEffect("splash_step_kanim", new Vector3(vector3.x, vector3.y, 0f) + new Vector3(-0.38f, 0.75f, -0.1f), null, update_looping_sounds_position: false, Grid.SceneLayer.FXFront);
				kBatchedAnimController.Play("fx1");
				kBatchedAnimController.destroyOnAnimComplete = true;
			}
			bool flag3 = false;
			int num6 = Grid.PosToCell(pos);
			if (Grid.IsValidCell(num6))
			{
				if (vector2.sqrMagnitude > 0.2f && Grid.IsValidCell(num2) && !Grid.Element[num2].IsLiquid && Grid.Element[num6].IsLiquid)
				{
					AmbienceType ambience = Grid.Element[num6].substance.GetAmbience();
					if (ambience != AmbienceType.None)
					{
						string text = Sounds.Instance.OreSplashSoundsMigrated[(int)ambience];
						if (CameraController.Instance != null && CameraController.Instance.IsAudibleSound(vector3, text))
						{
							SoundEvent.PlayOneShot(text, vector3);
						}
					}
				}
				bool flag4 = Grid.Solid[num6];
				if (!flag4 && value.landOnFakeFloors && Grid.FakeFloor[num6])
				{
					Navigator component = value.transform.GetComponent<Navigator>();
					if ((bool)component)
					{
						flag4 = component.NavGrid.NavTable.IsValid(num6);
						if (!flag4)
						{
							int cell = Grid.CellAbove(num6);
							flag4 = component.NavGrid.NavTable.IsValid(cell, NavType.Hover);
						}
					}
				}
				if (flag4)
				{
					vector3.y = Grid.CellToPosCBC(Grid.CellAbove(num6), Grid.SceneLayer.Move).y + value.bottomYOffset;
					value.velocity.x = 0f;
					flag3 = true;
				}
				else
				{
					Vector2 pos2 = vector3;
					pos2.x -= value.extents.x;
					int num7 = Grid.PosToCell(pos2);
					if (Grid.IsValidCell(num7) && Grid.Solid[num7])
					{
						vector3.x = Mathf.Floor(vector3.x - value.extents.x) + (1f + value.extents.x);
						value.velocity.x = -0.1f * value.velocity.x;
					}
					else
					{
						Vector3 pos3 = vector3;
						pos3.x += value.extents.x;
						int num8 = Grid.PosToCell(pos3);
						if (Grid.IsValidCell(num8) && Grid.Solid[num8])
						{
							vector3.x = Mathf.Floor(vector3.x + value.extents.x) - value.extents.x;
							value.velocity.x = -0.1f * value.velocity.x;
						}
					}
				}
			}
			data[i] = value;
			int cell2 = Grid.PosToCell(vector3);
			if (!value.mayLeaveWorld && Grid.IsValidCell(num2) && Grid.WorldIdx[num2] != ClusterManager.INVALID_WORLD_IDX && !Grid.IsValidCellInWorld(cell2, Grid.WorldIdx[num2]))
			{
				continue;
			}
			value.transform.SetPosition(new Vector3(vector3.x, vector3.y, position.z));
			if (flag3)
			{
				value.transform.gameObject.Trigger(1188683690, vector2);
				if (value.onLanded != null)
				{
					value.onLanded();
				}
			}
		}
	}
}
