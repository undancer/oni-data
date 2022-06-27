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

		public TransformerType transformerType { get; set; }

		public float power { get; set; }

		public Vector2f vector { get; set; }

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
				return new Turbulence
				{
					Power = power
				};
			}
			if (transformerType == TransformerType.RotatePoint)
			{
				return new RotatePoint
				{
					XAngle = vector.x,
					YAngle = vector.y,
					ZAngle = 0f
				};
			}
			if (transformerType == TransformerType.TranslatePoint)
			{
				return new TranslatePoint
				{
					XTranslate = vector.x,
					ZTranslate = vector.y
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
				Turbulence obj = target as Turbulence;
				obj.SourceModule = sourceModule;
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
				obj.XDistortModule = xDistortModule;
				obj.YDistortModule = new Constant(0f);
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
				obj.ZDistortModule = zDistortModule;
			}
			else if (transformerType == TransformerType.RotatePoint)
			{
				(target as RotatePoint).SourceModule = sourceModule;
			}
			else if (transformerType == TransformerType.TranslatePoint)
			{
				(target as TranslatePoint).SourceModule = sourceModule;
			}
			else
			{
				Displace obj2 = target as Displace;
				obj2.SourceModule = sourceModule;
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
				obj2.XDisplaceModule = xDisplaceModule;
				obj2.YDisplaceModule = new Constant(0f);
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
				obj2.ZDisplaceModule = zDisplaceModule;
			}
		}
	}
}
