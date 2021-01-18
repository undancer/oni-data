using Klei.AI;

namespace Database
{
	public class Diseases : ResourceSet<Disease>
	{
		public Disease FoodGerms;

		public Disease SlimeGerms;

		public Disease PollenGerms;

		public Disease ZombieSpores;

		public Disease RadiationPoisoning;

		public Diseases(ResourceSet parent, bool statsOnly = false)
			: base("Diseases", parent)
		{
			FoodGerms = Add(new FoodGerms(statsOnly));
			SlimeGerms = Add(new SlimeGerms(statsOnly));
			PollenGerms = Add(new PollenGerms(statsOnly));
			ZombieSpores = Add(new ZombieSpores(statsOnly));
			RadiationPoisoning = Add(new RadiationPoisoning(statsOnly));
		}

		public bool IsValidID(string id)
		{
			bool result = false;
			foreach (Disease resource in resources)
			{
				if (resource.Id == id)
				{
					result = true;
				}
			}
			return result;
		}

		public byte GetIndex(int hash)
		{
			for (byte b = 0; b < resources.Count; b = (byte)(b + 1))
			{
				Disease disease = resources[b];
				if (hash == disease.id.GetHashCode())
				{
					return b;
				}
			}
			return byte.MaxValue;
		}

		public byte GetIndex(HashedString id)
		{
			return GetIndex(id.GetHashCode());
		}
	}
}
