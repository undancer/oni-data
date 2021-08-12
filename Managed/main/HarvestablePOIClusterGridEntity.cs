using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class HarvestablePOIClusterGridEntity : ClusterGridEntity
{
	public string m_name;

	public string m_Anim;

	public override string Name => m_name;

	public override EntityLayer Layer => EntityLayer.POI;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("harvestable_space_poi_kanim"),
			initialAnim = (m_Anim.IsNullOrWhiteSpace() ? "cloud" : m_Anim)
		}
	};

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Peeked;

	public void Init(AxialI location)
	{
		base.Location = location;
	}
}
