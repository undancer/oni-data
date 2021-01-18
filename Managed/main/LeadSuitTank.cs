using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LeadSuitTank : KMonoBehaviour, IGameObjectEffectDescriptor
{
	[Serialize]
	public float batteryCharge = 1f;

	public const float REFILL_PERCENT = 0.25f;

	public float batteryDuration = 200f;

	public float coolingOperationalTemperature = 333.15f;

	public Tag coolantTag;

	private LeadSuitMonitor.Instance leadSuitMonitor;

	private static readonly EventSystem.IntraObjectHandler<LeadSuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<LeadSuitTank>(delegate(LeadSuitTank component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LeadSuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<LeadSuitTank>(delegate(LeadSuitTank component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	public float PercentFull()
	{
		return batteryCharge;
	}

	public bool IsEmpty()
	{
		return batteryCharge <= 0f;
	}

	public bool IsFull()
	{
		return PercentFull() >= 1f;
	}

	public bool NeedsRecharging()
	{
		return PercentFull() <= 0.25f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string text = string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.LEADSUIT_BATTERY, GameUtil.GetFormattedPercent(PercentFull() * 100f));
		list.Add(new Descriptor(text, text));
		return list;
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		NameDisplayScreen.Instance.SetSuitBatteryDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), PercentFull, bVisible: true);
		leadSuitMonitor = new LeadSuitMonitor.Instance(this, equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
		leadSuitMonitor.StartSM();
		if (NeedsRecharging())
		{
			equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().AddTag(GameTags.SuitBatteryLow);
		}
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		if (!equipment.destroyed)
		{
			equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.SuitBatteryLow);
			equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.SuitBatteryOut);
			NameDisplayScreen.Instance.SetSuitBatteryDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), null, bVisible: false);
		}
		if (leadSuitMonitor != null)
		{
			leadSuitMonitor.StopSM("Removed leadsuit tank");
			leadSuitMonitor = null;
		}
	}
}
