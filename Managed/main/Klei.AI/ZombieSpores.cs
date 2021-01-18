using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
	public class ZombieSpores : Disease
	{
		public const string ID = "ZombieSpores";

		public ZombieSpores(bool statsOnly)
			: base("ZombieSpores", 50, new RangeInfo(168.15f, 258.15f, 513.15f, 563.15f), new RangeInfo(10f, 1200f, 1200f, 10f), new RangeInfo(0f, 0f, 1000f, 1000f), RangeInfo.Idempotent(), statsOnly)
		{
		}

		protected override void PopulateElemGrowthInfo()
		{
			InitializeElemGrowthArray(ref elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
			AddGrowthRule(new GrowthRule
			{
				underPopulationDeathRate = 2.6666667f,
				minCountPerKG = 0.4f,
				populationHalfLife = 12000f,
				maxCountPerKG = 500f,
				overPopulationHalfLife = 1200f,
				minDiffusionCount = 1000,
				diffusionScale = 0.001f,
				minDiffusionInfestationTickCount = (byte)1
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Solid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 3000f,
				overPopulationHalfLife = 1200f,
				diffusionScale = 1E-06f,
				minDiffusionCount = 1000000
			});
			SimHashes[] array = new SimHashes[2]
			{
				SimHashes.Carbon,
				SimHashes.Diamond
			};
			foreach (SimHashes element in array)
			{
				AddGrowthRule(new ElementGrowthRule(element)
				{
					underPopulationDeathRate = 0f,
					populationHalfLife = float.PositiveInfinity,
					overPopulationHalfLife = 3000f,
					maxCountPerKG = 1000f,
					diffusionScale = 0.005f
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.BleachStone)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 100000,
				diffusionScale = 0.001f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Gas)
			{
				minCountPerKG = 250f,
				populationHalfLife = 12000f,
				overPopulationHalfLife = 1200f,
				maxCountPerKG = 10000f,
				minDiffusionCount = 5100,
				diffusionScale = 0.005f
			});
			SimHashes[] array2 = new SimHashes[3]
			{
				SimHashes.CarbonDioxide,
				SimHashes.Methane,
				SimHashes.SourGas
			};
			foreach (SimHashes element2 in array2)
			{
				AddGrowthRule(new ElementGrowthRule(element2)
				{
					underPopulationDeathRate = 0f,
					populationHalfLife = float.PositiveInfinity,
					overPopulationHalfLife = 6000f
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 100000,
				diffusionScale = 0.001f
			});
			AddGrowthRule(new StateGrowthRule(Element.State.Liquid)
			{
				minCountPerKG = 0.4f,
				populationHalfLife = 1200f,
				overPopulationHalfLife = 300f,
				maxCountPerKG = 100f,
				diffusionScale = 0.01f
			});
			SimHashes[] array3 = new SimHashes[4]
			{
				SimHashes.CrudeOil,
				SimHashes.Petroleum,
				SimHashes.Naphtha,
				SimHashes.LiquidMethane
			};
			foreach (SimHashes element3 in array3)
			{
				AddGrowthRule(new ElementGrowthRule(element3)
				{
					populationHalfLife = float.PositiveInfinity,
					overPopulationHalfLife = 6000f,
					maxCountPerKG = 1000f,
					diffusionScale = 0.005f
				});
			}
			AddGrowthRule(new ElementGrowthRule(SimHashes.Chlorine)
			{
				populationHalfLife = 10f,
				overPopulationHalfLife = 10f,
				minDiffusionCount = 100000,
				diffusionScale = 0.001f
			});
			InitializeElemExposureArray(ref elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
			AddExposureRule(new ExposureRule
			{
				populationHalfLife = float.PositiveInfinity
			});
			AddExposureRule(new ElementExposureRule(SimHashes.Chlorine)
			{
				populationHalfLife = 10f
			});
			AddExposureRule(new ElementExposureRule(SimHashes.ChlorineGas)
			{
				populationHalfLife = 10f
			});
		}
	}
}
