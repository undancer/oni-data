using UnityEngine;

public class OxygenMask : KMonoBehaviour, ISim200ms
{
	private static readonly EventSystem.IntraObjectHandler<OxygenMask> OnSuitTankDeltaDelegate = new EventSystem.IntraObjectHandler<OxygenMask>(delegate(OxygenMask component, object data)
	{
		component.CheckOxygenLevels(data);
	});

	[MyCmpGet]
	private SuitTank suitTank;

	[MyCmpGet]
	private Storage storage;

	private float leakRate = 0.1f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(608245985, OnSuitTankDeltaDelegate);
	}

	private void CheckOxygenLevels(object data)
	{
		if (!suitTank.IsEmpty())
		{
			return;
		}
		Equippable component = GetComponent<Equippable>();
		if (component.assignee != null)
		{
			Ownables soleOwner = component.assignee.GetSoleOwner();
			if (soleOwner != null)
			{
				soleOwner.GetComponent<Equipment>().Unequip(component);
			}
		}
	}

	public void Sim200ms(float dt)
	{
		Equippable component = GetComponent<Equippable>();
		if (component.assignee == null)
		{
			float a = leakRate * dt;
			float massAvailable = storage.GetMassAvailable(suitTank.elementTag);
			a = Mathf.Min(a, massAvailable);
			storage.DropSome(suitTank.elementTag, a, ventGas: true, dumpLiquid: true);
		}
		if (suitTank.IsEmpty())
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}
}
