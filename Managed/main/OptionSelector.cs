using System;
using System.Collections.Generic;
using UnityEngine;

public class OptionSelector : MonoBehaviour
{
	public class DisplayOptionInfo
	{
		public IList<Sprite> bgOptions;

		public IList<Sprite> fgOptions;

		public int bgIndex;

		public int fgIndex;

		public Color32 fillColour;
	}

	private object id;

	public Action<object, int> OnChangePriority;

	[SerializeField]
	private KImage selectedItem;

	[SerializeField]
	private KImage itemTemplate;

	private void Start()
	{
		selectedItem.GetComponent<KButton>().onBtnClick += OnClick;
	}

	public void Initialize(object id)
	{
		this.id = id;
	}

	private void OnClick(KKeyCode button)
	{
		switch (button)
		{
		case KKeyCode.Mouse0:
			OnChangePriority(id, 1);
			break;
		case KKeyCode.Mouse1:
			OnChangePriority(id, -1);
			break;
		}
	}

	public void ConfigureItem(bool disabled, DisplayOptionInfo display_info)
	{
		HierarchyReferences component = selectedItem.GetComponent<HierarchyReferences>();
		KImage kImage = component.GetReference("BG") as KImage;
		if (display_info.bgOptions == null)
		{
			kImage.gameObject.SetActive(value: false);
		}
		else
		{
			kImage.sprite = display_info.bgOptions[display_info.bgIndex];
		}
		KImage kImage2 = component.GetReference("FG") as KImage;
		if (display_info.fgOptions == null)
		{
			kImage2.gameObject.SetActive(value: false);
		}
		else
		{
			kImage2.sprite = display_info.fgOptions[display_info.fgIndex];
		}
		KImage kImage3 = component.GetReference("Fill") as KImage;
		if (kImage3 != null)
		{
			kImage3.enabled = !disabled;
			kImage3.color = display_info.fillColour;
		}
		KImage kImage4 = component.GetReference("Outline") as KImage;
		if (kImage4 != null)
		{
			kImage4.enabled = !disabled;
		}
	}
}
