namespace Database
{
	public class Expressions : ResourceSet<Expression>
	{
		public Expression Neutral;

		public Expression Happy;

		public Expression Uncomfortable;

		public Expression Cold;

		public Expression Hot;

		public Expression FullBladder;

		public Expression Tired;

		public Expression Hungry;

		public Expression Angry;

		public Expression Unhappy;

		public Expression RedAlert;

		public Expression Suffocate;

		public Expression RecoverBreath;

		public Expression Sick;

		public Expression SickSpores;

		public Expression Zombie;

		public Expression SickFierySkin;

		public Expression SickCold;

		public Expression Pollen;

		public Expression Relief;

		public Expression Productive;

		public Expression Determined;

		public Expression Sticker;

		public Expression Balloon;

		public Expression Sparkle;

		public Expression Music;

		public Expression Tickled;

		public Expression Radiation1;

		public Expression Radiation2;

		public Expression Radiation3;

		public Expression Radiation4;

		public Expressions(ResourceSet parent)
			: base("Expressions", parent)
		{
			Faces faces = Db.Get().Faces;
			Angry = new Expression("Angry", this, faces.Angry);
			Suffocate = new Expression("Suffocate", this, faces.Suffocate);
			RecoverBreath = new Expression("RecoverBreath", this, faces.Uncomfortable);
			RedAlert = new Expression("RedAlert", this, faces.Hot);
			Hungry = new Expression("Hungry", this, faces.Hungry);
			Radiation1 = new Expression("Radiation1", this, faces.Radiation1);
			Radiation2 = new Expression("Radiation2", this, faces.Radiation2);
			Radiation3 = new Expression("Radiation3", this, faces.Radiation3);
			Radiation4 = new Expression("Radiation4", this, faces.Radiation4);
			SickSpores = new Expression("SickSpores", this, faces.SickSpores);
			Zombie = new Expression("Zombie", this, faces.Zombie);
			SickFierySkin = new Expression("SickFierySkin", this, faces.SickFierySkin);
			SickCold = new Expression("SickCold", this, faces.SickCold);
			Pollen = new Expression("Pollen", this, faces.Pollen);
			Sick = new Expression("Sick", this, faces.Sick);
			Cold = new Expression("Cold", this, faces.Cold);
			Hot = new Expression("Hot", this, faces.Hot);
			FullBladder = new Expression("FullBladder", this, faces.Uncomfortable);
			Tired = new Expression("Tired", this, faces.Tired);
			Unhappy = new Expression("Unhappy", this, faces.Uncomfortable);
			Uncomfortable = new Expression("Uncomfortable", this, faces.Uncomfortable);
			Productive = new Expression("Productive", this, faces.Productive);
			Determined = new Expression("Determined", this, faces.Determined);
			Sticker = new Expression("Sticker", this, faces.Sticker);
			Balloon = new Expression("Sticker", this, faces.Balloon);
			Sparkle = new Expression("Sticker", this, faces.Sparkle);
			Music = new Expression("Music", this, faces.Music);
			Tickled = new Expression("Tickled", this, faces.Tickled);
			Happy = new Expression("Happy", this, faces.Happy);
			Relief = new Expression("Relief", this, faces.Happy);
			Neutral = new Expression("Neutral", this, faces.Neutral);
			for (int num = Count - 1; num >= 0; num--)
			{
				resources[num].priority = 100 * (Count - num);
			}
		}
	}
}
