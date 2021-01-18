using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/BuildingHP")]
public class BuildingHP : Workable
{
	public struct DamageSourceInfo
	{
		public int damage;

		public string source;

		public string popString;

		public SpawnFXHashes takeDamageEffect;

		public string fullDamageEffectName;

		public string statusItemID;

		public override string ToString()
		{
			return source;
		}
	}

	public class SMInstance : GameStateMachine<States, SMInstance, BuildingHP, object>.GameInstance
	{
		private ProgressBar progressBar = null;

		public SMInstance(BuildingHP master)
			: base(master)
		{
		}

		public Notification CreateBrokenMachineNotification()
		{
			return new Notification(MISC.NOTIFICATIONS.BROKENMACHINE.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.BROKENMACHINE.TOOLTIP, notificationList.ReduceMessages(countNames: false)), "/t• " + base.master.damageSourceInfo.source, expires: false);
		}

		public void ShowProgressBar(bool show)
		{
			if (show && Grid.IsValidCell(Grid.PosToCell(base.gameObject)) && Grid.IsVisible(Grid.PosToCell(base.gameObject)))
			{
				CreateProgressBar();
			}
			else if (progressBar != null)
			{
				progressBar.gameObject.DeleteObject();
				progressBar = null;
			}
		}

		public void UpdateMeter()
		{
			if (progressBar == null)
			{
				ShowProgressBar(show: true);
			}
			if ((bool)progressBar)
			{
				progressBar.Update();
			}
		}

		private float HealthPercent()
		{
			return (float)base.smi.master.HitPoints / (float)base.smi.master.building.Def.HitPoints;
		}

		private void CreateProgressBar()
		{
			if (!(progressBar != null))
			{
				progressBar = Util.KInstantiateUI<ProgressBar>(ProgressBarsConfig.Instance.progressBarPrefab);
				progressBar.transform.SetParent(GameScreenManager.Instance.worldSpaceCanvas.transform);
				progressBar.name = base.smi.master.name + "." + base.smi.master.GetType().Name + " ProgressBar";
				progressBar.transform.Find("Bar").GetComponent<Image>().color = ProgressBarsConfig.Instance.GetBarColor("ProgressBar");
				progressBar.SetUpdateFunc(HealthPercent);
				progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
				CanvasGroup component = progressBar.GetComponent<CanvasGroup>();
				component.interactable = false;
				component.blocksRaycasts = false;
				progressBar.Update();
				float d = 0.15f;
				Vector3 position = base.gameObject.transform.GetPosition() + Vector3.down * d;
				position.z += 0.05f;
				Rotatable component2 = GetComponent<Rotatable>();
				if (component2 == null || component2.GetOrientation() == Orientation.Neutral || base.smi.master.building.Def.WidthInCells < 2 || base.smi.master.building.Def.HeightInCells < 2)
				{
					position -= Vector3.right * 0.5f * (base.smi.master.building.Def.WidthInCells % 2);
				}
				else
				{
					position += Vector3.left * (1f + 0.5f * (float)(base.smi.master.building.Def.WidthInCells % 2));
				}
				progressBar.transform.SetPosition(position);
				progressBar.gameObject.SetActive(value: true);
			}
		}

		private static string ToolTipResolver(List<Notification> notificationList, object data)
		{
			string text = "";
			for (int i = 0; i < notificationList.Count; i++)
			{
				Notification notification = notificationList[i];
				text += string.Format(BUILDINGS.DAMAGESOURCES.NOTIFICATION_TOOLTIP, notification.NotifierName, (string)notification.tooltipData);
				if (i < notificationList.Count - 1)
				{
					text += "\n";
				}
			}
			return text;
		}

		public void ShowDamagedEffect()
		{
			if (base.master.damageSourceInfo.takeDamageEffect != 0)
			{
				BuildingDef def = base.master.GetComponent<BuildingComplete>().Def;
				int cell = Grid.PosToCell(base.master);
				int cell2 = Grid.OffsetCell(cell, 0, def.HeightInCells - 1);
				Game.Instance.SpawnFX(base.master.damageSourceInfo.takeDamageEffect, cell2, 0f);
			}
		}

