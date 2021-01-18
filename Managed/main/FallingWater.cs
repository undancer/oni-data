using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Database;
using FMOD.Studio;
using FMODUnity;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/FallingWater")]
public class FallingWater : KMonoBehaviour, ISim200ms
{
	[Serializable]
	private struct DecorInfo
	{
		public string[] names;

		public Vector2 offset;

		public Vector2 size;
	}

	private struct SoundInfo
	{
		public float startTime;

		public int splashCount;

		public HandleVector<int>.Handle handle;
	}

	private struct MistInfo
	{
		public KBatchedAnimController fx;

		public float deathTime;
	}

	private struct ParticlePhysics
	{
		public Vector2 position;

		public Vector2 velocity;

		public int frame;

		public Color32 colour;

		public ParticlePhysics(Vector2 position, Vector2 velocity, int frame, byte elementIdx)
		{
			this.position = position;
			this.velocity = velocity;
			this.frame = frame;
			colour = ElementLoader.elements[elementIdx].substance.colour;
			colour.a = 191;
		}
	}

	private struct SerializedParticleProperties
	{
		public SimHashes elementID;

		public HashedString diseaseID;

		public float mass;

		public float temperature;

		public int diseaseCount;
	}

	private struct ParticleProperties
	{
		public byte elementIdx;

		public byte diseaseIdx;

		public float mass;

		public float temperature;

		public int diseaseCount;

		public ParticleProperties(byte elementIdx, float mass, float temperature, byte disease_idx, int disease_count, bool debug_track)
		{
			this.elementIdx = elementIdx;
			diseaseIdx = disease_idx;
			this.mass = mass;
			this.temperature = temperature;
			diseaseCount = disease_count;
		}
	}

	private const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

	private const byte FORCED_ALPHA = 191;

	private int simUpdateDelay = 2;

	[SerializeField]
	private Vector2 particleSize;

	[SerializeField]
	private Vector2 initialOffset;

	[SerializeField]
	private float jitterStep;

	[SerializeField]
	private Vector3 renderOffset;

	[SerializeField]
	private float minSpawnDelay;

	[SerializeField]
	private float gravityScale = 0.05f;

	[SerializeField]
	private float particleMassToSplit = 75f;

	[SerializeField]
	private float particleMassVariation = 15f;

	[SerializeField]
	private Vector2 multipleOffsetRange;

	[SerializeField]
	private GameObject mistEffect;

	[SerializeField]
	private float mistEffectMinAliveTime = 2f;

	[SerializeField]
	private Material material;

	[SerializeField]
	private Texture2D texture;

	[SerializeField]
	private int numFrames;

	[SerializeField]
	private DecorInfo liquid_splash;

	[SerializeField]
	[EventRef]
	private string liquid_top_loop;

	[SerializeField]
	[EventRef]
	private string liquid_splash_initial;

	[SerializeField]
	[EventRef]
	private string liquid_splash_loop;

	[SerializeField]
	private float stopTopLoopDelay = 0.2f;

	[SerializeField]
	private float stopSplashLoopDelay = 1f;

	[SerializeField]
	private int splashCountLoopThreshold = 10;

	[Serialize]
	private List<ParticlePhysics> physics = new List<ParticlePhysics>();

	private List<ParticleProperties> particleProperties = new List<ParticleProperties>();

	[Serialize]
	private List<SerializedParticleProperties> serializedParticleProperties;

	[Serialize]
	private List<ParticleProperties> properties = new List<ParticleProperties>();

	private Dictionary<int, SoundInfo> topSounds = new Dictionary<int, SoundInfo>();

	private Dictionary<int, SoundInfo> splashSounds = new Dictionary<int, SoundInfo>();

	private ObjectPool mistPool;

	private Mesh mesh;

	private float offset = 0f;

	private float[] lastSpawnTime;

	private Dictionary<Pair<int, bool>, MistInfo> mistAlive = new Dictionary<Pair<int, bool>, MistInfo>();

	private Vector2 uvFrameSize;

	private MaterialPropertyBlock propertyBlock;

