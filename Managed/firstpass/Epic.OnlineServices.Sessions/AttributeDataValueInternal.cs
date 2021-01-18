using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	internal struct AttributeDataValueInternal : IDisposable
	{
		[FieldOffset(0)]
		private long m_AsInt64;

		[FieldOffset(0)]
		private double m_AsDouble;

		[FieldOffset(0)]
		private int m_AsBool;

		[FieldOffset(0)]
		private IntPtr m_AsUtf8;

		[FieldOffset(8)]
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
				AttributeType target = Helper.GetDefault<AttributeType>();
				Helper.TryMarshalGet(m_ValueType, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ValueType, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_AsUtf8, m_ValueType, AttributeType.String);
		}
	}
}
