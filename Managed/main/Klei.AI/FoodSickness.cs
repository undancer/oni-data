using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class FoodSickness : Sickness
	{
		public const string ID = "FoodSickness";

		public const string RECOVERY_ID = "FoodSicknessRecovery";

		private const float VOMIT_FREQUENCY = 200f;

		public FoodSickness()
			: base("FoodSickness", SicknessType.Pathogen, Severity.Minor, 0.005f, new List<InfectionVector>
			{
				InfectionVector.Digestion
			}, 1020f, "FoodSicknessRecovery")
		{
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[3]
			{
				new AttributeModifier("BladderDelta", 0.33333334f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME),
				new AttributeModifier("ToiletEfficiency", -0.2f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME),
				new AttributeModifier("StaminaDelta", -0.05f, DUPLICANTS.DISEASES.FOODSICKNESS.NAME)
			}));
			AddSicknessComponent(new AnimatedSickness(new HashedString[1]
			{
				"anim_idle_sick_kanim"
			}, Db.Get().Expressions.Sick));
			AddSicknessComponent(new PeriodicEmoteSickness("anim_idle_sick_kanim", new HashedString[3]
			{
				"idle_pre",
				"idle_default",
				"idle_pst"
			}, 10f));
		}
	}
}
