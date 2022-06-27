using System;
using Klei;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/EntitySplitter")]
public class EntitySplitter : KMonoBehaviour
{
	public float maxStackSize = PrimaryElement.MAX_MASS;

	private static readonly EventSystem.IntraObjectHandler<EntitySplitter> OnAbsorbDelegate = new EventSystem.IntraObjectHandler<EntitySplitter>(delegate(EntitySplitter component, object data)
	{
		component.OnAbsorb(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Pickupable pickupable = GetComponent<Pickupable>();
		if (pickupable == null)
		{
			Debug.LogError(base.name + " does not have a pickupable component!");
		}
		Pickupable pickupable2 = pickupable;
		pickupable2.OnTake = (Func<float, Pickupable>)Delegate.Combine(pickupable2.OnTake, (Func<float, Pickupable>)((float amount) => Split(pickupable, amount)));
		Rottable.Instance rottable = base.gameObject.GetSMI<Rottable.Instance>();
		pickupable.absorbable = true;
		pickupable.CanAbsorb = (Pickupable other) => CanFirstAbsorbSecond(pickupable, rottable, other, maxStackSize);
		Subscribe(-2064133523, OnAbsorbDelegate);
	}

	private static bool CanFirstAbsorbSecond(Pickupable pickupable, Rottable.Instance rottable, Pickupable other, float maxStackSize)
	{
		if (other == null)
		{
			return false;
		}
		KPrefabID component = pickupable.GetComponent<KPrefabID>();
		KPrefabID component2 = other.GetComponent<KPrefabID>();
		if (component == null)
		{
			return false;
		}
		if (component2 == null)
		{
			return false;
		}
		if (component.PrefabTag != component2.PrefabTag)
		{
			return false;
		}
		if (pickupable.TotalAmount + other.TotalAmount > maxStackSize)
		{
			return false;
		}
		if (pickupable.PrimaryElement.Mass + other.PrimaryElement.Mass > maxStackSize)
		{
			return false;
		}
		if (rottable != null)
		{
			Rottable.Instance sMI = other.GetSMI<Rottable.Instance>();
			if (sMI == null)
			{
				return false;
			}
			if (!rottable.IsRotLevelStackable(sMI))
			{
				return false;
			}
		}
		if (pickupable.HasTag(GameTags.Seed) || pickupable.HasTag(GameTags.CropSeed) || pickupable.HasTag(GameTags.Compostable))
		{
			MutantPlant component3 = pickupable.GetComponent<MutantPlant>();
			MutantPlant component4 = other.GetComponent<MutantPlant>();
			if (component3 != null || component4 != null)
			{
				if (component3 == null != (component4 == null))
				{
					return false;
				}
				if (component3.HasTag(GameTags.UnidentifiedSeed) != component4.HasTag(GameTags.UnidentifiedSeed))
				{
					return false;
				}
				if (component3.SubSpeciesID != component4.SubSpeciesID)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static Pickupable Split(Pickupable pickupable, float amount, GameObject prefab = null)
	{
		if (amount >= pickupable.TotalAmount && prefab == null)
		{
			return pickupable;
		}
		Storage storage = pickupable.storage;
		if (prefab == null)
		{
			prefab = Assets.GetPrefab(pickupable.GetComponent<KPrefabID>().PrefabTag);
		}
		GameObject parent = null;
		if (pickupable.transform.parent != null)
		{
			parent = pickupable.transform.parent.gameObject;
		}
		GameObject gameObject = GameUtil.KInstantiate(prefab, pickupable.transform.GetPosition(), Grid.SceneLayer.Ore, parent);
		Debug.Assert(gameObject != null, "WTH, the GO is null, shouldn't happen on instantiate");
		Pickupable component = gameObject.GetComponent<Pickupable>();
		if (component == null)
		{
			Debug.LogError("Edible::OnTake() No Pickupable component for " + gameObject.name, gameObject);
		}
		gameObject.SetActive(value: true);
		component.TotalAmount = Mathf.Min(amount, pickupable.TotalAmount);
		component.PrimaryElement.Temperature = pickupable.PrimaryElement.Temperature;
		bool keepZeroMassObject = pickupable.PrimaryElement.KeepZeroMassObject;
		pickupable.PrimaryElement.KeepZeroMassObject = true;
		pickupable.TotalAmount -= amount;
		component.Trigger(1335436905, pickupable);
		pickupable.PrimaryElement.KeepZeroMassObject = keepZeroMassObject;
		pickupable.TotalAmount += 0f;
		if (storage != null)
		{
			storage.Trigger(-1697596308, pickupable.gameObject);
			storage.Trigger(-778359855, storage);
		}
		return component;
	}

	private void OnAbsorb(object data)
	{
		Pickupable pickupable = (Pickupable)data;
		if (!(pickupable != null))
		{
			return;
		}
		PrimaryElement component = GetComponent<PrimaryElement>();
		PrimaryElement primaryElement = pickupable.PrimaryElement;
		if (!(primaryElement != null))
		{
			return;
		}
		float temperature = 0f;
		float mass = component.Mass;
		float mass2 = primaryElement.Mass;
		if (mass > 0f && mass2 > 0f)
		{
			temperature = SimUtil.CalculateFinalTemperature(mass, component.Temperature, mass2, primaryElement.Temperature);
		}
		else if (primaryElement.Mass > 0f)
		{
			temperature = primaryElement.Temperature;
		}
		component.SetMassTemperature(mass + mass2, temperature);
		if (CameraController.Instance != null)
		{
			string sound = GlobalAssets.GetSound("Ore_absorb");
			Vector3 position = pickupable.transform.GetPosition();
			position.z = 0f;
			if (sound != null && CameraController.Instance.IsAudibleSound(position, sound))
			{
				KFMOD.PlayOneShot(sound, position);
			}
		}
	}
}
