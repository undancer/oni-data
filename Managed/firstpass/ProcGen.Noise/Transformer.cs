using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Primitive;
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
			RotatePoint,
			TranslatePoint
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

		public Vector2f vector
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
			vector = new Vector2f(0, 0);
		}

		public IModule3D CreateModule()
		{
			if (transformerType == TransformerType.Turbulence)
			{
				Turbulence turbulence = new Turbulence();
				turbulence.Power = power;
				return turbulence;
			}
			if (transformerType == TransformerType.RotatePoint)
			{
				RotatePoint rotatePoint = new RotatePoint();
				rotatePoint.XAngle = vector.x;
				rotatePoint.YAngle = vector.y;
				rotatePoint.ZAngle = 0f;
				return rotatePoint;
			}
			if (transformerType == TransformerType.TranslatePoint)
			{
				TranslatePoint translatePoint = new TranslatePoint();
				translatePoint.XTranslate = vector.x;
				translatePoint.ZTranslate = vector.y;
				return translatePoint;
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
				return new RotatePoint(sourceModule, vector.x, vector.y, 0f);
			}
			if (transformerType == TransformerType.TranslatePoint)
			{
				return new TranslatePoint(sourceModule, vector.x, 0f, vector.y);
			}
			return new Displace(sourceModule, xModule, yModule, zModule);
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, IModule3D xModule, IModule3D yModule, IModule3D zModule)
		{
			if (transformerType == TransformerType.Turbulence)
			{
				Turbulence turbulence = target as Turbulence;
				turbulence.SourceModule = sourceModule;
				IModule3D xDistortModule;
				if (xModule == null)
				{
					IModule3D module3D = new Constant(0f);
					xDistortModule = module3D;
				}
				else
				{
					xDistortModule = xModule;
				}
				turbulence.XDistortModule = xDistortModule;
				turbulence.YDistortModule = new Constant(0f);
				IModule3D zDistortModule;
				if (yModule == null)
				{
					IModule3D module3D = new Constant(0f);
					zDistortModule = module3D;
				}
				else
				{
					zDistortModule = yModule;
				}
				turbulence.ZDistortModule = zDistortModule;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				RotatePoint rotatePoint = target as RotatePoint;
				rotatePoint.SourceModule = sourceModule;
			}
			else if (transformerType == TransformerType.TranslatePoint)
			{
				TranslatePoint translatePoint = target as TranslatePoint;
				translatePoint.SourceModule = sourceModule;
			}
			else
			{
				Displace displace = target as Displace;
				displace.SourceModule = sourceModule;
				IModule3D xDisplaceModule;
				if (xModule == null)
				{
					IModule3D module3D = new Constant(0f);
					xDisplaceModule = module3D;
				}
				else
				{
					xDisplaceModule = xModule;
				}
				displace.XDisplaceModule = xDisplaceModule;
				displace.YDisplaceModule = new Constant(0f);
				IModule3D zDisplaceModule;
				if (yModule == null)
				{
					IModule3D module3D = new Constant(0f);
					zDisplaceModule = module3D;
				}
				else
				{
					zDisplaceModule = yModule;
				}
				displace.ZDisplaceModule = zDisplaceModule;
			}
		}
	}
}
