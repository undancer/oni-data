using System;
using System.Collections.Generic;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Modifier;

namespace ProcGen.Noise
{
	public class Modifier : NoiseBase
	{
		public enum ModifyType
		{
			_UNSET_,
			Abs,
			Clamp,
			Exponent,
			Invert,
			ScaleBias,
			Scale2d,
			Curve,
			Terrace
		}

		public ModifyType modifyType
		{
			get;
			set;
		}

		public float lower
		{
			get;
			set;
		}

		public float upper
		{
			get;
			set;
		}

		public float exponent
		{
			get;
			set;
		}

		public bool invert
		{
			get;
			set;
		}

		public float scale
		{
			get;
			set;
		}

		public float bias
		{
			get;
			set;
		}

		public Vector2f scale2d
		{
			get;
			set;
		}

		public override Type GetObjectType()
		{
			return typeof(Modifier);
		}

		public Modifier()
		{
			modifyType = ModifyType.Abs;
			lower = -1f;
			upper = 1f;
			exponent = 0.02f;
			invert = false;
			scale = 1f;
			bias = 0f;
			scale2d = new Vector2f(1, 1);
		}

		public IModule3D CreateModule()
		{
			switch (modifyType)
			{
			case ModifyType.Abs:
				return new Abs();
			case ModifyType.Clamp:
			{
				Clamp clamp = new Clamp();
				clamp.LowerBound = lower;
				clamp.UpperBound = upper;
				return clamp;
			}
			case ModifyType.Exponent:
			{
				Exponent exponent = new Exponent();
				exponent.ExponentValue = this.exponent;
				return exponent;
			}
			case ModifyType.Invert:
				return new Invert();
			case ModifyType.Curve:
				return new Curve();
			case ModifyType.Terrace:
				return new Terrace();
			case ModifyType.ScaleBias:
			{
				ScaleBias scaleBias = new ScaleBias();
				scaleBias.Scale = scale;
				scaleBias.Bias = bias;
				return scaleBias;
			}
			case ModifyType.Scale2d:
			{
				Scale2d scale2d = new Scale2d();
				scale2d.Scale = this.scale2d;
				return scale2d;
			}
			default:
				return null;
			}
		}

		public IModule3D CreateModule(IModule3D sourceModule)
		{
			return modifyType switch
			{
				ModifyType.Abs => new Abs(sourceModule), 
				ModifyType.Clamp => new Clamp(sourceModule, lower, upper), 
				ModifyType.Exponent => new Exponent(sourceModule, exponent), 
				ModifyType.Invert => new Invert(sourceModule), 
				ModifyType.Curve => new Curve(sourceModule), 
				ModifyType.Terrace => new Terrace(sourceModule), 
				ModifyType.ScaleBias => new ScaleBias(sourceModule, scale, bias), 
				ModifyType.Scale2d => new Scale2d(sourceModule, scale2d), 
				_ => null, 
			};
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule, FloatList controlFloats, ControlPointList controlPoints)
		{
			(target as ModifierModule).SourceModule = sourceModule;
			if (modifyType == ModifyType.Curve)
			{
				Curve curve = target as Curve;
				curve.ClearControlPoints();
				List<ControlPoint> controls = controlPoints.GetControls();
				foreach (ControlPoint item in controls)
				{
					curve.AddControlPoint(item);
				}
			}
			else
			{
				if (modifyType != ModifyType.Terrace)
				{
					return;
				}
				Terrace terrace = target as Terrace;
				terrace.ClearControlPoints();
				foreach (float point in controlFloats.points)
				{
					terrace.AddControlPoint(point);
				}
			}
		}
	}
}
