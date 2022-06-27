using System;
using System.Collections.Generic;
using System.Reflection;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class LegacyModMain
{
	private struct Entry
	{
		public int count;

		public Type type;
	}

	private struct ElementInfo
	{
		public SimHashes id;

		public float decor;

		public float overheatMod;
	}

	public static void Load()
	{
		List<Type> list = new List<Type>();
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		for (int i = 0; i < assemblies.Length; i++)
		{
			Type[] types = assemblies[i].GetTypes();
			if (types != null)
			{
				list.AddRange(types);
			}
		}
		EntityTemplates.CreateTemplates();
		EntityTemplates.CreateBaseOreTemplates();
		LoadOre(list);
		LoadBuildings(list);
		ConfigElements();
		LoadEntities(list);
		LoadEquipment(list);
		EntityTemplates.DestroyBaseOreTemplates();
	}

	private static void Test()
	{
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Component));
		for (int i = 0; i < array.Length; i++)
		{
			Type type = ((Component)array[i]).GetType();
			int value = 0;
			dictionary.TryGetValue(type, out value);
			dictionary[type] = value + 1;
		}
		List<Entry> list = new List<Entry>();
		foreach (KeyValuePair<Type, int> item in dictionary)
		{
			if (item.Key.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy) != null)
			{
				list.Add(new Entry
				{
					count = item.Value,
					type = item.Key
				});
			}
		}
		list.Sort((Entry x, Entry y) => y.count.CompareTo(x.count));
		string text = "";
		foreach (Entry item2 in list)
		{
			string[] obj = new string[5]
			{
				text,
				item2.type.Name,
				": ",
				null,
				null
			};
			int i = item2.count;
			obj[3] = i.ToString();
			obj[4] = "\n";
			text = string.Concat(obj);
		}
		Debug.Log(text);
	}

	private static void ListUnusedTypes()
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject));
		for (int i = 0; i < array.Length; i++)
		{
			Component[] components = ((GameObject)array[i]).GetComponents<Component>();
			foreach (Component component in components)
			{
				if (!(component == null))
				{
					Type type = component.GetType();
					while (type != typeof(Component))
					{
						hashSet.Add(type);
						type = type.BaseType;
					}
				}
			}
		}
		HashSet<Type> hashSet2 = new HashSet<Type>();
		foreach (Type currentDomainType in App.GetCurrentDomainTypes())
		{
			if (typeof(MonoBehaviour).IsAssignableFrom(currentDomainType) && !hashSet.Contains(currentDomainType))
			{
				hashSet2.Add(currentDomainType);
			}
		}
		List<Type> list = new List<Type>(hashSet2);
		list.Sort((Type x, Type y) => x.FullName.CompareTo(y.FullName));
		string text = "Unused types:";
		foreach (Type item in list)
		{
			text = text + "\n" + item.FullName;
		}
		Debug.Log(text);
	}

	private static void DebugSelected()
	{
	}

	private static void DebugSelected(GameObject go)
	{
		Constructable component = go.GetComponent<Constructable>();
		_ = 0 + 1;
		Debug.Log(component);
	}

	private static void LoadOre(List<Type> types)
	{
		GeneratedOre.LoadGeneratedOre(types);
	}

	private static void LoadBuildings(List<Type> types)
	{
		LocString.CreateLocStringKeys(typeof(BUILDINGS.PREFABS), "STRINGS.BUILDINGS.");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DAMAGESOURCES), "STRINGS.BUILDINGS.DAMAGESOURCES");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.REPAIRABLE), "STRINGS.BUILDINGS.REPAIRABLE");
		LocString.CreateLocStringKeys(typeof(BUILDINGS.DISINFECTABLE), "STRINGS.BUILDINGS.DISINFECTABLE");
		GeneratedBuildings.LoadGeneratedBuildings(types);
	}

	private static void LoadEntities(List<Type> types)
	{
		EntityConfigManager.Instance.LoadGeneratedEntities(types);
		BuildingConfigManager.Instance.ConfigurePost();
	}

	private static void LoadEquipment(List<Type> types)
	{
		LocString.CreateLocStringKeys(typeof(EQUIPMENT.PREFABS), "STRINGS.EQUIPMENT.");
		GeneratedEquipment.LoadGeneratedEquipment(types);
	}

	private static void ConfigElements()
	{
		ElementInfo[] array = new ElementInfo[20]
		{
			new ElementInfo
			{
				id = SimHashes.Katairite,
				overheatMod = 200f
			},
			new ElementInfo
			{
				id = SimHashes.Cuprite,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.Copper,
				decor = 0.2f,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Gold,
				decor = 0.5f,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Lead,
				overheatMod = -20f
			},
			new ElementInfo
			{
				id = SimHashes.Granite,
				decor = 0.2f,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.SandStone,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.ToxicSand,
				overheatMod = -10f
			},
			new ElementInfo
			{
				id = SimHashes.Dirt,
				overheatMod = -10f
			},
			new ElementInfo
			{
				id = SimHashes.IgneousRock,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.Obsidian,
				overheatMod = 15f
			},
			new ElementInfo
			{
				id = SimHashes.Ceramic,
				overheatMod = 200f,
				decor = 0.2f
			},
			new ElementInfo
			{
				id = SimHashes.RefinedCarbon,
				overheatMod = 900f
			},
			new ElementInfo
			{
				id = SimHashes.Iron,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Tungsten,
				overheatMod = 50f
			},
			new ElementInfo
			{
				id = SimHashes.Steel,
				overheatMod = 200f
			},
			new ElementInfo
			{
				id = SimHashes.GoldAmalgam,
				overheatMod = 50f,
				decor = 0.1f
			},
			new ElementInfo
			{
				id = SimHashes.Diamond,
				overheatMod = 200f,
				decor = 1f
			},
			new ElementInfo
			{
				id = SimHashes.Niobium,
				decor = 0.5f,
				overheatMod = 500f
			},
			new ElementInfo
			{
				id = SimHashes.TempConductorSolid,
				overheatMod = 900f
			}
		};
		for (int i = 0; i < array.Length; i++)
		{
			ElementInfo elementInfo = array[i];
			Element element = ElementLoader.FindElementByHash(elementInfo.id);
			if (elementInfo.decor != 0f)
			{
				AttributeModifier item = new AttributeModifier("Decor", elementInfo.decor, element.name, is_multiplier: true);
				element.attributeModifiers.Add(item);
			}
			if (elementInfo.overheatMod != 0f)
			{
				AttributeModifier item2 = new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, elementInfo.overheatMod, element.name);
				element.attributeModifiers.Add(item2);
			}
		}
	}
}
