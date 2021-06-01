using System;
using System.Collections.Generic;
using FMOD.Studio;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Comet")]
public class Comet : KMonoBehaviour, ISim33ms
{
	public SimHashes EXHAUST_ELEMENT = SimHashes.CarbonDioxide;

	public float EXHAUST_RATE = 50f;

	public Vector2 spawnVelocity = new Vector2(12f, 15f);

	public Vector2 spawnAngle = new Vector2(-100f, -80f);

	public Vector2 massRange;

	public Vector2 temperatureRange;

	public SpawnFXHashes explosionEffectHash;

	public int splashRadius = 1;

	public int addTiles = 0;

	public int addTilesMinHeight;

	public int addTilesMaxHeight;

	public int entityDamage = 1;

	public float totalTileDamage = 0.2f;

	private float addTileMass;

	public int addDiseaseCount;

	public byte diseaseIdx = byte.MaxValue;

	public Vector2 elementReplaceTileTemperatureRange = new Vector2(800f, 1000f);

	public Vector2I explosionOreCount = new Vector2I(0, 0);

	private float explosionMass;

	public Vector2 explosionTemperatureRange = new Vector2(500f, 700f);

	public Vector2 explosionSpeedRange = new Vector2(8f, 14f);

	public float windowDamageMultiplier = 5f;

	public float bunkerDamageMultiplier = 0f;

	public string impactSound;

	public string flyingSound;

	public int flyingSoundID;

	private HashedString FLYING_SOUND_ID_PARAMETER = "meteorType";

	[Serialize]
	protected Vector2 velocity;

	[Serialize]
	private float remainingTileDamage;

	private Vector3 previousPosition;

	private bool hasExploded = false;

	public bool canHitDuplicants = false;

	public string[] craterPrefabs = null;

	public bool destroyOnExplode = true;

	private float age = 0f;

	public System.Action OnImpact;

	public Ref<KPrefabID> ignoreObstacleForDamage = new Ref<KPrefabID>();

	private LoopingSounds loopingSounds;

	private List<GameObject> damagedEntities = new List<GameObject>();

	private List<int> destroyedCells = new List<int>();

	private const float MAX_DISTANCE_TEST = 6f;

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	private float GetVolume(GameObject gameObject)
	{
		float result = 1f;
		if (gameObject != null && gameObject.GetComponent<KSelectable>() != null && gameObject.GetComponent<KSelectable>().IsSelected)
		{
			result = 1f;
		}
		return result;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		remainingTileDamage = totalTileDamage;
		loopingSounds = base.gameObject.GetComponent<LoopingSounds>();
		flyingSound = GlobalAssets.GetSound("Meteor_LP");
		RandomizeVelocity();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		RandomizeMassAndTemperature();
		StartLoopingSound();
	}

	public virtual void RandomizeVelocity()
	{
		float num = UnityEngine.Random.Range(spawnAngle.x, spawnAngle.y);
		float f = num * (float)Math.PI / 180f;
		float num2 = UnityEngine.Random.Range(spawnVelocity.x, spawnVelocity.y);
		velocity = new Vector2((0f - Mathf.Cos(f)) * num2, Mathf.Sin(f) * num2);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		component.Rotation = 0f - num - 90f;
	}

	public void RandomizeMassAndTemperature()
	{
		float num = UnityEngine.Random.Range(massRange.x, massRange.y);
		PrimaryElement component = GetComponent<PrimaryElement>();
		component.Mass = num;
		component.Temperature = UnityEngine.Random.Range(temperatureRange.x, temperatureRange.y);
		if (addTiles > 0)
		{
			float num2 = UnityEngine.Random.Range(0.95f, 0.98f);
			explosionMass = num * (1f - num2);
			addTileMass = num * num2;
		}
		else
		{
			explosionMass = num;
			addTileMass = 0f;
		}
	}

