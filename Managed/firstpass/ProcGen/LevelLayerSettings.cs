namespace ProcGen
{
	public class LevelLayerSettings : IMerge<LevelLayerSettings>
	{
		public LevelLayer LevelLayers
		{
			get;
			private set;
		}

		public LevelLayerSettings()
		{
			LevelLayers = new LevelLayer();
		}

		public LevelLayerSettings Merge(LevelLayerSettings other)
		{
			LevelLayers.Merge(other.LevelLayers);
			return this;
		}
	}
}
