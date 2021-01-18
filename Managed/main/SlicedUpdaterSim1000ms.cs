using System.Collections.Generic;
using KSerialization;

public abstract class SlicedUpdaterSim1000ms<T> : KMonoBehaviour, ISim200ms where T : KMonoBehaviour, ISlicedSim1000ms
{
	private class Slice
	{
		private float m_timeSinceLastUpdate = 0f;

		private List<T> m_updateList = new List<T>();

		private Dictionary<T, float> m_recentlyAdded = new Dictionary<T, float>();

		public int Count => m_updateList.Count + m_recentlyAdded.Count;

		public void Register(T toBeUpdated)
		{
			if (m_timeSinceLastUpdate == 0f)
			{
				m_updateList.Add(toBeUpdated);
			}
			else
			{
				m_recentlyAdded[toBeUpdated] = 0f;
			}
		}

		public void Unregister(T toBeUpdated)
		{
			if (!m_updateList.Remove(toBeUpdated))
			{
				m_recentlyAdded.Remove(toBeUpdated);
			}
		}

		public List<T> GetUpdateList()
		{
			List<T> list = new List<T>();
			list.AddRange(m_updateList);
			list.AddRange(m_recentlyAdded.Keys);
			return list;
		}

		public void Update()
		{
			foreach (T update in m_updateList)
			{
				update.SlicedSim1000ms(m_timeSinceLastUpdate);
			}
			foreach (KeyValuePair<T, float> item in m_recentlyAdded)
			{
				item.Key.SlicedSim1000ms(item.Value);
				m_updateList.Add(item.Key);
			}
			m_recentlyAdded.Clear();
			m_timeSinceLastUpdate = 0f;
		}

		public void IncrementDt(float dt)
		{
			m_timeSinceLastUpdate += dt;
			if (m_recentlyAdded.Count <= 0)
			{
				return;
			}
			List<T> list = new List<T>(m_recentlyAdded.Keys);
			foreach (T item in list)
			{
				m_recentlyAdded[item] += dt;
			}
		}
	}

	private static int NUM_200MS_BUCKETS = 5;

	public static SlicedUpdaterSim1000ms<T> instance;

	[Serialize]
	public int maxUpdatesPer200ms = 300;

	[Serialize]
	public int numSlicesPer200ms = 3;

	private List<Slice> m_slices;

	private int m_nextSliceIdx = 0;

	protected override void OnPrefabInit()
	{
		InitializeSlices();
		base.OnPrefabInit();
		instance = this;
	}

	private void InitializeSlices()
	{
		int num = NUM_200MS_BUCKETS * numSlicesPer200ms;
		m_slices = new List<Slice>();
		for (int i = 0; i < num; i++)
		{
			m_slices.Add(new Slice());
		}
		m_nextSliceIdx = 0;
	}

	private int GetSliceIdx(T toBeUpdated)
	{
		KPrefabID component = toBeUpdated.GetComponent<KPrefabID>();
		return component.InstanceID % m_slices.Count;
	}

	public void RegisterUpdate1000ms(T toBeUpdated)
	{
		Slice slice = m_slices[GetSliceIdx(toBeUpdated)];
		slice.Register(toBeUpdated);
		DebugUtil.DevAssert(slice.Count < maxUpdatesPer200ms, $"The SlicedUpdaterSim1000ms for {typeof(T).Name} wants to update no more than {maxUpdatesPer200ms} instances per 200ms tick, but a slice has grown more than the SlicedUpdaterSim1000ms can support.");
	}

	public void UnregisterUpdate1000ms(T toBeUpdated)
	{
		m_slices[GetSliceIdx(toBeUpdated)].Unregister(toBeUpdated);
	}

	public void Sim200ms(float dt)
	{
		foreach (Slice slice2 in m_slices)
		{
			slice2.IncrementDt(dt);
		}
		int num = 0;
		int num2 = 0;
		while (num2 < numSlicesPer200ms)
		{
			Slice slice = m_slices[m_nextSliceIdx];
			num += slice.Count;
			if (num > maxUpdatesPer200ms && num2 > 0)
			{
				break;
			}
			slice.Update();
			num2++;
			m_nextSliceIdx = (m_nextSliceIdx + 1) % m_slices.Count;
		}
	}
}
