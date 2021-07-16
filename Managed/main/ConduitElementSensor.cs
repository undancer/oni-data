using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitElementSensor : ConduitSensor
{
	[MyCmpGet]
	private Filterable filterable;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		filterable.onFilterChanged += OnFilterChanged;
		OnFilterChanged(filterable.SelectedTag);
	}

	private void OnFilterChanged(Tag tag)
	{
		if (tag.IsValid)
		{
			bool on = tag == GameTags.Void;
			GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoFilterElementSelected, on);
		}
	}

	protected override void ConduitUpdate(float dt)
	{
		GetContentsElement(out var element, out var hasMass);
		if (!base.IsSwitchedOn)
		{
			if (element == filterable.SelectedTag && hasMass)
			{
				Toggle();
			}
		}
		else if (element != filterable.SelectedTag || !hasMass)
		{
			Toggle();
		}
	}

	private void GetContentsElement(out Tag element, out bool hasMass)
	{
		int cell = Grid.PosToCell(this);
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(conduitType).GetContents(cell);
			element = contents.element.CreateTag();
			hasMass = contents.mass > 0f;
			return;
		}
		SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
		Pickupable pickupable = flowManager.GetPickupable(flowManager.GetContents(cell).pickupableHandle);
		KPrefabID kPrefabID = ((pickupable != null) ? pickupable.GetComponent<KPrefabID>() : null);
		if (kPrefabID != null && pickupable.PrimaryElement.Mass > 0f)
		{
			element = kPrefabID.PrefabTag;
			hasMass = true;
		}
		else
		{
			element = GameTags.Void;
			hasMass = false;
		}
	}
}
