using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptOut)]
[AddComponentMenu("KMonoBehaviour/scripts/UnstableGround")]
public class UnstableGround : KMonoBehaviour
{
	public SimHashes element;

	public float mass;

	public float temperature;

	public byte diseaseIdx;

	public int diseaseCount;
}
