using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EventSystem
{
	private struct Entry
	{
		public Action<object> handler;

		public int hash;

		public int id;

		public Entry(int hash, Action<object> handler, int id)
		{
			this.handler = handler;
			this.hash = hash;
			this.id = id;
		}
	}

	private struct SubscribedEntry
	{
		public Action<object> handler;

		public int hash;

		public GameObject go;

		public SubscribedEntry(GameObject go, int hash, Action<object> handler)
		{
			this.go = go;
			this.hash = hash;
			this.handler = handler;
		}
	}

	private struct IntraObjectRoute
	{
		public int eventHash;

		public int handlerIndex;

		public IntraObjectRoute(int eventHash, int handlerIndex)
		{
			this.eventHash = eventHash;
			this.handlerIndex = handlerIndex;
		}

		public bool IsValid()
		{
			return eventHash != 0;
		}
	}

	public abstract class IntraObjectHandlerBase
	{
		public abstract void Trigger(GameObject gameObject, object eventData);
	}

	public class IntraObjectHandler<ComponentType> : IntraObjectHandlerBase where ComponentType : Component
	{
		private Action<ComponentType, object> handler;

		public static bool IsStatic(Delegate del)
		{
			if (del.Target != null)
			{
				return del.Target.GetType().GetCustomAttributes(inherit: false).OfType<CompilerGeneratedAttribute>()
					.Any();
			}
			return true;
		}

		public IntraObjectHandler(Action<ComponentType, object> handler)
		{
			Debug.Assert(IsStatic(handler), "IntraObjectHandler method must be static to avoid allocations");
			this.handler = handler;
		}

		public static implicit operator IntraObjectHandler<ComponentType>(Action<ComponentType, object> handler)
		{
			return new IntraObjectHandler<ComponentType>(handler);
		}

		public override void Trigger(GameObject gameObject, object eventData)
		{
			ListPool<ComponentType, IntraObjectHandler<ComponentType>>.PooledList pooledList = ListPool<ComponentType, IntraObjectHandler<ComponentType>>.Allocate();
			gameObject.GetComponents(pooledList);
			foreach (ComponentType item in pooledList)
			{
				handler(item, eventData);
			}
			pooledList.Recycle();
		}

		public override string ToString()
		{
			return ((handler.Target != null) ? handler.Target.GetType().ToString() : "STATIC") + "." + handler.Method.ToString();
		}
	}

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private ArrayRef<SubscribedEntry> subscribedEvents;

	private Dictionary<int, List<Entry>> entryMap = new Dictionary<int, List<Entry>>();

	private Dictionary<int, int> idMap = new Dictionary<int, int>();

	private ArrayRef<IntraObjectRoute> intraObjectRoutes;

	private static Dictionary<int, List<IntraObjectHandlerBase>> intraObjectDispatcher = new Dictionary<int, List<IntraObjectHandlerBase>>();

	private LoggerFIO log;

	public EventSystem()
	{
		log = new LoggerFIO("Events");
	}

	public void Trigger(GameObject go, int hash, object data = null)
	{
		if (App.IsExiting)
		{
			return;
		}
		currentlyTriggering++;
		for (int i = 0; i != intraObjectRoutes.size; i++)
		{
			if (intraObjectRoutes[i].eventHash == hash)
			{
				intraObjectDispatcher[hash][intraObjectRoutes[i].handlerIndex].Trigger(go, data);
			}
		}
		if (entryMap.TryGetValue(hash, out var value))
		{
			int count = value.Count;
			for (int j = 0; j < count; j++)
			{
				if (value[j].hash == hash && value[j].handler != null)
				{
					value[j].handler(data);
				}
			}
		}
		currentlyTriggering--;
		if (!dirty || currentlyTriggering != 0)
		{
			return;
		}
		dirty = false;
		if (entryMap.TryGetValue(hash, out var value2))
		{
			value2.RemoveAll((Entry x) => x.handler == null);
		}
		intraObjectRoutes.RemoveAllSwap((IntraObjectRoute route) => !route.IsValid());
	}

	public void OnCleanUp()
	{
		for (int num = subscribedEvents.size - 1; num >= 0; num--)
		{
			SubscribedEntry subscribedEntry = subscribedEvents[num];
			if (subscribedEntry.go != null)
			{
				Unsubscribe(subscribedEntry.go, subscribedEntry.hash, subscribedEntry.handler);
			}
		}
		foreach (KeyValuePair<int, List<Entry>> item in entryMap)
		{
			List<Entry> value = item.Value;
			for (int i = 0; i < value.Count; i++)
			{
				Entry value2 = value[i];
				value2.handler = null;
				value[i] = value2;
			}
			value.Clear();
		}
		entryMap.Clear();
		subscribedEvents.Clear();
		intraObjectRoutes.Clear();
	}

	public void UnregisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		for (int i = 0; i < subscribedEvents.size; i++)
		{
			if (subscribedEvents[i].hash == eventName && subscribedEvents[i].handler == handler && subscribedEvents[i].go == target)
			{
				subscribedEvents.RemoveAt(i);
				break;
			}
		}
	}

	public void RegisterEvent(GameObject target, int eventName, Action<object> handler)
	{
		subscribedEvents.Add(new SubscribedEntry(target, eventName, handler));
	}

	public int Subscribe(int hash, Action<object> handler)
	{
		Entry item = new Entry(hash, handler, ++nextId);
		if (!entryMap.TryGetValue(hash, out var value))
		{
			value = new List<Entry>();
			entryMap.Add(hash, value);
		}
		value.Add(item);
		idMap.Add(item.id, item.hash);
		return nextId;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		if (!entryMap.TryGetValue(hash, out var value))
		{
			return;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (value[i].hash == hash && value[i].handler == handler)
			{
				if (currentlyTriggering == 0)
				{
					value.RemoveAt(i);
					break;
				}
				dirty = true;
				Entry value2 = value[i];
				value2.handler = null;
				value[i] = value2;
				break;
			}
		}
	}

	public void Unsubscribe(int id)
	{
		int value = -1;
		if (idMap.TryGetValue(id, out value) && entryMap.TryGetValue(value, out var value2))
		{
			for (int i = 0; i < value2.Count; i++)
			{
				if (value2[i].id == id)
				{
					if (currentlyTriggering == 0)
					{
						value2.RemoveAt(i);
						break;
					}
					dirty = true;
					Entry value3 = value2[i];
					value3.handler = null;
					value2[i] = value3;
					break;
				}
			}
		}
		idMap.Remove(id);
	}

	public int Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		RegisterEvent(target, eventName, handler);
		return KObjectManager.Instance.GetOrCreateObject(target).GetEventSystem().Subscribe(eventName, handler);
	}

	public int Subscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler) where ComponentType : Component
	{
		if (!intraObjectDispatcher.TryGetValue(eventName, out var value))
		{
			value = new List<IntraObjectHandlerBase>();
			intraObjectDispatcher.Add(eventName, value);
		}
		int num = value.IndexOf(handler);
		if (num == -1)
		{
			value.Add(handler);
			num = value.Count - 1;
		}
		intraObjectRoutes.Add(new IntraObjectRoute(eventName, num));
		return num;
	}

	public void Unsubscribe(GameObject target, int eventName, Action<object> handler)
	{
		UnregisterEvent(target, eventName, handler);
		if (!(target == null))
		{
			KObjectManager.Instance.GetOrCreateObject(target).GetEventSystem().Unsubscribe(eventName, handler);
		}
	}

	public void Unsubscribe(int eventName, int subscribeHandle, bool suppressWarnings = false)
	{
		int num = intraObjectRoutes.FindIndex((IntraObjectRoute route) => route.eventHash == eventName && route.handlerIndex == subscribeHandle);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarning("Failed to Unsubscribe event handler: " + intraObjectDispatcher[eventName][subscribeHandle].ToString() + "\nNot subscribed to event");
			}
		}
		else if (currentlyTriggering == 0)
		{
			intraObjectRoutes.RemoveAtSwap(num);
		}
		else
		{
			dirty = true;
			intraObjectRoutes[num] = default(IntraObjectRoute);
		}
	}

	public void Unsubscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler, bool suppressWarnings) where ComponentType : Component
	{
		if (!intraObjectDispatcher.TryGetValue(eventName, out var value))
		{
			if (!suppressWarnings)
			{
				Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNo subscriptions have been made to event");
			}
			return;
		}
		int num = value.IndexOf(handler);
		if (num == -1)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarning("Failed to Unsubscribe event handler: " + handler.ToString() + "\nNot subscribed to event");
			}
		}
		else
		{
			Unsubscribe(eventName, num, suppressWarnings);
		}
	}

	public void Unsubscribe(string[] eventNames, Action<object> handler)
	{
		for (int i = 0; i < eventNames.Length; i++)
		{
			int hash = Hash.SDBMLower(eventNames[i]);
			Unsubscribe(hash, handler);
		}
	}

	[Conditional("ENABLE_DETAILED_EVENT_PROFILE_INFO")]
	private static void BeginDetailedSample(string region_name)
	{
	}

	[Conditional("ENABLE_DETAILED_EVENT_PROFILE_INFO")]
	private static void EndDetailedSample(string region_name)
	{
	}
}
