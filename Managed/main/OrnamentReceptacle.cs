using UnityEngine;

public class OrnamentReceptacle : SingleEntityReceptacle
{
	[MyCmpReq]
	private SnapOn snapOn;

	private KBatchedAnimTracker occupyingTracker;

	private KAnimLink animLink;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.SetSymbolVisiblity("snapTo_ornament", is_visible: false);
	}

	protected override void PositionOccupyingObject()
	{
		KBatchedAnimController component = base.occupyingObject.GetComponent<KBatchedAnimController>();
		component.transform.SetLocalPosition(new Vector3(0f, 0f, -0.1f));
		occupyingTracker = base.occupyingObject.AddComponent<KBatchedAnimTracker>();
		occupyingTracker.symbol = new HashedString("snapTo_ornament");
		occupyingTracker.forceAlwaysVisible = true;
		animLink = new KAnimLink(GetComponent<KBatchedAnimController>(), component);
	}

	protected override void ClearOccupant()
	{
		if (occupyingTracker != null)
		{
			Object.Destroy(occupyingTracker);
			occupyingTracker = null;
		}
		if (animLink != null)
		{
			animLink.Unregister();
			animLink = null;
		}
		base.ClearOccupant();
	}
}
