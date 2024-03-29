using System.Collections.Generic;

namespace Klei.CustomSettings
{
	public abstract class SettingConfig
	{
		protected string default_level_id;

		protected string nosweat_default_level_id;

		public string id { get; private set; }

		public string label { get; private set; }

		public string tooltip { get; private set; }

		public int coordinate_dimension { get; protected set; }

		public int coordinate_dimension_width { get; protected set; }

		public string required_content { get; private set; }

		public string missing_content_default { get; private set; }

		public bool triggers_custom_game { get; protected set; }

		public bool debug_only { get; protected set; }

		public bool editor_only { get; protected set; }

		public SettingConfig(string id, string label, string tooltip, string default_level_id, string nosweat_default_level_id, int coordinate_dimension = -1, int coordinate_dimension_width = -1, bool debug_only = false, bool triggers_custom_game = true, string required_content = "", string missing_content_default = "", bool editor_only = false)
		{
			this.id = id;
			this.label = label;
			this.tooltip = tooltip;
			this.default_level_id = default_level_id;
			this.nosweat_default_level_id = nosweat_default_level_id;
			this.coordinate_dimension = coordinate_dimension;
			this.coordinate_dimension_width = coordinate_dimension_width;
			this.debug_only = debug_only;
			this.triggers_custom_game = triggers_custom_game;
			this.required_content = required_content;
			this.missing_content_default = missing_content_default;
			this.editor_only = editor_only;
		}

		public abstract SettingLevel GetLevel(string level_id);

		public abstract List<SettingLevel> GetLevels();

		public bool IsDefaultLevel(string level_id)
		{
			return level_id == default_level_id;
		}

		public string GetDefaultLevelId()
		{
			if (!DlcManager.IsContentActive(required_content) && !string.IsNullOrEmpty(missing_content_default))
			{
				return missing_content_default;
			}
			return default_level_id;
		}

		public string GetNoSweatDefaultLevelId()
		{
			if (!DlcManager.IsContentActive(required_content) && !string.IsNullOrEmpty(missing_content_default))
			{
				return missing_content_default;
			}
			return nosweat_default_level_id;
		}
	}
}
