using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/CO2")]
public class CO2 : KMonoBehaviour
{
	[NonSerialized]
	[Serialize]
	public Vector3 velocity = Vector3.zero;

	[NonSerialized]
	[Serialize]
	public float mass;

	[NonSerialized]
	[Serialize]
	public float temperature;

	[NonSerialized]
	[Serialize]
	public float lifetimeRemaining;

	public void StartLoop()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play("exhale_pre");
		component.Play("exhale_loop", KAnim.PlayMode.Loop);
	}

	public void TriggerDestroy()
	{
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Play("exhale_pst");
	}
}
