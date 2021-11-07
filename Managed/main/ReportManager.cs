using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ReportManager")]
public class ReportManager : KMonoBehaviour
{
	public delegate string FormattingFn(float v);

	public delegate string GroupFormattingFn(float v, float numEntries);

	public enum ReportType
	{
		DuplicantHeader,
		CaloriesCreated,
		StressDelta,
		LevelUp,
		DiseaseStatus,
		DiseaseAdded,
		ToiletIncident,
		ChoreStatus,
		TimeSpentHeader,
		TimeSpent,
		WorkTime,
		TravelTime,
		PersonalTime,
		IdleTime,
		BaseHeader,
		ContaminatedOxygenFlatulence,
		ContaminatedOxygenToilet,
		ContaminatedOxygenSublimation,
		OxygenCreated,
		EnergyCreated,
		EnergyWasted,
		DomesticatedCritters,
		WildCritters,
		RocketsInFlight
	}

	public struct ReportGroup
	{
		public FormattingFn formatfn;

		public GroupFormattingFn groupFormatfn;

		public string stringKey;

		public string positiveTooltip;

		public string negativeTooltip;

		public bool reportIfZero;

		public int group;

		public bool isHeader;

		public ReportEntry.Order posNoteOrder;

		public ReportEntry.Order negNoteOrder;

		public ReportGroup(FormattingFn formatfn, bool reportIfZero, int group, string stringKey, string positiveTooltip, string negativeTooltip, ReportEntry.Order pos_note_order = ReportEntry.Order.Unordered, ReportEntry.Order neg_note_order = ReportEntry.Order.Unordered, bool is_header = false, GroupFormattingFn group_format_fn = null)
		{
			this.formatfn = ((formatfn != null) ? formatfn : ((FormattingFn)((float v) => v.ToString())));
			groupFormatfn = group_format_fn;
			this.stringKey = stringKey;
			this.positiveTooltip = positiveTooltip;
			this.negativeTooltip = negativeTooltip;
			this.reportIfZero = reportIfZero;
			this.group = group;
			posNoteOrder = pos_note_order;
			negNoteOrder = neg_note_order;
			isHeader = is_header;
		}
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class ReportEntry
	{
		public struct Note
		{
			public float value;

			public string note;

			public Note(float value, string note)
			{
				this.value = value;
				this.note = note;
			}
		}

		public enum Order
		{
			Unordered,
			Ascending,
			Descending
		}

		[Serialize]
		public int noteStorageId;

		[Serialize]
		public int gameHash = -1;

		[Serialize]
		public ReportType reportType;

		[Serialize]
		public string context;

		[Serialize]
		public float accumulate;

		[Serialize]
		public float accPositive;

		[Serialize]
		public float accNegative;

		[Serialize]
		public ArrayRef<ReportEntry> contextEntries;

		public bool isChild;

		public float Positive => accPositive;

		public float Negative => accNegative;

		public float Net => accPositive + accNegative;

		public ReportEntry(ReportType reportType, int note_storage_id, string context, bool is_child = false)
		{
			this.reportType = reportType;
			this.context = context;
			isChild = is_child;
			accumulate = 0f;
			accPositive = 0f;
			accNegative = 0f;
			noteStorageId = note_storage_id;
		}

		[OnDeserializing]
		private void OnDeserialize()
		{
			contextEntries.Clear();
		}

		public void IterateNotes(Action<Note> callback)
		{
			Instance.noteStorage.IterateNotes(noteStorageId, callback);
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			if (gameHash != -1)
			{
				reportType = (ReportType)gameHash;
				gameHash = -1;
			}
		}

		public void AddData(NoteStorage note_storage, float value, string note = null, string dataContext = null)
		{
			AddActualData(note_storage, value, note);
			if (dataContext == null)
			{
				return;
			}
			ReportEntry reportEntry = null;
			for (int i = 0; i < contextEntries.Count; i++)
			{
				if (contextEntries[i].context == dataContext)
				{
					reportEntry = contextEntries[i];
					break;
				}
			}
			if (reportEntry == null)
			{
				reportEntry = new ReportEntry(reportType, note_storage.GetNewNoteId(), dataContext, is_child: true);
				contextEntries.Add(reportEntry);
			}
			reportEntry.AddActualData(note_storage, value, note);
		}

		private void AddActualData(NoteStorage note_storage, float value, string note = null)
		{
			accumulate += value;
			if (value > 0f)
			{
				accPositive += value;
			}
			else
			{
				accNegative += value;
			}
			if (note != null)
			{
				note_storage.Add(noteStorageId, value, note);
			}
		}

		public bool HasContextEntries()
		{
			return contextEntries.Count > 0;
		}
	}

