using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VirtualCursorOverlayFix : MonoBehaviour
{
	private RenderTexture cursorRendTex;

	public Camera screenSpaceCamera;

	public Image screenSpaceOverlayImage;

	public RawImage actualCursor;

	private void Awake()
	{
		int width = Screen.currentResolution.width;
		int height = Screen.currentResolution.height;
		cursorRendTex = new RenderTexture(width, height, 0);
		screenSpaceCamera.enabled = true;
		screenSpaceCamera.targetTexture = cursorRendTex;
		screenSpaceOverlayImage.material.SetTexture("_MainTex", cursorRendTex);
		StartCoroutine(RenderVirtualCursor());
	}

	private IEnumerator RenderVirtualCursor()
	{
		_ = KInputManager.currentControllerIsGamepad;
		while (Application.isPlaying)
		{
			bool ShowCursor = KInputManager.currentControllerIsGamepad;
			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.C))
			{
				ShowCursor = true;
			}
			screenSpaceCamera.enabled = true;
			if (!screenSpaceOverlayImage.enabled && ShowCursor)
			{
				yield return new WaitForSecondsRealtime(0.1f);
			}
			actualCursor.enabled = ShowCursor;
			screenSpaceOverlayImage.enabled = ShowCursor;
			screenSpaceOverlayImage.material.SetTexture("_MainTex", cursorRendTex);
			yield return null;
		}
	}
}
