using System.Collections.Generic;
using KSerialization;
using ProcGen;
using UnityEngine;

public abstract class ClusterGridEntity : KMonoBehaviour
{
	public struct AnimConfig
	{
		public KAnimFile animFile;

		public string initialAnim;
	}

	[Serialize]
	protected AxialI m_location;

	[MyCmpGet]
	private KSelectable m_selectable;

	[MyCmpReq]
	private Transform m_transform;

	public bool isWorldEntity;

	public abstract string Name
	{
		get;
	}

	public abstract EntityLayer Layer
	{
		get;
	}

	public abstract List<AnimConfig> AnimConfigs
	{
		get;
	}

	public abstract bool IsVisible
	{
		get;
	}

	public abstract ClusterRevealLevel IsVisibleInFOW
	{
		get;
	}

	public AxialI Location
	{
		get
		{
			return m_location;
		}
		set
		{
			if (value != m_location)
			{
				AxialI location = m_location;
				m_location = value;
				SendClusterLocationChangedEvent(location, m_location);
			}
		}
	}

	public virtual bool ShowName()
	{
		return false;
	}

	public virtual bool ShowProgressBar()
	{
		return false;
	}

	public virtual float GetProgress()
	{
		return 0f;
	}

	public virtual bool SpaceOutInSameHex()
	{
		return false;
	}

	public virtual bool ShowPath()
	{
		return true;
	}

	protected override void OnSpawn()
	{
		ClusterGrid.Instance.RegisterEntity(this);
		if (m_selectable != null)
		{
			m_selectable.SetName(Name);
		}
		if (!isWorldEntity)
		{
			m_transform.SetLocalPosition(new Vector3(-1f, 0f, 0f));
		}
	}

	protected override void OnCleanUp()
	{
		ClusterGrid.Instance.UnregisterEntity(this);
	}

	public virtual Sprite GetUISprite()
	{
		if (DlcManager.IsExpansion1Active())
		{
			List<AnimConfig> animConfigs = AnimConfigs;
			if (animConfigs.Count > 0)
			{
				return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile);
			}
		}
		else
		{
			WorldContainer component = GetComponent<WorldContainer>();
			if (component != null)
			{
				ProcGen.World worldData = SettingsCache.worlds.GetWorldData(component.worldName);
				return (worldData != null) ? Assets.GetSprite(worldData.asteroidIcon) : null;
			}
		}
		return null;
	}

	public void SendClusterLocationChangedEvent(AxialI oldLocation, AxialI newLocation)
	{
		ClusterLocationChangedEvent clusterLocationChangedEvent = default(ClusterLocationChangedEvent);
		clusterLocationChangedEvent.entity = this;
		clusterLocationChangedEvent.oldLocation = oldLocation;
		clusterLocationChangedEvent.newLocation = newLocation;
		ClusterLocationChangedEvent clusterLocationChangedEvent2 = clusterLocationChangedEvent;
		Trigger(-1298331547, clusterLocationChangedEvent2);
		Game.Instance.Trigger(-1298331547, clusterLocationChangedEvent2);
	}
}
