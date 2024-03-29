using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EdiblesManager")]
public class EdiblesManager : KMonoBehaviour
{
	public class FoodInfo : IConsumableUIItem
	{
		public string Id;

		public string DlcId;

		public string Name;

		public string Description;

		public float CaloriesPerUnit;

		public float PreserveTemperature;

		public float RotTemperature;

		public float StaleTime;

		public float SpoilTime;

		public bool CanRot;

		public int Quality;

		public List<string> Effects;

		public string ConsumableId => Id;

		public string ConsumableName => Name;

		public int MajorOrder => Quality;

		public int MinorOrder => (int)CaloriesPerUnit;

		public bool Display => CaloriesPerUnit != 0f;

		public FoodInfo(string id, string dlcId, float caloriesPerUnit, int quality, float preserveTemperatue, float rotTemperature, float spoilTime, bool can_rot)
		{
			Id = id;
			DlcId = dlcId;
			CaloriesPerUnit = caloriesPerUnit;
			Quality = quality;
			PreserveTemperature = preserveTemperatue;
			RotTemperature = rotTemperature;
			StaleTime = spoilTime / 2f;
			SpoilTime = spoilTime;
			CanRot = can_rot;
			Name = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".NAME");
			Description = Strings.Get("STRINGS.ITEMS.FOOD." + id.ToUpper() + ".DESC");
			Effects = new List<string>();
			s_allFoodTypes.Add(this);
			s_allFoodMap[Id] = this;
		}

		public FoodInfo AddEffects(List<string> effects, string[] dlcIds)
		{
			if (DlcManager.IsDlcListValidForCurrentContent(dlcIds))
			{
				Effects.AddRange(effects);
			}
			return this;
		}
	}

	private static List<FoodInfo> s_allFoodTypes = new List<FoodInfo>();

	private static Dictionary<string, FoodInfo> s_allFoodMap = new Dictionary<string, FoodInfo>();

	public static List<FoodInfo> GetAllFoodTypes()
	{
		return s_allFoodTypes.Where((FoodInfo x) => DlcManager.IsContentActive(x.DlcId)).ToList();
	}

	public static FoodInfo GetFoodInfo(string foodID)
	{
		string key = foodID.Replace("Compost", "");
		FoodInfo value = null;
		s_allFoodMap.TryGetValue(key, out value);
		return value;
	}
}