	public class DailyReport
	{
		[Serialize]
		public int day;

		[Serialize]
		public List<ReportEntry> reportEntries = new List<ReportEntry>();

		private NoteStorage noteStorage => Instance.noteStorage;

		public DailyReport(ReportManager manager)
		{
			foreach (KeyValuePair<ReportType, ReportGroup> reportGroup in manager.ReportGroups)
			{
				reportEntries.Add(new ReportEntry(reportGroup.Key, noteStorage.GetNewNoteId(), null));
			}
		}

		public ReportEntry GetEntry(ReportType reportType)
		{
			for (int i = 0; i < reportEntries.Count; i++)
			{
				ReportEntry reportEntry = reportEntries[i];
				if (reportEntry.reportType == reportType)
				{
					return reportEntry;
				}
			}
			ReportEntry reportEntry2 = new ReportEntry(reportType, noteStorage.GetNewNoteId(), null);
			reportEntries.Add(reportEntry2);
			return reportEntry2;
		}

		public void AddData(ReportType reportType, float value, string note = null, string context = null)
		{
			GetEntry(reportType).AddData(noteStorage, value, note, context);
		}
	}

	public class NoteStorage
	{
		private class StringTable
		{
			private Dictionary<int, string> strings = new Dictionary<int, string>();

			public int AddString(string str, int version = 6)
			{
				int num = Hash.SDBMLower(str);
				strings[num] = str;
				return num;
			}

			public string GetStringByHash(int hash)
			{
				string value = "";
				strings.TryGetValue(hash, out value);
				return value;
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(strings.Count);
				foreach (KeyValuePair<int, string> @string in strings)
				{
					writer.Write(@string.Value);
				}
			}

			public void Deserialize(BinaryReader reader, int version)
			{
				int num = reader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					string str = reader.ReadString();
					AddString(str, version);
				}
			}
		}

		private class NoteEntries
		{
			public struct NoteEntryKey
			{
				public int noteHash;

				public bool isPositive;
			}

			public class NoteEntryKeyComparer : IEqualityComparer<NoteEntryKey>
			{
				public bool Equals(NoteEntryKey a, NoteEntryKey b)
				{
					if (a.noteHash == b.noteHash)
					{
						return a.isPositive == b.isPositive;
					}
					return false;
				}

				public int GetHashCode(NoteEntryKey a)
				{
					return a.noteHash * (a.isPositive ? 1 : (-1));
				}
			}

			private static NoteEntryKeyComparer sKeyComparer = new NoteEntryKeyComparer();

			private Dictionary<int, Dictionary<NoteEntryKey, float>> entries = new Dictionary<int, Dictionary<NoteEntryKey, float>>();

			public void Add(int report_entry_id, float value, int note_id)
			{
				if (!entries.TryGetValue(report_entry_id, out var value2))
				{
					value2 = new Dictionary<NoteEntryKey, float>(sKeyComparer);
					entries[report_entry_id] = value2;
				}
				NoteEntryKey noteEntryKey = default(NoteEntryKey);
				noteEntryKey.noteHash = note_id;
				noteEntryKey.isPositive = value > 0f;
				NoteEntryKey noteEntryKey2 = noteEntryKey;
				if (value2.ContainsKey(noteEntryKey2))
				{
					Dictionary<NoteEntryKey, float> dictionary = value2;
					noteEntryKey = noteEntryKey2;
					dictionary[noteEntryKey] += value;
				}
				else
				{
					value2[noteEntryKey2] = value;
				}
			}

			public void Serialize(BinaryWriter writer)
			{
				writer.Write(entries.Count);
				foreach (KeyValuePair<int, Dictionary<NoteEntryKey, float>> entry in entries)
				{
					writer.Write(entry.Key);
					writer.Write(entry.Value.Count);
					foreach (KeyValuePair<NoteEntryKey, float> item in entry.Value)
					{
						writer.Write(item.Key.noteHash);
						writer.Write(item.Key.isPositive);
						writer.WriteSingleFast(item.Value);
					}
				}
			}

