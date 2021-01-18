using UnityEngine;

public class LegendEntry
{
	public string name;

	public string desc;

	public string desc_arg;

	public Color colour;

	public Sprite sprite;

	public bool displaySprite;

	public LegendEntry(string name, string desc, Color colour, string desc_arg = null, Sprite sprite = null, bool displaySprite = true)
	{
		this.name = name;
		this.desc = desc;
		this.colour = colour;
		this.desc_arg = desc_arg;
		this.sprite = ((sprite == null) ? Assets.instance.LegendColourBox : sprite);
		this.displaySprite = displaySprite;
	}
}
