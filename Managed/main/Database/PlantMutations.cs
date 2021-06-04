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
				.VisualTint(-0.4f, -0.4f, -0.4f);
			moderatelyTight = AddPlantMutation("moderatelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.5f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 0.5f, multiplier: true)
				.VisualTint(0.2f, 0.2f, 0.2f);
			extremelyTight = AddPlantMutation("extremelyTight").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.WiltTempRangeMod, -0.8f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, multiplier: true)
				.VisualTint(0.3f, 0.3f, 0.3f)
				.VisualBGFX("mutate_glow_fx_kanim");
			bonusLice = AddPlantMutation("bonusLice").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true).BonusCrop("BasicPlantFood", 1f)
				.VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "meal_lice_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "meal_lice_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_MealLice"));
			sunnySpeed = AddPlantMutation("sunnySpeed").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 1000f).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.5f, multiplier: true)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true)
				.VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "leaf_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "leaf_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_Leaf"));
			slowBurn = AddPlantMutation("slowBurn").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, -0.9f, multiplier: true).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, 3.5f, multiplier: true)
				.VisualTint(-0.3f, -0.3f, -0.5f);
			blooms = AddPlantMutation("blooms").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().BuildingAttributes.Decor, 20f).VisualSymbolOverride("snapTo_mutate1", "mutate_snaps_kanim", "blossom_mutate1")
				.VisualSymbolOverride("snapTo_mutate2", "mutate_snaps_kanim", "blossom_mutate2")
				.AddSoundEvent(GlobalAssets.GetSound("Plant_mutation_PrickleFlower"));
			loadedWithFruit = AddPlantMutation("loadedWithFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.YieldAmount, 1f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.HarvestTime, 4f, multiplier: true)
				.AttributeModifier(Db.Get().PlantAttributes.MinLightLux, 200f)
				.AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.2f, multiplier: true)
				.VisualSymbolScale("swap_crop01", 1.3f)
				.VisualSymbolScale("swap_crop02", 1.3f);
			rottenHeaps = AddPlantMutation("rottenHeaps").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute, -0.75f, multiplier: true).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.5f, multiplier: true)
				.BonusCrop(RotPileConfig.ID, 4f)
				.AddDiseaseToHarvest(Db.Get().Diseases.GetIndex(Db.Get().Diseases.FoodGerms.Id), 10000)
				.ForcePrefersDarkness()
				.RestrictPrefabID("GasGrass")
				.VisualFGFX("mutate_stink_fx_kanim")
				.VisualSymbolTint("swap_crop01", -0.2f, -0.1f, -0.5f)
				.VisualSymbolTint("swap_crop02", -0.2f, -0.1f, -0.5f);
			heavyFruit = AddPlantMutation("heavyFruit").AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold, 25f).AttributeModifier(Db.Get().PlantAttributes.FertilizerUsageMod, 0.25f, multiplier: true).ForceSelfHarvestOnGrown()
				.VisualSymbolTint("swap_crop01", -0.1f, -0.5f, -0.5f)
				.VisualSymbolTint("swap_crop02", -0.1f, -0.5f, -0.5f);
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
