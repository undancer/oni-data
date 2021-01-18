using Boo.Lang;
using Klei.AI;
using STRINGS;
using UnityEngine;

namespace Database
{
	public class PlantMutations : ResourceSet<PlantMutation>
	{
		public PlantMutations(ResourceSet parent)
			: base("PlantMutations", parent)
		{
			new PlantMutation("basicMutationA", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_A.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_A.DESCRIPTION, "YieldBonusDelta", 2f, positiveTrait: true, fertilization: false, irrigation: false, this);
			new PlantMutation("basicMutationB", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_B.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_B.DESCRIPTION, "TemperatureMax", 2f, positiveTrait: true, fertilization: false, irrigation: false, this);
			new PlantMutation("basicMutationC", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_C.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_C.DESCRIPTION, "FertilizationDelta", 2f, positiveTrait: false, fertilization: true, irrigation: false, this);
			new PlantMutation("basicMutationD", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_D.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_D.DESCRIPTION, "IrrigationDelta", 2f, positiveTrait: false, fertilization: false, irrigation: true, this);
			new PlantMutation("basicMutationE", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_E.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_E.DESCRIPTION, "FertilizationDelta", -2f, positiveTrait: true, fertilization: true, irrigation: false, this);
			new PlantMutation("basicMutationF", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_F.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_F.DESCRIPTION, "IrrigationDelta", -2f, positiveTrait: true, fertilization: false, irrigation: true, this);
			new PlantMutation("basicMutationG", CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_G.NAME, CREATURES.PLANT_MUTATIONS.BASIC_MUTATION_G.DESCRIPTION, "MaturityMax", -1f, positiveTrait: true, fertilization: false, irrigation: false, this);
		}

		public static Trait GetPlantMutation(PlantSubSpeciesCatalog.PlantSubSpecies fromSubSpecies, bool irrigation, bool fertilization)
		{
			List<PlantMutation> list = new List<PlantMutation>();
			foreach (PlantMutation resource in Db.Get().PlantMutations.resources)
			{
				if ((irrigation || !resource.irrigation) && (fertilization || !resource.fertilization) && !fromSubSpecies.mutations.Contains(resource.trait))
				{
					list.Add(resource);
				}
			}
			if (list.Count > 0)
			{
				return list[Random.Range(0, list.Count)].trait;
			}
			return null;
		}
	}
}
