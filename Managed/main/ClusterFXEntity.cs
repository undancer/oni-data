using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ClusterFXEntity : ClusterGridEntity
{
	[SerializeField]
	public string kAnimName;

	[SerializeField]
	public string animName;

	public KAnim.PlayMode animPlayMode = KAnim.PlayMode.Once;

	public Vector3 animOffset;

	public override string Name => UI.SPACEDESTINATIONS.TELESCOPE_TARGET.NAME;

	public override EntityLayer Layer => EntityLayer.FX;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim(kAnimName),
			initialAnim = animName,
			playMode = animPlayMode,
			animOffset = animOffset
		}
	};

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

	public void Init(AxialI location, Vector3 animOffset)
	{
		base.Location = location;
		this.animOffset = animOffset;
	}
}
