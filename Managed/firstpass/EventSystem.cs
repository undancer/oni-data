using System;
using System.Collections.Generic;
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

	public class IntraObjectHandler<ComponentType> : IntraObjectHandlerBase
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
			Debug.Assert(IsStatic(handler));
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

	private static bool ENABLE_DETAILED_EVENT_PROFILE_INFO = false;

	private int nextId;

	private int currentlyTriggering;

	private bool dirty;

	private ArrayRef<SubscribedEntry> subscribedEvents;

	private ArrayRef<Entry> entries;

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
		int size = entries.size;
		if (ENABLE_DETAILED_EVENT_PROFILE_INFO)
		{
			for (int j = 0; j < size; j++)
			{
				if (entries[j].hash == hash && entries[j].handler != null)
				{
					entries[j].handler(data);
				}
			}
		}
		else
		{
			for (int k = 0; k < size; k++)
			{
				if (entries[k].hash == hash && entries[k].handler != null)
				{
					entries[k].handler(data);
				}
			}
		}
		currentlyTriggering--;
		if (dirty && currentlyTriggering == 0)
		{
			dirty = false;
			entries.RemoveAllSwap((Entry x) => x.handler == null);
			intraObjectRoutes.RemoveAllSwap((IntraObjectRoute route) => !route.IsValid());
		}
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
		for (int i = 0; i < entries.size; i++)
		{
			Entry value = entries[i];
			value.handler = null;
			entries[i] = value;
		}
		entries.Clear();
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
		entries.Add(new Entry(hash, handler, ++nextId));
		return nextId;
	}

	public void Unsubscribe(int hash, Action<object> handler)
	{
		for (int i = 0; i < entries.size; i++)
		{
			if (entries[i].hash == hash && entries[i].handler == handler)
			{
				if (currentlyTriggering == 0)
				{
					entries.RemoveAt(i);
					break;
				}
				dirty = true;
				Entry value = entries[i];
				value.handler = null;
				entries[i] = value;
				break;
			}
		}
	}

	public void Unsubscribe(int id)
	{
		for (int i = 0; i < entries.size; i++)
		{
			if (entries[i].id == id)
			{
				if (currentlyTriggering == 0)
				{
					entries.RemoveAt(i);
					break;
				}
				dirty = true;
				Entry value = entries[i];
				value.handler = null;
				entries[i] = value;
				break;
			}
		}
	}

	public int Subscribe(GameObject target, int eventName, Action<object> handler)
	{
		RegisterEvent(target, eventName, handler);
		return KObjectManager.Instance.GetOrCreateObject(target).GetEventSystem().Subscribe(eventName, handler);
	}

	public int Subscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler)
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

	public void Unsubscribe<ComponentType>(int eventName, IntraObjectHandler<ComponentType> handler, bool suppressWarnings)
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
}
