using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SuitTank")]
public class SuitTank : KMonoBehaviour, IGameObjectEffectDescriptor, OxygenBreather.IGasProvider
{
	[Serialize]
	public string element;

	[Serialize]
	public float amount;

	public float capacity;

	public const float REFILL_PERCENT = 0.25f;

	public bool underwaterSupport;

	private SuitSuffocationMonitor.Instance suitSuffocationMonitor;

	private static readonly EventSystem.IntraObjectHandler<SuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>(delegate(SuitTank component, object data)
	{
		component.OnEquipped(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>(delegate(SuitTank component, object data)
	{
		component.OnUnequipped(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		amount = capacity;
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	public float PercentFull()
	{
		return amount / capacity;
	}

	public bool IsEmpty()
	{
		return amount <= 0f;
	}

	public bool IsFull()
	{
		return PercentFull() >= 1f;
	}

	public bool NeedsRecharging()
	{
		return PercentFull() < 0.25f;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (element.ToLower() == "oxygen")
		{
			string text = (underwaterSupport ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, GameUtil.GetFormattedMass(amount)) : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, GameUtil.GetFormattedMass(amount)));
			list.Add(new Descriptor(text, text));
		}
		return list;
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), PercentFull, bVisible: true);
		OxygenBreather component = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<OxygenBreather>();
		if (component != null)
		{
			component.SetGasProvider(this);
		}
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		if (!equipment.destroyed)
		{
			NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), PercentFull, bVisible: false);
			OxygenBreather component = equipment.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<OxygenBreather>();
			if (component != null)
			{
				component.SetGasProvider(new GasBreatherFromWorldProvider());
			}
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
		suitSuffocationMonitor = new SuitSuffocationMonitor.Instance(oxygen_breather, this);
		suitSuffocationMonitor.StartSM();
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
		suitSuffocationMonitor.StopSM("Removed suit tank");
		suitSuffocationMonitor = null;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
	{
		if (IsEmpty())
		{
			return false;
		}
		gas_consumed = Mathf.Min(gas_consumed, amount);
		amount -= gas_consumed;
		Game.Instance.accumulators.Accumulate(oxygen_breather.O2Accumulator, gas_consumed);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, 0f - gas_consumed, oxygen_breather.GetProperName());
		return true;
	}

	public bool ShouldEmitCO2()
	{
		return !GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);
	}

	public bool ShouldStoreCO2()
	{
		return GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);
	}

	[ContextMenu("SetToRefillAmount")]
	public void SetToRefillAmount()
	{
		amount = 0.25f * capacity;
	}

	[ContextMenu("Empty")]
	public void Empty()
	{
		amount = 0f;
	}
}
