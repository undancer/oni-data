using System.Collections.Generic;
using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public List<Texture2D> images;

		private Texture2DArray textureArray;

		private void Awake()
		{
			DebugUtil.DevAssert(backwallMaterial != null, "Expected a backwall material!");
			DebugUtil.DevAssert(images.Count > 0, "Expected backwall images (at least one)!");
			Texture2D texture2D = images[0];
			int count = images.Count;
			int mipmapCount = texture2D.mipmapCount;
			bool mipChain = mipmapCount > 0;
			textureArray = new Texture2DArray(texture2D.width, texture2D.height, count, TextureFormat.RGB24, mipChain);
			for (int i = 0; i < count; i++)
			{
				Texture2D texture2D2 = images[i];
				Debug.Log($"copying image {texture2D2.name} type {texture2D2.format} size {texture2D2.width}x{texture2D2.height}");
				for (int j = 0; j < mipmapCount; j++)
				{
					textureArray.SetPixels(texture2D2.GetPixels(j), i, j);
				}
			}
			textureArray.Apply();
			backwallMaterial.SetTexture("images", textureArray);
		}
	}
}
