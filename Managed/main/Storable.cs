public class Storable : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<Storable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Storable>(delegate(Storable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storable> RefreshStorageTagsDelegate = new EventSystem.IntraObjectHandler<Storable>(delegate(Storable component, object data)
	{
		component.RefreshStorageTags(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(856640610, OnStoreDelegate);
		Subscribe(-778359855, RefreshStorageTagsDelegate);
	}

	public void OnStore(object data)
	{
		RefreshStorageTags(data);
	}

	private void RefreshStorageTags(object data = null)
	{
		bool flag = data is Storage || (data != null && (bool)data);
		Storage storage = (Storage)data;
		if (storage != null && storage.gameObject == base.gameObject)
		{
			return;
		}
		KPrefabID component = GetComponent<KPrefabID>();
		SaveLoadRoot component2 = GetComponent<SaveLoadRoot>();
		KSelectable component3 = GetComponent<KSelectable>();
		if ((bool)component3)
		{
			component3.IsSelectable = !flag;
		}
		if (flag)
		{
			component.AddTag(GameTags.Stored);
			if (storage == null || !storage.allowItemRemoval)
			{
				component.AddTag(GameTags.StoredPrivate);
			}
			else
			{
				component.RemoveTag(GameTags.StoredPrivate);
			}
			if (component2 != null)
			{
				component2.SetRegistered(registered: false);
			}
		}
		else
		{
			component.RemoveTag(GameTags.Stored);
			component.RemoveTag(GameTags.StoredPrivate);
			if (component2 != null)
			{
				component2.SetRegistered(registered: true);
			}
		}
	}
}
