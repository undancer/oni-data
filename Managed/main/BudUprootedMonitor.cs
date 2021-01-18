using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BudUprootedMonitor")]
public class BudUprootedMonitor : KMonoBehaviour
{
	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted = false;

	public Ref<KPrefabID> parentObject = new Ref<KPrefabID>();

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<BudUprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<BudUprootedMonitor>(delegate(BudUprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted);
			component.uprooted = true;
			component.Trigger(-216549700);
		}
	});

	public bool IsUprooted => uprooted || GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(-216549700, OnUprootedDelegate);
	}

	public void SetParentObject(KPrefabID id)
	{
		parentObject = new Ref<KPrefabID>(id);
		Subscribe(id.gameObject, 1969584890, OnLoseParent);
	}

	private void OnLoseParent(object obj)
	{
		if (!uprooted && !base.isNull)
		{
			GetComponent<KPrefabID>().AddTag(GameTags.Uprooted);
			uprooted = true;
			Trigger(-216549700);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public static bool IsObjectUprooted(GameObject plant)
	{
		BudUprootedMonitor component = plant.GetComponent<BudUprootedMonitor>();
		if (component == null)
		{
			return false;
		}
		return component.IsUprooted;
	}
}
