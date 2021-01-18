using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Tranformer;

namespace ProcGen.Noise
{
	public class Transformer : NoiseBase
	{
		public enum TransformerType
		{
			_UNSET_,
			Displace,
			Turbulence,
			RotatePoint
		}

		public TransformerType transformerType
		{
			get;
			set;
		}

		public float power
		{
			get;
			set;
		}

		public Vector2f rotation
		{
			get;
			set;
		}

		public override Type GetObjectType()
		{
			return typeof(Transformer);
		}

		public Transformer()
		{
			transformerType = TransformerType.Displace;
			power = 1f;
			rotation = new Vector2f(0, 0);
		}

		public IModule3D CreateModule()
		{
			if (transformerType == TransformerType.Turbulence)
			{
				new Turbulence().Power = power;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				return new RotatePoint
				{
					XAngle = rotation.x,
					YAngle = rotation.y,
					ZAngle = 0f
				};
			}
			return new Displace();
		}

		public IModule3D CreateModule(IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (transformerType == TransformerType.Turbulence)
			{
				return new Turbulence(sourceModule, xModule, yModule, zModule, power);
			}
			if (transformerType == TransformerType.RotatePoint)
			{
				return new RotatePoint(sourceModule, rotation.x, rotation.y, 0f);
			}
			return new Displace(sourceModule, xModule, yModule, zModule);
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (transformerType == TransformerType.Turbulence)
			{
				Turbulence obj = target as Turbulence;
				obj.SourceModule = sourceModule;
				obj.XDistortModule = xModule;
				obj.YDistortModule = yModule;
				obj.ZDistortModule = zModule;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				(target as RotatePoint).SourceModule = sourceModule;
			}
			else
			{
				Displace obj2 = target as Displace;
				obj2.SourceModule = sourceModule;
				obj2.XDisplaceModule = xModule;
				obj2.YDisplaceModule = yModule;
				obj2.ZDisplaceModule = zModule;
			}
		}
	}
}
