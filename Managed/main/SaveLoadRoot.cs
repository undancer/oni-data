using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using KSerialization;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SaveLoadRoot")]
public class SaveLoadRoot : KMonoBehaviour
{
	private bool hasOnSpawnRun;

	private bool registered = true;

	[SerializeField]
	private List<string> m_optionalComponentTypeNames = new List<string>();

	private static Dictionary<string, ISerializableComponentManager> serializableComponentManagers;

	private static Dictionary<Type, string> sTypeToString = new Dictionary<Type, string>();

	public static void DestroyStatics()
	{
		serializableComponentManagers = null;
	}

	protected override void OnPrefabInit()
	{
		if (serializableComponentManagers != null)
		{
			return;
		}
		serializableComponentManagers = new Dictionary<string, ISerializableComponentManager>();
		FieldInfo[] fields = typeof(GameComps).GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			IComponentManager componentManager = (IComponentManager)fieldInfo.GetValue(null);
			if (typeof(ISerializableComponentManager).IsAssignableFrom(componentManager.GetType()))
			{
				Type type = componentManager.GetType();
				serializableComponentManagers[type.ToString()] = (ISerializableComponentManager)componentManager;
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (registered)
		{
			SaveLoader.Instance.saveManager.Register(this);
		}
		hasOnSpawnRun = true;
	}

	public void DeclareOptionalComponent<T>() where T : KMonoBehaviour
	{
		m_optionalComponentTypeNames.Add(typeof(T).ToString());
	}

	public void SetRegistered(bool registered)
	{
		if (this.registered == registered)
		{
			return;
		}
		this.registered = registered;
		if (hasOnSpawnRun)
		{
			if (registered)
			{
				SaveLoader.Instance.saveManager.Register(this);
			}
			else
			{
				SaveLoader.Instance.saveManager.Unregister(this);
			}
		}
	}

	protected override void OnCleanUp()
	{
		if (SaveLoader.Instance != null && SaveLoader.Instance.saveManager != null)
		{
			SaveLoader.Instance.saveManager.Unregister(this);
		}
		if (GameComps.WhiteBoards.Has(base.gameObject))
		{
			GameComps.WhiteBoards.Remove(base.gameObject);
		}
	}

	public void Save(BinaryWriter writer)
	{
		Transform transform = base.transform;
		writer.Write(transform.GetPosition());
		writer.Write(transform.rotation);
		writer.Write(transform.localScale);
		byte value = 0;
		writer.Write(value);
		SaveWithoutTransform(writer);
	}

	public void SaveWithoutTransform(BinaryWriter writer)
	{
		KMonoBehaviour[] components = GetComponents<KMonoBehaviour>();
		if (components == null)
		{
			return;
		}
		int num = 0;
		KMonoBehaviour[] array = components;
		foreach (KMonoBehaviour kMonoBehaviour in array)
		{
			if ((kMonoBehaviour is ISaveLoadableDetails || (object)kMonoBehaviour != null) && !kMonoBehaviour.GetType().IsDefined(typeof(SkipSaveFileSerialization), inherit: false))
			{
				num++;
			}
		}
		foreach (KeyValuePair<string, ISerializableComponentManager> serializableComponentManager in serializableComponentManagers)
		{
			ISerializableComponentManager value = serializableComponentManager.Value;
			if (value.Has(base.gameObject))
			{
				num++;
			}
		}
		writer.Write(num);
		KMonoBehaviour[] array2 = components;
		foreach (KMonoBehaviour kMonoBehaviour2 in array2)
		{
			if ((kMonoBehaviour2 is ISaveLoadableDetails || (object)kMonoBehaviour2 != null) && !kMonoBehaviour2.GetType().IsDefined(typeof(SkipSaveFileSerialization), inherit: false))
			{
				writer.WriteKleiString(kMonoBehaviour2.GetType().ToString());
				long position = writer.BaseStream.Position;
				writer.Write(0);
				long position2 = writer.BaseStream.Position;
				if (kMonoBehaviour2 is ISaveLoadableDetails)
				{
					ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)kMonoBehaviour2;
					Serializer.SerializeTypeless(kMonoBehaviour2, writer);
					saveLoadableDetails.Serialize(writer);
				}
				else if ((object)kMonoBehaviour2 != null)
				{
					Serializer.SerializeTypeless(kMonoBehaviour2, writer);
				}
				long position3 = writer.BaseStream.Position;
				long num2 = position3 - position2;
				writer.BaseStream.Position = position;
				writer.Write((int)num2);
				writer.BaseStream.Position = position3;
			}
		}
		foreach (KeyValuePair<string, ISerializableComponentManager> serializableComponentManager2 in serializableComponentManagers)
		{
			ISerializableComponentManager value2 = serializableComponentManager2.Value;
			if (value2.Has(base.gameObject))
			{
				string key = serializableComponentManager2.Key;
				writer.WriteKleiString(key);
				value2.Serialize(base.gameObject, writer);
			}
		}
	}

