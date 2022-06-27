using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SpriteSheetAnimManager")]
public class SpriteSheetAnimManager : KMonoBehaviour, IRenderEveryTick
{
	public const float SECONDS_PER_FRAME = 1f / 30f;

	[SerializeField]
	private SpriteSheet[] sheets;

	private Dictionary<int, SpriteSheetAnimator> nameIndexMap = new Dictionary<int, SpriteSheetAnimator>();

	public static SpriteSheetAnimManager instance;

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
	}

	protected override void OnSpawn()
	{
		for (int i = 0; i < sheets.Length; i++)
		{
			int key = Hash.SDBMLower(sheets[i].name);
			nameIndexMap[key] = new SpriteSheetAnimator(sheets[i]);
		}
	}

	public void Play(string name, Vector3 pos, Vector2 size, Color32 colour)
	{
		int name_hash = Hash.SDBMLower(name);
		Play(name_hash, pos, Quaternion.identity, size, colour);
	}

	public void Play(string name, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour)
	{
		int name_hash = Hash.SDBMLower(name);
		Play(name_hash, pos, rotation, size, colour);
	}

	public void Play(int name_hash, Vector3 pos, Quaternion rotation, Vector2 size, Color32 colour)
	{
		nameIndexMap[name_hash].Play(pos, rotation, size, colour);
	}

	public void RenderEveryTick(float dt)
	{
		UpdateAnims(dt);
		Render();
	}

	public void UpdateAnims(float dt)
	{
		foreach (KeyValuePair<int, SpriteSheetAnimator> item in nameIndexMap)
		{
			item.Value.UpdateAnims(dt);
		}
	}

	public void Render()
	{
		_ = Vector3.zero;
		foreach (KeyValuePair<int, SpriteSheetAnimator> item in nameIndexMap)
		{
			item.Value.Render();
		}
	}

	public SpriteSheetAnimator GetSpriteSheetAnimator(HashedString name)
	{
		return nameIndexMap[name.HashValue];
	}
}
