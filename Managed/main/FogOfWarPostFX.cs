using UnityEngine;

public class FogOfWarPostFX : MonoBehaviour
{
	[SerializeField]
	private Shader shader;

	private Material material;

	private Camera myCamera;

	private void Awake()
	{
		if (shader != null)
		{
			material = new Material(shader);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		SetupUVs();
		Graphics.Blit(source, destination, material, 0);
	}

	private void SetupUVs()
	{
		if (myCamera == null)
		{
			myCamera = GetComponent<Camera>();
			if (myCamera == null)
			{
				return;
			}
		}
		Ray ray = myCamera.ViewportPointToRay(Vector3.zero);
		float distance = Mathf.Abs(ray.origin.z / ray.direction.z);
		Vector3 point = ray.GetPoint(distance);
		Vector4 value = default(Vector4);
		value.x = point.x / Grid.WidthInMeters;
		value.y = point.y / Grid.HeightInMeters;
		ray = myCamera.ViewportPointToRay(Vector3.one);
		distance = Mathf.Abs(ray.origin.z / ray.direction.z);
		point = ray.GetPoint(distance);
		value.z = point.x / Grid.WidthInMeters - value.x;
		value.w = point.y / Grid.HeightInMeters - value.y;
		material.SetVector("_UVOffsetScale", value);
	}
}
