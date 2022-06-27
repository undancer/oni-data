using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UserNavigation")]
public class UserNavigation : KMonoBehaviour
{
	[Serializable]
	private struct NavPoint
	{
		public Vector3 pos;

		public float orthoSize;

		public static readonly NavPoint Invalid = new NavPoint
		{
			pos = Vector3.zero,
			orthoSize = 0f
		};

		public bool IsValid()
		{
			return orthoSize != 0f;
		}
	}

	[Serialize]
	private List<NavPoint> hotkeyNavPoints = new List<NavPoint>();

	[Serialize]
	private Dictionary<int, NavPoint> worldCameraPositions = new Dictionary<int, NavPoint>();

	public UserNavigation()
	{
		for (Action action = Action.SetUserNav1; action <= Action.SetUserNav10; action++)
		{
			hotkeyNavPoints.Add(NavPoint.Invalid);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Game.Instance.Subscribe(1983128072, delegate(object worlds)
		{
			Tuple<int, int> obj = (Tuple<int, int>)worlds;
			int first = obj.first;
			int second = obj.second;
			int num = Grid.PosToCell(CameraController.Instance.transform.position);
			if (!Grid.IsValidCell(num) || Grid.WorldIdx[num] != second)
			{
				WorldContainer world = ClusterManager.Instance.GetWorld(second);
				float x = Mathf.Clamp(CameraController.Instance.transform.position.x, world.minimumBounds.x, world.maximumBounds.x);
				float y = Mathf.Clamp(CameraController.Instance.transform.position.y, world.minimumBounds.y, world.maximumBounds.y);
				Vector3 position = new Vector3(x, y, CameraController.Instance.transform.position.z);
				CameraController.Instance.SetPosition(position);
			}
			worldCameraPositions[second] = new NavPoint
			{
				pos = CameraController.Instance.transform.position,
				orthoSize = CameraController.Instance.targetOrthographicSize
			};
			if (!worldCameraPositions.ContainsKey(first))
			{
				WorldContainer world2 = ClusterManager.Instance.GetWorld(first);
				Vector2I vector2I = world2.WorldOffset + new Vector2I(world2.Width / 2, world2.Height / 2);
				worldCameraPositions.Add(first, new NavPoint
				{
					pos = new Vector3(vector2I.x, vector2I.y),
					orthoSize = CameraController.Instance.targetOrthographicSize
				});
			}
			CameraController.Instance.SetTargetPosForWorldChange(worldCameraPositions[first].pos, worldCameraPositions[first].orthoSize, playSound: false);
		});
	}

	public void SetWorldCameraStartPosition(int world_id, Vector3 start_pos)
	{
		if (!worldCameraPositions.ContainsKey(world_id))
		{
			worldCameraPositions.Add(world_id, new NavPoint
			{
				pos = new Vector3(start_pos.x, start_pos.y),
				orthoSize = CameraController.Instance.targetOrthographicSize
			});
		}
		else
		{
			worldCameraPositions[world_id] = new NavPoint
			{
				pos = new Vector3(start_pos.x, start_pos.y),
				orthoSize = CameraController.Instance.targetOrthographicSize
			};
		}
	}

	private static int GetIndex(Action action)
	{
		int result = -1;
		if (Action.SetUserNav1 <= action && action <= Action.SetUserNav10)
		{
			result = (int)(action - 15);
		}
		else if (Action.GotoUserNav1 <= action && action <= Action.GotoUserNav10)
		{
			result = (int)(action - 25);
		}
		return result;
	}

	private void SetHotkeyNavPoint(Action action, Vector3 pos, float ortho_size)
	{
		int index = GetIndex(action);
		if (index >= 0)
		{
			hotkeyNavPoints[index] = new NavPoint
			{
				pos = pos,
				orthoSize = ortho_size
			};
			EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_set"), Vector3.zero);
			instance.setParameterByName("userNavPoint_ID", index);
			KFMOD.EndOneShot(instance);
		}
	}

	private void GoToHotkeyNavPoint(Action action)
	{
		int index = GetIndex(action);
		if (index >= 0)
		{
			NavPoint navPoint = hotkeyNavPoints[index];
			if (navPoint.IsValid())
			{
				CameraController.Instance.SetTargetPos(navPoint.pos, navPoint.orthoSize, playSound: true);
				EventInstance instance = KFMOD.BeginOneShot(GlobalAssets.GetSound("UserNavPoint_recall"), Vector3.zero);
				instance.setParameterByName("userNavPoint_ID", index);
				KFMOD.EndOneShot(instance);
			}
		}
	}

	public bool Handle(KButtonEvent e)
	{
		bool flag = false;
		for (Action action = Action.GotoUserNav1; action <= Action.GotoUserNav10; action++)
		{
			if (e.TryConsume(action))
			{
				GoToHotkeyNavPoint(action);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			for (Action action2 = Action.SetUserNav1; action2 <= Action.SetUserNav10; action2++)
			{
				if (e.TryConsume(action2))
				{
					Camera baseCamera = CameraController.Instance.baseCamera;
					Vector3 position = baseCamera.transform.GetPosition();
					SetHotkeyNavPoint(action2, position, baseCamera.orthographicSize);
					flag = true;
					break;
				}
			}
		}
		return flag;
	}
}
