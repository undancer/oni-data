using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FactionManager")]
public class FactionManager : KMonoBehaviour
{
	public enum FactionID
	{
		Duplicant,
		Friendly,
		Hostile,
		Prey,
		Predator,
		Pest,
		NumberOfFactions
	}

	public enum Disposition
	{
		Assist,
		Neutral,
		Attack
	}

	public static FactionManager Instance;

	public Faction Duplicant = new Faction(FactionID.Duplicant);

	public Faction Friendly = new Faction(FactionID.Friendly);

	public Faction Hostile = new Faction(FactionID.Hostile);

	public Faction Predator = new Faction(FactionID.Predator);

	public Faction Prey = new Faction(FactionID.Prey);

	public Faction Pest = new Faction(FactionID.Pest);

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public Faction GetFaction(FactionID faction)
	{
		return faction switch
		{
			FactionID.Duplicant => Duplicant, 
			FactionID.Friendly => Friendly, 
			FactionID.Hostile => Hostile, 
			FactionID.Predator => Predator, 
			FactionID.Prey => Prey, 
			FactionID.Pest => Pest, 
			_ => null, 
		};
	}

	public Disposition GetDisposition(FactionID of_faction, FactionID to_faction)
	{
		if (Instance.GetFaction(of_faction).Dispositions.ContainsKey(to_faction))
		{
			return Instance.GetFaction(of_faction).Dispositions[to_faction];
		}
		return Disposition.Neutral;
	}
}
