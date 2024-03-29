using System.Collections.Generic;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class TelescopeTarget : ClusterGridEntity
{
	public override string Name => UI.SPACEDESTINATIONS.TELESCOPE_TARGET.NAME;

	public override EntityLayer Layer => EntityLayer.Telescope;

	public override List<AnimConfig> AnimConfigs => new List<AnimConfig>
	{
		new AnimConfig
		{
			animFile = Assets.GetAnim("telescope_target_kanim"),
			initialAnim = "idle"
		}
	};

	public override bool IsVisible => true;

	public override ClusterRevealLevel IsVisibleInFOW => ClusterRevealLevel.Visible;

	public void Init(AxialI location)
	{
		base.Location = location;
	}

	public override bool ShowName()
	{
		return true;
	}

	public override bool ShowProgressBar()
	{
		return true;
	}

	public override float GetProgress()
	{
		return SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().GetRevealCompleteFraction(base.Location);
	}
}
