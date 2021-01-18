using System;
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
			return modifyType switch
			{
				ModifyType.Abs => new Abs(), 
				ModifyType.Clamp => new Clamp
				{
					LowerBound = lower,
					UpperBound = upper
				}, 
				ModifyType.Exponent => new Exponent
				{
					ExponentValue = exponent
				}, 
				ModifyType.Invert => new Invert(), 
				ModifyType.Curve => new Curve(), 
				ModifyType.Terrace => new Terrace(), 
				ModifyType.ScaleBias => new ScaleBias
				{
					Scale = scale,
					Bias = bias
				}, 
				ModifyType.Scale2d => new Scale2d
				{
					Scale = scale2d
				}, 
				_ => null, 
			};
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
				foreach (ControlPoint control in controlPoints.GetControls())
				{
					curve.AddControlPoint(control);
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
