using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterableSideScreenElement")]
public class TreeFilterableSideScreenElement : KMonoBehaviour
{
	[SerializeField]
	private LocText elementName;

	[SerializeField]
	private MultiToggle checkBox;

	[SerializeField]
	private KImage elementImg;

	private KImage checkBoxImg;

	private Tag elementTag;

	private TreeFilterableSideScreen parent;

	private bool initialized = false;

	public bool IsSelected => checkBox.CurrentState == 1;

	public TreeFilterableSideScreen Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
		}
	}

	public event Action<Tag, bool> OnSelectionChanged;

	public Tag GetElementTag()
	{
		return elementTag;
	}

	public MultiToggle GetCheckboxToggle()
	{
		return checkBox;
	}

	private void Initialize()
	{
		if (!initialized)
		{
			checkBoxImg = checkBox.gameObject.GetComponentInChildrenOnly<KImage>();
			checkBox.onClick = CheckBoxClicked;
			initialized = true;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Initialize();
	}

	public Sprite GetStorageObjectSprite(Tag t)
	{
		Sprite result = null;
		GameObject prefab = Assets.GetPrefab(t);
		if (prefab != null)
		{
			KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
			if (component != null)
			{
				result = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0]);
			}
		}
		return result;
	}

	public void SetSprite(Tag t)
	{
		Tuple<Sprite, Color> uISprite = Def.GetUISprite(t);
		elementImg.sprite = uISprite.first;
		elementImg.color = uISprite.second;
		elementImg.gameObject.SetActive(value: true);
	}

	public void SetTag(Tag newTag)
	{
		Initialize();
		elementTag = newTag;
		SetSprite(elementTag);
		string text = elementTag.ProperName();
		if (parent.IsStorage)
		{
			float amountInStorage = parent.GetAmountInStorage(elementTag);
			text = text + ": " + GameUtil.GetFormattedMass(amountInStorage);
		}
		elementName.text = text;
	}

	private void CheckBoxClicked()
	{
		SetCheckBox(!parent.IsTagAllowed(GetElementTag()));
	}

	public void SetCheckBox(bool checkBoxState)
	{
		checkBox.ChangeState(checkBoxState ? 1 : 0);
		checkBoxImg.enabled = checkBoxState;
		if (this.OnSelectionChanged != null)
		{
			this.OnSelectionChanged(GetElementTag(), checkBoxState);
		}
	}
}
