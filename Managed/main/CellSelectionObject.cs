using System;
using System.Collections.Generic;
using ProcGen;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CellSelectionObject")]
public class CellSelectionObject : KMonoBehaviour
{
	[HideInInspector]
	public CellSelectionObject alternateSelectionObject;

	private float zDepth = -0.5f;

	private float zDepthSelected;

	private KBoxCollider2D mCollider;

	private KSelectable mSelectable;

	private Vector3 offset = new Vector3(0.5f, 0.5f, 0f);

	public GameObject SelectedDisplaySprite;

	public Sprite Sprite_Selected;

	public Sprite Sprite_Hover;

	public int mouseCell;

	private int selectedCell;

	public string ElementName;

	public Element element;

	public Element.State state;

	public float Mass;

	public float temperature;

	public Tag tags;

	public byte diseaseIdx;

	public int diseaseCount;

	private float updateTimer;

	private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();

	private bool isAppFocused = true;

	public int SelectedCell => selectedCell;

	public float FlowRate => Grid.AccumulatedFlow[selectedCell] / 3f;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		mCollider = GetComponent<KBoxCollider2D>();
		mCollider.size = new Vector2(1.1f, 1.1f);
		mSelectable = GetComponent<KSelectable>();
		SelectedDisplaySprite.transform.localScale = Vector3.one * (25f / 64f);
		SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Hover;
		Subscribe(Game.Instance.gameObject, 493375141, ForceRefreshUserMenu);
		overlayFilterMap.Add(OverlayModes.Oxygen.ID, () => Grid.Element[mouseCell].IsGas);
		overlayFilterMap.Add(OverlayModes.GasConduits.ID, () => Grid.Element[mouseCell].IsGas);
		overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, () => Grid.Element[mouseCell].IsLiquid);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		isAppFocused = focusStatus;
	}

	private void Update()
	{
		if (!isAppFocused || SelectTool.Instance == null || Game.Instance == null || !Game.Instance.GameStarted())
		{
			return;
		}
		SelectedDisplaySprite.SetActive(PlayerController.Instance.IsUsingDefaultTool() && !DebugHandler.HideUI);
		if (SelectTool.Instance.selected != mSelectable)
		{
			mouseCell = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			if (Grid.IsValidCell(mouseCell) && Grid.IsVisible(mouseCell))
			{
				bool flag = true;
				foreach (KeyValuePair<HashedString, Func<bool>> item in overlayFilterMap)
				{
					if (item.Value == null)
					{
						Debug.LogWarning("Filter value is null");
					}
					else if (OverlayScreen.Instance == null)
					{
						Debug.LogWarning("Overlay screen Instance is null");
					}
					else if (OverlayScreen.Instance.GetMode() == item.Key)
					{
						flag = false;
						if (base.gameObject.layer != LayerMask.NameToLayer("MaskedOverlay"))
						{
							base.gameObject.layer = LayerMask.NameToLayer("MaskedOverlay");
						}
						if (!item.Value())
						{
							SelectedDisplaySprite.SetActive(value: false);
							return;
						}
						break;
					}
				}
				if (flag && base.gameObject.layer != LayerMask.NameToLayer("Default"))
				{
					base.gameObject.layer = LayerMask.NameToLayer("Default");
				}
				Vector3 position = Grid.CellToPos(mouseCell, 0f, 0f, 0f) + offset;
				position.z = zDepth;
				base.transform.SetPosition(position);
				mSelectable.SetName(Grid.Element[mouseCell].name);
			}
			if (SelectTool.Instance.hover != mSelectable)
			{
				SelectedDisplaySprite.SetActive(value: false);
			}
		}
		updateTimer += Time.deltaTime;
		if (updateTimer >= 0.5f)
		{
			updateTimer = 0f;
			if (SelectTool.Instance.selected == mSelectable)
			{
				UpdateValues();
			}
		}
	}

	public void UpdateValues()
	{
		if (!Grid.IsValidCell(selectedCell))
		{
			return;
		}
		Mass = Grid.Mass[selectedCell];
		element = Grid.Element[selectedCell];
		ElementName = element.name;
		state = element.state;
		tags = element.GetMaterialCategoryTag();
		temperature = Grid.Temperature[selectedCell];
		diseaseIdx = Grid.DiseaseIdx[selectedCell];
		diseaseCount = Grid.DiseaseCount[selectedCell];
		mSelectable.SetName(Grid.Element[selectedCell].name);
		DetailsScreen.Instance.Trigger(-1514841199);
		UpdateStatusItem();
		int num = Grid.CellAbove(selectedCell);
		bool flag = element.IsLiquid && Grid.IsValidCell(num) && (Grid.Element[num].IsGas || Grid.Element[num].IsVacuum);
		if (element.sublimateId != 0 && (element.IsSolid || flag))
		{
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationEmitting, this);
			GameUtil.IsEmissionBlocked(selectedCell, out var all_not_gaseous, out var all_over_pressure);
			if (all_not_gaseous)
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationBlocked, this);
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
			}
			else if (all_over_pressure)
			{
				mSelectable.AddStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure, this);
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
			}
			else
			{
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
				mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
			}
		}
		else
		{
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationEmitting);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationBlocked);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.SublimationOverpressure);
		}
		if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(selectedCell))
		{
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.BuriedItem, this);
		}
		else
		{
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.BuriedItem, immediate: true);
		}
		bool on = IsExposedToSpace(selectedCell);
		mSelectable.ToggleStatusItem(Db.Get().MiscStatusItems.Space, on);
	}

	public static bool IsExposedToSpace(int cell)
	{
		if (Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space)
		{
			return Grid.Objects[cell, 2] == null;
		}
		return false;
	}

	private void UpdateStatusItem()
	{
		if (element.id == SimHashes.Vacuum || element.id == SimHashes.Void)
		{
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalCategory, immediate: true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, immediate: true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalMass, immediate: true);
			mSelectable.RemoveStatusItem(Db.Get().MiscStatusItems.ElementalDisease, immediate: true);
			return;
		}
		if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalCategory))
		{
			Func<Element> data = () => element;
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, data);
		}
		if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalTemperature))
		{
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalTemperature, this);
		}
		if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalMass))
		{
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalMass, this);
		}
		if (!mSelectable.HasStatusItem(Db.Get().MiscStatusItems.ElementalDisease))
		{
			mSelectable.AddStatusItem(Db.Get().MiscStatusItems.ElementalDisease, this);
		}
	}

	public void OnObjectSelected(object o)
	{
		SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Hover;
		UpdateStatusItem();
		if (SelectTool.Instance.selected == mSelectable)
		{
			selectedCell = Grid.PosToCell(base.gameObject);
			UpdateValues();
			Vector3 position = Grid.CellToPos(selectedCell, 0f, 0f, 0f) + offset;
			position.z = zDepthSelected;
			base.transform.SetPosition(position);
			SelectedDisplaySprite.GetComponent<SpriteRenderer>().sprite = Sprite_Selected;
		}
	}

	public string MassString()
	{
		return $"{Mass:0.00}";
	}

	private void ForceRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.Refresh(base.gameObject);
	}
}
