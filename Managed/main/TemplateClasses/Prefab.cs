using System;
using System.Collections.Generic;

namespace TemplateClasses
{
	[Serializable]
	public class Prefab
	{
		public enum Type
		{
			Building,
			Ore,
			Pickupable,
			Other
		}

		[Serializable]
		public class template_amount_value
		{
			public string id { get; set; }

			public float value { get; set; }

			public template_amount_value()
			{
			}

			public template_amount_value(string id, float value)
			{
				this.id = id;
				this.value = value;
			}
		}

		public string id { get; set; }

		public int location_x { get; set; }

		public int location_y { get; set; }

		public SimHashes element { get; set; }

		public float temperature { get; set; }

		public float units { get; set; }

		public string diseaseName { get; set; }

		public int diseaseCount { get; set; }

		public Orientation rotationOrientation { get; set; }

		public List<StorageItem> storage { get; set; }

		public Type type { get; set; }

		public int connections { get; set; }

		public Rottable rottable { get; set; }

		public template_amount_value[] amounts { get; set; }

		public template_amount_value[] other_values { get; set; }

		public Prefab()
		{
			type = Type.Other;
		}

		public Prefab(string _id, Type _type, int loc_x, int loc_y, SimHashes _element, float _temperature = -1f, float _units = 1f, string _disease = null, int _disease_count = 0, Orientation _rotation = Orientation.Neutral, template_amount_value[] _amount_values = null, template_amount_value[] _other_values = null, int _connections = 0)
		{
			id = _id;
			type = _type;
			location_x = loc_x;
			location_y = loc_y;
			connections = _connections;
			element = _element;
			temperature = _temperature;
			units = _units;
			diseaseName = _disease;
			diseaseCount = _disease_count;
			rotationOrientation = _rotation;
			if (_amount_values != null && _amount_values.Length != 0)
			{
				amounts = _amount_values;
			}
			if (_other_values != null && _other_values.Length != 0)
			{
				other_values = _other_values;
			}
		}

		public Prefab Clone(Vector2I offset)
		{
			Prefab prefab = new Prefab(id, type, offset.x + location_x, offset.y + location_y, element, temperature, units, diseaseName, diseaseCount, rotationOrientation, amounts, other_values, connections);
			if (rottable != null)
			{
				prefab.rottable = new Rottable();
				prefab.rottable.rotAmount = rottable.rotAmount;
			}
			if (storage != null && storage.Count > 0)
			{
				prefab.storage = new List<StorageItem>();
				{
					foreach (StorageItem item in storage)
					{
						prefab.storage.Add(item.Clone());
					}
					return prefab;
				}
			}
			return prefab;
		}

		public void AssignStorage(StorageItem _storage)
		{
			if (storage == null)
			{
				storage = new List<StorageItem>();
			}
			storage.Add(_storage);
		}
	}
}
