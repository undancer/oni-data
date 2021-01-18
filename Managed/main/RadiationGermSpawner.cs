using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RadiationGermSpawner")]
public class RadiationGermSpawner : KMonoBehaviour
{
	private const float GERM_SCALE = 100f;

	private const int CELLS_PER_UPDATE = 1024;

	private int nextEvaluatedCell;

	private float cellRatio;

	private byte disease_idx;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		cellRatio = Grid.CellCount / 1024;
		disease_idx = byte.MaxValue;
	}

	private void Update()
	{
	}

	private void EvaluateRadiation()
	{
		for (int i = 0; i < 1024; i++)
		{
			int num = (nextEvaluatedCell + i) % Grid.CellCount;
			if (Grid.RadiationCount[num] < 0)
			{
				continue;
			}
			int num2 = Mathf.RoundToInt((float)Grid.RadiationCount[num] * 100f * (Time.deltaTime * cellRatio));
			if (Grid.DiseaseIdx[num] == disease_idx)
			{
				SimMessages.ModifyDiseaseOnCell(num, disease_idx, num2);
				continue;
			}
			int num3 = Grid.DiseaseCount[num] - num2;
			if (num3 < 0)
			{
				SimMessages.ModifyDiseaseOnCell(num, disease_idx, num3);
			}
			else
			{
				SimMessages.ModifyDiseaseOnCell(num, Grid.DiseaseIdx[num], -num2);
			}
		}
		nextEvaluatedCell = (nextEvaluatedCell + 1024) % Grid.CellCount;
	}
}
