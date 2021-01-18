using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Placeable")]
public class Placeable : KMonoBehaviour
{
	public Tag previewTag;
}
