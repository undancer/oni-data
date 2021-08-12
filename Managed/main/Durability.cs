using Klei.CustomSettings;
using KSerialization;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Durability")]
public class Durability : KMonoBehaviour
{
	private static readonly EventSystem.IntraObjectHandler<Durability> OnEquippedDelegate = new EventSystem.IntraObjectHandler<Durability>(delegate(Durability component, object data)
	{
		component.OnEquipped();
	});

	private static readonly EventSystem.IntraObjectHandler<Durability> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<Durability>(delegate(Durability component, object data)
	{
		component.OnUnequipped();
	});

	[Serialize]
	private bool isEquipped;

	[Serialize]
	private float timeEquipped;

	[Serialize]
	private float durability = 1f;

	public float durabilityLossPerCycle = -0.1f;

	public string wornEquipmentPrefabID;

	private float difficultySettingMod = 1f;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-1617557748, OnEquippedDelegate);
		Subscribe(-170173755, OnUnequippedDelegate);
	}

	protected override void OnSpawn()
	{
		GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.Durability, base.gameObject);
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Durability);
		if (currentQualitySetting != null)
		{
			switch (currentQualitySetting.id)
			{
			case "Indestructible":
				difficultySettingMod = EQUIPMENT.SUITS.INDESTRUCTIBLE_DURABILITY_MOD;
				break;
			case "Reinforced":
				difficultySettingMod = EQUIPMENT.SUITS.REINFORCED_DURABILITY_MOD;
				break;
			case "Flimsy":
				difficultySettingMod = EQUIPMENT.SUITS.FLIMSY_DURABILITY_MOD;
				break;
			case "Threadbare":
				difficultySettingMod = EQUIPMENT.SUITS.THREADBARE_DURABILITY_MOD;
				break;
			}
		}
	}

	private void OnEquipped()
	{
		if (!isEquipped)
		{
			isEquipped = true;
			timeEquipped = GameClock.Instance.GetTimeInCycles();
		}
	}

	private void OnUnequipped()
	{
		if (isEquipped)
		{
			isEquipped = false;
			float num = GameClock.Instance.GetTimeInCycles() - timeEquipped;
			DeltaDurability(num * durabilityLossPerCycle);
		}
	}

	private void DeltaDurability(float delta)
	{
		delta *= difficultySettingMod;
		durability = Mathf.Clamp01(durability + delta);
	}

	public void ConvertToWornObject()
	{
		GameObject obj = GameUtil.KInstantiate(Assets.GetPrefab(wornEquipmentPrefabID), Grid.SceneLayer.Ore);
		obj.transform.SetPosition(base.transform.GetPosition());
		obj.GetComponent<PrimaryElement>().SetElement(GetComponent<PrimaryElement>().ElementID, addTags: false);
		obj.SetActive(value: true);
		Storage component = base.gameObject.GetComponent<Storage>();
		if ((bool)component)
		{
			JetSuitTank component2 = base.gameObject.GetComponent<JetSuitTank>();
			if ((bool)component2)
			{
				component.AddLiquid(SimHashes.Petroleum, component2.amount, GetComponent<PrimaryElement>().Temperature, byte.MaxValue, 0);
			}
			component.DropAll();
		}
		Util.KDestroyGameObject(base.gameObject);
	}

	public float GetDurability()
	{
		if (isEquipped)
		{
			float num = GameClock.Instance.GetTimeInCycles() - timeEquipped;
			return durability - num * durabilityLossPerCycle;
		}
		return durability;
	}

	public bool IsWornOut()
	{
		return GetDurability() <= 0f;
	}
}
