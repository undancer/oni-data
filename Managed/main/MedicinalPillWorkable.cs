using Klei.AI;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/MedicinalPillWorkable")]
public class MedicinalPillWorkable : Workable, IConsumableUIItem
{
	public MedicinalPill pill;

	public string ConsumableId => this.PrefabID().Name;

	public string ConsumableName => this.GetProperName();

	public int MajorOrder => (int)(pill.info.medicineType + 1000);

	public int MinorOrder => 0;

	public bool Display => true;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		SetWorkTime(10f);
		showProgressBar = false;
		synchronizeAnims = false;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
		CreateChore();
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (!string.IsNullOrEmpty(pill.info.effect))
		{
			Effects component = worker.GetComponent<Effects>();
			EffectInstance effectInstance = component.Get(pill.info.effect);
			if (effectInstance != null)
			{
				effectInstance.timeRemaining = effectInstance.effect.duration;
			}
			else
			{
				component.Add(pill.info.effect, should_save: true);
			}
		}
		Sicknesses sicknesses = worker.GetSicknesses();
		foreach (string curedSickness in pill.info.curedSicknesses)
		{
			SicknessInstance sicknessInstance = sicknesses.Get(curedSickness);
			if (sicknessInstance != null)
			{
				Game.Instance.savedInfo.curedDisease = true;
				sicknessInstance.Cure();
			}
		}
		base.gameObject.DeleteObject();
	}

	private void CreateChore()
	{
		new TakeMedicineChore(this);
	}

	public bool CanBeTakenBy(GameObject consumer)
	{
		if (!string.IsNullOrEmpty(pill.info.effect))
		{
			Effects component = consumer.GetComponent<Effects>();
			if (component == null || component.HasEffect(pill.info.effect))
			{
				return false;
			}
		}
		if (pill.info.medicineType == MedicineInfo.MedicineType.Booster)
		{
			return true;
		}
		Sicknesses sicknesses = consumer.GetSicknesses();
		if (pill.info.medicineType == MedicineInfo.MedicineType.CureAny && sicknesses.Count > 0)
		{
			return true;
		}
		foreach (SicknessInstance item in sicknesses)
		{
			if (pill.info.curedSicknesses.Contains(item.modifier.Id))
			{
				return true;
			}
		}
		return false;
	}
}
