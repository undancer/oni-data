namespace Epic.OnlineServices.Metrics
{
	public class BeginPlayerSessionOptionsAccountId
	{
		private MetricsAccountIdType m_AccountIdType;

		private EpicAccountId m_Epic;

		private string m_External;

		public MetricsAccountIdType AccountIdType
		{
			get
			{
				return m_AccountIdType;
			}
			private set
			{
				m_AccountIdType = value;
			}
		}

		public EpicAccountId Epic
		{
			get
			{
				EpicAccountId target = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet(m_Epic, out target, m_AccountIdType, MetricsAccountIdType.Epic);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Epic, value, ref m_AccountIdType, MetricsAccountIdType.Epic, this);
			}
		}

		public string External
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_External, out target, m_AccountIdType, MetricsAccountIdType.External);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_External, value, ref m_AccountIdType, MetricsAccountIdType.External, this);
			}
		}

		public static implicit operator BeginPlayerSessionOptionsAccountId(EpicAccountId value)
		{
			return new BeginPlayerSessionOptionsAccountId
			{
				Epic = value
			};
		}

		public static implicit operator BeginPlayerSessionOptionsAccountId(string value)
		{
			return new BeginPlayerSessionOptionsAccountId
			{
				External = value
			};
		}
	}
}
