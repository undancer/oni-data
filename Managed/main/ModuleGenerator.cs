using System;

public class ModuleGenerator : Generator
{
	private Clustercraft clustercraft;

	private Guid poweringStatusItemHandle;

	private Guid notPoweringStatusItemHandle;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		connectedTags = new Tag[0];
		base.IsVirtual = true;
	}

	protected override void OnSpawn()
	{
		CraftModuleInterface craftModuleInterface = (CraftModuleInterface)(base.VirtualCircuitKey = GetComponent<RocketModuleCluster>().CraftInterface);
		clustercraft = craftModuleInterface.GetComponent<Clustercraft>();
		Game.Instance.electricalConduitSystem.AddToVirtualNetworks(base.VirtualCircuitKey, this, is_endpoint: true);
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.electricalConduitSystem.RemoveFromVirtualNetworks(base.VirtualCircuitKey, this, is_endpoint: true);
	}

	public override bool IsProducingPower()
	{
		return clustercraft.IsFlightInProgress();
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		if (IsProducingPower())
		{
			GenerateJoules(base.WattageRating * dt);
			if (poweringStatusItemHandle == Guid.Empty)
			{
				poweringStatusItemHandle = selectable.ReplaceStatusItem(notPoweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorPowered, this);
				notPoweringStatusItemHandle = Guid.Empty;
			}
		}
		else if (notPoweringStatusItemHandle == Guid.Empty)
		{
			notPoweringStatusItemHandle = selectable.ReplaceStatusItem(poweringStatusItemHandle, Db.Get().BuildingStatusItems.ModuleGeneratorNotPowered, this);
			poweringStatusItemHandle = Guid.Empty;
		}
	}
}
