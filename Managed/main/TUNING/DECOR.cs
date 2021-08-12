using STRINGS;

namespace TUNING
{
	public class DECOR
	{
		public class BONUS
		{
			public static readonly EffectorValues TIER0;

			public static readonly EffectorValues TIER1;

			public static readonly EffectorValues TIER2;

			public static readonly EffectorValues TIER3;

			public static readonly EffectorValues TIER4;

			public static readonly EffectorValues TIER5;

			public static readonly EffectorValues TIER6;

			public static readonly EffectorValues TIER7;

			public static readonly EffectorValues TIER8;

			static BONUS()
			{
				EffectorValues effectorValues = new EffectorValues
				{
					amount = 10,
					radius = 1
				};
				TIER0 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 15,
					radius = 2
				};
				TIER1 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 20,
					radius = 3
				};
				TIER2 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 25,
					radius = 4
				};
				TIER3 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 30,
					radius = 5
				};
				TIER4 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 35,
					radius = 6
				};
				TIER5 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 50,
					radius = 7
				};
				TIER6 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 80,
					radius = 7
				};
				TIER7 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = 200,
					radius = 8
				};
				TIER8 = effectorValues;
			}
		}

		public class PENALTY
		{
			public static readonly EffectorValues TIER0;

			public static readonly EffectorValues TIER1;

			public static readonly EffectorValues TIER2;

			public static readonly EffectorValues TIER3;

			public static readonly EffectorValues TIER4;

			public static readonly EffectorValues TIER5;

			static PENALTY()
			{
				EffectorValues effectorValues = new EffectorValues
				{
					amount = -5,
					radius = 1
				};
				TIER0 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -10,
					radius = 2
				};
				TIER1 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -15,
					radius = 3
				};
				TIER2 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -20,
					radius = 4
				};
				TIER3 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -20,
					radius = 5
				};
				TIER4 = effectorValues;
				effectorValues = new EffectorValues
				{
					amount = -25,
					radius = 6
				};
				TIER5 = effectorValues;
			}
		}

		public class SPACEARTIFACT
		{
			public static readonly ArtifactTier TIER_NONE = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER_NONE.key, NONE, 0f);

			public static readonly ArtifactTier TIER0 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER0.key, BONUS.TIER0, 0.25f);

			public static readonly ArtifactTier TIER1 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER1.key, BONUS.TIER2, 0.4f);

			public static readonly ArtifactTier TIER2 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER2.key, BONUS.TIER4, 0.55f);

			public static readonly ArtifactTier TIER3 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER3.key, BONUS.TIER5, 0.7f);

			public static readonly ArtifactTier TIER4 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER4.key, BONUS.TIER6, 0.85f);

			public static readonly ArtifactTier TIER5 = new ArtifactTier(UI.SPACEARTIFACTS.ARTIFACTTIERS.TIER5.key, BONUS.TIER7, 1f);
		}

		public static int LIT_BONUS = 15;

		public static readonly EffectorValues NONE = new EffectorValues
		{
			amount = 0,
			radius = 0
		};
	}
}