	public static SaveLoadRoot Load(Tag tag, IReader reader)
	{
		GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(tag);
		return Load(prefab, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, IReader reader)
	{
		Vector3 position = reader.ReadVector3();
		Quaternion rotation = reader.ReadQuaternion();
		Vector3 scale = reader.ReadVector3();
		reader.ReadByte();
		if (SaveManager.DEBUG_OnlyLoadThisCellsObjects > -1)
		{
			Vector3 vector = Grid.CellToPos(SaveManager.DEBUG_OnlyLoadThisCellsObjects);
			if ((position.x < vector.x || position.x >= vector.x + 1f || position.y < vector.y || position.y >= vector.y + 1f) && prefab.name != "SaveGame")
			{
				prefab = null;
			}
			else
			{
				Debug.Log("Keeping " + prefab.name);
			}
		}
		return Load(prefab, position, rotation, scale, reader);
	}

	public static SaveLoadRoot Load(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, IReader reader)
	{
		SaveLoadRoot saveLoadRoot = null;
		if (prefab != null)
		{
			GameObject gameObject = Util.KInstantiate(prefab, position, rotation, null, null, initialize_id: false);
			gameObject.transform.localScale = scale;
			gameObject.SetActive(value: true);
			saveLoadRoot = gameObject.GetComponent<SaveLoadRoot>();
			if (saveLoadRoot != null)
			{
				try
				{
					LoadInternal(gameObject, reader);
				}
				catch (ArgumentException ex)
				{
					DebugUtil.LogErrorArgs(gameObject, "Failed to load SaveLoadRoot ", ex.Message, "\n", ex.StackTrace);
				}
			}
			else
			{
				Debug.Log("missing SaveLoadRoot", gameObject);
			}
		}
		else
		{
			LoadInternal(null, reader);
		}
		return saveLoadRoot;
	}

	private static void LoadInternal(GameObject gameObject, IReader reader)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		KMonoBehaviour[] array = ((gameObject != null) ? gameObject.GetComponents<KMonoBehaviour>() : null);
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			int num2 = reader.ReadInt32();
			int position = reader.Position;
			if (serializableComponentManagers.TryGetValue(text, out var value))
			{
				value.Deserialize(gameObject, reader);
				continue;
			}
			int value2 = 0;
			dictionary.TryGetValue(text, out value2);
			KMonoBehaviour kMonoBehaviour = null;
			int num3 = 0;
			if (array != null)
			{
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j].GetType();
					if (!sTypeToString.TryGetValue(type, out var value3))
					{
						value3 = type.ToString();
						sTypeToString[type] = value3;
					}
					if (value3 == text)
					{
						if (num3 == value2)
						{
							kMonoBehaviour = array[j];
							break;
						}
						num3++;
					}
				}
			}
			if (kMonoBehaviour == null && gameObject != null)
			{
				SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
				int index;
				if (component != null && (index = component.m_optionalComponentTypeNames.IndexOf(text)) != -1)
				{
					Debug.Assert(value2 == 0 && num3 == 0, $"Implementation does not support multiple components with optional components, type {text}, {value2}, {num3}");
					Type type2 = Type.GetType(component.m_optionalComponentTypeNames[index]);
					kMonoBehaviour = (KMonoBehaviour)gameObject.AddComponent(type2);
				}
			}
			if (kMonoBehaviour == null)
			{
				reader.SkipBytes(num2);
				continue;
			}
			if ((object)kMonoBehaviour == null && !(kMonoBehaviour is ISaveLoadableDetails))
			{
				DebugUtil.LogErrorArgs("Component", text, "is not ISaveLoadable");
				reader.SkipBytes(num2);
				continue;
			}
			dictionary[text] = num3 + 1;
			if (kMonoBehaviour is ISaveLoadableDetails)
			{
				ISaveLoadableDetails saveLoadableDetails = (ISaveLoadableDetails)kMonoBehaviour;
				Deserializer.DeserializeTypeless(kMonoBehaviour, reader);
				saveLoadableDetails.Deserialize(reader);
			}
			else
			{
				Deserializer.DeserializeTypeless(kMonoBehaviour, reader);
			}
			if (reader.Position != position + num2)
			{
				DebugUtil.LogWarningArgs("Expected to be at offset", position + num2, "but was only at offset", reader.Position, ". Skipping to catch up.");
				reader.SkipBytes(position + num2 - reader.Position);
			}
		}
	}
}
