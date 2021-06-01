using System;
using UnityEngine;

public class OreSizeVisualizerComponents : KGameObjectComponentManager<OreSizeVisualizerData>
{
	private struct MassTier
	{
		public HashedString animName;

		public float massRequired;

		public float colliderRadius;
	}

	private static readonly MassTier[] MassTiers;

	public HandleVector<int>.Handle Add(GameObject go)
	{
		HandleVector<int>.Handle handle = Add(go, new OreSizeVisualizerData(go));
		OnPrefabInit(handle);
		return handle;
	}

	protected override void OnPrefabInit(HandleVector<int>.Handle handle)
	{
		Action<object> action = delegate(object ev_data)
		{
			OnMassChanged(handle, ev_data);
		};
		OreSizeVisualizerData data = GetData(handle);
		data.onMassChangedCB = action;
		data.primaryElement.Subscribe(-2064133523, action);
		data.primaryElement.Subscribe(1335436905, action);
		SetData(handle, data);
	}

	protected override void OnSpawn(HandleVector<int>.Handle handle)
	{
		OnMassChanged(handle, GetData(handle).primaryElement.GetComponent<Pickupable>());
	}

	protected override void OnCleanUp(HandleVector<int>.Handle handle)
	{
		OreSizeVisualizerData data = GetData(handle);
		if (data.primaryElement != null)
		{
			Action<object> onMassChangedCB = data.onMassChangedCB;
			data.primaryElement.Unsubscribe(-2064133523, onMassChangedCB);
			data.primaryElement.Unsubscribe(1335436905, onMassChangedCB);
		}
	}

	private static void OnMassChanged(HandleVector<int>.Handle handle, object other_data)
	{
		PrimaryElement primaryElement = GameComps.OreSizeVisualizers.GetData(handle).primaryElement;
		float num = primaryElement.Mass;
		if (other_data != null)
		{
			Pickupable pickupable = (Pickupable)other_data;
			PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
			num += component.Mass;
		}
		MassTier massTier = default(MassTier);
		for (int i = 0; i < MassTiers.Length; i++)
		{
			if (num <= MassTiers[i].massRequired)
			{
				massTier = MassTiers[i];
				break;
			}
		}
		KBatchedAnimController component2 = primaryElement.GetComponent<KBatchedAnimController>();
		component2.Play(massTier.animName);
		KCircleCollider2D component3 = primaryElement.GetComponent<KCircleCollider2D>();
		if (component3 != null)
		{
			component3.radius = massTier.colliderRadius;
		}
		primaryElement.Trigger(1807976145);
	}

	static OreSizeVisualizerComponents()
	{
		MassTier[] array = new MassTier[3];
		MassTier massTier = new MassTier
		{
			animName = "idle1",
			massRequired = 50f,
			colliderRadius = 0.15f
		};
		array[0] = massTier;
		massTier = new MassTier
		{
			animName = "idle2",
			massRequired = 600f,
			colliderRadius = 0.2f
		};
		array[1] = massTier;
		massTier = new MassTier
		{
			animName = "idle3",
			massRequired = float.MaxValue,
			colliderRadius = 0.25f
		};
		array[2] = massTier;
		MassTiers = array;
	}
}