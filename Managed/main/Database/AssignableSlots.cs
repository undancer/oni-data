using STRINGS;
using TUNING;

namespace Database
{
	public class AssignableSlots : ResourceSet<AssignableSlot>
	{
		public AssignableSlot Bed;

		public AssignableSlot MessStation;

		public AssignableSlot Clinic;

		public AssignableSlot GeneShuffler;

		public AssignableSlot MedicalBed;

		public AssignableSlot Toilet;

		public AssignableSlot MassageTable;

		public AssignableSlot RocketCommandModule;

		public AssignableSlot HabitatModule;

		public AssignableSlot ResetSkillsStation;

		public AssignableSlot WarpPortal;

		public AssignableSlot Toy;

		public AssignableSlot Suit;

		public AssignableSlot Tool;

		public AssignableSlot Outfit;

		public AssignableSlots()
		{
			Bed = Add(new OwnableSlot("Bed", MISC.TAGS.BED));
			MessStation = Add(new OwnableSlot("MessStation", MISC.TAGS.MESSSTATION));
			Clinic = Add(new OwnableSlot("Clinic", MISC.TAGS.CLINIC));
			MedicalBed = Add(new OwnableSlot("MedicalBed", MISC.TAGS.CLINIC));
			MedicalBed.showInUI = false;
			GeneShuffler = Add(new OwnableSlot("GeneShuffler", MISC.TAGS.GENE_SHUFFLER));
			GeneShuffler.showInUI = false;
			Toilet = Add(new OwnableSlot("Toilet", MISC.TAGS.TOILET));
			MassageTable = Add(new OwnableSlot("MassageTable", MISC.TAGS.MASSAGE_TABLE));
			RocketCommandModule = Add(new OwnableSlot("RocketCommandModule", MISC.TAGS.COMMAND_MODULE));
			HabitatModule = Add(new OwnableSlot("HabitatModule", MISC.TAGS.HABITAT_MODULE));
			ResetSkillsStation = Add(new OwnableSlot("ResetSkillsStation", "ResetSkillsStation"));
			WarpPortal = Add(new OwnableSlot("WarpPortal", MISC.TAGS.WARP_PORTAL));
			WarpPortal.showInUI = false;
			Toy = Add(new EquipmentSlot(TUNING.EQUIPMENT.TOYS.SLOT, MISC.TAGS.TOY, showInUI: false));
			Suit = Add(new EquipmentSlot(TUNING.EQUIPMENT.SUITS.SLOT, MISC.TAGS.SUIT));
			Tool = Add(new EquipmentSlot(TUNING.EQUIPMENT.TOOLS.TOOLSLOT, MISC.TAGS.MULTITOOL, showInUI: false));
			Outfit = Add(new EquipmentSlot(TUNING.EQUIPMENT.CLOTHING.SLOT, MISC.TAGS.CLOTHES));
		}
	}
}
