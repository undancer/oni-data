using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/PropertyTextures")]
public class PropertyTextures : KMonoBehaviour, ISim200ms
{
	public enum Property
	{
		StateChange,
		GasPressure,
		GasColour,
		GasDanger,
		FogOfWar,
		Flow,
		SolidDigAmount,
		SolidLiquidGasMass,
		WorldLight,
		Liquid,
		Temperature,
		ExposedToSunlight,
		FallingSolid,
		Radiation,
		Num
	}

	private struct TextureProperties
	{
		public string name;

		public Property simProperty;

		public TextureFormat textureFormat;

		public FilterMode filterMode;

		public bool updateEveryFrame;

		public bool updatedExternally;

		public bool blend;

		public float blendSpeed;

		public string texturePropertyName;
	}

	private struct WorkItem : IWorkItem<object>
	{
		public delegate void Callback(TextureRegion texture_region, int x0, int y0, int x1, int y1);

		private int x0;

		private int y0;

		private int x1;

		private int y1;

		private TextureRegion textureRegion;

		private Callback updateTextureCb;

		public WorkItem(TextureRegion texture_region, int x0, int y0, int x1, int y1, Callback update_texture_cb)
		{
			textureRegion = texture_region;
			this.x0 = x0;
			this.y0 = y0;
			this.x1 = x1;
			this.y1 = y1;
			updateTextureCb = update_texture_cb;
		}

		public void Run(object shared_data)
		{
			updateTextureCb(textureRegion, x0, y0, x1, y1);
		}
	}

	[NonSerialized]
	public bool ForceLightEverywhere;

	[SerializeField]
	private Vector2 PressureRange = new Vector2(15f, 200f);

	[SerializeField]
	private float MinPressureVisibility = 0.1f;

	[SerializeField]
	[Range(0f, 1f)]
	private float TemperatureStateChangeRange = 0.05f;

	public static PropertyTextures instance;

	public static IntPtr externalFlowTex;

	public static IntPtr externalLiquidTex;

	public static IntPtr externalExposedToSunlight;

	public static IntPtr externalSolidDigAmountTex;

	[SerializeField]
	private Vector2 coldRange;

	[SerializeField]
	private Vector2 hotRange;

	public static float FogOfWarScale;

	private int WorldSizeID;

	private int ClusterWorldSizeID;

	private int FogOfWarScaleID;

	private int PropTexWsToCsID;

	private int PropTexCsToWsID;

	private int TopBorderHeightID;

	private int NextPropertyIdx;

	public TextureBuffer[] textureBuffers;

	public TextureLerper[] lerpers;

	private TexturePagePool texturePagePool;

	[SerializeField]
	private Texture2D[] externallyUpdatedTextures;

	private TextureProperties[] textureProperties;

	private List<TextureProperties> allTextureProperties;

	private WorkItemCollection<WorkItem, object> workItems;

	public static bool IsFogOfWarEnabled => FogOfWarScale < 1f;

	public static void DestroyInstance()
	{
		ShaderReloader.Unregister(instance.OnShadersReloaded);
		externalFlowTex = IntPtr.Zero;
		externalLiquidTex = IntPtr.Zero;
		externalExposedToSunlight = IntPtr.Zero;
		externalSolidDigAmountTex = IntPtr.Zero;
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		base.OnPrefabInit();
		ShaderReloader.Register(OnShadersReloaded);
	}

	public void SetFilterMode(Property property, FilterMode mode)
	{
		textureProperties[(int)property].filterMode = mode;
	}

	public Texture GetTexture(Property property)
	{
		return textureBuffers[(int)property].texture;
	}

	private string GetShaderPropertyName(Property property)
	{
		return "_" + property.ToString() + "Tex";
	}

	protected override void OnSpawn()
	{
		if (GenericGameSettings.instance.disableFogOfWar)
		{
			FogOfWarScale = 1f;
		}
		WorldSizeID = Shader.PropertyToID("_WorldSizeInfo");
		ClusterWorldSizeID = Shader.PropertyToID("_ClusterWorldSizeInfo");
		FogOfWarScaleID = Shader.PropertyToID("_FogOfWarScale");
		PropTexWsToCsID = Shader.PropertyToID("_PropTexWsToCs");
		PropTexCsToWsID = Shader.PropertyToID("_PropTexCsToWs");
		TopBorderHeightID = Shader.PropertyToID("_TopBorderHeight");
	}

