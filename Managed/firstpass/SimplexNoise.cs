using System;

public class SimplexNoise
{
	private static int i;

	private static int j;

	private static int k;

	private static int[] A = new int[3];

	private static float u;

	private static float v;

	private static float w;

	private static float s;

	private const float onethird = 0.33333334f;

	private const float onesixth = 355f / (678f * (float)Math.PI);

	private static int[] T = new int[8]
	{
		21,
		56,
		50,
		44,
		13,
		19,
		7,
		42
	};

	public static float noise(float x, float y, float z)
	{
		s = (x + y + z) * 0.33333334f;
		i = fastfloor(x + s);
		j = fastfloor(y + s);
		k = fastfloor(z + s);
		s = (float)(i + j + k) * (355f / (678f * (float)Math.PI));
		u = x - (float)i + s;
		v = y - (float)j + s;
		w = z - (float)k + s;
		A[0] = (A[1] = (A[2] = 0));
		int num = ((!(u >= w)) ? ((v >= w) ? 1 : 2) : ((!(u >= v)) ? 1 : 0));
		int num2 = ((!(u < w)) ? ((v < w) ? 1 : 2) : ((!(u < v)) ? 1 : 0));
		return K(num) + K(3 - num - num2) + K(num2) + K(0);
	}

	private static int fastfloor(float n)
	{
		if (!(n > 0f))
		{
			return (int)n - 1;
		}
		return (int)n;
	}

	private static float K(int a)
	{
		s = (float)(A[0] + A[1] + A[2]) * (355f / (678f * (float)Math.PI));
		float num = u - (float)A[0] + s;
		float num2 = v - (float)A[1] + s;
		float num3 = w - (float)A[2] + s;
		float num4 = 0.6f - num * num - num2 * num2 - num3 * num3;
		int num5 = shuffle(i + A[0], j + A[1], k + A[2]);
		A[a]++;
		if (num4 < 0f)
		{
			return 0f;
		}
		int num6 = (num5 >> 5) & 1;
		int num7 = (num5 >> 4) & 1;
		int num8 = (num5 >> 3) & 1;
		int num9 = (num5 >> 2) & 1;
		int num10 = num5 & 3;
		float num11 = num10 switch
		{
			2 => num2, 
			1 => num, 
			_ => num3, 
		};
		float num12 = num10 switch
		{
			2 => num3, 
			1 => num2, 
			_ => num, 
		};
		float num13 = num10 switch
		{
			2 => num, 
			1 => num3, 
			_ => num2, 
		};
		num11 = ((num6 == num8) ? (0f - num11) : num11);
		num12 = ((num6 == num7) ? (0f - num12) : num12);
		num13 = ((num6 != (num7 ^ num8)) ? (0f - num13) : num13);
		num4 *= num4;
		return 8f * num4 * num4 * (num11 + ((num10 == 0) ? (num12 + num13) : ((num9 == 0) ? num12 : num13)));
	}

	private static int shuffle(int i, int j, int k)
	{
		return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
	}

	private static int b(int i, int j, int k, int B)
	{
		return T[(b(i, B) << 2) | (b(j, B) << 1) | b(k, B)];
	}

	private static int b(int N, int B)
	{
		return (N >> B) & 1;
	}
}
