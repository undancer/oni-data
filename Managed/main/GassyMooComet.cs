using System;
using UnityEngine;

public class GassyMooComet : Comet
{
	public override void RandomizeVelocity()
	{
		bool flag = false;
		byte id = Grid.WorldIdx[Grid.PosToCell(base.gameObject.transform.position)];
		WorldContainer world = ClusterManager.Instance.GetWorld(id);
		if (!(world == null))
		{
			int num = world.WorldOffset.x + world.Width / 2;
			if (Grid.PosToXY(base.gameObject.transform.position).x > num)
			{
				flag = true;
			}
			int num2 = (flag ? (-65) : 245);
			float f = (float)num2 * (float)Math.PI / 180f;
			float num3 = UnityEngine.Random.Range(spawnVelocity.x, spawnVelocity.y);
			velocity = new Vector2((0f - Mathf.Cos(f)) * num3, Mathf.Sin(f) * num3);
			KBatchedAnimController component = GetComponent<KBatchedAnimController>();
			component.FlipX = flag;
			component.Rotation = (flag ? (-10) : 10);
		}
	}

	protected override void SpawnCraterPrefabs()
	{
		if (craterPrefabs != null && craterPrefabs.Length != 0)
		{
			int cell = Grid.PosToCell(this);
			if (Grid.IsValidCell(Grid.CellAbove(cell)))
			{
				cell = Grid.CellAbove(cell);
			}
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(craterPrefabs[UnityEngine.Random.Range(0, craterPrefabs.Length)]), Grid.CellToPos(cell));
			gameObject.SetActive(value: true);
		}
	}
}