			public void Deserialize(BinaryReader reader, int version)
			{
				if (version < 6)
				{
					OldNoteEntriesV5 oldNoteEntriesV = new OldNoteEntriesV5();
					oldNoteEntriesV.Deserialize(reader);
					foreach (OldNoteEntriesV5.NoteStorageBlock storageBlock in oldNoteEntriesV.storageBlocks)
					{
						for (int i = 0; i < storageBlock.entryCount; i++)
						{
							OldNoteEntriesV5.NoteEntry noteEntry = storageBlock.entries.structs[i];
							Add(noteEntry.reportEntryId, noteEntry.value, noteEntry.noteHash);
						}
					}
					return;
				}
				int num = reader.ReadInt32();
				entries = new Dictionary<int, Dictionary<NoteEntryKey, float>>(num);
				for (int j = 0; j < num; j++)
				{
					int key = reader.ReadInt32();
					int num2 = reader.ReadInt32();
					Dictionary<NoteEntryKey, float> dictionary = new Dictionary<NoteEntryKey, float>(num2, sKeyComparer);
					entries[key] = dictionary;
					for (int k = 0; k < num2; k++)
					{
						NoteEntryKey noteEntryKey = default(NoteEntryKey);
						noteEntryKey.noteHash = reader.ReadInt32();
						noteEntryKey.isPositive = reader.ReadBoolean();
						NoteEntryKey key2 = noteEntryKey;
						dictionary[key2] = reader.ReadSingle();
					}
				}
			}

			public void IterateNotes(StringTable string_table, int report_entry_id, Action<ReportEntry.Note> callback)
			{
				if (!entries.TryGetValue(report_entry_id, out var value))
				{
					return;
				}
				foreach (KeyValuePair<NoteEntryKey, float> item in value)
				{
					string stringByHash = string_table.GetStringByHash(item.Key.noteHash);
					ReportEntry.Note obj = new ReportEntry.Note(item.Value, stringByHash);
					callback(obj);
				}
			}
		}

		public const int SERIALIZATION_VERSION = 6;

		private int nextNoteId;

		private NoteEntries noteEntries;

		private StringTable stringTable;

		public NoteStorage()
		{
			noteEntries = new NoteEntries();
			stringTable = new StringTable();
		}

		public void Add(int report_entry_id, float value, string note)
		{
			int note_id = stringTable.AddString(note);
			noteEntries.Add(report_entry_id, value, note_id);
		}

		public int GetNewNoteId()
		{
			return ++nextNoteId;
		}

		public void IterateNotes(int report_entry_id, Action<ReportEntry.Note> callback)
		{
			noteEntries.IterateNotes(stringTable, report_entry_id, callback);
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(6);
			writer.Write(nextNoteId);
			stringTable.Serialize(writer);
			noteEntries.Serialize(writer);
		}

