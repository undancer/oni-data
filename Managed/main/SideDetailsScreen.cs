using System.Collections.Generic;
using UnityEngine;

public class SideDetailsScreen : KScreen
{
	[SerializeField]
	private List<SideTargetScreen> screens;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private RectTransform body;

	private RectTransform rectTransform;

	private Dictionary<string, SideTargetScreen> screenMap;

	private SideTargetScreen activeScreen;

	public static SideDetailsScreen Instance;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Instance = this;
		Initialize();
		base.gameObject.SetActive(value: false);
	}

	protected override void OnForcedCleanUp()
	{
		Instance = null;
		base.OnForcedCleanUp();
	}

	private void Initialize()
	{
		if (screens == null)
		{
			return;
		}
		rectTransform = GetComponent<RectTransform>();
		screenMap = new Dictionary<string, SideTargetScreen>();
		List<SideTargetScreen> list = new List<SideTargetScreen>();
		foreach (SideTargetScreen screen in screens)
		{
			SideTargetScreen sideTargetScreen = Util.KInstantiateUI<SideTargetScreen>(screen.gameObject, body.gameObject);
			sideTargetScreen.gameObject.SetActive(value: false);
			list.Add(sideTargetScreen);
		}
		list.ForEach(delegate(SideTargetScreen s)
		{
			screenMap.Add(s.name, s);
		});
		backButton.onClick += delegate
		{
			Show(show: false);
		};
	}

	public void SetTitle(string newTitle)
	{
		title.text = newTitle;
	}

	public void SetScreen(string screenName, object content, float x)
	{
		if (!screenMap.ContainsKey(screenName))
		{
			Debug.LogError("Tried to open a screen that does exist on the manager!");
			return;
		}
		if (content == null)
		{
			Debug.LogError("Tried to set " + screenName + " with null content!");
			return;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(value: true);
		}
		Rect rect = rectTransform.rect;
		rectTransform.offsetMin = new Vector2(x, rectTransform.offsetMin.y);
		rectTransform.offsetMax = new Vector2(x + rect.width, rectTransform.offsetMax.y);
		if (activeScreen != null)
		{
			activeScreen.gameObject.SetActive(value: false);
		}
		activeScreen = screenMap[screenName];
		activeScreen.gameObject.SetActive(value: true);
		SetTitle(activeScreen.displayName);
		activeScreen.SetTarget(content);
	}
}
