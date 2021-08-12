namespace Klei
{
	public class SimSaveFileStructure
	{
		public int WidthInCells;

		public int HeightInCells;

		public int x;

		public int y;

		public byte[] Sim;

		public WorldDetailSave worldDetail;

		public SimSaveFileStructure()
		{
			worldDetail = new WorldDetailSave();
		}
	}
}
