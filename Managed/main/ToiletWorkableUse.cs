using System;
using Klei.AI;
using KSerialization;
using STRINGS;
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
		if (worker.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0f)
		{
			worker.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
		}
		Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject)?.roomType.TriggerRoomEffects(GetComponent<KPrefabID>(), worker.GetComponent<Effects>());
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
		base.OnStopWork(worker);
	}

	protected override void OnAbortWork(Worker worker)
	{
		worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
		base.OnAbortWork(worker);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Db.Get().Amounts.Bladder.Lookup(worker).SetValue(0f);
		worker.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
		AmountInstance amountInstance = Db.Get().Amounts.RadiationBalance.Lookup(worker);
		float num = Math.Min(amountInstance.value, 60f);
		if (num >= 1f)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Math.Floor(num).ToString() + UI.UNITSUFFIXES.RADIATION.RADS, worker.transform, Vector3.up * 2f);
		}
		amountInstance.ApplyDelta(0f - num);
		timesUsed++;
		Trigger(-350347868, worker);
		base.OnCompleteWork(worker);
	}
}
