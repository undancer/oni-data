using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public abstract class ResourceSet : Resource
{
	public abstract int Count
	{
		get;
	}

	public ResourceSet()
	{
	}

	public ResourceSet(string id, ResourceSet parent)
		: base(id, parent)
	{
	}

	public abstract Resource Add(Resource resource);

	public abstract void Remove(Resource resource);

	public abstract Resource GetResource(int idx);
}
[Serializable]
public class ResourceSet<T> : ResourceSet where T : Resource
{
	public List<T> resources = new List<T>();

	public T this[int idx] => resources[idx];

	public override int Count => resources.Count;

	public override Resource GetResource(int idx)
	{
		return resources[idx];
	}

	public ResourceSet()
	{
	}

	public ResourceSet(string id, ResourceSet parent)
		: base(id, parent)
	{
	}

	public override void Initialize()
	{
		foreach (T resource in resources)
		{
			resource.Initialize();
		}
	}

	public bool Exists(string id)
	{
		foreach (T resource in resources)
		{
			if (resource.Id == id)
			{
				return true;
			}
		}
		return false;
	}

	public T TryGet(string id)
	{
		foreach (T resource in resources)
		{
			if (resource.Id == id)
			{
				return resource;
			}
		}
		return null;
	}

	public T TryGet(HashedString id)
	{
		foreach (T resource in resources)
		{
			if (resource.IdHash == id)
			{
				return resource;
			}
		}
		return null;
	}

	public T Get(HashedString id)
	{
		foreach (T resource in resources)
		{
			if (new HashedString(resource.Id) == id)
			{
				return resource;
			}
		}
		string str = typeof(T).ToString();
		HashedString hashedString = id;
		Debug.LogError("Could not find " + str + ": " + hashedString.ToString());
		return null;
	}

	public T Get(string id)
	{
		foreach (T resource in resources)
		{
			if (resource.Id == id)
			{
				return resource;
			}
		}
		Debug.LogError("Could not find " + typeof(T).ToString() + ": " + id);
		return null;
	}

	public override void Remove(Resource resource)
	{
		T val = resource as T;
		if (val == null)
		{
			Debug.LogError("Resource type mismatch: " + resource.GetType().Name + " does not match " + typeof(T).Name);
		}
		resources.Remove(val);
	}

	public override Resource Add(Resource resource)
	{
		T val = resource as T;
		if (val == null)
		{
			Debug.LogError("Resource type mismatch: " + resource.GetType().Name + " does not match " + typeof(T).Name);
		}
		Add(val);
		return resource;
	}

	public T Add(T resource)
	{
		if (resource == null)
		{
			Debug.LogError("Tried to add a null to the resource set");
			return null;
		}
		resources.Add(resource);
		return resource;
	}

	public void ResolveReferences()
	{
		Type type = GetType();
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (fieldInfo.FieldType.IsSubclassOf(typeof(Resource)) && fieldInfo.GetValue(this) == null)
			{
				Resource resource = Get(fieldInfo.Name);
				if (resource != null)
				{
					fieldInfo.SetValue(this, resource);
				}
			}
		}
	}
}
