using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Combiner;

namespace ProcGen.Noise
{
	public class Combiner : NoiseBase
	{
		public enum CombinerType
		{
			_UNSET_,
			Add,
			Max,
			Min,
			Multiply,
			Power
		}

		public CombinerType combineType { get; set; }

		public override Type GetObjectType()
		{
			return typeof(Combiner);
		}

		public Combiner()
		{
			combineType = CombinerType.Add;
		}

		public IModule3D CreateModule()
		{
			return combineType switch
			{
				CombinerType.Add => new Add(), 
				CombinerType.Max => new Max(), 
				CombinerType.Min => new Min(), 
				CombinerType.Multiply => new Multiply(), 
				CombinerType.Power => new Power(), 
				_ => null, 
			};
		}

		public IModule3D CreateModule(IModule3D leftModule, IModule3D rightModule)
		{
			return combineType switch
			{
				CombinerType.Add => new Add(leftModule, rightModule), 
				CombinerType.Max => new Max(leftModule, rightModule), 
				CombinerType.Min => new Min(leftModule, rightModule), 
				CombinerType.Multiply => new Multiply(leftModule, rightModule), 
				CombinerType.Power => new Power(leftModule, rightModule), 
				_ => null, 
			};
		}

		public void SetSouces(IModule3D target, IModule3D leftModule, IModule3D rightModule)
		{
			(target as CombinerModule).LeftModule = leftModule;
			(target as CombinerModule).RightModule = rightModule;
		}
	}
}
