using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : KGameObjectComponentManager<WhiteBoard.Data>, IKComponentManager
{
	public struct Data
	{
		public Dictionary<HashedString, object> keyValueStore;
	}

	public HandleVector<int>.Handle Add(GameObject go)
	{
		Data cmp = default(Data);
		cmp.keyValueStore = new Dictionary<HashedString, object>();
		return Add(go, cmp);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		Data new_data = GetData(h);
		new_data.keyValueStore.Clear();
		new_data.keyValueStore = null;
		SetData(h, new_data);
	}

	public bool HasValue(HandleVector<int>.Handle h, HashedString key)
	{
		if (!h.IsValid())
		{
			return false;
		}
		return GetData(h).keyValueStore.ContainsKey(key);
	}

	public object GetValue(HandleVector<int>.Handle h, HashedString key)
	{
		return GetData(h).keyValueStore[key];
	}

	public void SetValue(HandleVector<int>.Handle h, HashedString key, object value)
	{
		if (h.IsValid())
		{
			Data new_data = GetData(h);
			new_data.keyValueStore[key] = value;
			SetData(h, new_data);
		}
	}

	public void RemoveValue(HandleVector<int>.Handle h, HashedString key)
	{
		if (h.IsValid())
		{
			Data new_data = GetData(h);
			new_data.keyValueStore.Remove(key);
			SetData(h, new_data);
		}
	}
}
