using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Breakable")]
public class Breakable : Workable
{
	private const float TIME_TO_BREAK_AT_FULL_HEALTH = 20f;

	private Notification notification;

	private float secondsPerTenPercentDamage = float.PositiveInfinity;

	private float elapsedDamageTime;

	private int tenPercentDamage = int.MaxValue;

	[MyCmpGet]
	private BuildingHP hp;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		showProgressBar = false;
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_break_kanim")
		};
		SetWorkTime(float.PositiveInfinity);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Breakables.Add(this);
	}

	public bool isBroken()
	{
		if (hp == null)
		{
			return true;
		}
		return hp.HitPoints <= 0;
	}

	public Notification CreateDamageNotification()
	{
		KSelectable component = GetComponent<KSelectable>();
		return new Notification(BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION, NotificationType.BadMinor, (List<Notification> notificationList, object data) => string.Concat(BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION_TOOLTIP, notificationList.ReduceMessages(countNames: false)), component.GetProperName(), expires: false);
	}

	private static string ToolTipResolver(List<Notification> notificationList, object data)
	{
		string text = "";
		for (int i = 0; i < notificationList.Count; i++)
		{
			Notification notification = notificationList[i];
			text += (string)notification.tooltipData;
			if (i < notificationList.Count - 1)
			{
				text += "\n";
			}
		}
		return string.Format(BUILDING.STATUSITEMS.ANGERDAMAGE.NOTIFICATION_TOOLTIP, text);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		secondsPerTenPercentDamage = 2f;
		tenPercentDamage = Mathf.CeilToInt((float)hp.MaxHitPoints * 0.1f);
		GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.AngerDamage, this);
		notification = CreateDamageNotification();
		base.gameObject.AddOrGet<Notifier>().Add(notification);
		elapsedDamageTime = 0f;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		if (elapsedDamageTime >= secondsPerTenPercentDamage)
		{
			elapsedDamageTime -= elapsedDamageTime;
			Trigger(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = tenPercentDamage,
				source = BUILDINGS.DAMAGESOURCES.MINION_DESTRUCTION,
				popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.MINION_DESTRUCTION
			});
		}
		elapsedDamageTime += dt;
		return hp.HitPoints <= 0;
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.AngerDamage);
		base.gameObject.AddOrGet<Notifier>().Remove(notification);
		if (worker != null)
		{
			worker.Trigger(-1734580852);
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		return false;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Breakables.Remove(this);
	}
}
