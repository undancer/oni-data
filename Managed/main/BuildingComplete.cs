using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using UnityEngine;

public class BuildingComplete : Building
{
	[MyCmpReq]
	private Modifiers modifiers;

	[MyCmpGet]
	public KPrefabID prefabid;

	public bool isManuallyOperated;

	public bool isArtable;

	public PrimaryElement primaryElement;

	[Serialize]
	public float creationTime = -1f;

	private bool hasSpawnedKComponents;

	private ObjectLayer replacingTileLayer = ObjectLayer.NumLayers;

	public List<AttributeModifier> regionModifiers = new List<AttributeModifier>();

	private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnEntombedChange = new EventSystem.IntraObjectHandler<BuildingComplete>(delegate(BuildingComplete component, object data)
	{
		component.OnEntombedChanged();
	});

	private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<BuildingComplete>(delegate(BuildingComplete component, object data)
	{
		component.OnObjectReplaced(data);
	});

	private HandleVector<int>.Handle scenePartitionerEntry;

	public static float MinKelvinSeen = float.MaxValue;

	private bool WasReplaced()
	{
		return replacingTileLayer != ObjectLayer.NumLayers;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Rotatable component2 = GetComponent<Rotatable>();
		if (component != null && component2 == null)
		{
			component.Offset = Def.GetVisualizerOffset();
		}
		KBoxCollider2D component3 = GetComponent<KBoxCollider2D>();
		if (component3 != null)
		{
			Vector3 visualizerOffset = Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		Attributes attributes = this.GetAttributes();
		foreach (Klei.AI.Attribute attribute2 in Def.attributes)
		{
			attributes.Add(attribute2);
		}
		foreach (AttributeModifier attributeModifier in Def.attributeModifiers)
		{
			Klei.AI.Attribute attribute = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
			if (attributes.Get(attribute) == null)
			{
				attributes.Add(attribute);
			}
			attributes.Add(attributeModifier);
		}
		foreach (AttributeInstance item2 in attributes)
		{
			AttributeModifier item = new AttributeModifier(item2.Id, item2.GetTotalValue());
			regionModifiers.Add(item);
		}
		if (Def.UseStructureTemperature)
		{
			GameComps.StructureTemperatures.Add(base.gameObject);
		}
		Subscribe(1606648047, OnObjectReplacedDelegate);
		if (Def.Entombable)
		{
			Subscribe(-1089732772, OnEntombedChange);
		}
	}

	private void OnEntombedChanged()
	{
		if (base.gameObject.HasTag(GameTags.Entombed))
		{
			Components.EntombedBuildings.Add(this);
		}
		else
		{
			Components.EntombedBuildings.Remove(this);
		}
	}

	public override void UpdatePosition()
	{
		base.UpdatePosition();
		GameScenePartitioner.Instance.UpdatePosition(scenePartitionerEntry, GetExtents());
	}

	private void OnObjectReplaced(object data)
	{
		replacingTileLayer = (ObjectLayer)data;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		primaryElement = GetComponent<PrimaryElement>();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		if (Def.IsFoundation)
		{
			int[] array = base.PlacementCells;
			foreach (int num in array)
			{
				Grid.Foundation[num] = true;
				Game.Instance.roomProber.SolidChangedEvent(num, ignoreDoors: false);
			}
		}
		if (Grid.IsValidCell(cell))
		{
			Vector3 position = Grid.CellToPosCBC(cell, Def.SceneLayer);
			base.transform.SetPosition(position);
		}
		if (primaryElement != null)
		{
			if (primaryElement.Mass == 0f)
			{
				primaryElement.Mass = Def.Mass[0];
			}
			float temperature = primaryElement.Temperature;
			if (temperature > 0f && !float.IsNaN(temperature) && !float.IsInfinity(temperature))
			{
				MinKelvinSeen = Mathf.Min(MinKelvinSeen, temperature);
			}
			PrimaryElement obj = primaryElement;
			obj.setTemperatureCallback = (PrimaryElement.SetTemperatureCallback)Delegate.Combine(obj.setTemperatureCallback, new PrimaryElement.SetTemperatureCallback(OnSetTemperature));
		}
		if (!base.gameObject.HasTag(GameTags.RocketInSpace))
		{
			Def.MarkArea(cell, base.Orientation, Def.ObjectLayer, base.gameObject);
			if (Def.IsTilePiece)
			{
				Def.MarkArea(cell, base.Orientation, Def.TileLayer, base.gameObject);
				Def.RunOnArea(cell, base.Orientation, delegate(int c)
				{
					TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
				});
			}
		}
		RegisterBlockTileRenderer();
		if (Def.PreventIdleTraversalPastBuilding)
		{
			for (int j = 0; j < base.PlacementCells.Length; j++)
			{
				Grid.PreventIdleTraversal[base.PlacementCells[j]] = true;
			}
		}
		Components.BuildingCompletes.Add(this);
		BuildingConfigManager.Instance.AddBuildingCompleteKComponents(base.gameObject, Def.Tag);
		hasSpawnedKComponents = true;
		scenePartitionerEntry = GameScenePartitioner.Instance.Add(base.name, this, GetExtents(), GameScenePartitioner.Instance.completeBuildings, null);
		if (prefabid.HasTag(GameTags.TemplateBuilding))
		{
			Components.TemplateBuildings.Add(this);
		}
		Attributes attributes = this.GetAttributes();
		if (attributes != null)
		{
			Deconstructable component = GetComponent<Deconstructable>();
			if (component != null)
			{
				for (int k = 1; k < component.constructionElements.Length; k++)
				{
					Tag tag = component.constructionElements[k];
					Element element = ElementLoader.GetElement(tag);
					if (element != null)
					{
						foreach (AttributeModifier attributeModifier in element.attributeModifiers)
						{
							attributes.Add(attributeModifier);
						}
						continue;
					}
					GameObject gameObject = Assets.TryGetPrefab(tag);
					if (!(gameObject != null))
					{
						continue;
					}
					PrefabAttributeModifiers component2 = gameObject.GetComponent<PrefabAttributeModifiers>();
					if (!(component2 != null))
					{
						continue;
					}
					foreach (AttributeModifier descriptor in component2.descriptors)
					{
						attributes.Add(descriptor);
					}
				}
			}
		}
		BuildingInventory.Instance.RegisterBuilding(this);
	}

	private void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		MinKelvinSeen = Mathf.Min(MinKelvinSeen, temperature);
	}

