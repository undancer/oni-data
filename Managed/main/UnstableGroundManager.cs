using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/UnstableGroundManager")]
public class UnstableGroundManager : KMonoBehaviour
{
	[Serializable]
	private struct EffectInfo
	{
		[HashedEnum]
		public SimHashes element;

		public GameObject prefab;
	}

	private struct EffectRuntimeInfo
	{
		public ObjectPool pool;

		public Action<GameObject> releaseFunc;
	}

	private struct SerializedInfo
	{
		public Vector3 position;

		public SimHashes element;

		public float mass;

		public float temperature;

		public int diseaseID;

		public int diseaseCount;
	}

	[SerializeField]
	private Vector3 spawnPuffOffset;

	[SerializeField]
	private Vector3 landEffectOffset;

	private Vector3 fallingTileOffset;

	[SerializeField]
	private EffectInfo[] effects;

	private List<GameObject> fallingObjects = new List<GameObject>();

	private List<int> pendingCells = new List<int>();

	private Dictionary<SimHashes, EffectRuntimeInfo> runtimeInfo = new Dictionary<SimHashes, EffectRuntimeInfo>();

	[Serialize]
	private List<SerializedInfo> serializedInfo;

	protected override void OnPrefabInit()
	{
		fallingTileOffset = new Vector3(0.5f, 0f, 0f);
		EffectInfo[] array = effects;
		for (int i = 0; i < array.Length; i++)
		{
			EffectInfo effectInfo = array[i];
			GameObject prefab = effectInfo.prefab;
			prefab.SetActive(value: false);
			EffectRuntimeInfo value = default(EffectRuntimeInfo);
			ObjectPool pool = new ObjectPool(() => InstantiateObj(prefab), 16);
			value.pool = pool;
			value.releaseFunc = delegate(GameObject go)
			{
				ReleaseGO(go);
				pool.ReleaseInstance(go);
			};
			runtimeInfo[effectInfo.element] = value;
		}
	}

	private void ReleaseGO(GameObject go)
	{
		if (GameComps.Gravities.Has(go))
		{
			GameComps.Gravities.Remove(go);
		}
		go.SetActive(value: false);
	}

	private GameObject InstantiateObj(GameObject prefab)
	{
		GameObject gameObject = GameUtil.KInstantiate(prefab, Grid.SceneLayer.BuildingBack);
		gameObject.SetActive(value: false);
		gameObject.name = "UnstablePool";
		return gameObject;
	}

