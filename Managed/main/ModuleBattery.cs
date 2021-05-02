public class ModuleBattery : Battery
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		connectedTags = new Tag[0];
		base.IsVirtual = true;
	}

	protected override void OnSpawn()
	{
		CraftModuleInterface craftModuleInterface = (CraftModuleInterface)(base.VirtualCircuitKey = GetComponent<RocketModuleCluster>().CraftInterface);
		base.OnSpawn();
		meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
	}
}
