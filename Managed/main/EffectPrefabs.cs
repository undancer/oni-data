using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
	public GameObject ThoughtBubble;

	public GameObject ThoughtBubbleConvo;

	public GameObject MeteorBackground;

	public GameObject SparkleStreakFX;

	public static EffectPrefabs Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		Instance = this;
	}
}
