using System;
using System.Collections.Generic;
using Klei.AI;

public class Personality : Resource
{
	public class StartingAttribute
	{
		public Klei.AI.Attribute attribute;

		public int value;

		public StartingAttribute(Klei.AI.Attribute attribute, int value)
		{
			this.attribute = attribute;
			this.value = value;
		}
	}

	public List<StartingAttribute> attributes = new List<StartingAttribute>();

	public List<Trait> traits = new List<Trait>();

	public int headShape;

	public int mouth;

	public int neck;

	public int eyes;

	public int hair;

	public int body;

	public string nameStringKey;

	public string genderStringKey;

	public string personalityType;

	public string stresstrait;

	public string joyTrait;

	public string stickerType;

	public string congenitaltrait;

	public string unformattedDescription;

	public bool startingMinion;

	public string description => GetDescription();

	[Obsolete("Modders: Use constructor with isStartingMinion parameter")]
	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description)
		: this(name_string_key, name, Gender, PersonalityType, StressTrait, JoyTrait, StickerType, CongenitalTrait, headShape, mouth, neck, eyes, hair, body, description, isStartingMinion: true)
	{
	}

	public Personality(string name_string_key, string name, string Gender, string PersonalityType, string StressTrait, string JoyTrait, string StickerType, string CongenitalTrait, int headShape, int mouth, int neck, int eyes, int hair, int body, string description, bool isStartingMinion)
		: base(name, name)
	{
		nameStringKey = name_string_key;
		genderStringKey = Gender;
		personalityType = PersonalityType;
		stresstrait = StressTrait;
		joyTrait = JoyTrait;
		stickerType = StickerType;
		congenitaltrait = CongenitalTrait;
		unformattedDescription = description;
		this.headShape = headShape;
		this.mouth = mouth;
		this.neck = neck;
		this.eyes = eyes;
		this.hair = hair;
		this.body = body;
		startingMinion = isStartingMinion;
	}

	public string GetDescription()
	{
		unformattedDescription = unformattedDescription.Replace("{0}", Name);
		return unformattedDescription;
	}

	public void SetAttribute(Klei.AI.Attribute attribute, int value)
	{
		StartingAttribute item = new StartingAttribute(attribute, value);
		attributes.Add(item);
	}

	public void AddTrait(Trait trait)
	{
		traits.Add(trait);
	}
}
