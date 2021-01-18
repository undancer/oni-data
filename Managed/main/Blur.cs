using UnityEngine;

public static class Blur
{
	private static Material blurMaterial;

	public static RenderTexture Run(Texture2D image)
	{
		if (blurMaterial == null)
		{
			blurMaterial = new Material(Shader.Find("Klei/PostFX/Blur"));
		}
		return null;
	}
}
