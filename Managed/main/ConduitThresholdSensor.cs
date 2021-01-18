using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public abstract class ConduitThresholdSensor : ConduitSensor
{
	[SerializeField]
	[Serialize]
	protected float threshold;

	[SerializeField]
	[Serialize]
	protected bool activateAboveThreshold = true;

	[Serialize]
	private bool dirty = true;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<ConduitThresholdSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ConduitThresholdSensor>(delegate(ConduitThresholdSensor component, object data)
	{
		component.OnCopySettings(data);
	});

	public abstract float CurrentValue
	{
		get;
	}

	public float Threshold
	{
		get
		{
			return threshold;
		}
		set
		{
			threshold = value;
			dirty = true;
		}
	}

	public bool ActivateAboveThreshold
	{
		get
		{
			return activateAboveThreshold;
		}
		set
		{
			activateAboveThreshold = value;
			dirty = true;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		ConduitThresholdSensor component = ((GameObject)data).GetComponent<ConduitThresholdSensor>();
		if (component != null)
		{
			Threshold = component.Threshold;
			ActivateAboveThreshold = component.ActivateAboveThreshold;
		}
	}

	protected override void ConduitUpdate(float dt)
	{
		if (GetContainedMass() <= 0f && !dirty)
		{
			return;
		}
		float currentValue = CurrentValue;
		dirty = false;
		if (activateAboveThreshold)
		{
			if ((currentValue > threshold && !base.IsSwitchedOn) || (currentValue <= threshold && base.IsSwitchedOn))
			{
				Toggle();
			}
		}
		else if ((currentValue > threshold && base.IsSwitchedOn) || (currentValue <= threshold && !base.IsSwitchedOn))
		{
			Toggle();
		}
	}

	private float GetContainedMass()
	{
		int cell = Grid.PosToCell(this);
		if (conduitType == ConduitType.Liquid || conduitType == ConduitType.Gas)
		{
			return Conduit.GetFlowManager(conduitType).GetContents(cell).mass;
		}
		SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
		Pickupable pickupable = flowManager.GetPickupable(flowManager.GetContents(cell).pickupableHandle);
		if (pickupable != null)
		{
			return pickupable.PrimaryElement.Mass;
		}
		return 0f;
	}
}
