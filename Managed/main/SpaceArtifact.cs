using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpaceArtifact")]
public class SpaceArtifact : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public const string ID = "SpaceArtifact";

	private const string charmedPrefix = "entombed_";

	private const string idlePrefix = "idle_";

	[SerializeField]
	private string ui_anim;

	[Serialize]
	private bool loadCharmed = true;

	public ArtifactTier artifactTier;

	public string uniqueAnimNameFragment;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (loadCharmed && DlcManager.IsExpansion1Active())
		{
			base.gameObject.AddTag(GameTags.CharmedArtifact);
		}
		else
		{
			loadCharmed = false;
		}
		UpdateStatusItem();
		Components.SpaceArtifacts.Add(this);
		UpdateAnim();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.SpaceArtifacts.Remove(this);
	}

	public void RemoveCharm()
	{
		base.gameObject.RemoveTag(GameTags.CharmedArtifact);
		UpdateStatusItem();
		loadCharmed = false;
		UpdateAnim();
	}

	public void UpdateStatusItem()
	{
		if (base.gameObject.HasTag(GameTags.CharmedArtifact))
		{
			base.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed);
		}
		else
		{
			base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.ArtifactEntombed);
		}
	}

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
		if (base.gameObject.HasTag(GameTags.CharmedArtifact))
		{
			Descriptor item = new Descriptor(BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE.Replace("{chance}", GameUtil.GetFormattedPercent(artifactTier.payloadDropChance * 100f)), BUILDINGS.PREFABS.ARTIFACTANALYSISSTATION.PAYLOAD_DROP_RATE_TOOLTIP.Replace("{chance}", GameUtil.GetFormattedPercent(artifactTier.payloadDropChance * 100f)));
			list.Add(item);
		}
		Descriptor item2 = new Descriptor($"This is an artifact from space", $"This is the tooltip string", Descriptor.DescriptorType.Information);
		list.Add(item2);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		return GetEffectDescriptions();
	}

	private void UpdateAnim()
	{
		string s = ((!base.gameObject.HasTag(GameTags.CharmedArtifact)) ? uniqueAnimNameFragment : ("entombed_" + uniqueAnimNameFragment.Replace("idle_", "")));
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play(s, KAnim.PlayMode.Loop);
	}
}
