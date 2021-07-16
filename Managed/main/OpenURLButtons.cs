using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OpenURLButtons")]
public class OpenURLButtons : KMonoBehaviour
{
	public enum URLButtonType
	{
		url,
		platformUrl,
		patchNotes,
		feedbackScreen
	}

	[Serializable]
	public class URLButtonData
	{
		public string stringKey;

		public URLButtonType urlType;

		public string url;
	}

	public GameObject buttonPrefab;

	public List<URLButtonData> buttonData;

	[SerializeField]
	private GameObject patchNotesScreenPrefab;

	[SerializeField]
	private FeedbackScreen feedbackScreenPrefab;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		for (int i = 0; i < buttonData.Count; i++)
		{
			URLButtonData data = buttonData[i];
			GameObject gameObject = Util.KInstantiateUI(buttonPrefab, base.gameObject, force_active: true);
			string text = Strings.Get(data.stringKey);
			gameObject.GetComponentInChildren<LocText>().SetText(text);
			switch (data.urlType)
			{
			case URLButtonType.patchNotes:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenPatchNotes();
				};
				break;
			case URLButtonType.url:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenURL(data.url);
				};
				break;
			case URLButtonType.platformUrl:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenPlatformURL(data.url);
				};
				break;
			case URLButtonType.feedbackScreen:
				gameObject.GetComponent<KButton>().onClick += delegate
				{
					OpenFeedbackScreen();
				};
				break;
			}
		}
	}

	public void OpenPatchNotes()
	{
		Util.KInstantiateUI(patchNotesScreenPrefab, FrontEndManager.Instance.gameObject, force_active: true);
	}

	public void OpenFeedbackScreen()
	{
		Util.KInstantiateUI(feedbackScreenPrefab.gameObject, FrontEndManager.Instance.gameObject, force_active: true);
	}

	public void OpenURL(string URL)
	{
		Application.OpenURL(URL);
	}

	public void OpenPlatformURL(string URL)
	{
		if (DistributionPlatform.Inst.Platform == "Steam" && DistributionPlatform.Inst.Initialized)
		{
			DistributionPlatform.Inst.GetAuthTicket(delegate(byte[] ticket)
			{
				string newValue = string.Concat(Array.ConvertAll(ticket, (byte x) => x.ToString("X2")));
				Application.OpenURL(URL.Replace("{SteamID}", DistributionPlatform.Inst.LocalUser.Id.ToInt64().ToString()).Replace("{SteamTicket}", newValue));
			});
		}
		else
		{
			string value = URL.Replace("{SteamID}", "").Replace("{SteamTicket}", "");
			Application.OpenURL("https://accounts.klei.com/login?goto={gotoUrl}".Replace("{gotoUrl}", WebUtility.HtmlEncode(value)));
		}
	}
}