	public void SetCreationTime(float time)
	{
		creationTime = time;
	}

	private string GetInspectSound()
	{
		return GlobalAssets.GetSound("AI_Inspect_" + GetComponent<KPrefabID>().PrefabTag.Name);
	}

	protected override void OnCleanUp()
	{
		if (Game.quitting)
		{
			return;
		}
		GameScenePartitioner.Instance.Free(ref scenePartitionerEntry);
		if (hasSpawnedKComponents)
		{
			BuildingConfigManager.Instance.DestroyBuildingCompleteKComponents(base.gameObject, Def.Tag);
		}
		if (Def.UseStructureTemperature)
		{
			GameComps.StructureTemperatures.Remove(base.gameObject);
		}
		base.OnCleanUp();
		if (!WasReplaced() && base.gameObject.GetMyWorldId() != ClusterManager.INVALID_WORLD_IDX)
		{
			int cell = Grid.PosToCell(this);
			Def.UnmarkArea(cell, base.Orientation, Def.ObjectLayer, base.gameObject);
			if (Def.IsTilePiece)
			{
				Def.UnmarkArea(cell, base.Orientation, Def.TileLayer, base.gameObject);
				Def.RunOnArea(cell, base.Orientation, delegate(int c)
				{
					TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
				});
			}
			if (Def.IsFoundation)
			{
				int[] array = base.PlacementCells;
				foreach (int num in array)
				{
					Grid.Foundation[num] = false;
					Game.Instance.roomProber.SolidChangedEvent(num, ignoreDoors: false);
				}
			}
			if (Def.PreventIdleTraversalPastBuilding)
			{
				for (int j = 0; j < base.PlacementCells.Length; j++)
				{
					Grid.PreventIdleTraversal[base.PlacementCells[j]] = false;
				}
			}
		}
		if (WasReplaced() && Def.IsTilePiece && replacingTileLayer != Def.TileLayer)
		{
			int cell2 = Grid.PosToCell(this);
			Def.UnmarkArea(cell2, base.Orientation, Def.TileLayer, base.gameObject);
			Def.RunOnArea(cell2, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
			});
		}
		Components.BuildingCompletes.Remove(this);
		Components.EntombedBuildings.Remove(this);
		Components.TemplateBuildings.Remove(this);
		UnregisterBlockTileRenderer();
		BuildingInventory.Instance.UnregisterBuilding(this);
		Trigger(-21016276, this);
	}
}
