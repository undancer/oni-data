using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class TextureUtil
{
	public static GraphicsFormat TextureFormatToGraphicsFormat(TextureFormat format)
	{
		switch (format)
		{
		case TextureFormat.Alpha8:
			return GraphicsFormat.R8_UNorm;
		case TextureFormat.RGB24:
			return GraphicsFormat.R8G8B8_SRGB;
		case TextureFormat.RGBA32:
			return GraphicsFormat.R8G8B8A8_SRGB;
		case TextureFormat.RGFloat:
			return GraphicsFormat.R32G32_SFloat;
		case TextureFormat.RGBAFloat:
			return GraphicsFormat.R32G32B32A32_SFloat;
		default:
			Debug.LogError("Unspecfied graphics format for texture format: " + format);
			throw new ArgumentOutOfRangeException();
		}
	}

	public static int GetBytesPerPixel(TextureFormat format)
	{
		return format switch
		{
			TextureFormat.Alpha8 => 1, 
			TextureFormat.RGB24 => 3, 
			TextureFormat.ARGB32 => 4, 
			TextureFormat.RGBA32 => 4, 
			TextureFormat.RGFloat => 8, 
			TextureFormat.RGBAFloat => 16, 
			TextureFormat.RFloat => 4, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	public static RenderTextureFormat GetRenderTextureFormat(TextureFormat format)
	{
		return format switch
		{
			TextureFormat.Alpha8 => RenderTextureFormat.ARGB32, 
			TextureFormat.RGB24 => RenderTextureFormat.ARGB32, 
			TextureFormat.ARGB32 => RenderTextureFormat.ARGB32, 
			TextureFormat.RGBA32 => RenderTextureFormat.ARGB32, 
			TextureFormat.RGFloat => RenderTextureFormat.RGFloat, 
			TextureFormat.RGBAFloat => RenderTextureFormat.ARGBHalf, 
			TextureFormat.RFloat => RenderTextureFormat.RFloat, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
