using System.Collections.Generic;
using UnityEngine;

public abstract class ClusterGridEntity : KMonoBehaviour
{
	public struct AnimConfig
	{
		public KAnimFile animFile;

		public string initialAnim;
	}

	[MyCmpGet]
	private KSelectable m_selectable;

	[MyCmpReq]
	private Transform m_transform;

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

	public abstract AxialI Location
	{
		get;
	}

	public abstract bool IsVisible
	{
		get;
	}

	protected override void OnSpawn()
	{
		ClusterGrid.Instance.RegisterEntity(this);
		if (m_selectable != null)
		{
			m_selectable.SetName(Name);
		}
		m_transform.SetLocalPosition(new Vector3(-1f, 0f, 0f));
	}

	protected override void OnCleanUp()
	{
		ClusterGrid.Instance.UnregisterEntity(this);
	}

	public Sprite GetUISprite()
	{
		List<AnimConfig> animConfigs = AnimConfigs;
		if (animConfigs.Count > 0)
		{
			return Def.GetUISpriteFromMultiObjectAnim(animConfigs[0].animFile);
		}
		return null;
	}
}