		public void Deserialize(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num >= 5)
			{
				nextNoteId = reader.ReadInt32();
				stringTable.Deserialize(reader, num);
				noteEntries.Deserialize(reader, num);
			}
		}
	}

	[MyCmpAdd]
	private Notifier notifier;

	private NoteStorage noteStorage;

	public Dictionary<ReportType, ReportGroup> ReportGroups = new Dictionary<ReportType, ReportGroup>
	{
		{
			ReportType.DuplicantHeader,
			new ReportGroup(null, reportIfZero: true, 1, UI.ENDOFDAYREPORT.DUPLICANT_DETAILS_HEADER, "", "", ReportEntry.Order.Unordered, ReportEntry.Order.Unordered, is_header: true)
		},
		{
			ReportType.CaloriesCreated,
			new ReportGroup((float v) => GameUtil.GetFormattedCalories(v), reportIfZero: true, 1, UI.ENDOFDAYREPORT.CALORIES_CREATED.NAME, UI.ENDOFDAYREPORT.CALORIES_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CALORIES_CREATED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.StressDelta,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v), reportIfZero: true, 1, UI.ENDOFDAYREPORT.STRESS_DELTA.NAME, UI.ENDOFDAYREPORT.STRESS_DELTA.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.STRESS_DELTA.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.DiseaseAdded,
			new ReportGroup(null, reportIfZero: false, 1, UI.ENDOFDAYREPORT.DISEASE_ADDED.NAME, UI.ENDOFDAYREPORT.DISEASE_ADDED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_ADDED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.DiseaseStatus,
			new ReportGroup((float v) => GameUtil.GetFormattedDiseaseAmount((int)v), reportIfZero: true, 1, UI.ENDOFDAYREPORT.DISEASE_STATUS.NAME, UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, UI.ENDOFDAYREPORT.DISEASE_STATUS.TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.LevelUp,
			new ReportGroup(null, reportIfZero: false, 1, UI.ENDOFDAYREPORT.LEVEL_UP.NAME, UI.ENDOFDAYREPORT.LEVEL_UP.TOOLTIP, UI.ENDOFDAYREPORT.NONE, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ToiletIncident,
			new ReportGroup(null, reportIfZero: false, 1, UI.ENDOFDAYREPORT.TOILET_INCIDENT.NAME, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, UI.ENDOFDAYREPORT.TOILET_INCIDENT.TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ChoreStatus,
			new ReportGroup(null, reportIfZero: true, 1, UI.ENDOFDAYREPORT.CHORE_STATUS.NAME, UI.ENDOFDAYREPORT.CHORE_STATUS.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CHORE_STATUS.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.DomesticatedCritters,
			new ReportGroup(null, reportIfZero: true, 1, UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.NAME, UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NUMBER_OF_DOMESTICATED_CRITTERS.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.WildCritters,
			new ReportGroup(null, reportIfZero: true, 1, UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.NAME, UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NUMBER_OF_WILD_CRITTERS.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.RocketsInFlight,
			new ReportGroup(null, reportIfZero: true, 1, UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.NAME, UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ROCKETS_IN_FLIGHT.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.TimeSpentHeader,
			new ReportGroup(null, reportIfZero: true, 2, UI.ENDOFDAYREPORT.TIME_DETAILS_HEADER, "", "", ReportEntry.Order.Unordered, ReportEntry.Order.Unordered, is_header: true)
		},
		{
			ReportType.WorkTime,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v / 600f * 100f), reportIfZero: true, 2, UI.ENDOFDAYREPORT.WORK_TIME.NAME, UI.ENDOFDAYREPORT.WORK_TIME.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NONE, ReportEntry.Order.Descending, ReportEntry.Order.Descending, is_header: false, (float v, float num_entries) => GameUtil.GetFormattedPercent(v / 600f * 100f / num_entries))
		},
		{
			ReportType.TravelTime,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v / 600f * 100f), reportIfZero: true, 2, UI.ENDOFDAYREPORT.TRAVEL_TIME.NAME, UI.ENDOFDAYREPORT.TRAVEL_TIME.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NONE, ReportEntry.Order.Descending, ReportEntry.Order.Descending, is_header: false, (float v, float num_entries) => GameUtil.GetFormattedPercent(v / 600f * 100f / num_entries))
		},
		{
			ReportType.PersonalTime,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v / 600f * 100f), reportIfZero: true, 2, UI.ENDOFDAYREPORT.PERSONAL_TIME.NAME, UI.ENDOFDAYREPORT.PERSONAL_TIME.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NONE, ReportEntry.Order.Descending, ReportEntry.Order.Descending, is_header: false, (float v, float num_entries) => GameUtil.GetFormattedPercent(v / 600f * 100f / num_entries))
		},
		{
			ReportType.IdleTime,
			new ReportGroup((float v) => GameUtil.GetFormattedPercent(v / 600f * 100f), reportIfZero: true, 2, UI.ENDOFDAYREPORT.IDLE_TIME.NAME, UI.ENDOFDAYREPORT.IDLE_TIME.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.NONE, ReportEntry.Order.Descending, ReportEntry.Order.Descending, is_header: false, (float v, float num_entries) => GameUtil.GetFormattedPercent(v / 600f * 100f / num_entries))
		},
		{
			ReportType.BaseHeader,
			new ReportGroup(null, reportIfZero: true, 3, UI.ENDOFDAYREPORT.BASE_DETAILS_HEADER, "", "", ReportEntry.Order.Unordered, ReportEntry.Order.Unordered, is_header: true)
		},
		{
			ReportType.OxygenCreated,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v), reportIfZero: true, 3, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NAME, UI.ENDOFDAYREPORT.OXYGEN_CREATED.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.OXYGEN_CREATED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.EnergyCreated,
			new ReportGroup(GameUtil.GetFormattedRoundedJoules, reportIfZero: true, 3, UI.ENDOFDAYREPORT.ENERGY_USAGE.NAME, UI.ENDOFDAYREPORT.ENERGY_USAGE.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.ENERGY_USAGE.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.EnergyWasted,
			new ReportGroup(GameUtil.GetFormattedRoundedJoules, reportIfZero: true, 3, UI.ENDOFDAYREPORT.ENERGY_WASTED.NAME, UI.ENDOFDAYREPORT.NONE, UI.ENDOFDAYREPORT.ENERGY_WASTED.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ContaminatedOxygenToilet,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v), reportIfZero: false, 3, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_TOILET.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		},
		{
			ReportType.ContaminatedOxygenSublimation,
			new ReportGroup((float v) => GameUtil.GetFormattedMass(v), reportIfZero: false, 3, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NAME, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.POSITIVE_TOOLTIP, UI.ENDOFDAYREPORT.CONTAMINATED_OXYGEN_SUBLIMATION.NEGATIVE_TOOLTIP, ReportEntry.Order.Descending, ReportEntry.Order.Descending)
		}
	};

	[Serialize]
	private List<DailyReport> dailyReports = new List<DailyReport>();

	[Serialize]
	private DailyReport todaysReport;

	[Serialize]
	private byte[] noteStorageBytes;

	public List<DailyReport> reports => dailyReports;

	public static ReportManager Instance { get; private set; }

	public DailyReport TodaysReport => todaysReport;

	public DailyReport YesterdaysReport
	{
		get
		{
			if (dailyReports.Count <= 1)
			{
				return null;
			}
			return dailyReports[dailyReports.Count - 1];
		}
	}

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
		Subscribe(Game.Instance.gameObject, -1917495436, OnSaveGameReady);
		noteStorage = new NoteStorage();
	}

	protected override void OnCleanUp()
	{
		Instance = null;
	}

	[CustomSerialize]
	private void CustomSerialize(BinaryWriter writer)
	{
		writer.Write(0);
		noteStorage.Serialize(writer);
	}

	[CustomDeserialize]
	private void CustomDeserialize(IReader reader)
	{
		if (noteStorageBytes == null)
		{
			Debug.Assert(reader.ReadInt32() == 0);
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(reader.RawBytes()));
			binaryReader.BaseStream.Position = reader.Position;
			noteStorage.Deserialize(binaryReader);
			reader.SkipBytes((int)binaryReader.BaseStream.Position - reader.Position);
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (noteStorageBytes != null)
		{
			noteStorage.Deserialize(new BinaryReader(new MemoryStream(noteStorageBytes)));
			noteStorageBytes = null;
		}
	}

	private void OnSaveGameReady(object data)
	{
		Subscribe(GameClock.Instance.gameObject, -722330267, OnNightTime);
		if (todaysReport == null)
		{
			todaysReport = new DailyReport(this);
			todaysReport.day = GameUtil.GetCurrentCycle();
		}
	}

	public void ReportValue(ReportType reportType, float value, string note = null, string context = null)
	{
		TodaysReport.AddData(reportType, value, note, context);
	}

	private void OnNightTime(object data)
	{
		dailyReports.Add(todaysReport);
		int day = todaysReport.day;
		ManagementMenuNotification notification = new ManagementMenuNotification(Action.ManageReport, NotificationValence.Good, null, string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TITLE, day), NotificationType.Good, (List<Notification> n, object d) => string.Format(UI.ENDOFDAYREPORT.NOTIFICATION_TOOLTIP, day), null, expires: true, 0f, delegate
		{
			ManagementMenu.Instance.OpenReports(day);
		});
		if (notifier == null)
		{
			Debug.LogError("Cant notify, null notifier");
		}
		else
		{
			notifier.Add(notification);
		}
		todaysReport = new DailyReport(this);
		todaysReport.day = GameUtil.GetCurrentCycle() + 1;
	}

	public DailyReport FindReport(int day)
	{
		foreach (DailyReport dailyReport in dailyReports)
		{
			if (dailyReport.day == day)
			{
				return dailyReport;
			}
		}
		if (todaysReport.day == day)
		{
			return todaysReport;
		}
		return null;
	}
}
