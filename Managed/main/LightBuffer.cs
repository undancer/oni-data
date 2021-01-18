using System;
using UnityEngine;

public class LightBuffer : MonoBehaviour
{
	private Mesh Mesh;

	private Camera Camera;

	[NonSerialized]
	public Material Material;

	[NonSerialized]
	public Material CircleMaterial;

	[NonSerialized]
	public Material ConeMaterial;

	private int ColorRangeTag;

	private int LightPosTag;

	private int LightDirectionAngleTag;

	private int TintColorTag;

	private int Layer;

	public RenderTexture Texture;

	public Texture WorldLight;

	public static LightBuffer Instance;

	private const RenderTextureFormat RTFormat = RenderTextureFormat.ARGBHalf;

	private void Awake()
	{
		Instance = this;
		ColorRangeTag = Shader.PropertyToID("_ColorRange");
		LightPosTag = Shader.PropertyToID("_LightPos");
		LightDirectionAngleTag = Shader.PropertyToID("_LightDirectionAngle");
		TintColorTag = Shader.PropertyToID("_TintColor");
		Camera = GetComponent<Camera>();
		Layer = LayerMask.NameToLayer("Lights");
		Mesh = new Mesh();
		Mesh.name = "Light Mesh";
		Mesh.vertices = new Vector3[4]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, -1f, 0f),
			new Vector3(1f, 1f, 0f)
		};
		Mesh.uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f)
		};
		Mesh.triangles = new int[6]
		{
			0,
			1,
			2,
			2,
			1,
			3
		};
		Mesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		Texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
		Texture.name = "LightBuffer";
		Camera.targetTexture = Texture;
	}

	private void LateUpdate()
	{
		if (PropertyTextures.instance == null)
		{
			return;
		}
		if (Texture.width != Screen.width || Texture.height != Screen.height)
		{
			Texture.DestroyRenderTexture();
			Texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
			Texture.name = "LightBuffer";
			Camera.targetTexture = Texture;
		}
		Matrix4x4 matrix = default(Matrix4x4);
		WorldLight = PropertyTextures.instance.GetTexture(PropertyTextures.Property.WorldLight);
		Material.SetTexture("_PropertyWorldLight", WorldLight);
		CircleMaterial.SetTexture("_PropertyWorldLight", WorldLight);
		ConeMaterial.SetTexture("_PropertyWorldLight", WorldLight);
		foreach (Light2D worldItem in Components.Light2Ds.GetWorldItems(ClusterManager.Instance.activeWorldId))
		{
			if (worldItem == null || !worldItem.enabled)
			{
				continue;
			}
			MaterialPropertyBlock materialPropertyBlock = worldItem.materialPropertyBlock;
			materialPropertyBlock.SetVector(ColorRangeTag, new Vector4(worldItem.Color.r * worldItem.IntensityAnimation, worldItem.Color.g * worldItem.IntensityAnimation, worldItem.Color.b * worldItem.IntensityAnimation, worldItem.Range));
			Vector3 position = worldItem.transform.GetPosition();
			position.x += worldItem.Offset.x;
			position.y += worldItem.Offset.y;
			materialPropertyBlock.SetVector(LightPosTag, new Vector4(position.x, position.y, 0f, 0f));
			Vector2 normalized = worldItem.Direction.normalized;
			materialPropertyBlock.SetVector(LightDirectionAngleTag, new Vector4(normalized.x, normalized.y, 0f, worldItem.Angle));
			Graphics.DrawMesh(Mesh, Vector3.zero, Quaternion.identity, Material, Layer, Camera, 0, materialPropertyBlock, castShadows: false, receiveShadows: false);
			if (worldItem.drawOverlay)
			{
				materialPropertyBlock.SetColor(TintColorTag, worldItem.overlayColour);
				switch (worldItem.shape)
				{
				case LightShape.Circle:
					matrix.SetTRS(position, Quaternion.identity, Vector3.one * worldItem.Range);
					Graphics.DrawMesh(Mesh, matrix, CircleMaterial, Layer, Camera, 0, materialPropertyBlock);
					break;
				case LightShape.Cone:
					matrix.SetTRS(position - Vector3.up * (worldItem.Range * 0.5f), Quaternion.identity, new Vector3(1f, 0.5f, 1f) * worldItem.Range);
					Graphics.DrawMesh(Mesh, matrix, ConeMaterial, Layer, Camera, 0, materialPropertyBlock);
					break;
				}
			}
		}
	}

	private void OnDestroy()
	{
		Instance = null;
	}
}
