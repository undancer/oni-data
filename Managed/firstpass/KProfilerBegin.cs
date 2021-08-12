using UnityEngine;

public class KProfilerBegin : MonoBehaviour
{
	public static int begin_counter;

	private void Start()
	{
		Debug.Log("KProfiler: Start");
		KProfiler.BeginThread("Main", "Game");
	}

	private void Update()
	{
		KProfiler.BeginFrame();
	}
}
