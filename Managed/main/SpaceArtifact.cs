using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SpaceArtifact")]
public class SpaceArtifact : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public const string ID = "SpaceArtifact";

	[SerializeField]
	private string ui_anim;

	[SerializeField]
	private ArtifactTier artifactTier;

	public void SetArtifactTier(ArtifactTier tier)
	{
		artifactTier = tier;
	}

	public ArtifactTier GetArtifactTier()
	{
		return artifactTier;
	}

	public void SetUIAnim(string anim)
	{
		ui_anim = anim;
	}

	public string GetUIAnim()
	{
		return ui_anim;
	}

	public List<Descriptor> GetEffectDescriptions()
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = new Descriptor($"This is an artifact from space", $"This is the tooltip string", Descriptor.DescriptorType.Information);
		list.Add(item);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}
}