	public void OnReset(object data = null)
	{
		lerpers = new TextureLerper[14];
		texturePagePool = new TexturePagePool();
		textureBuffers = new TextureBuffer[14];
		externallyUpdatedTextures = new Texture2D[14];
		for (int i = 0; i < 14; i++)
		{
			TextureProperties textureProperties = default(TextureProperties);
			textureProperties.textureFormat = TextureFormat.Alpha8;
			textureProperties.filterMode = FilterMode.Bilinear;
			textureProperties.blend = false;
			textureProperties.blendSpeed = 1f;
			TextureProperties item = textureProperties;
			for (int j = 0; j < this.textureProperties.Length; j++)
			{
				if (i == (int)this.textureProperties[j].simProperty)
				{
					item = this.textureProperties[j];
				}
			}
			Property property = (Property)i;
			item.name = property.ToString();
			if (externallyUpdatedTextures[i] != null)
			{
				UnityEngine.Object.Destroy(externallyUpdatedTextures[i]);
				externallyUpdatedTextures[i] = null;
			}
			Texture texture;
			if (item.updatedExternally)
			{
				externallyUpdatedTextures[i] = new Texture2D(Grid.WidthInCells, Grid.HeightInCells, TextureUtil.TextureFormatToGraphicsFormat(item.textureFormat), TextureCreationFlags.None);
				texture = externallyUpdatedTextures[i];
			}
			else
			{
				TextureBuffer[] array = textureBuffers;
				int num = i;
				property = (Property)i;
				array[num] = new TextureBuffer(property.ToString(), Grid.WidthInCells, Grid.HeightInCells, item.textureFormat, item.filterMode, texturePagePool);
				texture = textureBuffers[i].texture;
			}
			if (item.blend)
			{
				TextureLerper[] array2 = lerpers;
				int num2 = i;
				Texture target_texture = texture;
				property = (Property)i;
				array2[num2] = new TextureLerper(target_texture, property.ToString(), texture.filterMode, item.textureFormat);
				lerpers[i].Speed = item.blendSpeed;
			}
			Shader.SetGlobalTexture(item.texturePropertyName = (texture.name = GetShaderPropertyName((Property)i)), texture);
			allTextureProperties.Add(item);
		}
	}

	private void OnShadersReloaded()
	{
		for (int i = 0; i < 14; i++)
		{
			TextureLerper textureLerper = lerpers[i];
			if (textureLerper != null)
			{
				Shader.SetGlobalTexture(allTextureProperties[i].texturePropertyName, textureLerper.Update());
			}
		}
	}

	public void Sim200ms(float dt)
	{
		if (lerpers != null && lerpers.Length != 0)
		{
			for (int i = 0; i < lerpers.Length; i++)
			{
				lerpers[i]?.LongUpdate(dt);
			}
		}
	}

	private void UpdateTextureThreaded(TextureRegion texture_region, int x0, int y0, int x1, int y1, WorkItem.Callback update_texture_cb)
	{
		workItems.Reset(null);
		int num = 16;
		for (int i = y0; i <= y1; i += num)
		{
			int y2 = Math.Min(i + num - 1, y1);
			workItems.Add(new WorkItem(texture_region, x0, i, x1, y2, update_texture_cb));
		}
		GlobalJobManager.Run(workItems);
	}

