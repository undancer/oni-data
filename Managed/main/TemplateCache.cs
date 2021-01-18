using System.Collections.Generic;
using System.IO;
using Klei;
using UnityEngine;

public static class TemplateCache
{
	private static string baseTemplatePath = null;

	private static Dictionary<string, TemplateContainer> templates = null;

	private const string defaultAssetFolder = "bases";

	public static bool Initted
	{
		get;
		private set;
	}

	public static void Init()
	{
		templates = new Dictionary<string, TemplateContainer>();
		baseTemplatePath = FileSystem.Normalize(Path.Combine(Application.streamingAssetsPath, "templates"));
		Initted = true;
	}

	public static void Clear()
	{
		templates = null;
		baseTemplatePath = null;
		Initted = false;
	}

	public static string GetTemplatePath()
	{
		return baseTemplatePath;
	}

	public static TemplateContainer GetStartingBaseTemplate(string startingTemplateName)
	{
		DebugUtil.Assert(startingTemplateName != null, "Tried loading a starting template named ", startingTemplateName);
		if (baseTemplatePath == null)
		{
			Init();
		}
		return GetTemplate(Path.Combine("bases", startingTemplateName));
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!templates.ContainsKey(templatePath))
		{
			templates.Add(templatePath, null);
		}
		if (templates[templatePath] == null)
		{
			string text = FileSystem.Normalize(Path.Combine(baseTemplatePath, templatePath));
			TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(text + ".yaml");
			if (templateContainer == null)
			{
				Debug.LogWarning("Missing template [" + text + ".yaml]");
			}
			templates[templatePath] = templateContainer;
		}
		return templates[templatePath];
	}

	private static void GetAssetPaths(string folder, List<string> paths)
	{
		FileSystem.GetFiles(FileSystem.Normalize(Path.Combine(baseTemplatePath, folder)), "*.yaml", paths);
	}

	public static List<string> CollectBaseTemplateNames(string folder = "bases")
	{
		List<string> list = new List<string>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		GetAssetPaths(folder, pooledList);
		foreach (string item in pooledList)
		{
			string text = FileSystem.Normalize(Path.Combine(folder, Path.GetFileNameWithoutExtension(item)));
			list.Add(text);
			if (!templates.ContainsKey(text))
			{
				templates.Add(text, null);
			}
		}
		pooledList.Recycle();
		list.Sort((string x, string y) => x.CompareTo(y));
		return list;
	}

	public static List<TemplateContainer> CollectBaseTemplateAssets(string folder = "bases")
	{
		List<TemplateContainer> list = new List<TemplateContainer>();
		ListPool<string, TemplateContainer>.PooledList pooledList = ListPool<string, TemplateContainer>.Allocate();
		Debug.Log("Getting templates for " + folder + "...");
		GetAssetPaths(folder, pooledList);
		foreach (string item in pooledList)
		{
			string templatePath = FileSystem.Normalize(Path.Combine(folder, Path.GetFileNameWithoutExtension(item)));
			list.Add(GetTemplate(templatePath));
		}
		Debug.Log($"FINISHED getting templates for {folder} ({list.Count} total)...");
		pooledList.Recycle();
		list.Sort((TemplateContainer x, TemplateContainer y) => (y.priority - x.priority == 0) ? x.name.CompareTo(y.name) : (y.priority - x.priority));
		return list;
	}
}
