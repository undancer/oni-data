using UnityEngine;

public class KProfilerEnd : MonoBehaviour
{
	private void Start()
	{
	}

	private void LateUpdate()
	{
		KProfiler.EndFrame();
	}
}
