using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OrbitalObject")]
[SerializationConfig(MemberSerialization.OptIn)]
public class OrbitalObject : KMonoBehaviour, IRenderEveryTick
{
	private WorldContainer world;

	private OrbitalData orbitData;

	[Serialize]
	private string animFilename;

	[Serialize]
	private string initialAnim;

	[Serialize]
	private Vector3 worldOrbitingOrigin;

	[Serialize]
	private int orbitingWorldId;

	[Serialize]
	private float angle;

	[Serialize]
	private float timeoffset;

	[Serialize]
	public string orbitalDBId;

	public void Init(string orbit_data_name, WorldContainer orbiting_world)
	{
		OrbitalData orbitalData = Db.Get().OrbitalTypeCategories.Get(orbit_data_name);
		if (orbiting_world != null)
		{
			orbitingWorldId = orbiting_world.id;
			world = orbiting_world;
			worldOrbitingOrigin = GetWorldOrigin(world, orbitalData);
		}
		else
		{
			worldOrbitingOrigin = new Vector3((float)Grid.WidthInCells * 0.5f, (float)Grid.HeightInCells * orbitalData.yGridPercent, 0f);
		}
		animFilename = orbitalData.animFile;
		initialAnim = GetInitialAnim(orbitalData);
		angle = GetAngle(orbitalData);
		timeoffset = UnityEngine.Random.Range(0f, 600f);
		orbitalDBId = orbitalData.Id;
	}

	protected override void OnSpawn()
	{
		world = ClusterManager.Instance.GetWorld(orbitingWorldId);
		orbitData = Db.Get().OrbitalTypeCategories.Get(orbitalDBId);
		base.gameObject.SetActive(value: false);
		KBatchedAnimController kBatchedAnimController = base.gameObject.AddComponent<KBatchedAnimController>();
		kBatchedAnimController.isMovable = true;
		kBatchedAnimController.initialAnim = initialAnim;
		kBatchedAnimController.AnimFiles = new KAnimFile[1] { Assets.GetAnim(animFilename) };
		kBatchedAnimController.initialMode = KAnim.PlayMode.Loop;
		kBatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
	}

	public void RenderEveryTick(float dt)
	{
		float time = GameClock.Instance.GetTime();
		bool behind;
		Vector3 vector = CalculateWorldPos(time, out behind);
		Vector3 position = vector;
		if (orbitData.periodInCycles > 0f)
		{
			position.x = vector.x / (float)Grid.WidthInCells;
			position.y = vector.y / (float)Grid.HeightInCells;
			position.x = Camera.main.ViewportToWorldPoint(position).x;
			position.y = Camera.main.ViewportToWorldPoint(position).y;
		}
		bool flag = (!orbitData.rotatesBehind || !behind) && (world == null || ClusterManager.Instance.activeWorldId == world.id);
		base.gameObject.transform.SetPosition(position);
		if (orbitData.periodInCycles > 0f)
		{
			base.gameObject.transform.localScale = Vector3.one * (Camera.main.orthographicSize / orbitData.distance);
		}
		else
		{
			base.gameObject.transform.localScale = Vector3.one * orbitData.distance;
		}
		if (base.gameObject.activeSelf != flag)
		{
			base.gameObject.SetActive(flag);
		}
	}

	private Vector3 CalculateWorldPos(float time, out bool behind)
	{
		Vector3 result;
		if (orbitData.periodInCycles > 0f)
		{
			float num = orbitData.periodInCycles * 600f;
			float f = ((time + timeoffset) / num - (float)(int)((time + timeoffset) / num)) * 2f * (float)Math.PI;
			float num2 = 0.5f * orbitData.radiusScale * (float)world.WorldSize.x;
			Vector3 vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			behind = vector.z > orbitData.behindZ;
			Vector3 vector2 = Quaternion.Euler(angle, 0f, 0f) * (vector * num2);
			result = worldOrbitingOrigin + vector2;
			result.z = orbitData.renderZ;
		}
		else
		{
			behind = false;
			result = worldOrbitingOrigin;
			result.z = orbitData.renderZ;
		}
		return result;
	}

	private string GetInitialAnim(OrbitalData data)
	{
		if (data.initialAnim.IsNullOrWhiteSpace())
		{
			KAnimFileData data2 = Assets.GetAnim(data.animFile).GetData();
			int index = new System.Random().Next(0, data2.animCount - 1);
			return data2.GetAnim(index).name;
		}
		return data.initialAnim;
	}

	private Vector3 GetWorldOrigin(WorldContainer wc, OrbitalData data)
	{
		if (wc != null)
		{
			float x = (float)wc.WorldOffset.x + (float)wc.WorldSize.x * data.xGridPercent;
			float y = (float)wc.WorldOffset.y + (float)wc.WorldSize.y * data.yGridPercent;
			return new Vector3(x, y, 0f);
		}
		return new Vector3((float)Grid.WidthInCells * data.xGridPercent, (float)Grid.HeightInCells * data.yGridPercent, 0f);
	}

	private float GetAngle(OrbitalData data)
	{
		return UnityEngine.Random.Range(data.minAngle, data.maxAngle);
	}
}
