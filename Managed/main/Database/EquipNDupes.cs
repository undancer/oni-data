using STRINGS;

namespace Database
{
	public class EquipNDupes : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		private AssignableSlot equipmentSlot;

		private int numToEquip;

		public EquipNDupes(AssignableSlot equipmentSlot, int numToEquip)
		{
			this.equipmentSlot = equipmentSlot;
			this.numToEquip = numToEquip;
		}

		public override bool Success()
		{
			int num = 0;
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Equipment equipment = item.GetEquipment();
				if (equipment != null && equipment.IsSlotOccupied(equipmentSlot))
				{
					num++;
				}
			}
			return num >= numToEquip;
		}

		public void Deserialize(IReader reader)
		{
			string id = reader.ReadKleiString();
			equipmentSlot = Db.Get().AssignableSlots.Get(id);
			numToEquip = reader.ReadInt32();
		}

		public override string GetProgress(bool complete)
		{
			int num = 0;
			foreach (MinionIdentity item in Components.MinionIdentities.Items)
			{
				Equipment equipment = item.GetEquipment();
				if (equipment != null && equipment.IsSlotOccupied(equipmentSlot))
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CLOTHE_DUPES, complete ? numToEquip : num, numToEquip);
		}
	}
}
