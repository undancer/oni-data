using System.Collections.Generic;
using System.Linq;
using Klei.AI;

namespace Database
{
	public class PlantMutations : ResourceSet<PlantMutation>
	{
		public PlantMutation moderatelyLoose;

		public PlantMutation moderatelyTight;

		public PlantMutation extremelyTight;

		public PlantMutation bonusLice;

		public PlantMutation sunnySpeed;

		public PlantMutation slowBurn;

		public PlantMutation blooms;

		public PlantMutation loadedWithFruit;

		public PlantMutation heavyFruit;

		public PlantMutation rottenHeaps;

		public PlantMutation AddPlantMutation(string id)
		{
			StringEntry entry = Strings.Get(new StringKey("STRINGS.CREATURES.PLANT_MUTATIONS." + id.ToUpper() + ".NAME"));
			StringEntry entry2 = Strings.Get(new StringKey("STRINGS.CREATURES.PLANT_MUTATIONS." + id.ToUpper() + ".DESCRIPTION"));
			PlantMutation plantMutation = new PlantMutation(id, entry, entry2);
			Add(plantMutation);
			return plantMutation;
		}

		public PlantMutations(ResourceSet parent)
			: base("PlantMutations", parent)
		{
			moderatelyLoose = AddPlantMutation("moderatelyLoose").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, 0.5f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, -0.25f, multiplier: true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, -0.5f, multiplier: true)
				.VisualTint(-0.2f, -0.2f, -0.2f);
			moderatelyTight = AddPlantMutation("moderatelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.5f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 0.5f, multiplier: true)
				.VisualTint(0.2f, 0.2f, 0.2f);
			extremelyTight = AddPlantMutation("extremelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.8f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, multiplier: true)
				.VisualTint(0.2f, 0.2f, 0.2f)
				.VisualBGFX("plant_mutation_subtle_glow");
			bonusLice = AddPlantMutation("bonusLice").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true).BonusCrop("BasicPlantFood", 1f)
				.VisualSymbolOverride("mutation_snap_1", "meallice_mutation_snap", "mutation_snap");
			sunnySpeed = AddPlantMutation("sunnySpeed").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 1000f).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.5f, multiplier: true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true)
				.VisualSymbolOverride("mutation_snap_1", "sunleaves_mutation_snap", "mutation_snap");
			slowBurn = AddPlantMutation("slowBurn").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, -0.9f, multiplier: true).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, 3.5f, multiplier: true)
				.VisualTint(-0.3f, -0.3f, -0.3f);
			blooms = AddPlantMutation("blooms").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().BuildingAttributes.Decor, 20f).VisualSymbolOverride("mutation_snap_1", "blooms_mutation_snap", "mutation_snap");
			loadedWithFruit = AddPlantMutation("loadedWithFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.HarvestTime, 4f, multiplier: true)
				.AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 200f)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.2f, multiplier: true)
				.VisualSymbolScale("fruit", 1.5f);
			rottenHeaps = AddPlantMutation("rottenHeaps").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.75f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.5f, multiplier: true)
				.BonusCrop(RotPileConfig.ID, 4f)
				.AddDiseaseToHarvest(Db.Get().Diseases.GetIndex(Db.Get().Diseases.FoodGerms.Id), 10000)
				.ForcePrefersDarkness()
				.VisualFGFX("plant_mutation_stink_clouds");
			heavyFruit = AddPlantMutation("heavyFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true).ForceSelfHarvestOnGrown()
				.VisualSymbolTint("fruit", -0.4f, -0.4f, 0.4f);
		}

		public List<string> GetNamesForMutations(List<string> mutationIDs)
		{
			List<string> list = new List<string>(mutationIDs.Count);
			foreach (string mutationID in mutationIDs)
			{
				list.Add(Get(mutationID).Name);
			}
			return list;
		}

		public PlantMutation GetRandomMutation(string targetPlantPrefabID)
		{
			List<PlantMutation> tList = resources.Where((PlantMutation m) => !m.originalMutation && !m.restrictedPrefabIDs.Contains(targetPlantPrefabID) && (m.requiredPrefabIDs.Count == 0 || m.requiredPrefabIDs.Contains(targetPlantPrefabID))).ToList();
			return tList.GetRandom();
		}
	}
}
