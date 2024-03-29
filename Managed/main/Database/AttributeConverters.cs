using System.Collections.Generic;
using Klei.AI;
using STRINGS;

namespace Database
{
	public class AttributeConverters : ResourceSet<AttributeConverter>
	{
		public AttributeConverter MovementSpeed;

		public AttributeConverter ConstructionSpeed;

		public AttributeConverter DiggingSpeed;

		public AttributeConverter MachinerySpeed;

		public AttributeConverter HarvestSpeed;

		public AttributeConverter PlantTendSpeed;

		public AttributeConverter CompoundingSpeed;

		public AttributeConverter ResearchSpeed;

		public AttributeConverter TrainingSpeed;

		public AttributeConverter CookingSpeed;

		public AttributeConverter ArtSpeed;

		public AttributeConverter DoctorSpeed;

		public AttributeConverter TidyingSpeed;

		public AttributeConverter AttackDamage;

		public AttributeConverter PilotingSpeed;

		public AttributeConverter ImmuneLevelBoost;

		public AttributeConverter ToiletSpeed;

		public AttributeConverter CarryAmountFromStrength;

		public AttributeConverter TemperatureInsulation;

		public AttributeConverter SeedHarvestChance;

		public AttributeConverter RanchingEffectDuration;

		public AttributeConverter FarmedEffectDuration;

		public AttributeConverter PowerTinkerEffectDuration;

		public AttributeConverter CapturableSpeed;

		public AttributeConverter Create(string id, string name, string description, Attribute attribute, float multiplier, float base_value, IAttributeFormatter formatter, string[] available_dlcs)
		{
			AttributeConverter attributeConverter = new AttributeConverter(id, name, description, multiplier, base_value, attribute, formatter);
			if (DlcManager.IsDlcListValidForCurrentContent(available_dlcs))
			{
				Add(attributeConverter);
				attribute.converters.Add(attributeConverter);
			}
			return attributeConverter;
		}

