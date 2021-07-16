using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ElementDropper")]
public class ElementDropper : KMonoBehaviour
{
	[SerializeField]
	public Tag emitTag;

	[SerializeField]
	public float emitMass;

	[SerializeField]
	public Vector3 emitOffset = Vector3.zero;

	[MyCmpGet]
	private Storage storage;

	private static readonly EventSystem.IntraObjectHandler<ElementDropper> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ElementDropper>(delegate(ElementDropper component, object data)
	{
		component.OnStorageChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-1697596308, OnStorageChangedDelegate);
	}

	private void OnStorageChanged(object data)
	{
		if (storage.GetMassAvailable(emitTag) >= emitMass)
		{
			storage.DropSome(emitTag, emitMass, ventGas: false, dumpLiquid: false, emitOffset, doDiseaseTransfer: true, showInWorldNotification: true);
		}
	}
}
