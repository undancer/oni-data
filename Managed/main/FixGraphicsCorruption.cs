using UnityEngine;

public class FixGraphicsCorruption : MonoBehaviour
{
	private void Start()
	{
		Camera component = GetComponent<Camera>();
		component.transparencySortMode = TransparencySortMode.Orthographic;
		component.tag = "Untagged";
	}

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Graphics.Blit(source, dest);
	}
}
