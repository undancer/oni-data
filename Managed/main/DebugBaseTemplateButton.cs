using System;
using System.Collections.Generic;
using Klei.AI;
using TemplateClasses;
using TMPro;
using UnityEngine;

public class DebugBaseTemplateButton : KScreen
{
	private bool SaveAllBuildings = false;

	private bool SaveAllPickups = false;

	public KButton saveBaseButton;

	public KButton clearButton;

	private TemplateContainer pasteAndSelectAsset;

	public KButton AddSelectionButton;

	public KButton RemoveSelectionButton;

	public KButton clearSelectionButton;

	public KButton DestroyButton;

	public KButton DeconstructButton;

	public KButton MoveButton;

	public TemplateContainer moveAsset;

	public TMP_InputField nameField;

	private string SaveName = "enter_template_name";

	public GameObject Placer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public List<int> SelectedCells = new List<int>();

	public static DebugBaseTemplateButton Instance
	{
		get;
		private set;
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
		base.gameObject.SetActive(value: false);
		SetupLocText();
		base.ConsumeMouseScroll = true;
		TMP_InputField tMP_InputField = nameField;
		tMP_InputField.onFocus = (System.Action)Delegate.Combine(tMP_InputField.onFocus, (System.Action)delegate
		{
			base.isEditing = true;
		});
		nameField.onEndEdit.AddListener(delegate
		{
			base.isEditing = false;
		});
		nameField.onValueChanged.AddListener(delegate
		{
			Util.ScrubInputField(nameField, isPath: true);
		});
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (saveBaseButton != null)
		{
			saveBaseButton.onClick -= OnClickSaveBase;
			saveBaseButton.onClick += OnClickSaveBase;
		}
		if (clearButton != null)
		{
			clearButton.onClick -= OnClickClear;
			clearButton.onClick += OnClickClear;
		}
		if (AddSelectionButton != null)
		{
			AddSelectionButton.onClick -= OnClickAddSelection;
			AddSelectionButton.onClick += OnClickAddSelection;
		}
		if (RemoveSelectionButton != null)
		{
			RemoveSelectionButton.onClick -= OnClickRemoveSelection;
			RemoveSelectionButton.onClick += OnClickRemoveSelection;
		}
		if (clearSelectionButton != null)
		{
			clearSelectionButton.onClick -= OnClickClearSelection;
			clearSelectionButton.onClick += OnClickClearSelection;
		}
		if (MoveButton != null)
		{
			MoveButton.onClick -= OnClickMove;
			MoveButton.onClick += OnClickMove;
		}
		if (DestroyButton != null)
		{
			DestroyButton.onClick -= OnClickDestroySelection;
			DestroyButton.onClick += OnClickDestroySelection;
		}
		if (DeconstructButton != null)
		{
			DeconstructButton.onClick -= OnClickDeconstructSelection;
			DeconstructButton.onClick += OnClickDeconstructSelection;
		}
	}

	private void SetupLocText()
	{
	}

