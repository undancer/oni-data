using UnityEngine;

public class Audio : ScriptableObject
{
	private static Audio _Instance;

	public float listenerMinZ;

	public float listenerMinOrthographicSize;

	public float listenerReferenceZ;

	public float listenerReferenceOrthographicSize;

	public static Audio Get()
	{
		if (_Instance == null)
		{
			_Instance = Resources.Load<Audio>("Audio");
		}
		return _Instance;
	}
}
