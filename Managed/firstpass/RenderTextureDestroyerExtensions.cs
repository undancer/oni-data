using UnityEngine;

public static class RenderTextureDestroyerExtensions
{
	public static void DestroyRenderTexture(this RenderTexture render_texture)
	{
		if (RenderTextureDestroyer.Instance != null)
		{
			RenderTextureDestroyer.Instance.Add(render_texture);
		}
		else
		{
			Object.Destroy(render_texture);
		}
	}
}
