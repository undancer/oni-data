using System;
using UnityEngine;

[ExecuteInEditMode]
public class Lighting : MonoBehaviour
{
	public LightingSettings Settings;

	public static Lighting Instance;

	[NonSerialized]
	public bool disableLighting;

	private static int _liquidZ = Shader.PropertyToID("_LiquidZ");

	private static int _SceneLayerMax = Shader.PropertyToID("_SceneLayerMax");

	private static int _DigMapMapParameters = Shader.PropertyToID("_DigMapMapParameters");

	private static int _DigDamageMap = Shader.PropertyToID("_DigDamageMap");

	private static int _StateTransitionMap = Shader.PropertyToID("_StateTransitionMap");

	private static int _StateTransitionColor = Shader.PropertyToID("_StateTransitionColor");

	private static int _StateTransitionParameters = Shader.PropertyToID("_StateTransitionParameters");

	private static int _FallingSolidMap = Shader.PropertyToID("_FallingSolidMap");

	private static int _FallingSolidColor = Shader.PropertyToID("_FallingSolidColor");

	private static int _FallingSolidParameters = Shader.PropertyToID("_FallingSolidParameters");

	private static int _WaterTrimColor = Shader.PropertyToID("_WaterTrimColor");

	private static int _WaterParameters2 = Shader.PropertyToID("_WaterParameters2");

	private static int _WaterWaveParameters = Shader.PropertyToID("_WaterWaveParameters");

	private static int _WaterWaveParameters2 = Shader.PropertyToID("_WaterWaveParameters2");

	private static int _WaterDetailParameters = Shader.PropertyToID("_WaterDetailParameters");

	private static int _WaterDistortionParameters = Shader.PropertyToID("_WaterDistortionParameters");

	private static int _BloomParameters = Shader.PropertyToID("_BloomParameters");

	private static int _LiquidParameters2 = Shader.PropertyToID("_LiquidParameters2");

	private static int _GridParameters = Shader.PropertyToID("_GridParameters");

	private static int _GridColor = Shader.PropertyToID("_GridColor");

	private static int _EdgeGlowParameters = Shader.PropertyToID("_EdgeGlowParameters");

	private static int _SubstanceParameters = Shader.PropertyToID("_SubstanceParameters");

	private static int _TileEdgeParameters = Shader.PropertyToID("_TileEdgeParameters");

	private static int _AnimParameters = Shader.PropertyToID("_AnimParameters");

	private static int _GasOpacity = Shader.PropertyToID("_GasOpacity");

	private static int _DarkenTintBackground = Shader.PropertyToID("_DarkenTintBackground");

	private static int _DarkenTintMidground = Shader.PropertyToID("_DarkenTintMidground");

	private static int _DarkenTintForeground = Shader.PropertyToID("_DarkenTintForeground");

	private static int _BrightenOverlay = Shader.PropertyToID("_BrightenOverlay");

	private static int _ColdFG = Shader.PropertyToID("_ColdFG");

	private static int _ColdMG = Shader.PropertyToID("_ColdMG");

	private static int _ColdBG = Shader.PropertyToID("_ColdBG");

	private static int _HotFG = Shader.PropertyToID("_HotFG");

	private static int _HotMG = Shader.PropertyToID("_HotMG");

	private static int _HotBG = Shader.PropertyToID("_HotBG");

	private static int _TemperatureParallax = Shader.PropertyToID("_TemperatureParallax");

	private static int _ColdUVOffset1 = Shader.PropertyToID("_ColdUVOffset1");

	private static int _ColdUVOffset2 = Shader.PropertyToID("_ColdUVOffset2");

	private static int _HotUVOffset1 = Shader.PropertyToID("_HotUVOffset1");

	private static int _HotUVOffset2 = Shader.PropertyToID("_HotUVOffset2");

	private static int _DustColour = Shader.PropertyToID("_DustColour");

	private static int _DustInfo = Shader.PropertyToID("_DustInfo");

	private static int _DustTex = Shader.PropertyToID("_DustTex");

	private static int _DebugShowInfo = Shader.PropertyToID("_DebugShowInfo");

	private static int _HeatHazeParameters = Shader.PropertyToID("_HeatHazeParameters");

	private static int _HeatHazeTexture = Shader.PropertyToID("_HeatHazeTexture");

