using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/SimDebugView")]
public class SimDebugView : KMonoBehaviour
{
	public static class OverlayModes
	{
		public static readonly HashedString Mass = "Mass";

		public static readonly HashedString Pressure = "Pressure";

		public static readonly HashedString GameGrid = "GameGrid";

		public static readonly HashedString ScenePartitioner = "ScenePartitioner";

		public static readonly HashedString ConduitUpdates = "ConduitUpdates";

		public static readonly HashedString Flow = "Flow";

		public static readonly HashedString StateChange = "StateChange";

		public static readonly HashedString SimCheckErrorMap = "SimCheckErrorMap";

		public static readonly HashedString DupePassable = "DupePassable";

		public static readonly HashedString Foundation = "Foundation";

		public static readonly HashedString FakeFloor = "FakeFloor";

		public static readonly HashedString CritterImpassable = "CritterImpassable";

		public static readonly HashedString DupeImpassable = "DupeImpassable";

		public static readonly HashedString MinionGroupProber = "MinionGroupProber";

		public static readonly HashedString PathProber = "PathProber";

		public static readonly HashedString Reserved = "Reserved";

		public static readonly HashedString AllowPathFinding = "AllowPathFinding";

		public static readonly HashedString Danger = "Danger";

		public static readonly HashedString MinionOccupied = "MinionOccupied";

		public static readonly HashedString TileType = "TileType";

		public static readonly HashedString State = "State";

		public static readonly HashedString SolidLiquid = "SolidLiquid";

		public static readonly HashedString Joules = "Joules";
	}

	public enum GameGridMode
	{
		GameSolidMap,
		Lighting,
		RoomMap,
		Style,
		PlantDensity,
		DigAmount,
		DupePassable
	}

	[Serializable]
	public struct ColorThreshold
	{
		public string colorName;

		public float value;
	}

	private struct UpdateSimViewSharedData
	{
		public SimDebugView instance;

		public HashedString simViewMode;

		public SimDebugView simDebugView;

		public byte[] textureBytes;

		public UpdateSimViewSharedData(SimDebugView instance, byte[] texture_bytes, HashedString sim_view_mode, SimDebugView sim_debug_view)
		{
			this.instance = instance;
			textureBytes = texture_bytes;
			simViewMode = sim_view_mode;
			simDebugView = sim_debug_view;
		}
	}

	private struct UpdateSimViewWorkItem : IWorkItem<UpdateSimViewSharedData>
	{
		private int x0;

		private int y0;

		private int x1;

		private int y1;

		public UpdateSimViewWorkItem(int x0, int y0, int x1, int y1)
		{
			this.x0 = Mathf.Clamp(x0, 0, Grid.WidthInCells - 1);
			this.x1 = Mathf.Clamp(x1, 0, Grid.WidthInCells - 1);
			this.y0 = Mathf.Clamp(y0, 0, Grid.HeightInCells - 1);
			this.y1 = Mathf.Clamp(y1, 0, Grid.HeightInCells - 1);
		}

