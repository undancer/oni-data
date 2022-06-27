using UnityEngine;

namespace rendering
{
	public class BackWall : MonoBehaviour
	{
		[SerializeField]
		public Material backwallMaterial;

		[SerializeField]
		public Texture2DArray array;

		private void Awake()
		{
			backwallMaterial.SetTexture("images", array);
		}
	}
}
