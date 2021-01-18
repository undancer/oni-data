using Klei.AI;

public abstract class Need : KMonoBehaviour
{
	protected class ModifierType
	{
		public AttributeModifier modifier;

		public StatusItem statusItem;

		public Thought thought;
	}

	protected AttributeInstance expectationAttribute;

	protected ModifierType stressBonus;

	protected ModifierType stressNeutral;

	protected ModifierType stressPenalty;

	protected ModifierType currentStressModifier;

	public string Name
	{
		get;
		protected set;
	}

	public string ExpectationTooltip
	{
		get;
		protected set;
	}

	public string Tooltip
	{
		get;
		protected set;
	}

	public Attribute GetExpectationAttribute()
	{
		return expectationAttribute.Attribute;
	}

	protected void SetModifier(ModifierType modifier)
	{
		if (currentStressModifier != modifier)
		{
			if (currentStressModifier != null)
			{
				UnapplyModifier(currentStressModifier);
			}
			if (modifier != null)
			{
				ApplyModifier(modifier);
			}
			currentStressModifier = modifier;
		}
	}

	private void ApplyModifier(ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			this.GetAttributes().Add(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			GetComponent<KSelectable>().AddStatusItem(modifier.statusItem);
		}
		if (modifier.thought != null)
		{
			this.GetSMI<ThoughtGraph.Instance>().AddThought(modifier.thought);
		}
	}

	private void UnapplyModifier(ModifierType modifier)
	{
		if (modifier.modifier != null)
		{
			this.GetAttributes().Remove(modifier.modifier);
		}
		if (modifier.statusItem != null)
		{
			GetComponent<KSelectable>().RemoveStatusItem(modifier.statusItem);
		}
		if (modifier.thought != null)
		{
			this.GetSMI<ThoughtGraph.Instance>().RemoveThought(modifier.thought);
		}
	}
}
