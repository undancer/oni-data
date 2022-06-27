using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ClusterTraveler : KMonoBehaviour, ISim200ms
{
	[MyCmpReq]
	private ClusterDestinationSelector m_destinationSelector;

	[MyCmpReq]
	private ClusterGridEntity m_clusterGridEntity;

	[Serialize]
	private float m_movePotential;

	public Func<float> getSpeedCB;

	public Func<bool, bool> getCanTravelCB;

	public Func<AxialI, bool> validateTravelCB;

	public System.Action onTravelCB;

	private AxialI m_cachedPathDestination;

	private List<AxialI> m_cachedPath;

	private bool m_isPathDirty;

	public bool stopAndNotifyWhenPathChanges;

	private static EventSystem.IntraObjectHandler<ClusterTraveler> ClusterDestinationChangedHandler = new EventSystem.IntraObjectHandler<ClusterTraveler>(delegate(ClusterTraveler cmp, object data)
	{
		cmp.OnClusterDestinationChanged(data);
	});

	public List<AxialI> CurrentPath
	{
		get
		{
			if (m_cachedPath == null || m_destinationSelector.GetDestination() != m_cachedPathDestination)
			{
				m_cachedPathDestination = m_destinationSelector.GetDestination();
				m_cachedPath = ClusterGrid.Instance.GetPath(m_clusterGridEntity.Location, m_cachedPathDestination, m_destinationSelector);
			}
			return m_cachedPath;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.ClusterTravelers.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.ClusterTravelers.Remove(this);
		Game.Instance.Unsubscribe(-1991583975, OnClusterFogOfWarRevealed);
		base.OnCleanUp();
	}

	private void ForceRevealLocation(AxialI location)
	{
		if (!ClusterGrid.Instance.IsCellVisible(location))
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(location);
		}
	}

	protected override void OnSpawn()
	{
		Subscribe(543433792, ClusterDestinationChangedHandler);
		Game.Instance.Subscribe(-1991583975, OnClusterFogOfWarRevealed);
		UpdateAnimationTags();
		MarkPathDirty();
		RevalidatePath(react_to_change: false);
		ForceRevealLocation(m_clusterGridEntity.Location);
	}

	private void MarkPathDirty()
	{
		m_isPathDirty = true;
	}

	private void OnClusterFogOfWarRevealed(object data)
	{
		MarkPathDirty();
	}

	private void OnClusterDestinationChanged(object data)
	{
		if (m_destinationSelector.IsAtDestination())
		{
			m_movePotential = 0f;
			if (CurrentPath != null)
			{
				CurrentPath.Clear();
			}
		}
		MarkPathDirty();
	}

	public int GetDestinationWorldID()
	{
		return m_destinationSelector.GetDestinationWorld();
	}

	public float TravelETA()
	{
		if (!IsTraveling() || getSpeedCB == null)
		{
			return 0f;
		}
		return RemainingTravelDistance() / getSpeedCB();
	}

	public float RemainingTravelDistance()
	{
		int num = RemainingTravelNodes();
		if (GetDestinationWorldID() >= 0)
		{
			num--;
			num = Mathf.Max(num, 0);
		}
		return (float)num * 600f - m_movePotential;
	}

	public int RemainingTravelNodes()
	{
		int count = CurrentPath.Count;
		return Mathf.Max(0, count);
	}

	public float GetMoveProgress()
	{
		return m_movePotential / 600f;
	}

	public bool IsTraveling()
	{
		return !m_destinationSelector.IsAtDestination();
	}

	public void Sim200ms(float dt)
	{
		if (!IsTraveling())
		{
			return;
		}
		bool flag = CurrentPath != null && CurrentPath.Count > 0;
		bool flag2 = m_destinationSelector.HasAsteroidDestination();
		bool arg = flag2 && flag && CurrentPath.Count == 1;
		if (getCanTravelCB != null && !getCanTravelCB(arg))
		{
			return;
		}
		_ = m_clusterGridEntity.Location;
		if (flag)
		{
			if (flag2)
			{
				_ = m_destinationSelector.requireLaunchPadOnAsteroidDestination;
			}
			else
				_ = 0;
			if (!flag2 || CurrentPath.Count > 1)
			{
				float num = dt * getSpeedCB();
				m_movePotential += num;
				if (m_movePotential >= 600f)
				{
					m_movePotential = 600f;
					if (AdvancePathOneStep())
					{
						Debug.Assert(ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(m_clusterGridEntity.Location, EntityLayer.Asteroid) == null, $"Somehow this clustercraft pathed through an asteroid at {m_clusterGridEntity.Location}");
						m_movePotential -= 600f;
						if (onTravelCB != null)
						{
							onTravelCB();
						}
					}
				}
			}
			else
			{
				AdvancePathOneStep();
			}
		}
		RevalidatePath();
	}

	public bool AdvancePathOneStep()
	{
		if (validateTravelCB != null && !validateTravelCB(CurrentPath[0]))
		{
			return false;
		}
		AxialI location = CurrentPath[0];
		CurrentPath.RemoveAt(0);
		ForceRevealLocation(location);
		m_clusterGridEntity.Location = location;
		UpdateAnimationTags();
		return true;
	}

	private void UpdateAnimationTags()
	{
		if (CurrentPath == null)
		{
			m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
			m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
			m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
		}
		else if (ClusterGrid.Instance.GetAsteroidAtCell(m_clusterGridEntity.Location) != null)
		{
			if (CurrentPath.Count == 0 || m_clusterGridEntity.Location == CurrentPath[CurrentPath.Count - 1])
			{
				m_clusterGridEntity.AddTag(GameTags.BallisticEntityLanding);
				m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
				m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
			}
			else
			{
				m_clusterGridEntity.AddTag(GameTags.BallisticEntityLaunching);
				m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
				m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityMoving);
			}
		}
		else
		{
			m_clusterGridEntity.AddTag(GameTags.BallisticEntityMoving);
			m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLanding);
			m_clusterGridEntity.RemoveTag(GameTags.BallisticEntityLaunching);
		}
	}

	public void RevalidatePath(bool react_to_change = true)
	{
		if (!HasCurrentPathChanged(out var reason, out var updatedPath))
		{
			return;
		}
		if (stopAndNotifyWhenPathChanges && react_to_change)
		{
			m_destinationSelector.SetDestination(m_destinationSelector.GetMyWorldLocation());
			string message = MISC.NOTIFICATIONS.BADROCKETPATH.TOOLTIP;
			Notification notification = new Notification(MISC.NOTIFICATIONS.BADROCKETPATH.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => message + notificationList.ReduceMessages(countNames: false) + "\n\n" + reason);
			GetComponent<Notifier>().Add(notification);
		}
		else
		{
			m_cachedPath = updatedPath;
		}
	}

	private bool HasCurrentPathChanged(out string reason, out List<AxialI> updatedPath)
	{
		if (!m_isPathDirty)
		{
			reason = null;
			updatedPath = null;
			return false;
		}
		m_isPathDirty = false;
		updatedPath = ClusterGrid.Instance.GetPath(m_clusterGridEntity.Location, m_cachedPathDestination, m_destinationSelector, out reason);
		if (updatedPath == null)
		{
			return true;
		}
		if (updatedPath.Count != m_cachedPath.Count)
		{
			return true;
		}
		for (int i = 0; i < m_cachedPath.Count; i++)
		{
			if (m_cachedPath[i] != updatedPath[i])
			{
				return true;
			}
		}
		return false;
	}

	[ContextMenu("Fill Move Potential")]
	public void FillMovePotential()
	{
		m_movePotential = 600f;
	}
}
