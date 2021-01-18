using System;
using System.Collections;
using UnityEngine;

public class MultipleRenderTarget : MonoBehaviour
{
	private MultipleRenderTargetProxy renderProxy;

	private FullScreenQuad quad;

	public bool isFrontEnd = false;

	public event Action<Camera> onSetupComplete;

	private void Start()
	{
		StartCoroutine(SetupProxy());
	}

	private IEnumerator SetupProxy()
	{
		yield return null;
		Camera camera = GetComponent<Camera>();
		GameObject new_camera_go = new GameObject();
		Camera new_camera = new_camera_go.AddComponent<Camera>();
		new_camera.CopyFrom(camera);
		renderProxy = new_camera.gameObject.AddComponent<MultipleRenderTargetProxy>();
		new_camera.name = camera.name + " MRT";
		new_camera.transform.parent = camera.transform;
		new_camera.transform.SetLocalPosition(Vector3.zero);
		new_camera.depth = camera.depth - 1f;
		camera.cullingMask = 0;
		camera.clearFlags = CameraClearFlags.Color;
		quad = new FullScreenQuad("MultipleRenderTarget", camera, invert: true);
		if (this.onSetupComplete != null)
		{
			this.onSetupComplete(new_camera);
		}
	}

	private void OnPreCull()
	{
		if (renderProxy != null)
		{
			quad.Draw(renderProxy.Textures[0]);
		}
	}

	public void ToggleColouredOverlayView(bool enabled)
	{
		if (renderProxy != null)
		{
			renderProxy.ToggleColouredOverlayView(enabled);
		}
	}
}
