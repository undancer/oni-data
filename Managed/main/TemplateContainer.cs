using System;
using System.Collections.Generic;
using System.IO;
using Klei;
using TemplateClasses;
using UnityEngine;

[Serializable]
public class TemplateContainer
{
	[Serializable]
	public class Info
	{
		public Vector2f size
		{
			get;
			set;
		}

		public int area
		{
			get;
			set;
		}

		public Tag[] tags
		{
			get;
			set;
		}
	}

	public string name
	{
		get;
		private set;
	}

	public int priority
	{
		get;
		set;
	}

	public Info info
	{
		get;
		set;
	}

	public List<Cell> cells
	{
		get;
		set;
	}

	public List<Prefab> buildings
	{
		get;
		set;
	}

	public List<Prefab> pickupables
	{
		get;
		set;
	}

	public List<Prefab> elementalOres
	{
		get;
		set;
	}

	public List<Prefab> otherEntities
	{
		get;
		set;
	}

	public void Init(List<Cell> _cells, List<Prefab> _buildings, List<Prefab> _pickupables, List<Prefab> _elementalOres, List<Prefab> _otherEntities)
	{
		if (_cells != null && _cells.Count > 0)
		{
			cells = _cells;
		}
		if (_buildings != null && _buildings.Count > 0)
		{
			buildings = _buildings;
		}
		if (_pickupables != null && _pickupables.Count > 0)
		{
			pickupables = _pickupables;
		}
		if (_elementalOres != null && _elementalOres.Count > 0)
		{
			elementalOres = _elementalOres;
		}
		if (_otherEntities != null && _otherEntities.Count > 0)
		{
			otherEntities = _otherEntities;
		}
		info = new Info();
		RefreshInfo();
	}

	public RectInt GetTemplateBounds(int padding = 0)
	{
		return GetTemplateBounds(Vector2I.zero, padding);
	}

	public RectInt GetTemplateBounds(Vector2 position, int padding = 0)
	{
		return GetTemplateBounds(new Vector2I((int)position.x, (int)position.y), padding);
	}

	public RectInt GetTemplateBounds(Vector2I position, int padding = 0)
	{
		int num = 1;
		int num2 = -1;
		int num3 = 1;
		int num4 = -1;
		foreach (Cell cell in cells)
		{
			if (cell.location_x < num)
			{
				num = cell.location_x;
			}
			if (cell.location_x > num2)
			{
				num2 = cell.location_x;
			}
			if (cell.location_y < num3)
			{
				num3 = cell.location_y;
			}
			if (cell.location_y > num4)
			{
				num4 = cell.location_y;
			}
		}
		return new RectInt(position.x + num - padding, position.y + num3 - padding, (int)info.size.x + padding * 2, (int)info.size.y + padding * 2);
	}

	public void RefreshInfo()
	{
		if (cells == null)
		{
			return;
		}
		int num = 1;
		int num2 = -1;
		int num3 = 1;
		int num4 = -1;
		foreach (Cell cell in cells)
		{
			if (cell.location_x < num)
			{
				num = cell.location_x;
			}
			if (cell.location_x > num2)
			{
				num2 = cell.location_x;
			}
			if (cell.location_y < num3)
			{
				num3 = cell.location_y;
			}
			if (cell.location_y > num4)
			{
				num4 = cell.location_y;
			}
		}
		info.size = new Vector2(1 + (num2 - num), 1 + (num4 - num3));
		info.area = cells.Count;
	}

	public void SaveToYaml(string save_name)
	{
		string text = save_name;
		while (text.Contains("/"))
		{
			int num = text.IndexOf('/') + 1;
			if (text.Length > num)
			{
				text = text.Substring(num);
			}
		}
		name = text;
		string templatePath = TemplateCache.GetTemplatePath();
		if (!File.Exists(templatePath))
		{
			Directory.CreateDirectory(templatePath);
		}
		YamlIO.Save(this, templatePath + "/" + save_name + ".yaml");
	}
}
