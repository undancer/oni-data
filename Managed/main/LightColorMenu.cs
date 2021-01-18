using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightColorMenu")]
public class LightColorMenu : KMonoBehaviour
{
	[Serializable]
	public struct LightColor
	{
		public string name;

		public Color color;

		public LightColor(string name, Color color)
		{
			this.name = name;
			this.color = color;
		}
	}

	public LightColor[] lightColors;

	private int currentColor;

	private static readonly EventSystem.IntraObjectHandler<LightColorMenu> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LightColorMenu>(delegate(LightColorMenu component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		SetColor(0);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (lightColors.Length == 0)
		{
			return;
		}
		int num = lightColors.Length;
		for (int i = 0; i < num; i++)
		{
			if (i != currentColor)
			{
				int new_color = i;
				Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo(lightColors[i].name, lightColors[i].name, delegate
				{
					SetColor(new_color);
				}));
			}
		}
	}

	private void SetColor(int color_index)
	{
		if (lightColors.Length != 0 && color_index < lightColors.Length)
		{
			Light2D[] componentsInChildren = GetComponentsInChildren<Light2D>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Color = lightColors[color_index].color;
			}
			MeshRenderer[] componentsInChildren2 = GetComponentsInChildren<MeshRenderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				Material[] materials = componentsInChildren2[i].materials;
				foreach (Material material in materials)
				{
					if (material.name.StartsWith("matScriptedGlow01"))
					{
						material.color = lightColors[color_index].color;
					}
				}
			}
		}
		currentColor = color_index;
	}
}