	private void UpdateProperty(ref TextureProperties p, int x0, int y0, int x1, int y1)
	{
		if (Game.Instance.IsLoading())
		{
			return;
		}
		int simProperty = (int)p.simProperty;
		if (!p.updatedExternally)
		{
			TextureRegion texture_region = textureBuffers[simProperty].Lock(x0, y0, x1 - x0 + 1, y1 - y0 + 1);
			switch (p.simProperty)
			{
			case Property.StateChange:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateStateChange);
				break;
			case Property.GasPressure:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdatePressure);
				break;
			case Property.GasColour:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateGasColour);
				break;
			case Property.GasDanger:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateDanger);
				break;
			case Property.FogOfWar:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateFogOfWar);
				break;
			case Property.SolidDigAmount:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateSolidDigAmount);
				break;
			case Property.SolidLiquidGasMass:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateSolidLiquidGasMass);
				break;
			case Property.WorldLight:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateWorldLight);
				break;
			case Property.Temperature:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateTemperature);
				break;
			case Property.FallingSolid:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateFallingSolidChange);
				break;
			case Property.Radiation:
				UpdateTextureThreaded(texture_region, x0, y0, x1, y1, UpdateRadiation);
				break;
			}
			texture_region.Unlock();
		}
		else
		{
			switch (p.simProperty)
			{
			case Property.Flow:
				externallyUpdatedTextures[simProperty].LoadRawTextureData(externalFlowTex, 8 * Grid.WidthInCells * Grid.HeightInCells);
				break;
			case Property.Liquid:
				externallyUpdatedTextures[simProperty].LoadRawTextureData(externalLiquidTex, 4 * Grid.WidthInCells * Grid.HeightInCells);
				break;
			case Property.ExposedToSunlight:
				externallyUpdatedTextures[simProperty].LoadRawTextureData(externalExposedToSunlight, Grid.WidthInCells * Grid.HeightInCells);
				break;
			}
			externallyUpdatedTextures[simProperty].Apply();
		}
	}

	private void LateUpdate()
	{
		if (!Grid.IsInitialized())
		{
			return;
		}
		Shader.SetGlobalVector(WorldSizeID, new Vector4(Grid.WidthInCells, Grid.HeightInCells, 1f / (float)Grid.WidthInCells, 1f / (float)Grid.HeightInCells));
		WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
		Vector2I worldOffset = activeWorld.WorldOffset;
		Vector2I worldSize = activeWorld.WorldSize;
		if (DlcManager.IsPureVanilla() || (CameraController.Instance != null && CameraController.Instance.ignoreClusterFX))
		{
			Shader.SetGlobalVector(ClusterWorldSizeID, new Vector4(Grid.WidthInCells, Grid.HeightInCells, 0f, 0f));
		}
		else
		{
			Shader.SetGlobalVector(ClusterWorldSizeID, new Vector4(worldSize.x, worldSize.y, 1f / (float)(worldSize.x + worldOffset.x), 1f / (float)(worldSize.y + worldOffset.y)));
		}
		Shader.SetGlobalVector(PropTexWsToCsID, new Vector4(0f, 0f, 1f, 1f));
		Shader.SetGlobalVector(PropTexCsToWsID, new Vector4(0f, 0f, 1f, 1f));
		Shader.SetGlobalFloat(TopBorderHeightID, ClusterManager.Instance.activeWorld.FullyEnclosedBorder ? 0f : ((float)Grid.TopBorderHeight));
		GetVisibleCellRange(out var x, out var y, out var x2, out var y2);
		Shader.SetGlobalFloat(FogOfWarScaleID, FogOfWarScale);
		int num = NextPropertyIdx++ % allTextureProperties.Count;
		TextureProperties textureProperties = allTextureProperties[num];
		while (textureProperties.updateEveryFrame)
		{
			num = NextPropertyIdx++ % allTextureProperties.Count;
			textureProperties = allTextureProperties[num];
		}
		for (int i = 0; i < allTextureProperties.Count; i++)
		{
			TextureProperties p = allTextureProperties[i];
			if (num == i || p.updateEveryFrame || GameUtil.IsCapturingTimeLapse())
			{
				UpdateProperty(ref p, x, y, x2, y2);
			}
		}
		for (int j = 0; j < 14; j++)
		{
			TextureLerper textureLerper = lerpers[j];
			if (textureLerper != null)
			{
				if (Time.timeScale == 0f)
				{
					textureLerper.LongUpdate(Time.unscaledDeltaTime);
				}
				Shader.SetGlobalTexture(allTextureProperties[j].texturePropertyName, textureLerper.Update());
			}
		}
	}

	private void GetVisibleCellRange(out int x0, out int y0, out int x1, out int y1)
	{
		int num = 16;
		Grid.GetVisibleExtents(out x0, out y0, out x1, out y1);
		int widthInCells = Grid.WidthInCells;
		int heightInCells = Grid.HeightInCells;
		int num2 = 0;
		int num3 = 0;
		x0 = Math.Max(num2, x0 - num);
		y0 = Math.Max(num3, y0 - num);
		x0 = Mathf.Min(x0, widthInCells - 1);
		y0 = Mathf.Min(y0, heightInCells - 1);
		x1 = Mathf.CeilToInt(x1 + num);
		y1 = Mathf.CeilToInt(y1 + num);
		x1 = Mathf.Max(x1, num2);
		y1 = Mathf.Max(y1, num3);
		x1 = Mathf.Min(x1, widthInCells - 1);
		y1 = Mathf.Min(y1, heightInCells - 1);
	}

	private static void UpdateFogOfWar(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		byte[] visible = Grid.Visible;
		WorldContainer worldContainer = ((ClusterManager.Instance != null) ? ClusterManager.Instance.activeWorld : null);
		int y2 = ((worldContainer != null) ? (worldContainer.WorldSize.y + worldContainer.WorldOffset.y - 1) : Grid.HeightInCells);
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					int num2 = Grid.XYToCell(j, y2);
					if (Grid.IsValidCell(num2))
					{
						region.SetBytes(j, i, visible[num2]);
					}
					else
					{
						region.SetBytes(j, i, 0);
					}
				}
				else
				{
					region.SetBytes(j, i, visible[num]);
				}
			}
		}
	}

	private static void UpdatePressure(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 pressureRange = instance.PressureRange;
		float minPressureVisibility = instance.MinPressureVisibility;
		float num = pressureRange.y - pressureRange.x;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num2 = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num2))
				{
					region.SetBytes(j, i, 0);
					continue;
				}
				float num3 = 0f;
				Element element = Grid.Element[num2];
				if (element.IsGas)
				{
					float num4 = Grid.Pressure[num2];
					float b = ((num4 > 0f) ? minPressureVisibility : 0f);
					num3 = Mathf.Max(Mathf.Clamp01((num4 - pressureRange.x) / num), b);
				}
				else if (element.IsLiquid)
				{
					int num5 = Grid.CellAbove(num2);
					if (Grid.IsValidCell(num5) && Grid.Element[num5].IsGas)
					{
						float num6 = Grid.Pressure[num5];
						float b2 = ((num6 > 0f) ? minPressureVisibility : 0f);
						num3 = Mathf.Max(Mathf.Clamp01((num6 - pressureRange.x) / num), b2);
					}
				}
				region.SetBytes(j, i, (byte)(num3 * 255f));
			}
		}
	}

	private static void UpdateDanger(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0);
					continue;
				}
				byte b = (byte)((Grid.Element[num].id != SimHashes.Oxygen) ? 255u : 0u);
				region.SetBytes(j, i, b);
			}
		}
	}

	private static void UpdateStateChange(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		float temperatureStateChangeRange = instance.TemperatureStateChangeRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0);
					continue;
				}
				float num2 = 0f;
				Element element = Grid.Element[num];
				if (!element.IsVacuum)
				{
					float num3 = Grid.Temperature[num];
					float num4 = element.lowTemp * temperatureStateChangeRange;
					float a = Mathf.Abs(num3 - element.lowTemp) / num4;
					float num5 = element.highTemp * temperatureStateChangeRange;
					float b = Mathf.Abs(num3 - element.highTemp) / num5;
					num2 = Mathf.Max(num2, 1f - Mathf.Min(a, b));
				}
				region.SetBytes(j, i, (byte)(num2 * 255f));
			}
		}
	}

	private static void UpdateFallingSolidChange(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0);
					continue;
				}
				float num2 = 0f;
				Element element = Grid.Element[num];
				if (element.id == SimHashes.Mud || element.id == SimHashes.ToxicMud)
				{
					num2 = 0.65f;
				}
				region.SetBytes(j, i, (byte)(num2 * 255f));
			}
		}
	}

	private static void UpdateGasColour(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0, 0, 0, 0);
					continue;
				}
				Element element = Grid.Element[num];
				if (element.IsGas)
				{
					region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
				}
				else if (element.IsLiquid)
				{
					if (Grid.IsValidCell(Grid.CellAbove(num)))
					{
						region.SetBytes(j, i, element.substance.colour.r, element.substance.colour.g, element.substance.colour.b, byte.MaxValue);
					}
					else
					{
						region.SetBytes(j, i, 0, 0, 0, 0);
					}
				}
				else
				{
					region.SetBytes(j, i, 0, 0, 0, 0);
				}
			}
		}
	}

	private static void UpdateLiquid(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = x0; i <= x1; i++)
		{
			int num = Grid.XYToCell(i, y1);
			Element element = Grid.Element[num];
			for (int num2 = y1; num2 >= y0; num2--)
			{
				int num3 = Grid.XYToCell(i, num2);
				if (!Grid.IsActiveWorld(num3))
				{
					region.SetBytes(i, num2, 0, 0, 0, 0);
				}
				else
				{
					Element element2 = Grid.Element[num3];
					if (element2.IsLiquid)
					{
						Color32 colour = element2.substance.colour;
						float liquidMaxMass = Lighting.Instance.Settings.LiquidMaxMass;
						float liquidAmountOffset = Lighting.Instance.Settings.LiquidAmountOffset;
						float num4;
						if (element.IsLiquid || element.IsSolid)
						{
							num4 = 1f;
						}
						else
						{
							num4 = liquidAmountOffset + (1f - liquidAmountOffset) * Mathf.Min(Grid.Mass[num3] / liquidMaxMass, 1f);
							num4 = Mathf.Pow(Mathf.Min(Grid.Mass[num3] / liquidMaxMass, 1f), 0.45f);
						}
						region.SetBytes(i, num2, (byte)(num4 * 255f), colour.r, colour.g, colour.b);
					}
					else
					{
						region.SetBytes(i, num2, 0, 0, 0, 0);
					}
					element = element2;
				}
			}
		}
	}

	private static void UpdateSolidDigAmount(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		int elementIndex = ElementLoader.GetElementIndex(SimHashes.Void);
		for (int i = y0; i <= y1; i++)
		{
			int num = Grid.XYToCell(x0, i);
			int num2 = Grid.XYToCell(x1, i);
			int num3 = num;
			int num4 = x0;
			while (num3 <= num2)
			{
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if (Grid.ElementIdx[num3] != elementIndex)
				{
					b3 = byte.MaxValue;
				}
				if (Grid.Solid[num3])
				{
					b = byte.MaxValue;
					b2 = (byte)(255f * Grid.Damage[num3]);
				}
				region.SetBytes(num4, i, b, b2, b3);
				num3++;
				num4++;
			}
		}
	}

	private static void UpdateSolidLiquidGasMass(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0, 0, 0, 0);
					continue;
				}
				Element element = Grid.Element[num];
				byte b = 0;
				byte b2 = 0;
				byte b3 = 0;
				if (element.IsSolid)
				{
					b = byte.MaxValue;
				}
				else if (element.IsLiquid)
				{
					b2 = byte.MaxValue;
				}
				else if (element.IsGas || element.IsVacuum)
				{
					b3 = byte.MaxValue;
				}
				float num2 = Grid.Mass[num];
				float num3 = Mathf.Min(1f, num2 / 2000f);
				if (num2 > 0f)
				{
					num3 = Mathf.Max(0.003921569f, num3);
				}
				region.SetBytes(j, i, b, b2, b3, (byte)(num3 * 255f));
			}
		}
	}

	private static void GetTemperatureAlpha(float t, Vector2 cold_range, Vector2 hot_range, out byte cold_alpha, out byte hot_alpha)
	{
		cold_alpha = 0;
		hot_alpha = 0;
		if (t <= cold_range.y)
		{
			float num = Mathf.Clamp01((cold_range.y - t) / (cold_range.y - cold_range.x));
			cold_alpha = (byte)(num * 255f);
		}
		else if (t >= hot_range.x)
		{
			float num2 = Mathf.Clamp01((t - hot_range.x) / (hot_range.y - hot_range.x));
			hot_alpha = (byte)(num2 * 255f);
		}
	}

	private static void UpdateTemperature(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		Vector2 cold_range = instance.coldRange;
		Vector2 hot_range = instance.hotRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0, 0, 0);
					continue;
				}
				float num2 = Grid.Temperature[num];
				GetTemperatureAlpha(num2, cold_range, hot_range, out var cold_alpha, out var hot_alpha);
				byte b = (byte)(255f * Mathf.Pow(Mathf.Clamp(num2 / 1000f, 0f, 1f), 0.45f));
				region.SetBytes(j, i, cold_alpha, hot_alpha, b);
			}
		}
	}

	private static void UpdateWorldLight(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		if (!instance.ForceLightEverywhere)
		{
			for (int i = y0; i <= y1; i++)
			{
				int num = Grid.XYToCell(x0, i);
				int num2 = Grid.XYToCell(x1, i);
				int num3 = num;
				int num4 = x0;
				while (num3 <= num2)
				{
					Color32 color = ((Grid.LightCount[num3] > 0) ? Lighting.Instance.Settings.LightColour : new Color32(0, 0, 0, byte.MaxValue));
					region.SetBytes(num4, i, color.r, color.g, color.b, (byte)((color.r + color.g + color.b > 0) ? 255u : 0u));
					num3++;
					num4++;
				}
			}
			return;
		}
		for (int j = y0; j <= y1; j++)
		{
			for (int k = x0; k <= x1; k++)
			{
				region.SetBytes(k, j, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			}
		}
	}

	private static void UpdateRadiation(TextureRegion region, int x0, int y0, int x1, int y1)
	{
		_ = instance.coldRange;
		_ = instance.hotRange;
		for (int i = y0; i <= y1; i++)
		{
			for (int j = x0; j <= x1; j++)
			{
				int num = Grid.XYToCell(j, i);
				if (!Grid.IsActiveWorld(num))
				{
					region.SetBytes(j, i, 0, 0, 0);
					continue;
				}
				float v = Grid.Radiation[num];
				region.SetBytes(j, i, v);
			}
		}
	}

	public PropertyTextures()
	{
		TextureProperties[] array = new TextureProperties[14];
		TextureProperties textureProperties = new TextureProperties
		{
			simProperty = Property.Flow,
			textureFormat = TextureFormat.RGFloat,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 0.25f
		};
		array[0] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.Liquid,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = true,
			blendSpeed = 1f
		};
		array[1] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.ExposedToSunlight,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = true,
			blend = false,
			blendSpeed = 0f
		};
		array[2] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.SolidDigAmount,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[3] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.GasColour,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		};
		array[4] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.GasDanger,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		};
		array[5] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.GasPressure,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = true,
			blendSpeed = 0.25f
		};
		array[6] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.FogOfWar,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[7] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.WorldLight,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[8] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.StateChange,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[9] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.FallingSolid,
			textureFormat = TextureFormat.Alpha8,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[10] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.SolidLiquidGasMass,
			textureFormat = TextureFormat.RGBA32,
			filterMode = FilterMode.Point,
			updateEveryFrame = true,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[11] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.Temperature,
			textureFormat = TextureFormat.RGB24,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[12] = textureProperties;
		textureProperties = new TextureProperties
		{
			simProperty = Property.Radiation,
			textureFormat = TextureFormat.RFloat,
			filterMode = FilterMode.Bilinear,
			updateEveryFrame = false,
			updatedExternally = false,
			blend = false,
			blendSpeed = 0f
		};
		array[13] = textureProperties;
		this.textureProperties = array;
		allTextureProperties = new List<TextureProperties>();
		workItems = new WorkItemCollection<WorkItem, object>();
		base._002Ector();
	}
}
