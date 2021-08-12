using System.Collections.Generic;
using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public List<BackWallImage> images;

		private Texture2DArray textureArray;

		private void Awake()
		{
			DebugUtil.DevAssert(backwallMaterial != null, "Expected a backwall material!");
			DebugUtil.DevAssert(images.Count > 0, "Expected backwall images (at least one)!");
			BackWallImage backWallImage = images[0];
			int count = images.Count;
			int mipmapCount = backWallImage.image.mipmapCount;
			bool mipChain = mipmapCount > 0;
			textureArray = new Texture2DArray(backWallImage.image.width, backWallImage.image.height, count, TextureFormat.RGB24, mipChain);
			for (int i = 0; i < count; i++)
			{
				BackWallImage backWallImage2 = images[i];
				for (int j = 0; j < mipmapCount; j++)
				{
					textureArray.SetPixels(backWallImage2.image.GetPixels(j), i, j);
				}
			}
			textureArray.Apply();
			backwallMaterial.SetTexture("images", textureArray);
		}
	}
}