	[ContextMenu("Explode")]
	private void Explode(Vector3 pos, int cell, int prev_cell, Element element)
	{
		PlayImpactSound(pos);
		Vector3 pos2 = pos;
		pos2.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
		Game.Instance.SpawnFX(explosionEffectHash, pos2, 0f);
		Substance substance = element.substance;
		int num = UnityEngine.Random.Range(explosionOreCount.x, explosionOreCount.y + 1);
		Vector2 a = -velocity.normalized;
		Vector2 a2 = new Vector2(a.y, 0f - a.x);
		ListPool<ScenePartitionerEntry, Comet>.PooledList pooledList = ListPool<ScenePartitionerEntry, Comet>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)pos.x - 3, (int)pos.y - 3, 6, 6, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry item2 in pooledList)
		{
			GameObject gameObject = (item2.obj as Pickupable).gameObject;
			if (!(gameObject.GetComponent<MinionIdentity>() != null) && gameObject.GetDef<CreatureFallMonitor.Def>() == null)
			{
				Vector2 normalized = ((Vector2)(gameObject.transform.GetPosition() - pos)).normalized;
				normalized += new Vector2(0f, 0.55f);
				normalized *= 0.5f * UnityEngine.Random.Range(explosionSpeedRange.x, explosionSpeedRange.y);
				if (GameComps.Fallers.Has(gameObject))
				{
					GameComps.Fallers.Remove(gameObject);
				}
				if (GameComps.Gravities.Has(gameObject))
				{
					GameComps.Gravities.Remove(gameObject);
				}
				GameComps.Fallers.Add(gameObject, normalized);
			}
		}
		pooledList.Recycle();
		int num2 = splashRadius + 1;
		for (int i = -num2; i <= num2; i++)
		{
			for (int j = -num2; j <= num2; j++)
			{
				int num3 = Grid.OffsetCell(cell, j, i);
				if (Grid.IsValidCell(num3) && !destroyedCells.Contains(num3))
				{
					float num4 = (1f - (float)Mathf.Abs(j) / (float)num2) * (1f - (float)Mathf.Abs(i) / (float)num2);
					if (num4 > 0f)
					{
						DamageTiles(num3, prev_cell, num4 * totalTileDamage * 0.5f);
					}
				}
			}
		}
		float mass = ((num > 0) ? (explosionMass / (float)num) : 1f);
		float temperature = UnityEngine.Random.Range(explosionTemperatureRange.x, explosionTemperatureRange.y);
		for (int k = 0; k < num; k++)
		{
			Vector2 normalized2 = (a + a2 * UnityEngine.Random.Range(-1f, 1f)).normalized;
			Vector3 v = normalized2 * UnityEngine.Random.Range(explosionSpeedRange.x, explosionSpeedRange.y);
			Vector3 position = normalized2.normalized * 0.75f;
			position += new Vector3(0f, 0.55f, 0f);
			position += pos;
			GameObject go = substance.SpawnResource(position, mass, temperature, byte.MaxValue, 0);
			if (GameComps.Fallers.Has(go))
			{
				GameComps.Fallers.Remove(go);
			}
			GameComps.Fallers.Add(go, v);
		}
		if (addTiles > 0)
		{
			int depthOfElement = GetDepthOfElement(cell, element);
			float num5 = 1f;
			float num6 = (float)(depthOfElement - addTilesMinHeight) / (float)(addTilesMaxHeight - addTilesMinHeight);
			if (!float.IsNaN(num6))
			{
				num5 -= num6;
			}
			int num7 = Mathf.Min(addTiles, Mathf.Clamp(Mathf.RoundToInt((float)addTiles * num5), 1, addTiles));
			HashSetPool<int, Comet>.PooledHashSet pooledHashSet = HashSetPool<int, Comet>.Allocate();
			HashSetPool<int, Comet>.PooledHashSet pooledHashSet2 = HashSetPool<int, Comet>.Allocate();
			QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue = QueuePool<GameUtil.FloodFillInfo, Comet>.Allocate();
			int num8 = -1;
			int num9 = 1;
			if (velocity.x < 0f)
			{
				num8 *= -1;
				num9 *= -1;
			}
			GameUtil.FloodFillInfo item = new GameUtil.FloodFillInfo
			{
				cell = prev_cell,
				depth = 0
			};
			pooledQueue.Enqueue(item);
			item = new GameUtil.FloodFillInfo
			{
				cell = Grid.OffsetCell(prev_cell, new CellOffset(num8, 0)),
				depth = 0
			};
			pooledQueue.Enqueue(item);
			item = new GameUtil.FloodFillInfo
			{
				cell = Grid.OffsetCell(prev_cell, new CellOffset(num9, 0)),
				depth = 0
			};
			pooledQueue.Enqueue(item);
			GameUtil.FloodFillConditional(pooledQueue, SpawnTilesCellTest, pooledHashSet2, pooledHashSet, 10);
			float mass2 = ((num7 > 0) ? (addTileMass / (float)addTiles) : 1f);
			int disease_count = addDiseaseCount / num7;
			if (element.HasTag(GameTags.Unstable))
			{
				UnstableGroundManager component = World.Instance.GetComponent<UnstableGroundManager>();
				foreach (int item3 in pooledHashSet)
				{
					if (num7 <= 0)
					{
						break;
					}
					component.Spawn(item3, element, mass2, temperature, byte.MaxValue, 0);
					num7--;
				}
			}
			else
			{
				foreach (int item4 in pooledHashSet)
				{
					if (num7 <= 0)
					{
						break;
					}
					SimMessages.AddRemoveSubstance(item4, element.id, CellEventLogger.Instance.ElementEmitted, mass2, temperature, diseaseIdx, disease_count);
					num7--;
				}
			}
			pooledHashSet.Recycle();
			pooledHashSet2.Recycle();
			pooledQueue.Recycle();
		}
		SpawnCraterPrefabs();
		if (OnImpact != null)
		{
			OnImpact();
		}
	}

	protected virtual void SpawnCraterPrefabs()
	{
		if (craterPrefabs != null && craterPrefabs.Length != 0)
		{
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(craterPrefabs[UnityEngine.Random.Range(0, craterPrefabs.Length)]), Grid.CellToPos(Grid.PosToCell(this)));
			gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -19.5f);
			gameObject.SetActive(value: true);
		}
	}

	private int GetDepthOfElement(int cell, Element element)
	{
		int num = 0;
		int num2 = Grid.CellBelow(cell);
		while (Grid.IsValidCell(num2) && Grid.Element[num2] == element)
		{
			num++;
			num2 = Grid.CellBelow(num2);
		}
		return num;
	}

	private bool SpawnTilesCellTest(int cell)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell];
	}

	[ContextMenu("DamageTiles")]
	private float DamageTiles(int cell, int prev_cell, float input_damage)
	{
		GameObject gameObject = Grid.Objects[cell, 9];
		float num = 1f;
		bool flag = false;
		if (gameObject != null)
		{
			if (gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Window))
			{
				num = windowDamageMultiplier;
			}
			else if (gameObject.GetComponent<KPrefabID>().HasTag(GameTags.Bunker))
			{
				num = bunkerDamageMultiplier;
				if (gameObject.GetComponent<Door>() != null)
				{
					Game.Instance.savedInfo.blockedCometWithBunkerDoor = true;
				}
			}
			SimCellOccupier component = gameObject.GetComponent<SimCellOccupier>();
			if (component != null && !component.doReplaceElement)
			{
				flag = true;
			}
		}
		Element element = ((!flag) ? Grid.Element[cell] : gameObject.GetComponent<PrimaryElement>().Element);
		if (element.strength == 0f)
		{
			return 0f;
		}
		float num2 = input_damage * num / element.strength;
		PlayTileDamageSound(element, Grid.CellToPos(cell), gameObject);
		if (num2 == 0f)
		{
			return 0f;
		}
		float num3;
		if (flag)
		{
			BuildingHP component2 = gameObject.GetComponent<BuildingHP>();
			float a = (float)component2.HitPoints / (float)component2.MaxHitPoints;
			float f = num2 * (float)component2.MaxHitPoints;
			component2.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = Mathf.RoundToInt(f),
				source = BUILDINGS.DAMAGESOURCES.COMET,
				popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
			});
			num3 = Mathf.Min(a, num2);
		}
		else
		{
			num3 = WorldDamage.Instance.ApplyDamage(cell, num2, prev_cell, BUILDINGS.DAMAGESOURCES.COMET, UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET);
		}
		destroyedCells.Add(cell);
		float num4 = num3 / num2;
		return input_damage * (1f - num4);
	}

	private void DamageThings(Vector3 pos, int cell, int damage, GameObject ignoreObject = null)
	{
		if (!Grid.IsValidCell(cell))
		{
			return;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null && gameObject != ignoreObject)
		{
			BuildingHP component = gameObject.GetComponent<BuildingHP>();
			Building component2 = gameObject.GetComponent<Building>();
			if (component != null && !damagedEntities.Contains(gameObject))
			{
				KPrefabID component3 = gameObject.GetComponent<KPrefabID>();
				float f = (component3.HasTag(GameTags.Bunker) ? ((float)damage * bunkerDamageMultiplier) : ((float)damage));
				if (component2 != null && component2.Def != null)
				{
					PlayBuildingDamageSound(component2.Def, Grid.CellToPos(cell), gameObject);
				}
				component.gameObject.Trigger(-794517298, new BuildingHP.DamageSourceInfo
				{
					damage = Mathf.RoundToInt(f),
					source = BUILDINGS.DAMAGESOURCES.COMET,
					popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
				});
				damagedEntities.Add(gameObject);
			}
		}
		ListPool<ScenePartitionerEntry, Comet>.PooledList pooledList = ListPool<ScenePartitionerEntry, Comet>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)pos.x, (int)pos.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
		foreach (ScenePartitionerEntry item in pooledList)
		{
			Pickupable pickupable = item.obj as Pickupable;
			Health component4 = pickupable.GetComponent<Health>();
			if (component4 != null && !damagedEntities.Contains(pickupable.gameObject))
			{
				KPrefabID component5 = pickupable.GetComponent<KPrefabID>();
				float amount = (component5.HasTag(GameTags.Bunker) ? ((float)damage * bunkerDamageMultiplier) : ((float)damage));
				component4.Damage(amount);
				damagedEntities.Add(pickupable.gameObject);
			}
		}
		pooledList.Recycle();
	}

	private float GetDistanceFromImpact()
	{
		float num = velocity.x / velocity.y;
		Vector3 position = base.transform.GetPosition();
		float num2 = 0f;
		while (num2 > -6f)
		{
			num2 -= 1f;
			float num3 = Mathf.Ceil(position.y + num2) - 0.2f;
			num2 = num3 - position.y;
			float x = num2 * num;
			Vector3 b = new Vector3(x, num2, 0f);
			int num4 = Grid.PosToCell(position + b);
			if (Grid.IsValidCell(num4) && Grid.Solid[num4])
			{
				return b.magnitude;
			}
		}
		return 6f;
	}

	public float GetSoundDistance()
	{
		return GetDistanceFromImpact();
	}

	private void PlayTileDamageSound(Element element, Vector3 pos, GameObject tile_go)
	{
		string text = element.substance.GetMiningBreakSound();
		if (text == null)
		{
			text = (element.HasTag(GameTags.RefinedMetal) ? "RefinedMetal" : ((!element.HasTag(GameTags.Metal)) ? "Rock" : "RawMetal"));
		}
		text = "MeteorDamage_" + text;
		text = GlobalAssets.GetSound(text);
		if ((bool)CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, text))
		{
			float volume = GetVolume(tile_go);
			KFMOD.PlayOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(pos), volume);
		}
	}

	private void PlayBuildingDamageSound(BuildingDef def, Vector3 pos, GameObject building_go)
	{
		if (def != null)
		{
			string name = StringFormatter.Combine("MeteorDamage_Building_", def.AudioCategory);
			string sound = GlobalAssets.GetSound(name);
			if (sound == null)
			{
				name = "MeteorDamage_Building_Metal";
				sound = GlobalAssets.GetSound(name);
			}
			if (sound != null && (bool)CameraController.Instance && CameraController.Instance.IsAudibleSound(pos, sound))
			{
				float volume = GetVolume(building_go);
				KFMOD.PlayOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(pos), volume);
			}
		}
	}

	public void Sim33ms(float dt)
	{
		if (hasExploded)
		{
			return;
		}
		Vector2 vector = new Vector2(Grid.WidthInCells, Grid.HeightInCells) * -0.1f;
		Vector2 vector2 = new Vector2(Grid.WidthInCells, Grid.HeightInCells) * 1.1f;
		Vector3 position = base.transform.GetPosition();
		Vector3 vector3 = position + new Vector3(velocity.x * dt, velocity.y * dt, 0f);
		int num = Grid.PosToCell(vector3);
		loopingSounds.UpdateVelocity(flyingSound, vector3 - position);
		Element element = ElementLoader.FindElementByHash(EXHAUST_ELEMENT);
		if (EXHAUST_ELEMENT != SimHashes.Void && Grid.IsValidCell(num) && !Grid.Solid[num])
		{
			SimMessages.EmitMass(num, element.idx, dt * EXHAUST_RATE, element.defaultValues.temperature, diseaseIdx, Mathf.RoundToInt((float)addDiseaseCount * dt));
		}
		if (vector3.x < vector.x || vector2.x < vector3.x || vector3.y < vector.y)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
		int num2 = Grid.PosToCell(this);
		int num3 = Grid.PosToCell(previousPosition);
		if (num2 != num3)
		{
			if (Grid.IsValidCell(num2) && Grid.Solid[num2])
			{
				PrimaryElement component = GetComponent<PrimaryElement>();
				remainingTileDamage = DamageTiles(num2, num3, remainingTileDamage);
				if (remainingTileDamage <= 0f)
				{
					Explode(position, num2, num3, component.Element);
					hasExploded = true;
					if (destroyOnExplode)
					{
						Util.KDestroyGameObject(base.gameObject);
					}
					return;
				}
			}
			else
			{
				GameObject ignoreObject = ((ignoreObstacleForDamage.Get() == null) ? null : ignoreObstacleForDamage.Get().gameObject);
				DamageThings(position, num2, entityDamage, ignoreObject);
			}
		}
		if (canHitDuplicants && age > 0.25f && Grid.Objects[Grid.PosToCell(position), 0] != null)
		{
			base.transform.position = Grid.CellToPos(Grid.PosToCell(position));
			Explode(position, num2, num3, GetComponent<PrimaryElement>().Element);
			if (destroyOnExplode)
			{
				Util.KDestroyGameObject(base.gameObject);
			}
		}
		else
		{
			previousPosition = position;
			base.transform.SetPosition(vector3);
			age += dt;
		}
	}

	private void PlayImpactSound(Vector3 pos)
	{
		if (impactSound == null)
		{
			impactSound = "Meteor_Large_Impact";
		}
		loopingSounds.StopSound(flyingSound);
		string sound = GlobalAssets.GetSound(impactSound);
		if (CameraController.Instance.IsAudibleSound(pos, sound))
		{
			float volume = GetVolume(base.gameObject);
			pos.z = 0f;
			EventInstance instance = KFMOD.BeginOneShot(sound, pos, volume);
			instance.setParameterByName("userVolume_SFX", KPlayerPrefs.GetFloat("Volume_SFX"));
			KFMOD.EndOneShot(instance);
		}
	}

	private void StartLoopingSound()
	{
		loopingSounds.StartSound(flyingSound);
		loopingSounds.UpdateFirstParameter(flyingSound, FLYING_SOUND_ID_PARAMETER, flyingSoundID);
	}
}
