using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HealthyGameMessageScreen")]
public class HealthyGameMessageScreen : KMonoBehaviour
{
	public KButton confirmButton;

	public CanvasGroup canvasGroup;

	private float spawnTime;

	private float totalTime = 10f;

	private float fadeTime = 1.5f;

	private bool isFirstUpdate = true;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		confirmButton.onClick += delegate
		{
			UnityEngine.Object.Destroy(base.gameObject);
		};
		confirmButton.gameObject.SetActive(value: false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Update()
	{
		if (isFirstUpdate)
		{
			isFirstUpdate = false;
			spawnTime = Time.unscaledTime;
			return;
		}
		float num = Mathf.Min(Time.unscaledDeltaTime, 71f / (678f * (float)Math.PI));
		float num2 = Time.unscaledTime - spawnTime;
		if (num2 < totalTime - fadeTime)
		{
			canvasGroup.alpha += num * (1f / fadeTime);
		}
		else if (num2 >= totalTime + 0.75f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (num2 >= totalTime - fadeTime)
		{
			canvasGroup.alpha -= num * (1f / fadeTime);
		}
	}
}