	public void Spawn(int cell, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Debug.LogError("Tried to spawn unstable ground with NaN temperature");
			temperature = 293f;
		}
		KBatchedAnimController kBatchedAnimController = Spawn(vector, element, mass, temperature, disease_idx, disease_count);
		kBatchedAnimController.Play("start");
		kBatchedAnimController.Play("loop", KAnim.PlayMode.Loop);
		kBatchedAnimController.gameObject.name = "Falling " + element.name;
		GameComps.Gravities.Add(kBatchedAnimController.gameObject, Vector2.zero);
		fallingObjects.Add(kBatchedAnimController.gameObject);
		SpawnPuff(vector, element, mass, temperature, disease_idx, disease_count);
		Substance substance = element.substance;
		if (substance != null && substance.fallingStartSound != null && CameraController.Instance.IsAudibleSound(vector, substance.fallingStartSound))
		{
			SoundEvent.PlayOneShot(substance.fallingStartSound, vector);
		}
	}

	private void SpawnOld(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			Debug.LogError("Spawning falling ground with a stable element");
		}
		KBatchedAnimController kBatchedAnimController = Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		GameComps.Gravities.Add(kBatchedAnimController.gameObject, Vector2.zero);
		kBatchedAnimController.Play("loop", KAnim.PlayMode.Loop);
		fallingObjects.Add(kBatchedAnimController.gameObject);
		kBatchedAnimController.gameObject.name = "SpawnOld " + element.name;
	}

	private void SpawnPuff(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!element.IsUnstable)
		{
			Debug.LogError("Spawning sand puff with a stable element");
		}
		KBatchedAnimController kBatchedAnimController = Spawn(pos, element, mass, temperature, disease_idx, disease_count);
		kBatchedAnimController.Play("sandPuff");
		kBatchedAnimController.gameObject.name = "Puff " + element.name;
		kBatchedAnimController.transform.SetPosition(kBatchedAnimController.transform.GetPosition() + spawnPuffOffset);
	}

	private KBatchedAnimController Spawn(Vector3 pos, Element element, float mass, float temperature, byte disease_idx, int disease_count)
	{
		if (!runtimeInfo.TryGetValue(element.id, out var value))
		{
			Debug.LogError(element.id.ToString() + " needs unstable ground info hookup!");
		}
		GameObject instance = value.pool.GetInstance();
		instance.transform.SetPosition(pos);
		if (float.IsNaN(temperature) || float.IsInfinity(temperature))
		{
			Debug.LogError("Tried to spawn unstable ground with NaN temperature");
			temperature = 293f;
		}
		UnstableGround component = instance.GetComponent<UnstableGround>();
		component.element = element.id;
		component.mass = mass;
		component.temperature = temperature;
		component.diseaseIdx = disease_idx;
		component.diseaseCount = disease_count;
		instance.SetActive(value: true);
		KBatchedAnimController component2 = instance.GetComponent<KBatchedAnimController>();
		component2.onDestroySelf = value.releaseFunc;
		component2.Stop();
		if (element.substance != null)
		{
			component2.TintColour = element.substance.colour;
		}
		return component2;
	}

	public List<int> GetCellsContainingFallingAbove(Vector2I cellXY)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < fallingObjects.Count; i++)
		{
			GameObject gameObject = fallingObjects[i];
			Grid.PosToXY(gameObject.transform.GetPosition(), out var xy);
			if (xy.x == cellXY.x || xy.y >= cellXY.y)
			{
				int item = Grid.PosToCell(xy);
				list.Add(item);
			}
		}
		for (int j = 0; j < pendingCells.Count; j++)
		{
			Vector2I vector2I = Grid.CellToXY(pendingCells[j]);
			if (vector2I.x == cellXY.x || vector2I.y >= cellXY.y)
			{
				list.Add(pendingCells[j]);
			}
		}
		return list;
	}

	private void RemoveFromPending(int cell)
	{
		pendingCells.Remove(cell);
	}

	private void Update()
	{
		if (App.isLoading)
		{
			return;
		}
		int num = 0;
		while (num < fallingObjects.Count)
		{
			GameObject gameObject = fallingObjects[num];
			if (gameObject == null)
			{
				continue;
			}
			Vector3 position = gameObject.transform.GetPosition();
			int cell = Grid.PosToCell(position);
			int num2 = Grid.CellRight(cell);
			int num3 = Grid.CellLeft(cell);
			int num4 = Grid.CellBelow(cell);
			int num5 = Grid.CellRight(num4);
			int num6 = Grid.CellLeft(num4);
			int num7 = cell;
			if (!Grid.IsValidCell(num4) || Grid.Element[num4].IsSolid || (Grid.Properties[num4] & 4u) != 0)
			{
				UnstableGround component = gameObject.GetComponent<UnstableGround>();
				pendingCells.Add(num7);
				SimMessages.AddRemoveSubstance(callbackIdx: Game.Instance.callbackManager.Add(new Game.CallbackInfo(delegate
				{
					RemoveFromPending(cell);
				})).index, gameCell: num7, new_element: component.element, ev: CellEventLogger.Instance.UnstableGround, mass: component.mass, temperature: component.temperature, disease_idx: component.diseaseIdx, disease_count: component.diseaseCount);
				ListPool<ScenePartitionerEntry, UnstableGroundManager>.PooledList pooledList = ListPool<ScenePartitionerEntry, UnstableGroundManager>.Allocate();
				Vector2I vector2I = Grid.CellToXY(cell);
				vector2I.x = Mathf.Max(0, vector2I.x - 1);
				vector2I.y = Mathf.Min(Grid.HeightInCells - 1, vector2I.y + 1);
				GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 3, 3, GameScenePartitioner.Instance.collisionLayer, pooledList);
				foreach (ScenePartitionerEntry item in pooledList)
				{
					if (item.obj is KCollider2D)
					{
						GameObject gameObject2 = (item.obj as KCollider2D).gameObject;
						gameObject2.Trigger(-975551167);
					}
				}
				pooledList.Recycle();
				Element element = ElementLoader.FindElementByHash(component.element);
				if (element != null && element.substance != null && element.substance.fallingStopSound != null && CameraController.Instance.IsAudibleSound(position, element.substance.fallingStopSound))
				{
					SoundEvent.PlayOneShot(element.substance.fallingStopSound, position);
				}
				GameObject gameObject3 = GameUtil.KInstantiate(Assets.GetPrefab(EffectConfigs.OreAbsorbId), position + landEffectOffset, Grid.SceneLayer.Front);
				gameObject3.SetActive(value: true);
				fallingObjects[num] = fallingObjects[fallingObjects.Count - 1];
				fallingObjects.RemoveAt(fallingObjects.Count - 1);
				ReleaseGO(gameObject);
			}
			else
			{
				num++;
			}
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (fallingObjects.Count > 0)
		{
			serializedInfo = new List<SerializedInfo>();
		}
		foreach (GameObject fallingObject in fallingObjects)
		{
			UnstableGround component = fallingObject.GetComponent<UnstableGround>();
			byte diseaseIdx = component.diseaseIdx;
			int diseaseID = ((diseaseIdx != byte.MaxValue) ? Db.Get().Diseases[diseaseIdx].id.HashValue : 0);
			serializedInfo.Add(new SerializedInfo
			{
				position = fallingObject.transform.GetPosition(),
				element = component.element,
				mass = component.mass,
				temperature = component.temperature,
				diseaseID = diseaseID,
				diseaseCount = component.diseaseCount
			});
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		serializedInfo = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (serializedInfo == null)
		{
			return;
		}
		fallingObjects.Clear();
		HashedString id = default(HashedString);
		foreach (SerializedInfo item in serializedInfo)
		{
			Element element = ElementLoader.FindElementByHash(item.element);
			id.HashValue = item.diseaseID;
			byte index = Db.Get().Diseases.GetIndex(id);
			int disease_count = item.diseaseCount;
			if (index == byte.MaxValue)
			{
				disease_count = 0;
			}
			SpawnOld(item.position, element, item.mass, item.temperature, index, disease_count);
		}
	}
}
