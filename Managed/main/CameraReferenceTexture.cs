using UnityEngine;

public class CameraReferenceTexture : MonoBehaviour
{
	public Camera referenceCamera;

	private FullScreenQuad quad;

	private void OnPreCull()
	{
		if (quad == null)
		{
			quad = new FullScreenQuad("CameraReferenceTexture", GetComponent<Camera>(), referenceCamera.GetComponent<CameraRenderTexture>().ShouldFlip());
		}
		if (referenceCamera != null)
		{
			quad.Draw(referenceCamera.GetComponent<CameraRenderTexture>().GetTexture());
		}
	}
}
