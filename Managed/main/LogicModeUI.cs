using UnityEngine;

public class LogicModeUI : ScriptableObject
{
	[Header("Base Assets")]
	public Sprite inputSprite;

	public Sprite outputSprite;

	public Sprite resetSprite;

	public GameObject prefab;

	public GameObject ribbonInputPrefab;

	public GameObject ribbonOutputPrefab;

	public GameObject controlInputPrefab;

	[Header("Colouring")]
	public Color32 colourOn = new Color32(0, byte.MaxValue, 0, 0);

	public Color32 colourOff = new Color32(byte.MaxValue, 0, 0, 0);

	public Color32 colourDisconnected = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public Color32 colourOnProtanopia = new Color32(179, 204, 0, 0);

	public Color32 colourOffProtanopia = new Color32(166, 51, 102, 0);

	public Color32 colourOnDeuteranopia = new Color32(128, 0, 128, 0);

	public Color32 colourOffDeuteranopia = new Color32(byte.MaxValue, 153, 0, 0);

	public Color32 colourOnTritanopia = new Color32(51, 102, byte.MaxValue, 0);

	public Color32 colourOffTritanopia = new Color32(byte.MaxValue, 153, 0, 0);
}
