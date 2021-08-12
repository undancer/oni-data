using UnityEngine;

public class BuildingUnderConstruction : Building
{
	[MyCmpAdd]
	private KSelectable selectable;

	[MyCmpAdd]
	private SaveLoadRoot saveLoadRoot;

	[MyCmpAdd]
	private KPrefabID kPrefabID;

	protected override void OnPrefabInit()
	{
		Vector3 position = base.transform.GetPosition();
		position.z = Grid.GetLayerZ(Def.SceneLayer);
		base.transform.SetPosition(position);
		base.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Construction"));
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		Rotatable component2 = GetComponent<Rotatable>();
		if (component != null && component2 == null)
		{
			component.Offset = Def.GetVisualizerOffset();
		}
		KBoxCollider2D component3 = GetComponent<KBoxCollider2D>();
		if (component3 != null)
		{
			Vector3 visualizerOffset = Def.GetVisualizerOffset();
			component3.offset += new Vector2(visualizerOffset.x, visualizerOffset.y);
		}
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (Def.IsTilePiece)
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			Def.RunOnArea(cell, base.Orientation, delegate(int c)
			{
				TileVisualizer.RefreshCell(c, Def.TileLayer, Def.ReplacementLayer);
			});
		}
		RegisterBlockTileRenderer();
	}

	protected override void OnCleanUp()
	{
		UnregisterBlockTileRenderer();
		base.OnCleanUp();
	}
}
