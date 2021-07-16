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

	public Tag elementTag;

	[MyCmpReq]
	public Storage storage;

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
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (amount != 0f)
		{
			storage.AddGasChunk(SimHashes.Oxygen, amount, GetComponent<PrimaryElement>().Temperature, byte.MaxValue, 0, keep_zero_mass: false);
			amount = 0f;
		}
	}

	public float GetTankAmount()
	{
		if (storage == null)
		{
			storage = GetComponent<Storage>();
		}
		return storage.GetMassAvailable(elementTag);
	}

	public float PercentFull()
	{
		return GetTankAmount() / capacity;
	}

	public bool IsEmpty()
	{
		return GetTankAmount() <= 0f;
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
		if (elementTag == GameTags.Breathable)
		{
			string text = (underwaterSupport ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, GameUtil.GetFormattedMass(GetTankAmount())) : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, GameUtil.GetFormattedMass(GetTankAmount())));
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
			component.AddTag(GameTags.HasSuitTank);
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
				component.RemoveTag(GameTags.HasSuitTank);
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
		storage.ConsumeAndGetDisease(elementTag, gas_consumed, out var amount_consumed, out var _, out var _);
		Game.Instance.accumulators.Accumulate(oxygen_breather.O2Accumulator, amount_consumed);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, 0f - amount_consumed, oxygen_breather.GetProperName());
		Trigger(608245985, base.gameObject);
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
		float tankAmount = GetTankAmount();
		float num = 0.25f * capacity;
		if (tankAmount > num)
		{
			storage.ConsumeIgnoringDisease(elementTag, tankAmount - num);
		}
	}

	[ContextMenu("Empty")]
	public void Empty()
	{
		storage.ConsumeIgnoringDisease(elementTag, GetTankAmount());
	}
}
