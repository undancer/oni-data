using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class SandboxSprinkleTool : BrushTool
{
	public static SandboxSprinkleTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();

	private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.noiseScaleSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.noiseDensitySlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(value: true);
		SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(settings.GetIntSetting("SandboxTools.BrushSize"));
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(value: false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int recentlyAffectedCell in recentlyAffectedCells)
		{
			Color color = new Color(recentAffectedCellColor[recentlyAffectedCell].r, recentAffectedCellColor[recentlyAffectedCell].g, recentAffectedCellColor[recentlyAffectedCell].b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 5f), -1f, 1f, 0.1f, 0.2f));
			colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, color));
		}
		foreach (int item in cellsInRadius)
		{
			if (recentlyAffectedCells.Contains(item))
			{
				Color color2 = radiusIndicatorColor;
				Color color3 = recentAffectedCellColor[item];
				color3.a = 0.2f;
				Color color4 = new Color((color2.r + color3.r) / 2f, (color2.g + color3.g) / 2f, (color2.b + color3.b) / 2f, color2.a + (1f - color2.a) * color3.a);
				colors.Add(new ToolMenu.CellColorData(item, color4));
			}
			else
			{
				colors.Add(new ToolMenu.CellColorData(item, radiusIndicatorColor));
			}
		}
	}

	public override void SetBrushSize(int radius)
	{
		brushRadius = radius;
		brushOffsets.Clear();
		for (int i = 0; i < brushRadius * 2; i++)
		{
			for (int j = 0; j < brushRadius * 2; j++)
			{
				if (Vector2.Distance(new Vector2(i, j), new Vector2(brushRadius, brushRadius)) < (float)brushRadius - 0.8f)
				{
					Vector2 vector = Grid.CellToXY(Grid.OffsetCell(currentCell, i, j));
					float num = PerlinSimplexNoise.noise(vector.x / settings.GetFloatSetting("SandboxTools.NoiseDensity"), vector.y / settings.GetFloatSetting("SandboxTools.NoiseDensity"), Time.realtimeSinceStartup);
					if (settings.GetFloatSetting("SandboxTools.NoiseScale") <= num)
					{
						brushOffsets.Add(new Vector2(i - brushRadius, j - brushRadius));
					}
				}
			}
		}
	}

	private void Update()
	{
		OnMouseMove(Grid.CellToPos(currentCell));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		SetBrushSize(settings.GetIntSetting("SandboxTools.BrushSize"));
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		recentlyAffectedCells.Add(cell);
		Element element = ElementLoader.elements[settings.GetIntSetting("SandboxTools.SelectedElement")];
		if (!recentAffectedCellColor.ContainsKey(cell))
		{
			recentAffectedCellColor.Add(cell, element.substance.uiColour);
		}
		else
		{
			recentAffectedCellColor[cell] = element.substance.uiColour;
		}
		Game.CallbackInfo item = new Game.CallbackInfo(delegate
		{
			recentlyAffectedCells.Remove(cell);
			recentAffectedCellColor.Remove(cell);
		});
		int index = Game.Instance.callbackManager.Add(item).index;
		byte index2 = Db.Get().Diseases.GetIndex(Db.Get().Diseases.Get("FoodPoisoning").id);
		Disease disease = Db.Get().Diseases.TryGet(settings.GetStringSetting("SandboxTools.SelectedDisease"));
		if (disease != null)
		{
			index2 = Db.Get().Diseases.GetIndex(disease.id);
		}
		int gameCell = cell;
		SimHashes id = element.id;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		float floatSetting = settings.GetFloatSetting("SandboxTools.Mass");
		float floatSetting2 = settings.GetFloatSetting("SandbosTools.Temperature");
		int callbackIdx = index;
		SimMessages.ReplaceElement(gameCell, id, sandBoxTool, floatSetting, floatSetting2, index2, settings.GetIntSetting("SandboxTools.DiseaseCount"), callbackIdx);
		SetBrushSize(brushRadius);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.SandboxCopyElement))
		{
			int cell = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			if (Grid.IsValidCell(cell))
			{
				SandboxSampleTool.Sample(cell);
			}
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}
}
