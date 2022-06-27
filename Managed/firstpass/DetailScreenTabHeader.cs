using UnityEngine;
using UnityEngine.UI;

public class DetailScreenTabHeader : KTabMenuHeader
{
	public float SelectedHeight = 36f;

	public float UnselectedHeight = 30f;

	public override void ActivateTabArtwork(int tabIdx)
	{
		base.ActivateTabArtwork(tabIdx);
		if (tabIdx >= base.transform.childCount)
		{
			return;
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			LayoutElement component = base.transform.GetChild(i).GetComponent<LayoutElement>();
			if (component != null)
			{
				if (i == tabIdx)
				{
					component.preferredHeight = SelectedHeight;
					component.transform.Find("Icon").GetComponent<Image>().color = new Color(0.14509805f, 14f / 85f, 0.23137255f);
				}
				else
				{
					component.preferredHeight = UnselectedHeight;
					component.transform.Find("Icon").GetComponent<Image>().color = new Color(0.35686275f, 19f / 51f, 23f / 51f);
				}
			}
		}
	}
}
