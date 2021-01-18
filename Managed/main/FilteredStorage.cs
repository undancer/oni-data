using System;
using UnityEngine;

public class FilteredStorage
{
	public static readonly HashedString FULL_PORT_ID = "FULL";

	private KMonoBehaviour root;

	private FetchList2 fetchList;

	private IUserControlledCapacity capacityControl;

	private TreeFilterable filterable;

	private Storage storage;

	private MeterController meter;

	private MeterController logicMeter;

	public static readonly Color32 FILTER_TINT = Color.white;

	public static readonly Color32 NO_FILTER_TINT = new Color(128f / 255f, 128f / 255f, 128f / 255f, 1f);

	public Color32 filterTint = FILTER_TINT;

	public Color32 noFilterTint = NO_FILTER_TINT;

	private Tag[] requiredTags;

	private Tag[] forbiddenTags;

	private bool hasMeter = true;

	private bool useLogicMeter = false;

	private static StatusItem noFilterStatusItem;

	private ChoreType choreType;

	public void SetHasMeter(bool has_meter)
	{
		hasMeter = has_meter;
	}

	public FilteredStorage(KMonoBehaviour root, Tag[] required_tags, Tag[] forbidden_tags, IUserControlledCapacity capacity_control, bool use_logic_meter, ChoreType fetch_chore_type)
	{
		this.root = root;
		requiredTags = required_tags;
		forbiddenTags = forbidden_tags;
		capacityControl = capacity_control;
		useLogicMeter = use_logic_meter;
		choreType = fetch_chore_type;
		root.Subscribe(-1697596308, OnStorageChanged);
		root.Subscribe(-543130682, OnUserSettingsChanged);
		filterable = root.FindOrAdd<TreeFilterable>();
		TreeFilterable treeFilterable = filterable;
		treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Combine(treeFilterable.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
		storage = root.GetComponent<Storage>();
		storage.Subscribe(644822890, OnOnlyFetchMarkedItemsSettingChanged);
		storage.Subscribe(-1852328367, OnFunctionalChanged);
		if (noFilterStatusItem == null)
		{
			noFilterStatusItem = new StatusItem("NoStorageFilterSet", "BUILDING", "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, allow_multiples: false, OverlayModes.None.ID);
		}
	}

	private void OnOnlyFetchMarkedItemsSettingChanged(object data)
	{
		OnFilterChanged(filterable.GetTags());
	}

	private void CreateMeter()
	{
		if (hasMeter)
		{
			meter = new MeterController(root.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_frame", "meter_level");
		}
	}

	private void CreateLogicMeter()
	{
		if (hasMeter)
		{
			logicMeter = new MeterController(root.GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer);
		}
	}

	public void CleanUp()
	{
		if (filterable != null)
		{
			TreeFilterable treeFilterable = filterable;
			treeFilterable.OnFilterChanged = (Action<Tag[]>)Delegate.Remove(treeFilterable.OnFilterChanged, new Action<Tag[]>(OnFilterChanged));
		}
		if (fetchList != null)
		{
			fetchList.Cancel("Parent destroyed");
		}
	}

	public void FilterChanged()
	{
		if (hasMeter)
		{
			if (meter == null)
			{
				CreateMeter();
			}
			if (logicMeter == null && useLogicMeter)
			{
				CreateLogicMeter();
			}
		}
		OnFilterChanged(filterable.GetTags());
		UpdateMeter();
	}

	private void OnUserSettingsChanged(object data)
	{
		OnFilterChanged(filterable.GetTags());
		UpdateMeter();
	}

	private void OnStorageChanged(object data)
	{
		if (fetchList == null)
		{
			OnFilterChanged(filterable.GetTags());
		}
		UpdateMeter();
	}

	private void OnFunctionalChanged(object data)
	{
		OnFilterChanged(filterable.GetTags());
	}

	private void UpdateMeter()
	{
		float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
		float positionPercent = Mathf.Clamp01(GetAmountStored() / maxCapacityMinusStorageMargin);
		if (meter != null)
		{
			meter.SetPositionPercent(positionPercent);
		}
	}

	public bool IsFull()
	{
		float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
		float num = Mathf.Clamp01(GetAmountStored() / maxCapacityMinusStorageMargin);
		if (meter != null)
		{
			meter.SetPositionPercent(num);
		}
		return (num >= 1f) ? true : false;
	}

	private void OnFetchComplete()
	{
		OnFilterChanged(filterable.GetTags());
	}

	private float GetMaxCapacity()
	{
		float num = storage.capacityKg;
		if (capacityControl != null)
		{
			num = Mathf.Min(num, capacityControl.UserMaxCapacity);
		}
		return num;
	}

	private float GetMaxCapacityMinusStorageMargin()
	{
		return GetMaxCapacity() - storage.storageFullMargin;
	}

	private float GetAmountStored()
	{
		float result = storage.MassStored();
		if (capacityControl != null)
		{
			result = capacityControl.AmountStored;
		}
		return result;
	}

	private bool IsFunctional()
	{
		Operational component = storage.GetComponent<Operational>();
		return component == null || component.IsFunctional;
	}

	private void OnFilterChanged(Tag[] tags)
	{
		KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
		bool flag = tags != null && tags.Length != 0;
		component.TintColour = (flag ? filterTint : noFilterTint);
		if (fetchList != null)
		{
			fetchList.Cancel("");
			fetchList = null;
		}
		float maxCapacityMinusStorageMargin = GetMaxCapacityMinusStorageMargin();
		float amountStored = GetAmountStored();
		float num = Mathf.Max(0f, maxCapacityMinusStorageMargin - amountStored);
		if (num > 0f && flag && IsFunctional())
		{
			num = Mathf.Max(0f, GetMaxCapacity() - amountStored);
			fetchList = new FetchList2(storage, choreType);
			fetchList.ShowStatusItem = false;
			fetchList.Add(tags, requiredTags, forbiddenTags, num, FetchOrder2.OperationalRequirement.Functional);
			fetchList.Submit(OnFetchComplete, check_storage_contents: false);
		}
		root.GetComponent<KSelectable>().ToggleStatusItem(noFilterStatusItem, !flag, this);
	}

	public void SetLogicMeter(bool on)
	{
		if (logicMeter != null)
		{
			logicMeter.SetPositionPercent(on ? 1f : 0f);
		}
	}
}
