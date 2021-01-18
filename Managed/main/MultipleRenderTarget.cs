using System;
using System.Collections;
using UnityEngine;

public class MultipleRenderTarget : MonoBehaviour
{
	private MultipleRenderTargetProxy renderProxy;

	private FullScreenQuad quad;

	public bool isFrontEnd;

	public event Action<Camera> onSetupComplete;

	private void Start()
	{
		StartCoroutine(SetupProxy());
	}

	private IEnumerator SetupProxy()
	{
		yield return null;
		Camera component = GetComponent<Camera>();
		Camera camera = new GameObject().AddComponent<Camera>();
		camera.CopyFrom(component);
		renderProxy = camera.gameObject.AddComponent<MultipleRenderTargetProxy>();
		camera.name = component.name + " MRT";
		camera.transform.parent = component.transform;
		camera.transform.SetLocalPosition(Vector3.zero);
		camera.depth = component.depth - 1f;
		component.cullingMask = 0;
		component.clearFlags = CameraClearFlags.Color;
		quad = new FullScreenQuad("MultipleRenderTarget", component, invert: true);
		if (this.onSetupComplete != null)
		{
			this.onSetupComplete(camera);
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
