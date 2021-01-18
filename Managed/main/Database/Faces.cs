namespace Database
{
	public class Faces : ResourceSet<Face>
	{
		public Face Neutral;

		public Face Happy;

		public Face Uncomfortable;

		public Face Cold;

		public Face Hot;

		public Face Tired;

		public Face Sleep;

		public Face Hungry;

		public Face Angry;

		public Face Suffocate;

		public Face Dead;

		public Face Sick;

		public Face SickSpores;

		public Face Zombie;

		public Face SickFierySkin;

		public Face SickCold;

		public Face Productive;

		public Face Determined;

		public Face Sticker;

		public Face Balloon;

		public Face Sparkle;

		public Face Tickled;

		public Faces()
		{
			Neutral = Add(new Face("Neutral"));
			Happy = Add(new Face("Happy"));
			Uncomfortable = Add(new Face("Uncomfortable"));
			Cold = Add(new Face("Cold"));
			Hot = Add(new Face("Hot"));
			Tired = Add(new Face("Tired"));
			Sleep = Add(new Face("Sleep"));
			Hungry = Add(new Face("Hungry"));
			Angry = Add(new Face("Angry"));
			Suffocate = Add(new Face("Suffocate"));
			Sick = Add(new Face("Sick"));
			SickSpores = Add(new Face("Spores"));
			Zombie = Add(new Face("Zombie"));
			SickFierySkin = Add(new Face("Fiery"));
			SickCold = Add(new Face("Cold"));
			Dead = Add(new Face("Death"));
			Productive = Add(new Face("Productive"));
			Determined = Add(new Face("Determined"));
			Sticker = Add(new Face("Sticker"));
			Sparkle = Add(new Face("Sparkle"));
			Balloon = Add(new Face("Balloon"));
			Tickled = Add(new Face("Tickled"));
		}
	}
}
