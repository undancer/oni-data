using System;
using STRINGS;

public class RoomDetails
{
	public class Detail
	{
		public Func<Room, string> resolve_string_function;

		public Detail(Func<Room, string> resolve_string_function)
		{
			this.resolve_string_function = resolve_string_function;
		}
	}

	public static readonly Detail AVERAGE_TEMPERATURE = new Detail(delegate
	{
		float num3 = 0f;
		return (num3 == 0f) ? string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, UI.OVERLAYS.TEMPERATURE.EXTREMECOLD) : string.Format(ROOMS.DETAILS.AVERAGE_TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num3));
	});

	public static readonly Detail AVERAGE_ATMO_MASS = new Detail(delegate
	{
		float num = 0f;
		float num2 = 0f;
		return string.Format(arg0: GameUtil.GetFormattedMass((!(num2 > 0f)) ? 0f : (num / num2)), format: ROOMS.DETAILS.AVERAGE_ATMO_MASS.NAME);
	});

	public static readonly Detail ASSIGNED_TO = new Detail(delegate(Room room)
	{
		string text = "";
		foreach (KPrefabID primaryEntity in room.GetPrimaryEntities())
		{
			if (!(primaryEntity == null))
			{
				Assignable component = primaryEntity.GetComponent<Assignable>();
				if (!(component == null))
				{
					IAssignableIdentity assignee = component.assignee;
					if (assignee == null)
					{
						text += ((text == "") ? ("<color=#BCBCBC>    • " + primaryEntity.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED) : ("\n<color=#BCBCBC>    • " + primaryEntity.GetProperName() + ": " + ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED));
						text += "</color>";
					}
					else
					{
						text += ((text == "") ? ("    • " + primaryEntity.GetProperName() + ": " + assignee.GetProperName()) : ("\n    • " + primaryEntity.GetProperName() + ": " + assignee.GetProperName()));
					}
				}
			}
		}
		if (text == "")
		{
			text = ROOMS.DETAILS.ASSIGNED_TO.UNASSIGNED;
		}
		return string.Format(ROOMS.DETAILS.ASSIGNED_TO.NAME, text);
	});

	public static readonly Detail SIZE = new Detail((Room room) => string.Format(ROOMS.DETAILS.SIZE.NAME, room.cavity.numCells));

	public static readonly Detail BUILDING_COUNT = new Detail((Room room) => string.Format(ROOMS.DETAILS.BUILDING_COUNT.NAME, room.buildings.Count));

	public static readonly Detail CREATURE_COUNT = new Detail((Room room) => string.Format(ROOMS.DETAILS.CREATURE_COUNT.NAME, room.cavity.creatures.Count + room.cavity.eggs.Count));

	public static readonly Detail PLANT_COUNT = new Detail((Room room) => string.Format(ROOMS.DETAILS.PLANT_COUNT.NAME, room.cavity.plants.Count));

	public static readonly Detail EFFECT = new Detail((Room room) => room.roomType.effect);

	public static readonly Detail EFFECTS = new Detail((Room room) => room.roomType.GetRoomEffectsString());

	public static string RoomDetailString(Room room)
	{
		string str = "";
		str = string.Concat(str, "<b>", ROOMS.DETAILS.HEADER, "</b>");
		Detail[] display_details = room.roomType.display_details;
		foreach (Detail detail in display_details)
		{
			str = str + "\n    • " + detail.resolve_string_function(room);
		}
		return str;
	}
}
