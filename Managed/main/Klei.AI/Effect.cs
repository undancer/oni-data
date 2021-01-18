using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Effect : Modifier
	{
		public float duration;

		public bool showInUI;

		public bool triggerFloatingText;

		public bool isBad;

		public string customIcon;

		public string emoteAnim;

		public float emoteCooldown;

		public List<Reactable.ReactablePrecondition> emotePreconditions;

		public string stompGroup;

		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad, string emote_anim = null, float emote_cooldown = 0f, string stompGroup = null, string custom_icon = "")
			: base(id, name, description)
		{
			this.duration = duration;
			showInUI = show_in_ui;
			triggerFloatingText = trigger_floating_text;
			isBad = is_bad;
			emoteAnim = emote_anim;
			emoteCooldown = emote_cooldown;
			this.stompGroup = stompGroup;
			customIcon = custom_icon;
		}

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
		}

		public void AddEmotePrecondition(Reactable.ReactablePrecondition precon)
		{
			if (emotePreconditions == null)
			{
				emotePreconditions = new List<Reactable.ReactablePrecondition>();
			}
			emotePreconditions.Add(precon);
		}

		public static string CreateTooltip(Effect effect, bool showDuration, string linePrefix = "\n    • ", bool showHeader = true)
		{
			string text = (showHeader ? DUPLICANTS.MODIFIERS.EFFECT_HEADER.text : "");
			foreach (AttributeModifier selfModifier in effect.SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.TryGet(selfModifier.AttributeId);
				if (attribute == null)
				{
					attribute = Db.Get().CritterAttributes.TryGet(selfModifier.AttributeId);
				}
				if (attribute != null && attribute.ShowInUI != Attribute.Display.Never)
				{
					text = text + linePrefix + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, attribute.Name, selfModifier.GetFormattedString(null));
				}
			}
			if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".ADDITIONAL_EFFECTS", out var result))
			{
				text = text + linePrefix + result;
			}
			if (showDuration && effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_TOTAL, GameUtil.GetFormattedCycles(effect.duration));
			}
			return text;
		}

		public static string CreateFullTooltip(Effect effect, bool showDuration)
		{
			return effect.Name + "\n\n" + effect.description + "\n\n" + CreateTooltip(effect, showDuration);
		}

		public static void AddModifierDescriptions(GameObject parent, List<Descriptor> descs, string effect_id, bool increase_indent = false)
		{
			Effect effect = Db.Get().effects.Get(effect_id);
			foreach (AttributeModifier selfModifier in effect.SelfModifiers)
			{
				Descriptor item = new Descriptor(string.Concat(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME"), ": ", selfModifier.GetFormattedString(parent)), "");
				if (increase_indent)
				{
					item.IncreaseIndent();
				}
				descs.Add(item);
			}
		}
	}
}
