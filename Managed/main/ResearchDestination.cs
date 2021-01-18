using System.Collections.Generic;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class ResearchDestination : ClusterGridEntity
{
	[Serialize]
	private AxialI m_location;

	public override string Name => UI.SPACEDESTINATIONS.RESEARCHDESTINATION.NAME;

	public override EntityLayer Layer => EntityLayer.POI;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>();

	public override AxialI Location => m_location;

	public override bool IsVisible => false;

	public void Init(AxialI location)
	{
		m_location = location;
	}
}
