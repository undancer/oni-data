public class Painting : Artable
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		multitoolContext = "paint";
		multitoolHitEffectTag = "fx_paint_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Paintings.Add(this);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Paintings.Remove(this);
	}
}
