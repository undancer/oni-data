using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootMenu : KScreen
{
	private DetailsScreen detailsScreen;

	private UserMenuScreen userMenu;

	[SerializeField]
	private GameObject detailsScreenPrefab;

	[SerializeField]
	private UserMenuScreen userMenuPrefab;

	private GameObject userMenuParent;

	[SerializeField]
	private TileScreen tileScreen;

	public KScreen buildMenu;

	private List<KScreen> subMenus = new List<KScreen>();

	private TileScreen tileScreenInst;

	public bool canTogglePauseScreen = true;

	public GameObject selectedGO;

	public static RootMenu Instance { get; private set; }

	public static void DestroyInstance()
	{
		Instance = null;
	}

	public override float GetSortKey()
	{
		return -1f;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		Subscribe(Game.Instance.gameObject, -1503271301, OnSelectObject);
		Subscribe(Game.Instance.gameObject, 288942073, OnUIClear);
		Subscribe(Game.Instance.gameObject, -809948329, OnBuildingStatechanged);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		detailsScreen = Util.KInstantiateUI(detailsScreenPrefab, base.gameObject, force_active: true).GetComponent<DetailsScreen>();
		detailsScreen.gameObject.SetActive(value: true);
		userMenuParent = detailsScreen.UserMenuPanel.gameObject;
		userMenu = Util.KInstantiateUI(userMenuPrefab.gameObject, userMenuParent).GetComponent<UserMenuScreen>();
		detailsScreen.gameObject.SetActive(value: false);
		userMenu.gameObject.SetActive(value: false);
	}

	private void OnClickCommon()
	{
		CloseSubMenus();
	}

	public void AddSubMenu(KScreen sub_menu)
	{
		if (sub_menu.activateOnSpawn)
		{
			sub_menu.Show();
		}
		subMenus.Add(sub_menu);
	}

	public void RemoveSubMenu(KScreen sub_menu)
	{
		subMenus.Remove(sub_menu);
	}

	private void CloseSubMenus()
	{
		foreach (KScreen subMenu in subMenus)
		{
			if (subMenu != null)
			{
				if (subMenu.activateOnSpawn)
				{
					subMenu.gameObject.SetActive(value: false);
				}
				else
				{
					subMenu.Deactivate();
				}
			}
		}
		subMenus.Clear();
	}

	private void OnSelectObject(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject != null)
		{
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (component != null && !component.IsInitialized())
			{
				return;
			}
		}
		if (gameObject != selectedGO)
		{
			selectedGO = gameObject;
			CloseSubMenus();
			if (selectedGO != null && (selectedGO.GetComponent<KPrefabID>() != null || CellSelectionObject.IsSelectionObject(selectedGO)))
			{
				AddSubMenu(detailsScreen);
				detailsScreen.Refresh(selectedGO);
				AddSubMenu(userMenu);
				userMenu.SetSelected(selectedGO);
				userMenu.Refresh(selectedGO);
			}
			else
			{
				userMenu.SetSelected(null);
			}
		}
	}

	private void OnBuildingStatechanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == selectedGO)
		{
			OnSelectObject(gameObject);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(Action.Escape) && SelectTool.Instance.enabled)
		{
			if (!canTogglePauseScreen)
			{
				return;
			}
			if (AreSubMenusOpen())
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back"));
				CloseSubMenus();
				SelectTool.Instance.Select(null);
			}
			else if (e.IsAction(Action.Escape))
			{
				if (!SelectTool.Instance.enabled)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
				}
				if (PlayerController.Instance.IsUsingDefaultTool())
				{
					if (SelectTool.Instance.selected != null)
					{
						SelectTool.Instance.Select(null);
					}
					else
					{
						CameraController.Instance.ForcePanningState(state: false);
						TogglePauseScreen();
					}
				}
				else
				{
					Game.Instance.Trigger(288942073);
				}
				ToolMenu.Instance.ClearSelection();
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		base.OnKeyUp(e);
		if (!e.Consumed && e.TryConsume(Action.AlternateView) && tileScreenInst != null)
		{
			tileScreenInst.Deactivate();
			tileScreenInst = null;
		}
	}

	public void TogglePauseScreen()
	{
		PauseScreen.Instance.Show();
	}

	public void ExternalClose()
	{
		OnClickCommon();
	}

	private void OnUIClear(object data)
	{
		CloseSubMenus();
		if (UnityEngine.EventSystems.EventSystem.current != null)
		{
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		}
		else
		{
			Debug.LogWarning("OnUIClear() Event system is null");
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
	}

	private bool AreSubMenusOpen()
	{
		return subMenus.Count > 0;
	}

	private KToggleMenu.ToggleInfo[] GetFillers()
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		List<KToggleMenu.ToggleInfo> list = new List<KToggleMenu.ToggleInfo>();
		foreach (Pickupable item in Components.Pickupables.Items)
		{
			KPrefabID kPrefabID = item.KPrefabID;
			if (kPrefabID.HasTag(GameTags.Filler) && hashSet.Add(kPrefabID.PrefabTag))
			{
				string text = kPrefabID.GetComponent<PrimaryElement>().Element.id.ToString();
				list.Add(new KToggleMenu.ToggleInfo(text));
			}
		}
		return list.ToArray();
	}

	public bool IsBuildingChorePanelActive()
	{
		if (detailsScreen != null)
		{
			return detailsScreen.GetActiveTab() is BuildingChoresPanel;
		}
		return false;
	}
}
