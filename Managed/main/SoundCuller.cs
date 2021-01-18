using UnityEngine;

public struct SoundCuller
{
	public class Tuning : TuningData<Tuning>
	{
		public float extraYRange;
	}

	private Vector2 min;

	private Vector2 max;

	private Vector2 cameraPos;

	private float zoomScaler;

	public static bool IsAudibleWorld(Vector2 pos)
	{
		bool result = false;
		int num = Grid.PosToCell(pos);
		if (Grid.IsValidCell(num) && Grid.WorldIdx[num] == ClusterManager.Instance.activeWorldId)
		{
			result = true;
		}
		return result;
	}

	public bool IsAudible(Vector2 pos)
	{
		return IsAudibleWorld(pos) && min.LessEqual(pos) && pos.LessEqual(max);
	}

	public bool IsAudibleNoCameraScaling(Vector2 pos, float falloff_distance_sq)
	{
		float num = (pos.x - cameraPos.x) * (pos.x - cameraPos.x) + (pos.y - cameraPos.y) * (pos.y - cameraPos.y);
		return num < falloff_distance_sq;
	}

	public bool IsAudible(Vector2 pos, float falloff_distance_sq)
	{
		if (!IsAudibleWorld(pos))
		{
			return false;
		}
		pos = GetVerticallyScaledPosition(pos);
		return IsAudibleNoCameraScaling(pos, falloff_distance_sq);
	}

	public bool IsAudible(Vector2 pos, HashedString sound_path)
	{
		if (!sound_path.IsValid)
		{
			return false;
		}
		return IsAudible(pos, KFMOD.GetSoundEventDescription(sound_path).falloffDistanceSq);
	}

	public Vector3 GetVerticallyScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
	{
		float num = 0f;
		float num2 = 1f;
		if (pos.y > max.y)
		{
			num = Mathf.Abs(pos.y - max.y);
		}
		else if (pos.y < min.y)
		{
			num = Mathf.Abs(pos.y - min.y);
			num2 = -1f;
		}
		else
		{
			num = 0f;
		}
		float extraYRange = TuningData<Tuning>.Get().extraYRange;
		num = ((num < extraYRange) ? num : extraYRange);
		float num3 = num * num / (4f * zoomScaler);
		num3 *= num2;
		Vector3 result = new Vector3(pos.x, pos.y + num3, 0f);
		if (objectIsSelectedAndVisible)
		{
			result.z = pos.z;
		}
		return result;
	}

	public static SoundCuller CreateCuller()
	{
		SoundCuller result = default(SoundCuller);
		Camera main = Camera.main;
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(1f, 1f, Camera.main.transform.GetPosition().z));
		Vector3 vector2 = main.ViewportToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.GetPosition().z));
		result.min = new Vector3(vector2.x, vector2.y, 0f);
		result.max = new Vector3(vector.x, vector.y, 0f);
		result.cameraPos = main.transform.GetPosition();
		Audio audio = Audio.Get();
		float orthographicSize = CameraController.Instance.cameras[0].orthographicSize;
		float num = orthographicSize / (audio.listenerReferenceZ - audio.listenerMinZ);
		num = (result.zoomScaler = ((!(num <= 0f)) ? 1f : 2f));
		return result;
	}
}
