using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantRadiationMonitor : KMonoBehaviour
{
	public const float MAX_MUTATION_PROBABILITY = 0.33f;

	public const float MAX_RADIATION_FOR_MUTATION = 500f;

	public bool ShouldMutate()
	{
		int num = Grid.PosToCell(base.gameObject);
		float value = (Grid.IsValidCell(num) ? Grid.Radiation[num] : 0f);
		value = Mathf.Clamp(value, 0f, 500f);
		float num2 = value / 500f * 0.33f;
		float value2 = Random.value;
		Debug.Log($"Should Mutate? Rolled {value2} against {num2} in a cell with {value} rads: {value2 < num2}", base.gameObject);
		return value2 < num2;
	}
}
