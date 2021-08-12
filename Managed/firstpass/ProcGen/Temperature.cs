namespace ProcGen
{
	public class Temperature
	{
		public enum Range
		{
			ExtremelyCold,
			VeryCold,
			Cold,
			Chilly,
			Cool,
			Mild,
			Room,
			HumanWarm,
			HumanHot,
			Hot,
			VeryHot,
			ExtremelyHot
		}

		public float min { get; private set; }

		public float max { get; private set; }

		public Temperature()
		{
			min = 0f;
			max = 0f;
		}
	}
}
