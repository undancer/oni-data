using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FilterSideScreenRow")]
public class FilterSideScreenRow : KMonoBehaviour
{
	[SerializeField]
	private LocText labelText;

	[SerializeField]
	private Image BG;

	[SerializeField]
	private Image outline;

	[SerializeField]
	private Color outlineHighLightColor = new Color32(168, 74, 121, byte.MaxValue);

	[SerializeField]
	private Color BGHighLightColor = new Color32(168, 74, 121, 80);

	[SerializeField]
	private Color outlineDefaultColor = new Color32(204, 204, 204, byte.MaxValue);

	private Color regularColor = Color.white;

	[SerializeField]
	public KButton button;

	public new Tag tag { get; private set; }

	public bool isSelected { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		regularColor = outline.color;
		if (!(button != null))
		{
			return;
		}
		button.onPointerEnter += delegate
		{
			if (!isSelected)
			{
				outline.color = outlineHighLightColor;
			}
		};
		button.onPointerExit += delegate
		{
			if (!isSelected)
			{
				outline.color = regularColor;
			}
		};
	}

	public void SetTag(Tag tag)
	{
		this.tag = tag;
		SetText((tag == GameTags.Void) ? UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION.text : tag.ProperName());
	}

	private void SetText(string assignmentStr)
	{
		labelText.text = ((!string.IsNullOrEmpty(assignmentStr)) ? assignmentStr : "-");
	}

	public void SetSelected(bool selected)
	{
		isSelected = selected;
		outline.color = (selected ? outlineHighLightColor : outlineDefaultColor);
		BG.color = (selected ? BGHighLightColor : Color.white);
	}
}
