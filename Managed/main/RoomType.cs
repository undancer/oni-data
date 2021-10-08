using Klei.AI;
using STRINGS;

public class RoomType : Resource
{
	public enum RoomIdentificationResult
	{
		all_satisfied,
		primary_satisfied,
		primary_unsatisfied
	}

	public string tooltip { get; private set; }

	public string effect { get; private set; }

	public RoomConstraints.Constraint primary_constraint { get; private set; }

	public RoomConstraints.Constraint[] additional_constraints { get; private set; }

	public int priority { get; private set; }

	public bool single_assignee { get; private set; }

	public RoomDetails.Detail[] display_details { get; private set; }

	public bool priority_building_use { get; private set; }

	public RoomTypeCategory category { get; private set; }

	public RoomType[] upgrade_paths { get; private set; }

	public string[] effects { get; private set; }

	public int sortKey { get; private set; }

	public RoomType(string id, string name, string tooltip, string effect, RoomTypeCategory category, RoomConstraints.Constraint primary_constraint, RoomConstraints.Constraint[] additional_constraints, RoomDetails.Detail[] display_details, int priority = 0, RoomType[] upgrade_paths = null, bool single_assignee = false, bool priority_building_use = false, string[] effects = null, int sortKey = 0)
		: base(id, name)
	{
		this.tooltip = tooltip;
		this.effect = effect;
		this.category = category;
		this.primary_constraint = primary_constraint;
		this.additional_constraints = additional_constraints;
		this.display_details = display_details;
		this.priority = priority;
		this.upgrade_paths = upgrade_paths;
		this.single_assignee = single_assignee;
		this.priority_building_use = priority_building_use;
		this.effects = effects;
		this.sortKey = sortKey;
		if (this.upgrade_paths != null)
		{
			RoomType[] array = this.upgrade_paths;
			for (int i = 0; i < array.Length; i++)
			{
				Debug.Assert(array[i] != null, name + " has a null upgrade path. Maybe it wasn't initialized yet.");
			}
		}
	}

	public RoomIdentificationResult isSatisfactory(Room candidate_room)
	{
		if (primary_constraint != null && !primary_constraint.isSatisfied(candidate_room))
		{
			return RoomIdentificationResult.primary_unsatisfied;
		}
		if (additional_constraints != null)
		{
			RoomConstraints.Constraint[] array = additional_constraints;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].isSatisfied(candidate_room))
				{
					return RoomIdentificationResult.primary_satisfied;
				}
			}
		}
		return RoomIdentificationResult.all_satisfied;
	}

	public string GetCriteriaString()
	{
		string text = "<b>" + Name + "</b>\n" + tooltip + UI.HORIZONTAL_BR_RULE + ROOMS.CRITERIA.HEADER;
		if (this == Db.Get().RoomTypes.Neutral)
		{
			text = text + "\n    • " + ROOMS.CRITERIA.NEUTRAL_TYPE;
		}
		text += ((primary_constraint == null) ? "" : ("\n    • " + primary_constraint.name));
		if (additional_constraints != null)
		{
			RoomConstraints.Constraint[] array = additional_constraints;
			foreach (RoomConstraints.Constraint constraint in array)
			{
				text = text + "\n    • " + constraint.name;
			}
		}
		return text;
	}

	public string GetRoomEffectsString()
	{
		if (effects != null && effects.Length != 0)
		{
			string text = ROOMS.EFFECTS.HEADER;
			string[] array = effects;
			foreach (string id in array)
			{
				Effect effect = Db.Get().effects.Get(id);
				text += Effect.CreateTooltip(effect, showDuration: false, "\n    • ", showHeader: false);
			}
			return text;
		}
		return null;
	}

	public void TriggerRoomEffects(KPrefabID triggerer, Effects target)
	{
		if (primary_constraint != null && !(triggerer == null) && effects != null && primary_constraint.building_criteria(triggerer))
		{
			string[] array = effects;
			foreach (string effect_id in array)
			{
				target.Add(effect_id, should_save: true);
			}
		}
	}
}
