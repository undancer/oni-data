using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RadiationGermSpawner")]
public class RadiationGermSpawner : KMonoBehaviour
{
	private const float GERM_SCALE = 10f;

	private const int CELLS_PER_UPDATE = 2048;

	private int nextEvaluatedCell;

	private float cellRatio;

	private byte radiation_disease_idx;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		cellRatio = Grid.CellCount / 2048;
		radiation_disease_idx = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.Id);
	}

	private void Update()
	{
		EvaluateRadiation();
	}

	private void EvaluateRadiation()
	{
		if (KMonoBehaviour.isLoadingScene)
		{
			return;
		}
		for (int i = 0; i < 2048; i++)
		{
			int num = (nextEvaluatedCell + i) % Grid.CellCount;
			if (!(Grid.Radiation[num] <= 0f))
			{
				int num2 = Mathf.RoundToInt(Grid.Radiation[num] * 10f * (Time.deltaTime * cellRatio));
				if (Grid.DiseaseIdx[num] != radiation_disease_idx)
				{
					SimMessages.ModifyDiseaseOnCell(num, Grid.DiseaseIdx[num], -num2);
				}
			}
		}
		nextEvaluatedCell = (nextEvaluatedCell + 2048) % Grid.CellCount;
	}
}
