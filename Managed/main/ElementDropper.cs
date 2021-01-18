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
		GameObject gameObject = storage.FindFirst(emitTag);
		if (!(gameObject == null) && gameObject.GetComponent<PrimaryElement>().Mass >= emitMass)
		{
			Pickupable pickupable = gameObject.GetComponent<Pickupable>();
			if (pickupable != null)
			{
				pickupable = pickupable.Take(emitMass);
				pickupable.transform.SetPosition(pickupable.transform.GetPosition() + emitOffset);
				pickupable.transform.parent = null;
				Trigger(-1697596308, pickupable.gameObject);
				pickupable.Trigger(856640610);
			}
			else
			{
				storage.Drop(gameObject);
				gameObject.transform.SetPosition(gameObject.transform.GetPosition() + emitOffset);
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, pickupable.GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform);
		}
	}
}
