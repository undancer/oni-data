using UnityEngine;

public class HoverTextScreen : KScreen
{
	[SerializeField]
	private HoverTextSkin skin;

	public Sprite[] HoverIcons;

	public HoverTextDrawer drawer;

	public static HoverTextScreen Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		drawer = new HoverTextDrawer(skin.skin, GetComponent<RectTransform>());
	}

	public HoverTextDrawer BeginDrawing()
	{
		Vector2 localPoint = Vector2.zero;
		Vector2 screenPoint = KInputManager.GetMousePos();
		RectTransform rectTransform = base.transform.parent as RectTransform;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, base.transform.parent.GetComponent<Canvas>().worldCamera, out localPoint);
		localPoint.x += rectTransform.sizeDelta.x / 2f;
		localPoint.y -= rectTransform.sizeDelta.y / 2f;
		drawer.BeginDrawing(localPoint);
		return drawer;
	}

	private void Update()
	{
		bool flag = PlayerController.Instance.ActiveTool.ShowHoverUI();
		drawer.SetEnabled(flag);
	}

	public Sprite GetSprite(string byName)
	{
		Sprite[] hoverIcons = HoverIcons;
		foreach (Sprite sprite in hoverIcons)
		{
			if (sprite != null && sprite.name == byName)
			{
				return sprite;
			}
		}
		Debug.LogWarning("No icon named " + byName + " was found on HoverTextScreen.prefab");
		return null;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		drawer.Cleanup();
	}
}