		public FXAnim.Instance InstantiateDamageFX()
		{
			if (base.master.damageSourceInfo.fullDamageEffectName == null)
			{
				return null;
			}
			BuildingDef def = base.master.GetComponent<BuildingComplete>().Def;
			Vector3 zero = Vector3.zero;
			return new FXAnim.Instance(offset: (def.HeightInCells > 1) ? new Vector3(0f, def.HeightInCells - 1, 0f) : new Vector3(0f, 0.5f, 0f), master: base.smi.master, kanim_file: base.master.damageSourceInfo.fullDamageEffectName, anim: "idle", mode: KAnim.PlayMode.Loop, tint_colour: Color.white);
		}

		public void SetCrackOverlayValue(float value)
		{
			KBatchedAnimController component = base.master.GetComponent<KBatchedAnimController>();
			if (!(component == null))
			{
				component.SetBlendValue(value);
				kbacQueryList.Clear();
				base.master.GetComponentsInChildren(kbacQueryList);
				for (int i = 0; i < kbacQueryList.Count; i++)
				{
					Meter meter = kbacQueryList[i];
					KBatchedAnimController component2 = meter.GetComponent<KBatchedAnimController>();
					component2.SetBlendValue(value);
				}
			}
		}
	}

	public class States : GameStateMachine<States, SMInstance, BuildingHP>
	{
		public class Healthy : State
		{
			public ImperfectStates imperfect;

			public State perfect;
		}

		public class ImperfectStates : State
		{
			public State playEffect;

			public State waiting;
		}

		private static readonly Operational.Flag healthyFlag = new Operational.Flag("healthy", Operational.Flag.Type.Functional);

		public State damaged;

		public Healthy healthy;

		public override void InitializeStates(out BaseState default_state)
		{
			base.serializable = SerializeType.Both_DEPRECATED;
			default_state = healthy;
			healthy.DefaultState(healthy.imperfect).EventTransition(GameHashes.BuildingReceivedDamage, damaged, (SMInstance smi) => smi.master.HitPoints <= 0);
			healthy.imperfect.Enter(delegate(SMInstance smi)
			{
				smi.ShowProgressBar(show: true);
			}).DefaultState(healthy.imperfect.playEffect).EventTransition(GameHashes.BuildingPartiallyRepaired, healthy.perfect, (SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.ToggleStatusItem((SMInstance smi) => (smi.master.damageSourceInfo.statusItemID != null) ? Db.Get().BuildingStatusItems.Get(smi.master.damageSourceInfo.statusItemID) : null)
				.Exit(delegate(SMInstance smi)
				{
					smi.ShowProgressBar(show: false);
				});
			healthy.imperfect.playEffect.Transition(healthy.imperfect.waiting, (SMInstance smi) => true);
			healthy.imperfect.waiting.ScheduleGoTo((SMInstance smi) => UnityEngine.Random.Range(15f, 30f), healthy.imperfect.playEffect);
			healthy.perfect.EventTransition(GameHashes.BuildingReceivedDamage, healthy.imperfect, (SMInstance smi) => smi.master.HitPoints < smi.master.building.Def.HitPoints);
			damaged.Enter(delegate(SMInstance smi)
			{
				Operational component2 = smi.GetComponent<Operational>();
				if (component2 != null)
				{
					component2.SetFlag(healthyFlag, value: false);
				}
				smi.ShowProgressBar(show: true);
				smi.master.Trigger(774203113, smi.master);
				smi.SetCrackOverlayValue(1f);
			}).ToggleNotification((SMInstance smi) => smi.CreateBrokenMachineNotification()).ToggleStatusItem(Db.Get().BuildingStatusItems.Broken)
				.ToggleFX((SMInstance smi) => smi.InstantiateDamageFX())
				.EventTransition(GameHashes.BuildingPartiallyRepaired, healthy.perfect, (SMInstance smi) => smi.master.HitPoints == smi.master.building.Def.HitPoints)
				.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(SMInstance smi)
				{
					smi.UpdateMeter();
				})
				.Exit(delegate(SMInstance smi)
				{
					Operational component = smi.GetComponent<Operational>();
					if (component != null)
					{
						component.SetFlag(healthyFlag, value: true);
					}
					smi.ShowProgressBar(show: false);
					smi.SetCrackOverlayValue(0f);
				});
		}

		private Chore CreateRepairChore(SMInstance smi)
		{
			return new WorkChore<BuildingHP>(Db.Get().ChoreTypes.Repair, smi.master, null, run_until_complete: true, null, null, null, allow_in_red_alert: true, null, ignore_schedule_block: false, only_when_operational: false);
		}
	}

	[Serialize]
	[SerializeField]
	private int hitpoints;

	[Serialize]
	private DamageSourceInfo damageSourceInfo;

	private static readonly EventSystem.IntraObjectHandler<BuildingHP> OnDoBuildingDamageDelegate = new EventSystem.IntraObjectHandler<BuildingHP>(delegate(BuildingHP component, object data)
	{
		component.OnDoBuildingDamage(data);
	});

	private static readonly EventSystem.IntraObjectHandler<BuildingHP> DestroyOnDamagedDelegate = new EventSystem.IntraObjectHandler<BuildingHP>(delegate(BuildingHP component, object data)
	{
		component.DestroyOnDamaged(data);
	});

	public static List<Meter> kbacQueryList = new List<Meter>();

	public bool destroyOnDamaged = false;

	public bool invincible = false;

	[MyCmpGet]
	private Building building;

	private SMInstance smi;

	private float minDamagePopInterval = 4f;

	private float lastPopTime = 0f;

	public int HitPoints => hitpoints;

	public int MaxHitPoints => building.Def.HitPoints;

	public bool IsBroken => hitpoints == 0;

	public bool NeedsRepairs => HitPoints < building.Def.HitPoints;

	public void SetHitPoints(int hp)
	{
		hitpoints = hp;
	}

	public DamageSourceInfo GetDamageSourceInfo()
	{
		return damageSourceInfo;
	}

	protected override void OnLoadLevel()
	{
		smi = null;
		base.OnLoadLevel();
	}

	public void DoDamage(int damage)
	{
		if (!invincible)
		{
			damage = Math.Max(0, damage);
			hitpoints = Math.Max(0, hitpoints - damage);
			Trigger(-1964935036, this);
		}
	}

	public void Repair(int repair_amount)
	{
		if (hitpoints + repair_amount < hitpoints)
		{
			hitpoints = building.Def.HitPoints;
		}
		else
		{
			hitpoints = Math.Min(hitpoints + repair_amount, building.Def.HitPoints);
		}
		Trigger(-1699355994, this);
		if (hitpoints >= building.Def.HitPoints)
		{
			Trigger(-1735440190, this);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetWorkTime(10f);
		multitoolContext = "build";
		multitoolHitEffectTag = EffectConfigs.BuildSplashId;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		smi = new SMInstance(this);
		smi.StartSM();
		Subscribe(-794517298, OnDoBuildingDamageDelegate);
		if (destroyOnDamaged)
		{
			Subscribe(774203113, DestroyOnDamagedDelegate);
		}
		if (hitpoints <= 0)
		{
			Trigger(774203113, this);
		}
	}

	private void DestroyOnDamaged(object data)
	{
		Util.KDestroyGameObject(base.gameObject);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.Machinery.Lookup(worker);
		int num = (int)attributeInstance.GetTotalValue();
		int repair_amount = 10 + Math.Max(0, num * 10);
		Repair(repair_amount);
	}

	private void OnDoBuildingDamage(object data)
	{
		if (!invincible)
		{
			damageSourceInfo = (DamageSourceInfo)data;
			DoDamage(damageSourceInfo.damage);
			DoDamagePopFX(damageSourceInfo);
			DoTakeDamageFX(damageSourceInfo);
		}
	}

	private void DoTakeDamageFX(DamageSourceInfo info)
	{
		if (info.takeDamageEffect != 0)
		{
			BuildingDef def = GetComponent<BuildingComplete>().Def;
			int cell = Grid.PosToCell(this);
			int cell2 = Grid.OffsetCell(cell, 0, def.HeightInCells - 1);
			Game.Instance.SpawnFX(info.takeDamageEffect, cell2, 0f);
		}
	}

	private void DoDamagePopFX(DamageSourceInfo info)
	{
		if (info.popString != null && Time.time > lastPopTime + minDamagePopInterval)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, info.popString, base.gameObject.transform);
			lastPopTime = Time.time;
		}
	}
}
