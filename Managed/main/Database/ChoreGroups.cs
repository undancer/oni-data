using Klei.AI;
using STRINGS;

namespace Database
{
	public class ChoreGroups : ResourceSet<ChoreGroup>
	{
		public ChoreGroup Build;

		public ChoreGroup Basekeeping;

		public ChoreGroup Cook;

		public ChoreGroup Art;

		public ChoreGroup Dig;

		public ChoreGroup Research;

		public ChoreGroup Farming;

		public ChoreGroup Ranching;

		public ChoreGroup Hauling;

		public ChoreGroup Storage;

		public ChoreGroup MachineOperating;

		public ChoreGroup MedicalAid;

		public ChoreGroup Combat;

		public ChoreGroup LifeSupport;

		public ChoreGroup Toggle;

		public ChoreGroup Recreation;

		public ChoreGroup Rocketry;

		private ChoreGroup Add(string id, string name, Attribute attribute, string sprite, int default_personal_priority, bool user_prioritizable = true)
		{
			ChoreGroup choreGroup = new ChoreGroup(id, name, attribute, sprite, default_personal_priority, user_prioritizable);
			Add(choreGroup);
			return choreGroup;
		}

		public ChoreGroups(ResourceSet parent)
			: base("ChoreGroups", parent)
		{
			Combat = Add("Combat", DUPLICANTS.CHOREGROUPS.COMBAT.NAME, Db.Get().Attributes.Digging, "icon_errand_combat", 5);
			LifeSupport = Add("LifeSupport", DUPLICANTS.CHOREGROUPS.LIFESUPPORT.NAME, Db.Get().Attributes.LifeSupport, "icon_errand_life_support", 5);
			Toggle = Add("Toggle", DUPLICANTS.CHOREGROUPS.TOGGLE.NAME, Db.Get().Attributes.Toggle, "icon_errand_toggle", 5);
			MedicalAid = Add("MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME, Db.Get().Attributes.Caring, "icon_errand_care", 4);
			Rocketry = Add("Rocketry", DUPLICANTS.CHOREGROUPS.ROCKETRY.NAME, Db.Get().Attributes.SpaceNavigation, "icon_errand_tidy", 4);
			Basekeeping = Add("Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME, Db.Get().Attributes.Strength, "icon_errand_tidy", 4);
			Cook = Add("Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME, Db.Get().Attributes.Cooking, "icon_errand_cook", 3);
			Art = Add("Art", DUPLICANTS.CHOREGROUPS.ART.NAME, Db.Get().Attributes.Art, "icon_errand_art", 3);
			Research = Add("Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME, Db.Get().Attributes.Learning, "icon_errand_research", 3);
			MachineOperating = Add("MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME, Db.Get().Attributes.Machinery, "icon_errand_operate", 3);
			Farming = Add("Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME, Db.Get().Attributes.Botanist, "icon_errand_farm", 3);
			Ranching = Add("Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME, Db.Get().Attributes.Ranching, "icon_errand_ranch", 3);
			Build = Add("Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME, Db.Get().Attributes.Construction, "icon_errand_toggle", 2);
			Dig = Add("Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME, Db.Get().Attributes.Digging, "icon_errand_dig", 2);
			Hauling = Add("Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME, Db.Get().Attributes.Strength, "icon_errand_supply", 1);
			Storage = Add("Storage", DUPLICANTS.CHOREGROUPS.STORAGE.NAME, Db.Get().Attributes.Strength, "icon_errand_storage", 1);
			Recreation = Add("Recreation", DUPLICANTS.CHOREGROUPS.RECREATION.NAME, Db.Get().Attributes.Strength, "icon_errand_storage", 1, user_prioritizable: false);
			Debug.Assert(condition: true);
		}

		public ChoreGroup FindByHash(HashedString id)
		{
			ChoreGroup result = null;
			foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
			{
				if (resource.IdHash == id)
				{
					result = resource;
					break;
				}
			}
			return result;
		}
	}
}
