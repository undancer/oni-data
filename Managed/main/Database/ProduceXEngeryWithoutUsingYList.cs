using System.Collections.Generic;

namespace Database
{
	public class ProduceXEngeryWithoutUsingYList : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public List<Tag> disallowedBuildings = new List<Tag>();

		public float amountToProduce;

		private float amountProduced;

		private bool usedDisallowedBuilding;

		public ProduceXEngeryWithoutUsingYList(float amountToProduce, List<Tag> disallowedBuildings)
		{
			this.disallowedBuildings = disallowedBuildings;
			this.amountToProduce = amountToProduce;
			usedDisallowedBuilding = false;
		}

		public override bool Success()
		{
			float num = 0f;
			foreach (KeyValuePair<Tag, float> item in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!disallowedBuildings.Contains(item.Key))
				{
					num += item.Value;
				}
			}
			return num / 1000f > amountToProduce;
		}

		public override bool Fail()
		{
			foreach (Tag disallowedBuilding in disallowedBuildings)
			{
				if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(disallowedBuilding))
				{
					return true;
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
			int num = reader.ReadInt32();
			disallowedBuildings = new List<Tag>(num);
			for (int i = 0; i < num; i++)
			{
				string name = reader.ReadKleiString();
				disallowedBuildings.Add(new Tag(name));
			}
			amountProduced = (float)reader.ReadDouble();
			amountToProduce = (float)reader.ReadDouble();
			usedDisallowedBuilding = reader.ReadByte() != 0;
		}

		public float GetProductionAmount(bool complete)
		{
			float num = 0f;
			foreach (KeyValuePair<Tag, float> item in Game.Instance.savedInfo.powerCreatedbyGeneratorType)
			{
				if (!disallowedBuildings.Contains(item.Key))
				{
					num += item.Value;
				}
			}
			if (!complete)
			{
				return num;
			}
			return amountToProduce;
		}
	}
}
