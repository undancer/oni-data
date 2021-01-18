using UnityEngine;

[AddComponentMenu("KMonoBehaviour/prefabs/UIRotator")]
public class UIRotator : KMonoBehaviour
{
	public float minRotationSpeed = 1f;

	public float maxRotationSpeed = 1f;

	public float rotationSpeed = 1f;

	protected override void OnPrefabInit()
	{
		rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
	}

	private void Update()
	{
		GetComponent<RectTransform>().Rotate(0f, 0f, rotationSpeed * Time.unscaledDeltaTime);
	}
}
