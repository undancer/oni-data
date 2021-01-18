using System.Collections.Generic;

public static class NotificationExtensions
{
	public static string ReduceMessages(this List<Notification> notifications, bool countNames = true)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (Notification notification in notifications)
		{
			int value = 0;
			if (!dictionary.TryGetValue(notification.NotifierName, out value))
			{
				dictionary[notification.NotifierName] = 0;
			}
			dictionary[notification.NotifierName] = value + 1;
		}
		string text = "";
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			text = ((!countNames) ? (text + "\n" + item.Key) : (text + "\n" + item.Key + "(" + item.Value + ")"));
		}
		return text;
	}
}
