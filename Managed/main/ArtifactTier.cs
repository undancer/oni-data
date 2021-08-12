public class ArtifactTier
{
	public EffectorValues decorValues;

	public StringKey name_key;

	public float payloadDropChance;

	public ArtifactTier(StringKey str_key, EffectorValues values, float payload_drop_chance)
	{
		decorValues = values;
		name_key = str_key;
		payloadDropChance = payload_drop_chance;
	}
}
