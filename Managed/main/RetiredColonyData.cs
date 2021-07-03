using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class RetiredColonyData
{
	public static class DataIDs
	{
		public static string OxygenProduced = "oxygenProduced";

		public static string OxygenConsumed = "oxygenConsumed";

		public static string CaloriesProduced = "caloriesProduced";

		public static string CaloriesRemoved = "caloriesRemoved";

		public static string PowerProduced = "powerProduced";

		public static string PowerWasted = "powerWasted";

		public static string WorkTime = "workTime";

		public static string TravelTime = "travelTime";

		public static string AverageWorkTime = "averageWorkTime";

		public static string AverageTravelTime = "averageTravelTime";

		public static string LiveDuplicants = "liveDuplicants";

		public static string AverageStressCreated = "averageStressCreated";

		public static string AverageStressRemoved = "averageStressRemoved";

		public static string DomesticatedCritters = "domesticatedCritters";

		public static string WildCritters = "wildCritters";

		public static string AverageGerms = "averageGerms";

		public static string RocketsInFlight = "rocketsInFlight";
	}

	public class RetiredColonyStatistic
	{
		public string id;

		public Tuple<float, float>[] value;

		public string name;

		public string nameX;

		public string nameY;

		public RetiredColonyStatistic()
		{
		}

		public RetiredColonyStatistic(string id, Tuple<float, float>[] data, string name, string axisNameX, string axisNameY)
		{
			this.id = id;
			value = data;
			this.name = name;
			nameX = axisNameX;
			nameY = axisNameY;
		}

		public Tuple<float, float> GetByMaxValue()
		{
			if (value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
			int num = -1;
			float num2 = -1f;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i].second > num2)
				{
					num2 = value[i].second;
					num = i;
				}
			}
			if (num == -1)
			{
				num = 0;
			}
			return value[num];
		}

		public Tuple<float, float> GetByMaxKey()
		{
			if (value.Length == 0)
			{
				return new Tuple<float, float>(0f, 0f);
			}
			int num = -1;
			float num2 = -1f;
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i].first > num2)
				{
					num2 = value[i].first;
					num = i;
				}
			}
			return value[num];
		}
	}

	public class RetiredDuplicantData
	{
		public string name;

		public int age;

		public int skillPointsGained;

		public Dictionary<string, string> accessories;
	}

	public string colonyName
	{
		get;
		set;
	}

	public int cycleCount
	{
		get;
		set;
	}

	public string date
	{
		get;
		set;
	}

	public string[] achievements
	{
		get;
		set;
	}

	public RetiredDuplicantData[] Duplicants
	{
		get;
		set;
	}

	public List<Tuple<string, int>> buildings
	{
		get;
		set;
	}

	public RetiredColonyStatistic[] Stats
	{
		get;
		set;
	}

	public Dictionary<string, string> worldIdentities
	{
		get;
		set;
	}

	public string startWorld
	{
		get;
		set;
	}

	public RetiredColonyData()
	{
	}

	public RetiredColonyData(string colonyName, int cycleCount, string date, string[] achievements, MinionAssignablesProxy[] minions, BuildingComplete[] buildingCompletes, string startWorld, Dictionary<string, string> worldIdentities)
	{
		this.colonyName = colonyName;
		this.cycleCount = cycleCount;
		this.achievements = achievements;
		this.date = date;
		Duplicants = new RetiredDuplicantData[(minions != null) ? minions.Length : 0];
		for (int i = 0; i < Duplicants.Length; i++)
		{
			Duplicants[i] = new RetiredDuplicantData();
			Duplicants[i].name = minions[i].GetProperName();
			Duplicants[i].age = (int)Mathf.Floor((float)GameClock.Instance.GetCycle() - minions[i].GetArrivalTime());
			Duplicants[i].skillPointsGained = minions[i].GetTotalSkillpoints();
			Duplicants[i].accessories = new Dictionary<string, string>();
			if (minions[i].GetTargetGameObject().GetComponent<Accessorizer>() != null)
			{
				foreach (ResourceRef<Accessory> accessory in minions[i].GetTargetGameObject().GetComponent<Accessorizer>().GetAccessories())
				{
					if (accessory.Get() != null)
					{
						Duplicants[i].accessories.Add(accessory.Get().slot.Id, accessory.Get().Id);
					}
				}
				continue;
			}
			StoredMinionIdentity component = minions[i].GetTargetGameObject().GetComponent<StoredMinionIdentity>();
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Eyes.Id, Db.Get().Accessories.Get(component.bodyData.eyes).Id);
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Arm.Id, Db.Get().Accessories.Get(component.bodyData.arms).Id);
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Body.Id, Db.Get().Accessories.Get(component.bodyData.body).Id);
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hair.Id, Db.Get().Accessories.Get(component.bodyData.hair).Id);
			if (component.bodyData.hat != HashedString.Invalid)
			{
				Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Hat.Id, Db.Get().Accessories.Get(component.bodyData.hat).Id);
			}
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.HeadShape.Id, Db.Get().Accessories.Get(component.bodyData.headShape).Id);
			Duplicants[i].accessories.Add(Db.Get().AccessorySlots.Mouth.Id, Db.Get().Accessories.Get(component.bodyData.mouth).Id);
		}
		Dictionary<Tag, int> dictionary = new Dictionary<Tag, int>();
		if (buildingCompletes != null)
		{
			foreach (BuildingComplete cmp in buildingCompletes)
			{
				if (!dictionary.ContainsKey(cmp.PrefabID()))
				{
					dictionary[cmp.PrefabID()] = 0;
				}
				dictionary[cmp.PrefabID()]++;
			}
		}
		buildings = new List<Tuple<string, int>>();
		foreach (KeyValuePair<Tag, int> item in dictionary)
		{
			buildings.Add(new Tuple<string, int>(item.Key.ToString(), item.Value));
		}
		Stats = null;
		Tuple<float, float>[] array = null;
		Tuple<float, float>[] array2 = null;
		Tuple<float, float>[] array3 = null;
		Tuple<float, float>[] array4 = null;
		Tuple<float, float>[] array5 = null;
		Tuple<float, float>[] array6 = null;
		Tuple<float, float>[] array7 = null;
		Tuple<float, float>[] array8 = null;
		Tuple<float, float>[] array9 = null;
		Tuple<float, float>[] array10 = null;
		Tuple<float, float>[] array11 = null;
		Tuple<float, float>[] array12 = null;
		Tuple<float, float>[] array13 = null;
		Tuple<float, float>[] array14 = null;
		Tuple<float, float>[] array15 = null;
		Tuple<float, float>[] array16 = null;
		if (!(ReportManager.Instance != null))
		{
			return;
		}
		array = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int k = 0; k < array.Length; k++)
		{
			array[k] = new Tuple<float, float>(ReportManager.Instance.reports[k].day, ReportManager.Instance.reports[k].GetEntry(ReportManager.ReportType.OxygenCreated).accPositive);
		}
		array2 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int l = 0; l < array2.Length; l++)
		{
			array2[l] = new Tuple<float, float>(ReportManager.Instance.reports[l].day, ReportManager.Instance.reports[l].GetEntry(ReportManager.ReportType.OxygenCreated).accNegative * -1f);
		}
		array3 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int m = 0; m < array3.Length; m++)
		{
			array3[m] = new Tuple<float, float>(ReportManager.Instance.reports[m].day, ReportManager.Instance.reports[m].GetEntry(ReportManager.ReportType.CaloriesCreated).accPositive * 0.001f);
		}
		array4 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int n = 0; n < array4.Length; n++)
		{
			array4[n] = new Tuple<float, float>(ReportManager.Instance.reports[n].day, ReportManager.Instance.reports[n].GetEntry(ReportManager.ReportType.CaloriesCreated).accNegative * 0.001f * -1f);
		}
		array5 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num = 0; num < array5.Length; num++)
		{
			array5[num] = new Tuple<float, float>(ReportManager.Instance.reports[num].day, ReportManager.Instance.reports[num].GetEntry(ReportManager.ReportType.EnergyCreated).accPositive * 0.001f);
		}
		array6 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num2 = 0; num2 < array6.Length; num2++)
		{
			array6[num2] = new Tuple<float, float>(ReportManager.Instance.reports[num2].day, ReportManager.Instance.reports[num2].GetEntry(ReportManager.ReportType.EnergyWasted).accNegative * -1f * 0.001f);
		}
		array7 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num3 = 0; num3 < array7.Length; num3++)
		{
			array7[num3] = new Tuple<float, float>(ReportManager.Instance.reports[num3].day, ReportManager.Instance.reports[num3].GetEntry(ReportManager.ReportType.WorkTime).accPositive);
		}
		array9 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num4 = 0; num4 < array7.Length; num4++)
		{
			int num5 = 0;
			float num6 = 0f;
			ReportManager.ReportEntry entry = ReportManager.Instance.reports[num4].GetEntry(ReportManager.ReportType.WorkTime);
			for (int num7 = 0; num7 < entry.contextEntries.Count; num7++)
			{
				num5++;
				num6 += entry.contextEntries[num7].accPositive;
			}
			num6 /= (float)num5;
			num6 /= 600f;
			num6 *= 100f;
			array9[num4] = new Tuple<float, float>(ReportManager.Instance.reports[num4].day, num6);
		}
		array8 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num8 = 0; num8 < array8.Length; num8++)
		{
			array8[num8] = new Tuple<float, float>(ReportManager.Instance.reports[num8].day, ReportManager.Instance.reports[num8].GetEntry(ReportManager.ReportType.TravelTime).accPositive);
		}
		array10 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num9 = 0; num9 < array8.Length; num9++)
		{
			int num10 = 0;
			float num11 = 0f;
			ReportManager.ReportEntry entry2 = ReportManager.Instance.reports[num9].GetEntry(ReportManager.ReportType.TravelTime);
			for (int num12 = 0; num12 < entry2.contextEntries.Count; num12++)
			{
				num10++;
				num11 += entry2.contextEntries[num12].accPositive;
			}
			num11 /= (float)num10;
			num11 /= 600f;
			num11 *= 100f;
			array10[num9] = new Tuple<float, float>(ReportManager.Instance.reports[num9].day, num11);
		}
		array11 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num13 = 0; num13 < array7.Length; num13++)
		{
			array11[num13] = new Tuple<float, float>(ReportManager.Instance.reports[num13].day, ReportManager.Instance.reports[num13].GetEntry(ReportManager.ReportType.WorkTime).contextEntries.Count);
		}
		array12 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num14 = 0; num14 < array12.Length; num14++)
		{
			int num15 = 0;
			float num16 = 0f;
			ReportManager.ReportEntry entry3 = ReportManager.Instance.reports[num14].GetEntry(ReportManager.ReportType.StressDelta);
			for (int num17 = 0; num17 < entry3.contextEntries.Count; num17++)
			{
				num15++;
				num16 += entry3.contextEntries[num17].accPositive;
			}
			array12[num14] = new Tuple<float, float>(ReportManager.Instance.reports[num14].day, num16 / (float)num15);
		}
		array13 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num18 = 0; num18 < array13.Length; num18++)
		{
			int num19 = 0;
			float num20 = 0f;
			ReportManager.ReportEntry entry4 = ReportManager.Instance.reports[num18].GetEntry(ReportManager.ReportType.StressDelta);
			for (int num21 = 0; num21 < entry4.contextEntries.Count; num21++)
			{
				num19++;
				num20 += entry4.contextEntries[num21].accNegative;
			}
			num20 *= -1f;
			array13[num18] = new Tuple<float, float>(ReportManager.Instance.reports[num18].day, num20 / (float)num19);
		}
		array14 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num22 = 0; num22 < array14.Length; num22++)
		{
			array14[num22] = new Tuple<float, float>(ReportManager.Instance.reports[num22].day, ReportManager.Instance.reports[num22].GetEntry(ReportManager.ReportType.DomesticatedCritters).accPositive);
		}
		array15 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num23 = 0; num23 < array15.Length; num23++)
		{
			array15[num23] = new Tuple<float, float>(ReportManager.Instance.reports[num23].day, ReportManager.Instance.reports[num23].GetEntry(ReportManager.ReportType.WildCritters).accPositive);
		}
		array16 = new Tuple<float, float>[ReportManager.Instance.reports.Count];
		for (int num24 = 0; num24 < array16.Length; num24++)
		{
			array16[num24] = new Tuple<float, float>(ReportManager.Instance.reports[num24].day, ReportManager.Instance.reports[num24].GetEntry(ReportManager.ReportType.RocketsInFlight).accPositive);
		}
		Stats = new RetiredColonyStatistic[16]
		{
			new RetiredColonyStatistic(DataIDs.OxygenProduced, array, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
			new RetiredColonyStatistic(DataIDs.OxygenConsumed, array2, UI.RETIRED_COLONY_INFO_SCREEN.STATS.OXYGEN_CONSUMED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.MASS.KILOGRAM),
			new RetiredColonyStatistic(DataIDs.CaloriesProduced, array3, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
			new RetiredColonyStatistic(DataIDs.CaloriesRemoved, array4, UI.RETIRED_COLONY_INFO_SCREEN.STATS.CALORIES_CONSUMED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CALORIES.KILOCALORIE),
			new RetiredColonyStatistic(DataIDs.PowerProduced, array5, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
			new RetiredColonyStatistic(DataIDs.PowerWasted, array6, UI.RETIRED_COLONY_INFO_SCREEN.STATS.POWER_WASTED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE),
			new RetiredColonyStatistic(DataIDs.WorkTime, array7, UI.RETIRED_COLONY_INFO_SCREEN.STATS.WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
			new RetiredColonyStatistic(DataIDs.AverageWorkTime, array9, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_WORK_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
			new RetiredColonyStatistic(DataIDs.TravelTime, array8, UI.RETIRED_COLONY_INFO_SCREEN.STATS.TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.SECONDS),
			new RetiredColonyStatistic(DataIDs.AverageTravelTime, array10, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_TRAVEL_TIME, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
			new RetiredColonyStatistic(DataIDs.LiveDuplicants, array11, UI.RETIRED_COLONY_INFO_SCREEN.STATS.LIVE_DUPLICANTS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.DUPLICANTS),
			new RetiredColonyStatistic(DataIDs.RocketsInFlight, array16, UI.RETIRED_COLONY_INFO_SCREEN.STATS.ROCKET_MISSIONS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.ROCKET_MISSIONS),
			new RetiredColonyStatistic(DataIDs.AverageStressCreated, array12, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_CREATED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
			new RetiredColonyStatistic(DataIDs.AverageStressRemoved, array13, UI.RETIRED_COLONY_INFO_SCREEN.STATS.AVERAGE_STRESS_REMOVED, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.PERCENT),
			new RetiredColonyStatistic(DataIDs.DomesticatedCritters, array14, UI.RETIRED_COLONY_INFO_SCREEN.STATS.NUMBER_DOMESTICATED_CRITTERS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CRITTERS),
			new RetiredColonyStatistic(DataIDs.WildCritters, array15, UI.RETIRED_COLONY_INFO_SCREEN.STATS.NUMBER_WILD_CRITTERS, UI.MATH_PICTURES.AXIS_LABELS.CYCLES, UI.UNITSUFFIXES.CRITTERS)
		};
		this.startWorld = startWorld;
		this.worldIdentities = worldIdentities;
	}
}
