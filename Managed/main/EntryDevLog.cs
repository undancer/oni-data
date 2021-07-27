using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class EntryDevLog
{
	public class ModificationRecord
	{
		public enum ActionType
		{
			Created,
			ChangeSubEntry,
			ChangeContent,
			ValueChange,
			YAMLData
		}

		public ActionType actionType { get; private set; }

		public string target { get; private set; }

		public object newValue { get; private set; }

		public string author { get; private set; }

		public ModificationRecord(ActionType actionType, string target, object newValue, string author)
		{
			this.target = target;
			this.newValue = newValue;
			this.author = author;
			this.actionType = actionType;
		}
	}

	[SerializeField]
	public List<ModificationRecord> modificationRecords = new List<ModificationRecord>();

	[Conditional("UNITY_EDITOR")]
	public void AddModificationRecord(ModificationRecord.ActionType actionType, string target, object newValue)
	{
		string author = TrimAuthor();
		modificationRecords.Add(new ModificationRecord(actionType, target, newValue, author));
	}

	[Conditional("UNITY_EDITOR")]
	public void InsertModificationRecord(int index, ModificationRecord.ActionType actionType, string target, object newValue)
	{
		string author = TrimAuthor();
		modificationRecords.Insert(index, new ModificationRecord(actionType, target, newValue, author));
	}

	private string TrimAuthor()
	{
		string text = "";
		string[] array = new string[7] { "Invoke", "CreateInstance", "AwakeInternal", "Internal", "<>", "YamlDotNet", "Deserialize" };
		string[] array2 = new string[13]
		{
			".ctor", "Trigger", "AddContentContainerRange", "AddContentContainer", "InsertContentContainer", "KInstantiateUI", "Start", "InitializeComponentAwake", "TrimAuthor", "InsertModificationRecord",
			"AddModificationRecord", "SetValue", "Write"
		};
		StackTrace stackTrace = new StackTrace();
		int num = 0;
		int num2 = 0;
		int num3 = 3;
		while (num < num3)
		{
			num2++;
			if (stackTrace.FrameCount <= num2)
			{
				break;
			}
			MethodBase method = stackTrace.GetFrame(num2).GetMethod();
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				flag = flag || method.Name.Contains(array[i]);
			}
			for (int j = 0; j < array2.Length; j++)
			{
				flag = flag || method.Name.Contains(array2[j]);
			}
			if (!flag && !stackTrace.GetFrame(num2).GetMethod().Name.StartsWith("set_") && !stackTrace.GetFrame(num2).GetMethod().Name.StartsWith("Instantiate"))
			{
				if (num != 0)
				{
					text += " < ";
				}
				num++;
				text += stackTrace.GetFrame(num2).GetMethod().Name;
			}
		}
		return text;
	}
}