	private static int _ShineParams = Shader.PropertyToID("_ShineParams");

	private static int _ShineParams2 = Shader.PropertyToID("_ShineParams2");

	private static int _WorldZoneGasBlend = Shader.PropertyToID("_WorldZoneGasBlend");

	private static int _WorldZoneLiquidBlend = Shader.PropertyToID("_WorldZoneLiquidBlend");

	private static int _WorldZoneForegroundBlend = Shader.PropertyToID("_WorldZoneForegroundBlend");

	private static int _WorldZoneSimpleAnimBlend = Shader.PropertyToID("_WorldZoneSimpleAnimBlend");

	private static int _CharacterLitColour = Shader.PropertyToID("_CharacterLitColour");

	private static int _CharacterUnlitColour = Shader.PropertyToID("_CharacterUnlitColour");

	private static int _BuildingDamagedTex = Shader.PropertyToID("_BuildingDamagedTex");

	private static int _BuildingDamagedUVParameters = Shader.PropertyToID("_BuildingDamagedUVParameters");

	private static int _DiseaseOverlayTex = Shader.PropertyToID("_DiseaseOverlayTex");

	private static int _DiseaseOverlayTexInfo = Shader.PropertyToID("_DiseaseOverlayTexInfo");

	private static int _RadHazeColor = Shader.PropertyToID("_RadHazeColor");

	private static int _RadUVOffset1 = Shader.PropertyToID("_RadUVOffset1");

	private static int _RadUVOffset2 = Shader.PropertyToID("_RadUVOffset2");

	private static int _RadUVScales = Shader.PropertyToID("_RadUVScales");

	private static int _RadRange1 = Shader.PropertyToID("_RadRange1");

	private static int _RadRange2 = Shader.PropertyToID("_RadRange2");

	private static int _LightBufferTex = Shader.PropertyToID("_LightBufferTex");

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	private Color PremultiplyAlpha(Color c)
	{
		return c * c.a;
	}

	private void Start()
	{
		UpdateLighting();
	}

	private void Update()
	{
		UpdateLighting();
	}

