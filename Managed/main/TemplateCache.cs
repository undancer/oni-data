using System.Collections.Generic;
using Klei;
using ProcGen;

public static class TemplateCache
{
	private const string defaultAssetFolder = "bases";

	private static Dictionary<string, TemplateContainer> templates = null;

	public static bool Initted
	{
		get;
		private set;
	}

	public static void Init()
	{
		if (!Initted)
		{
			templates = new Dictionary<string, TemplateContainer>();
			Initted = true;
		}
	}

	public static void Clear()
	{
		templates = null;
		Initted = false;
	}

	public static string RewriteTemplatePath(string scopePath)
	{
		SettingsCache.GetDlcIdAndPath(scopePath, out var dlcId, out var path);
		return SettingsCache.GetAbsoluteContentPath(dlcId, "templates/" + path);
	}

	public static string RewriteTemplateYaml(string scopePath)
	{
		return RewriteTemplatePath(scopePath) + ".yaml";
	}

	public static TemplateContainer GetTemplate(string templatePath)
	{
		if (!templates.ContainsKey(templatePath))
		{
			templates.Add(templatePath, null);
		}
		if (templates[templatePath] == null)
		{
			string text = RewriteTemplateYaml(templatePath);
			TemplateContainer templateContainer = YamlIO.LoadFile<TemplateContainer>(text);
			if (templateContainer == null)
			{
				Debug.LogWarning("Missing template [" + text + "]");
			}
			templates[templatePath] = templateContainer;
		}
		return templates[templatePath];
	}

	public static bool TemplateExists(string templatePath)
	{
		string path = RewriteTemplateYaml(templatePath);
		return FileSystem.FileExists(path);
	}
}
