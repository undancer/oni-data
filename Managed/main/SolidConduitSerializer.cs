using System.Collections.Generic;
using System.IO;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitSerializer")]
public class SolidConduitSerializer : KMonoBehaviour, ISaveLoadableDetails
{
	protected override void OnPrefabInit()
	{
	}

	protected override void OnSpawn()
	{
	}

	public void Serialize(BinaryWriter writer)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		List<int> cells = solidConduitFlow.GetSOAInfo().Cells;
		int num = 0;
		for (int i = 0; i < cells.Count; i++)
		{
			int cell = cells[i];
			SolidConduitFlow.ConduitContents contents = solidConduitFlow.GetContents(cell);
			if (contents.pickupableHandle.IsValid() && (bool)solidConduitFlow.GetPickupable(contents.pickupableHandle))
			{
				num++;
			}
		}
		writer.Write(num);
		for (int j = 0; j < cells.Count; j++)
		{
			int num2 = cells[j];
			SolidConduitFlow.ConduitContents contents2 = solidConduitFlow.GetContents(num2);
			if (!contents2.pickupableHandle.IsValid())
			{
				continue;
			}
			Pickupable pickupable = solidConduitFlow.GetPickupable(contents2.pickupableHandle);
			if ((bool)pickupable)
			{
				writer.Write(num2);
				SaveLoadRoot component = pickupable.GetComponent<SaveLoadRoot>();
				if (component != null)
				{
					string name = pickupable.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
					writer.WriteKleiString(name);
					component.Save(writer);
				}
				else
				{
					Debug.Log("Tried to save obj in solid conduit but obj has no SaveLoadRoot", pickupable.gameObject);
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		SolidConduitFlow solidConduitFlow = Game.Instance.solidConduitFlow;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			int cell = reader.ReadInt32();
			Tag tag = TagManager.Create(reader.ReadKleiString());
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			if (saveLoadRoot != null)
			{
				Pickupable component = saveLoadRoot.GetComponent<Pickupable>();
				if (component != null)
				{
					solidConduitFlow.SetContents(cell, component);
				}
			}
			else
			{
				Debug.Log("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
			}
		}
	}
}
