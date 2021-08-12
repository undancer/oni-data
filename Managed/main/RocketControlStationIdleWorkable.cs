using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RocketControlStationIdleWorkable")]
public class RocketControlStationIdleWorkable : Workable
{
	[MyCmpReq]
	private Operational operational;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_rocket_control_station_kanim") };
		showProgressBar = true;
		resetProgressOnStop = true;
		synchronizeAnims = true;
		SetWorkTime(30f);
	}
}