	private static FallingWater _instance;

	private List<int> clearList = new List<int>();

	private List<Pair<int, bool>> mistClearList = new List<Pair<int, bool>>();

	private static HashedString HASH_LIQUIDDEPTH = "liquidDepth";

	private static HashedString HASH_LIQUIDVOLUME = "liquidVolume";

	public static FallingWater instance
	{
		get
		{
			return _instance;
		}
		private set
		{
		}
	}

	public static void DestroyInstance()
	{
		_instance = null;
	}

	protected override void OnPrefabInit()
	{
		_instance = this;
		base.OnPrefabInit();
		mistEffect.SetActive(value: false);
		mistPool = new ObjectPool(InstantiateMist, 16);
	}

	protected override void OnSpawn()
	{
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.name = "FallingWater";
		lastSpawnTime = new float[Grid.WidthInCells * Grid.HeightInCells];
		for (int i = 0; i < lastSpawnTime.Length; i++)
		{
			lastSpawnTime[i] = 0f;
		}
		propertyBlock = new MaterialPropertyBlock();
		propertyBlock.SetTexture("_MainTex", texture);
		uvFrameSize = new Vector2(1f / (float)numFrames, 1f);
	}

	protected override void OnCleanUp()
	{
		instance = null;
		base.OnCleanUp();
	}

	private float GetTime()
	{
		return Time.time % 360f;
	}

	public void AddParticle(int cell, byte elementIdx, float base_mass, float temperature, byte disease_idx, int base_disease_count, bool skip_sound = false, bool skip_decor = false, bool debug_track = false, bool disable_randomness = false)
	{
		Vector2 root_pos = Grid.CellToPos2D(cell);
		AddParticle(root_pos, elementIdx, base_mass, temperature, disease_idx, base_disease_count, skip_sound, skip_decor, debug_track, disable_randomness);
	}

	public void AddParticle(Vector2 root_pos, byte elementIdx, float base_mass, float temperature, byte disease_idx, int base_disease_count, bool skip_sound = false, bool skip_decor = false, bool debug_track = false, bool disable_randomness = false)
	{
		int num = Grid.PosToCell(root_pos);
		if (!Grid.IsValidCell(num))
		{
			KCrashReporter.Assert(condition: false, "Trying to add falling water outside of the scene");
			return;
		}
		if (temperature <= 0f || base_mass <= 0f)
		{
			Debug.LogError($"Unexpected water mass/temperature values added to the falling water manager T({temperature}) M({base_mass})");
		}
		float time = GetTime();
		if (!skip_sound)
		{
			if (!topSounds.TryGetValue(num, out var value))
			{
				value = default(SoundInfo);
				value.handle = LoopingSoundManager.StartSound(liquid_top_loop, root_pos);
			}
			value.startTime = time;
			LoopingSoundManager.Get().UpdateSecondParameter(value.handle, HASH_LIQUIDVOLUME, SoundUtil.GetLiquidVolume(base_mass));
			topSounds[num] = value;
		}
		while (base_mass > 0f)
		{
			float num2 = UnityEngine.Random.value * 2f * particleMassVariation - particleMassVariation;
			float num3 = Mathf.Max(0f, Mathf.Min(base_mass, particleMassToSplit + num2));
			float num4 = num3 / base_mass;
			base_mass -= num3;
			int disease_count = (int)(num4 * (float)base_disease_count);
			int frame = UnityEngine.Random.Range(0, numFrames);
			Vector2 b = (disable_randomness ? Vector2.zero : new Vector2(jitterStep * Mathf.Sin(offset), jitterStep * Mathf.Sin(offset + 17f)));
			Vector2 b2 = (disable_randomness ? Vector2.zero : new Vector2(UnityEngine.Random.Range(0f - multipleOffsetRange.x, multipleOffsetRange.x), UnityEngine.Random.Range(0f - multipleOffsetRange.y, multipleOffsetRange.y)));
			Element element = ElementLoader.elements[elementIdx];
			Vector2 vector = root_pos;
			bool flag = !skip_decor && SpawnLiquidTopDecor(time, Grid.CellLeft(num), flip: false, element);
			bool flag2 = !skip_decor && SpawnLiquidTopDecor(time, Grid.CellRight(num), flip: true, element);
			Vector2 vector2 = Vector2.ClampMagnitude(initialOffset + b + b2, 1f);
			if (flag || flag2)
			{
				if (flag && flag2)
				{
					vector += vector2;
					vector.x += 0.5f;
				}
				else if (flag)
				{
					vector += vector2;
				}
				else
				{
					vector.x += 1f - vector2.x;
					vector.y += vector2.y;
				}
			}
			else
			{
				vector += vector2;
				vector.x += 0.5f;
			}
			int num5 = Grid.PosToCell(vector);
			Element element2 = Grid.Element[num5];
			Element.State state = element2.state & Element.State.Solid;
			if (state == Element.State.Solid || (Grid.Properties[num5] & 2u) != 0)
			{
				vector.y = Mathf.Floor(vector.y + 1f);
			}
			physics.Add(new ParticlePhysics(vector, Vector2.zero, frame, elementIdx));
			particleProperties.Add(new ParticleProperties(elementIdx, num3, temperature, disease_idx, disease_count, debug_track));
		}
	}

