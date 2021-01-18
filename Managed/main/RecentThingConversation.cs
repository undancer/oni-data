using System.Collections.Generic;
using UnityEngine;

public class RecentThingConversation : ConversationType
{
	public static Dictionary<Conversation.ModeType, List<Conversation.ModeType>> transitions = new Dictionary<Conversation.ModeType, List<Conversation.ModeType>>
	{
		{
			Conversation.ModeType.Query,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Agreement,
				Conversation.ModeType.Disagreement,
				Conversation.ModeType.Musing
			}
		},
		{
			Conversation.ModeType.Statement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Agreement,
				Conversation.ModeType.Disagreement,
				Conversation.ModeType.Query,
				Conversation.ModeType.Segue
			}
		},
		{
			Conversation.ModeType.Agreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Satisfaction
			}
		},
		{
			Conversation.ModeType.Disagreement,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Dissatisfaction
			}
		},
		{
			Conversation.ModeType.Musing,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Query,
				Conversation.ModeType.Statement,
				Conversation.ModeType.Segue
			}
		},
		{
			Conversation.ModeType.Satisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Nominal,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		},
		{
			Conversation.ModeType.Dissatisfaction,
			new List<Conversation.ModeType>
			{
				Conversation.ModeType.Segue,
				Conversation.ModeType.End
			}
		}
	};

	public RecentThingConversation()
	{
		id = "RecentThingConversation";
	}

	public override void NewTarget(MinionIdentity speaker)
	{
		ConversationMonitor.Instance sMI = speaker.GetSMI<ConversationMonitor.Instance>();
		target = sMI.GetATopic();
	}

	public override Conversation.Topic GetNextTopic(MinionIdentity speaker, Conversation.Topic lastTopic)
	{
		if (string.IsNullOrEmpty(target))
		{
			return null;
		}
		List<Conversation.ModeType> list = ((lastTopic != null) ? transitions[lastTopic.mode] : new List<Conversation.ModeType>
		{
			Conversation.ModeType.Query,
			Conversation.ModeType.Statement,
			Conversation.ModeType.Musing
		});
		Conversation.ModeType mode = list[Random.Range(0, list.Count)];
		return new Conversation.Topic(target, mode);
	}

	public override Sprite GetSprite(string topic)
	{
		return Def.GetUISprite(topic, "ui", centered: true)?.first;
	}
}
