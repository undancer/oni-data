using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SessionSearchSetParameterOptionsInternal : IDisposable
	{
		private int m_ApiVersion;

		private IntPtr m_Parameter;

		private ComparisonOp m_ComparisonOp;

		public int ApiVersion
		{
			get
			{
				int target = Helper.GetDefault<int>();
				Helper.TryMarshalGet(m_ApiVersion, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ApiVersion, value);
			}
		}

		public AttributeDataInternal? Parameter
		{
			get
			{
				AttributeDataInternal? target = Helper.GetDefault<AttributeDataInternal?>();
				Helper.TryMarshalGet(m_Parameter, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_Parameter, value);
			}
		}

		public ComparisonOp ComparisonOp
		{
			get
			{
				ComparisonOp target = Helper.GetDefault<ComparisonOp>();
				Helper.TryMarshalGet(m_ComparisonOp, out target);
				return target;
			}
			set
			{
				Helper.TryMarshalSet(ref m_ComparisonOp, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref m_Parameter);
		}
	}
}
