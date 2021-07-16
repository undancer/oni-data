using System;
using System.Collections;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduit")]
public class SolidConduit : KMonoBehaviour, IHaveUtilityNetworkMgr
{
	[MyCmpReq]
	private KAnimGraphTileVisualizer graphTileDependency;

	private System.Action firstFrameCallback;

	public Vector3 Position => base.transform.GetPosition();

	public void SetFirstFrameCallback(System.Action ffCb)
	{
		firstFrameCallback = ffCb;
		StartCoroutine(RunCallback());
	}

	private IEnumerator RunCallback()
	{
		yield return null;
		if (firstFrameCallback != null)
		{
			firstFrameCallback();
			firstFrameCallback = null;
		}
		yield return null;
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.solidConduitSystem;
	}

	public UtilityNetwork GetNetwork()
	{
		return GetNetworkManager().GetNetworkForCell(Grid.PosToCell(this));
	}

	public static SolidConduitFlow GetFlowManager()
	{
		return Game.Instance.solidConduitFlow;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Conveyor, this);
	}

	protected override void OnCleanUp()
	{
		int cell = Grid.PosToCell(this);
		BuildingComplete component = GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[cell, (int)component.Def.ReplacementLayer] == null)
		{
			GetNetworkManager().RemoveFromNetworks(cell, this, is_endpoint: false);
			GetFlowManager().EmptyConduit(cell);
		}
		base.OnCleanUp();
	}
}
