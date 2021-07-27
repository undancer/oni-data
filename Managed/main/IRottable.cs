using UnityEngine;

public interface IRottable
{
	GameObject gameObject { get; }

	float RotTemperature { get; }

	float PreserveTemperature { get; }
}
