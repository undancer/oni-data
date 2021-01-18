using System;
using System.Collections.Generic;

[Serializable]
public class MedicineInfo
{
	public enum MedicineType
	{
		Booster,
		CureAny,
		CureSpecific
	}

	public string id;

	public string effect;

	public MedicineType medicineType;

	public List<string> curedSicknesses;

	public string doctorStationId;

	public MedicineInfo(string id, string effect, MedicineType medicineType, string doctorStationId, string[] curedDiseases = null)
	{
		Debug.Assert(!string.IsNullOrEmpty(effect) || (curedDiseases != null && curedDiseases.Length != 0), "Medicine should have an effect or cure diseases");
		this.id = id;
		this.effect = effect;
		this.medicineType = medicineType;
		this.doctorStationId = doctorStationId;
		if (curedDiseases != null)
		{
			curedSicknesses = new List<string>(curedDiseases);
		}
		else
		{
			curedSicknesses = new List<string>();
		}
	}

	public Tag GetSupplyTag()
	{
		return GetSupplyTagForStation(doctorStationId);
	}

	public static Tag GetSupplyTagForStation(string stationID)
	{
		return new Tag(stationID + GameTags.MedicalSupplies.Name);
	}
}