	private bool SpawnLiquidTopDecor(float time, int cell, bool flip, Element element)
	{
		if (Grid.IsValidCell(cell) && Grid.Element[cell] == element)
		{
			Vector3 vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.TileMain);
			if (CameraController.Instance.IsVisiblePos(vector))
			{
				Pair<int, bool> key = new Pair<int, bool>(cell, flip);
				if (!mistAlive.TryGetValue(key, out var value))
				{
					value = default(MistInfo);
					value.fx = SpawnMist();
					value.fx.TintColour = element.substance.colour;
					Vector3 position = vector + (flip ? (-Vector3.right) : Vector3.right) * 0.5f;
					value.fx.transform.SetPosition(position);
					value.fx.FlipX = flip;
				}
				value.deathTime = Time.time + mistEffectMinAliveTime;
				mistAlive[key] = value;
				return true;
			}
		}
		return false;
	}

	public void SpawnLiquidSplash(float x, int cell, byte elementIdx, bool forceSplash = false)
	{
		float time = GetTime();
		float num = lastSpawnTime[cell];
		if (time - num >= minSpawnDelay || forceSplash)
		{
			lastSpawnTime[cell] = time;
			Vector2 a = Grid.CellToPos2D(cell);
			a.x = x - 0.5f;
			int num2 = UnityEngine.Random.Range(0, liquid_splash.names.Length);
			Vector2 vector = a + new Vector2(liquid_splash.offset.x, liquid_splash.offset.y);
			SpriteSheetAnimManager.instance.Play(liquid_splash.names[num2], new Vector3(vector.x, vector.y, renderOffset.z), new Vector2(liquid_splash.size.x, liquid_splash.size.y), Color.white);
		}
	}

	public void UpdateParticles(float dt)
	{
		if (dt <= 0f || simUpdateDelay >= 0)
		{
			return;
		}
		offset = (offset + dt) % 360f;
		int num_particles = physics.Count;
		Vector2 vector = Physics.gravity * dt * gravityScale;
		for (int i = 0; i < num_particles; i++)
		{
			ParticlePhysics value = physics[i];
			Vector3 vector2 = value.position;
			Grid.PosToXY(vector2, out var x, out var y);
			value.velocity += vector;
			Vector3 b = value.velocity * dt;
			Vector3 v = vector2 + b;
			value.position = v;
			physics[i] = value;
			Grid.PosToXY(value.position, out var _, out var y2);
			int num = ((y > y2) ? y : y2);
			int num2 = ((y > y2) ? y2 : y);
			int num3 = num;
			while (num3 >= num2)
			{
				int num4 = num3 * Grid.WidthInCells + x;
				int cell = (num3 + 1) * Grid.WidthInCells + x;
				if (Grid.IsValidCell(num4))
				{
					Element element = Grid.Element[num4];
					Element.State state = element.state & Element.State.Solid;
					bool flag = false;
					if (state == Element.State.Solid || (Grid.Properties[num4] & 2u) != 0)
					{
						AddToSim(cell, i, ref num_particles);
					}
					else
					{
						switch (state)
						{
						case Element.State.Gas:
							flag = true;
							break;
						case Element.State.Liquid:
						{
							ParticleProperties particleProperties = this.particleProperties[i];
							Element element2 = ElementLoader.elements[particleProperties.elementIdx];
							if (element2.id == element.id)
							{
								if (Grid.Mass[num4] <= element.defaultValues.mass)
								{
									flag = true;
									break;
								}
								SpawnLiquidSplash(value.position.x, cell, particleProperties.elementIdx);
								AddToSim(num4, i, ref num_particles);
							}
							else if (element2.molarMass > element.molarMass)
							{
								flag = true;
							}
							else
							{
								SpawnLiquidSplash(value.position.x, cell, particleProperties.elementIdx);
								AddToSim(cell, i, ref num_particles);
							}
							break;
						}
						case Element.State.Vacuum:
							if (element.id == SimHashes.Vacuum)
							{
								flag = true;
							}
							else
							{
								RemoveParticle(i, ref num_particles);
							}
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num3--;
					continue;
				}
				if (Grid.IsValidCell(cell))
				{
					SpawnLiquidSplash(elementIdx: this.particleProperties[i].elementIdx, x: value.position.x, cell: cell);
					AddToSim(cell, i, ref num_particles);
				}
				else
				{
					RemoveParticle(i, ref num_particles);
				}
				break;
			}
		}
		float time = GetTime();
		UpdateSounds(time);
		UpdateMistFX(Time.time);
	}

	private void UpdateMistFX(float t)
	{
		mistClearList.Clear();
		foreach (KeyValuePair<Pair<int, bool>, MistInfo> item in mistAlive)
		{
			if (t > item.Value.deathTime)
			{
				item.Value.fx.Play("end");
				mistClearList.Add(item.Key);
			}
		}
		foreach (Pair<int, bool> mistClear in mistClearList)
		{
			mistAlive.Remove(mistClear);
		}
		mistClearList.Clear();
	}

	private void UpdateSounds(float t)
	{
		clearList.Clear();
		foreach (KeyValuePair<int, SoundInfo> topSound in topSounds)
		{
			SoundInfo value = topSound.Value;
			float num = t - value.startTime;
			if (num >= stopTopLoopDelay)
			{
				if (value.handle != HandleVector<int>.InvalidHandle)
				{
					LoopingSoundManager.StopSound(value.handle);
				}
				clearList.Add(topSound.Key);
			}
		}
		foreach (int clear in clearList)
		{
			topSounds.Remove(clear);
		}
		clearList.Clear();
		foreach (KeyValuePair<int, SoundInfo> splashSound in splashSounds)
		{
			SoundInfo value2 = splashSound.Value;
			float num2 = t - value2.startTime;
			if (num2 >= stopSplashLoopDelay)
			{
				if (value2.handle != HandleVector<int>.InvalidHandle)
				{
					LoopingSoundManager.StopSound(value2.handle);
				}
				clearList.Add(splashSound.Key);
			}
		}
		foreach (int clear2 in clearList)
		{
			splashSounds.Remove(clear2);
		}
		clearList.Clear();
	}

	public Dictionary<int, float> GetInfo(int cell)
	{
		Dictionary<int, float> dictionary = new Dictionary<int, float>();
		int count = physics.Count;
		for (int i = 0; i < count; i++)
		{
			int num = Grid.PosToCell(physics[i].position);
			if (num == cell)
			{
				ParticleProperties particleProperties = this.particleProperties[i];
				float value = 0f;
				dictionary.TryGetValue(particleProperties.elementIdx, out value);
				value += particleProperties.mass;
				dictionary[particleProperties.elementIdx] = value;
			}
		}
		return dictionary;
	}

	private float GetParticleVolume(float mass)
	{
		return Mathf.Clamp01((mass - (particleMassToSplit - particleMassVariation)) / (2f * particleMassVariation));
	}

	private void AddToSim(int cell, int particleIdx, ref int num_particles)
	{
		bool flag = false;
		do
		{
			Element element = Grid.Element[cell];
			Element.State state = element.state & Element.State.Solid;
			if (state == Element.State.Solid || (Grid.Properties[cell] & 2u) != 0)
			{
				cell += Grid.WidthInCells;
				if (!Grid.IsValidCell(cell))
				{
					return;
				}
			}
			else
			{
				flag = true;
			}
		}
		while (!flag);
		ParticleProperties particleProperties = this.particleProperties[particleIdx];
		SimMessages.AddRemoveSubstance(cell, particleProperties.elementIdx, CellEventLogger.Instance.FallingWaterAddToSim, particleProperties.mass, particleProperties.temperature, particleProperties.diseaseIdx, particleProperties.diseaseCount);
		RemoveParticle(particleIdx, ref num_particles);
		float time = GetTime();
		float num = lastSpawnTime[cell];
		if (!(time - num >= minSpawnDelay))
		{
			return;
		}
		lastSpawnTime[cell] = time;
		Vector3 vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.TileMain);
		vector.z = 0f;
		if (!CameraController.Instance.IsAudibleSound(vector))
		{
			return;
		}
		bool flag2 = true;
		if (splashSounds.TryGetValue(cell, out var value))
		{
			value.splashCount++;
			if (value.splashCount > splashCountLoopThreshold)
			{
				if (value.handle == HandleVector<int>.InvalidHandle)
				{
					value.handle = LoopingSoundManager.StartSound(liquid_splash_loop, vector);
				}
				LoopingSoundManager.Get().UpdateFirstParameter(value.handle, HASH_LIQUIDDEPTH, SoundUtil.GetLiquidDepth(cell));
				LoopingSoundManager.Get().UpdateSecondParameter(value.handle, HASH_LIQUIDVOLUME, GetParticleVolume(particleProperties.mass));
				flag2 = false;
			}
		}
		else
		{
			value = default(SoundInfo);
			value.handle = HandleVector<int>.InvalidHandle;
		}
		value.startTime = time;
		splashSounds[cell] = value;
		if (flag2)
		{
			EventInstance instance = SoundEvent.BeginOneShot(liquid_splash_initial, vector);
			instance.setParameterByName("liquidDepth", SoundUtil.GetLiquidDepth(cell));
			instance.setParameterByName("liquidVolume", GetParticleVolume(particleProperties.mass));
			SoundEvent.EndOneShot(instance);
		}
	}

	private void RemoveParticle(int particleIdx, ref int num_particles)
	{
		num_particles--;
		physics[particleIdx] = physics[num_particles];
		particleProperties[particleIdx] = particleProperties[num_particles];
		physics.RemoveAt(num_particles);
		particleProperties.RemoveAt(num_particles);
	}

	public void Render()
	{
		List<Vector3> vertices = MeshUtil.vertices;
		List<Color32> colours = MeshUtil.colours32;
		List<Vector2> uvs = MeshUtil.uvs;
		List<int> indices = MeshUtil.indices;
		uvs.Clear();
		vertices.Clear();
		indices.Clear();
		colours.Clear();
		float num = particleSize.x * 0.5f;
		float num2 = particleSize.y * 0.5f;
		Vector2 a = new Vector2(0f - num, 0f - num2);
		Vector2 a2 = new Vector2(num, 0f - num2);
		Vector2 a3 = new Vector2(num, num2);
		Vector2 a4 = new Vector2(0f - num, num2);
		float y = 1f;
		float y2 = 0f;
		int num3 = Mathf.Min(physics.Count, 16249);
		if (num3 < physics.Count)
		{
			DebugUtil.LogWarningArgs("Too many water particles to render. Wanted", physics.Count, "but truncating to limit");
		}
		for (int i = 0; i < num3; i++)
		{
			Vector2 position = physics[i].position;
			float d = Mathf.Lerp(0.25f, 1f, Mathf.Clamp01(particleProperties[i].mass / particleMassToSplit));
			vertices.Add(position + a * d);
			vertices.Add(position + a2 * d);
			vertices.Add(position + a3 * d);
			vertices.Add(position + a4 * d);
			int frame = physics[i].frame;
			float x = (float)frame * uvFrameSize.x;
			float x2 = (float)(frame + 1) * uvFrameSize.x;
			uvs.Add(new Vector2(x, y2));
			uvs.Add(new Vector2(x2, y2));
			uvs.Add(new Vector2(x2, y));
			uvs.Add(new Vector2(x, y));
			Color32 colour = physics[i].colour;
			colours.Add(colour);
			colours.Add(colour);
			colours.Add(colour);
			colours.Add(colour);
			int num4 = i * 4;
			indices.Add(num4);
			indices.Add(num4 + 1);
			indices.Add(num4 + 2);
			indices.Add(num4);
			indices.Add(num4 + 2);
			indices.Add(num4 + 3);
		}
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetUVs(0, uvs);
		mesh.SetColors(colours);
		mesh.SetTriangles(indices, 0);
		int layer = LayerMask.NameToLayer("Water");
		Graphics.DrawMesh(mesh, renderOffset, Quaternion.identity, material, layer, null, 0, propertyBlock);
	}

	private KBatchedAnimController SpawnMist()
	{
		GameObject instance = mistPool.GetInstance();
		instance.SetActive(value: true);
		KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
		component.Play("loop", KAnim.PlayMode.Loop);
		return component;
	}

	private GameObject InstantiateMist()
	{
		GameObject gameObject = GameUtil.KInstantiate(mistEffect, Grid.SceneLayer.BuildingBack);
		gameObject.SetActive(value: false);
		KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
		component.onDestroySelf = ReleaseMist;
		return gameObject;
	}

	private void ReleaseMist(GameObject go)
	{
		go.SetActive(value: false);
		mistPool.ReleaseInstance(go);
	}

	public void Sim200ms(float dt)
	{
		if (simUpdateDelay >= 0)
		{
			simUpdateDelay--;
		}
		else
		{
			SimAndRenderScheduler.instance.Remove(this);
		}
	}

	[OnSerializing]
	private void OnSerializing()
	{
		List<Element> elements = ElementLoader.elements;
		Diseases diseases = Db.Get().Diseases;
		serializedParticleProperties = new List<SerializedParticleProperties>();
		foreach (ParticleProperties particleProperty in particleProperties)
		{
			SerializedParticleProperties item = default(SerializedParticleProperties);
			item.elementID = elements[particleProperty.elementIdx].id;
			item.diseaseID = ((particleProperty.diseaseIdx != byte.MaxValue) ? diseases[particleProperty.diseaseIdx].IdHash : HashedString.Invalid);
			item.mass = particleProperty.mass;
			item.temperature = particleProperty.temperature;
			item.diseaseCount = particleProperty.diseaseCount;
			serializedParticleProperties.Add(item);
		}
	}

	[OnSerialized]
	private void OnSerialized()
	{
		serializedParticleProperties = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (serializedParticleProperties != null)
		{
			Diseases diseases = Db.Get().Diseases;
			particleProperties.Clear();
			foreach (SerializedParticleProperties serializedParticleProperty in serializedParticleProperties)
			{
				ParticleProperties item = default(ParticleProperties);
				item.elementIdx = (byte)ElementLoader.GetElementIndex(serializedParticleProperty.elementID);
				item.diseaseIdx = ((serializedParticleProperty.diseaseID != HashedString.Invalid) ? diseases.GetIndex(serializedParticleProperty.diseaseID) : byte.MaxValue);
				item.mass = serializedParticleProperty.mass;
				item.temperature = serializedParticleProperty.temperature;
				item.diseaseCount = serializedParticleProperty.diseaseCount;
				particleProperties.Add(item);
			}
		}
		else
		{
			particleProperties = properties;
		}
		properties = null;
	}
}
