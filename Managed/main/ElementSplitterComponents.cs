using System;
using UnityEngine;

public class ElementSplitterComponents : KGameObjectComponentManager<ElementSplitter>
{
	private const float MAX_STACK_SIZE = 25000f;

	public HandleVector<int>.Handle Add(GameObject go)
	{
		return Add(go, new ElementSplitter(go));
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		ElementSplitter new_data = GetData(handle);
		Pickupable component = new_data.primaryElement.GetComponent<Pickupable>();
		Func<float, Pickupable> func = (float amount) => OnTake(handle, amount);
		component.OnTake = (Func<float, Pickupable>)Delegate.Combine(component.OnTake, func);
		Func<Pickupable, bool> func2 = delegate(Pickupable other)
		{
			HandleVector<int>.Handle handle2 = GetHandle(other.gameObject);
			return CanFirstAbsorbSecond(handle, handle2);
		};
		component.CanAbsorb = (Func<Pickupable, bool>)Delegate.Combine(component.CanAbsorb, func2);
		component.absorbable = true;
		new_data.onTakeCB = func;
		new_data.canAbsorbCB = func2;
		SetData(handle, new_data);
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		ElementSplitter elementSplitter = GetData(handle);
		if (elementSplitter.primaryElement != null)
		{
			Pickupable component = elementSplitter.primaryElement.GetComponent<Pickupable>();
			if (component != null)
			{
				component.OnTake = (Func<float, Pickupable>)Delegate.Remove(component.OnTake, elementSplitter.onTakeCB);
				component.CanAbsorb = (Func<Pickupable, bool>)Delegate.Remove(component.CanAbsorb, elementSplitter.canAbsorbCB);
			}
		}
	}

	private static bool CanFirstAbsorbSecond(HandleVector<int>.Handle first, HandleVector<int>.Handle second)
	{
		if (first == HandleVector<int>.InvalidHandle || second == HandleVector<int>.InvalidHandle)
		{
			return false;
		}
		ElementSplitter elementSplitter = GameComps.ElementSplitters.GetData(first);
		ElementSplitter elementSplitter2 = GameComps.ElementSplitters.GetData(second);
		if (elementSplitter.primaryElement.ElementID == elementSplitter2.primaryElement.ElementID)
		{
			return elementSplitter.primaryElement.Units + elementSplitter2.primaryElement.Units < 25000f;
		}
		return false;
	}

	private static Pickupable OnTake(HandleVector<int>.Handle handle, float amount)
	{
		ElementSplitter elementSplitter = GameComps.ElementSplitters.GetData(handle);
		Pickupable component = elementSplitter.primaryElement.GetComponent<Pickupable>();
		Pickupable pickupable = component;
		Storage storage = component.storage;
		PrimaryElement component2 = component.GetComponent<PrimaryElement>();
		pickupable = component2.Element.substance.SpawnResource(component.transform.GetPosition(), amount, component2.Temperature, byte.MaxValue, 0, prevent_merge: true).GetComponent<Pickupable>();
		component.TotalAmount -= amount;
		pickupable.Trigger(1335436905, component);
		CopyRenderSettings(component.GetComponent<KBatchedAnimController>(), pickupable.GetComponent<KBatchedAnimController>());
		if (storage != null)
		{
			storage.Trigger(-1697596308, elementSplitter.primaryElement.gameObject);
			storage.Trigger(-778359855, storage);
		}
		return pickupable;
	}

	private static void CopyRenderSettings(KBatchedAnimController src, KBatchedAnimController dest)
	{
		if (src != null && dest != null)
		{
			dest.OverlayColour = src.OverlayColour;
		}
	}
}
