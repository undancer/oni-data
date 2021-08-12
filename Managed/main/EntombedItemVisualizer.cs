using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntombedItemVisualizer")]
public class EntombedItemVisualizer : KMonoBehaviour
{
	private struct Data
	{
		public int refCount;

		public KBatchedAnimController controller;
	}

	[SerializeField]
	private GameObject entombedItemPrefab;

	private static readonly string[] EntombedVisualizerAnims = new string[4] { "idle1", "idle2", "idle3", "idle4" };

	private ObjectPool entombedItemPool;

	private Dictionary<int, Data> cellEntombedCounts = new Dictionary<int, Data>();

	public void Clear()
	{
		cellEntombedCounts.Clear();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		entombedItemPool = new ObjectPool(InstantiateEntombedObject, 32);
	}

	public bool AddItem(int cell)
	{
		bool result = false;
		if (Grid.Objects[cell, 9] == null)
		{
			result = true;
			cellEntombedCounts.TryGetValue(cell, out var value);
			if (value.refCount == 0)
			{
				GameObject instance = entombedItemPool.GetInstance();
				instance.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront));
				instance.transform.rotation = Quaternion.Euler(0f, 0f, Random.value * 360f);
				KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
				int num = Random.Range(0, EntombedVisualizerAnims.Length);
				string text = (component.initialAnim = EntombedVisualizerAnims[num]);
				instance.SetActive(value: true);
				component.Play(text);
				value.controller = component;
			}
			value.refCount++;
			cellEntombedCounts[cell] = value;
		}
		return result;
	}

	public void RemoveItem(int cell)
	{
		if (cellEntombedCounts.TryGetValue(cell, out var value))
		{
			value.refCount--;
			if (value.refCount == 0)
			{
				ReleaseVisualizer(cell, value);
			}
			else
			{
				cellEntombedCounts[cell] = value;
			}
		}
	}

	public void ForceClear(int cell)
	{
		if (cellEntombedCounts.TryGetValue(cell, out var value))
		{
			ReleaseVisualizer(cell, value);
		}
	}

	private void ReleaseVisualizer(int cell, Data data)
	{
		if (data.controller != null)
		{
			data.controller.gameObject.SetActive(value: false);
			entombedItemPool.ReleaseInstance(data.controller.gameObject);
		}
		cellEntombedCounts.Remove(cell);
	}

	public bool IsEntombedItem(int cell)
	{
		if (cellEntombedCounts.ContainsKey(cell))
		{
			return cellEntombedCounts[cell].refCount > 0;
		}
		return false;
	}

	private GameObject InstantiateEntombedObject()
	{
		GameObject obj = GameUtil.KInstantiate(entombedItemPrefab, Grid.SceneLayer.FXFront);
		obj.SetActive(value: false);
		return obj;
	}
}
