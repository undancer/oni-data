using UnityEngine;

public class RocketConduitStorageAccess : KMonoBehaviour, ISim200ms
{
	[SerializeField]
	public Storage storage;

	[SerializeField]
	public float targetLevel;

	[SerializeField]
	public CargoBay.CargoType cargoType;

	[MyCmpGet]
	private Filterable filterable;

	[MyCmpGet]
	private Operational operational;

	private const float TOLERANCE = 0.01f;

	private CraftModuleInterface craftModuleInterface;

	protected override void OnSpawn()
	{
		WorldContainer myWorld = this.GetMyWorld();
		craftModuleInterface = myWorld.GetComponent<CraftModuleInterface>();
	}

	public void Sim200ms(float dt)
	{
		if (operational != null && !operational.IsOperational)
		{
			return;
		}
		float num = storage.MassStored();
		if (!(num < targetLevel - 0.01f) && !(num > targetLevel + 0.01f))
		{
			return;
		}
		if (operational != null)
		{
			operational.SetActive(value: true);
		}
		float num2 = targetLevel - num;
		foreach (Ref<RocketModuleCluster> clusterModule in craftModuleInterface.ClusterModules)
		{
			CargoBayCluster component = clusterModule.Get().GetComponent<CargoBayCluster>();
			if (!(component != null) || component.storageType != cargoType)
			{
				continue;
			}
			if (num2 > 0f && component.storage.MassStored() > 0f)
			{
				for (int num3 = component.storage.items.Count - 1; num3 >= 0; num3--)
				{
					GameObject gameObject = component.storage.items[num3];
					if (!(filterable != null) || !(filterable.SelectedTag != GameTags.Void) || !(gameObject.PrefabID() != filterable.SelectedTag))
					{
						Pickupable pickupable = gameObject.GetComponent<Pickupable>().Take(num2);
						if (pickupable != null)
						{
							num2 -= pickupable.PrimaryElement.Mass;
							storage.Store(pickupable.gameObject, hide_popups: true);
						}
						if (num2 <= 0f)
						{
							break;
						}
					}
				}
				if (num2 <= 0f)
				{
					break;
				}
			}
			if (!(num2 < 0f) || !(component.storage.RemainingCapacity() > 0f))
			{
				continue;
			}
			Mathf.Min(0f - num2, component.storage.RemainingCapacity());
			for (int num4 = storage.items.Count - 1; num4 >= 0; num4--)
			{
				Pickupable pickupable2 = storage.items[num4].GetComponent<Pickupable>().Take(0f - num2);
				if (pickupable2 != null)
				{
					num2 += pickupable2.PrimaryElement.Mass;
					component.storage.Store(pickupable2.gameObject, hide_popups: true);
				}
				if (num2 >= 0f)
				{
					break;
				}
			}
			if (num2 >= 0f)
			{
				break;
			}
		}
	}
}
