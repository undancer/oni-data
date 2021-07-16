using UnityEngine;

public class SceneInitializerLoader : MonoBehaviour
{
	public struct DeferredError
	{
		public string msg;

		public string stack_trace;

		public bool IsValid => !string.IsNullOrEmpty(msg);
	}

	public delegate void DeferredErrorDelegate(DeferredError deferred_error);

	public SceneInitializer sceneInitializer;

	public static DeferredError deferred_error;

	public static DeferredErrorDelegate ReportDeferredError;

	private void Awake()
	{
		Camera[] array = Object.FindObjectsOfType<Camera>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		KMonoBehaviour.isLoadingScene = false;
		Singleton<StateMachineManager>.Instance.Clear();
		Util.KInstantiate(sceneInitializer);
		if (ReportDeferredError != null && deferred_error.IsValid)
		{
			ReportDeferredError(deferred_error);
			deferred_error = default(DeferredError);
		}
	}
}
