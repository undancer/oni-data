namespace Epic.OnlineServices.Lobby
{
	public class AttributeDataValue
	{
		private long m_AsInt64;

		private double m_AsDouble;

		private bool m_AsBool;

		private string m_AsUtf8;

		private AttributeType m_ValueType;

		public long? AsInt64
		{
			get
			{
				long? target = Helper.GetDefault<long?>();
				Helper.TryMarshalGet(m_AsInt64, out target, m_ValueType, AttributeType.Int64);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AsInt64, value, ref m_ValueType, AttributeType.Int64, this);
			}
		}

		public double? AsDouble
		{
			get
			{
				double? target = Helper.GetDefault<double?>();
				Helper.TryMarshalGet(m_AsDouble, out target, m_ValueType, AttributeType.Double);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AsDouble, value, ref m_ValueType, AttributeType.Double, this);
			}
		}

		public bool? AsBool
		{
			get
			{
				bool? target = Helper.GetDefault<bool?>();
				Helper.TryMarshalGet(m_AsBool, out target, m_ValueType, AttributeType.Boolean);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AsBool, value, ref m_ValueType, AttributeType.Boolean, this);
			}
		}

		public string AsUtf8
		{
			get
			{
				string target = Helper.GetDefault<string>();
				Helper.TryMarshalGet(m_AsUtf8, out target, m_ValueType, AttributeType.String);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_AsUtf8, value, ref m_ValueType, AttributeType.String, this);
			}
		}

		public AttributeType ValueType
		{
			get
			{
				return m_ValueType;
			}
			private set
			{
				m_ValueType = value;
			}
		}

		public static implicit operator AttributeDataValue(long value)
		{
			return new AttributeDataValue
			{
				AsInt64 = value
			};
		}

		public static implicit operator AttributeDataValue(double value)
		{
			return new AttributeDataValue
			{
				AsDouble = value
			};
		}

		public static implicit operator AttributeDataValue(bool value)
		{
			return new AttributeDataValue
			{
				AsBool = value
			};
		}

		public static implicit operator AttributeDataValue(string value)
		{
			return new AttributeDataValue
			{
				AsUtf8 = value
			};
		}
	}
}
