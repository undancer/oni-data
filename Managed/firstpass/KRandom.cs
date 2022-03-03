using System;
using System.Runtime.InteropServices;

[Serializable]
[ComVisible(true)]
public class KRandom
{
	private const int MBIG = int.MaxValue;

	private const int MSEED = 161803398;

	private const int MZ = 0;

	private int inext;

	private int inextp;

	private int[] SeedArray = new int[56];

	public KRandom()
		: this(Environment.TickCount)
	{
	}

	public KRandom(int Seed)
	{
		int num = ((Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed));
		int num2 = 161803398 - num;
		SeedArray[55] = num2;
		int num3 = 1;
		for (int i = 1; i < 55; i++)
		{
			int num4 = 21 * i % 55;
			SeedArray[num4] = num3;
			num3 = num2 - num3;
			if (num3 < 0)
			{
				num3 += int.MaxValue;
			}
			num2 = SeedArray[num4];
		}
		for (int j = 1; j < 5; j++)
		{
			for (int k = 1; k < 56; k++)
			{
				SeedArray[k] -= SeedArray[1 + (k + 30) % 55];
				if (SeedArray[k] < 0)
				{
					SeedArray[k] += int.MaxValue;
				}
			}
		}
		inext = 0;
		inextp = 21;
		Seed = 1;
	}

	protected virtual double Sample()
	{
		return (double)InternalSample() * 4.656612875245797E-10;
	}

	private int InternalSample()
	{
		int num = inext;
		int num2 = inextp;
		if (++num >= 56)
		{
			num = 1;
		}
		if (++num2 >= 56)
		{
			num2 = 1;
		}
		int num3 = SeedArray[num] - SeedArray[num2];
		if (num3 == int.MaxValue)
		{
			num3--;
		}
		if (num3 < 0)
		{
			num3 += int.MaxValue;
		}
		SeedArray[num] = num3;
		inext = num;
		inextp = num2;
		return num3;
	}

	public virtual int Next()
	{
		return InternalSample();
	}

	private double GetSampleForLargeRange()
	{
		int num = InternalSample();
		if ((InternalSample() % 2 == 0) ? true : false)
		{
			num = -num;
		}
		return ((double)num + 2147483646.0) / 4294967293.0;
	}

	public virtual int Next(int minValue, int maxValue)
	{
		long num = (long)maxValue - (long)minValue;
		if (num <= int.MaxValue)
		{
			return (int)(Sample() * (double)num) + minValue;
		}
		return (int)((long)(GetSampleForLargeRange() * (double)num) + minValue);
	}

	public virtual int Next(int maxValue)
	{
		_ = 0;
		return (int)(Sample() * (double)maxValue);
	}

	public virtual double NextDouble()
	{
		return Sample();
	}

	public virtual void NextBytes(byte[] buffer)
	{
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		for (int i = 0; i < buffer.Length; i++)
		{
			buffer[i] = (byte)(InternalSample() % 256);
		}
	}
}
