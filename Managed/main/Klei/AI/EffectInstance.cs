using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{effect.Id}")]
	public class EffectInstance : ModifierInstance<Effect>
	{
		public Effect effect;

		public bool shouldSave;

		public StatusItem statusItem;

		public float timeRemaining;

		public Reactable reactable;

		public EffectInstance(GameObject game_object, Effect effect, bool should_save)
			: base(game_object, effect)
		{
			this.effect = effect;
			shouldSave = should_save;
			ConfigureStatusItem();
			if (effect.showInUI)
			{
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				if (!component.GetStatusItemGroup().HasStatusItemID(statusItem))
				{
					component.AddStatusItem(statusItem, this);
				}
			}
			if (effect.triggerFloatingText && PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, effect.Name, game_object.transform);
			}
			if (string.IsNullOrEmpty(effect.emoteAnim))
			{
				return;
			}
			ReactionMonitor.Instance sMI = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (sMI == null)
			{
				return;
			}
			if (effect.emoteCooldown < 0f)
			{
				SelfEmoteReactable selfEmoteReactable = (SelfEmoteReactable)new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, 100000f).AddStep(new EmoteReactable.EmoteStep
				{
					anim = "react"
				});
				selfEmoteReactable.AddPrecondition(NotInATube);
				if (effect.emotePreconditions != null)
				{
					foreach (Reactable.ReactablePrecondition emotePrecondition in effect.emotePreconditions)
					{
						selfEmoteReactable.AddPrecondition(emotePrecondition);
					}
				}
				sMI.AddOneshotReactable(selfEmoteReactable);
				return;
			}
			reactable = new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, effect.emoteCooldown).AddStep(new EmoteReactable.EmoteStep
			{
				anim = "react"
			});
			reactable.AddPrecondition(NotInATube);
			if (effect.emotePreconditions == null)
			{
				return;
			}
			foreach (Reactable.ReactablePrecondition emotePrecondition2 in effect.emotePreconditions)
			{
				reactable.AddPrecondition(emotePrecondition2);
			}
		}

		private bool NotInATube(GameObject go, Navigator.ActiveTransition transition)
		{
			if (transition.navGridTransition.start != NavType.Tube)
			{
				return transition.navGridTransition.end != NavType.Tube;
			}
			return false;
		}

		public override void OnCleanUp()
		{
			if (statusItem != null)
			{
				base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(statusItem);
				statusItem = null;
			}
			if (reactable != null)
			{
				reactable.Cleanup();
				reactable = null;
			}
		}

		public float GetTimeRemaining()
		{
			return timeRemaining;
		}

		public bool IsExpired()
		{
			if (effect.duration > 0f)
			{
				return timeRemaining <= 0f;
			}
			return false;
		}

		private void ConfigureStatusItem()
		{
			StatusItem.IconType icon_type = (effect.isBad ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info);
			if (!effect.customIcon.IsNullOrWhiteSpace())
			{
				icon_type = StatusItem.IconType.Custom;
			}
			statusItem = new StatusItem(effect.Id, effect.Name, effect.description, effect.customIcon, icon_type, effect.isBad ? NotificationType.Bad : NotificationType.Neutral, allow_multiples: false, OverlayModes.None.ID, 2, showWorldIcon: false);
			statusItem.resolveStringCallback = ResolveString;
			statusItem.resolveTooltipCallback = ResolveTooltip;
		}

		private string ResolveString(string str, object data)
		{
			return str;
		}

		private string ResolveTooltip(string str, object data)
		{
			string text = str;
			EffectInstance obj = (EffectInstance)data;
			string text2 = Effect.CreateTooltip(obj.effect, showDuration: false);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "\n\n" + text2;
			}
			if (obj.effect.duration > 0f)
			{
				text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_REMAINING, GameUtil.GetFormattedCycles(GetTimeRemaining()));
			}
			return text;
		}
	}
}