		public AttributeConverters()
		{
			ToPercentAttributeFormatter formatter = new ToPercentAttributeFormatter(1f);
			StandardAttributeFormatter formatter2 = new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None);
			MovementSpeed = Create("MovementSpeed", "Movement Speed", DUPLICANTS.ATTRIBUTES.ATHLETICS.SPEEDMODIFIER, Db.Get().Attributes.Athletics, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			ConstructionSpeed = Create("ConstructionSpeed", "Construction Speed", DUPLICANTS.ATTRIBUTES.CONSTRUCTION.SPEEDMODIFIER, Db.Get().Attributes.Construction, 0.25f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			DiggingSpeed = Create("DiggingSpeed", "Digging Speed", DUPLICANTS.ATTRIBUTES.DIGGING.SPEEDMODIFIER, Db.Get().Attributes.Digging, 0.25f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			MachinerySpeed = Create("MachinerySpeed", "Machinery Speed", DUPLICANTS.ATTRIBUTES.MACHINERY.SPEEDMODIFIER, Db.Get().Attributes.Machinery, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			HarvestSpeed = Create("HarvestSpeed", "Harvest Speed", DUPLICANTS.ATTRIBUTES.BOTANIST.HARVEST_SPEED_MODIFIER, Db.Get().Attributes.Botanist, 0.05f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			PlantTendSpeed = Create("PlantTendSpeed", "Plant Tend Speed", DUPLICANTS.ATTRIBUTES.BOTANIST.TINKER_MODIFIER, Db.Get().Attributes.Botanist, 0.025f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			CompoundingSpeed = Create("CompoundingSpeed", "Compounding Speed", DUPLICANTS.ATTRIBUTES.CARING.FABRICATE_SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			ResearchSpeed = Create("ResearchSpeed", "Research Speed", DUPLICANTS.ATTRIBUTES.LEARNING.RESEARCHSPEED, Db.Get().Attributes.Learning, 0.4f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			TrainingSpeed = Create("TrainingSpeed", "Training Speed", DUPLICANTS.ATTRIBUTES.LEARNING.SPEEDMODIFIER, Db.Get().Attributes.Learning, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			CookingSpeed = Create("CookingSpeed", "Cooking Speed", DUPLICANTS.ATTRIBUTES.COOKING.SPEEDMODIFIER, Db.Get().Attributes.Cooking, 0.05f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			ArtSpeed = Create("ArtSpeed", "Art Speed", DUPLICANTS.ATTRIBUTES.ART.SPEEDMODIFIER, Db.Get().Attributes.Art, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			DoctorSpeed = Create("DoctorSpeed", "Doctor Speed", DUPLICANTS.ATTRIBUTES.CARING.SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.2f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			TidyingSpeed = Create("TidyingSpeed", "Tidying Speed", DUPLICANTS.ATTRIBUTES.STRENGTH.SPEEDMODIFIER, Db.Get().Attributes.Strength, 0.25f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			AttackDamage = Create("AttackDamage", "Attack Damage", DUPLICANTS.ATTRIBUTES.DIGGING.ATTACK_MODIFIER, Db.Get().Attributes.Digging, 0.05f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			PilotingSpeed = Create("PilotingSpeed", "Piloting Speed", DUPLICANTS.ATTRIBUTES.SPACENAVIGATION.SPEED_MODIFIER, Db.Get().Attributes.SpaceNavigation, 0.025f, 0f, formatter, DlcManager.AVAILABLE_EXPANSION1_ONLY);
			ImmuneLevelBoost = Create("ImmuneLevelBoost", "Immune Level Boost", DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_MODIFIER, Db.Get().Attributes.Immunity, 0.0016666667f, 0f, new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.PerCycle), DlcManager.AVAILABLE_ALL_VERSIONS);
			ToiletSpeed = Create("ToiletSpeed", "Toilet Speed", "", Db.Get().Attributes.ToiletEfficiency, 1f, -1f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			CarryAmountFromStrength = Create("CarryAmountFromStrength", "Carry Amount", DUPLICANTS.ATTRIBUTES.STRENGTH.CARRYMODIFIER, Db.Get().Attributes.Strength, 40f, 0f, formatter2, DlcManager.AVAILABLE_ALL_VERSIONS);
			TemperatureInsulation = Create("TemperatureInsulation", "Temperature Insulation", DUPLICANTS.ATTRIBUTES.INSULATION.SPEEDMODIFIER, Db.Get().Attributes.Insulation, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			SeedHarvestChance = Create("SeedHarvestChance", "Seed Harvest Chance", DUPLICANTS.ATTRIBUTES.BOTANIST.BONUS_SEEDS, Db.Get().Attributes.Botanist, 0.033f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			CapturableSpeed = Create("CapturableSpeed", "Capturable Speed", DUPLICANTS.ATTRIBUTES.RANCHING.CAPTURABLESPEED, Db.Get().Attributes.Ranching, 0.05f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			RanchingEffectDuration = Create("RanchingEffectDuration", "Ranching Effect Duration", DUPLICANTS.ATTRIBUTES.RANCHING.EFFECTMODIFIER, Db.Get().Attributes.Ranching, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			FarmedEffectDuration = Create("FarmedEffectDuration", "Farmer's Touch Duration", DUPLICANTS.ATTRIBUTES.BOTANIST.TINKER_EFFECT_MODIFIER, Db.Get().Attributes.Botanist, 0.1f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
			PowerTinkerEffectDuration = Create("PowerTinkerEffectDuration", "Engie's Tune-Up Effect Duration", DUPLICANTS.ATTRIBUTES.MACHINERY.TINKER_EFFECT_MODIFIER, Db.Get().Attributes.Machinery, 0.025f, 0f, formatter, DlcManager.AVAILABLE_ALL_VERSIONS);
		}

		public List<AttributeConverter> GetConvertersForAttribute(Attribute attrib)
		{
			List<AttributeConverter> list = new List<AttributeConverter>();
			foreach (AttributeConverter resource in resources)
			{
				if (resource.attribute == attrib)
				{
					list.Add(resource);
				}
			}
			return list;
		}
	}
}
