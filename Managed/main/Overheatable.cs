using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class Overheatable : StateMachineComponent<Overheatable.StatesInstance>, IGameObjectEffectDescriptor
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, Overheatable, object>.GameInstance
	{
		public float lastOverheatDamageTime;

		public StatesInstance(Overheatable smi)
			: base(smi)
		{
		}

		public void TryDoOverheatDamage()
		{
			if (!(Time.time - lastOverheatDamageTime < 7.5f))
			{
				lastOverheatDamageTime += 7.5f;
				base.master.Trigger(-794517298, new BuildingHP.DamageSourceInfo
				{
					damage = 1,
					source = BUILDINGS.DAMAGESOURCES.BUILDING_OVERHEATED,
					popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.OVERHEAT,
					fullDamageEffectName = "smoke_damage_kanim"
				});
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, Overheatable>
	{
		public State invulnerable;

		public State safeTemperature;

		public State overheated;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = safeTemperature;
			root.EventTransition(GameHashes.BuildingBroken, invulnerable);
			invulnerable.EventHandler(GameHashes.BuildingPartiallyRepaired, delegate(StatesInstance smi)
			{
				smi.master.ResetTemperature();
			}).EventTransition(GameHashes.BuildingPartiallyRepaired, safeTemperature);
			safeTemperature.TriggerOnEnter(GameHashes.OptimalTemperatureAchieved).EventTransition(GameHashes.BuildingOverheated, overheated);
			overheated.Enter(delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_OverheatingBuildings);
			}).EventTransition(GameHashes.BuildingNoLongerOverheated, safeTemperature).ToggleStatusItem(Db.Get().BuildingStatusItems.Overheated)
				.ToggleNotification((StatesInstance smi) => smi.master.CreateOverheatedNotification())
				.TriggerOnEnter(GameHashes.TooHotWarning)
				.Enter("InitOverheatTime", delegate(StatesInstance smi)
				{
					smi.lastOverheatDamageTime = Time.time;
				})
				.Update("OverheatDamage", delegate(StatesInstance smi, float dt)
				{
					smi.TryDoOverheatDamage();
				}, UpdateRate.SIM_4000ms);
		}
	}

	private bool modifiersInitialized;

	private AttributeInstance overheatTemp;

	private AttributeInstance fatalTemp;

	public float baseOverheatTemp;

	public float baseFatalTemp;

	public float OverheatTemperature
	{
		get
		{
			InitializeModifiers();
			if (overheatTemp == null)
			{
				return 10000f;
			}
			return overheatTemp.GetTotalValue();
		}
	}

	public void ResetTemperature()
	{
		GetComponent<PrimaryElement>().Temperature = 293.15f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		overheatTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.OverheatTemperature);
		fatalTemp = this.GetAttributes().Add(Db.Get().BuildingAttributes.FatalTemperature);
	}

	private void InitializeModifiers()
	{
		if (!modifiersInitialized)
		{
			modifiersInitialized = true;
			AttributeModifier modifier = new AttributeModifier(overheatTemp.Id, baseOverheatTemp, UI.TOOLTIPS.BASE_VALUE);
			AttributeModifier modifier2 = new AttributeModifier(fatalTemp.Id, baseFatalTemp, UI.TOOLTIPS.BASE_VALUE);
			this.GetAttributes().Add(modifier);
			this.GetAttributes().Add(modifier2);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		InitializeModifiers();
		HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(base.gameObject);
		if (handle.IsValid() && GameComps.StructureTemperatures.IsEnabled(handle))
		{
			GameComps.StructureTemperatures.Disable(handle);
			GameComps.StructureTemperatures.Enable(handle);
		}
		base.smi.StartSM();
	}

	public Notification CreateOverheatedNotification()
	{
		KSelectable component = GetComponent<KSelectable>();
		return new Notification(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => string.Concat(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP, notificationList.ReduceMessages(countNames: false)), "/t• " + component.GetProperName(), expires: false);
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
		return string.Format(MISC.NOTIFICATIONS.BUILDINGOVERHEATED.TOOLTIP, text);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (overheatTemp != null && fatalTemp != null)
		{
			string formattedValue = overheatTemp.GetFormattedValue();
			string formattedValue2 = fatalTemp.GetFormattedValue();
			string str = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			str = str + "\n\n" + overheatTemp.GetAttributeValueTooltip();
			Descriptor item = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedValue, formattedValue2), string.Format(str, formattedValue, formattedValue2));
			list.Add(item);
		}
		else if (baseOverheatTemp != 0f)
		{
			string formattedTemperature = GameUtil.GetFormattedTemperature(baseOverheatTemp);
			string formattedTemperature2 = GameUtil.GetFormattedTemperature(baseFatalTemp);
			string format = UI.BUILDINGEFFECTS.TOOLTIPS.OVERHEAT_TEMP;
			Descriptor item2 = new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVERHEAT_TEMP, formattedTemperature, formattedTemperature2), string.Format(format, formattedTemperature, formattedTemperature2));
			list.Add(item2);
		}
		return list;
	}
}
