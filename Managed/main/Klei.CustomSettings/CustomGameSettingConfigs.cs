using System.Collections.Generic;
using STRINGS;

namespace Klei.CustomSettings
{
	public static class CustomGameSettingConfigs
	{
		public static SeedSettingConfig WorldgenSeed = new SeedSettingConfig("WorldgenSeed", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLDGEN_SEED.TOOLTIP, debug_only: false, triggers_custom_game: false);

		public static ListSettingConfig World = new ListSettingConfig("World", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.WORLD_CHOICE.TOOLTIP, null, null, null, -1, -1, debug_only: false, triggers_custom_game: false);

		public static SettingConfig SandboxMode = new ToggleSettingConfig("SandboxMode", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.DISABLED.TOOLTIP), new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SANDBOXMODE.LEVELS.ENABLED.TOOLTIP), "Disabled", "Disabled");

		public static SettingConfig FastWorkersMode = new ToggleSettingConfig("FastWorkersMode", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.LEVELS.DISABLED.TOOLTIP), new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.LEVELS.ENABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.FASTWORKERSMODE.LEVELS.ENABLED.TOOLTIP), "Disabled", "Disabled", -1, -1, debug_only: true);

		public static SettingConfig SaveToCloud = new ToggleSettingConfig("SaveToCloud", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.LEVELS.DISABLED.TOOLTIP), new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.LEVELS.ENABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.LEVELS.ENABLED.TOOLTIP), "Enabled", "Enabled", -1, -1, debug_only: false, triggers_custom_game: false);

		public static SettingConfig CalorieBurn = new ListSettingConfig("CalorieBurn", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DISABLED.TOOLTIP, 2),
			new SettingLevel("Easy", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.EASY.TOOLTIP, 1),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.DEFAULT.TOOLTIP),
			new SettingLevel("Hard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.HARD.TOOLTIP, 3),
			new SettingLevel("VeryHard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CALORIE_BURN.LEVELS.VERYHARD.TOOLTIP, 4)
		}, "Default", "Easy", 1, 8);

		public static SettingConfig ImmuneSystem = new ListSettingConfig("ImmuneSystem", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Invincible", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.INVINCIBLE.TOOLTIP, 2),
			new SettingLevel("Strong", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.STRONG.TOOLTIP, 1),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.DEFAULT.TOOLTIP),
			new SettingLevel("Weak", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.WEAK.TOOLTIP, 3),
			new SettingLevel("Compromised", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.IMMUNESYSTEM.LEVELS.COMPROMISED.TOOLTIP, 4)
		}, "Default", "Strong", 8, 8);

		public static SettingConfig Morale = new ListSettingConfig("Morale", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DISABLED.TOOLTIP, 2),
			new SettingLevel("Easy", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.EASY.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.EASY.TOOLTIP, 1),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.DEFAULT.TOOLTIP),
			new SettingLevel("Hard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.HARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.HARD.TOOLTIP, 3),
			new SettingLevel("VeryHard", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.VERYHARD.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.MORALE.LEVELS.VERYHARD.TOOLTIP, 4)
		}, "Default", "Easy", 64, 8);

		public static SettingConfig Stress = new ListSettingConfig("Stress", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.TOOLTIP, new List<SettingLevel>
		{
			new SettingLevel("Indomitable", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.INDOMITABLE.TOOLTIP, 2),
			new SettingLevel("Optimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.OPTIMISTIC.TOOLTIP, 1),
			new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DEFAULT.TOOLTIP),
			new SettingLevel("Pessimistic", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.PESSIMISTIC.TOOLTIP, 3),
			new SettingLevel("Doomed", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS.LEVELS.DOOMED.TOOLTIP, 4)
		}, "Default", "Optimistic", 512, 8);

		public static SettingConfig StressBreaks = new ToggleSettingConfig("StressBreaks", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DISABLED.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DISABLED.TOOLTIP, 1), new SettingLevel("Default", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DEFAULT.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.STRESS_BREAKS.LEVELS.DEFAULT.TOOLTIP), "Default", "Default", 4096, 5);

		public static SettingConfig CarePackages = new ToggleSettingConfig("CarePackages", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.TOOLTIP, new SettingLevel("Disabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.LEVELS.DUPLICANTS_ONLY.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.LEVELS.DUPLICANTS_ONLY.TOOLTIP, 1), new SettingLevel("Enabled", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.LEVELS.NORMAL.NAME, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.LEVELS.NORMAL.TOOLTIP), "Enabled", "Enabled", 20480, 5);
	}
}
