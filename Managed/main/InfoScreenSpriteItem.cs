using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/InfoScreenSpriteItem")]
public class InfoScreenSpriteItem : KMonoBehaviour
{
	[SerializeField]
	private Image image;

	[SerializeField]
	private LayoutElement layout;

	public void SetSprite(Sprite sprite)
	{
		image.sprite = sprite;
		float num = sprite.rect.width / sprite.rect.height;
		layout.preferredWidth = layout.preferredHeight * num;
	}
}
