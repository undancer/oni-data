using System;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RequireAttachedComponent : ProcessCondition
{
	private string typeNameString;

	private Type requiredType;

	private AttachableBuilding myAttachable;

	public Type RequiredType
	{
		get
		{
			return requiredType;
		}
		set
		{
			requiredType = value;
			typeNameString = requiredType.Name;
		}
	}

	public RequireAttachedComponent(AttachableBuilding myAttachable, Type required_type, string type_name_string)
	{
		this.myAttachable = myAttachable;
		requiredType = required_type;
		typeNameString = type_name_string;
	}

	public override Status EvaluateCondition()
	{
		if (myAttachable != null)
		{
			foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(myAttachable))
			{
				if ((bool)item.GetComponent(requiredType))
				{
					return Status.Ready;
				}
			}
		}
		return Status.Failure;
	}

	public override string GetStatusMessage(Status status)
	{
		_ = 2;
		return typeNameString;
	}

	public override string GetStatusTooltip(Status status)
	{
		if (status == Status.Ready)
		{
			return string.Format(UI.STARMAP.LAUNCHCHECKLIST.INSTALLED_TOOLTIP, typeNameString.ToLower());
		}
		return string.Format(UI.STARMAP.LAUNCHCHECKLIST.MISSING_TOOLTIP, typeNameString.ToLower());
	}

	public override bool ShowInUI()
	{
		return true;
	}
}
