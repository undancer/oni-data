namespace rail
{
	public class RailID : RailComparableID
	{
		public RailID()
		{
		}

		public RailID(ulong id)
			: base(id)
		{
		}

		public EnumRailIDDomain GetDomain()
		{
			if ((int)(id_ >> 56) == 1)
			{
				return EnumRailIDDomain.kRailIDDomainPublic;
			}
			return EnumRailIDDomain.kRailIDDomainInvalid;
		}
	}
}
