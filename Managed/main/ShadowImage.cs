using UnityEngine;
using UnityEngine.UI;

public class ShadowImage : ShadowRect
{
	private Image shadowImage;

	private Image mainImage;

	protected override void MatchRect()
	{
		base.MatchRect();
		if (RectMain == null || RectShadow == null)
		{
			return;
		}
		if (shadowImage == null)
		{
			shadowImage = RectShadow.GetComponent<Image>();
		}
		if (mainImage == null)
		{
			mainImage = RectMain.GetComponent<Image>();
		}
		if (mainImage == null)
		{
			if (shadowImage != null)
			{
				shadowImage.color = Color.clear;
			}
		}
		else
		{
			if (shadowImage == null)
			{
				return;
			}
			if (shadowImage.sprite != mainImage.sprite)
			{
				shadowImage.sprite = mainImage.sprite;
			}
			if (shadowImage.color != shadowColor)
			{
				if (shadowImage.sprite != null)
				{
					shadowImage.color = shadowColor;
				}
				else
				{
					shadowImage.color = Color.clear;
				}
			}
		}
	}
}
