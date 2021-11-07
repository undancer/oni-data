using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/OrbitalMechanics")]
public class OrbitalMechanics : KMonoBehaviour
{
	[Serialize]
	private List<Ref<OrbitalObject>> orbitingObjects = new List<Ref<OrbitalObject>>();

	private EventSystem.IntraObjectHandler<OrbitalMechanics> OnClusterLocationChangedDelegate = new EventSystem.IntraObjectHandler<OrbitalMechanics>(delegate(OrbitalMechanics cmp, object data)
	{
		cmp.OnClusterLocationChanged(data);
	});

	protected override void OnPrefabInit()
	{
		Subscribe(-1298331547, OnClusterLocationChangedDelegate);
	}

	private void OnClusterLocationChanged(object data)
	{
		UpdateLocation(((ClusterLocationChangedEvent)data).newLocation);
	}

	protected override void OnCleanUp()
	{
		if (orbitingObjects == null)
		{
			return;
		}
		foreach (Ref<OrbitalObject> orbitingObject in orbitingObjects)
		{
			if (!orbitingObject.Get().IsNullOrDestroyed())
			{
				Util.KDestroyGameObject(orbitingObject.Get());
			}
		}
	}

	[ContextMenu("Rebuild")]
	private void Rebuild()
	{
		List<string> list = new List<string>();
		if (orbitingObjects != null)
		{
			foreach (Ref<OrbitalObject> orbitingObject in orbitingObjects)
			{
				if (!orbitingObject.Get().IsNullOrDestroyed())
				{
					list.Add(orbitingObject.Get().orbitalDBId);
					Util.KDestroyGameObject(orbitingObject.Get());
				}
			}
			orbitingObjects = new List<Ref<OrbitalObject>>();
		}
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				CreateOrbitalObject(list[i]);
			}
		}
	}

	private void UpdateLocation(AxialI location)
	{
		if (orbitingObjects.Count > 0)
		{
			foreach (Ref<OrbitalObject> orbitingObject in orbitingObjects)
			{
				if (!orbitingObject.Get().IsNullOrDestroyed())
				{
					Util.KDestroyGameObject(orbitingObject.Get());
				}
			}
			orbitingObjects = new List<Ref<OrbitalObject>>();
		}
		ClusterGridEntity visibleEntityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(location, EntityLayer.POI);
		if (visibleEntityOfLayerAtCell != null)
		{
			ArtifactPOIClusterGridEntity component = visibleEntityOfLayerAtCell.GetComponent<ArtifactPOIClusterGridEntity>();
			if (component != null)
			{
				ArtifactPOIStates.Instance sMI = component.GetSMI<ArtifactPOIStates.Instance>();
				if (sMI != null && sMI.configuration.poiType.orbitalObject != null)
				{
					foreach (string item in sMI.configuration.poiType.orbitalObject)
					{
						CreateOrbitalObject(item);
					}
				}
			}
			HarvestablePOIClusterGridEntity component2 = visibleEntityOfLayerAtCell.GetComponent<HarvestablePOIClusterGridEntity>();
			if (!(component2 != null))
			{
				return;
			}
			HarvestablePOIStates.Instance sMI2 = component2.GetSMI<HarvestablePOIStates.Instance>();
			if (sMI2 != null && sMI2.configuration.poiType.orbitalObject != null)
			{
				List<string> orbitalObject = sMI2.configuration.poiType.orbitalObject;
				System.Random random = new System.Random();
				float num = sMI2.poiCapacity / sMI2.configuration.GetMaxCapacity() * (float)sMI2.configuration.poiType.maxNumOrbitingObjects;
				for (int i = 0; (float)i < num; i++)
				{
					int index = random.Next(orbitalObject.Count);
					CreateOrbitalObject(orbitalObject[index]);
				}
			}
			return;
		}
		Clustercraft component3 = GetComponent<Clustercraft>();
		if (component3 != null)
		{
			if (component3.GetOrbitAsteroid() != null || component3.Status == Clustercraft.CraftStatus.Launching)
			{
				CreateOrbitalObject(Db.Get().OrbitalTypeCategories.orbit.Id);
			}
			else if (component3.Status == Clustercraft.CraftStatus.Landing)
			{
				CreateOrbitalObject(Db.Get().OrbitalTypeCategories.landed.Id);
			}
		}
	}

	public void CreateOrbitalObject(string orbit_db_name)
	{
		WorldContainer component = GetComponent<WorldContainer>();
		GameObject obj = Util.KInstantiate(Assets.GetPrefab(OrbitalBGConfig.ID), base.gameObject);
		OrbitalObject component2 = obj.GetComponent<OrbitalObject>();
		component2.Init(orbit_db_name, component, orbitingObjects);
		obj.SetActive(value: true);
		orbitingObjects.Add(new Ref<OrbitalObject>(component2));
	}
}
