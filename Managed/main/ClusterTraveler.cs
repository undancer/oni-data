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

	protected override void OnSpawn()
	{
		Subscribe(543433792, ClusterDestinationChangedHandler);
		UpdateAnimationTags();
	}

	private void OnClusterDestinationChanged(object data)
	{
		if (m_destinationSelector.IsAtDestination())
		{
			m_movePotential = 0f;
			CurrentPath.Clear();
		}
	}

	public float TravelETA()
	{
		if (!IsTraveling())
		{
			return 0f;
		}
		return RemainingTravelDistance() / getSpeedCB();
	}

	public float RemainingTravelDistance()
	{
		return (float)RemainingTravelNodes() * 600f - m_movePotential;
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

	private void RevalidatePath()
	{
		if (HasCurrentPathChanged(out var reason))
		{
			m_destinationSelector.SetDestination(m_destinationSelector.GetMyWorldLocation());
			string message = MISC.NOTIFICATIONS.BADROCKETPATH.TOOLTIP;
			Notification notification = new Notification(MISC.NOTIFICATIONS.BADROCKETPATH.NAME, NotificationType.BadMinor, (List<Notification> notificationList, object data) => message + notificationList.ReduceMessages(countNames: false) + "\n\n" + reason);
			GetComponent<Notifier>().Add(notification);
		}
	}

	private bool HasCurrentPathChanged(out string reason)
	{
		List<AxialI> path = ClusterGrid.Instance.GetPath(m_clusterGridEntity.Location, m_cachedPathDestination, m_destinationSelector, out reason);
		if (path == null)
		{
			return true;
		}
		if (path.Count != m_cachedPath.Count)
		{
			return true;
		}
		for (int i = 0; i < m_cachedPath.Count; i++)
		{
			if (m_cachedPath[i] != path[i])
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
