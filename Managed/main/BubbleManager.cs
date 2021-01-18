using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BubbleManager")]
public class BubbleManager : KMonoBehaviour, ISim33ms, IRenderEveryTick
{
	private struct Bubble
	{
		public Vector2 position;

		public Vector2 velocity;

		public float elapsedTime;

		public int frame;

		public SimHashes element;

		public float temperature;

		public float mass;
	}

	public static BubbleManager instance;

	private List<Bubble> bubbles = new List<Bubble>();

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		instance = this;
	}

	public void SpawnBubble(Vector2 position, Vector2 velocity, SimHashes element, float mass, float temperature)
	{
		Bubble bubble = default(Bubble);
		bubble.position = position;
		bubble.velocity = velocity;
		bubble.element = element;
		bubble.temperature = temperature;
		bubble.mass = mass;
		Bubble item = bubble;
		bubbles.Add(item);
	}

	public void Sim33ms(float dt)
	{
		ListPool<Bubble, BubbleManager>.PooledList pooledList = ListPool<Bubble, BubbleManager>.Allocate();
		ListPool<Bubble, BubbleManager>.PooledList pooledList2 = ListPool<Bubble, BubbleManager>.Allocate();
		foreach (Bubble bubble in bubbles)
		{
			Bubble current = bubble;
			current.position += current.velocity * dt;
			current.elapsedTime += dt;
			int num = Grid.PosToCell(current.position);
			if (!Grid.IsVisiblyInLiquid(current.position) || Grid.Element[num].id == current.element)
			{
				pooledList2.Add(current);
			}
			else
			{
				pooledList.Add(current);
			}
		}
		foreach (Bubble item in pooledList2)
		{
			SimMessages.AddRemoveSubstance(Grid.PosToCell(item.position), item.element, CellEventLogger.Instance.FallingWaterAddToSim, item.mass, item.temperature, byte.MaxValue, 0);
		}
		bubbles.Clear();
		bubbles.AddRange(pooledList);
		pooledList2.Recycle();
		pooledList.Recycle();
	}

	public void RenderEveryTick(float dt)
	{
		ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.PooledList pooledList = ListPool<SpriteSheetAnimator.AnimInfo, BubbleManager>.Allocate();
		SpriteSheetAnimator spriteSheetAnimator = SpriteSheetAnimManager.instance.GetSpriteSheetAnimator("liquid_splash1");
		foreach (Bubble bubble in bubbles)
		{
			SpriteSheetAnimator.AnimInfo animInfo = default(SpriteSheetAnimator.AnimInfo);
			animInfo.frame = spriteSheetAnimator.GetFrameFromElapsedTimeLooping(bubble.elapsedTime);
			animInfo.elapsedTime = bubble.elapsedTime;
			animInfo.pos = new Vector3(bubble.position.x, bubble.position.y, 0f);
			animInfo.rotation = Quaternion.identity;
			animInfo.size = Vector2.one;
			animInfo.colour = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			SpriteSheetAnimator.AnimInfo item = animInfo;
			pooledList.Add(item);
		}
		pooledList.Recycle();
	}
}
