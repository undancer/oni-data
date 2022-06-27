using STRINGS;

namespace Database
{
	public class Thoughts : ResourceSet<Thought>
	{
		public Thought Starving;

		public Thought Hot;

		public Thought Cold;

		public Thought BreakBladder;

		public Thought FullBladder;

		public Thought Happy;

		public Thought Unhappy;

		public Thought PoorDecor;

		public Thought PoorFoodQuality;

		public Thought GoodFoodQuality;

		public Thought Sleepy;

		public Thought Suffocating;

		public Thought Angry;

		public Thought Raging;

		public Thought GotInfected;

		public Thought PutridOdour;

		public Thought Noisy;

		public Thought NewRole;

		public Thought Chatty;

		public Thought Encourage;

		public Thought CatchyTune;

		public Thoughts(ResourceSet parent)
			: base("Thoughts", parent)
		{
			GotInfected = new Thought("GotInfected", this, "crew_state_sick", null, "crew_state_sick", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.GOTINFECTED.TOOLTIP);
			Starving = new Thought("Starving", this, "crew_state_hungry", null, "crew_state_hungry", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.STARVING.TOOLTIP);
			Hot = new Thought("Hot", this, "crew_state_temp_up", null, "crew_state_temp_up", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.HOT.TOOLTIP);
			Cold = new Thought("Cold", this, "crew_state_temp_down", null, "crew_state_temp_down", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.COLD.TOOLTIP);
			BreakBladder = new Thought("BreakBladder", this, "crew_state_full_bladder", null, "crew_state_full_bladder", "bubble_conversation", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.BREAKBLADDER.TOOLTIP);
			FullBladder = new Thought("FullBladder", this, "crew_state_full_bladder", null, "crew_state_full_bladder", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.FULLBLADDER.TOOLTIP);
			PoorDecor = new Thought("PoorDecor", this, "crew_state_decor", null, "crew_state_decor", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.POORDECOR.TOOLTIP);
			PoorFoodQuality = new Thought("PoorFoodQuality", this, "crew_state_yuck", null, "crew_state_yuck", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.POOR_FOOD_QUALITY.TOOLTIP);
			GoodFoodQuality = new Thought("GoodFoodQuality", this, "crew_state_happy", null, "crew_state_happy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.GOOD_FOOD_QUALITY.TOOLTIP);
			Happy = new Thought("Happy", this, "crew_state_happy", null, "crew_state_happy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.HAPPY.TOOLTIP);
			Unhappy = new Thought("Unhappy", this, "crew_state_unhappy", null, "crew_state_unhappy", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.UNHAPPY.TOOLTIP);
			Sleepy = new Thought("Sleepy", this, "crew_state_sleepy", null, "crew_state_sleepy", "bubble_conversation", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.SLEEPY.TOOLTIP);
			Suffocating = new Thought("Suffocating", this, "crew_state_cantbreathe", null, "crew_state_cantbreathe", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.SUFFOCATING.TOOLTIP);
			Angry = new Thought("Angry", this, "crew_state_angry", null, "crew_state_angry", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.ANGRY.TOOLTIP);
			Raging = new Thought("Enraged", this, "crew_state_enraged", null, "crew_state_enraged", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.RAGING.TOOLTIP);
			PutridOdour = new Thought("PutridOdour", this, "crew_state_smelled_putrid_odour", null, "crew_state_smelled_putrid_odour", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.PUTRIDODOUR.TOOLTIP, show_immediately: true);
			Noisy = new Thought("Noisy", this, "crew_state_noisey", null, "crew_state_noisey", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.NOISY.TOOLTIP, show_immediately: true);
			NewRole = new Thought("NewRole", this, "crew_state_role", null, "crew_state_role", "bubble_alert", SpeechMonitor.PREFIX_SAD, DUPLICANTS.THOUGHTS.NEWROLE.TOOLTIP);
			Encourage = new Thought("Encourage", this, "crew_state_encourage", null, "crew_state_happy", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, DUPLICANTS.THOUGHTS.ENCOURAGE.TOOLTIP);
			Chatty = new Thought("Chatty", this, "crew_state_chatty", null, "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, DUPLICANTS.THOUGHTS.CHATTY.TOOLTIP);
			CatchyTune = new Thought("CatchyTune", this, "crew_state_music", null, "crew_state_music", "bubble_conversation", SpeechMonitor.PREFIX_SINGER, DUPLICANTS.THOUGHTS.CATCHYTUNE.TOOLTIP, show_immediately: true);
			for (int num = Count - 1; num >= 0; num--)
			{
				resources[num].priority = 100 * (Count - num);
			}
		}
	}
}
