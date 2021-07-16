using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/game/MedicinalPill")]
public class MedicinalPill : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public MedicineInfo info;

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public List<Descriptor> EffectDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (string.IsNullOrEmpty(info.doctorStationId))
		{
			if (info.medicineType == MedicineInfo.MedicineType.Booster)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_BOOSTER), string.Format(DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_BOOSTER_TOOLTIP)));
			}
			else
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_CURE), string.Format(DUPLICANTS.DISEASES.MEDICINE.SELF_ADMINISTERED_CURE_TOOLTIP)));
			}
		}
		else
		{
			string properName = Assets.GetPrefab(info.doctorStationId).GetProperName();
			if (info.medicineType == MedicineInfo.MedicineType.Booster)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_BOOSTER.Replace("{Station}", properName)), string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_BOOSTER_TOOLTIP.Replace("{Station}", properName))));
			}
			else
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_CURE.Replace("{Station}", properName)), string.Format(DUPLICANTS.DISEASES.MEDICINE.DOCTOR_ADMINISTERED_CURE_TOOLTIP.Replace("{Station}", properName))));
			}
		}
		switch (info.medicineType)
		{
		case MedicineInfo.MedicineType.CureAny:
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_ANY_TOOLTIP)));
			break;
		case MedicineInfo.MedicineType.CureSpecific:
		{
			List<string> list2 = new List<string>();
			foreach (string curedSickness in info.curedSicknesses)
			{
				list2.Add(Strings.Get("STRINGS.DUPLICANTS.DISEASES." + curedSickness.ToUpper() + ".NAME"));
			}
			string arg = string.Join(",", list2.ToArray());
			list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES, arg), string.Format(DUPLICANTS.DISEASES.MEDICINE.CURES_TOOLTIP, arg)));
			break;
		}
		}
		if (!string.IsNullOrEmpty(info.effect))
		{
			Effect effect = Db.Get().effects.Get(info.effect);
			list.Add(new Descriptor(string.Format(DUPLICANTS.MODIFIERS.MEDICINE_GENERICPILL.EFFECT_DESC, effect.Name), $"{effect.description}\n{Effect.CreateTooltip(effect, showDuration: true)}"));
		}
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return EffectDescriptors(go);
	}
}
