using System.Collections.Generic;
using STRINGS;

namespace Klei.AI
{
	public class ColdBrain : Sickness
	{
		public const string ID = "ColdSickness";

		public ColdBrain()
			: base("ColdSickness", SicknessType.Ailment, Severity.Minor, 0.005f, new List<InfectionVector> { InfectionVector.Inhalation }, 180f)
		{
			AddSicknessComponent(new CommonSickEffectSickness());
			AddSicknessComponent(new AttributeModifierSickness(new AttributeModifier[5]
			{
				new AttributeModifier("Learning", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME),
				new AttributeModifier("Machinery", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME),
				new AttributeModifier("Construction", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME),
				new AttributeModifier("Cooking", -5f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME),
				new AttributeModifier("Sneezyness", 1f, DUPLICANTS.DISEASES.COLDSICKNESS.NAME)
			}));
			AddSicknessComponent(new AnimatedSickness(new HashedString[3] { "anim_idle_cold_kanim", "anim_loco_run_cold_kanim", "anim_loco_walk_cold_kanim" }, Db.Get().Expressions.SickCold));
			AddSicknessComponent(new PeriodicEmoteSickness("anim_idle_cold_kanim", new HashedString[3] { "idle_pre", "idle_default", "idle_pst" }, 15f));
		}
	}
}
