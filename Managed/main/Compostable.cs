using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Compostable")]
public class Compostable : KMonoBehaviour
{
	[SerializeField]
	public bool isMarkedForCompost;

	public GameObject originalPrefab;

	public GameObject compostPrefab;

	private static readonly EventSystem.IntraObjectHandler<Compostable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Compostable>(delegate(Compostable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Compostable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Compostable>(delegate(Compostable component, object data)
	{
		component.OnStore(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		isMarkedForCompost = GetComponent<KPrefabID>().HasTag(GameTags.Compostable);
		if (isMarkedForCompost)
		{
			MarkForCompost();
		}
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(856640610, OnStoreDelegate);
	}

	private void MarkForCompost(bool force = false)
	{
		RefreshStatusItem();
		Storage storage = GetComponent<Pickupable>().storage;
		if (storage != null)
		{
			storage.Drop(base.gameObject);
		}
	}

	private void OnToggleCompost()
	{
		if (!isMarkedForCompost)
		{
			Pickupable component = GetComponent<Pickupable>();
			if (component.storage != null)
			{
				component.storage.Drop(base.gameObject);
			}
			Pickupable pickupable = EntitySplitter.Split(component, component.TotalAmount, compostPrefab);
			if (pickupable != null)
			{
				SelectTool.Instance.SelectNextFrame(pickupable.GetComponent<KSelectable>(), skipSound: true);
			}
		}
		else
		{
			Pickupable component2 = GetComponent<Pickupable>();
			Pickupable pickupable2 = EntitySplitter.Split(component2, component2.TotalAmount, originalPrefab);
			SelectTool.Instance.SelectNextFrame(pickupable2.GetComponent<KSelectable>(), skipSound: true);
		}
	}

	private void RefreshStatusItem()
	{
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompost);
		component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage);
		if (isMarkedForCompost)
		{
			if (GetComponent<Pickupable>() != null && GetComponent<Pickupable>().storage == null)
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompost);
			}
			else
			{
				component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage);
			}
		}
	}

	private void OnStore(object data)
	{
		RefreshStatusItem();
	}

	private void OnRefreshUserMenu(object data)
	{
		KIconButtonMenu.ButtonInfo buttonInfo = null;
		buttonInfo = (isMarkedForCompost ? new KIconButtonMenu.ButtonInfo("action_compost", UI.USERMENUACTIONS.COMPOST.NAME_OFF, OnToggleCompost, Action.NumActions, null, null, null, UI.USERMENUACTIONS.COMPOST.TOOLTIP_OFF) : new KIconButtonMenu.ButtonInfo("action_compost", UI.USERMENUACTIONS.COMPOST.NAME, OnToggleCompost, Action.NumActions, null, null, null, UI.USERMENUACTIONS.COMPOST.TOOLTIP));
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo);
	}
}
