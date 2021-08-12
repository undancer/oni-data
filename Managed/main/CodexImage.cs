using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexImage : CodexWidget<CodexImage>
{
	public Sprite sprite { get; set; }

	public Color color { get; set; }

	public string spriteName
	{
		get
		{
			return "--> " + ((sprite == null) ? "NULL" : sprite.ToString());
		}
		set
		{
			sprite = Assets.GetSprite(value);
		}
	}

	public string batchedAnimPrefabSourceID
	{
		get
		{
			return "--> " + ((sprite == null) ? "NULL" : sprite.ToString());
		}
		set
		{
			GameObject prefab = Assets.GetPrefab(value);
			KBatchedAnimController kBatchedAnimController = ((prefab != null) ? prefab.GetComponent<KBatchedAnimController>() : null);
			KAnimFile kAnimFile = ((kBatchedAnimController != null) ? kBatchedAnimController.AnimFiles[0] : null);
			sprite = ((kAnimFile != null) ? Def.GetUISpriteFromMultiObjectAnim(kAnimFile) : null);
		}
	}

	public CodexImage()
	{
		color = Color.white;
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite, Color color)
		: base(preferredWidth, preferredHeight)
	{
		this.sprite = sprite;
		this.color = color;
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite)
		: this(preferredWidth, preferredHeight, sprite, Color.white)
	{
	}

	public CodexImage(int preferredWidth, int preferredHeight, Tuple<Sprite, Color> coloredSprite)
		: this(preferredWidth, preferredHeight, coloredSprite.first, coloredSprite.second)
	{
	}

	public CodexImage(Tuple<Sprite, Color> coloredSprite)
		: this(-1, -1, coloredSprite)
	{
	}

	public void ConfigureImage(Image image)
	{
		image.sprite = sprite;
		image.color = color;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		ConfigureImage(contentGameObject.GetComponent<Image>());
		ConfigurePreferredLayout(contentGameObject);
	}
}