		public void Run(UpdateSimViewSharedData shared_data)
		{
			if (!shared_data.instance.getColourFuncs.TryGetValue(shared_data.simViewMode, out var value))
			{
				value = GetBlack;
			}
			for (int i = y0; i <= y1; i++)
			{
				int num = Grid.XYToCell(x0, i);
				int num2 = Grid.XYToCell(x1, i);
				for (int j = num; j <= num2; j++)
				{
					int num3 = j * 4;
					if (Grid.IsActiveWorld(j))
					{
						Color color = value(shared_data.instance, j);
						shared_data.textureBytes[num3] = (byte)(Mathf.Min(color.r, 1f) * 255f);
						shared_data.textureBytes[num3 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
						shared_data.textureBytes[num3 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
						shared_data.textureBytes[num3 + 3] = (byte)(Mathf.Min(color.a, 1f) * 255f);
					}
					else
					{
						shared_data.textureBytes[num3] = 0;
						shared_data.textureBytes[num3 + 1] = 0;
						shared_data.textureBytes[num3 + 2] = 0;
						shared_data.textureBytes[num3 + 3] = 0;
					}
				}
			}
		}
	}

	public enum DangerAmount
	{
		None = 0,
		VeryLow = 1,
		Low = 2,
		Moderate = 3,
		High = 4,
		VeryHigh = 5,
		Extreme = 6,
		MAX_DANGERAMOUNT = 6
	}

	[SerializeField]
	public Material material;

	public Material diseaseMaterial;

	public bool hideFOW;

	public const int colourSize = 4;

	private byte[] texBytes;

	private int currentFrame;

	[SerializeField]
	private Texture2D tex;

	[SerializeField]
	private GameObject plane;

	private HashedString mode = global::OverlayModes.Power.ID;

	private GameGridMode gameGridMode = GameGridMode.DigAmount;

	private PathProber selectedPathProber;

	public float minTempExpected = 173.15f;

	public float maxTempExpected = 423.15f;

	public float minMassExpected = 1.0001f;

	public float maxMassExpected = 10000f;

	public float minPressureExpected = 1.300003f;

	public float maxPressureExpected = 201.3f;

	public float minThermalConductivity;

	public float maxThermalConductivity = 30f;

	public float thresholdRange = 0.001f;

	public float thresholdOpacity = 0.8f;

	public static float minimumBreathable = 0.05f;

	public static float optimallyBreathable = 1f;

	public ColorThreshold[] temperatureThresholds;

	public ColorThreshold[] heatFlowThresholds;

	public Color32[] networkColours;

	public Gradient breathableGradient = new Gradient();

	public Color32 unbreathableColour = new Color(0.5f, 0f, 0f);

	public Color32[] toxicColour = new Color32[2]
	{
		new Color(0.5f, 0f, 0.5f),
		new Color(1f, 0f, 1f)
	};

	public static SimDebugView Instance;

	private WorkItemCollection<UpdateSimViewWorkItem, UpdateSimViewSharedData> updateSimViewWorkItems = new WorkItemCollection<UpdateSimViewWorkItem, UpdateSimViewSharedData>();

	private int selectedCell;

	private Dictionary<HashedString, Action<SimDebugView, Texture>> dataUpdateFuncs = new Dictionary<HashedString, Action<SimDebugView, Texture>>
	{
		{
			global::OverlayModes.Temperature.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.Oxygen.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.Decor.ID,
			SetDefaultBilinear
		},
		{
			global::OverlayModes.TileMode.ID,
			SetDefaultPoint
		},
		{
			global::OverlayModes.Disease.ID,
			SetDisease
		}
	};

	private Dictionary<HashedString, Func<SimDebugView, int, Color>> getColourFuncs = new Dictionary<HashedString, Func<SimDebugView, int, Color>>
	{
		{
			global::OverlayModes.ThermalConductivity.ID,
			GetThermalConductivityColour
		},
		{
			global::OverlayModes.Temperature.ID,
			GetNormalizedTemperatureColourMode
		},
		{
			global::OverlayModes.Disease.ID,
			GetDiseaseColour
		},
		{
			global::OverlayModes.Decor.ID,
			GetDecorColour
		},
		{
			global::OverlayModes.Oxygen.ID,
			GetOxygenMapColour
		},
		{
			global::OverlayModes.Light.ID,
			GetLightColour
		},
		{
			global::OverlayModes.Radiation.ID,
			GetRadiationColour
		},
		{
			global::OverlayModes.Rooms.ID,
			GetRoomsColour
		},
		{
			global::OverlayModes.TileMode.ID,
			GetTileColour
		},
		{
			global::OverlayModes.Suit.ID,
			GetBlack
		},
		{
			global::OverlayModes.Priorities.ID,
			GetBlack
		},
		{
			global::OverlayModes.Crop.ID,
			GetBlack
		},
		{
			global::OverlayModes.Harvest.ID,
			GetBlack
		},
		{
			OverlayModes.GameGrid,
			GetGameGridColour
		},
		{
			OverlayModes.StateChange,
			GetStateChangeColour
		},
		{
			OverlayModes.SimCheckErrorMap,
			GetSimCheckErrorMapColour
		},
		{
			OverlayModes.Foundation,
			GetFoundationColour
		},
		{
			OverlayModes.FakeFloor,
			GetFakeFloorColour
		},
		{
			OverlayModes.DupePassable,
			GetDupePassableColour
		},
		{
			OverlayModes.DupeImpassable,
			GetDupeImpassableColour
		},
		{
			OverlayModes.CritterImpassable,
			GetCritterImpassableColour
		},
		{
			OverlayModes.MinionGroupProber,
			GetMinionGroupProberColour
		},
		{
			OverlayModes.PathProber,
			GetPathProberColour
		},
		{
			OverlayModes.Reserved,
			GetReservedColour
		},
		{
			OverlayModes.AllowPathFinding,
			GetAllowPathFindingColour
		},
		{
			OverlayModes.Danger,
			GetDangerColour
		},
		{
			OverlayModes.MinionOccupied,
			GetMinionOccupiedColour
		},
		{
			OverlayModes.Pressure,
			GetPressureMapColour
		},
		{
			OverlayModes.TileType,
			GetTileTypeColour
		},
		{
			OverlayModes.State,
			GetStateMapColour
		},
		{
			OverlayModes.SolidLiquid,
			GetSolidLiquidMapColour
		},
		{
			OverlayModes.Mass,
			GetMassColour
		},
		{
			OverlayModes.Joules,
			GetJoulesColour
		}
	};

	public static readonly Color[] dbColours = new Color[13]
	{
		new Color(0f, 0f, 0f, 0f),
		new Color(1f, 1f, 1f, 0.3f),
		new Color(0.7058824f, 0.8235294f, 1f, 0.2f),
		new Color(0f, 16f / 51f, 1f, 0.3f),
		new Color(0.7058824f, 1f, 0.7058824f, 0.5f),
		new Color(4f / 51f, 1f, 0f, 0.7f),
		new Color(1f, 46f / 51f, 0.7058824f, 0.9f),
		new Color(1f, 0.8235294f, 0f, 0.9f),
		new Color(1f, 61f / 85f, 77f / 255f, 0.9f),
		new Color(1f, 106f / 255f, 0f, 0.9f),
		new Color(1f, 0.7058824f, 0.7058824f, 1f),
		new Color(1f, 0f, 0f, 1f),
		new Color(1f, 0f, 0f, 1f)
	};

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		material = UnityEngine.Object.Instantiate(material);
		diseaseMaterial = UnityEngine.Object.Instantiate(diseaseMaterial);
	}

