using System;
using LibNoiseDotNet.Graphics.Tools.Noise;
using LibNoiseDotNet.Graphics.Tools.Noise.Filter;

namespace ProcGen.Noise
{
	public class Filter : NoiseBase
	{
		public enum NoiseFilter
		{
			_UNSET_,
			Pipe,
			SumFractal,
			SinFractal,
			Billow,
			MultiFractal,
			HeterogeneousMultiFractal,
			HybridMultiFractal,
			RidgedMultiFractal,
			Voronoi
		}

		public NoiseFilter filter
		{
			get;
			set;
		}

		public float frequency
		{
			get;
			set;
		}

		public float lacunarity
		{
			get;
			set;
		}

		public int octaves
		{
			get;
			set;
		}

		public float offset
		{
			get;
			set;
		}

		public float gain
		{
			get;
			set;
		}

		public float exponent
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

		public override Type GetObjectType()
		{
			return typeof(Filter);
		}

		public Filter()
		{
			filter = NoiseFilter.RidgedMultiFractal;
			frequency = 0.1f;
			lacunarity = 3f;
			octaves = 0;
			offset = 1f;
			gain = 1f;
			exponent = 0.9f;
			scale = 1f;
			bias = 0f;
		}

		public Filter(Filter src)
		{
			filter = src.filter;
			frequency = src.frequency;
			lacunarity = src.lacunarity;
			octaves = src.octaves;
			offset = src.offset;
			gain = src.gain;
			exponent = src.exponent;
		}

		public IModule3D CreateModule()
		{
			FilterModule filterModule = null;
			switch (filter)
			{
			case NoiseFilter.Pipe:
				filterModule = new Pipe();
				break;
			case NoiseFilter.SumFractal:
				filterModule = new SumFractal();
				break;
			case NoiseFilter.SinFractal:
				filterModule = new SinFractal();
				break;
			case NoiseFilter.MultiFractal:
				filterModule = new MultiFractal();
				break;
			case NoiseFilter.Billow:
				filterModule = new Billow();
				break;
			case NoiseFilter.HeterogeneousMultiFractal:
				filterModule = new HeterogeneousMultiFractal();
				break;
			case NoiseFilter.HybridMultiFractal:
				filterModule = new HybridMultiFractal();
				break;
			case NoiseFilter.RidgedMultiFractal:
				filterModule = new RidgedMultiFractal();
				break;
			case NoiseFilter.Voronoi:
				filterModule = new Voronoi();
				break;
			}
			if (filterModule != null)
			{
				filterModule.Frequency = frequency;
				filterModule.Lacunarity = lacunarity;
				filterModule.OctaveCount = octaves;
				filterModule.Offset = offset;
				filterModule.Gain = gain;
				filterModule.SpectralExponent = exponent;
				if (filter == NoiseFilter.Billow)
				{
					Billow obj = (Billow)filterModule;
					obj.Scale = scale;
					obj.Bias = bias;
				}
			}
			return (IModule3D)filterModule;
		}

		public void SetSouces(IModule3D target, IModule3D sourceModule)
		{
			(target as FilterModule).Primitive3D = sourceModule;
		}
	}
}
