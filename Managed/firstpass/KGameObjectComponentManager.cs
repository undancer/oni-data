using UnityEngine;

public abstract class KGameObjectComponentManager<T> : KComponentManager<T> where T : new()
{
	public HandleVector<int>.Handle Add(GameObject go, T cmp)
	{
		return InternalAddComponent(go, cmp);
	}

	public virtual void Remove(GameObject go)
	{
		HandleVector<int>.Handle handle = GetHandle(go);
		CleanupInfo info = new CleanupInfo(go, handle);
		if (!KComponentCleanUp.InCleanUpPhase)
		{
			AddToCleanupList(info);
			return;
		}
		RemoveFromCleanupList(go);
		OnCleanUp(handle);
		InternalRemoveComponent(info);
	}

	public HandleVector<int>.Handle GetHandle(GameObject obj)
	{
		return GetHandle((object)obj);
	}

	public HandleVector<int>.Handle GetHandle(MonoBehaviour obj)
	{
		return GetHandle((object)obj.gameObject);
	}
}
