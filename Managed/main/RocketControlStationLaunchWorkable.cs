using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/RocketControlStationLaunchWorkable")]
public class RocketControlStationLaunchWorkable : Workable
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

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.GetSMI<RocketControlStation.StatesInstance>()?.LaunchRocket();
	}
}
