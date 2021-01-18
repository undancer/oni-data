using Klei.AI;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableUse")]
public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
	[Serialize]
	public int timesUsed;

	private ToiletWorkableUse()
	{
		SetReportType(ReportManager.ReportType.PersonalTime);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = true;
		resetProgressOnStop = true;
		attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
		SetWorkTime(8.5f);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject)?.roomType.TriggerRoomEffects(GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Db.Get().Amounts.Bladder.Lookup(worker).SetValue(0f);
		timesUsed++;
		Trigger(-350347868, worker);
		base.OnCompleteWork(worker);
	}
}