	protected override void OnSpawn()
	{
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[0].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[1].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[2].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color3", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[3].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color4", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[4].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color5", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[5].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color6", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[6].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color7", GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[7].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", GlobalAssets.Instance.colorSet.GetColorByName(heatFlowThresholds[0].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", GlobalAssets.Instance.colorSet.GetColorByName(heatFlowThresholds[1].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", GlobalAssets.Instance.colorSet.GetColorByName(heatFlowThresholds[2].colorName));
		SetMode(global::OverlayModes.None.ID);
	}

	public void OnReset()
	{
		plane = CreatePlane("SimDebugView", base.transform);
		tex = CreateTexture(out texBytes, Grid.WidthInCells, Grid.HeightInCells);
		plane.GetComponent<Renderer>().sharedMaterial = material;
		plane.GetComponent<Renderer>().sharedMaterial.mainTexture = tex;
		plane.transform.SetLocalPosition(new Vector3(0f, 0f, -6f));
		SetMode(global::OverlayModes.None.ID);
	}

	public static Texture2D CreateTexture(int width, int height)
	{
		return new Texture2D(width, height)
		{
			name = "SimDebugView",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	public static Texture2D CreateTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		return new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None)
		{
			name = "SimDebugView",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	public static GameObject CreatePlane(string layer, Transform parent)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "overlayViewDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		gameObject.transform.SetParent(parent);
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.AddComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		Mesh mesh2 = (gameObject.AddComponent<MeshFilter>().mesh = new Mesh());
		Vector3[] array = new Vector3[4];
		Vector2[] array2 = new Vector2[4];
		int[] array3 = new int[6];
		float y = 2f * (float)Grid.HeightInCells;
		array = new Vector3[4]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(Grid.WidthInCells, 0f, 0f),
			new Vector3(0f, y, 0f),
			new Vector3(Grid.WidthInMeters, y, 0f)
		};
		array2 = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 2f),
			new Vector2(1f, 2f)
		};
		array3 = new int[6]
		{
			0,
			2,
			1,
			1,
			2,
			3
		};
		mesh2.vertices = array;
		mesh2.uv = array2;
		mesh2.triangles = array3;
		Vector2 vector = new Vector2(Grid.WidthInCells, y);
		mesh2.bounds = new Bounds(new Vector3(0.5f * vector.x, 0.5f * vector.y, 0f), new Vector3(vector.x, vector.y, 0f));
		return gameObject;
	}

	private void Update()
	{
		if (!(plane == null))
		{
			bool flag = mode != global::OverlayModes.None.ID;
			plane.SetActive(flag);
			SimDebugViewCompositor.Instance.Toggle(mode != global::OverlayModes.None.ID && !GameUtil.IsCapturingTimeLapse());
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
			float x = 0f;
			if (mode == global::OverlayModes.ThermalConductivity.ID || mode == global::OverlayModes.Temperature.ID)
			{
				x = 1f;
			}
			SimDebugViewCompositor.Instance.material.SetVector("_ThresholdParameters", new Vector4(x, thresholdRange, thresholdOpacity, 0f));
			if (flag)
			{
				UpdateData(tex, texBytes, mode, 192);
			}
		}
	}

	private static void SetDefaultBilinear(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	private static void SetDefaultPoint(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Point;
	}

	private static void SetDisease(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.diseaseMaterial;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	public void UpdateData(Texture2D texture, byte[] textureBytes, HashedString viewMode, byte alpha)
	{
		if (!dataUpdateFuncs.TryGetValue(viewMode, out var value))
		{
			value = SetDefaultPoint;
		}
		value(this, texture);
		Grid.GetVisibleExtents(out var min_x, out var min_y, out var max_x, out var max_y);
		selectedPathProber = null;
		KSelectable selected = SelectTool.Instance.selected;
		if (selected != null)
		{
			selectedPathProber = selected.GetComponent<PathProber>();
		}
		updateSimViewWorkItems.Reset(new UpdateSimViewSharedData(this, texBytes, viewMode, this));
		int num = 16;
		for (int i = min_y; i <= max_y; i += num)
		{
			int y = Math.Min(i + num - 1, max_y);
			updateSimViewWorkItems.Add(new UpdateSimViewWorkItem(min_x, i, max_x, y));
		}
		currentFrame = Time.frameCount;
		selectedCell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		GlobalJobManager.Run(updateSimViewWorkItems);
		texture.LoadRawTextureData(textureBytes);
		texture.Apply();
	}

	public void SetGameGridMode(GameGridMode mode)
	{
		gameGridMode = mode;
	}

	public GameGridMode GetGameGridMode()
	{
		return gameGridMode;
	}

	public void SetMode(HashedString mode)
	{
		this.mode = mode;
		Game.Instance.gameObject.Trigger(1798162660, mode);
	}

	public HashedString GetMode()
	{
		return mode;
	}

	public static Color TemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, 1f, 1f);
	}

	public static Color LiquidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float value = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num = Mathf.Clamp(value, 0.5f, 1f);
		float s = Mathf.Clamp(value, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public static Color SolidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0.5f, 1f);
		float s = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public static Color GasTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0f, 0.5f);
		float s = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, s, 1f);
	}

	public Color NormalizedTemperature(float temperature)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < temperatureThresholds.Length; i++)
		{
			if (temperature <= temperatureThresholds[i].value)
			{
				num2 = i;
				break;
			}
			num = i;
			num2 = i;
		}
		float a = 0f;
		if (num != num2)
		{
			a = (temperature - temperatureThresholds[num].value) / (temperatureThresholds[num2].value - temperatureThresholds[num].value);
		}
		a = Mathf.Max(a, 0f);
		a = Mathf.Min(a, 1f);
		return Color.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[num].colorName), GlobalAssets.Instance.colorSet.GetColorByName(temperatureThresholds[num2].colorName), a);
	}

	public Color NormalizedHeatFlow(int cell)
	{
		int num = 0;
		int num2 = 0;
		float thermalComfort = GameUtil.GetThermalComfort(cell);
		for (int i = 0; i < heatFlowThresholds.Length; i++)
		{
			if (thermalComfort <= heatFlowThresholds[i].value)
			{
				num2 = i;
				break;
			}
			num = i;
			num2 = i;
		}
		float a = 0f;
		if (num != num2)
		{
			a = (thermalComfort - heatFlowThresholds[num].value) / (heatFlowThresholds[num2].value - heatFlowThresholds[num].value);
		}
		a = Mathf.Max(a, 0f);
		a = Mathf.Min(a, 1f);
		Color result = Color.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(heatFlowThresholds[num].colorName), GlobalAssets.Instance.colorSet.GetColorByName(heatFlowThresholds[num2].colorName), a);
		if (Grid.Solid[cell])
		{
			result = Color.black;
		}
		return result;
	}

	private static bool IsInsulated(int cell)
	{
		return (Grid.Element[cell].state & Element.State.TemperatureInsulated) != 0;
	}

	private static Color GetDiseaseColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (Grid.DiseaseIdx[cell] != byte.MaxValue)
		{
			Disease disease = Db.Get().Diseases[Grid.DiseaseIdx[cell]];
			result = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
			result.a = SimUtil.DiseaseCountToAlpha(Grid.DiseaseCount[cell]);
		}
		else
		{
			result.a = 0f;
		}
		return result;
	}

	private static Color GetHeatFlowColour(SimDebugView instance, int cell)
	{
		return instance.NormalizedHeatFlow(cell);
	}

	private static Color GetBlack(SimDebugView instance, int cell)
	{
		return Color.black;
	}

	public static Color GetLightColour(SimDebugView instance, int cell)
	{
		Color result = GlobalAssets.Instance.colorSet.lightOverlay;
		result.a = Mathf.Clamp(Mathf.Sqrt(Grid.LightIntensity[cell] + LightGridManager.previewLux[cell]) / Mathf.Sqrt(80000f), 0f, 1f);
		if (Grid.LightIntensity[cell] > 72000)
		{
			float num = ((float)Grid.LightIntensity[cell] + (float)LightGridManager.previewLux[cell] - 72000f) / 8000f;
			num /= 10f;
			result.r += Mathf.Min(0.1f, PerlinSimplexNoise.noise(Grid.CellToPos2D(cell).x / 8f, Grid.CellToPos2D(cell).y / 8f + (float)instance.currentFrame / 32f) * num);
		}
		return result;
	}

	public static Color GetRadiationColour(SimDebugView instance, int cell)
	{
		float a = Mathf.Clamp(Mathf.Sqrt(Grid.Radiation[cell]) / 30f, 0f, 1f);
		return new Color(0.2f, 0.9f, 0.3f, a);
	}

	public static Color GetRoomsColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (Grid.IsValidCell(instance.selectedCell))
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			if (cavityForCell != null && cavityForCell.room != null)
			{
				Room room = cavityForCell.room;
				result = GlobalAssets.Instance.colorSet.GetColorByName(room.roomType.category.colorName);
				result.a = 0.45f;
				if (Game.Instance.roomProber.GetCavityForCell(instance.selectedCell) == cavityForCell)
				{
					result.a += 0.3f;
				}
			}
		}
		return result;
	}

	public static Color GetJoulesColour(SimDebugView instance, int cell)
	{
		float num = Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
		float t = 0.5f * num / (ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294f * 1000000f);
		return Color.Lerp(Color.black, Color.red, t);
	}

	public static Color GetNormalizedTemperatureColourMode(SimDebugView instance, int cell)
	{
		return Game.Instance.temperatureOverlayMode switch
		{
			Game.TemperatureOverlayModes.AbsoluteTemperature => GetNormalizedTemperatureColour(instance, cell), 
			Game.TemperatureOverlayModes.HeatFlow => GetHeatFlowColour(instance, cell), 
			Game.TemperatureOverlayModes.AdaptiveTemperature => GetNormalizedTemperatureColour(instance, cell), 
			Game.TemperatureOverlayModes.StateChange => GetStateChangeProximityColour(instance, cell), 
			_ => GetNormalizedTemperatureColour(instance, cell), 
		};
	}

	public static Color GetStateChangeProximityColour(SimDebugView instance, int cell)
	{
		float temperature = Grid.Temperature[cell];
		Element element = Grid.Element[cell];
		float lowTemp = element.lowTemp;
		float highTemp = element.highTemp;
		if (element.IsGas)
		{
			highTemp = Mathf.Min(lowTemp + 150f, highTemp);
			return GasTemperatureToColor(temperature, lowTemp, highTemp);
		}
		if (element.IsSolid)
		{
			lowTemp = Mathf.Max(highTemp - 150f, lowTemp);
			return SolidTemperatureToColor(temperature, lowTemp, highTemp);
		}
		return TemperatureToColor(temperature, lowTemp, highTemp);
	}

	public static Color GetNormalizedTemperatureColour(SimDebugView instance, int cell)
	{
		float temperature = Grid.Temperature[cell];
		return instance.NormalizedTemperature(temperature);
	}

	private static Color GetGameGridColour(SimDebugView instance, int cell)
	{
		Color result = new Color32(0, 0, 0, byte.MaxValue);
		switch (instance.gameGridMode)
		{
		case GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num = Grid.Damage[cell] / 255f;
				result = Color.HSVToRGB(1f - num, 1f, 1f);
			}
			break;
		case GameGridMode.GameSolidMap:
			result = (Grid.Solid[cell] ? Color.white : Color.black);
			break;
		case GameGridMode.Lighting:
			result = ((Grid.LightCount[cell] > 0 || LightGridManager.previewLux[cell] > 0) ? Color.white : Color.black);
			break;
		case GameGridMode.DupePassable:
			result = (Grid.DupePassable[cell] ? Color.white : Color.black);
			break;
		}
		return result;
	}

	public Color32 GetColourForID(int id)
	{
		return networkColours[id % networkColours.Length];
	}

	private static Color GetThermalConductivityColour(SimDebugView instance, int cell)
	{
		bool num = IsInsulated(cell);
		Color result = Color.black;
		float num2 = instance.maxThermalConductivity - instance.minThermalConductivity;
		if (!num && num2 != 0f)
		{
			float a = (Grid.Element[cell].thermalConductivity - instance.minThermalConductivity) / num2;
			a = Mathf.Max(a, 0f);
			a = Mathf.Min(a, 1f);
			result = new Color(a, a, a);
		}
		return result;
	}

	private static Color GetPressureMapColour(SimDebugView instance, int cell)
	{
		Color32 c = Color.black;
		if (Grid.Pressure[cell] > 0f)
		{
			float num = Mathf.Clamp((Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - instance.minPressureExpected), 0f, 1f) * 0.9f;
			c = new Color(num, num, num, 1f);
		}
		return c;
	}

	private static Color GetOxygenMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
		{
			if (Grid.Mass[cell] > minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
			{
				float time = Mathf.Clamp((Grid.Mass[cell] - minimumBreathable) / optimallyBreathable, 0f, 1f);
				result = instance.breathableGradient.Evaluate(time);
			}
			else
			{
				result = instance.unbreathableColour;
			}
		}
		return result;
	}

	private static Color GetTileColour(SimDebugView instance, int cell)
	{
		float num = 0.33f;
		Color result = new Color(num, num, num);
		Element element = Grid.Element[cell];
		bool flag = false;
		foreach (Tag tileOverlayFilter in Game.Instance.tileOverlayFilters)
		{
			if (element.HasTag(tileOverlayFilter))
			{
				flag = true;
			}
		}
		if (flag)
		{
			return element.substance.uiColour;
		}
		return result;
	}

	private static Color GetTileTypeColour(SimDebugView instance, int cell)
	{
		return Grid.Element[cell].substance.uiColour;
	}

	private static Color GetStateMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Solid:
			result = Color.blue;
			break;
		case Element.State.Liquid:
			result = Color.green;
			break;
		case Element.State.Gas:
			result = Color.yellow;
			break;
		}
		return result;
	}

	private static Color GetSolidLiquidMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Solid:
			result = Color.blue;
			break;
		case Element.State.Liquid:
			result = Color.green;
			break;
		}
		return result;
	}

	private static Color GetStateChangeColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		Element element = Grid.Element[cell];
		if (!element.IsVacuum)
		{
			float num = Grid.Temperature[cell];
			float num2 = element.lowTemp * 0.05f;
			float a = Mathf.Abs(num - element.lowTemp) / num2;
			float num3 = element.highTemp * 0.05f;
			float b = Mathf.Abs(num - element.highTemp) / num3;
			float t = Mathf.Max(0f, 1f - Mathf.Min(a, b));
			result = Color.Lerp(Color.black, Color.red, t);
		}
		return result;
	}

	private static Color GetDecorColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!Grid.Solid[cell])
		{
			float num = GameUtil.GetDecorAtCell(cell) / 100f;
			result = ((!(num > 0f)) ? Color.Lerp(GlobalAssets.Instance.colorSet.decorBaseline, GlobalAssets.Instance.colorSet.decorNegative, Mathf.Abs(num)) : Color.Lerp(GlobalAssets.Instance.colorSet.decorBaseline, GlobalAssets.Instance.colorSet.decorPositive, Mathf.Abs(num)));
		}
		return result;
	}

	private static Color GetDangerColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		DangerAmount dangerAmount = DangerAmount.None;
		if (!Grid.Element[cell].IsSolid)
		{
			float num = 0f;
			if (Grid.Temperature[cell] < minMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - minMinionTemperature);
			}
			if (Grid.Temperature[cell] > maxMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - maxMinionTemperature);
			}
			if (num > 0f)
			{
				if (num < 10f)
				{
					dangerAmount = DangerAmount.VeryLow;
				}
				else if (num < 30f)
				{
					dangerAmount = DangerAmount.Low;
				}
				else if (num < 100f)
				{
					dangerAmount = DangerAmount.Moderate;
				}
				else if (num < 200f)
				{
					dangerAmount = DangerAmount.High;
				}
				else if (num < 400f)
				{
					dangerAmount = DangerAmount.VeryHigh;
				}
				else if (num > 800f)
				{
					dangerAmount = DangerAmount.Extreme;
				}
			}
		}
		if (dangerAmount < DangerAmount.VeryHigh && (Grid.Element[cell].IsVacuum || (Grid.Element[cell].IsGas && (Grid.Element[cell].id != SimHashes.Oxygen || Grid.Pressure[cell] < minMinionPressure))))
		{
			dangerAmount++;
		}
		if (dangerAmount != 0)
		{
			float num2 = (float)dangerAmount / 6f;
			result = Color.HSVToRGB((80f - num2 * 80f) / 360f, 1f, 1f);
		}
		return result;
	}

	private static Color GetSimCheckErrorMapColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		Element element = Grid.Element[cell];
		float num = Grid.Mass[cell];
		float num2 = Grid.Temperature[cell];
		if (float.IsNaN(num) || float.IsNaN(num2) || num > 10000f || num2 > 10000f)
		{
			return Color.red;
		}
		if (element.IsVacuum)
		{
			result = ((num2 != 0f) ? Color.yellow : ((num == 0f) ? Color.gray : Color.blue));
		}
		else if (num2 < 10f)
		{
			result = Color.red;
		}
		else if (Grid.Mass[cell] < 1f && Grid.Pressure[cell] < 1f)
		{
			result = Color.green;
		}
		else if (num2 > element.highTemp + 3f && element.highTempTransition != null)
		{
			result = Color.magenta;
		}
		else if (num2 < element.lowTemp + 3f && element.lowTempTransition != null)
		{
			result = Color.cyan;
		}
		return result;
	}

	private static Color GetFakeFloorColour(SimDebugView instance, int cell)
	{
		if (!Grid.FakeFloor[cell])
		{
			return Color.black;
		}
		return Color.cyan;
	}

	private static Color GetFoundationColour(SimDebugView instance, int cell)
	{
		if (!Grid.Foundation[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetDupePassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.DupePassable[cell])
		{
			return Color.black;
		}
		return Color.green;
	}

	private static Color GetCritterImpassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.CritterImpassable[cell])
		{
			return Color.black;
		}
		return Color.yellow;
	}

	private static Color GetDupeImpassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.DupeImpassable[cell])
		{
			return Color.black;
		}
		return Color.red;
	}

	private static Color GetMinionOccupiedColour(SimDebugView instance, int cell)
	{
		if (!(Grid.Objects[cell, 0] != null))
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetMinionGroupProberColour(SimDebugView instance, int cell)
	{
		if (!MinionGroupProber.Get().IsReachable(cell))
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetPathProberColour(SimDebugView instance, int cell)
	{
		if (!(instance.selectedPathProber != null) || instance.selectedPathProber.GetCost(cell) == -1)
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetReservedColour(SimDebugView instance, int cell)
	{
		if (!Grid.Reserved[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetAllowPathFindingColour(SimDebugView instance, int cell)
	{
		if (!Grid.AllowPathfinding[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetMassColour(SimDebugView instance, int cell)
	{
		Color result = Color.black;
		if (!IsInsulated(cell))
		{
			float num = Grid.Mass[cell];
			if (num > 0f)
			{
				float num2 = (num - Instance.minMassExpected) / (Instance.maxMassExpected - Instance.minMassExpected);
				result = Color.HSVToRGB(1f - num2, 1f, 1f);
			}
		}
		return result;
	}
}
