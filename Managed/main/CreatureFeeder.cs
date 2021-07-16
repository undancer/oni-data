using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CreatureFeeder")]
public class CreatureFeeder : KMonoBehaviour
{
	public string effectId;

	private static readonly EventSystem.IntraObjectHandler<CreatureFeeder> OnAteFromStorageDelegate = new EventSystem.IntraObjectHandler<CreatureFeeder>(delegate(CreatureFeeder component, object data)
	{
		component.OnAteFromStorage(data);
	});

	protected override void OnSpawn()
	{
		Components.CreatureFeeders.Add(this);
		Subscribe(-1452790913, OnAteFromStorageDelegate);
	}

	protected override void OnCleanUp()
	{
		Components.CreatureFeeders.Remove(this);
	}

	private void OnAteFromStorage(object data)
	{
		if (!string.IsNullOrEmpty(effectId))
		{
			(data as GameObject).GetComponent<Effects>().Add(effectId, should_save: true);
		}
	}
}
