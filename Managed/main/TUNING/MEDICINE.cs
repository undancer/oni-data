namespace TUNING
{
	public class MEDICINE
	{
		public const float DEFAULT_MASS = 1f;

		public const float RECUPERATION_DISEASE_MULTIPLIER = 1.1f;

		public const float RECUPERATION_DOCTORED_DISEASE_MULTIPLIER = 1.2f;

		public const float WORK_TIME = 10f;

		public static readonly MedicineInfo BASICBOOSTER = new MedicineInfo("BasicBooster", "Medicine_BasicBooster", MedicineInfo.MedicineType.Booster, null);

		public static readonly MedicineInfo INTERMEDIATEBOOSTER = new MedicineInfo("IntermediateBooster", "Medicine_IntermediateBooster", MedicineInfo.MedicineType.Booster, null);

		public static readonly MedicineInfo BASICCURE = new MedicineInfo("BasicCure", null, MedicineInfo.MedicineType.CureSpecific, null, new string[1]
		{
			"FoodSickness"
		});

		public static readonly MedicineInfo ANTIHISTAMINE = new MedicineInfo("Antihistamine", "HistamineSuppression", MedicineInfo.MedicineType.CureSpecific, null, new string[1]
		{
			"Allergies"
		});

		public static readonly MedicineInfo INTERMEDIATECURE = new MedicineInfo("IntermediateCure", null, MedicineInfo.MedicineType.CureSpecific, "DoctorStation", new string[1]
		{
			"SlimeSickness"
		});

		public static readonly MedicineInfo ADVANCEDCURE = new MedicineInfo("AdvancedCure", null, MedicineInfo.MedicineType.CureSpecific, "AdvancedDoctorStation", new string[1]
		{
			"SlimeSickness"
		});

		public static readonly MedicineInfo BASICRADPILL = new MedicineInfo("BasicRadPill", "Medicine_BasicRadPill", MedicineInfo.MedicineType.Booster, null);

		public static readonly MedicineInfo INTERMEDIATERADPILL = new MedicineInfo("IntermediateRadPill", "Medicine_IntermediateRadPill", MedicineInfo.MedicineType.Booster, "AdvancedDoctorStation");
	}
}