	private void UpdateLighting()
	{
		Shader.SetGlobalInt(_liquidZ, -28);
		Shader.SetGlobalInt(_SceneLayerMax, 34);
		Shader.SetGlobalVector(_DigMapMapParameters, new Vector4(Settings.DigMapColour.r, Settings.DigMapColour.g, Settings.DigMapColour.b, Settings.DigMapScale));
		Shader.SetGlobalTexture(_DigDamageMap, Settings.DigDamageMap);
		Shader.SetGlobalTexture(_StateTransitionMap, Settings.StateTransitionMap);
		Shader.SetGlobalColor(_StateTransitionColor, Settings.StateTransitionColor);
		Shader.SetGlobalVector(_StateTransitionParameters, new Vector4(1f / Settings.StateTransitionUVScale, Settings.StateTransitionUVOffsetRate.x, Settings.StateTransitionUVOffsetRate.y, 0f));
		Shader.SetGlobalTexture(_FallingSolidMap, Settings.FallingSolidMap);
		Shader.SetGlobalColor(_FallingSolidColor, Settings.FallingSolidColor);
		Shader.SetGlobalVector(_FallingSolidParameters, new Vector4(1f / Settings.FallingSolidUVScale, Settings.FallingSolidUVOffsetRate.x, Settings.FallingSolidUVOffsetRate.y, 0f));
		Shader.SetGlobalColor(_WaterTrimColor, Settings.WaterTrimColor);
		Shader.SetGlobalVector(_WaterParameters2, new Vector4(Settings.WaterTrimSize, Settings.WaterAlphaTrimSize, 0f, Settings.WaterAlphaThreshold));
		Shader.SetGlobalVector(_WaterWaveParameters, new Vector4(Settings.WaterWaveAmplitude, Settings.WaterWaveFrequency, Settings.WaterWaveSpeed, 0f));
		Shader.SetGlobalVector(_WaterWaveParameters2, new Vector4(Settings.WaterWaveAmplitude2, Settings.WaterWaveFrequency2, Settings.WaterWaveSpeed2, 0f));
		Shader.SetGlobalVector(_WaterDetailParameters, new Vector4(Settings.WaterCubeMapScale, Settings.WaterDetailTiling, Settings.WaterColorScale, Settings.WaterDetailTiling2));
		Shader.SetGlobalVector(_WaterDistortionParameters, new Vector4(Settings.WaterDistortionScaleStart, Settings.WaterDistortionScaleEnd, Settings.WaterDepthColorOpacityStart, Settings.WaterDepthColorOpacityEnd));
		Shader.SetGlobalVector(_BloomParameters, new Vector4(Settings.BloomScale, 0f, 0f, 0f));
		Shader.SetGlobalVector(_LiquidParameters2, new Vector4(Settings.LiquidMin, Settings.LiquidMax, Settings.LiquidCutoff, Settings.LiquidTransparency));
		Shader.SetGlobalVector(_GridParameters, new Vector4(Settings.GridLineWidth, Settings.GridSize, Settings.GridMinIntensity, Settings.GridMaxIntensity));
		Shader.SetGlobalColor(_GridColor, Settings.GridColor);
		Shader.SetGlobalVector(_EdgeGlowParameters, new Vector4(Settings.EdgeGlowCutoffStart, Settings.EdgeGlowCutoffEnd, Settings.EdgeGlowIntensity, 0f));
		if (disableLighting)
		{
			Shader.SetGlobalVector(_SubstanceParameters, Vector4.one);
			Shader.SetGlobalVector(_TileEdgeParameters, Vector4.one);
		}
		else
		{
			Shader.SetGlobalVector(_SubstanceParameters, new Vector4(Settings.substanceEdgeParameters.intensity, Settings.substanceEdgeParameters.edgeIntensity, Settings.substanceEdgeParameters.directSunlightScale, Settings.substanceEdgeParameters.power));
			Shader.SetGlobalVector(_TileEdgeParameters, new Vector4(Settings.tileEdgeParameters.intensity, Settings.tileEdgeParameters.edgeIntensity, Settings.tileEdgeParameters.directSunlightScale, Settings.tileEdgeParameters.power));
		}
		float w = ((SimDebugView.Instance != null && SimDebugView.Instance.GetMode() == OverlayModes.Disease.ID) ? 1f : 0f);
		if (disableLighting)
		{
			Shader.SetGlobalVector(_AnimParameters, new Vector4(1f, Settings.WorldZoneAnimBlend, 0f, w));
		}
		else
		{
			Shader.SetGlobalVector(_AnimParameters, new Vector4(Settings.AnimIntensity, Settings.WorldZoneAnimBlend, 0f, w));
		}
		Shader.SetGlobalVector(_GasOpacity, new Vector4(Settings.GasMinOpacity, Settings.GasMaxOpacity, 0f, 0f));
		Shader.SetGlobalColor(_DarkenTintBackground, Settings.DarkenTints[0]);
		Shader.SetGlobalColor(_DarkenTintMidground, Settings.DarkenTints[1]);
		Shader.SetGlobalColor(_DarkenTintForeground, Settings.DarkenTints[2]);
		Shader.SetGlobalColor(_BrightenOverlay, Settings.BrightenOverlayColour);
		Shader.SetGlobalColor(_ColdFG, PremultiplyAlpha(Settings.ColdColours[2]));
		Shader.SetGlobalColor(_ColdMG, PremultiplyAlpha(Settings.ColdColours[1]));
		Shader.SetGlobalColor(_ColdBG, PremultiplyAlpha(Settings.ColdColours[0]));
		Shader.SetGlobalColor(_HotFG, PremultiplyAlpha(Settings.HotColours[2]));
		Shader.SetGlobalColor(_HotMG, PremultiplyAlpha(Settings.HotColours[1]));
		Shader.SetGlobalColor(_HotBG, PremultiplyAlpha(Settings.HotColours[0]));
		Shader.SetGlobalVector(_TemperatureParallax, Settings.TemperatureParallax);
		Shader.SetGlobalVector(_ColdUVOffset1, new Vector4(Settings.ColdBGUVOffset.x, Settings.ColdBGUVOffset.y, Settings.ColdMGUVOffset.x, Settings.ColdMGUVOffset.y));
		Shader.SetGlobalVector(_ColdUVOffset2, new Vector4(Settings.ColdFGUVOffset.x, Settings.ColdFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalVector(_HotUVOffset1, new Vector4(Settings.HotBGUVOffset.x, Settings.HotBGUVOffset.y, Settings.HotMGUVOffset.x, Settings.HotMGUVOffset.y));
		Shader.SetGlobalVector(_HotUVOffset2, new Vector4(Settings.HotFGUVOffset.x, Settings.HotFGUVOffset.y, 0f, 0f));
		Shader.SetGlobalColor(_DustColour, PremultiplyAlpha(Settings.DustColour));
		Shader.SetGlobalVector(_DustInfo, new Vector4(Settings.DustScale, Settings.DustMovement.x, Settings.DustMovement.y, Settings.DustMovement.z));
		Shader.SetGlobalTexture(_DustTex, Settings.DustTex);
		Shader.SetGlobalVector(_DebugShowInfo, new Vector4(Settings.ShowDust, Settings.ShowGas, Settings.ShowShadow, Settings.ShowTemperature));
		Shader.SetGlobalVector(_HeatHazeParameters, Settings.HeatHazeParameters);
		Shader.SetGlobalTexture(_HeatHazeTexture, Settings.HeatHazeTexture);
		Shader.SetGlobalVector(_ShineParams, new Vector4(Settings.ShineCenter.x, Settings.ShineCenter.y, Settings.ShineRange.x, Settings.ShineRange.y));
		Shader.SetGlobalVector(_ShineParams2, new Vector4(Settings.ShineZoomSpeed, 0f, 0f, 0f));
		Shader.SetGlobalFloat(_WorldZoneGasBlend, Settings.WorldZoneGasBlend);
		Shader.SetGlobalFloat(_WorldZoneLiquidBlend, Settings.WorldZoneLiquidBlend);
		Shader.SetGlobalFloat(_WorldZoneForegroundBlend, Settings.WorldZoneForegroundBlend);
		Shader.SetGlobalFloat(_WorldZoneSimpleAnimBlend, Settings.WorldZoneSimpleAnimBlend);
		Shader.SetGlobalColor(_CharacterLitColour, Settings.characterLighting.litColour);
		Shader.SetGlobalColor(_CharacterUnlitColour, Settings.characterLighting.unlitColour);
		Shader.SetGlobalTexture(_BuildingDamagedTex, Settings.BuildingDamagedTex);
		Shader.SetGlobalVector(_BuildingDamagedUVParameters, Settings.BuildingDamagedUVParameters);
		Shader.SetGlobalTexture(_DiseaseOverlayTex, Settings.DiseaseOverlayTex);
		Shader.SetGlobalVector(_DiseaseOverlayTexInfo, Settings.DiseaseOverlayTexInfo);
		if (Settings.ShowRadiation)
		{
			Shader.SetGlobalColor(_RadHazeColor, PremultiplyAlpha(Settings.RadColor));
		}
		else
		{
			Shader.SetGlobalColor(_RadHazeColor, Color.clear);
		}
		Shader.SetGlobalVector(_RadUVOffset1, new Vector4(Settings.Rad1UVOffset.x, Settings.Rad1UVOffset.y, Settings.Rad2UVOffset.x, Settings.Rad2UVOffset.y));
		Shader.SetGlobalVector(_RadUVOffset2, new Vector4(Settings.Rad3UVOffset.x, Settings.Rad3UVOffset.y, Settings.Rad4UVOffset.x, Settings.Rad4UVOffset.y));
		Shader.SetGlobalVector(_RadUVScales, new Vector4(1f / Settings.RadUVScales.x, 1f / Settings.RadUVScales.y, 1f / Settings.RadUVScales.z, 1f / Settings.RadUVScales.w));
		Shader.SetGlobalVector(_RadRange1, new Vector4(Settings.Rad1Range.x, Settings.Rad1Range.y, Settings.Rad2Range.x, Settings.Rad2Range.y));
		Shader.SetGlobalVector(_RadRange2, new Vector4(Settings.Rad3Range.x, Settings.Rad3Range.y, Settings.Rad4Range.x, Settings.Rad4Range.y));
		if (LightBuffer.Instance != null && LightBuffer.Instance.Texture != null)
		{
			Shader.SetGlobalTexture(_LightBufferTex, LightBuffer.Instance.Texture);
		}
	}
}