	private void OnClickDestroySelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Destroy);
	}

	private void OnClickDeconstructSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Deconstruct);
	}

	private void OnClickMove()
	{
		DebugTool.Instance.DeactivateTool();
		moveAsset = GetSelectionAsAsset();
		StampTool.Instance.Activate(moveAsset);
	}

	private void OnClickAddSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.AddSelection);
	}

	private void OnClickRemoveSelection()
	{
		DebugTool.Instance.Activate(DebugTool.Type.RemoveSelection);
	}

	private void OnClickClearSelection()
	{
		ClearSelection();
		nameField.text = "";
	}

	private void OnClickClear()
	{
		DebugTool.Instance.Activate(DebugTool.Type.Clear);
	}

	protected override void OnDeactivate()
	{
		if (DebugTool.Instance != null)
		{
			DebugTool.Instance.DeactivateTool();
		}
		base.OnDeactivate();
	}

	private void OnDisable()
	{
		if (DebugTool.Instance != null)
		{
			DebugTool.Instance.DeactivateTool();
		}
	}

	private TemplateContainer GetSelectionAsAsset()
	{
		List<Cell> list = new List<Cell>();
		List<Prefab> list2 = new List<Prefab>();
		List<Prefab> list3 = new List<Prefab>();
		List<Prefab> _primaryElementOres = new List<Prefab>();
		List<Prefab> _otherEntities = new List<Prefab>();
		HashSet<GameObject> _excludeEntities = new HashSet<GameObject>();
		float num = 0f;
		float num2 = 0f;
		foreach (int selectedCell in SelectedCells)
		{
			num += (float)Grid.CellToXY(selectedCell).x;
			num2 += (float)Grid.CellToXY(selectedCell).y;
		}
		float x = num / (float)SelectedCells.Count;
		float y = (num2 /= (float)SelectedCells.Count);
		int cell = Grid.PosToCell(new Vector3(x, y, 0f));
		Grid.CellToXY(cell, out var rootX, out var rootY);
		for (int i = 0; i < SelectedCells.Count; i++)
		{
			int i2 = SelectedCells[i];
			Grid.CellToXY(SelectedCells[i], out var x2, out var y2);
			Element element = ElementLoader.elements[Grid.ElementIdx[i2]];
			string diseaseName = ((Grid.DiseaseIdx[i2] != byte.MaxValue) ? Db.Get().Diseases[Grid.DiseaseIdx[i2]].Id : null);
			list.Add(new Cell(x2 - rootX, y2 - rootY, element.id, Grid.Temperature[i2], Grid.Mass[i2], diseaseName, Grid.DiseaseCount[i2], Grid.PreventFogOfWarReveal[SelectedCells[i]]));
		}
		for (int j = 0; j < Components.BuildingCompletes.Count; j++)
		{
			BuildingComplete buildingComplete = Components.BuildingCompletes[j];
			if (_excludeEntities.Contains(buildingComplete.gameObject))
			{
				continue;
			}
			Grid.CellToXY(Grid.PosToCell(buildingComplete), out var x3, out var y3);
			if (!SaveAllBuildings && !SelectedCells.Contains(Grid.PosToCell(buildingComplete)))
			{
				continue;
			}
			int[] placementCells = buildingComplete.PlacementCells;
			string diseaseName2;
			foreach (int num3 in placementCells)
			{
				Grid.CellToXY(num3, out var xplace, out var yplace);
				diseaseName2 = ((Grid.DiseaseIdx[num3] != byte.MaxValue) ? Db.Get().Diseases[Grid.DiseaseIdx[num3]].Id : null);
				if (list.Find((Cell c) => c.location_x == xplace - rootX && c.location_y == yplace - rootY) == null)
				{
					list.Add(new Cell(xplace - rootX, yplace - rootY, Grid.Element[num3].id, Grid.Temperature[num3], Grid.Mass[num3], diseaseName2, Grid.DiseaseCount[num3]));
				}
			}
			Orientation rotation = Orientation.Neutral;
			Rotatable component = buildingComplete.gameObject.GetComponent<Rotatable>();
			if (component != null)
			{
				rotation = component.GetOrientation();
			}
			SimHashes element2 = SimHashes.Void;
			float value = 280f;
			diseaseName2 = null;
			int disease_count = 0;
			PrimaryElement component2 = buildingComplete.GetComponent<PrimaryElement>();
			if (component2 != null)
			{
				element2 = component2.ElementID;
				value = component2.Temperature;
				diseaseName2 = ((component2.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[component2.DiseaseIdx].Id : null);
				disease_count = component2.DiseaseCount;
			}
			List<Prefab.template_amount_value> list4 = new List<Prefab.template_amount_value>();
			List<Prefab.template_amount_value> list5 = new List<Prefab.template_amount_value>();
			foreach (AmountInstance amount in buildingComplete.gameObject.GetAmounts())
			{
				list4.Add(new Prefab.template_amount_value(amount.amount.Id, amount.value));
			}
			float num4 = 0f;
			Battery component3 = buildingComplete.GetComponent<Battery>();
			if (component3 != null)
			{
				num4 = component3.JoulesAvailable;
				list5.Add(new Prefab.template_amount_value("joulesAvailable", num4));
			}
			float num5 = 0f;
			Unsealable component4 = buildingComplete.GetComponent<Unsealable>();
			if (component4 != null)
			{
				num5 = (component4.facingRight ? 1 : 0);
				list5.Add(new Prefab.template_amount_value("sealedDoorDirection", num5));
			}
			float num6 = 0f;
			LogicSwitch component5 = buildingComplete.GetComponent<LogicSwitch>();
			if (component5 != null)
			{
				num6 = (component5.IsSwitchedOn ? 1 : 0);
				list5.Add(new Prefab.template_amount_value("switchSetting", num6));
			}
			x3 -= rootX;
			y3 -= rootY;
			value = Mathf.Clamp(value, 1f, 99999f);
			Prefab prefab = new Prefab(buildingComplete.PrefabID().Name, Prefab.Type.Building, x3, y3, element2, value, 0f, diseaseName2, disease_count, rotation, list4.ToArray(), list5.ToArray());
			Storage component6 = buildingComplete.gameObject.GetComponent<Storage>();
			if (component6 != null)
			{
				foreach (GameObject item2 in component6.items)
				{
					float units = 0f;
					SimHashes element3 = SimHashes.Vacuum;
					float temp = 280f;
					string disease = null;
					int disease_count2 = 0;
					bool isOre = false;
					PrimaryElement component7 = item2.GetComponent<PrimaryElement>();
					if (component7 != null)
					{
						units = component7.Units;
						element3 = component7.ElementID;
						temp = component7.Temperature;
						disease = ((component7.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[component7.DiseaseIdx].Id : null);
						disease_count2 = component7.DiseaseCount;
					}
					float rotAmount = 0f;
					Rottable.Instance sMI = item2.gameObject.GetSMI<Rottable.Instance>();
					if (sMI != null)
					{
						rotAmount = sMI.RotValue;
					}
					ElementChunk component8 = item2.GetComponent<ElementChunk>();
					if (component8 != null)
					{
						isOre = true;
					}
					StorageItem storageItem = new StorageItem(item2.PrefabID().Name, units, temp, element3, disease, disease_count2, isOre);
					if (sMI != null)
					{
						storageItem.rottable.rotAmount = rotAmount;
					}
					prefab.AssignStorage(storageItem);
					_excludeEntities.Add(item2);
				}
			}
			list2.Add(prefab);
			_excludeEntities.Add(buildingComplete.gameObject);
		}
		for (int l = 0; l < list2.Count; l++)
		{
			Prefab prefab2 = list2[l];
			int x4 = prefab2.location_x + rootX;
			int y4 = prefab2.location_y + rootY;
			int cell2 = Grid.XYToCell(x4, y4);
			switch (prefab2.id)
			{
			default:
				prefab2.connections = 0;
				break;
			case "Wire":
			case "InsulatedWire":
			case "HighWattageWire":
			case "WireRefined":
				prefab2.connections = (int)Game.Instance.electricalConduitSystem.GetConnections(cell2, is_physical_building: true);
				break;
			case "GasConduit":
			case "InsulatedGasConduit":
				prefab2.connections = (int)Game.Instance.gasConduitSystem.GetConnections(cell2, is_physical_building: true);
				break;
			case "LiquidConduit":
			case "InsulatedLiquidConduit":
				prefab2.connections = (int)Game.Instance.liquidConduitSystem.GetConnections(cell2, is_physical_building: true);
				break;
			case "LogicWire":
				prefab2.connections = (int)Game.Instance.logicCircuitSystem.GetConnections(cell2, is_physical_building: true);
				break;
			case "SolidConduit":
				prefab2.connections = (int)Game.Instance.solidConduitSystem.GetConnections(cell2, is_physical_building: true);
				break;
			}
		}
		for (int m = 0; m < Components.Pickupables.Count; m++)
		{
			if (!Components.Pickupables[m].gameObject.activeSelf)
			{
				continue;
			}
			Pickupable pickupable = Components.Pickupables[m];
			if (_excludeEntities.Contains(pickupable.gameObject))
			{
				continue;
			}
			int num7 = Grid.PosToCell(pickupable);
			if ((SaveAllPickups || SelectedCells.Contains(num7)) && !Components.Pickupables[m].gameObject.GetComponent<MinionBrain>())
			{
				Grid.CellToXY(num7, out var x5, out var y5);
				x5 -= rootX;
				y5 -= rootY;
				SimHashes element4 = SimHashes.Void;
				float temperature = 280f;
				float units2 = 1f;
				string disease2 = null;
				int disease_count3 = 0;
				float rotAmount2 = 0f;
				Rottable.Instance sMI2 = pickupable.gameObject.GetSMI<Rottable.Instance>();
				if (sMI2 != null)
				{
					rotAmount2 = sMI2.RotValue;
				}
				PrimaryElement component9 = pickupable.gameObject.GetComponent<PrimaryElement>();
				if (component9 != null)
				{
					element4 = component9.ElementID;
					units2 = component9.Units;
					temperature = component9.Temperature;
					disease2 = ((component9.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[component9.DiseaseIdx].Id : null);
					disease_count3 = component9.DiseaseCount;
				}
				ElementChunk component10 = pickupable.gameObject.GetComponent<ElementChunk>();
				if (component10 != null)
				{
					Prefab item = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Ore, x5, y5, element4, temperature, units2, disease2, disease_count3);
					_primaryElementOres.Add(item);
				}
				else
				{
					Prefab item = new Prefab(pickupable.PrefabID().Name, Prefab.Type.Pickupable, x5, y5, element4, temperature, units2, disease2, disease_count3);
					item.rottable = new TemplateClasses.Rottable();
					item.rottable.rotAmount = rotAmount2;
					list3.Add(item);
				}
				_excludeEntities.Add(pickupable.gameObject);
			}
		}
		GetEntities(Components.Crops.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Health.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Harvestables.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities(Components.Edibles.Items, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<Geyser>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<OccupyArea>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		GetEntities<FogOfWarMask>(rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
		TemplateContainer templateContainer = new TemplateContainer();
		templateContainer.Init(list, list2, list3, _primaryElementOres, _otherEntities);
		return templateContainer;
	}

	private void GetEntities<T>(int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		object[] array = UnityEngine.Object.FindObjectsOfType(typeof(T));
		object[] component_collection = array;
		GetEntities(component_collection, rootX, rootY, ref _primaryElementOres, ref _otherEntities, ref _excludeEntities);
	}

	private void GetEntities<T>(IEnumerable<T> component_collection, int rootX, int rootY, ref List<Prefab> _primaryElementOres, ref List<Prefab> _otherEntities, ref HashSet<GameObject> _excludeEntities)
	{
		foreach (T item2 in component_collection)
		{
			if (_excludeEntities.Contains((item2 as KMonoBehaviour).gameObject) || !(item2 as KMonoBehaviour).gameObject.activeSelf)
			{
				continue;
			}
			int num = Grid.PosToCell(item2 as KMonoBehaviour);
			if (!SelectedCells.Contains(num) || (bool)(item2 as KMonoBehaviour).gameObject.GetComponent<MinionBrain>())
			{
				continue;
			}
			Grid.CellToXY(num, out var x, out var y);
			x -= rootX;
			y -= rootY;
			SimHashes element = SimHashes.Void;
			float temperature = 280f;
			float units = 1f;
			string disease = null;
			int disease_count = 0;
			PrimaryElement component = (item2 as KMonoBehaviour).gameObject.GetComponent<PrimaryElement>();
			if (component != null)
			{
				element = component.ElementID;
				units = component.Units;
				temperature = component.Temperature;
				disease = ((component.DiseaseIdx != byte.MaxValue) ? Db.Get().Diseases[component.DiseaseIdx].Id : null);
				disease_count = component.DiseaseCount;
			}
			List<Prefab.template_amount_value> list = new List<Prefab.template_amount_value>();
			if ((item2 as KMonoBehaviour).gameObject.GetAmounts() != null)
			{
				foreach (AmountInstance amount in (item2 as KMonoBehaviour).gameObject.GetAmounts())
				{
					list.Add(new Prefab.template_amount_value(amount.amount.Id, amount.value));
				}
			}
			ElementChunk component2 = (item2 as KMonoBehaviour).gameObject.GetComponent<ElementChunk>();
			if (component2 != null)
			{
				Prefab item = new Prefab((item2 as KMonoBehaviour).PrefabID().Name, Prefab.Type.Ore, x, y, element, temperature, units, disease, disease_count, Orientation.Neutral, list.ToArray());
				_primaryElementOres.Add(item);
				_excludeEntities.Add((item2 as KMonoBehaviour).gameObject);
			}
			else
			{
				Prefab item = new Prefab((item2 as KMonoBehaviour).PrefabID().Name, Prefab.Type.Other, x, y, element, temperature, units, disease, disease_count, Orientation.Neutral, list.ToArray());
				_otherEntities.Add(item);
				_excludeEntities.Add((item2 as KMonoBehaviour).gameObject);
			}
		}
	}

	private void OnClickSaveBase()
	{
		TemplateContainer selectionAsAsset = GetSelectionAsAsset();
		if (SelectedCells.Count <= 0)
		{
			Debug.LogWarning("No cells selected. Use buttons above to select the area you want to save.");
			return;
		}
		SaveName = nameField.text;
		if (SaveName == null || SaveName == "")
		{
			Debug.LogWarning("Invalid save name. Please enter a name in the input field.");
			return;
		}
		selectionAsAsset.SaveToYaml(SaveName);
		TemplateCache.Clear();
		TemplateCache.Init();
		PasteBaseTemplateScreen.Instance.RefreshStampButtons();
	}

	public void ClearSelection()
	{
		for (int num = SelectedCells.Count - 1; num >= 0; num--)
		{
			RemoveFromSelection(SelectedCells[num]);
		}
	}

	public void DestroySelection()
	{
	}

	public void DeconstructSelection()
	{
	}

	public void AddToSelection(int cell)
	{
		if (!SelectedCells.Contains(cell))
		{
			GameObject gameObject2 = (Grid.Objects[cell, 7] = Util.KInstantiate(Placer));
			Vector3 position = Grid.CellToPosCBC(cell, visualizerLayer);
			float num = -0.15f;
			position.z += num;
			gameObject2.transform.SetPosition(position);
			SelectedCells.Add(cell);
		}
	}

	public void RemoveFromSelection(int cell)
	{
		if (SelectedCells.Contains(cell))
		{
			GameObject gameObject = Grid.Objects[cell, 7];
			if (gameObject != null)
			{
				gameObject.DeleteObject();
			}
			SelectedCells.Remove(cell);
		}
	}
}
