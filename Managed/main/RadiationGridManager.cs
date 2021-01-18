using System.Collections.Generic;
using UnityEngine;

public static class RadiationGridManager
{
	public const float STANDARD_MASS_FALLOFF = 1000000f;

	public const int RADIATION_LINGER_RATE = 4;

	public static List<RadiationGridEmitter> emitters = new List<RadiationGridEmitter>();

	public static List<Tuple<int, int>> previewLightCells = new List<Tuple<int, int>>();

	public static int[] previewLux;

	public static int CalculateFalloff(float falloffRate, int cell, int origin)
	{
		return Mathf.Max(1, Mathf.RoundToInt(falloffRate * (float)Mathf.Max(Grid.GetCellDistance(origin, cell), 1)));
	}

	public static void Initialise()
	{
		emitters = new List<RadiationGridEmitter>();
	}

	public static void Shutdown()
	{
		emitters.Clear();
	}

	public static void Refresh()
	{
		for (int i = 0; i < emitters.Count; i++)
		{
			if (emitters[i].enabled)
			{
				emitters[i].Emit();
			}
		}
	}
}
